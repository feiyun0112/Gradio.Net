using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Channels;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The queue as a client uses it: join, read the stream, ask what the queue is doing, cancel.
/// </summary>
/// <remarks>
/// The tests go through the same endpoints a browser does rather than reaching into the queue,
/// because what is being asserted is what a client sees (the testing decisions in the spec).
/// </remarks>
internal sealed class QueueClient : IDisposable
{
    private readonly HttpClient _client;

    /// <summary>Creates a client for one host.</summary>
    /// <param name="baseAddress">The address the host listens on.</param>
    /// <param name="cookies">
    /// The jar the requests carry, or <see langword="null"/> for a client that keeps its own — which
    /// is what a test shares with the client that signed in, so that a queued run is made by a
    /// browser the application knows.
    /// </param>
    internal QueueClient(Uri baseAddress, CookieContainer? cookies = null)
    {
        // A stream stays open for as long as the test wants it to, so the client's own timeout
        // would end it rather than the test deciding when. Each read carries its own deadline.
        _client = new HttpClient(new HttpClientHandler { CookieContainer = cookies ?? new CookieContainer() })
        {
            BaseAddress = baseAddress,
            Timeout = Timeout.InfiniteTimeSpan,
        };
    }

    /// <summary>The binding id an endpoint is served under, as the config names it.</summary>
    /// <param name="apiName">The endpoint's name.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    /// <returns>The id a join names.</returns>
    internal async Task<int> EndpointAsync(string apiName, CancellationToken cancellationToken)
    {
        var config = await GetJsonAsync("/config", cancellationToken);
        var dependency = config["dependencies"]!.AsArray()
            .FirstOrDefault(entry => entry?["api_name"]?.GetValue<string>() == apiName)
            ?? throw new InvalidOperationException($"The application serves no endpoint named '{apiName}'.");

        return dependency["id"]!.GetValue<int>();
    }

    /// <summary>Asks for a run, and returns the id the queue gave it.</summary>
    /// <param name="fnIndex">Which binding to run.</param>
    /// <param name="data">The values the binding's inputs are read from.</param>
    /// <param name="sessionHash">The session the run belongs to.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    /// <param name="eventData">
    /// What the event says about the thing that raised it, for a handler that reads one of the
    /// event-data classes, or <see langword="null"/> for an event that says nothing.
    /// </param>
    /// <returns>The event id.</returns>
    internal async Task<string> JoinAsync(
        int fnIndex,
        JsonArray data,
        string sessionHash,
        CancellationToken cancellationToken,
        JsonNode? eventData = null)
    {
        var body = new JsonObject
        {
            ["data"] = data,
            ["fn_index"] = fnIndex,
            ["session_hash"] = sessionHash,
        };

        if (eventData is not null)
        {
            body["event_data"] = eventData.DeepClone();
        }

        using var content = new StringContent(body.ToJsonString(), Encoding.UTF8, "application/json");
        using var response = await _client.PostAsync("/gradio_api/queue/join", content, cancellationToken);

        var answer = await ReadJsonAsync(response, cancellationToken);

        return answer["event_id"]?.GetValue<string>()
            ?? throw new InvalidOperationException($"The queue refused the run: {answer.ToJsonString()}");
    }

    /// <summary>Opens the stream that describes a session's runs.</summary>
    /// <param name="sessionHash">The session.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    /// <returns>The stream, positioned at its first message.</returns>
    internal async Task<QueueSession> OpenAsync(string sessionHash, CancellationToken cancellationToken)
    {
        var path = $"/gradio_api/queue/data?session_hash={Uri.EscapeDataString(sessionHash)}";

        return await QueueSession.OpenAsync(_client, path, cancellationToken);
    }

    /// <summary>Stops a run.</summary>
    /// <param name="sessionHash">The session the run belongs to.</param>
    /// <param name="eventId">The run.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    /// <returns>The endpoint's answer.</returns>
    internal async Task<JsonObject> CancelAsync(
        string sessionHash,
        string eventId,
        CancellationToken cancellationToken)
    {
        var body = new JsonObject
        {
            ["session_hash"] = sessionHash,
            ["event_id"] = eventId,
        };

        using var content = new StringContent(body.ToJsonString(), Encoding.UTF8, "application/json");
        using var response = await _client.PostAsync("/gradio_api/cancel", content, cancellationToken);

        return await ReadJsonAsync(response, cancellationToken);
    }

    /// <summary>What the whole queue is doing.</summary>
    /// <param name="cancellationToken">Cancels the request.</param>
    /// <returns>The status message.</returns>
    internal async Task<JsonObject> StatusAsync(CancellationToken cancellationToken) =>
        await GetJsonAsync("/gradio_api/queue/status", cancellationToken);

    /// <summary>Opens a heartbeat connection and drops it, which is how a client leaves.</summary>
    /// <param name="sessionHash">The session.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    /// <returns>A task that completes once the connection is open.</returns>
    internal async Task AbandonHeartbeatAsync(string sessionHash, CancellationToken cancellationToken)
    {
        var path = $"/gradio_api/heartbeat/{Uri.EscapeDataString(sessionHash)}";

        using var response = await _client.GetAsync(
            path,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    public void Dispose() => _client.Dispose();

    private async Task<JsonObject> GetJsonAsync(string path, CancellationToken cancellationToken)
    {
        using var response = await _client.GetAsync(path, cancellationToken);

        return await ReadJsonAsync(response, cancellationToken);
    }

    private static async Task<JsonObject> ReadJsonAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        response.EnsureSuccessStatusCode();

        return JsonNode.Parse(body)!.AsObject();
    }
}

/// <summary>
/// One session's stream, read message by message.
/// </summary>
/// <remarks>
/// <para>
/// The stream is read by a task of its own and handed over through a channel, rather than being
/// read on demand. The reason is the tests that assert something did <em>not</em> arrive within a
/// window — a second run starting while the first still holds its slot: bounding a read on the
/// socket itself with a cancellation token tears the connection down, so the next read fails and the
/// test cannot go on to assert what happened after the window. Cancelling a channel read costs
/// nothing.
/// </para>
/// <para>
/// Every message the stream carried is kept, whether or not the test asked for it, because "one
/// completion, not two" is an assertion about what arrived rather than about what was read.
/// </para>
/// </remarks>
internal sealed class QueueSession : IAsyncDisposable
{
    private readonly HttpResponseMessage _response;
    private readonly StreamReader _reader;
    private readonly Channel<JsonObject> _incoming = Channel.CreateUnbounded<JsonObject>();
    private readonly CancellationTokenSource _stopping = new();
    private readonly List<JsonObject> _seen = [];
    private readonly Task _pump;

    private QueueSession(HttpResponseMessage response, StreamReader reader)
    {
        _response = response;
        _reader = reader;
        _pump = Task.Run(() => PumpAsync(_stopping.Token));
    }

    /// <summary>Every message the stream carried so far, in the order they arrived.</summary>
    internal IReadOnlyList<JsonObject> Messages
    {
        get
        {
            lock (_seen)
            {
                return [.. _seen];
            }
        }
    }

    /// <summary>Opens a stream.</summary>
    /// <param name="client">The client to open it with.</param>
    /// <param name="path">The stream's path, session included.</param>
    /// <param name="cancellationToken">Cancels the open.</param>
    /// <returns>The stream, positioned at its first message.</returns>
    internal static async Task<QueueSession> OpenAsync(
        HttpClient client,
        string path,
        CancellationToken cancellationToken)
    {
        var response = await client.GetAsync(path, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        return new QueueSession(response, new StreamReader(stream));
    }

    /// <summary>The next message, or <see langword="null"/> when none arrived in time or the stream ended.</summary>
    /// <param name="within">How long to wait.</param>
    /// <param name="cancellationToken">Cancels the read.</param>
    /// <returns>The message, or <see langword="null"/>.</returns>
    internal async Task<JsonObject?> NextAsync(TimeSpan within, CancellationToken cancellationToken)
    {
        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        deadline.CancelAfter(within);

        try
        {
            if (await _incoming.Reader.WaitToReadAsync(deadline.Token)
                && _incoming.Reader.TryRead(out var message))
            {
                return message;
            }
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            // Nothing arrived in the window, which for these tests is an answer rather than a
            // failure: several of them assert that a message did not arrive.
        }

        return null;
    }

    /// <summary>Reads until a message matches, or the window passes.</summary>
    /// <param name="matches">What to look for.</param>
    /// <param name="within">How long to keep reading.</param>
    /// <param name="cancellationToken">Cancels the read.</param>
    /// <returns>The message that matched, or <see langword="null"/>.</returns>
    /// <remarks>
    /// The window is the whole read rather than each message, so a stream that keeps producing
    /// messages the caller is not looking for — an estimate, a heartbeat — still ends when the
    /// caller said it should.
    /// </remarks>
    internal async Task<JsonObject?> UntilAsync(
        Func<JsonObject, bool> matches,
        TimeSpan within,
        CancellationToken cancellationToken)
    {
        var deadline = System.DateTime.UtcNow + within;

        while (true)
        {
            var remaining = deadline - System.DateTime.UtcNow;
            if (remaining <= TimeSpan.Zero)
            {
                return null;
            }

            var message = await NextAsync(remaining, cancellationToken);
            if (message is null)
            {
                return null;
            }

            if (matches(message))
            {
                return message;
            }
        }
    }

    /// <summary>Reads until the stream says it is over.</summary>
    /// <param name="within">How long to keep reading.</param>
    /// <param name="cancellationToken">Cancels the read.</param>
    /// <returns>Whether the stream ended within the window.</returns>
    internal async Task<bool> UntilClosedAsync(TimeSpan within, CancellationToken cancellationToken) =>
        await UntilAsync(IsClose, within, cancellationToken) is not null;

    /// <summary>Whether a message is the one that closes the stream.</summary>
    /// <param name="message">The message.</param>
    /// <returns>Whether it closes the stream.</returns>
    internal static bool IsClose(JsonObject message) => Kind(message) == "close_stream";

    /// <summary>A message's kind, which is what the protocol dispatches on.</summary>
    /// <param name="message">The message.</param>
    /// <returns>The <c>msg</c> field.</returns>
    internal static string Kind(JsonObject message) => message["msg"]?.GetValue<string>() ?? string.Empty;

    /// <summary>Whether a message is of a given kind, and about a given run.</summary>
    /// <param name="message">The message.</param>
    /// <param name="kind">The <c>msg</c> field it has to have.</param>
    /// <param name="eventId">The run it has to belong to, or <see langword="null"/> for any run.</param>
    /// <returns>Whether it is that message.</returns>
    /// <remarks>
    /// A session's stream carries the messages of every run in that session, so "the run started"
    /// is only answerable together with which run: a test that asked for the kind alone would accept
    /// the second run's start as the first's.
    /// </remarks>
    internal static bool Is(JsonObject message, string kind, string? eventId = null) =>
        Kind(message) == kind
        && (eventId is null || message["event_id"]?.GetValue<string>() == eventId);

    public async ValueTask DisposeAsync()
    {
        await _stopping.CancelAsync();
        await _pump;

        _stopping.Dispose();
        _reader.Dispose();
        _response.Dispose();
    }

    /// <summary>Reads the stream into the channel for as long as it carries anything.</summary>
    /// <param name="cancellationToken">Cancels the read, which is how disposal ends it.</param>
    /// <returns>A task that completes when the stream is over.</returns>
    /// <remarks>
    /// A read that fails is the stream ending, whichever way it ended: the server closed it, the
    /// client stopped reading, or the connection broke. None of those is a test failure by itself —
    /// what the test asserts is the messages that did arrive.
    /// </remarks>
    private async Task PumpAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (await _reader.ReadLineAsync(cancellationToken) is { } line)
            {
                if (!line.StartsWith("data: ", StringComparison.Ordinal))
                {
                    continue;
                }

                var message = JsonNode.Parse(line["data: ".Length..])!.AsObject();

                lock (_seen)
                {
                    _seen.Add(message);
                }

                await _incoming.Writer.WriteAsync(message, cancellationToken);

                if (IsClose(message))
                {
                    return;
                }
            }
        }
        catch (Exception)
        {
            // The stream is over.
        }
        finally
        {
            _incoming.Writer.TryComplete();
        }
    }
}

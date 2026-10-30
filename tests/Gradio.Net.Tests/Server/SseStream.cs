using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Channels;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// An event stream read frame by frame, names included.
/// </summary>
/// <remarks>
/// <para>
/// The queue's stream is read by <see cref="QueueSession"/>, which drops the event name because the
/// queue's protocol dispatches on a field inside the data. The reload stream is the other way
/// round: the name <em>is</em> the message — <c>reload</c>, <c>error</c>, <c>heartbeat</c> — so what
/// a test has to see is the pair, not just the payload.
/// </para>
/// <para>
/// The reading happens on a task of its own and arrives through a channel for the reason
/// <see cref="QueueSession"/> gives: a read that is bounded on the socket tears the connection down
/// when the bound expires, which would end the very stream a test is asserting something did not
/// arrive on.
/// </para>
/// </remarks>
internal sealed class SseStream : IAsyncDisposable
{
    private readonly HttpClient _client;
    private readonly HttpResponseMessage _response;
    private readonly StreamReader _reader;
    private readonly Channel<Frame> _incoming = Channel.CreateUnbounded<Frame>();
    private readonly CancellationTokenSource _stopping = new();
    private readonly Task _pump;

    private SseStream(HttpClient client, HttpResponseMessage response, StreamReader reader)
    {
        _client = client;
        _response = response;
        _reader = reader;
        _pump = Task.Run(() => PumpAsync(_stopping.Token));
    }

    /// <summary>Opens a stream.</summary>
    /// <param name="baseAddress">The address the host listens on.</param>
    /// <param name="path">The stream's path.</param>
    /// <param name="cancellationToken">Cancels the open.</param>
    /// <returns>The stream, positioned at its first frame.</returns>
    /// <remarks>
    /// The client's own timeout is switched off: these streams stay open until the test closes
    /// them, and a client that gave up after a hundred seconds would end a stream the test meant to
    /// keep.
    /// </remarks>
    internal static async Task<SseStream> OpenAsync(
        Uri baseAddress,
        string path,
        CancellationToken cancellationToken)
    {
        var client = new HttpClient { BaseAddress = baseAddress, Timeout = Timeout.InfiniteTimeSpan };
        var response = await client.GetAsync(path, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        return new SseStream(client, response, new StreamReader(stream, Encoding.UTF8));
    }

    /// <summary>The media type the stream was served with.</summary>
    internal string? ContentType => _response.Content.Headers.ContentType?.MediaType;

    /// <summary>The status the stream was opened with.</summary>
    internal HttpStatusCode StatusCode => _response.StatusCode;

    /// <summary>The next frame, or <see langword="null"/> when none arrived in time.</summary>
    /// <param name="within">How long to wait.</param>
    /// <param name="cancellationToken">Cancels the read.</param>
    /// <returns>The frame, or <see langword="null"/>.</returns>
    internal async Task<Frame?> NextAsync(TimeSpan within, CancellationToken cancellationToken)
    {
        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        deadline.CancelAfter(within);

        try
        {
            if (await _incoming.Reader.WaitToReadAsync(deadline.Token)
                && _incoming.Reader.TryRead(out var frame))
            {
                return frame;
            }
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            // Nothing arrived in the window, which several tests assert as an answer.
        }

        return null;
    }

    /// <summary>Reads until a frame matches, or the window passes.</summary>
    /// <param name="matches">What to look for.</param>
    /// <param name="within">How long to keep reading.</param>
    /// <param name="cancellationToken">Cancels the read.</param>
    /// <returns>The frame that matched, or <see langword="null"/>.</returns>
    /// <remarks>
    /// The window is the whole read rather than each frame, so a stream that keeps heartbeating
    /// still ends when the caller said it should.
    /// </remarks>
    internal async Task<Frame?> UntilAsync(
        Func<Frame, bool> matches,
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

            var frame = await NextAsync(remaining, cancellationToken);
            if (frame is null)
            {
                return null;
            }

            if (matches(frame.Value))
            {
                return frame;
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _stopping.CancelAsync();
        await _pump;

        _stopping.Dispose();
        _reader.Dispose();
        _response.Dispose();
        _client.Dispose();
    }

    /// <summary>Reads frames for as long as the stream carries any.</summary>
    /// <param name="cancellationToken">Cancels the read, which is how disposal ends it.</param>
    /// <returns>A task that completes when the stream is over.</returns>
    /// <remarks>
    /// A frame is the <c>event:</c> line and the <c>data:</c> line that follows it. A read that
    /// fails is the stream ending, whichever way it ended; what the test asserts is the frames that
    /// did arrive.
    /// </remarks>
    private async Task PumpAsync(CancellationToken cancellationToken)
    {
        try
        {
            var name = string.Empty;

            while (await _reader.ReadLineAsync(cancellationToken) is { } line)
            {
                if (line.Length == 0)
                {
                    name = string.Empty;
                    continue;
                }

                if (line.StartsWith("event: ", StringComparison.Ordinal))
                {
                    name = line["event: ".Length..];
                    continue;
                }

                if (!line.StartsWith("data: ", StringComparison.Ordinal))
                {
                    continue;
                }

                await _incoming.Writer.WriteAsync(new Frame(name, line["data: ".Length..]), cancellationToken);
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

    /// <summary>One frame: the event it is named, and its data.</summary>
    /// <param name="Name">The <c>event:</c> value, empty when the frame carried none.</param>
    /// <param name="Data">The <c>data:</c> value, exactly as it arrived.</param>
    internal readonly record struct Frame(string Name, string Data);
}

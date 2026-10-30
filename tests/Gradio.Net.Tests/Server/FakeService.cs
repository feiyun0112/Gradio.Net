using System.Net;
using System.Text;
using System.Text.Json.Nodes;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// A service on the other end of a loader: it records what it was asked and answers from a script.
/// </summary>
/// <remarks>
/// <para>
/// What a loader is verified against is the request it builds and what it does with an answer, and
/// neither needs a server: the handler here stands where the network would be, so a test can say
/// what arrived and hand back a response byte for byte. That is the pattern for an external
/// dependency: behind an interface, with a local stand-in — and it is why no test in this folder
/// needs a network or a model.
/// </para>
/// <para>
/// The answers are a queue: the first request gets the first answer, and a request with none left
/// is a failure rather than a repeat, so a test that expects two requests and gets three says so.
/// </para>
/// </remarks>
internal sealed class FakeService : HttpMessageHandler
{
    private readonly Queue<Func<RecordedCall, HttpResponseMessage>> _answers = new();

    /// <summary>What the service was asked, in the order it was asked.</summary>
    internal List<RecordedCall> Calls { get; } = [];

    /// <summary>The client a loader is handed, so that it talks to this service.</summary>
    /// <returns>A client that keeps this handler alive.</returns>
    internal HttpClient Client() => new(this, disposeHandler: false);

    /// <summary>Answers the next request with a JSON body.</summary>
    /// <param name="body">The body.</param>
    /// <param name="status">The status to answer with.</param>
    /// <returns>This service, so that answers can be queued.</returns>
    internal FakeService Answering(string body, HttpStatusCode status = HttpStatusCode.OK)
    {
        _answers.Enqueue(_ => new HttpResponseMessage(status)
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json"),
        });

        return this;
    }

    /// <summary>Answers the next request with a status and a body, as a refusal does.</summary>
    /// <param name="status">The status.</param>
    /// <param name="body">The body the service explains itself with.</param>
    /// <returns>This service, so that answers can be queued.</returns>
    internal FakeService Refusing(HttpStatusCode status, string body) => Answering(body, status);

    /// <summary>Answers the next request with a stream of server-sent events.</summary>
    /// <param name="pieces">The text of each event, in the order the service says them.</param>
    /// <returns>This service, so that answers can be queued.</returns>
    /// <remarks>
    /// The events are written the way an OpenAI-compatible server writes them — one
    /// <c>data:</c> line each, a blank line between, and <c>[DONE]</c> at the end — because the
    /// reading of that shape is half of what a streaming loader is.
    /// </remarks>
    internal FakeService Streaming(params string[] pieces)
    {
        var body = new StringBuilder();

        foreach (var piece in pieces)
        {
            var chunk = new JsonObject
            {
                ["choices"] = new JsonArray(new JsonObject
                {
                    ["delta"] = new JsonObject { ["content"] = piece },
                }),
            };

            body.Append("data: ").Append(chunk.ToJsonString()).Append("\n\n");
        }

        body.Append("data: [DONE]\n\n");

        _answers.Enqueue(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(body.ToString(), Encoding.UTF8, "text/event-stream"),
        });

        return this;
    }

    /// <summary>Answers the next request with a body built from what arrived.</summary>
    /// <param name="answer">What to answer with, given the call.</param>
    /// <returns>This service, so that answers can be queued.</returns>
    internal FakeService AnsweringWith(Func<RecordedCall, HttpResponseMessage> answer)
    {
        _answers.Enqueue(answer);

        return this;
    }

    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var bytes = request.Content is null
            ? []
            : await request.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);

        var call = new RecordedCall(
            request.Method.Method,
            request.RequestUri!.ToString(),
            request.Headers.Authorization?.ToString(),
            request.Content?.Headers.ContentType?.ToString(),
            Encoding.UTF8.GetString(bytes),
            bytes);

        Calls.Add(call);

        if (_answers.Count == 0)
        {
            throw new InvalidOperationException(
                $"The service was asked for more than it was told to answer: {call.Method} {call.Address}.");
        }

        return _answers.Dequeue()(call);
    }
}

/// <summary>One request a service received.</summary>
/// <param name="Method">The method it was asked with.</param>
/// <param name="Address">The address it was asked at.</param>
/// <param name="Authorization">The authorization header, or <see langword="null"/> for none.</param>
/// <param name="ContentType">The body's media type, or <see langword="null"/> for a body without one.</param>
/// <param name="Body">The body, as text.</param>
/// <param name="BodyBytes">The body, as the bytes that arrived.</param>
internal sealed record RecordedCall(
    string Method,
    string Address,
    string? Authorization,
    string? ContentType,
    string Body,
    byte[] BodyBytes);

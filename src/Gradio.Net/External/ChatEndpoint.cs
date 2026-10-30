using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// One OpenAI-compatible chat endpoint, asked for an answer over HTTP.
/// </summary>
/// <remarks>
/// <para>
/// This is the whole of what a loaded chat needs from a model server: a POST to
/// <c>{base_url}chat/completions</c> carrying the model, the messages and whether the answer is
/// wanted a piece at a time, and a reading of the answer in the shape it comes back in. The
/// endpoint speaks the protocol rather than a vendor's SDK, so any server that speaks it — a local
/// Ollama, vLLM, LM Studio, or a hosted API — is the same server to this class.
/// </para>
/// <para>
/// The address is the base URL with <c>chat/completions</c> appended, and a base URL that does not
/// end in a slash is completed with one first — which is what the OpenAI client does to the address
/// it is given (<c>_enforce_trailing_slash</c>), so the endpoint that is reached here is the one a
/// caller's own OpenAI client reaches with the same base URL.
/// </para>
/// </remarks>
internal sealed class ChatEndpoint
{
    /// <summary>The key an endpoint that needs none is given, as the OpenAI client's own default.</summary>
    private const string PlaceholderToken = "***";

    private readonly HttpClient _client;
    private readonly string _address;
    private readonly string _model;
    private readonly string _token;

    /// <summary>Describes an endpoint, taking its own client unless one is given.</summary>
    /// <param name="baseUrl">The base URL, e.g. <c>http://localhost:11434/v1/</c>.</param>
    /// <param name="model">The model to ask for.</param>
    /// <param name="token">The key to send, or <see langword="null"/> for the placeholder.</param>
    /// <param name="client">The client to send with, or <see langword="null"/> for one of its own.</param>
    /// <exception cref="ArgumentException"><paramref name="baseUrl"/> or <paramref name="model"/> is empty.</exception>
    internal ChatEndpoint(string baseUrl, string model, string? token, HttpClient? client)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new ArgumentException("A chat endpoint needs a base URL.", nameof(baseUrl));
        }

        if (string.IsNullOrWhiteSpace(model))
        {
            throw new ArgumentException("A chat endpoint needs a model.", nameof(model));
        }

        _client = client ?? new HttpClient();
        _address = (baseUrl.EndsWith('/') ? baseUrl : baseUrl + "/") + "chat/completions";
        _model = model;
        _token = string.IsNullOrEmpty(token) ? PlaceholderToken : token;
    }

    /// <summary>The client the endpoint asks with, which is also what fetches a file it is sent.</summary>
    internal HttpClient Client => _client;

    /// <summary>Asks for the whole answer at once.</summary>
    /// <param name="conversation">The messages to send.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    /// <returns>The answer's text.</returns>
    /// <exception cref="HttpRequestException">The endpoint did not answer with a success.</exception>
    /// <remarks>
    /// A response that carries no text — one that only calls a tool, which this loader does not ask
    /// for — is answered as an empty message rather than as a failure, because the request itself
    /// succeeded.
    /// </remarks>
    internal async Task<string> AskAsync(JsonArray conversation, CancellationToken cancellationToken)
    {
        using var request = Request(conversation, stream: false);
        using var response = await _client
            .SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken)
            .ConfigureAwait(false);

        var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        ThrowIfRefused(response, body);

        var document = JsonNode.Parse(body) as JsonObject
            ?? throw new HttpRequestException(
                $"The chat endpoint answered something that is not an object: {body}",
                inner: null,
                response.StatusCode);

        return JsonValues.Text(document["choices"]?[0]?["message"]?["content"]) ?? string.Empty;
    }

    /// <summary>Asks for the answer a piece at a time.</summary>
    /// <param name="conversation">The messages to send.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    /// <returns>The answer so far, after each piece arrives.</returns>
    /// <exception cref="HttpRequestException">The endpoint did not answer with a success.</exception>
    /// <remarks>
    /// What is yielded is the answer as it stands rather than the piece that just arrived, which is
    /// the Python version's behaviour and the chat's: the conversation's last message is replaced
    /// by what it is given, so a client that was sent the piece alone would show one word at a
    /// time.
    /// </remarks>
    internal async IAsyncEnumerable<string> StreamAsync(
        JsonArray conversation,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var request = Request(conversation, stream: true);
        using var response = await _client
            .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            ThrowIfRefused(
                response,
                await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false));
        }

        await using var stream = await response.Content
            .ReadAsStreamAsync(cancellationToken)
            .ConfigureAwait(false);

        using var reader = new StreamReader(stream, Encoding.UTF8);
        var answer = new StringBuilder();

        while (await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false) is { } line)
        {
            // A server-sent event is a few named lines and then the data; only the data carries the
            // answer, and the blank line between two events carries nothing.
            if (!line.StartsWith("data:", StringComparison.Ordinal))
            {
                continue;
            }

            var payload = line["data:".Length..].Trim();

            if (payload.Length == 0)
            {
                continue;
            }

            if (payload == "[DONE]")
            {
                yield break;
            }

            var piece = JsonValues.Text(JsonNode.Parse(payload)?["choices"]?[0]?["delta"]?["content"]);

            if (piece is null)
            {
                continue;
            }

            answer.Append(piece);

            yield return answer.ToString();
        }
    }

    /// <summary>The request that asks the endpoint.</summary>
    /// <param name="conversation">The messages to send.</param>
    /// <param name="stream">Whether the answer is wanted a piece at a time.</param>
    /// <returns>The request.</returns>
    private HttpRequestMessage Request(JsonArray conversation, bool stream)
    {
        var body = new JsonObject
        {
            ["model"] = _model,
            ["messages"] = conversation,
        };

        // Only a streaming request says so: the OpenAI client sends `stream` when it was asked for a
        // stream and leaves it out of the body otherwise, so a server that refuses an unknown field
        // is asked the same thing here as it is asked there.
        if (stream)
        {
            body["stream"] = true;
        }

        var request = new HttpRequestMessage(HttpMethod.Post, _address)
        {
            Content = new StringContent(body.ToJsonString(), Encoding.UTF8, "application/json"),
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

        return request;
    }

    /// <summary>Fails the way the endpoint refused, when it refused.</summary>
    /// <param name="response">The response.</param>
    /// <param name="body">What it answered with.</param>
    /// <exception cref="HttpRequestException">The response is not a success.</exception>
    /// <remarks>
    /// The body is part of the message because that is where an OpenAI-compatible server says what
    /// was wrong — an unknown model, a key that is not accepted — and a caller reading the failure
    /// in the chat wants to read that rather than a status code on its own.
    /// </remarks>
    private static void ThrowIfRefused(HttpResponseMessage response, string body)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var reason = string.IsNullOrEmpty(response.ReasonPhrase)
            ? string.Empty
            : $" {response.ReasonPhrase}";

        throw new HttpRequestException(
            $"The chat endpoint answered {(int)response.StatusCode}{reason}: {body}",
            inner: null,
            response.StatusCode);
    }
}

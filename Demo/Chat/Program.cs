using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using Gradio.Net;

// A chat whose answers come from a model endpoint — here, one that runs in this process.
//
// The page is one call: `gr.ChatInterface` builds the chat window, the field, the submit button and
// the endpoints behind them, and the handler is what your application actually does. The Python version's is
// the same call with `fn=echo`:
//
//     def echo(message, history):
//         return message
//
//     gr.ChatInterface(fn=echo, title="Echo").launch()
//
// A handler that streams takes the same two arguments and returns an iterator; each value it yields
// is the whole answer so far, so the chat window replaces the message as it grows.

await using var model = await LocalModel.StartAsync();
using var client = new HttpClient { BaseAddress = model.BaseUrl };

async IAsyncEnumerable<string> Answer(
    string message,
    IReadOnlyList<ChatMessage> history,
    [EnumeratorCancellation] CancellationToken cancellationToken)
{
    // The conversation as the endpoint wants it: the messages before this one, then this one.
    var messages = new JsonArray();

    foreach (var earlier in history)
    {
        messages.Add(new JsonObject { ["role"] = earlier.Role, ["content"] = earlier.Text });
    }

    messages.Add(new JsonObject { ["role"] = ChatRole.User, ["content"] = message });

    var body = new JsonObject
    {
        ["model"] = "local-echo",
        ["stream"] = true,
        ["messages"] = messages,
    };

    using var request = new HttpRequestMessage(HttpMethod.Post, "v1/chat/completions")
    {
        Content = new StringContent(body.ToJsonString(), Encoding.UTF8, "application/json"),
    };

    // The token is the run's: the page's stop button cancels it, and a handler that ignores it keeps
    // reading from the endpoint after the reader has moved on.
    using var response = await client.SendAsync(
        request,
        HttpCompletionOption.ResponseHeadersRead,
        cancellationToken);

    response.EnsureSuccessStatusCode();

    await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
    using var reader = new StreamReader(stream);

    var answer = new StringBuilder();

    while (await reader.ReadLineAsync(cancellationToken) is { } line)
    {
        // Server-sent events: the payload is on a `data: ` line, and `[DONE]` ends the answer.
        if (!line.StartsWith("data: ", StringComparison.Ordinal))
        {
            continue;
        }

        var payload = line["data: ".Length..];

        if (payload == "[DONE]")
        {
            yield break;
        }

        var piece = PieceOf(payload);

        if (string.IsNullOrEmpty(piece))
        {
            continue;
        }

        answer.Append(piece);

        // The whole answer so far, not the piece: that is what the chat window draws.
        yield return answer.ToString();
    }

    // The one line of the protocol this handler has to know: where the text sits in a chunk. An
    // endpoint that answers like OpenAI's puts it here, and so does the one in LocalModel.cs.
    static string? PieceOf(string payload) =>
        JsonNode.Parse(payload)?["choices"]?[0]?["delta"]?["content"]?.GetValue<string>();
}

var demo = gr.ChatInterface(
    Answer,
    title: "Chat with a local endpoint",
    description:
        "The endpoint is in this repository and runs in this process, so the demo needs no account "
        + "and no key, and nothing leaves this machine. Replace `LocalModel` with your own address "
        + "to talk to a real model.",
    apiName: "chat");

// Blocks until Ctrl-C. The page is at http://127.0.0.1:7860.
demo.Launch();

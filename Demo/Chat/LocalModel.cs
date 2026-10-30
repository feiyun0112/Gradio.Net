using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

// A model endpoint that answers like an OpenAI-compatible one, running in this process.
//
// It is here so the demo needs nothing from outside: no account, no key, and no address that is not
// this machine. Everything it does is the protocol — a POST to /v1/chat/completions whose answer
// arrives as server-sent events — which is what a real endpoint does too, so pointing the chat at
// your own server is a change of address.
//
// A real endpoint also needs a key. This one ignores the Authorization header it is sent, and the
// demo never sets one: nothing in this repository reads a token from the environment.
//
// Starting a host on a port the operating system picks is the six lines this file uses. A demo is one
// directory, and copying `Demo/Chat` out of the repository has to give a program that runs.
internal sealed class LocalModel : IAsyncDisposable
{
    private readonly WebApplication _app;

    private LocalModel(WebApplication app, Uri baseUrl)
    {
        _app = app;
        BaseUrl = baseUrl;
    }

    /// <summary>The address the chat is pointed at, with the trailing slash a base URL needs.</summary>
    internal Uri BaseUrl { get; }

    /// <summary>Starts the endpoint on a port the operating system picks.</summary>
    /// <returns>The endpoint, listening.</returns>
    internal static async Task<LocalModel> StartAsync()
    {
        var builder = WebApplication.CreateSlimBuilder();

        builder.Logging.ClearProviders();

        // Port 0 is "pick one": two demos can run at once, and nothing collides with a server the
        // reader already has. The address it picked is read back below.
        builder.WebHost.UseUrls("http://127.0.0.1:0");

        var app = builder.Build();

        app.MapPost("/v1/chat/completions", ReplyAsync);

        await app.StartAsync();

        return new LocalModel(app, new Uri(app.Urls.First() + "/"));
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await _app.StopAsync();
        await _app.DisposeAsync();
    }

    /// <summary>Answers the last message of the request, a word at a time.</summary>
    /// <param name="context">The request.</param>
    /// <returns>Nothing: the answer is the response body.</returns>
    private static async Task ReplyAsync(HttpContext context)
    {
        var cancellationToken = context.RequestAborted;

        var body = await JsonNode.ParseAsync(context.Request.Body, cancellationToken: cancellationToken);
        var asked = body?["messages"]?.AsArray() is { Count: > 0 } messages
            ? messages[^1]?["content"]?.GetValue<string>() ?? string.Empty
            : string.Empty;

        var answer =
            $"You said \"{asked}\". This answer came from an endpoint in the same process, so nothing "
            + "left this machine. Point the chat at your own server and the same handler talks to it.";

        context.Response.ContentType = "text/event-stream";

        foreach (var word in answer.Split(' '))
        {
            var chunk = new JsonObject
            {
                ["choices"] = new JsonArray(new JsonObject
                {
                    ["delta"] = new JsonObject { ["content"] = word + " " },
                }),
            };

            await context.Response.WriteAsync($"data: {chunk.ToJsonString()}\n\n", cancellationToken);
            await context.Response.Body.FlushAsync(cancellationToken);

            await Task.Delay(30, cancellationToken);
        }

        // The Python version's protocol ends a stream with this line, and the client below stops on it.
        await context.Response.WriteAsync("data: [DONE]\n\n", cancellationToken);
    }
}

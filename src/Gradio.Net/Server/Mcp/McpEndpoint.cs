using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Gradio.Net;
/// <summary>
/// The Model Context Protocol endpoint an application offers when it is asked to.
/// </summary>
/// <remarks>
/// <para>
/// The Python version mounts a third-party MCP server under <c>/gradio_api/mcp</c>, which brings
/// three transports with it: the streamable HTTP one at the mount's own address, the older SSE one
/// at <c>/sse</c> with its message channel at <c>/messages/</c>, and a schema document at
/// <c>/schema</c> that a registry can read without opening a connection. All three are here,
/// because a client picks one and an application cannot know which.
/// </para>
/// <para>
/// The mount is why these routes are not in <see cref="GradioRouteTable"/>: what the Python version
/// mounts is a sub-application rather than a route of its own, and the route-set comparison records
/// it as a mount for that reason. What matters is that it exists exactly when the application asked
/// for it, which is what <see cref="McpRouteDataSource"/> decides at the moment the host builds its
/// matcher.
/// </para>
/// <para>
/// A plain browser <c>GET</c> to the endpoint is answered with a page rather than with a JSON-RPC
/// error, because that is what a person who clicked the address printed at launch gets: the
/// streamable transport wants an event-stream request, and a page is a better answer than a
/// refusal to read.
/// </para>
/// </remarks>
internal sealed class McpEndpoint
{
    /// <summary>Where the mount lives, as the Python version spells it.</summary>
    internal const string Prefix = "/gradio_api/mcp";

    /// <summary>The address a streamable HTTP client posts to.</summary>
    internal const string StreamablePath = Prefix;

    /// <summary>The alias the Python version also answers on.</summary>
    internal const string HttpPath = Prefix + "/http";

    /// <summary>The address the older transport's stream is opened on.</summary>
    internal const string SsePath = Prefix + "/sse";

    /// <summary>The address a message is posted to, with the session as a query parameter.</summary>
    internal const string MessagesPath = Prefix + "/messages";

    /// <summary>The address the tool schemas are read from without a connection.</summary>
    internal const string SchemaPath = Prefix + "/schema";

    private readonly Blocks _blocks;
    private readonly McpProtocol _protocol;
    private readonly McpSessions _sessions;
    private readonly ILogger<McpEndpoint> _logger;
    private readonly Action<ILogger, string, Exception?> _logFailed;

    /// <summary>Creates the endpoint.</summary>
    /// <param name="blocks">The application an agent sees.</param>
    /// <param name="protocol">The exchange a message goes through.</param>
    /// <param name="sessions">The open SSE connections.</param>
    /// <param name="logger">Where a failed exchange is reported.</param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    public McpEndpoint(
        Blocks blocks,
        McpProtocol protocol,
        McpSessions sessions,
        ILogger<McpEndpoint> logger)
    {
        ArgumentNullException.ThrowIfNull(blocks);
        ArgumentNullException.ThrowIfNull(protocol);
        ArgumentNullException.ThrowIfNull(sessions);
        ArgumentNullException.ThrowIfNull(logger);

        _blocks = blocks;
        _protocol = protocol;
        _sessions = sessions;
        _logger = logger;

        _logFailed = LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(1, "GradioMcpExchangeFailed"),
            "An MCP exchange failed at '{Path}'.");
    }

    /// <summary>Answers a request on any of the mount's addresses.</summary>
    /// <param name="context">The request.</param>
    /// <returns>A task that completes when the answer has been written.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
    internal async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var path = (context.Request.Path.Value ?? string.Empty).TrimEnd('/');

        try
        {
            switch (path)
            {
                case StreamablePath or HttpPath:
                    await StreamableAsync(context).ConfigureAwait(false);
                    break;

                case SsePath:
                    await StreamAsync(context).ConfigureAwait(false);
                    break;

                case MessagesPath:
                    await ReceiveAsync(context).ConfigureAwait(false);
                    break;

                case SchemaPath:
                    await SchemaAsync(context).ConfigureAwait(false);
                    break;

                default:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    break;
            }
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // The client went away, which ends the exchange rather than failing it.
        }
        catch (Exception failure)
        {
            _logFailed(_logger, path, failure);

            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
        }
    }

    /// <summary>Answers on the streamable HTTP transport's address.</summary>
    /// <param name="context">The request.</param>
    /// <returns>A task that completes when the answer has been written.</returns>
    private async Task StreamableAsync(HttpContext context)
    {
        if (HttpMethods.IsPost(context.Request.Method))
        {
            await PostAsync(context).ConfigureAwait(false);

            return;
        }

        if (!HttpMethods.IsGet(context.Request.Method))
        {
            context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;

            return;
        }

        // A client that asks for an event stream is asking to be kept informed of what the server
        // has to say on its own. Nothing here does: every answer belongs to a request, so there is
        // nothing to stream and the transport's own answer for that is a refusal.
        if (Accepts(context, "text/event-stream"))
        {
            context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;

            return;
        }

        await WriteHtmlAsync(context, McpLandingPage.For(RootUrl.Of(context))).ConfigureAwait(false);
    }

    /// <summary>Handles a message on the streamable transport.</summary>
    /// <param name="context">The request.</param>
    /// <returns>A task that completes when the answer has been written.</returns>
    private async Task PostAsync(HttpContext context)
    {
        var message = await ReadAsync(context).ConfigureAwait(false);
        if (message is null)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            return;
        }

        var response = await _protocol.HandleAsync(message, context, context.RequestAborted).ConfigureAwait(false);

        // A message the client does not expect an answer to is accepted and nothing more, which is
        // what the transport says to do with a notification.
        if (response is null)
        {
            context.Response.StatusCode = StatusCodes.Status202Accepted;

            return;
        }

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "text/event-stream";
        context.Response.Headers.CacheControl = "no-cache";

        await WriteEventAsync(context, "message", JsonText.Serialize(response)).ConfigureAwait(false);
    }

    /// <summary>Opens the older transport's stream.</summary>
    /// <param name="context">The request.</param>
    /// <returns>A task that completes when the stream ends.</returns>
    /// <remarks>
    /// The first event is the address the client posts its messages to, session included; every
    /// event after it is an answer to one of those messages. That is the whole of the transport,
    /// which is why the session lives exactly as long as this request does.
    /// </remarks>
    private async Task StreamAsync(HttpContext context)
    {
        if (!HttpMethods.IsGet(context.Request.Method))
        {
            context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;

            return;
        }

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "text/event-stream";
        context.Response.Headers.CacheControl = "no-cache";

        await context.Response.StartAsync(context.RequestAborted).ConfigureAwait(false);

        var (id, answers) = _sessions.Open();

        try
        {
            await WriteEventAsync(
                context,
                "endpoint",
                $"{context.Request.PathBase}{MessagesPath}/?session_id={id}").ConfigureAwait(false);

            while (await answers.WaitToReadAsync(context.RequestAborted).ConfigureAwait(false))
            {
                while (answers.TryRead(out var answer))
                {
                    await WriteEventAsync(context, "message", JsonText.Serialize(answer)).ConfigureAwait(false);
                }
            }
        }
        finally
        {
            _sessions.Close(id);
        }
    }

    /// <summary>Receives one message of the older transport.</summary>
    /// <param name="context">The request.</param>
    /// <returns>A task that completes when the message has been accepted.</returns>
    private async Task ReceiveAsync(HttpContext context)
    {
        if (!HttpMethods.IsPost(context.Request.Method))
        {
            context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;

            return;
        }

        var session = context.Request.Query["session_id"].ToString();

        if (session.Length == 0 || !_sessions.IsOpen(session))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            return;
        }

        var message = await ReadAsync(context).ConfigureAwait(false);
        if (message is null)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            return;
        }

        var response = await _protocol.HandleAsync(message, context, context.RequestAborted).ConfigureAwait(false);

        if (response is not null)
        {
            _sessions.Answer(session, response);
        }

        context.Response.StatusCode = StatusCodes.Status202Accepted;
    }

    /// <summary>Serves the schema document.</summary>
    /// <param name="context">The request.</param>
    /// <returns>A task that completes when the document has been written.</returns>
    /// <remarks>
    /// The document exists so that a registry can learn what an application offers without holding
    /// a connection open, which is what the Hugging Face MCP server does with it. It describes
    /// everything the application offers — tools, resources and prompts — and narrows to the names
    /// the request asked for, exactly as the lists do.
    /// </remarks>
    private async Task SchemaAsync(HttpContext context)
    {
        if (!HttpMethods.IsGet(context.Request.Method))
        {
            context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;

            return;
        }

        // One catalog, read twice: whether the application offers anything at all is the same
        // question as whether the document has an entry, and answering it by building the endpoint
        // list twice would read every handler's schema twice per request.
        var catalog = McpCatalog.Of(_blocks);

        JsonNode schemas = catalog.Entries.Count == 0
            ? new JsonObject()
            : catalog.Schema(McpProtocol.Selected(context));

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(JsonText.Serialize(schemas), context.RequestAborted).ConfigureAwait(false);
    }

    /// <summary>Whether the request accepts a media type.</summary>
    /// <param name="context">The request.</param>
    /// <param name="mediaType">The media type.</param>
    /// <returns>Whether it does.</returns>
    private static bool Accepts(HttpContext context, string mediaType) =>
        context.Request.Headers.Accept.ToString().Contains(mediaType, StringComparison.OrdinalIgnoreCase);

    /// <summary>Reads a request body as a JSON-RPC message.</summary>
    /// <param name="context">The request.</param>
    /// <returns>The message, or <see langword="null"/> when the body is not a JSON object.</returns>
    private static async Task<JsonObject?> ReadAsync(HttpContext context)
    {
        try
        {
            return await JsonNode.ParseAsync(context.Request.Body, cancellationToken: context.RequestAborted)
                .ConfigureAwait(false) as JsonObject;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>Writes one server-sent event.</summary>
    /// <param name="context">The response to write to.</param>
    /// <param name="name">The event's name.</param>
    /// <param name="data">The event's data, which must not span lines.</param>
    /// <returns>A task that completes when it has been flushed.</returns>
    private static async Task WriteEventAsync(HttpContext context, string name, string data)
    {
        await context.Response.WriteAsync($"event: {name}\ndata: {data}\n\n", context.RequestAborted)
            .ConfigureAwait(false);

        await context.Response.Body.FlushAsync(context.RequestAborted).ConfigureAwait(false);
    }

    /// <summary>Writes an HTML page.</summary>
    /// <param name="context">The response to write to.</param>
    /// <param name="html">The page.</param>
    /// <returns>A task that completes when it has been written.</returns>
    private static async Task WriteHtmlAsync(HttpContext context, string html)
    {
        var bytes = Encoding.UTF8.GetBytes(html);

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "text/html; charset=utf-8";
        context.Response.ContentLength = bytes.Length;

        await context.Response.Body.WriteAsync(bytes, context.RequestAborted).ConfigureAwait(false);
    }
}

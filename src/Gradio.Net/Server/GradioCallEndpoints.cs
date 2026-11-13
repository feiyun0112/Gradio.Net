using System.Collections.Concurrent;
using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// The job-style routes: a client submits a run, is handed an identifier, and reads the result from
/// a second request.
/// </summary>
/// <remarks>
/// <para>
/// This is the Python version's <c>/call/{api_name}</c> pair. It exists because a client that cannot
/// hold a stream open — a shell script, a scheduler, an HTTP tool with a short timeout — still needs
/// to call a queued binding, and the queue's own protocol answers per session rather than per run.
/// </para>
/// <para>
/// The two halves are one session: the <c>POST</c> opens it under a name of its own choosing and the
/// <c>GET</c> reads it. That is why the identifier the <c>POST</c> returns is remembered against the
/// session here — the queue's stream is addressed by session, and the client only ever learns the
/// run's identifier.
/// </para>
/// <para>
/// Both spellings the route table carries are answered: <c>/call/{api_name}</c> and its
/// <c>/call/v2/{api_name}</c> successor. The Python version serves the two from one handler, and the
/// difference between them is the version of the request body, which
/// <see cref="PredictBody.Parse"/> already accepts either way.
/// </para>
/// <para>
/// The result is not the queue's own stream. The Python version's <c>simple_predict_get</c>
/// maps each queue message to a named event through <c>process_msg</c>, and <see cref="JobStream"/>
/// holds that mapping here: the page still reads <c>data: {json}</c> with a kind inside it, while a
/// caller of this route reads <c>event: complete</c> with the outputs in <c>data</c>.
/// </para>
/// </remarks>
internal sealed class GradioCallEndpoints : IGradioEndpoint
{
    private const string CallPath = "/gradio_api/call/{api_name}";
    private const string CallV2Path = "/gradio_api/call/v2/{api_name}";
    private const string ResultPath = "/gradio_api/call/{api_name}/{event_id}";
    private const string ResultV2Path = "/gradio_api/call/v2/{api_name}/{event_id}";

    /// <summary>The prefix a session this route opened is named with.</summary>
    private const string SessionPrefix = "call-";

    /// <summary>How many submitted-but-unread runs are remembered.</summary>
    /// <remarks>
    /// A client that submits and never reads leaves its entry behind, and nothing sweeps the map:
    /// the cap is what keeps that from being unbounded. Past it, the oldest entries are dropped,
    /// which costs those clients their result — the same answer a run nobody wanted deserves.
    /// </remarks>
    private const int RememberedRuns = 1024;

    private readonly Blocks _blocks;
    private readonly GradioQueue _queue;
    private readonly GradioQueueEndpoints _stream;
    private readonly GradioAuthentication _authentication;

    /// <summary>
    /// The session each run this route submitted belongs to, by run identifier.
    /// </summary>
    /// <remarks>
    /// Entries are removed when their stream is read, which is the only way a client learns anything
    /// about a run. A client that submits and never reads leaves its entry behind: the map is bounded
    /// by the number of such clients rather than by time, which is the trade a server without a
    /// sweeper makes. A run nobody reads is a run nobody wanted.
    /// </remarks>
    private readonly ConcurrentDictionary<string, string> _sessions = new(StringComparer.Ordinal);

    /// <summary>Creates the capability.</summary>
    /// <param name="blocks">The application whose bindings are submitted.</param>
    /// <param name="queue">The queue a submission joins.</param>
    /// <param name="stream">The stream a result is read from, shared with the queue's own route.</param>
    /// <param name="authentication">Who is asking, which a run carries to its handler.</param>
    /// <exception cref="ArgumentNullException">One of the arguments is <see langword="null"/>.</exception>
    public GradioCallEndpoints(
        Blocks blocks,
        GradioQueue queue,
        GradioQueueEndpoints stream,
        GradioAuthentication authentication)
    {
        ArgumentNullException.ThrowIfNull(blocks);
        ArgumentNullException.ThrowIfNull(queue);
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(authentication);

        _blocks = blocks;
        _queue = queue;
        _stream = stream;
        _authentication = authentication;
    }

    /// <inheritdoc />
    public IReadOnlyCollection<string> Routes { get; } =
    [
        new GradioRoute("POST", CallPath).Key,
        new GradioRoute("POST", CallV2Path).Key,
        new GradioRoute("GET", ResultPath).Key,
        new GradioRoute("GET", ResultV2Path).Key,
    ];

    /// <inheritdoc />
    public Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var path = context.Request.Path.Value ?? string.Empty;

        // The method decides which half this is: both halves are named by the same path shape, and
        // only the verb tells a submission from a read.
        return HttpMethods.IsPost(context.Request.Method)
            ? SubmitAsync(context)
            : ReadAsync(context);
    }

    /// <summary>Admits a run to the queue and answers with the identifier its result is read by.</summary>
    /// <param name="context">The request, whose body carries the values.</param>
    /// <returns>A task that completes when the response has been written.</returns>
    private async Task SubmitAsync(HttpContext context)
    {
        var apiName = context.Request.RouteValues["api_name"] as string ?? string.Empty;
        var raw = await ReadBodyAsync(context).ConfigureAwait(false);
        var body = PredictBody.Parse(raw);

        var dependency = DependencyOf(apiName, body);
        if (dependency is null)
        {
            await WriteJsonAsync(
                context,
                new JsonObject { ["detail"] = $"Could not infer function index for API name: {apiName}" },
                StatusCodes.Status404NotFound).ConfigureAwait(false);

            return;
        }

        // A binding the application marked private is not callable by clients, which is what
        // `ApiVisibility.Private` says and what this route now enforces. The page is not a client:
        // it reaches its bindings through the queue, so hiding one from here hides it from other
        // programs without taking its button away.
        if (dependency.ApiVisibility == ApiVisibility.Private)
        {
            await WriteJsonAsync(
                context,
                new JsonObject { ["detail"] = $"Could not infer function index for API name: {apiName}" },
                StatusCodes.Status404NotFound).ConfigureAwait(false);

            return;
        }

        // The document these routes are described by says the body is an object of named
        // parameters — that is what `CallPath` writes, and what a client generated from it will
        // send. The queue's own shape is `{"data": [...]}`, so a body that named its arguments is
        // turned into one here rather than refused: the two routes differ in how they are described,
        // not in what they do.
        if (body.Data.Count == 0 && raw is JsonObject named)
        {
            body.Data = NamedArguments(dependency, named);
        }

        // A caller that named no session gets one, because the result is read by session and this
        // route is the only thing that knows which session its own submission opened.
        var sessionHash = string.IsNullOrEmpty(body.SessionHash)
            ? SessionPrefix + Guid.NewGuid().ToString("N")
            : body.SessionHash;

        body.SessionHash = sessionHash;

        if (body.FnIndex is null)
        {
            body.FnIndex = _blocks.Dependencies.ToList().IndexOf(dependency);
        }

        if (_queue.Stopped)
        {
            await WriteJsonAsync(
                context,
                new JsonObject { ["detail"] = "Queue is stopped." },
                StatusCodes.Status503ServiceUnavailable).ConfigureAwait(false);

            return;
        }

        var result = await _queue.PushAsync(
            body,
            await _authentication.UserNameOfAsync(context).ConfigureAwait(false)).ConfigureAwait(false);

        if (!result.Success)
        {
            var status = result.State switch
            {
                PushResult.QueueFull => StatusCodes.Status503ServiceUnavailable,
                PushResult.ValidatorError => StatusCodes.Status422UnprocessableEntity,
                _ => StatusCodes.Status400BadRequest,
            };

            await WriteJsonAsync(
                context,
                new JsonObject { ["detail"] = result.Detail },
                status).ConfigureAwait(false);

            return;
        }

        if (_sessions.Count >= RememberedRuns)
        {
            foreach (var stale in _sessions.Keys.Take(_sessions.Count - RememberedRuns + 1))
            {
                _sessions.TryRemove(stale, out _);
            }
        }

        _sessions[result.Detail] = sessionHash;

        await WriteJsonAsync(
            context,
            new JsonObject { ["event_id"] = result.Detail },
            StatusCodes.Status200OK).ConfigureAwait(false);
    }

    /// <summary>Streams the result of a run this route submitted.</summary>
    /// <param name="context">The request, whose route carries the run identifier.</param>
    /// <returns>A task that completes when the stream ends.</returns>
    /// <remarks>
    /// <para>
    /// The result is framed as the Python version's <c>simple_predict_get</c> frames it: named
    /// server-sent events whose <c>data</c> is the run's outputs, not the queue messages the page
    /// reads. A client generated from <c>openapi.json</c> waits for <c>event: complete</c>, which
    /// is exactly what a successful run writes here.
    /// </para>
    /// <para>
    /// A run this route did not submit has no session to describe, and the answer is the same
    /// refusal a vanished session gets — an <c>event: error</c> — rather than an empty stream a
    /// client would wait on.
    /// </para>
    /// </remarks>
    private async Task ReadAsync(HttpContext context)
    {
        var eventId = context.Request.RouteValues["event_id"] as string ?? string.Empty;

        if (!_sessions.TryRemove(eventId, out var sessionHash))
        {
            await GradioQueueEndpoints.StreamMissingAsync(context, JobStream.Format).ConfigureAwait(false);

            return;
        }

        await _stream.StreamSessionAsync(context, sessionHash, JobStream.Format).ConfigureAwait(false);
    }

    /// <summary>The values a body named, in the order the binding reads them.</summary>
    /// <param name="dependency">The binding the values are for.</param>
    /// <param name="named">The body, whose properties are the parameter names.</param>
    /// <returns>The values, position by position.</returns>
    /// <remarks>
    /// The names are the ones the document was written with: a binding that gave its own names has
    /// those, and one that did not is named by position — the same rule the API document follows,
    /// which is what keeps the two halves of the description in step.
    /// </remarks>
    private static JsonArray NamedArguments(Dependency dependency, JsonObject named)
    {
        var data = new JsonArray();
        var names = dependency.ParameterNames;

        for (var index = 0; index < dependency.Inputs.Count; index++)
        {
            var name = names is not null && index < names.Count && !string.IsNullOrEmpty(names[index])
                ? names[index]!
                : $"param_{index}";

            data.Add(named[name]?.DeepClone());
        }

        return data;
    }

    /// <summary>The binding a request names, by index when it gave one and by name otherwise.</summary>
    /// <param name="apiName">The name the route carried.</param>
    /// <param name="body">The request, which may name the binding by index instead.</param>
    /// <returns>The binding, or <see langword="null"/> when there is none.</returns>
    private Dependency? DependencyOf(string apiName, PredictBody body)
    {
        if (body.FnIndex is { } index)
        {
            return _blocks.DependencyAt(index);
        }

        return _blocks.Dependencies.FirstOrDefault(
            dependency => string.Equals(dependency.ApiName, apiName, StringComparison.Ordinal));
    }

    private static async Task<JsonNode?> ReadBodyAsync(HttpContext context)
    {
        using var reader = new StreamReader(context.Request.Body);

        var text = await reader.ReadToEndAsync(context.RequestAborted).ConfigureAwait(false);

        return string.IsNullOrWhiteSpace(text) ? null : JsonNode.Parse(text);
    }

    private static async Task WriteJsonAsync(HttpContext context, JsonNode document, int status)
    {
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(document.ToJsonString(), context.RequestAborted).ConfigureAwait(false);
    }
}

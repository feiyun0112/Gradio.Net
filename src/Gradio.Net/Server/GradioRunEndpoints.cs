using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Gradio.Net;
/// <summary>
/// Runs a binding for a caller that does not want the queue: the direct route a
/// <c>queue=False</c> binding is called through.
/// </summary>
/// <remarks>
/// <para>
/// The frontend uses this route for every binding that said it need not be queued — a flag button,
/// an example that only fills fields, the clear button's server half — and the client libraries use
/// it to call an application without watching it. The Python version serves it from the same
/// handler as <c>/api/{api_name}</c>: two names for one thing, kept apart because one is what the
/// frontend calls and the other is what a person's script calls.
/// </para>
/// <para>
/// The answer is the payload the Python version's <c>process_api</c> returns, which is the same
/// shape the queue puts inside its messages: the values, whether more are coming, how long the call
/// took, and the state components the call changed. A failure answers with the error payload — a
/// null <c>error</c> unless the application shows its errors, which is the Python version's rule
/// and the reason nothing here reports what the handler threw beyond the log.
/// </para>
/// <para>
/// No validator runs here. The Python version runs one when a run is admitted to the queue, and a
/// binding that skips the queue skips that step with it; the binding carries the validator either
/// way, so this is a property of the route rather than of the binding.
/// </para>
/// </remarks>
internal sealed class GradioRunEndpoints : IGradioEndpoint
{
    private const string RunPath = "/gradio_api/run/{api_name}";
    private const string ApiPath = "/gradio_api/api/{api_name}";

    /// <summary>The refusal a direct call gets when the application closed its direct routes.</summary>
    private const string QueueOnly =
        "This API endpoint does not accept direct HTTP POST requests. Please join the queue to use this API.";

    private readonly Blocks _blocks;
    private readonly ISessionStore _sessions;
    private readonly GradioQueue _queue;
    private readonly GradioAuthentication _authentication;
    private readonly TimeProvider _clock;
    private readonly RunHistoryRecorder? _history;
    private readonly IAnalyticsSink? _analyticsSink;
    private readonly Action<ILogger, string, Exception?> _logRunFailed;

    /// <summary>Creates the capability.</summary>
    /// <param name="blocks">The application whose bindings run.</param>
    /// <param name="sessions">The sessions a run's state lives in.</param>
    /// <param name="queue">The queue, read for how long a binding usually takes.</param>
    /// <param name="authentication">Who is asking, which a run carries to its handler.</param>
    /// <param name="clock">The clock a run's duration is measured with.</param>
    /// <param name="history">
    /// Where a finished run is recorded, or <see langword="null"/> for a host that keeps no history.
    /// </param>
    /// <param name="telemetry">
    /// Where a run that raised is reported, or <see langword="null"/> for a host whose errors go no
    /// further than its log.
    /// </param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    /// <remarks>
    /// No logger is taken here: this capability is a singleton, and a request's logger carries the
    /// request's own scope, so <see cref="Logger"/> reads it from the request instead.
    /// </remarks>
    public GradioRunEndpoints(
        Blocks blocks,
        ISessionStore sessions,
        GradioQueue queue,
        GradioAuthentication authentication,
        TimeProvider clock,
        RunHistoryRecorder? history = null,
        IAnalyticsSink? telemetry = null)
    {
        ArgumentNullException.ThrowIfNull(blocks);
        ArgumentNullException.ThrowIfNull(sessions);
        ArgumentNullException.ThrowIfNull(queue);
        ArgumentNullException.ThrowIfNull(authentication);
        ArgumentNullException.ThrowIfNull(clock);

        _blocks = blocks;
        _sessions = sessions;
        _queue = queue;
        _authentication = authentication;
        _clock = clock;
        _history = history;
        _analyticsSink = telemetry;

        _logRunFailed = LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(1, "GradioDirectRunFailed"),
            "A direct run of '{Endpoint}' failed.");
    }

    /// <inheritdoc />
    public IReadOnlyCollection<string> Routes { get; } =
    [
        new GradioRoute("POST", RunPath).Key,
        new GradioRoute("POST", ApiPath).Key,
    ];

    /// <inheritdoc />
    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var apiName = context.Request.RouteValues["api_name"] as string ?? string.Empty;
        var body = PredictBody.Parse(await ReadAsync(context).ConfigureAwait(false));

        var dependency = DependencyOf(apiName, body);
        if (dependency is null)
        {
            await WriteJsonAsync(
                context,
                new JsonObject { ["detail"] = $"Could not infer function index for API name: {apiName}" },
                StatusCodes.Status404NotFound).ConfigureAwait(false);

            return;
        }

        // The Python version closes the direct routes with one switch, so an application that closed them
        // answers 404 rather than running the binding anyway: a queued binding is exactly the one
        // the switch is about.
        if (!_blocks.QueueSettings.ApiOpen && dependency.Queue)
        {
            await WriteJsonAsync(
                context,
                new JsonObject { ["detail"] = QueueOnly },
                StatusCodes.Status404NotFound).ConfigureAwait(false);

            return;
        }

        var state = body.SessionHash is { Length: > 0 } hash ? _sessions.GetOrAdd(hash).State : null;
        var run = new DependencyRun(
            dependency.Shape,
            _clock,
            state,
            new Request(
                await _authentication.UserNameOfAsync(context).ConfigureAwait(false),
                body.SessionHash));

        try
        {
            var startedAt = RunHistoryKeys.Now();
            var output = await RunOnceAsync(run, dependency, body, context.RequestAborted).ConfigureAwait(false);
            _queue.Measure(dependency, output);

            // A direct call is a run like any other, and a finished one is recorded: the route is
            // how a client that does not watch the queue calls an application, and its runs belong
            // in the same history.
            if (!output.IsGenerating)
            {
                _history?.Record(dependency, body.Data.DeepClone(), output.Data, startedAt);
            }

            await WriteJsonAsync(
                context,
                PayloadOf(output, dependency),
                StatusCodes.Status200OK).ConfigureAwait(false);
        }
        catch (Exception failure)
        {
            // A refusal that said it was not to be printed is already on its way to the user.
            if (ErrorPayloads.IsLogWorthy(failure))
            {
                _logRunFailed(Logger(context), dependency.ApiName, failure);
            }

            // The error the framework caught, reported the way the Python version reports one. A direct call
            // is a run like any other, so its failure is the same event a queued run's is.
            ApplicationAnalytics.Failed(_blocks, _analyticsSink, failure.Message);

            // The handler's own enumerator is still open, and closing it is what lets the handler's
            // `finally` run. The Python version ends the run's streams on the same path.
            await run.ResetAsync().ConfigureAwait(false);

            await WriteJsonAsync(
                context,
                ErrorPayloads.Of(failure, _blocks.ShowError),
                StatusCodes.Status500InternalServerError).ConfigureAwait(false);
        }
    }

    /// <summary>The binding a request names, by index when it gave one and by name otherwise.</summary>
    /// <param name="apiName">The name the route carried.</param>
    /// <param name="body">The request, which may name the binding by index instead.</param>
    /// <returns>The binding, or <see langword="null"/> when there is none.</returns>
    /// <remarks>
    /// The index wins because that is what the Python version reads first: the frontend sends both,
    /// and the index is the one that cannot be ambiguous when two bindings share a name.
    /// </remarks>
    private Dependency? DependencyOf(string apiName, PredictBody body)
    {
        if (body.FnIndex is { } index)
        {
            return _blocks.DependencyAt(index);
        }

        return _blocks.Dependencies.FirstOrDefault(
            dependency => string.Equals(dependency.ApiName, apiName, StringComparison.Ordinal));
    }

    /// <summary>Runs the handler once, or takes the first chunk of a streamed one.</summary>
    /// <param name="run">The run.</param>
    /// <param name="dependency">The binding being run.</param>
    /// <param name="body">The values the client sent.</param>
    /// <param name="cancellationToken">Cancels the run.</param>
    /// <returns>What the run produced.</returns>
    /// <remarks>
    /// A streamed run answers with its first chunk and stops there, which is what the Python
    /// version's direct route does: the client that called it has no way to ask for the next one,
    /// and the run's enumerator is closed rather than left open for nobody.
    /// </remarks>
    private static async Task<RunOutput> RunOnceAsync(
        DependencyRun run,
        Dependency dependency,
        PredictBody body,
        CancellationToken cancellationToken)
    {
        var inputs = InputsOf(body);
        var batched = dependency.Batch && body.Batched;

        // A batching handler called with one sample is called with a batch of one, and the answer
        // is unwrapped again: the Python version's rule, and the reason a caller need not know whether the
        // binding batches. What the client sent is already one value per input, which is what one
        // sample is, so the values are read as a batch rather than rewrapped into one — wrapping
        // them would make each sample the list the client sent instead of its value.
        var singleAsBatch = dependency.Batch && !body.Batched;
        if (singleAsBatch)
        {
            batched = true;
        }

        var output = await run.NextAsync(inputs, batched, new Progress(), eventData: null, cancellationToken).ConfigureAwait(false);

        return singleAsBatch
            ? output with { Data = output.Data[0] is JsonArray slice ? slice : [] }
            : output;
    }

    /// <summary>The values a client sent, one array per input component.</summary>
    /// <param name="body">The request.</param>
    /// <returns>One single-value array per entry of the request's data.</returns>
    private static List<JsonArray> InputsOf(PredictBody body)
    {
        var inputs = new List<JsonArray>(body.Data.Count);

        foreach (var value in body.Data)
        {
            inputs.Add([value?.DeepClone()]);
        }

        return inputs;
    }

    /// <summary>The payload a run is answered with, in the Python version's key order.</summary>
    /// <param name="output">What the run produced.</param>
    /// <param name="dependency">The binding that ran.</param>
    /// <returns>The payload.</returns>
    private JsonObject PayloadOf(RunOutput output, Dependency dependency)
    {
        var changed = new JsonArray();

        foreach (var id in output.ChangedStateIds)
        {
            changed.Add(id);
        }

        return new JsonObject
        {
            ["data"] = output.Data.DeepClone(),
            ["is_generating"] = output.IsGenerating,
            ["duration"] = output.Duration,
            ["average_duration"] = _queue.AverageRunTimeOf(dependency),
            ["render_config"] = null,
            ["changed_state_ids"] = changed,
            ["used_cache"] = null,
        };
    }

    /// <summary>The logger to report a failed run to.</summary>
    /// <param name="context">The request being answered.</param>
    /// <returns>The logger, when the host registered one.</returns>
    /// <remarks>
    /// Read from the request rather than held, because this capability is a singleton and a
    /// request's logger carries the request's own scope.
    /// </remarks>
    private static ILogger Logger(HttpContext context) =>
        context.RequestServices.GetService(typeof(ILogger<GradioRunEndpoints>)) as ILogger<GradioRunEndpoints>
        ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<GradioRunEndpoints>.Instance;

    /// <summary>Reads a request body as JSON.</summary>
    /// <param name="context">The request.</param>
    /// <returns>The parsed body, or <see langword="null"/> when it is not JSON.</returns>
    private static async Task<JsonNode?> ReadAsync(HttpContext context)
    {
        try
        {
            return await JsonNode.ParseAsync(context.Request.Body, cancellationToken: context.RequestAborted)
                .ConfigureAwait(false);
        }
        catch (System.Text.Json.JsonException)
        {
            return null;
        }
    }

    /// <summary>Answers with a JSON document.</summary>
    /// <param name="context">The request and the response to write.</param>
    /// <param name="document">The document.</param>
    /// <param name="status">The status to answer with.</param>
    private static async Task WriteJsonAsync(HttpContext context, JsonNode document, int status)
    {
        var body = JsonText.Serialize(document);
        var bytes = System.Text.Encoding.UTF8.GetBytes(body);

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";
        context.Response.ContentLength = bytes.Length;

        await context.Response.Body.WriteAsync(bytes, context.RequestAborted).ConfigureAwait(false);
    }
}

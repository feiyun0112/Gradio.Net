using System.Text.Json;
using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// Runs one of the application's endpoints for an agent.
/// </summary>
/// <remarks>
/// <para>
/// A tool call is a call of an endpoint with the arguments the agent sent, by name, and the answer
/// as the values the endpoint returned. Two things follow from "an endpoint" rather than "a
/// function": the arguments are put in the order the binding's inputs are in — an agent names them,
/// a handler reads them positionally — and the run takes the same road a client's call would, so a
/// binding that is queued is queued, a concurrency limit limits it, and a state input starts from
/// its default because an agent has no session to carry one.
/// </para>
/// <para>
/// The Python version reaches the same place by looping back over HTTP to its own call endpoint.
/// That is not needed here: the queue is in this process, so an agent's run joins it directly. What
/// is reproduced is the part that matters — the run is admitted the way any other is — and what is
/// not is a round trip through a socket to reach code that is already here.
/// </para>
/// </remarks>
internal sealed class McpToolRunner
{
    private readonly ISessionStore _sessions;
    private readonly GradioQueue _queue;
    private readonly TimeProvider _clock;

    /// <summary>Creates the runner.</summary>
    /// <param name="sessions">The sessions a run's state lives in.</param>
    /// <param name="queue">The queue a queued binding's run waits in.</param>
    /// <param name="clock">The clock a run's duration is measured with.</param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    public McpToolRunner(ISessionStore sessions, GradioQueue queue, TimeProvider clock)
    {
        ArgumentNullException.ThrowIfNull(sessions);
        ArgumentNullException.ThrowIfNull(queue);
        ArgumentNullException.ThrowIfNull(clock);

        _sessions = sessions;
        _queue = queue;
        _clock = clock;
    }

    /// <summary>Runs a binding with the arguments an agent sent.</summary>
    /// <param name="dependency">The binding.</param>
    /// <param name="names">The endpoint's parameter names, in the order the binding reads them.</param>
    /// <param name="arguments">The arguments, by name.</param>
    /// <param name="userName">Who is calling, or <see langword="null"/> when nobody is signed in.</param>
    /// <param name="cancellationToken">Cancels the run.</param>
    /// <returns>The values the endpoint returned, one per output.</returns>
    /// <exception cref="McpCallFailure">The run was refused, or the handler failed.</exception>
    /// <remarks>
    /// A value that does not describe the type the endpoint declared is reported the way a failing
    /// handler is — as a call that failed — rather than escaping as a broken exchange: the caller
    /// sent it, so the caller is who has to read what was wrong with it.
    /// </remarks>
    internal async Task<JsonArray> RunAsync(
        Dependency dependency,
        IReadOnlyList<string> names,
        JsonObject arguments,
        string? userName,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(dependency);
        ArgumentNullException.ThrowIfNull(names);
        ArgumentNullException.ThrowIfNull(arguments);

        var data = DataOf(dependency, names, arguments);

        try
        {
            return dependency.Queue
                ? await QueuedAsync(dependency, data, userName, cancellationToken).ConfigureAwait(false)
                : await DirectAsync(dependency, data, userName, cancellationToken).ConfigureAwait(false);
        }
        catch (JsonException failure)
        {
            throw new McpCallFailure(failure.Message);
        }
    }

    /// <summary>The values the binding's inputs are read from, in the binding's order.</summary>
    /// <param name="dependency">The binding.</param>
    /// <param name="names">The endpoint's parameter names.</param>
    /// <param name="arguments">The arguments, by name.</param>
    /// <returns>One value per input, with an absent one for an input the API does not describe.</returns>
    /// <remarks>
    /// <para>
    /// An input the API does not describe — a state — is sent as nothing, which is the Python
    /// version's placeholder: the value belongs to the session rather than to the caller, and a run
    /// that named one would be a run a caller could put a value into a place it cannot see.
    /// </para>
    /// <para>
    /// An argument the caller left out is the default the endpoint declared, not nothing: the
    /// schema says the parameter is optional, so a call that omits it has to run the way the same
    /// call with the default spelled out would. The Python version fills the same gap where it
    /// reads a client's arguments against the endpoint's parameters.
    /// </para>
    /// </remarks>
    private static JsonArray DataOf(Dependency dependency, IReadOnlyList<string> names, JsonObject arguments)
    {
        var data = new JsonArray();
        var next = 0;

        foreach (var input in dependency.Inputs)
        {
            if (input.SkipApi)
            {
                data.Add(null);

                continue;
            }

            var name = next < names.Count ? names[next] : string.Empty;
            next++;

            data.Add(arguments.TryGetPropertyValue(name, out var value)
                ? value?.DeepClone()
                : (input as Api)?.Value?.DeepClone());
        }

        return data;
    }

    /// <summary>Runs a queued binding, and waits for the queue to say it is over.</summary>
    /// <param name="dependency">The binding.</param>
    /// <param name="data">The values the inputs are read from.</param>
    /// <param name="cancellationToken">Cancels the wait.</param>
    /// <param name="userName">Who is calling, or <see langword="null"/> when nobody is signed in.</param>
    /// <returns>The values the endpoint returned.</returns>
    /// <remarks>
    /// The run joins a session of its own, which is what a call from outside the page has: one run,
    /// one session, and nothing carried from one call to the next — an agent that calls twice gets
    /// the default state twice, which is the Python version's answer for the same case.
    /// </remarks>
    private async Task<JsonArray> QueuedAsync(
        Dependency dependency,
        JsonArray data,
        string? userName,
        CancellationToken cancellationToken)
    {
        var session = $"mcp-{Guid.NewGuid():N}";
        var pushed = await _queue.PushAsync(
            new PredictBody { SessionHash = session, FnIndex = dependency.Id, Data = data },
            userName).ConfigureAwait(false);

        if (!pushed.Success)
        {
            throw new McpCallFailure(pushed.Detail);
        }

        var answers = _queue.OpenSession(session);

        try
        {
            while (await answers.WaitToReadAsync(cancellationToken).ConfigureAwait(false))
            {
                while (answers.TryRead(out var message))
                {
                    if (JsonValues.Text(message["msg"]) != QueueMessageKinds.ProcessCompleted)
                    {
                        continue;
                    }

                    if (message["success"]?.GetValue<bool>() != true)
                    {
                        throw new McpCallFailure(FailureOf(message));
                    }

                    return message["output"]?["data"] as JsonArray ?? [];
                }
            }

            throw new McpCallFailure("The run ended without a result.");
        }
        finally
        {
            // Dropped rather than closed: the session was made for this call, and no page was
            // holding it, so nothing the application bound to a visitor leaving applies.
            await _queue.DiscardSessionAsync(session).ConfigureAwait(false);
        }
    }

    /// <summary>Runs a binding that does not go through the queue.</summary>
    /// <param name="dependency">The binding.</param>
    /// <param name="data">The values the inputs are read from.</param>
    /// <param name="cancellationToken">Cancels the run.</param>
    /// <param name="userName">Who is calling, or <see langword="null"/> when nobody is signed in.</param>
    /// <returns>The values the endpoint returned.</returns>
    /// <remarks>
    /// A streamed binding answers with its first chunk and stops there, which is what a direct call
    /// does for any caller: whoever asked has no way to ask for the next one.
    /// </remarks>
    private async Task<JsonArray> DirectAsync(
        Dependency dependency,
        JsonArray data,
        string? userName,
        CancellationToken cancellationToken)
    {
        var state = _sessions.GetOrAdd($"mcp-{Guid.NewGuid():N}").State;
        var run = new DependencyRun(dependency.Shape, _clock, state, new Request(userName));

        var inputs = new List<JsonArray>(data.Count);
        foreach (var value in data)
        {
            inputs.Add([value?.DeepClone()]);
        }

        // A batching handler called with one sample is called with a batch of one, and the answer is
        // unwrapped again: the Python version's rule, and the reason an agent need not know whether the
        // binding batches.
        if (dependency.Batch)
        {
            inputs = [.. inputs.Select(values => new JsonArray(values.ToArray()))];

            var batched = await run.NextAsync(inputs, batched: true, new Progress(), eventData: null, cancellationToken)
                .ConfigureAwait(false);

            return batched.Data[0] as JsonArray ?? [];
        }

        var output = await run.NextAsync(inputs, batched: false, new Progress(), eventData: null, cancellationToken)
            .ConfigureAwait(false);

        return output.Data;
    }

    /// <summary>What a failed run says about itself.</summary>
    /// <param name="message">The completion message.</param>
    /// <returns>The text to report.</returns>
    /// <remarks>
    /// The Python version composes the same sentence out of the payload's title and error, and
    /// falls back to a bare "Error!" when a run reports neither — which happens when the
    /// application does not show its errors, so the sentence is all a caller is told.
    /// </remarks>
    private static string FailureOf(JsonObject message)
    {
        var title = JsonValues.Text(message["title"]);
        var error = JsonValues.Text(message["output"]?["error"]);

        return (title, error) switch
        {
            ({ Length: > 0 } heading, { Length: > 0 } detail) => $"{heading}: {detail}",
            ({ Length: > 0 } heading, _) => heading,
            (_, { Length: > 0 } detail) => detail,
            _ => "Error!",
        };
    }
}

/// <summary>
/// A run an agent asked for that did not produce an answer.
/// </summary>
/// <remarks>
/// Its own type rather than an exception a caller would have to recognise by message, because the
/// protocol reports a failed call differently from a failed request: the first is an answer that
/// says the call failed, and the second is an error in the exchange itself.
/// </remarks>
internal sealed class McpCallFailure : Exception
{
    /// <summary>Creates the failure.</summary>
    /// <param name="message">What went wrong, as a caller is told it.</param>
    internal McpCallFailure(string message)
        : base(message)
    {
    }
}

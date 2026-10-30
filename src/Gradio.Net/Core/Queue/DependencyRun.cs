using System.Reflection;
using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>One result of a run, or one chunk of a streamed one.</summary>
/// <param name="Data">
/// One entry per output for a single run, or one entry per batched sample — each itself one entry
/// per output — for a batched one.
/// </param>
/// <param name="IsGenerating">Whether the run has more chunks to produce.</param>
/// <param name="Duration">How long the handler call took, in seconds.</param>
/// <param name="ChangedStateIds">
/// The state components whose value the run changed and whose <c>change</c> event something is
/// listening to, which is what makes the frontend dispatch that event.
/// </param>
/// <param name="StreamValues">
/// What a streaming output was handed, by component id. It is the handler's own value rather than
/// the one the output postprocessed, because a streaming output's value is the stream: the queue
/// turns each of these into a segment and puts the stream's file object in the payload.
/// </param>
/// <remarks>
/// This is what the Python version's <c>process_api</c> returns, minus the parts the queue adds:
/// the payload it is wrapped into carries the queue's own statistics, which is why they are not
/// here.
/// </remarks>
internal sealed record RunOutput(
    JsonArray Data,
    bool IsGenerating,
    double Duration,
    IReadOnlyList<int> ChangedStateIds,
    IReadOnlyDictionary<int, object?> StreamValues);

/// <summary>
/// What a run needs to know about the binding it is running.
/// </summary>
/// <param name="Handler">The handler to call.</param>
/// <param name="Inputs">The components the handler reads, in order.</param>
/// <param name="Outputs">The components the result updates, in order.</param>
/// <param name="Preprocess">Whether the input components turn their values into the handler's arguments.</param>
/// <param name="Postprocess">Whether the output components turn the result into the values they carry.</param>
/// <param name="TrackedStateIds">
/// The stateful outputs whose value the run reports as changed, which are the ones with a
/// <c>change</c> listener bound to them.
/// </param>
/// <param name="Remembers">
/// Whether the values the run read and produced are recorded in the session, which is what a share
/// link restores. False for a run whose answer is not a value anybody saw — a validator's verdict.
/// </param>
/// <remarks>
/// A binding is one thing and a run of it is another, and the difference is a validator: it runs
/// with the binding's inputs and no outputs at all, which is a shape rather than a binding. Taking
/// the shape rather than the binding is what lets one runner serve both without a second type.
/// </remarks>
internal sealed record RunShape(
    Delegate? Handler,
    IReadOnlyList<Block> Inputs,
    IReadOnlyList<Block> Outputs,
    bool Preprocess,
    bool Postprocess,
    IReadOnlyList<int> TrackedStateIds,
    bool Remembers = true);

/// <summary>
/// Runs one binding's handler: reads the components' values, calls the handler, and turns what it
/// returned into the values the outputs receive.
/// </summary>
/// <remarks>
/// <para>
/// The Python version calls this <c>process_api</c> and keeps it on the application; here it is one
/// object per admitted run, because a run is what owns a generator's enumerator. Holding the
/// enumerator on the event is what makes a generator resumable: the queue calls
/// <see cref="NextAsync"/> again when the client asks for the next chunk, and the enumerator is
/// still where it was left.
/// </para>
/// <para>
/// A handler parameter of type <see cref="Progress"/> or <see cref="CancellationToken"/> is
/// supplied by the framework rather than read from an input, which is how a handler reports progress
/// or observes a cancellation. Everything else maps to an input positionally, because a .NET
/// delegate carries no parameter names to match on.
/// </para>
/// </remarks>
internal sealed class DependencyRun
{
    private static readonly MethodInfo AdapterFactory = typeof(DependencyRun)
        .GetMethod(nameof(Adapter), BindingFlags.NonPublic | BindingFlags.Static)!;

    private readonly RunShape _shape;
    private readonly TimeProvider _clock;
    private readonly SessionState _state;
    private readonly bool _hasSession;

    /// <summary>Who the run is for, which a handler that asks for one reads.</summary>
    private readonly Request _request;

    private IAsyncEnumerator<object?>? _chunks;

    /// <summary>The last chunk produced, whole, which is what the client has and what the next
    /// chunk is described as an edit to.</summary>
    private JsonArray? _lastChunk;

    /// <summary>What the streaming outputs of this call were handed, by component id.</summary>
    private readonly Dictionary<int, object?> _streamValues = [];

    /// <summary>
    /// The trace this run's phases are measured into, or <see langword="null"/> when the application
    /// is not profiled.
    /// </summary>
    /// <remarks>
    /// Set by the queue when the run claims a slot, which is the only moment a trace exists: it
    /// begins with the queue wait and ends with the last chunk, and neither is this object's to
    /// decide. A run whose trace is null pays one null check per phase and nothing else, which is
    /// what makes profiling something an application turns on rather than pays for always.
    /// </remarks>
    internal RequestTrace? Trace { get; set; }

    /// <summary>Creates the run of one binding.</summary>
    /// <param name="shape">What the binding looks like to a run.</param>
    /// <param name="clock">The clock the run's duration is measured with.</param>
    /// <param name="state">
    /// The session's values, or <see langword="null"/> for a run that belongs to no session — a
    /// direct call rather than a served one, which gets a session of its own so that a stateful
    /// component still behaves like one.
    /// </param>
    /// <param name="request">
    /// Who the run is for, or <see langword="null"/> for a run nobody asked for — an unload
    /// handler, a validator, an example being cached. Such a run still hands its handler a request,
    /// with nothing in it, because the Python version hands one over either way.
    /// </param>
    internal DependencyRun(
        RunShape shape,
        TimeProvider clock,
        SessionState? state = null,
        Request? request = null)
    {
        _shape = shape;
        _clock = clock;
        _hasSession = state is not null;
        _state = state ?? new SessionState();
        _request = request ?? new Request();
    }

    /// <summary>Runs the handler, or takes the next chunk of a streamed one.</summary>
    /// <param name="inputs">
    /// One array per input component. A single run's array holds its one value; a batched run's
    /// holds one value per sample in the batch.
    /// </param>
    /// <param name="batched">Whether the values are a batch.</param>
    /// <param name="progress">The tracker to hand a handler that asks for one.</param>
    /// <param name="eventData">
    /// What the event that started the run said about itself, for a handler that reads it, or
    /// <see langword="null"/> for a run whose event says nothing.
    /// </param>
    /// <param name="cancellationToken">Cancels the run.</param>
    /// <returns>The run's result, or its next chunk.</returns>
    /// <remarks>
    /// <para>
    /// A streamed run's chunks are not all sent the same way. The first is sent whole, because the
    /// client has nothing to apply an edit to; every chunk after it is sent as the edit that turns
    /// the one before it into it; and the run's last message carries the last chunk whole again,
    /// because by then there is no next edit to expect (see <see cref="JsonDiff"/>).
    /// </para>
    /// <para>
    /// A batched run is never a stream, whatever its handler returned: the Python version's batch
    /// path reads one whole result and never keeps an iterator, so there is nothing for a second
    /// chunk to come from.
    /// </para>
    /// </remarks>
    internal async Task<RunOutput> NextAsync(
        IReadOnlyList<JsonArray> inputs,
        bool batched,
        Progress progress,
        JsonNode? eventData,
        CancellationToken cancellationToken)
    {
        var started = _clock.GetTimestamp();

        // The tracked values are read before anything runs, because what a client is told is which
        // of them the run changed: a hash taken afterwards would have nothing to compare against.
        var tracked = TrackedStates();
        _streamValues.Clear();
        JsonArray data;

        try
        {
            if (_chunks is null)
            {
                var preprocessed = PhaseStart();
                var arguments = Arguments(inputs, batched, progress, eventData, cancellationToken);
                Measure(ProfilingPhases.Preprocess, preprocessed);

                var called = PhaseStart();
                var result = await DelegateInvocation
                    .InvokeAsync(_shape.Handler!, arguments, cancellationToken)
                    .ConfigureAwait(false);
                Measure(ProfilingPhases.FunctionCall, called);

                if (batched || !EventResult.IsStream(result))
                {
                    var postprocessed = PhaseStart();
                    data = Postprocess(EventResult.Unpack(result), batched);
                    Measure(ProfilingPhases.Postprocess, postprocessed);
                    Remember(inputs, data, batched);

                    return Output(data, generating: false, started, tracked);
                }

                _chunks = Cursor(result!, cancellationToken);
            }

            var moved = PhaseStart();
            var next = await _chunks.MoveNextAsync().ConfigureAwait(false);
            Measure(ProfilingPhases.FunctionCall, moved);

            if (next)
            {
                var postprocessed = PhaseStart();
                var whole = Postprocess(EventResult.Unpack(_chunks.Current), batched);
                Measure(ProfilingPhases.Postprocess, postprocessed);
                Remember(inputs, whole, batched);

                var encoded = PhaseStart();
                data = _lastChunk is null ? whole : Edits(_lastChunk, whole);
                Measure(ProfilingPhases.StreamingDiff, encoded);
                _lastChunk = whole;

                return Output(data, generating: true, started, tracked);
            }

            await _chunks.DisposeAsync().ConfigureAwait(false);
            _chunks = null;

            // A generator that has run out produces no value of its own, but its outputs still have to
            // be said: what they are is the last chunk, which is what the client applied. The Python version
            // spells that as the sentinel its postprocessing turns into nothing, and then overwrites
            // the whole chunk back in — the same answer, reached here without the detour.
            data = _lastChunk ?? Postprocess(EventResult.Unpack(FinishedIterating.Instance), batched);

            return Output(data, generating: false, started, tracked);
        }
        finally
        {
            // The whole call, measured once: the phase the summary reports as what a run costs, and
            // the one whose count is how many chunks the run produced.
            Measure(ProfilingPhases.Total, started);
        }
    }

    /// <summary>When a phase began, or zero when nothing is measuring it.</summary>
    /// <returns>A clock reading, or <c>0</c> when the run is not traced.</returns>
    /// <remarks>
    /// The reading is what an unprofiled run must not pay for, so the branch comes before the call
    /// rather than inside it: <see cref="Measure"/> cannot skip an argument its caller has already
    /// evaluated, and reading the clock is the only thing here that costs anything.
    /// </remarks>
    private long PhaseStart() => Trace is null ? 0 : _clock.GetTimestamp();

    /// <summary>Adds one phase's measurement to the trace, when the application is profiled.</summary>
    /// <param name="phase">The phase, named as <see cref="ProfilingPhases"/> spells it.</param>
    /// <param name="started">When the phase began, as a clock reading.</param>
    /// <remarks>
    /// The elapsed time is computed only when there is a trace to add to, because the argument of a
    /// null conditional is not evaluated: an application that is not profiled pays the branch and
    /// nothing more.
    /// </remarks>
    private void Measure(string phase, long started) =>
        Trace?.Add(phase, _clock.GetElapsedTime(started).TotalMilliseconds);

    /// <summary>The result of one call, with the state components it changed.</summary>
    /// <param name="data">What the outputs receive.</param>
    /// <param name="generating">Whether the run has more chunks to produce.</param>
    /// <param name="started">When the call began, as a clock reading.</param>
    /// <param name="before">The tracked values as they were before the call.</param>
    /// <returns>The result.</returns>
    private RunOutput Output(
        JsonArray data,
        bool generating,
        long started,
        IReadOnlyDictionary<int, JsonNode?> before) =>
        new(
            data,
            generating,
            Elapsed(started),
            ChangedStateIds(before),
            new Dictionary<int, object?>(_streamValues));

    /// <summary>The tracked values, as they are now.</summary>
    /// <returns>One entry per tracked state component.</returns>
    /// <remarks>
    /// A run that belongs to no session tracks nothing, which is the Python version's rule: it
    /// computes changed ids only <c>if state</c>, and a direct call is a run whose state lives no
    /// longer than the call itself.
    /// </remarks>
    private Dictionary<int, JsonNode?> TrackedStates()
    {
        var values = new Dictionary<int, JsonNode?>();

        if (!_hasSession)
        {
            return values;
        }

        foreach (var id in _shape.TrackedStateIds)
        {
            _state.TryGet(id, out var stored);
            values[id] = stored?.DeepClone();
        }

        return values;
    }

    /// <summary>Which of the tracked values the call changed.</summary>
    /// <param name="before">The values as they were before the call.</param>
    /// <returns>The ids, in the order the binding declares its outputs.</returns>
    /// <remarks>
    /// A value counts as changed when it is not the same JSON any more, which is the Python
    /// version's rule expressed as a comparison rather than as a hash of the value: the question is
    /// whether the client should be told, not what the value was.
    /// </remarks>
    private List<int> ChangedStateIds(IReadOnlyDictionary<int, JsonNode?> before)
    {
        var changed = new List<int>();

        foreach (var (id, was) in before)
        {
            _state.TryGet(id, out var stored);

            if (!JsonNode.DeepEquals(was, stored))
            {
                changed.Add(id);
            }
        }

        return changed;
    }

    /// <summary>The edit that turns one chunk into the next, one entry per output.</summary>
    private static JsonArray Edits(JsonArray previous, JsonArray current)
    {
        var edits = new JsonArray();

        for (var index = 0; index < current.Count; index++)
        {
            edits.Add(JsonDiff.Between(
                index < previous.Count ? previous[index] : null,
                current[index]));
        }

        return edits;
    }

    /// <summary>Closes a stream this run still holds, so that the handler suspended in it can end.</summary>
    /// <returns>A task that completes once the enumerator is closed.</returns>
    /// <remarks>
    /// A generator's own <c>finally</c> blocks run when its enumerator is disposed, which is why
    /// this matters for a run that was cancelled or abandoned rather than one that ran out: the
    /// client is done with it, but the handler is still suspended inside it, holding whatever it
    /// opened before its last yield. The Python version closes the iterator the same way, and
    /// ignores whatever closing it throws — a handler that failed is already being described to the
    /// client.
    /// </remarks>
    internal async ValueTask ResetAsync()
    {
        if (_chunks is null)
        {
            return;
        }

        var chunks = _chunks;
        _chunks = null;

        try
        {
            await chunks.DisposeAsync().ConfigureAwait(false);
        }
        catch (Exception)
        {
            // There is nobody left to tell.
        }
    }

    /// <summary>How long the run has been going, in seconds.</summary>
    private double Elapsed(long started) => _clock.GetElapsedTime(started).TotalSeconds;

    /// <summary>The arguments to call the handler with, in the order its signature declares.</summary>
    /// <param name="inputs">The raw values, one array per input.</param>
    /// <param name="batched">Whether the values are a batch.</param>
    /// <param name="progress">The tracker to hand a handler that asks for one.</param>
    /// <param name="eventData">What the event said about itself, or <see langword="null"/>.</param>
    /// <param name="cancellationToken">Cancels the run.</param>
    /// <returns>One argument per parameter.</returns>
    /// <remarks>
    /// Four parameters are supplied by the framework rather than read from an input, and each is
    /// recognised by its type: a <see cref="Progress"/>, a <see cref="CancellationToken"/>, the
    /// <see cref="Request"/> the run belongs to, and one of the event-data classes — which is
    /// what lets a handler put the event's own payload where it reads best instead of in a fixed
    /// position. Everything else maps to an input positionally, because a .NET delegate carries no
    /// parameter names to match on.
    /// </remarks>
    private object?[] Arguments(
        IReadOnlyList<JsonArray> inputs,
        bool batched,
        Progress progress,
        JsonNode? eventData,
        CancellationToken cancellationToken)
    {
        var handler = _shape.Handler!;
        var parameters = DelegateInvocation.ParametersOf(handler);
        var arguments = new object?[parameters.Length];
        var next = 0;

        for (var index = 0; index < parameters.Length; index++)
        {
            var type = parameters[index].ParameterType;

            if (type == typeof(Progress))
            {
                arguments[index] = progress;
            }
            else if (type == typeof(CancellationToken))
            {
                arguments[index] = cancellationToken;
            }
            else if (type == typeof(Request))
            {
                arguments[index] = _request;
            }
            else if (EventData.Is(type))
            {
                arguments[index] = EventData.Of(type, eventData);
            }
            else if (next < _shape.Inputs.Count)
            {
                arguments[index] = Preprocess(next++, inputs, batched, type);
            }
            else
            {
                arguments[index] = null;
            }
        }

        return arguments;
    }

    /// <summary>One input component's value, preprocessed.</summary>
    /// <param name="index">Which input.</param>
    /// <param name="inputs">The raw values, one array per input.</param>
    /// <param name="batched">Whether to preprocess every sample rather than the one value.</param>
    /// <param name="declared">The type the handler declared for this parameter.</param>
    /// <returns>The handler's argument: a value, or one value per sample for a batched run.</returns>
    /// <remarks>
    /// A batched handler takes an array per input rather than one value per sample per input, which
    /// is how a .NET method spells the Python version's list argument. The array is the one the
    /// handler declared — <c>int[]</c> for a handler that said so — because a delegate can only be
    /// called with the type its parameter names.
    /// </remarks>
    private object? Preprocess(int index, IReadOnlyList<JsonArray> inputs, bool batched, Type declared)
    {
        var block = _shape.Inputs[index];
        var values = index < inputs.Count ? inputs[index] : [];

        if (batched)
        {
            return Samples(block, values, declared);
        }

        return Value(block, values.Count > 0 ? values[0] : null);
    }

    /// <summary>The samples of one input, as the array the handler declared.</summary>
    /// <param name="block">The input component.</param>
    /// <param name="values">What the client sent, one entry per sample.</param>
    /// <param name="declared">The type the handler declared.</param>
    /// <returns>The samples, typed when the declaration names a type they fit.</returns>
    /// <remarks>
    /// A handler that declared <c>object?[]</c> — the shape every batched handler used to have to
    /// take — is handed the samples as they are. A handler that named the element type gets an array
    /// of that type, which is what makes <c>int[]</c> callable. A sample that does not fit the
    /// declared element is left untyped rather than converted: the call then fails with the type it
    /// could not satisfy instead of quietly receiving something else.
    /// </remarks>
    private object Samples(Block block, JsonArray values, Type declared)
    {
        var samples = values.Select(value => Value(block, value)).ToArray();

        if (declared.GetElementType() is not { } element
            || element == typeof(object)
            || samples.Any(sample => sample is not null && !element.IsInstanceOfType(sample)))
        {
            return samples;
        }

        var typed = Array.CreateInstance(element, samples.Length);
        for (var index = 0; index < samples.Length; index++)
        {
            typed.SetValue(samples[index], index);
        }

        return typed;
    }

    /// <summary>One value, as the component that receives it wants it.</summary>
    /// <param name="block">The input component.</param>
    /// <param name="value">What the client sent.</param>
    /// <returns>The handler's argument.</returns>
    /// <remarks>
    /// A stateful component's value is not the client's to send: what the client sent is ignored
    /// and the session's value is read instead, which is the Python version's rule and the whole
    /// point of the component. Everything else is the component's own preprocessing.
    /// </remarks>
    private object? Value(Block block, JsonNode? value)
    {
        if (block is IStatefulComponent stateful)
        {
            return stateful.ReadState(StoredOrCreated(stateful, block.Id));
        }

        return _shape.Preprocess && block is Component component
            ? component.Preprocess(value)
            : JsonValues.ToObject(value);
    }

    /// <summary>The session's value for a stateful component, creating it when the session has none.</summary>
    /// <param name="stateful">The component whose value is read.</param>
    /// <param name="blockId">Its id, which is what the session keys the value by.</param>
    /// <returns>The value the handler reads.</returns>
    /// <remarks>
    /// A value created here is the component's initial one and carries no lifetime: the Python
    /// version dates a value when it is written, not when it is first read.
    /// </remarks>
    private JsonNode? StoredOrCreated(IStatefulComponent stateful, int blockId) =>
        _state.GetOrAdd(blockId, stateful.CreateState);

    /// <summary>What the outputs receive from what the handler returned.</summary>
    /// <param name="values">One value per output, as <see cref="EventResult"/> read them.</param>
    /// <param name="batched">Whether each value is a sequence of per-sample results.</param>
    /// <returns>One entry per output, or one entry per sample for a batched run.</returns>
    private JsonArray Postprocess(IReadOnlyList<object?> values, bool batched) =>
        batched ? PostprocessBatch(values) : PostprocessSingle(values);

    private JsonArray PostprocessSingle(IReadOnlyList<object?> values)
    {
        var outputs = _shape.Outputs;
        var data = new JsonArray();

        for (var index = 0; index < outputs.Count; index++)
        {
            data.Add(PostprocessValue(outputs[index], index < values.Count ? values[index] : null));
        }

        return data;
    }

    private JsonArray PostprocessBatch(IReadOnlyList<object?> values)
    {
        var outputs = _shape.Outputs;
        var columns = new List<object?[]>(outputs.Count);

        for (var index = 0; index < outputs.Count; index++)
        {
            columns.Add(SamplesOf(index < values.Count ? values[index] : null));
        }

        var samples = new JsonArray();
        var count = columns.Count == 0 ? 0 : columns.Max(column => column.Length);

        for (var sample = 0; sample < count; sample++)
        {
            var element = new JsonArray();
            for (var index = 0; index < columns.Count; index++)
            {
                element.Add(PostprocessValue(
                    outputs[index],
                    sample < columns[index].Length ? columns[index][sample] : null));
            }

            samples.Add(element);
        }

        return samples;
    }

    /// <summary>The samples one output's value carries.</summary>
    /// <param name="value">What the handler returned for that output.</param>
    /// <returns>The samples, or nothing when the value is not a sequence.</returns>
    /// <remarks>
    /// The samples are handed on as the handler produced them rather than round-tripped through
    /// JSON, which is how the one-sample path works too: a value that went out to JSON and came back
    /// would be the JSON number rather than the <c>int</c> the endpoint declared, and the output
    /// would refuse to describe a type the handler had already satisfied.
    /// </remarks>
    private static object?[] SamplesOf(object? value) =>
        value is null or string || value is not System.Collections.IEnumerable items
            ? []
            : [.. items.Cast<object?>()];

    /// <summary>Records what the client was shown, so that a share link can open it again.</summary>
    /// <param name="inputs">The values the run read, one array per input.</param>
    /// <param name="data">The values the run produced, one per output.</param>
    /// <param name="batched">Whether the run carried several samples rather than one.</param>
    /// <remarks>
    /// <para>
    /// A state's value is the session's already; everything else belongs to the browser, and what
    /// is remembered here is the last value the server saw it carry — the input of a run and the
    /// output of one. The Python version keeps the same record for the same purpose.
    /// </para>
    /// <para>
    /// A batched run is not remembered: one value per component is what the record is, and a batch is
    /// several. A run with no session has nowhere to put anything, which is every run an example
    /// gallery works out on its own.
    /// </para>
    /// </remarks>
    private void Remember(IReadOnlyList<JsonArray> inputs, JsonArray data, bool batched)
    {
        if (!_shape.Remembers || batched || !_hasSession)
        {
            return;
        }

        for (var index = 0; index < inputs.Count && index < _shape.Inputs.Count; index++)
        {
            _state.Remember(_shape.Inputs[index].Id, inputs[index].Count > 0 ? inputs[index][0] : null);
        }

        for (var index = 0; index < data.Count && index < _shape.Outputs.Count; index++)
        {
            _state.Remember(_shape.Outputs[index].Id, data[index]);
        }
    }

    /// <summary>A batched output's per-sample values.</summary>
    private static JsonArray Sequence(object? value)
    {
        if (value is null or string || value is not System.Collections.IEnumerable items)
        {
            return [];
        }

        var sequence = new JsonArray();
        foreach (var item in items)
        {
            sequence.Add(JsonValues.FromObject(item));
        }

        return sequence;
    }

    /// <summary>One output value, postprocessed by the component that receives it.</summary>
    /// <remarks>
    /// A stateful output is not told to the client: what the handler returned is stored in the
    /// session and the client is sent nothing, which is the Python version's rule — the value
    /// belongs to the server, and a client that received it could only send it back.
    /// </remarks>
    private JsonNode? PostprocessValue(Block output, object? value)
    {
        if (value is FinishedIterating)
        {
            return null;
        }

        if (output is IStatefulComponent stateful)
        {
            // A handler that returns a component is asking for an update rather than handing over a
            // value; the Python version stores the update's `value`, which is what the returned component
            // carries. Returning the state being written to is not an update at all — there is no
            // new value in it — so the session's value is left as it is.
            if (value is Block update)
            {
                if (ReferenceEquals(update, output))
                {
                    return null;
                }

                value = stateful.ValueOfUpdate(update);
            }

            _state.Set(
                output.Id,
                stateful.WriteState(value),
                stateful.StateTimeToLive,
                stateful.DeleteState,
                _clock.GetUtcNow());

            return null;
        }

        if (EventResult.IsComponentUpdate(value))
        {
            return UpdateOf((Block)value!);
        }

        // A handler may also answer with the update itself — the props to merge and the marker that
        // says so — which is the Python version's `gr.update(...)`: a partial update, rather than the whole
        // prop set a component instance carries. A chat needs one to say "empty this field and make
        // it editable again" without also resetting the placeholder the field was drawn with.
        if (value is JsonObject partial && PropertyUpdate.Is(partial))
        {
            return partial.DeepClone();
        }

        // The typed form of the same answer: a handler that returns an <see cref="Update"/> named
        // the props it changed, and the record writes them by the config's own rule.
        if (value is Update typed)
        {
            return typed.ToJson();
        }

        // A streaming output's value is not a value the client is sent: it is a chunk of a stream,
        // and what the client is told is the stream's own file object, which the queue puts in
        // place of this. The handler's value is kept here rather than postprocessed because the
        // postprocessing is exactly what streaming replaces.
        if (output is IStreamingOutput streaming && streaming.Streaming)
        {
            _streamValues[output.Id] = value;

            return null;
        }

        return _shape.Postprocess && output is Component component
            ? component.Postprocess(value)
            : JsonValues.FromObject(value);
    }

    /// <summary>
    /// A component returned as a result, read as an update to the output rather than as a value.
    /// </summary>
    /// <remarks>
    /// Returning the component you want the output to become is one of the three output shapes. The
    /// frontend reads it as the component's props plus the marker that says the props are to be
    /// merged rather than the value replaced.
    /// </remarks>
    private static JsonObject UpdateOf(Block block) => PropertyUpdate.Of(block);

    /// <summary>Adapts a stream to a cursor over boxed values.</summary>
    private static StreamCursor<T> Adapter<T>(IAsyncEnumerable<T> stream, CancellationToken cancellationToken) =>
        new(stream.GetAsyncEnumerator(cancellationToken));

    /// <summary>A cursor over a stream whose element type is not known here.</summary>
    private static IAsyncEnumerator<object?> Cursor(object stream, CancellationToken cancellationToken)
    {
        var element = stream.GetType().GetInterfaces()
            .First(implemented =>
                implemented.IsGenericType
                && implemented.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>))
            .GetGenericArguments()[0];

        return (IAsyncEnumerator<object?>)AdapterFactory
            .MakeGenericMethod(element)
            .Invoke(null, [stream, cancellationToken])!;
    }

    /// <summary>Boxes each element of a stream, so that the run can hold it without knowing its type.</summary>
    private sealed class StreamCursor<T>(IAsyncEnumerator<T> inner) : IAsyncEnumerator<object?>
    {
        public object? Current => inner.Current;

        public ValueTask<bool> MoveNextAsync() => inner.MoveNextAsync();

        public ValueTask DisposeAsync() => inner.DisposeAsync();
    }

    /// <summary>Stands for "the generator produced no more values".</summary>
    private sealed class FinishedIterating
    {
        /// <summary>The one instance; a sentinel is compared by identity.</summary>
        internal static readonly FinishedIterating Instance = new();
    }
}

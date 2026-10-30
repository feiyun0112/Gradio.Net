using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A value the server keeps for one session, across the runs of that session.
/// </summary>
/// <remarks>
/// <para>
/// The component is invisible: it draws nothing, the client is never sent its value, and what a
/// handler returns for it is stored rather than displayed. That is what makes it the way to keep
/// per-user state — a conversation, a counter, a loaded model — without a database and without
/// trusting the client.
/// </para>
/// <para>
/// The value is JSON, because that is the one shape this framework can copy the way the Python
/// version copies a Python object: two sessions reading the same component must not end up sharing
/// one mutable object, so each session gets its own copy, and JSON is what can be copied without
/// the application writing a clone method.
/// </para>
/// </remarks>
[GradioBlock("state")]
[HandlerValue("object?", "change")]
public sealed partial class State : Component, IStatefulComponent
{
    internal State()
    {
    }

    private JsonNode? _value;

    /// <summary>The callback a state that was given none has: it does nothing.</summary>
    /// <remarks>
    /// A default rather than <see langword="null"/>, because the Python version's default is a
    /// function and the config carries the parameter either way: the key exists with a value this
    /// side cannot reproduce, so it is written as null rather than left out (see
    /// <c>PropWriter</c>).
    /// </remarks>
    private static readonly Action<object?> _noDelete = static _ => { };

    /// <summary>The value a session starts from.</summary>
    /// <value>
    /// The initial value, or <see langword="null"/> for a session that starts empty. The Python
    /// version deep-copies what the application passes, so mutating the node afterwards does not
    /// change what a session starts from; this property stores a copy for the same reason.
    /// </value>
    /// <remarks>
    /// Not a prop of the config, which is why it is marked: the Python version deletes <c>value</c>
    /// from a state's props, because the value belongs to the session rather than to the page.
    /// </remarks>
    [OmitFromConfig]
    public JsonNode? Value
    {
        get => _value;
        set => _value = value?.DeepClone();
    }

    /// <summary>How long a value lives after it is set, in seconds.</summary>
    /// <value>
    /// <see cref="double.PositiveInfinity"/> for a value that is kept until the session ends, which
    /// is the Python version's default and its spelling of "no time to live".
    /// </value>
    /// <remarks>
    /// Measured from the moment the value was set, not from the last time the session was touched:
    /// a state that is written and then read for an hour is a state that expired an hour ago, which
    /// is the Python version's rule and the reason the expiry is per component rather than per
    /// session.
    /// </remarks>
    public double TimeToLive { get; set; } = double.PositiveInfinity;

    /// <summary>A callback invoked with a value when the session drops it.</summary>
    /// <value>
    /// A callback taking the value, defaulting to one that does nothing, which is the Python
    /// version's default.
    /// </value>
    /// <remarks>
    /// Called when a value's lifetime elapses, which is the one moment a state's owner learns that
    /// the state is gone: a handler can close a connection or release a resource there instead of
    /// leaving it to a finalizer. Typed rather than a bare <see cref="Delegate"/> so that a
    /// callback of the wrong shape is a compile error rather than an exception on the sweep thread.
    /// </remarks>
    public Action<object?> DeleteCallback { get; set; } = _noDelete;

    /// <inheritdoc />
    /// <value>Always <see langword="true"/>: a state is not something a client can call.</value>
    public override bool SkipApi => true;

    /// <summary>
    /// Whether this block ends a run of siblings that would otherwise be grouped into one container.
    /// </summary>
    /// <value>
    /// <see langword="false"/>, which is the Python version's rule for a state: it is invisible, so
    /// a state written between two textboxes must not split the form those textboxes belong to.
    /// </value>
    protected internal override bool BreaksGrouping => false;

    /// <summary>The typing information of a state.</summary> <value>Any valid JSON, which is what
    /// the Python version declares — a state is never in the API.</value>
    public override JsonObject ApiInfo => new()
    {
        ["type"] = new JsonObject(),
        ["description"] = "any valid json",
    };

    /// <inheritdoc />
    TimeSpan? IStatefulComponent.StateTimeToLive =>
        double.IsPositiveInfinity(TimeToLive) ? null : TimeSpan.FromSeconds(TimeToLive);

    /// <inheritdoc />
    JsonNode? IStatefulComponent.CreateState() => _value?.DeepClone();

    /// <inheritdoc />
    object? IStatefulComponent.ReadState(JsonNode? stored) => JsonValues.ToObject(stored);

    /// <inheritdoc />
    /// <remarks>
    /// <para>
    /// What a handler returns is turned into JSON, because that is the shape the session keeps: an
    /// <c>int</c> and a <c>double</c> that mean the same number must not make two runs disagree
    /// about whether the state changed.
    /// </para>
    /// <para>
    /// The conversion is the general one rather than the framework's narrow mapping, because a
    /// state's value is the application's own shape — a dictionary, a record, an array of whatever
    /// it likes. The narrow mapping covers the JSON scalars and lists of them, which is enough for
    /// a component's value and not enough for a state's.
    /// </para>
    /// </remarks>
    JsonNode? IStatefulComponent.WriteState(object? value) => JsonValues.FromAnyObject(value);

    /// <inheritdoc />
    /// <remarks>
    /// The returned component is read as the value it carries: a state returned with a value is the
    /// same request as returning that value, which is what the Python version makes of
    /// <c>gr.update(value=…)</c> on a state.
    /// </remarks>
    object? IStatefulComponent.ValueOfUpdate(Block update) => (update as State)?.Value;

    /// <inheritdoc />
    /// <remarks>
    /// The callback is handed the value in the shape a handler would read it, which is the shape the
    /// application stored: a callback that closed a connection over an object expects that object,
    /// not the JSON it was kept as.
    /// </remarks>
    void IStatefulComponent.DeleteState(JsonNode? value) => DeleteCallback(JsonValues.ToObject(value));
}

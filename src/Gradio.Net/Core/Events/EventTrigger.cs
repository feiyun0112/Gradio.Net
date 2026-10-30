namespace Gradio.Net;
/// <summary>
/// One thing an event binding listens for: a named signal on a block, or a step that follows
/// another binding.
/// </summary>
/// <param name="Block">
/// The block the signal belongs to, or <see langword="null"/> for a step that follows another
/// binding rather than a block.
/// </param>
/// <param name="EventName">The signal's name, as the Python version spells it — <c>change</c>,
/// <c>click</c>.</param>
/// <remarks>
/// The pair is what the config spells as <c>[block id, event name]</c>, and it is a pair rather
/// than a block with a name attached because the second half of it is <see langword="null"/> for
/// every chained step: there is no block whose signal a <c>then</c> follows.
/// </remarks>
public sealed record EventTarget(Block? Block, string EventName)
{
    /// <summary>The block's id, or <see langword="null"/> when the target is not a block.</summary>
    public int? BlockId => Block?.Id;
}

/// <summary>
/// A named signal, or a set of them, that an event binding listens for
/// </summary>
/// <remarks>
/// <para>
/// A trigger is what a component hands out — <c>textbox.Change</c> — and what an application merges
/// with <c>Blocks.On</c> when one handler should answer several signals.
/// The binding itself is <see cref="EventBinding"/>, kept separate because "which signals"
/// and "what to run" are two decisions: a set of triggers can be assembled and passed around before
/// anything is bound to it.
/// </para>
/// <para>
/// A chained trigger — <see cref="Dependency.Then"/> and its siblings — carries no block, because it
/// follows a binding rather than a signal. It carries the binding it follows instead, and the root
/// it was created in, so that binding to it needs nothing from the caller.
/// </para>
/// </remarks>
public sealed class EventTrigger
{
    private readonly IReadOnlyList<EventTarget> _targets;
    private readonly Blocks? _root;
    private readonly int? _triggerAfter;
    private readonly bool _triggerOnlyOnSuccess;
    private readonly bool _triggerOnlyOnFailure;
    private readonly IReadOnlyList<string>? _eventSpecificArgs;
    private readonly bool _merged;

    /// <summary>Creates a trigger for one signal on one block.</summary> <param name="target">The
    /// block the signal belongs to.</param> <param name="name">The signal's name, as the Python
    /// version spells it.</param> <exception cref="ArgumentNullException"><paramref name="target"/>
    /// is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="name"/> is empty or
    /// whitespace.</exception>
    public EventTrigger(Block target, string name)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        _targets = [new EventTarget(target, name)];
    }

    private EventTrigger(
        IReadOnlyList<EventTarget> targets,
        Blocks? root,
        int? triggerAfter,
        bool triggerOnlyOnSuccess,
        bool triggerOnlyOnFailure,
        IReadOnlyList<string>? eventSpecificArgs,
        bool merged)
    {
        _targets = targets;
        _root = root;
        _triggerAfter = triggerAfter;
        _triggerOnlyOnSuccess = triggerOnlyOnSuccess;
        _triggerOnlyOnFailure = triggerOnlyOnFailure;
        _eventSpecificArgs = eventSpecificArgs;
        _merged = merged;
    }

    /// <summary>The signals this trigger listens for, in the order they were given.</summary>
    public IReadOnlyList<EventTarget> Targets => _targets;

    /// <summary>
    /// The root this trigger belongs to, when it was created by something that already knew.
    /// </summary>
    /// <remarks>
    /// A chained trigger knows its root because the binding it follows did. A trigger taken from a
    /// component does not, and the binding resolves it by walking up from the block.
    /// </remarks>
    internal Blocks? Root => _root;

    /// <summary>The binding this one follows, or <see langword="null"/> for one that follows a signal.</summary>
    internal int? TriggerAfter => _triggerAfter;

    /// <summary>Whether this step runs only when the binding it follows succeeded.</summary>
    internal bool TriggerOnlyOnSuccess => _triggerOnlyOnSuccess;

    /// <summary>Whether this step runs only when the binding it follows failed.</summary>
    internal bool TriggerOnlyOnFailure => _triggerOnlyOnFailure;

    /// <summary>
    /// The event-specific argument names this trigger contributes, or <see langword="null"/> when it
    /// contributes none.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Python version carries these on the event itself, and merges them across a trigger set.
    /// The distinction between <see langword="null"/> and an empty list is the Python version's and
    /// is visible in the config: a set assembled by <c>gr.on</c> reports an empty list, a single
    /// component event reports nothing.
    /// </para>
    /// <para>
    /// No event this batch implements contributes any, so the only non-<see langword="null"/> value
    /// reachable here is the empty list a merge produces. The field exists because the config has it
    /// and the comparison is field by field.
    /// </para>
    /// </remarks>
    internal IReadOnlyList<string>? EventSpecificArgs => _eventSpecificArgs;

    /// <summary>
    /// Whether this trigger came from merging a set of triggers rather than from one component.
    /// </summary>
    /// <remarks>
    /// The Python version carries an event-specific argument list on the event itself, and merging
    /// them is what makes the list exist: a binding to a component's own signal reports no list,
    /// and one assembled by <c>gr.on</c> reports a list even when it is empty. The distinction is
    /// visible in the config, so it has to survive the trip from the trigger to the binding.
    /// </remarks>
    internal bool IsMerged => _merged;

    /// <summary>
    /// The same trigger, marked as one that was assembled rather than taken from a component.
    /// </summary>
    /// <returns>The trigger.</returns>
    /// <remarks>
    /// The signals and the binding a chained step follows are kept as they are: what merging adds is
    /// the event-specific argument list, and a chained step that was bound through
    /// <c>Blocks.On</c> carries both — the step it follows and the list a merged set reports.
    /// </remarks>
    internal EventTrigger AsMerged() =>
        new(
            _targets,
            _root,
            _triggerAfter,
            _triggerOnlyOnSuccess,
            _triggerOnlyOnFailure,
            _eventSpecificArgs,
            merged: true);

    /// <summary>Creates the trigger for a step that follows a binding.</summary>
    /// <param name="root">The root the binding belongs to.</param> <param name="after">The binding
    /// the step follows.</param> <param name="name">The step's name, as the Python version spells
    /// it — <c>then</c>, <c>success</c>.</param> <param name="onlyOnSuccess">Whether the step runs
    /// only when the binding it follows succeeded.</param> <param name="onlyOnFailure">Whether the
    /// step runs only when the binding it follows failed.</param> <returns>The trigger.</returns>
    internal static EventTrigger Chained(
        Blocks root,
        int after,
        string name,
        bool onlyOnSuccess,
        bool onlyOnFailure) =>
        new(
            [new EventTarget(Block: null, name)],
            root,
            after,
            onlyOnSuccess,
            onlyOnFailure,
            eventSpecificArgs: null,
            merged: false);

    /// <summary>
    /// The render root this trigger's binding belongs to.
    /// </summary>
    /// <param name="inputs">The binding's inputs, read when no target names a root.</param>
    /// <param name="outputs">The binding's outputs, read when no target names a root.</param>
    /// <returns>The root.</returns>
    /// <exception cref="InvalidOperationException">
    /// No target, input or output belongs to a render root, so the binding has nowhere to live.
    /// </exception>
    /// <remarks>
    /// A trigger taken from a component is resolved by walking up from that component, which is what
    /// keeps a binding from needing a container: the component already knows which application it
    /// was rendered into.
    /// </remarks>
    internal Blocks ResolveRoot(IReadOnlyList<Block> inputs, IReadOnlyList<Block> outputs)
    {
        var root = _root
            ?? FirstRoot(_targets.Select(target => target.Block))
            ?? FirstRoot(inputs)
            ?? FirstRoot(outputs);

        return root ?? throw new InvalidOperationException(
            "An event binding has to belong to an application. Bind it on a block that was added "
            + "to a Blocks, or merge its triggers with Blocks.On first.");
    }

    private static Blocks? FirstRoot(IEnumerable<Block?> blocks)
    {
        foreach (var block in blocks)
        {
            if (block?.Root is { } root)
            {
                return root;
            }
        }

        return null;
    }
}

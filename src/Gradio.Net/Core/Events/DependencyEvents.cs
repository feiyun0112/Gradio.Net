namespace Gradio.Net;
/// <summary>
/// The steps that follow a binding, as values an application can assemble and bind
/// </summary>
/// <remarks>
/// <para>
/// A chained step is a binding whose trigger is not a block's signal but the end of another
/// binding: its target in the config is the pair <c>[null, "then"]</c>, and its
/// <c>trigger_after</c> is the id of the binding it follows. That is how the Python version
/// expresses "run this second".
/// </para>
/// <para>
/// The binding methods on <see cref="Dependency"/> are the ordinary way to use them —
/// <c>dep.Then(fn, ...)</c> — and these properties are the same three steps as values, for an
/// application that hands them to <c>Blocks.On</c> instead. They live here for the reason a
/// component's triggers live under <c>Events</c>: the binding took the name.
/// </para>
/// </remarks>
public sealed class DependencyEvents
{
    private readonly Dependency _dependency;

    /// <summary>Creates the trigger set of a binding.</summary>
    /// <param name="dependency">The binding the steps follow.</param>
    /// <exception cref="ArgumentNullException"><paramref name="dependency"/> is <see langword="null"/>.</exception>
    internal DependencyEvents(Dependency dependency)
    {
        ArgumentNullException.ThrowIfNull(dependency);

        _dependency = dependency;
    }

    /// <summary>A step that runs after the binding, whatever its outcome.</summary>
    public EventTrigger Then => _dependency.ChainedTrigger("then", onlyOnSuccess: false, onlyOnFailure: false);

    /// <summary>A step that runs after the binding, only when it succeeded.</summary>
    public EventTrigger Success => _dependency.ChainedTrigger("success", onlyOnSuccess: true, onlyOnFailure: false);

    /// <summary>A step that runs after the binding, only when it failed.</summary>
    public EventTrigger Failure => _dependency.ChainedTrigger("failure", onlyOnSuccess: false, onlyOnFailure: true);
}

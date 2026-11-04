namespace Gradio.Net;
/// <summary>
/// The channel a handler's <c>gr.Info(...)</c> reaches the page through.
/// </summary>
/// <remarks>
/// <para>
/// A callout is written by a static method, so it cannot be handed to a handler the way
/// <see cref="Progress"/> is handed in as a parameter: nothing names it. What carries it instead is
/// the ambient scope of the run the handler is executing in, set by whoever started that run and
/// read by the static methods that want to say something.
/// </para>
/// <para>
/// The scope flows with the async context, so a handler that awaits still finds it — which is the
/// same mechanism an ambient render scope uses, and the reason this needs no parameter.
/// </para>
/// <para>
/// Outside a run there is nothing to say anything to, and a callout there does nothing. That is the
/// Python version's behaviour for a handler also called directly: the sentence is dropped rather
/// than raised, because a handler should not have to know which way it was called.
/// </para>
/// </remarks>
internal static class Callouts
{
    private static readonly AsyncLocal<Action<string, string, double?>?> _sink = new();

    /// <summary>Makes <paramref name="sink"/> the destination for the rest of the flow.</summary>
    /// <param name="sink">Where a callout goes, or <see langword="null"/> to drop them.</param>
    /// <returns>A scope that restores the previous destination when it is disposed.</returns>
    internal static IDisposable Begin(Action<string, string, double?>? sink)
    {
        var previous = _sink.Value;
        _sink.Value = sink;

        return new Scope(previous);
    }

    /// <summary>Says something to whoever is listening.</summary>
    /// <param name="level">What kind of line it is.</param>
    /// <param name="message">The text.</param>
    /// <param name="duration">How long the page shows it, or <see langword="null"/> for its own
    /// default.</param>
    internal static void Post(string level, string message, double? duration) =>
        _sink.Value?.Invoke(level, message, duration);

    private sealed class Scope(Action<string, string, double?>? previous) : IDisposable
    {
        public void Dispose() => _sink.Value = previous;
    }
}

namespace Gradio.Net;
/// <summary>
/// How many runs of one event binding may be in flight at once.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>concurrency_limit</c> has three states rather than two — "use the
/// application's default", "at most n", and "no limit" — so a nullable integer cannot carry it:
/// <see langword="null"/> already means the first. The third state is what makes the distinction
/// worth a type rather than a comment.
/// </para>
/// <para>
/// The value is not part of the config: it is read by the queue when a run is admitted, which is why
/// it lives on the binding rather than in the served document.
/// </para>
/// </remarks>
public readonly record struct ConcurrencyLimit
{
    private ConcurrencyLimit(int? value) => Value = value;

    /// <summary>The application's default limit is in force — the Python version's
    /// <c>"default"</c>.</summary>
    public static ConcurrencyLimit ApplicationDefault => new(null);

    /// <summary>Any number of runs may be in flight — the Python version's <c>None</c>.</summary>
    public static ConcurrencyLimit Unlimited => new(-1);

    /// <summary>
    /// The limit, <c>-1</c> for <see cref="Unlimited"/>, or <see langword="null"/> for
    /// <see cref="ApplicationDefault"/>.
    /// </summary>
    public int? Value { get; }

    /// <summary>At most <paramref name="runs"/> runs of the binding may be in flight at once.</summary>
    /// <param name="runs">The number of runs, which must be positive.</param>
    /// <returns>The limit.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="runs"/> is not positive.</exception>
    public static ConcurrencyLimit Of(int runs)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(runs, 1);

        return new ConcurrencyLimit(runs);
    }
}

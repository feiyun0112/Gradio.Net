namespace Gradio.Net;
public static partial class gr
{
    /// <summary>Schedules the application's runs, the way the Python version's <c>demo.queue()</c>
    /// does.</summary> <param name="app">The application whose queue is being configured.</param>
    /// <param name="statusUpdateRate">
    /// How often the queue re-describes itself, in seconds, or the string <c>"auto"</c> — or
    /// <see langword="null"/> for the same, which is the Python version's default.
    /// </param>
    /// <param name="apiOpen">Whether the direct call routes are open, or <see langword="null"/> to leave the setting as it is.</param>
    /// <param name="maxSize">The most events the queue holds, or <see langword="null"/> for no limit.</param>
    /// <param name="defaultConcurrencyLimit">
    /// The limit a binding that names none runs under: a number, the string <c>"not_set"</c>, or
    /// <see langword="null"/> for the Python version's default —
    /// <c>GRADIO_DEFAULT_CONCURRENCY_LIMIT</c>, or one run at a time when that is unset. The Python
    /// version's "no limit" is <see cref="ConcurrencyLimit.Unlimited"/>, which is a value rather
    /// than an omission.
    /// </param>
    /// <returns>The application, so that the call can be chained.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="app"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// <para>
    /// This is the Python version's <c>queue</c> with the Python version's parameter names, over a
    /// core member that spells the same four settings the C# way (<see cref="Blocks.Queue"/>).
    /// The two do not collide: a call that names one of the Python version's parameters is not
    /// applicable to the core member, so it resolves here, and a call that names the core's — or
    /// names nothing — resolves there. That is the "two spellings coexist, the extension yields"
    /// arrangement this layer is built on, and it is why this method forwards rather than
    /// reimplements: the day the core grows a member with these names, every call here resolves to
    /// it and this method is simply unused.
    /// </para>
    /// <para>
    /// The two parameters the Python version types as a union are read the way the Python version
    /// reads them: <c>statusUpdateRate</c> is a number of seconds or the word <c>"auto"</c>, and
    /// <c>defaultConcurrencyLimit</c> is a number or the word <c>"not_set"</c>. Nothing is
    /// decided here — the values are translated and handed on.
    /// </para>
    /// </remarks>
    public static Blocks Queue(
        this Blocks app,
        object? statusUpdateRate = null,
        bool? apiOpen = null,
        int? maxSize = null,
        object? defaultConcurrencyLimit = null)
    {
        ArgumentNullException.ThrowIfNull(app);

        return app.Queue(
            statusUpdateRate: Values.Seconds(statusUpdateRate),
            apiOpen: apiOpen,
            maxSize: maxSize,
            defaultConcurrencyLimit: Values.Concurrency(defaultConcurrencyLimit));
    }
}

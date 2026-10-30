namespace Gradio.Net;
/// <summary>
/// Whether this application collects profiling traces, and therefore whether it offers the routes
/// that read them.
/// </summary>
/// <remarks>
/// <para>
/// The Python version decides this once, when its module is imported, from <c>GRADIO_PROFILING</c>
/// — and the decision is not only about cost: the three profiling routes exist only when it is on,
/// so an application that does not collect traces does not answer them at all. That is why this is
/// an option the container holds rather than a flag a request checks.
/// </para>
/// <para>
/// The default is off, which is the Python version's: tracing every run costs a timestamp per
/// phase, and an application that wants the traces says so.
/// </para>
/// </remarks>
public sealed class ProfilingOptions
{
    /// <summary>Whether traces are collected.</summary>
    /// <value>
    /// <see langword="true"/> only when <c>GRADIO_PROFILING</c> says <c>1</c> or <c>true</c>, which
    /// is the pair of spellings the Python version accepts.
    /// </value>
    /// <remarks>
    /// Settable where the container is built and not afterwards: the decision is what decides
    /// whether the three routes exist at all, so an application that flipped it later would have
    /// traces nobody could read, or routes with nothing behind them.
    /// </remarks>
    public bool Enabled { get; init; } = EnabledFromEnvironment();

    /// <summary>Whether the environment turns profiling on.</summary>
    /// <returns><see langword="true"/> only for the two spellings the Python version
    /// accepts.</returns>
    /// <remarks>
    /// The comparison is exact rather than case-insensitive: <c>TRUE</c> is not one of the values
    /// the Python version recognises, and reading it as one here would make an application that is
    /// profiled in one implementation and not in the other.
    /// </remarks>
    private static bool EnabledFromEnvironment()
    {
        var configured = Environment.GetEnvironmentVariable("GRADIO_PROFILING")?.Trim();

        return configured is "1" or "true";
    }
}

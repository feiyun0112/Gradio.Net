namespace Gradio.Net.Tests.Server;

/// <summary>
/// Sets an environment variable for the length of a test, and puts back whatever was there.
/// </summary>
/// <remarks>
/// <para>
/// The variables this package reads are process-wide and are read once — when a server is built, or
/// when a <see cref="Blocks"/> is — so a test that wants one of them set has to set
/// it before the thing that reads it is built, and restore it afterwards. The restore is the part
/// that is easy to forget: a leaked value would make the next test read something nobody asked for,
/// which is a failure somewhere else entirely.
/// </para>
/// <para>
/// Putting it back is not enough on its own, because the window is process-wide and every host any
/// other test builds inside it reads the same value. A collection that sets one of these therefore
/// declares <see cref="Xunit.CollectionDefinitionAttribute.DisableParallelization"/>, so the rest of
/// the suite waits for it rather than reading what it set.
/// </para>
/// </remarks>
internal sealed class EnvironmentVariable : IDisposable
{
    private readonly string _name;
    private readonly string? _previous;

    private EnvironmentVariable(string name, string? previous)
    {
        _name = name;
        _previous = previous;
    }

    /// <summary>Sets a variable.</summary>
    /// <param name="name">Its name.</param>
    /// <param name="value">What to set it to.</param>
    /// <returns>What puts the previous value back.</returns>
    internal static EnvironmentVariable Set(string name, string value)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentNullException.ThrowIfNull(value);

        var previous = Environment.GetEnvironmentVariable(name);

        Environment.SetEnvironmentVariable(name, value);

        return new EnvironmentVariable(name, previous);
    }

    public void Dispose() => Environment.SetEnvironmentVariable(_name, _previous);
}

namespace Gradio.Net;
/// <summary>
/// Whether server-side rendering is on, and which Node runtime would render it.
/// </summary>
/// <remarks>
/// <para>
/// The Python version resolves the same two settings in the same order: the argument to
/// <c>launch</c> wins, then the environment variable, then off. Keeping the order is what lets an
/// app be turned over to SSR without recompiling, which is how a container image is usually
/// deployed.
/// </para>
/// <para>
/// Off is the default, and off means nothing about Node is touched: no path is resolved, no
/// process is started, and the page is served from the .NET template exactly as it is when SSR
/// does not exist.
/// </para>
/// </remarks>
internal sealed class SsrOptions
{
    /// <summary>The Python version's environment variable for the mode.</summary>
    internal const string ModeVariable = "GRADIO_SSR_MODE";

    /// <summary>The Python version's environment variable for the runtime.</summary>
    internal const string NodePathVariable = "GRADIO_NODE_PATH";

    /// <summary>Holds a resolved pair; <see cref="Resolve"/> is the only way to get one.</summary>
    /// <param name="enabled">Whether the page should be rendered by the SSR runtime.</param>
    /// <param name="nodePath">The runtime the caller named, or <see langword="null"/>.</param>
    private SsrOptions(bool enabled, string? nodePath)
    {
        Enabled = enabled;
        NodePath = nodePath;
    }

    /// <summary>Whether the page should be rendered by the SSR runtime.</summary>
    internal bool Enabled { get; }

    /// <summary>The runtime the caller named, or <see langword="null"/> to look for one.</summary>
    internal string? NodePath { get; }

    /// <summary>Resolves the settings for a host.</summary>
    /// <param name="options">What the entry point was configured with.</param>
    /// <returns>The resolved settings; <see cref="Enabled"/> is <see langword="false"/> by default.</returns>
    internal static SsrOptions Resolve(GradioServerOptions options)
    {
        return new SsrOptions(
            options.SsrMode ?? ReadModeFromEnvironment(),
            options.SsrNodePath ?? ReadFromEnvironment(NodePathVariable));
    }

    private static bool ReadModeFromEnvironment()
    {
        // The Python version compares the lowercased value to "true", so "True" and "TRUE" both count.
        return string.Equals(
            Environment.GetEnvironmentVariable(ModeVariable),
            "true",
            StringComparison.OrdinalIgnoreCase);
    }

    private static string? ReadFromEnvironment(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);

        return string.IsNullOrWhiteSpace(value) ? null : value;
    }
}

using System.Diagnostics;

namespace Gradio.Net;
/// <summary>
/// Finds the Node runtime an application brings with it, and refuses one that is too old to run
/// the vendored SSR server.
/// </summary>
/// <remarks>
/// <para>
/// Node is never a dependency of this package: an application that leaves SSR off never calls
/// this, and one that turns it on is told exactly what to install and where to point when it is
/// missing. The Python version asks for Node 20 or newer for the same reason - the compiled
/// server it ships is a modern ES module.
/// </para>
/// <para>
/// The version is read by running the runtime rather than by trusting the path, because a path
/// can name anything: a shell script, an old binary, a broken shim. Running it once at start-up
/// turns "the page 502s" into a message that names the version that was found.
/// </para>
/// </remarks>
internal static class NodeRuntime
{
    /// <summary>The oldest major version the vendored SSR server runs on, as the Python version
    /// requires.</summary>
    internal const int MinimumMajorVersion = 20;

    private const string ExecutableName = "node";

    /// <summary>How the messages name the setting a caller can point at a runtime.</summary>
    private const string SettingName =
        nameof(GradioServerOptions) + "." + nameof(GradioServerOptions.SsrNodePath);

    private static readonly TimeSpan _versionTimeout = TimeSpan.FromSeconds(10);

    /// <summary>Resolves the runtime to start, or explains why there is none.</summary>
    /// <param name="configured">
    /// The path the caller named, or <see langword="null"/> to look for one on <c>PATH</c>.
    /// </param>
    /// <returns>The absolute path of a runtime that reports a usable version.</returns>
    /// <exception cref="InvalidOperationException">
    /// No runtime was found, the one that was found does not run, or it is older than
    /// <see cref="MinimumMajorVersion"/>.
    /// </exception>
    internal static string Locate(string? configured)
    {
        var path = configured is null ? OnPath() : Named(configured);

        var version = VersionOf(path);
        if (version.Major < MinimumMajorVersion)
        {
            throw new InvalidOperationException(
                $"Server-side rendering needs Node {MinimumMajorVersion} or newer, but '{path}' reports "
                + $"{version}. Install a newer Node, point {SettingName} (or "
                + $"{SsrOptions.NodePathVariable}) at one, or leave SSR off and serve the page from the "
                + ".NET server instead.");
        }

        return path;
    }

    /// <summary>The absolute path of the runtime a caller named, checked to exist.</summary>
    /// <param name="configured">A path, absolute or relative to the working directory.</param>
    /// <returns>The absolute path.</returns>
    /// <exception cref="InvalidOperationException">Nothing exists at that path.</exception>
    private static string Named(string configured)
    {
        var path = Path.GetFullPath(configured);

        if (!System.IO.File.Exists(path))
        {
            throw new InvalidOperationException(
                $"Server-side rendering was configured with the Node path '{configured}', but nothing "
                + $"exists there. Point {SettingName} (or "
                + $"{SsrOptions.NodePathVariable}) at a Node {MinimumMajorVersion}+ executable, or leave "
                + "SSR off and serve the page from the .NET server instead.");
        }

        return path;
    }

    /// <summary>The first runtime named <c>node</c> on <c>PATH</c>, checked to exist.</summary>
    /// <returns>The absolute path.</returns>
    /// <exception cref="InvalidOperationException">No runtime is on <c>PATH</c>.</exception>
    private static string OnPath()
    {
        var names = OperatingSystem.IsWindows()
            ? new[] { ExecutableName + ".exe", ExecutableName + ".cmd", ExecutableName }
            : new[] { ExecutableName };

        foreach (var directory in (Environment.GetEnvironmentVariable("PATH") ?? string.Empty)
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            foreach (var name in names)
            {
                var candidate = Path.Combine(directory, name);
                if (System.IO.File.Exists(candidate))
                {
                    return candidate;
                }
            }
        }

        throw new InvalidOperationException(
            $"Server-side rendering needs a Node {MinimumMajorVersion}+ runtime and none was found on "
            + $"PATH. Install Node {MinimumMajorVersion} or newer, or point "
            + $"{SettingName} (or {SsrOptions.NodePathVariable}) at one. SSR is "
            + "off by default: leave it off and the page is served from the .NET server, which needs no "
            + "Node at all.");
    }

    /// <summary>Runs the runtime's <c>--version</c> and parses what it reports.</summary>
    /// <param name="path">The runtime to run.</param>
    /// <returns>The version it reports.</returns>
    /// <exception cref="InvalidOperationException">
    /// The runtime could not be started, or reported something that is not a version.
    /// </exception>
    private static Version VersionOf(string path)
    {
        var startInfo = new ProcessStartInfo(path)
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };

        startInfo.ArgumentList.Add("--version");

        string reported;
        try
        {
            using var process = Process.Start(startInfo)
                ?? throw new InvalidOperationException($"'{path}' could not be started.");

            using var cancellation = new CancellationTokenSource(_versionTimeout);
            var output = process.StandardOutput.ReadToEndAsync(cancellation.Token);

            if (!process.WaitForExit(_versionTimeout))
            {
                process.Kill(entireProcessTree: true);
                throw new InvalidOperationException(
                    $"'{path}' did not answer --version within {_versionTimeout.TotalSeconds:0} seconds.");
            }

            reported = output.GetAwaiter().GetResult().Trim();
        }
        catch (System.ComponentModel.Win32Exception error)
        {
            throw new InvalidOperationException(
                $"Server-side rendering needs Node {MinimumMajorVersion}+, but '{path}' could not be run: "
                + $"{error.Message}",
                error);
        }

        // Node prints "v20.11.1"; anything else is not a runtime this package knows how to use.
        var text = reported.StartsWith('v') ? reported[1..] : reported;
        var dash = text.IndexOf('-', StringComparison.Ordinal);
        if (dash >= 0)
        {
            text = text[..dash];
        }

        return Version.TryParse(text, out var version)
            ? version
            : throw new InvalidOperationException(
                $"Server-side rendering needs Node {MinimumMajorVersion}+, but '{path} --version' reported "
                + $"'{reported}', which is not a version.");
    }
}

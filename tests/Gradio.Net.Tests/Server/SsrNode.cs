using System.Diagnostics;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// Finds the Node runtime the SSR tests need, or says why there is none.
/// </summary>
/// <remarks>
/// Deliberately a second implementation of the discovery the package does: a test that asked the
/// code under test whether it can run would pass by agreeing with itself. Where no runtime is
/// found the tests skip instead of failing - SSR is an opt-in capability, and a machine without
/// Node is a machine that cannot exercise it.
/// </remarks>
internal static class SsrNode
{
    /// <summary>The runtime to test with, or <see langword="null"/> when this machine has none.</summary>
    /// <returns>An absolute path to a Node 20+ executable.</returns>
    internal static string? Locate()
    {
        var configured = Environment.GetEnvironmentVariable("GRADIO_NODE_PATH");
        var candidates = string.IsNullOrWhiteSpace(configured)
            ? OnPath()
            : [Path.GetFullPath(configured)];

        return candidates.FirstOrDefault(candidate => Major(candidate) >= 20);
    }

    /// <summary>A path no runtime is at, for the tests that need one to be missing.</summary>
    /// <returns>An absolute path under the temporary directory that nothing creates.</returns>
    /// <remarks>
    /// Named rather than created: what those tests need is a path the package will look for and not
    /// find, so the one thing the path must not be is real.
    /// </remarks>
    internal static string NoSuchRuntimePath() =>
        Path.Combine(Path.GetTempPath(), "gradio-net-no-such-node-" + Guid.NewGuid().ToString("N"), "node");

    private static IEnumerable<string> OnPath()
    {
        var names = OperatingSystem.IsWindows()
            ? new[] { "node.exe", "node.cmd", "node" }
            : new[] { "node" };

        foreach (var directory in (Environment.GetEnvironmentVariable("PATH") ?? string.Empty)
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            foreach (var name in names)
            {
                var candidate = Path.Combine(directory, name);
                if (System.IO.File.Exists(candidate))
                {
                    yield return candidate;
                }
            }
        }
    }

    /// <summary>The major version a runtime reports, or 0 when it cannot be run at all.</summary>
    /// <param name="path">The runtime to ask.</param>
    /// <returns>The major version.</returns>
    private static int Major(string path)
    {
        try
        {
            using var process = Process.Start(new ProcessStartInfo(path)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                ArgumentList = { "--version" },
            });

            if (process is null)
            {
                return 0;
            }

            using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var output = process.StandardOutput.ReadToEndAsync(cancellation.Token);

            if (!process.WaitForExit(TimeSpan.FromSeconds(10)))
            {
                process.Kill(entireProcessTree: true);

                return 0;
            }

            var reported = output.GetAwaiter().GetResult().Trim().TrimStart('v');
            var dot = reported.IndexOf('.', StringComparison.Ordinal);

            return int.TryParse(dot < 0 ? reported : reported[..dot], out var major) ? major : 0;
        }
        catch (Exception error) when (error is System.ComponentModel.Win32Exception or InvalidOperationException)
        {
            return 0;
        }
    }
}

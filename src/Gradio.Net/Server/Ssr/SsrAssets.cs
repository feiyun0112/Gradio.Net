namespace Gradio.Net;
/// <summary>
/// The vendored SSR runtime, read out of this assembly and written where Node can load it.
/// </summary>
/// <remarks>
/// <para>
/// Node loads modules from a file, not from an assembly, so the one thing this class exists for
/// is turning embedded resources back into a directory. That is also why the tree is embedded at
/// all: a package reference, a <c>publish</c> and a single-file deployment all carry it, and an
/// application that turns SSR off never pays for the extraction.
/// </para>
/// <para>
/// The layout is the Python version's, unchanged: <c>register.mjs</c> beside <c>node/build</c>. The
/// register hook resolves relative to itself, so the tree is written as it is rather than
/// rearranged into something tidier.
/// </para>
/// <para>
/// Every extraction gets a directory of its own and the proxy that asked for it deletes it on
/// disposal. Sharing one directory between servers would need a reference count to know when it
/// is safe to remove, and a leftover directory in the temporary path is cheaper than that.
/// </para>
/// </remarks>
internal static class SsrAssets
{
    /// <summary>The logical-name prefix the build embeds the tree under.</summary>
    internal const string Prefix = "Gradio.Net.ssr/";

    /// <summary>The entry point the runtime is started with, relative to the tree root.</summary>
    internal const string RegisterModule = "register.mjs";

    /// <summary>The compiled server the register hook loads, relative to the tree root.</summary>
    internal const string ServerDirectory = "node/build";

    /// <summary>Every file of the tree, relative to the tree root, with <c>/</c> separators.</summary>
    /// <returns>The paths, in the order the assembly lists them.</returns>
    internal static IReadOnlyList<string> Paths()
    {
        return typeof(SsrAssets).Assembly
            .GetManifestResourceNames()
            .Where(name => name.StartsWith(Prefix, StringComparison.Ordinal))
            .Select(name => name[Prefix.Length..])
            .ToArray();
    }

    /// <summary>Opens one file of the tree.</summary>
    /// <param name="path">A path returned by <see cref="Paths"/>.</param>
    /// <returns>The file's bytes, as a stream the caller disposes.</returns>
    /// <exception cref="InvalidOperationException">The assembly carries no such file.</exception>
    internal static Stream Open(string path)
    {
        return typeof(SsrAssets).Assembly.GetManifestResourceStream(Prefix + path)
            ?? throw new InvalidOperationException(
                $"The assembly carries no '{Prefix + path}'. The vendored SSR tree is embedded at "
                + "build time; a build that dropped it cannot render a page server-side.");
    }

    /// <summary>Writes the tree to a new directory under the temporary path.</summary>
    /// <returns>The directory the tree was written to, which the caller owns and deletes.</returns>
    /// <exception cref="IOException">A file could not be written.</exception>
    /// <exception cref="UnauthorizedAccessException">The temporary path is not writable.</exception>
    internal static string ExtractToTemporaryDirectory()
    {
        var directory = Path.Combine(
            Path.GetTempPath(),
            "gradio-net-ssr",
            Guid.NewGuid().ToString("N"));

        try
        {
            foreach (var path in Paths())
            {
                var target = Path.Combine(directory, path.Replace('/', Path.DirectorySeparatorChar));
                Directory.CreateDirectory(Path.GetDirectoryName(target)!);

                using var source = Open(path);
                using var destination = new FileStream(target, FileMode.Create, FileAccess.Write);
                source.CopyTo(destination);
            }

            return directory;
        }
        catch
        {
            // A half-written tree is no use to the runtime, and leaving it behind would make the
            // failure look like a disk-space problem on the next start.
            TryDelete(directory);

            throw;
        }
    }

    /// <summary>Removes an extracted tree, if it is still there.</summary>
    /// <param name="directory">A directory <see cref="ExtractToTemporaryDirectory"/> returned.</param>
    /// <remarks>
    /// Deletion is best effort: a file the runtime still holds open leaves the tree behind in the
    /// temporary path, and that is not a reason for a caller's disposal to fail.
    /// </remarks>
    internal static void TryDelete(string directory)
    {
        try
        {
            Directory.Delete(directory, recursive: true);
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or DirectoryNotFoundException)
        {
            // Nothing to delete, or nothing this can do about it.
        }
    }
}

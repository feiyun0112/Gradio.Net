namespace Gradio.Net.Tests;

/// <summary>
/// Walks the checked-in tree, so that tests can assert on what the repository declares rather than
/// only on what the build produced.
/// </summary>
/// <remarks>
/// The same walk serves the version tests (which read MSBuild files) and the Python-declaration
/// test (which reads JSON), and both need the same exclusions: build output is not checked in, and
/// a directory starting with a dot is a tool's, not the repository's. What differs between them -
/// the vendored trees, which are the Python version's artifacts and carry the Python version's own
/// files - is passed in.
/// </remarks>
internal static class RepositoryTree
{
    private static readonly string[] _buildOutput = ["bin", "obj", "artifacts"];

    /// <summary>
    /// Every checked-in file whose name matches one of <paramref name="patterns"/>.
    /// </summary>
    /// <param name="patterns">Glob patterns, as <see cref="Directory.EnumerateFiles(string, string)"/> takes them.</param>
    /// <param name="skippedSubtrees">Repository-relative directories to leave out entirely.</param>
    internal static IEnumerable<string> EnumerateFiles(IEnumerable<string> patterns, IEnumerable<string> skippedSubtrees)
    {
        var patternList = patterns.ToArray();
        var skipped = skippedSubtrees
            .Select(subtree => subtree.Replace('\\', '/'))
            .ToArray();

        var pending = new Queue<string>();
        pending.Enqueue(RepositoryRoot.FullPath);

        while (pending.Count > 0)
        {
            var directory = pending.Dequeue();

            foreach (var child in Directory.EnumerateDirectories(directory))
            {
                var name = Path.GetFileName(child);

                if (name.StartsWith('.') || _buildOutput.Contains(name, StringComparer.Ordinal) || IsSkipped(child, skipped))
                {
                    continue;
                }

                pending.Enqueue(child);
            }

            foreach (var pattern in patternList)
            {
                foreach (var file in Directory.EnumerateFiles(directory, pattern))
                {
                    yield return file;
                }
            }
        }
    }

    private static bool IsSkipped(string directory, string[] skippedSubtrees)
    {
        if (skippedSubtrees.Length == 0)
        {
            return false;
        }

        var relative = Path.GetRelativePath(RepositoryRoot.FullPath, directory).Replace('\\', '/');

        return skippedSubtrees.Contains(relative, StringComparer.Ordinal);
    }
}

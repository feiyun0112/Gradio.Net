namespace Gradio.Net.Tests;

/// <summary>
/// Locates the repository root from the test process, so that tests can assert on the
/// build inputs (the version source, the MSBuild files) as well as on the build output.
/// </summary>
internal static class RepositoryRoot
{
    private static readonly Lazy<string> _fullPath = new(Locate);

    /// <summary>The absolute path of the repository root.</summary>
    internal static string FullPath => _fullPath.Value;

    private static string Locate()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (System.IO.File.Exists(System.IO.Path.Combine(directory.FullName, "Gradio.Net.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException(
            $"Could not find Gradio.Net.sln above '{AppContext.BaseDirectory}'.");
    }
}

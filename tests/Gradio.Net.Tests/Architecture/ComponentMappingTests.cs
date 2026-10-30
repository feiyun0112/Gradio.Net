using Xunit;

namespace Gradio.Net.Tests.Architecture;

/// <summary>
/// A component describes itself with the mapping the source generator wrote from the Python
/// parameter manifest, not by reading its own members while the application runs.
/// </summary>
/// <remarks>
/// <para>
/// The generated mapping is the mechanism and reflection is the thing it replaced. The compiler
/// enforces half of the rule — a block whose the Python version constructor has a fixed parameter
/// list cannot write its own <c>WriteProps</c>, because the generated method is already there — but
/// the hand-written hooks beside it (<c>WriteExtraProps</c>, <c>WriteEventConfig</c>) are open to
/// anything, so the other half is asserted here, over the sources.
/// </para>
/// <para>
/// What the rule covers, and why the reflection that remains in the package is not a miss, is not
/// restated here: a list written twice is a list that drifts.
/// </para>
/// </remarks>
public sealed class ComponentMappingTests
{
    /// <summary>
    /// The directories the rule covers, repository-relative: the component layer, and the block
    /// model it renders into.
    /// </summary>
    private static readonly string[] _coveredDirectories =
    [
        "src/Gradio.Net/Components",
        "src/Gradio.Net/Core/Blocks",
    ];

    /// <summary>A file that has to be among the scanned ones, or the scan is looking at nothing.</summary>
    private const string KnownFile = "src/Gradio.Net/Components/Textbox.cs";

    [Fact]
    public void The_component_layer_reads_no_member_by_reflection()
    {
        var scanned = RepositoryTree
            .EnumerateFiles(["*.cs"], skippedSubtrees: [])
            .Select(path => (Absolute: path, Relative: Relative(path)))
            .Where(file => IsCovered(file.Relative))
            .ToList();

        // A scan that found no files would pass without checking anything, and a directory that
        // was renamed would take the rule away without failing this test.
        Assert.True(
            scanned.Count >= 100,
            $"Only {scanned.Count} file(s) were scanned under {string.Join(" and ", _coveredDirectories)}; "
            + "the component layer is much larger than that, so this test is not looking at it.");

        Assert.True(
            scanned.Exists(file => string.Equals(file.Relative, KnownFile, StringComparison.Ordinal)),
            $"'{KnownFile}' was not among the scanned files, so the walk is not reaching the component layer.");

        var offenders = scanned
            .Where(file => System.IO.File.ReadAllText(file.Absolute).Contains("System.Reflection", StringComparison.Ordinal))
            .Select(file => file.Relative)
            .Order(StringComparer.Ordinal)
            .ToList();

        Assert.True(
            offenders.Count == 0,
            "A component or block reads its own members by reflection, which takes away the compile "
            + "error that a parameter the Python version adds is supposed to cause: "
            + string.Join(", ", offenders));
    }

    private static string Relative(string path) =>
        Path.GetRelativePath(RepositoryRoot.FullPath, path).Replace('\\', '/');

    private static bool IsCovered(string relative) =>
        _coveredDirectories.Any(directory => relative.StartsWith(directory + "/", StringComparison.Ordinal));
}

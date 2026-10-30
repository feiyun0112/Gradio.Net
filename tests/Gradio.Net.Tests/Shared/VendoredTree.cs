using System.Security.Cryptography;
using System.Text;

namespace Gradio.Net.Tests.Shared;

/// <summary>
/// Locates one of the vendored trees and hashes what is on disk, so that a test can compare what
/// the assembly serves against the files the package was built from.
/// </summary>
/// <remarks>
/// The trees are the Python version's compiled artifacts, checked in verbatim. Reading them from
/// the repository rather than from a recorded list is what lets a test say "what is served is what
/// is checked in" without a second file having to agree with both.
/// </remarks>
internal sealed class VendoredTree
{
    private VendoredTree(string section, string rootRelativePath)
    {
        Section = section;
        RootRelativePath = rootRelativePath;
    }

    /// <summary>The Python version's compiled Svelte bundle, served to the browser as it
    /// is.</summary>
    internal static VendoredTree Frontend { get; } =
        new("frontend", "src/upstream/frontend");

    /// <summary>The Python version's compiled SvelteKit server, which renders the page when SSR is
    /// on.</summary>
    internal static VendoredTree Ssr { get; } =
        new("ssr", "src/upstream/ssr");

    /// <summary>The Python version's curated workflow templates, which the canvas offers an
    /// author.</summary>
    internal static VendoredTree WorkflowCurated { get; } =
        new("workflowCurated", "src/upstream/workflow");

    /// <summary>The Python version's sample media, which the five media helpers hand a
    /// handler.</summary>
    internal static VendoredTree MediaAssets { get; } =
        new("mediaAssets", "src/upstream/media");

    /// <summary>The name this tree is known by.</summary>
    internal string Section { get; }

    /// <summary>The tree's directory, relative to the repository root.</summary>
    internal string RootRelativePath { get; }

    /// <summary>This tree's directory.</summary>
    internal string RootPath => Path.Combine(RepositoryRoot.FullPath, RootRelativePath);

    /// <summary>The tree on disk, as {path relative to the tree root: sha256}.</summary>
    internal Dictionary<string, string> HashTree()
    {
        var entries = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (string path in Directory.EnumerateFiles(RootPath, "*", SearchOption.AllDirectories))
        {
            entries[Path.GetRelativePath(RootPath, path).Replace('\\', '/')] = HashFile(path);
        }

        return entries;
    }

    /// <summary>Hashes a file.</summary>
    /// <param name="path">The file.</param>
    /// <returns>Its SHA-256, lowercase hexadecimal.</returns>
    internal static string HashFile(string path)
    {
        using var stream = System.IO.File.OpenRead(path);

        return Convert.ToHexString(SHA256.HashData(stream)).ToLowerInvariant();
    }

    /// <summary>Hashes bytes that were read rather than written.</summary>
    /// <param name="bytes">The content.</param>
    /// <returns>Its SHA-256, lowercase hexadecimal.</returns>
    internal static string HashBytes(byte[] bytes)
    {
        return Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
    }

    /// <summary>Hashes text as UTF-8.</summary>
    /// <param name="text">The text.</param>
    /// <returns>Its SHA-256, lowercase hexadecimal.</returns>
    internal static string HashText(string text)
    {
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text))).ToLowerInvariant();
    }

    /// <summary>Every way an observed tree differs from the one on disk, in both directions.</summary>
    /// <param name="expected">The tree on disk, as {path: sha256}.</param>
    /// <param name="observed">The tree as it was found, in the same shape.</param>
    /// <param name="observedBy">What was read, for the message: "vendored" or "embedded".</param>
    /// <returns>One line per difference; empty when the two agree.</returns>
    /// <remarks>
    /// Shared by the two places that compare a tree: the directory on disk against the resources
    /// embedded in the assembly. They differ in where they read from, not in what "the same tree"
    /// means.
    /// </remarks>
    internal static IReadOnlyList<string> Differences(
        IReadOnlyDictionary<string, string> expected,
        IReadOnlyDictionary<string, string> observed,
        string observedBy)
    {
        var differences = new List<string>();

        foreach (var path in expected.Keys.Except(observed.Keys).Order())
        {
            differences.Add($"vendored but not {observedBy}: {path}");
        }

        foreach (var path in observed.Keys.Except(expected.Keys).Order())
        {
            differences.Add($"{observedBy} but not vendored: {path}");
        }

        foreach (var path in expected.Keys.Intersect(observed.Keys).Order())
        {
            if (!string.Equals(expected[path], observed[path], StringComparison.Ordinal))
            {
                differences.Add($"changed: {path} ({observedBy} {observed[path]}, vendored {expected[path]})");
            }
        }

        return differences;
    }
}

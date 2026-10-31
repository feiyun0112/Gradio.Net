using System.Globalization;
using System.Text.RegularExpressions;

namespace Gradio.Net.Tests.Versioning;

/// <summary>
/// Reads the single version source and re-derives the package version from it.
/// </summary>
/// <remarks>
/// The mapping is deliberately re-implemented here rather than called out to the build:
/// the point of the version tests is to check the build's derivation against an
/// independent one, so sharing code with it would defeat them.
/// </remarks>
internal static class VersionSource
{
    internal const string RelativePath = "eng/Versions.props";

    private static readonly Regex _upstreamDeclaration = new(
        @"<GradioUpstreamVersion[^>]*>([^<]+)</GradioUpstreamVersion>",
        RegexOptions.CultureInvariant);

    private static readonly Regex _patchDeclaration = new(
        @"<GradioNetPatchVersion[^>]*>([^<]+)</GradioNetPatchVersion>",
        RegexOptions.CultureInvariant);

    private static readonly string[] _msbuildFilePatterns = ["*.props", "*.targets", "*.csproj", "*.proj"];

    internal static string FilePath => System.IO.Path.Combine(RepositoryRoot.FullPath, "eng", "Versions.props");

    internal static string ReadUpstreamVersion()
    {
        var declaration = _upstreamDeclaration.Match(System.IO.File.ReadAllText(FilePath));

        return declaration.Success
            ? declaration.Groups[1].Value.Trim()
            : throw new InvalidOperationException($"{RelativePath} declares no GradioUpstreamVersion.");
    }

    /// <summary>The port's own patch number, or zero when the file declares none.</summary>
    internal static int ReadPatchVersion()
    {
        var declaration = _patchDeclaration.Match(System.IO.File.ReadAllText(FilePath));

        return declaration.Success ? int.Parse(declaration.Groups[1].Value.Trim(), CultureInfo.InvariantCulture) : 0;
    }

    /// <summary>The Python version <c>X.Y.Z</c> maps to Gradio.Net <c>0.X.Y</c>, plus a patch when
    /// the port has shipped fixes on top of it.</summary>
    internal static string DerivePackageVersion(string upstreamVersion)
    {
        var version = Version.Parse(upstreamVersion);
        var patch = ReadPatchVersion();

        return patch == 0
            ? $"0.{version.Major}.{version.Minor}"
            : $"0.{version.Major}.{version.Minor}.{patch}";
    }

    /// <summary>Every MSBuild file in the repository, skipping build output and tool caches.</summary>
    internal static IEnumerable<string> EnumerateMsBuildFiles() =>
        RepositoryTree.EnumerateFiles(_msbuildFilePatterns, skippedSubtrees: []);
}

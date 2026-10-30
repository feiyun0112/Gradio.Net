using System.Text.RegularExpressions;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Versioning;

/// <summary>
/// Guards the input side of the version mapping: there is exactly one version source, and no other
/// MSBuild file declares a version of ours. The previous implementation shipped three
/// disagreeing version numbers, which is what this prevents.
/// </summary>
public sealed class VersionSourceTests
{
    private static readonly Regex _upstreamDeclaration = new(
        @"<GradioUpstreamVersion[^>]*>([^<]+)</GradioUpstreamVersion>",
        RegexOptions.CultureInvariant);

    private static readonly Regex _versionLiteral = new(
        @"<(Version|VersionPrefix|AssemblyVersion|FileVersion|InformationalVersion|GradioUpstreamVersion)\b[^>]*>\s*[0-9]",
        RegexOptions.CultureInvariant);

    [Fact]
    public void The_single_version_source_declares_exactly_one_upstream_version()
    {
        var declarations = _upstreamDeclaration
            .Matches(System.IO.File.ReadAllText(VersionSource.FilePath))
            .Select(match => match.Groups[1].Value.Trim())
            .ToArray();

        Assert.Equal(GradioVersion.Upstream, Assert.Single(declarations));
    }

    [Fact]
    public void No_other_msbuild_file_declares_a_version_literal()
    {
        var offenders = VersionSource.EnumerateMsBuildFiles()
            .Where(file => !string.Equals(file, VersionSource.FilePath, StringComparison.OrdinalIgnoreCase))
            .Where(file => _versionLiteral.IsMatch(System.IO.File.ReadAllText(file)))
            .Select(file => System.IO.Path.GetRelativePath(RepositoryRoot.FullPath, file))
            .ToArray();

        Assert.True(
            offenders.Length == 0,
            $"Versions must be derived from {VersionSource.RelativePath}; these files declare a version literal: {string.Join(", ", offenders)}.");
    }
}

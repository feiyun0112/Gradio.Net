using System.Reflection;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Versioning;

/// <summary>
/// Guards the output side of the version mapping: what the package declares - the
/// generated constants and the assembly metadata - follows the single version source.
/// Editing that one literal moves all of them together; any place that stops following
/// the derivation fails here.
/// </summary>
public sealed class DeclaredVersionTests
{
    private static readonly Assembly _package = typeof(GradioVersion).Assembly;

    [Fact]
    public void The_declared_package_version_follows_the_upstream_mapping()
    {
        Assert.Equal(GradioVersion.PackageVersion, VersionSource.DerivePackageVersion(GradioVersion.Upstream));
    }

    [Fact]
    public void The_assembly_version_is_derived_from_the_single_source()
    {
        var upstreamVersion = VersionSource.ReadUpstreamVersion();
        var packageVersion = VersionSource.DerivePackageVersion(upstreamVersion);

        // An assembly version has four parts whether or not the package version does: a package of
        // 0.6.30 is the assembly 0.6.30.0, and one of 0.6.30.1 is the assembly 0.6.30.1.
        Assert.Equal(FourParts(packageVersion), _package.GetName().Version?.ToString());

        var informationalVersion = _package.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        Assert.Equal($"{packageVersion}+gradio.{upstreamVersion}", informationalVersion);
    }

    /// <summary>A version padded with zeros to the four parts an assembly version has.</summary>
    /// <param name="version">The version, as the package spells it.</param>
    /// <returns>The same version with four parts.</returns>
    private static string FourParts(string version)
    {
        var parts = version.Split('.');

        return string.Join('.', [.. parts, .. Enumerable.Repeat("0", 4 - parts.Length)]);
    }
}

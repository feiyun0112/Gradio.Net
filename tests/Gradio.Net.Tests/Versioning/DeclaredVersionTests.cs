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

        Assert.Equal($"{packageVersion}.0", _package.GetName().Version?.ToString());

        var informationalVersion = _package.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        Assert.Equal($"{packageVersion}+gradio.{upstreamVersion}", informationalVersion);
    }
}

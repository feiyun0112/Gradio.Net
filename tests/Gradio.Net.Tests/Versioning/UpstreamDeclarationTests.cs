using System.Text.Json.Nodes;
using Xunit;

namespace Gradio.Net.Tests.Versioning;

/// <summary>
/// Guards the other half of the single version source: the declarations that name the
/// the Python version release, wherever they are checked in.
/// </summary>
/// <remarks>
/// <para>
/// <c>eng/Versions.props</c> is the source. Everything else that names a release is a copy of it
/// taken at generation time: the component parameters, the exports and the events under <c>src/the
/// Python version/</c>. Nothing compared them, so an upgrade that re-vendored the assets but left a
/// declaration behind would build cleanly and describe a release this tree no longer targets.
/// </para>
/// <para>
/// The declarations are found by walking the tree rather than by listing them, so a fourth one
/// added later is covered without anyone remembering to add it here.
/// </para>
/// </remarks>
public sealed class UpstreamDeclarationTests
{
    [Fact]
    public void Every_declaration_names_the_release_of_the_version_source()
    {
        var expected = VersionSource.ReadUpstreamVersion();
        var declarations = UpstreamDeclarations.Read();

        // A tree with no declarations would make this test pass without checking anything.
        Assert.True(
            declarations.Count >= 3,
            $"Found {declarations.Count} the Python version declaration(s); the component parameters, the exports and the events should all be there.");

        var offenders = declarations
            .Where(declaration => !string.Equals(declaration.UpstreamVersion, expected, StringComparison.Ordinal))
            .Select(declaration => $"{declaration.RelativePath}: {declaration.UpstreamVersion}")
            .ToArray();

        Assert.True(
            offenders.Length == 0,
            $"{VersionSource.RelativePath} targets the Python version {expected}, but these declarations name another release: "
            + $"{string.Join(", ", offenders)}. Re-record them from {expected} rather than editing the number.");
    }

    [Fact]
    public void Every_declaration_that_records_a_wheel_records_the_same_one()
    {
        // Two declarations can agree on the release and still describe different wheels - one
        // re-extracted from a rebuilt artifact, the other left as it was. The digest is what says
        // which artifact the bytes came from, so that is what has to agree; the file name and size
        // travel with it, and a mismatch in either is worth reading out too.
        //
        // Not every declaration records a wheel: the route table is extracted from the Python
        // source tree, and the theme set from the release's own theme data, so neither has a wheel
        // to name. Those are simply not part of this check.
        var wheels = UpstreamDeclarations.Read()
            .Select(declaration => (declaration.RelativePath, Wheel: WheelOf(declaration.Document)))
            .Where(candidate => candidate.Wheel is not null)
            .Select(candidate => (candidate.RelativePath, Wheel: candidate.Wheel!))
            .ToArray();

        Assert.True(
            wheels.Length >= 2,
            $"Found {wheels.Length} declaration(s) recording the Python wheel; the vendored manifest and the component manifest both should.");

        var expected = DigestOf(wheels[0].Wheel);
        var offenders = wheels
            .Where(candidate => !string.Equals(DigestOf(candidate.Wheel), expected, StringComparison.Ordinal))
            .Select(candidate => $"{candidate.RelativePath}: {Describe(candidate.Wheel)}")
            .ToArray();

        Assert.True(
            offenders.Length == 0,
            $"The declarations disagree about the Python wheel. {wheels[0].RelativePath} says {Describe(wheels[0].Wheel)}; "
            + $"these say something else: {string.Join(", ", offenders)}.");
    }

    /// <summary>
    /// The wheel a declaration was extracted from, under <c>source</c>, which records where the
    /// contents came from.
    /// </summary>
    private static JsonObject? WheelOf(JsonObject document) =>
        document["distribution"] as JsonObject
        ?? (document["source"] as JsonObject)?["distribution"] as JsonObject;

    private static string DigestOf(JsonObject wheel) =>
        wheel["sha256"]?.GetValue<string>()
        ?? throw new InvalidDataException($"A recorded wheel has no sha256: {wheel.ToJsonString()}");

    private static string Describe(JsonObject wheel) =>
        $"{wheel["filename"]?.GetValue<string>()} ({wheel["size"]?.GetValue<long>()} bytes, sha256 {DigestOf(wheel)})";
}

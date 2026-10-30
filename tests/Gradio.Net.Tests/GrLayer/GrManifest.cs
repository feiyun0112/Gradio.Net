namespace Gradio.Net.Tests.GrLayer;

/// <summary>
/// The Python side of the parameter comparison: each block's parameters, and their defaults.
/// </summary>
/// <remarks>
/// <para>
/// The recorded declaration, not a list typed here: a parameter the Python version adds arrives in
/// the manifest the next time it is recorded, and every comparison that reads it starts demanding
/// it without anyone remembering it exists.
/// </para>
/// <para>
/// A separate document from the export declaration <see cref="GrSurface"/> reads, because the two
/// answer different questions: which names exist, and what each of them takes. Where it lives is
/// <see cref="UpstreamDeclaration"/>'s business, so that the three comparisons cannot disagree
/// about it.
/// </para>
/// </remarks>
internal static class GrManifest
{
    /// <summary>Each block the Python version declares, with the parameters it declares for
    /// it.</summary> <returns>The block's class name and its parameter list, in the Python
    /// version's order.</returns>
    internal static IEnumerable<(string Block, IReadOnlyList<string> Parameters)> Blocks() =>
        UpstreamDeclaration.Parameters();

    /// <summary>Each parameter's default, as the Python version's signature declares it.</summary>
    /// <param name="block">The block's class name, as the manifest spells it.</param>
    /// <returns>The recorded defaults, keyed by parameter name.</returns>
    internal static IReadOnlyDictionary<string, System.Text.Json.Nodes.JsonNode?> Defaults(string block) =>
        UpstreamDeclaration.Defaults(block);
}

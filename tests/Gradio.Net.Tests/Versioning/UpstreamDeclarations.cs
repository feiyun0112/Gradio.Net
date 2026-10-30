using System.Text.Json.Nodes;

namespace Gradio.Net.Tests.Versioning;

/// <summary>One checked-in document that restates the Python release it was recorded
/// from.</summary> <param name="RelativePath">Where it lives, for the failure message.</param>
/// <param name="UpstreamVersion">The release it names.</param> <param name="Document">The document
/// itself, for the assertions that read more than the version.</param>
internal sealed record UpstreamDeclaration(string RelativePath, string UpstreamVersion, JsonObject Document);

/// <summary>
/// Finds every checked-in JSON document that declares an <c>upstreamVersion</c>.
/// </summary>
/// <remarks>
/// <para>
/// Found by walking the tree rather than by naming the files, because the point of the check is
/// that a declaration nobody remembered is still a declaration. The ones that exist today are the
/// declarations the build reads (<c>src/upstream/&lt;version&gt;/</c>: the component
/// parameters, the exports, the events).
/// </para>
/// <para>
/// The two vendored trees are skipped: they are the Python version's compiled artifacts, they carry
/// the the Python version project's own JSON (thousands of files, including its
/// <c>node_modules</c>), and nothing in them is this repository's declaration.
/// </para>
/// <para>
/// A file is only parsed when its text mentions <c>upstreamVersion</c> at all. Parsing every JSON
/// file in the tree would make a deliberately malformed fixture - a parser test's input, say -
/// fail the version tests, and the reason would have nothing to do with versions.
/// </para>
/// </remarks>
internal static class UpstreamDeclarations
{
    private const string Field = "upstreamVersion";

    private static readonly string[] _skippedSubtrees =
    [
        Path.Combine("src", "Gradio.Net", "frontend"),
        Path.Combine("src", "Gradio.Net", "ssr"),
        Path.Combine("src", "Gradio.Net", "media"),
    ];

    internal static IReadOnlyList<UpstreamDeclaration> Read()
    {
        var declarations = new List<UpstreamDeclaration>();

        foreach (var file in RepositoryTree.EnumerateFiles(["*.json"], _skippedSubtrees))
        {
            var text = System.IO.File.ReadAllText(file);
            if (!text.Contains(Field, StringComparison.Ordinal))
            {
                continue;
            }

            JsonNode? parsed;

            try
            {
                parsed = JsonNode.Parse(text);
            }
            catch (System.Text.Json.JsonException exception)
            {
                throw new InvalidDataException($"{Relative(file)} is not valid JSON: {exception.Message}", exception);
            }

            if (parsed is not JsonObject document
                || !document.TryGetPropertyValue(Field, out var version)
                || version is not JsonValue value
                || !value.TryGetValue<string>(out var release))
            {
                throw new InvalidDataException(
                    $"{Relative(file)} mentions '{Field}' but does not declare it as a string at the top level.");
            }

            declarations.Add(new UpstreamDeclaration(Relative(file), release, document));
        }

        return declarations;
    }

    private static string Relative(string path) => Path.GetRelativePath(RepositoryRoot.FullPath, path);
}

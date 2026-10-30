using System.Text.Json.Nodes;
using Gradio.Net;
using Gradio.Net.Tests.Versioning;
namespace Gradio.Net.Tests;

/// <summary>
/// The recorded Python declarations, read from one place.
/// </summary>
/// <remarks>
/// <para>
/// Three comparisons read these files — the export surface, the parameter lists, the event lists —
/// and each of them would otherwise carry its own idea of where a declaration lives and how it is
/// shaped. One reader is what keeps "the recorded declaration" a single thing: a manifest that
/// moves, or a version that is recorded twice, breaks here rather than in three places differently.
/// </para>
/// <para>
/// What is read is the declaration the build itself reads, not a copy: a parameter or an event the
/// Python version adds arrives in these files the next time they are recorded, and every comparison
/// that reads them starts demanding it.
/// </para>
/// </remarks>
internal static class UpstreamDeclaration
{
    /// <summary>The names the Python version exports at its top level, each with the kind it
    /// declares.</summary> <returns>Export name to kind: <c>component</c>, <c>layout</c>,
    /// <c>function</c>…</returns>
    internal static IReadOnlyDictionary<string, string> Exports()
    {
        var exports = Read("exports.json")["exports"] as JsonObject
            ?? throw new InvalidDataException("the export declaration lists no 'exports'");

        return exports.ToDictionary(
            pair => pair.Key,
            pair => pair.Value?["kind"]?.GetValue<string>() ?? string.Empty,
            StringComparer.Ordinal);
    }

    /// <summary>Each block the Python version declares, with the parameters it declares for
    /// it.</summary> <returns>The block's class name and its parameter list, in the Python
    /// version's order.</returns>
    /// <remarks>
    /// A block whose constructor ends in <c>**kwargs</c> records the parameters before it and says
    /// the list is not the whole of it; those are still a signature, so they are still compared.
    /// </remarks>
    internal static IEnumerable<(string Block, IReadOnlyList<string> Parameters)> Parameters()
    {
        foreach (var (_, entry) in Blocks("component-params.json"))
        {
            if (entry["params"] is not JsonArray parameters)
            {
                continue;
            }

            yield return (
                entry["class"]!.GetValue<string>(),
                [.. parameters.Select(parameter => parameter!.GetValue<string>())]);
        }
    }

    /// <summary>Each parameter's default, as the Python version's signature declares it.</summary>
    /// <param name="block">The block's class name, as the manifest spells it.</param>
    /// <returns>The recorded defaults, keyed by parameter name.</returns>
    /// <remarks>
    /// Recorded rather than inferred, because a factory's default has to be the Python version's
    /// and Roslyn cannot read a property initialiser. A default that is not a value — the sentinel
    /// a dropdown's value starts as, an icon that names a file inside the Python version's
    /// installation — is recorded as an object with one member, <c>unspellable</c>, which no C#
    /// literal can be written from and which the comparison therefore treats as "the block's own
    /// default decides".
    /// </remarks>
    internal static IReadOnlyDictionary<string, JsonNode?> Defaults(string block)
    {
        var entry = Blocks("component-params.json")
            .FirstOrDefault(pair => string.Equals(pair.Entry["class"]!.GetValue<string>(), block, StringComparison.Ordinal))
            .Entry;

        return entry?["defaults"] is JsonObject defaults
            ? defaults.ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.Ordinal)
            : new Dictionary<string, JsonNode?>(StringComparer.Ordinal);
    }

    /// <summary>Each block's events, keyed by the block name the manifest uses.</summary>
    /// <returns>Block name to its class name and the events the Python version declares, in the
    /// Python version's order.</returns>
    internal static IReadOnlyDictionary<string, (string Block, IReadOnlyList<string> Events)> Events()
    {
        var blocks = new Dictionary<string, (string, IReadOnlyList<string>)>(StringComparer.Ordinal);

        foreach (var (key, entry) in Blocks("component-events.json"))
        {
            if (entry["events"] is not JsonArray events)
            {
                continue;
            }

            blocks[key] = (
                entry["class"]!.GetValue<string>(),
                [.. events.Select(item => item!.GetValue<string>())]);
        }

        return blocks;
    }

    /// <summary>Reads one recorded declaration for the pinned release.</summary>
    /// <param name="file">The file's name under <c>src/the Python
    /// version/&lt;version&gt;/</c>.</param> <returns>The document.</returns>
    internal static JsonObject Read(string file)
    {
        var path = Path.Combine(
            RepositoryRoot.FullPath,
            "src",
            "upstream",
            file);

        return JsonNode.Parse(System.IO.File.ReadAllText(path)) as JsonObject
            ?? throw new InvalidDataException($"{path} is not a JSON object");
    }

    private static IEnumerable<(string Key, JsonObject Entry)> Blocks(string file)
    {
        if (Read(file)["blocks"] is not JsonObject blocks)
        {
            throw new InvalidDataException($"{file} declares no 'blocks'");
        }

        foreach (var (key, entry) in blocks)
        {
            yield return (key, entry!.AsObject());
        }
    }
}

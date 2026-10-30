using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using Gradio.Net;
using Gradio.Net.Tests.Versioning;

namespace Gradio.Net.Tests.GrLayer;

/// <summary>
/// The set comparison this layer exists to satisfy: what the Python version exports at its top
/// level against what this layer offers, both spelled the C# way.
/// </summary>
/// <remarks>
/// <para>
/// "Mirrors the Python version's surface" is a claim about two sets, so it is checked as one. The
/// Python side is the recorded declaration (<c>src/the Python
/// version/&lt;version&gt;/exports.json</c>), not a list typed here: a component the Python version
/// adds shows up in the comparison without anyone remembering it exists.
/// </para>
/// <para>
/// A name is compared after both sides are spelled the same way. The Python version is Python, so
/// its names are either already PascalCase or snake_case; the rule is the one this layer follows —
/// the words stay and only their case changes — which for <c>on</c> is <c>On</c>, for
/// <c>load_chat</c> is <c>LoadChat</c>, and for a name that already reads as a type is itself.
/// </para>
/// </remarks>
internal static class GrSurface
{
    /// <summary>The names this layer offers at its top level, spelled the C# way.</summary>
    /// <returns>The public static methods, properties and fields of <see cref="gr"/> that are not extensions.</returns>
    /// <remarks>
    /// <para>
    /// All three kinds of member, because the Python version's top level has all three: a factory
    /// or a function is a method, a module it exposes without declaring (<c>gr.themes</c>) is a
    /// property here, and a name that holds a value (<c>gr.NO_RELOAD</c>) is a field. Counting only
    /// methods would report two names the layer has as names it is missing.
    /// </para>
    /// <para>
    /// Extension methods are excluded on purpose: they mirror the Python version's methods on an
    /// object (<c>demo.launch()</c>), not the Python version's top-level exports, and counting them
    /// here would report every one of them as a name the layer invented.
    /// </para>
    /// </remarks>
    internal static IReadOnlySet<string> LayerMembers() =>
        new HashSet<string>(
            [
                .. typeof(gr)
                    .GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Where(method => !method.IsDefined(typeof(ExtensionAttribute), inherit: false))
                    .Where(method => !method.IsSpecialName)
                    .Select(method => method.Name),
                .. typeof(gr).GetProperties(BindingFlags.Public | BindingFlags.Static).Select(property => property.Name),
                .. typeof(gr).GetFields(BindingFlags.Public | BindingFlags.Static).Select(field => field.Name),
            ],
            StringComparer.Ordinal);

    /// <summary>The names the Python version declares at its top level, spelled the C#
    /// way.</summary> <returns>The exported names, normalised.</returns>
    internal static IReadOnlySet<string> UpstreamExports() =>
        Read("exports")
            .Select(entry => Pascal(entry.Key))
            .ToHashSet(StringComparer.Ordinal);

    /// <summary>
    /// The modules the Python version exposes at its top level without declaring them, spelled the
    /// C# way.
    /// </summary>
    /// <returns>The module names, normalised.</returns>
    /// <remarks>
    /// <c>themes</c> is the one that matters: the Python version lets an application write
    /// <c>gr.themes.Soft()</c> and does not list the module in <c>__all__</c>. A member of this
    /// layer that mirrors a module is therefore not an invented name, which is why this set is part
    /// of the comparison rather than left to be discovered.
    /// </remarks>
    internal static IReadOnlySet<string> UpstreamModules() =>
        Document["moduleAttributes"] is JsonArray modules
            ? modules.OfType<JsonValue>()
                .Select(value => value.GetValue<string>())
                .Select(Pascal)
                .ToHashSet(StringComparer.Ordinal)
            : throw new InvalidDataException("the export declaration lists no module attributes");

    /// <summary>
    /// The callables the Python version exposes at its top level without declaring them, spelled
    /// the C# way.
    /// </summary>
    /// <returns>The names, normalised.</returns>
    /// <remarks>
    /// <c>api</c> is the one that matters: <c>__all__</c> does not list it, and a ported file writes
    /// <c>gr.api(...)</c> — the decorator this layer replaces with an explicit call.
    /// <c>component</c> is the other, and the layer does not mirror it: a custom component here is a
    /// class deriving from <see cref="Core.Component"/> rather than a class made by a factory.
    /// </remarks>
    internal static IReadOnlySet<string> UpstreamUndeclared() =>
        Document["functionAttributes"] is JsonArray functions
            ? functions.OfType<JsonValue>()
                .Select(value => value.GetValue<string>())
                .Select(Pascal)
                .ToHashSet(StringComparer.Ordinal)
            : throw new InvalidDataException("the export declaration lists no undeclared functions");

    /// <summary>The names this layer has to offer: what the Python version declares, and what it
    /// exposes anyway.</summary> <returns>The exports and the undeclared functions,
    /// together.</returns>
    /// <remarks>
    /// The modules are not here on purpose. A module is a name the layer may mirror rather than one
    /// it owes — <c>gr.themes</c> is mirrored, and the forty-eight modules beside it (the Python
    /// version's own internals) are not something a ported application writes. That is why
    /// <see cref="UpstreamModules"/> is subtracted in the other direction only, where it says a
    /// member mirroring a module is not an invented name.
    /// </remarks>
    internal static IReadOnlySet<string> UpstreamSurface() =>
        UpstreamExports().Concat(UpstreamUndeclared()).ToHashSet(StringComparer.Ordinal);

    /// <summary>Every name a ported file can write at the Python version's top level, spelled the
    /// C# way.</summary> <returns>The surface, and the modules beside it.</returns>
    internal static IReadOnlySet<string> UpstreamNames() =>
        UpstreamSurface().Concat(UpstreamModules()).ToHashSet(StringComparer.Ordinal);

    /// <summary>
    /// The names this layer adds, with the reason each one is not a name the Python version has.
    /// </summary>
    /// <returns>The name to the reason it exists.</returns>
    /// <remarks>
    /// The rule is that a name invented here is a name a port would not have had, so the set is
    /// empty in principle. It has two entries, and neither is a second vocabulary for something the
    /// Python version spells differently. `LoadOpenApiAsync` is the awaited half of a pair whose
    /// the Python version function is synchronous — an application that cannot block while a
    /// document is fetched has no use for the sync spelling, and a port writes the sync one.
    /// `Navbar` is the Python version's own name for a class it has on the module but not in
    /// `__all__`, which is the list the generated factories come from, so the one name a multipage
    /// page cannot do without is written by hand.
    /// </remarks>
    internal static IReadOnlyDictionary<string, string> OwnAdditions { get; } =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["LoadOpenApiAsync"] =
                "the awaited LoadOpenapi: reading a document over HTTP while a host starts is the one "
                + "place a .NET application would rather await, and the Python version's function is synchronous",
            ["Navbar"] =
                "the Python version has the class on its module but not in __all__, which is what the generated "
                + "factories are named after; a page with more than one address needs it, so it is the "
                + "one factory written by hand and compared to the recorded signature like the rest",
        };

    /// <summary>Spells a Python name the way this layer spells a member.</summary>
    /// <param name="name">The Python name.</param>
    /// <returns>The name with the same words and C# casing.</returns>
    /// <remarks>
    /// Two rules, because the Python version's names are of two shapes. A name with underscores is
    /// <c>snake_case</c> and becomes <c>PascalCase</c>, one word per part; a name without them is
    /// already a type or a function name and only needs its first letter raised — which is what
    /// leaves <c>CSVLogger</c> and <c>JSON</c> alone and turns <c>on</c> into <c>On</c>.
    /// </remarks>
    internal static string Pascal(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (!name.Contains('_', StringComparison.Ordinal))
        {
            return char.ToUpperInvariant(name[0]) + name[1..];
        }

        return string.Concat(
            name.Split('_', StringSplitOptions.RemoveEmptyEntries)
                .Select(part => char.ToUpperInvariant(part[0]) + part[1..].ToLowerInvariant()));
    }

    private static JsonObject Document => UpstreamDeclaration.Read("exports.json");

    private static JsonObject Read(string field) =>
        Document[field] as JsonObject
            ?? throw new InvalidDataException($"the export declaration has no '{field}'");
}

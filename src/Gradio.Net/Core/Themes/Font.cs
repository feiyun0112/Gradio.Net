using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// A font family, as a theme declares it: either a name the browser already knows, or a face the
/// theme brings with it.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>Font</c>: a name and a way to say what it needs. A plain name is a
/// family the browser resolves — <c>system-ui</c>, <c>Consolas</c>, <c>Optima</c> — and needs
/// nothing from the theme; <see cref="LocalFont"/> ships its own <c>@font-face</c> rules and
/// <see cref="GoogleFont"/> either does or points at Google's font service, depending on whether
/// the vendored frontend already carries the face.
/// </para>
/// <para>
/// A string converts to a <see cref="Font"/> implicitly, so a font stack can be written the way
/// the Python version writes one: <c>new Font[] { "ui-sans-serif", "sans-serif" }</c>.
/// </para>
/// </remarks>
public class Font
{
    /// <summary>The family names CSS treats as generic, which are written without quotes.</summary>
    private static readonly string[] _generic =
        ["sans-serif", "serif", "monospace", "cursive", "fantasy"];

    private readonly bool _bareName;

    /// <summary>Creates a family the browser resolves by name.</summary>
    /// <param name="name">The family name.</param>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    public Font(string name)
        : this(name, bareName: false)
    {
    }

    /// <summary>Creates a family, saying whether it is a bare name or a <see cref="Font"/>
    /// object.</summary> <param name="name">The family name.</param> <param name="bareName">Whether
    /// the Python version would hold this as a plain string rather than a font.</param>
    /// <remarks>
    /// The Python version keeps a font stack as a list of strings and <c>Font</c> objects, and the
    /// difference is observable twice: a string is written into <c>--font</c> as it is, without
    /// quotes, and it is serialised as a string rather than as a tagged object. A family that came
    /// from a string therefore remembers that it did.
    /// </remarks>
    private Font(string name, bool bareName)
    {
        ArgumentNullException.ThrowIfNull(name);

        Name = name;
        _bareName = bareName;
    }

    /// <summary>The family name.</summary>
    public string Name { get; }

    /// <summary>Reads a bare family name as a family the browser resolves itself.</summary>
    /// <param name="name">The family name.</param>
    /// <remarks>
    /// This is what lets a font stack be written the way the Python version writes one:
    /// <c>new Font[] { new LocalFont("Lora"), "ui-sans-serif", "sans-serif" }</c>.
    /// </remarks>
    public static implicit operator Font(string name) => new(name, bareName: true);

    /// <summary>What the theme has to carry for this family to render.</summary>
    /// <returns>
    /// Neither a URL nor CSS for a family the browser already knows, which is the case for every
    /// name that is not a <see cref="LocalFont"/> or a <see cref="GoogleFont"/>.
    /// </returns>
    public virtual FontStylesheet Stylesheet() => new(Url: null, Css: null);

    /// <summary>Writes the family the way a CSS <c>font-family</c> list does.</summary>
    /// <returns>The name, quoted unless it is one of CSS's generic families.</returns>
    /// <remarks>
    /// Quoting is not cosmetic: a family whose name contains a space — <c>Source Sans Pro</c> —
    /// is only one family when it is quoted, and would otherwise be read as three.
    /// </remarks>
    public override string ToString() =>
        _bareName || _generic.Contains(Name, StringComparer.Ordinal) ? Name : $"'{Name}'";

    /// <summary>Serialises the family the way the Python version's <c>FontEncoder</c>
    /// does.</summary> <returns>The tagged object a theme file carries.</returns>
    internal virtual JsonNode ToJson() => _bareName
        ? JsonValue.Create(Name)!
        : new JsonObject
        {
            ["__gradio_font__"] = true,
            ["name"] = Name,
            ["class"] = "font",
            ["weights"] = null,
        };

    /// <summary>Reads a family back from <see cref="ToJson"/>'s output.</summary>
    /// <param name="node">The tagged object.</param>
    /// <returns>The family.</returns>
    /// <exception cref="ArgumentException">The node is not a tagged font.</exception>
    internal static Font FromJson(JsonNode? node)
    {
        if (node is JsonValue value && value.TryGetValue<string>(out var bare))
        {
            return new Font(bare, bareName: true);
        }

        if (node is not JsonObject document
            || document["__gradio_font__"]?.GetValue<bool>() != true
            || document["name"]?.GetValue<string>() is not { } name)
        {
            throw new ArgumentException("A font entry is not a tagged font object.");
        }

        var weights = document["weights"] is JsonArray array
            ? array.Select(weight => weight!.GetValue<int>()).ToArray()
            : null;

        return document["class"]?.GetValue<string>() switch
        {
            "google" => new GoogleFont(name, weights),
            "local" => new LocalFont(name, weights),
            _ => new Font(name),
        };
    }
}

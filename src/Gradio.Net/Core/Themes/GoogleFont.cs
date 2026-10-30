using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// A font family requested by name from Google Fonts, which the frontend may already carry.
/// </summary>
/// <remarks>
/// <para>
/// The name is a Google Fonts family — <c>Lora</c>, <c>Inter</c> — and the weights are the ones to
/// declare. What the theme does with it depends on a question the application cannot see: whether
/// the vendored frontend already ships that face. If it does, the font is served from the
/// application itself and no request leaves the machine; if it does not, the page loads Google's
/// stylesheet instead. Both are the Python version's behaviour, and it is why a theme built with a
/// Google font renders the same offline as the preset that names the same family as a
/// <see cref="LocalFont"/>.
/// </para>
/// <para>
/// The weights matter to that decision, not just to the CSS: the frontend carries <c>Lora</c> at
/// 400 and 600, so <c>GoogleFont("Lora")</c> resolves locally and
/// <c>GoogleFont("Lora", weights: [400, 700])</c> does not — 700 has no file, and a rule pointing
/// at one would 404.
/// </para>
/// </remarks>
public sealed class GoogleFont : Font
{
    /// <summary>Creates a Google font family.</summary>
    /// <param name="name">The family name as Google Fonts spells it.</param>
    /// <param name="weights">
    /// The weights to declare. <see langword="null"/> means in the Python version's default of 400 and
    /// 600.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    public GoogleFont(string name, IEnumerable<int>? weights = null)
        : base(name)
    {
        Weights = weights is null ? [400, 600] : [.. weights];
    }

    /// <summary>The weights the family declares, in the order it declares them.</summary>
    public IReadOnlyList<int> Weights { get; }

    /// <inheritdoc />
    public override FontStylesheet Stylesheet() =>
        VendoredFonts.Contains(Name, Weights)
            ? new LocalFont(Name, Weights).Stylesheet()
            : new FontStylesheet(
                Url: $"https://fonts.googleapis.com/css2?family={Name.Replace(" ", "+", StringComparison.Ordinal)}"
                    + $":wght@{string.Join(";", Weights)}&display=swap",
                Css: null);

    /// <inheritdoc />
    internal override JsonNode ToJson() => new JsonObject
    {
        ["__gradio_font__"] = true,
        ["name"] = Name,
        ["class"] = "google",
        ["weights"] = new JsonArray([.. Weights.Select(weight => JsonValue.Create(weight))]),
    };
}

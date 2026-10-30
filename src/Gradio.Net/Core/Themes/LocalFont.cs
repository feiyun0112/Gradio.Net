using System.Collections.Frozen;
using System.Text;
using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// A font family whose files travel with the frontend, declared to the browser by the theme.
/// </summary>
/// <remarks>
/// <para>
/// The vendored frontend carries the faces Gradio's own themes use (<c>static/fonts</c>), so a theme
/// that names one of them does not have to fetch anything: it writes the <c>@font-face</c> rules and
/// the browser loads the file from the same server. That is why every built-in theme renders the
/// same offline, and why <see cref="GoogleFont"/> prefers this route when the face is there.
/// </para>
/// <para>
/// The weights are the ones declared, in the order declared, and each becomes one rule. Declaring a
/// weight the frontend does not carry produces a rule that 404s in the browser — the Python version
/// does not check, and neither does this: checking would mean a theme that renders differently
/// depending on which build produced it.
/// </para>
/// </remarks>
public sealed class LocalFont : Font
{
    /// <summary>The file name the Python version gives each weight.</summary>
    /// <remarks>
    /// A weight that is not in this table is spelled with its number, which is what the Python
    /// version does for a weight outside the nine CSS names.
    /// </remarks>
    private static readonly FrozenDictionary<int, string> _weightNames = new Dictionary<int, string>
    {
        [100] = "Thin",
        [200] = "ExtraLight",
        [300] = "Light",
        [400] = "Regular",
        [500] = "Medium",
        [600] = "SemiBold",
        [700] = "Bold",
        [800] = "ExtraBold",
        [900] = "Black",
    }.ToFrozenDictionary();

    /// <summary>Creates a local family.</summary>
    /// <param name="name">The family name, which is also how its directory is named.</param>
    /// <param name="weights">
    /// The weights to declare. <see langword="null"/> means in the Python version's default of 400 and
    /// 600.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    public LocalFont(string name, IEnumerable<int>? weights = null)
        : base(name)
    {
        Weights = weights is null ? [400, 600] : [.. weights];
    }

    /// <summary>The weights the family declares, in the order it declares them.</summary>
    public IReadOnlyList<int> Weights { get; }

    /// <inheritdoc />
    /// <remarks>
    /// One rule per weight, each preceded by a blank line and followed by one, which is how
    /// the Python version's template lays them out.
    /// </remarks>
    public override FontStylesheet Stylesheet()
    {
        var rules = new List<string>(Weights.Count);
        var directory = FileName(Name);

        foreach (var weight in Weights)
        {
            var css = new StringBuilder();
            css.Append("\n@font-face {\n");
            css.Append("    font-family: '").Append(Name).Append("';\n");
            css.Append("    src: url('static/fonts/").Append(directory).Append('/').Append(directory)
                .Append('-').Append(WeightFileName(weight)).Append(".woff2') format('woff2');\n");
            css.Append("    font-weight: ").Append(weight).Append(";\n");
            css.Append("    font-style: normal;\n");
            css.Append("}\n");

            rules.Add(css.ToString());
        }

        return new FontStylesheet(Url: null, Css: string.Join("\n", rules));
    }

    /// <inheritdoc />
    internal override JsonNode ToJson() => new JsonObject
    {
        ["__gradio_font__"] = true,
        ["name"] = Name,
        ["class"] = "local",
        ["weights"] = new JsonArray([.. Weights.Select(weight => JsonValue.Create(weight))]),
    };

    /// <summary>The name a family's files are kept under: the family name without its spaces.</summary>
    /// <param name="name">The family name.</param>
    /// <returns>The directory and file stem the frontend uses.</returns>
    internal static string FileName(string name) => name.Replace(" ", string.Empty, StringComparison.Ordinal);

    /// <summary>The file name suffix for a weight.</summary> <param name="weight">The
    /// weight.</param> <returns>The Python version's name for it, or its number when it is not one
    /// of the nine.</returns>
    internal static string WeightFileName(int weight) =>
        _weightNames.TryGetValue(weight, out var name) ? name : weight.ToString(System.Globalization.CultureInfo.InvariantCulture);
}

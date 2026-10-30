using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// One token of a highlighted text: a substring and the category or confidence it belongs to.
/// </summary>
/// <param name="Token">The substring.</param>
/// <param name="ClassOrConfidence">Its category, a confidence, or <see langword="null"/> for none.</param>
/// <remarks>
/// The Python version's <c>(word, category)</c> tuple. It is a record here rather than a value
/// tuple so that both directions of the component's value can name it — a handler reading a
/// highlighted text and one returning it speak about the same thing.
/// </remarks>
public sealed record HighlightedToken(string Token, object? ClassOrConfidence = null);

/// <summary>
/// Text whose spans are highlighted by category or by confidence.
/// </summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. The value is a list of tokens rather than one string:
/// every character of the text is in exactly one token, and a token's second half says how it is
/// painted.
/// </para>
/// <para>
/// A handler may also return a dictionary of a text and the entities found in it, which is the
/// shape a named-entity model produces. The Python version splits that into tokens around the
/// entities, and so does this component — the splitting is part of what the value means rather than
/// something the application has to do first.
/// </para>
/// </remarks>
[GradioBlock("highlightedtext")]
[HandlerValue("IReadOnlyList<HighlightedToken>", "change", "select")]
public sealed partial class HighlightedText : Component
{
    internal HighlightedText()
    {
    }

    private object? _value;

    /// <summary>The text the component starts with.</summary>
    /// <value>
    /// A list of <see cref="HighlightedToken"/> values, a list of pairs, a dictionary of a text and
    /// its entities, or <see langword="null"/> for no text.
    /// </value>
    public object? Value
    {
        get => Postprocess(_value);
        set => _value = value;
    }

    /// <summary>The colour each category is drawn in.</summary>
    /// <remarks>Hex codes or colour names, as the frontend's stylesheet reads them.</remarks>
    public IReadOnlyDictionary<string, string>? ColorMap { get; set; }

    /// <summary>Whether the categories are listed in a legend rather than drawn inline.</summary>
    public bool ShowLegend { get; set; }

    /// <summary>Whether a token's category is drawn beside it when there is no legend.</summary>
    public bool ShowInlineCategory { get; set; } = true;

    /// <summary>Whether neighbouring tokens of one category are merged into one span.</summary>
    public bool CombineAdjacent { get; set; }

    /// <summary>What is put between two tokens that are merged.</summary>
    public string AdjacentSeparator { get; set; } = string.Empty;

    /// <summary>Whether the whitespace around a token is kept.</summary>
    /// <remarks>
    /// With it off, each token is trimmed before it is sent — which is what a tokenizer's output
    /// needs when the spaces are not part of the highlighted text.
    /// </remarks>
    public bool ShowWhitespaces { get; set; } = true;

    /// <summary>The label drawn above the text.</summary>
    public override string? Label { get; set; }

    /// <summary>The relative width of the text next to its neighbours.</summary>
    public int? Scale { get; set; }

    /// <summary>The width below which the text wraps.</summary>
    public int? MinWidth { get; set; } = 160;

    /// <summary>Whether the text is drawn right to left.</summary>
    public bool Rtl { get; set; }

    /// <summary>The buttons drawn in the component's corner.</summary>
    /// <value>No buttons unless the application asks for one.</value>
    /// <remarks>
    /// Names only, for now: a custom <c>Button</c> is a block, and the button family is its own
    /// batch (ticket 15). The Python version's default is an empty list rather than a missing key.
    /// </remarks>
    public IReadOnlyList<string> Buttons { get; set; } = [];

    /// <inheritdoc />
    public override JsonObject ApiInfo => DataSchemas.HighlightedTextSchema();

    /// <inheritdoc />
    public override JsonNode? ExampleInputs => new JsonArray(
        new JsonObject { ["token"] = "The", ["class_or_confidence"] = null },
        new JsonObject { ["token"] = "quick", ["class_or_confidence"] = "adj" });

    /// <summary>The tokens a handler reads.</summary>
    /// <param name="value">What the client sent: a list of tokens.</param>
    /// <returns>The tokens, or <see langword="null"/> for an empty text.</returns>
    public override object? Preprocess(JsonNode? value)
    {
        if (value is not JsonArray tokens)
        {
            return null;
        }

        return tokens
            .OfType<JsonObject>()
            .Select(token => new HighlightedToken(
                JsonValues.Text(token["token"]) ?? string.Empty,
                JsonValues.ToObject(token["class_or_confidence"])))
            .ToArray();
    }

    /// <summary>The tokens the client receives from what a handler returned.</summary>
    /// <param name="value">Tokens, pairs, a text with its entities, or none.</param>
    /// <returns>The tokens, merged and normalized as the options say.</returns>
    /// <exception cref="ArgumentException">
    /// A dictionary was given without the two keys the Python version requires.
    /// </exception>
    public override JsonNode? Postprocess(object? value)
    {
        if (value is null)
        {
            return null;
        }

        var tokens = CombineAdjacent ? Combine(TokensOf(value)) : TokensOf(value);

        return new JsonArray([.. tokens.Select(token => (JsonNode?)new JsonObject
        {
            ["token"] = Normalize(token.Token),
            ["class_or_confidence"] = JsonValues.FromAnyObject(token.ClassOrConfidence),
        })]);
    }

    /// <summary>The tokens a value holds.</summary>
    /// <exception cref="ArgumentException">The value is of a shape no token list can be made from.</exception>
    private static List<HighlightedToken> TokensOf(object? value) => value switch
    {
        null => [],
        JsonArray tokens => [.. tokens.OfType<JsonObject>().Select(TokenOf)],
        JsonObject document when document["text"] is not null => Split(document),
        IReadOnlyDictionary<string, object?> document => Split(document),
        IEnumerable<HighlightedToken> tokens => [.. tokens],
        IEnumerable<ITuple> pairs => [.. pairs.Select(pair => new HighlightedToken(
            Convert.ToString(pair[0], System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
            pair.Length > 1 ? pair[1] : null))],
        _ => throw new ArgumentException(
            $"A highlighted text is made of tokens, pairs, or a text with its entities; it was "
            + $"handed a {value.GetType()}.",
            nameof(value)),
    };

    /// <summary>One token as the protocol carries it.</summary>
    private static HighlightedToken TokenOf(JsonObject token) => new(
        JsonValues.Text(token["token"]) ?? string.Empty,
        JsonValues.ToObject(token["class_or_confidence"]));

    /// <summary>A text and the entities found in it, split into tokens around them.</summary>
    /// <param name="document">The document, as JSON.</param>
    private static List<HighlightedToken> Split(JsonObject document)
    {
        var text = JsonValues.Text(document["text"])
            ?? throw new ArgumentException(
                "Expected a dictionary with keys 'text' and 'entities' for the value of the "
                + "HighlightedText component.",
                nameof(document));

        var entities = document["entities"] as JsonArray
            ?? throw new ArgumentException(
                "Expected a dictionary with keys 'text' and 'entities' for the value of the "
                + "HighlightedText component.",
                nameof(document));

        return Split(text, entities.OfType<JsonObject>().Select(entity => new Entity(
            JsonValues.Int32(entity["start"]) ?? 0,
            JsonValues.Int32(entity["end"]) ?? 0,
            JsonValues.Text(entity["entity"]) ?? JsonValues.Text(entity["entity_group"]))));
    }

    /// <summary>A text and the entities found in it, split into tokens around them.</summary>
    /// <param name="document">The document, as CLR values.</param>
    private static List<HighlightedToken> Split(IReadOnlyDictionary<string, object?> document)
    {
        if (!document.TryGetValue("text", out var text) || text is not string content
            || !document.TryGetValue("entities", out var entities)
            || entities is not IReadOnlyList<object?> items)
        {
            throw new ArgumentException(
                "Expected a dictionary with keys 'text' and 'entities' for the value of the "
                + "HighlightedText component.",
                nameof(document));
        }

        return Split(content, items.OfType<IReadOnlyDictionary<string, object?>>().Select(entity => new Entity(
            Convert.ToInt32(entity.GetValueOrDefault("start"), System.Globalization.CultureInfo.InvariantCulture),
            Convert.ToInt32(entity.GetValueOrDefault("end"), System.Globalization.CultureInfo.InvariantCulture),
            entity.GetValueOrDefault("entity") as string ?? entity.GetValueOrDefault("entity_group") as string)));
    }

    /// <summary>A text split into the spans between and around its entities.</summary>
    private static List<HighlightedToken> Split(string text, IEnumerable<Entity> entities)
    {
        var ordered = entities.OrderBy(entity => entity.Start).ToArray();

        if (ordered.Length == 0)
        {
            return [new HighlightedToken(text)];
        }

        var tokens = new List<HighlightedToken>();
        var index = 0;

        foreach (var entity in ordered)
        {
            tokens.Add(new HighlightedToken(text[index..Math.Min(entity.Start, text.Length)]));
            tokens.Add(new HighlightedToken(
                text[Math.Min(entity.Start, text.Length)..Math.Min(entity.End, text.Length)],
                entity.Category));
            index = entity.End;
        }

        tokens.Add(new HighlightedToken(text[Math.Min(index, text.Length)..]));

        return tokens;
    }

    /// <summary>Neighbouring tokens of one category, merged.</summary>
    /// <remarks>
    /// Empty tokens are dropped before merging rather than after: an empty token would otherwise
    /// seed a run of its own, and the separator would appear where there is nothing to separate.
    /// </remarks>
    private List<HighlightedToken> Combine(List<HighlightedToken> tokens)
    {
        var output = new List<HighlightedToken>();
        string? runningText = null;
        object? runningCategory = null;

        foreach (var token in tokens)
        {
            if (token.Token.Length == 0)
            {
                continue;
            }

            if (runningText is null)
            {
                runningText = token.Token;
                runningCategory = token.ClassOrConfidence;
            }
            else if (Equals(token.ClassOrConfidence, runningCategory))
            {
                runningText += AdjacentSeparator + token.Token;
            }
            else
            {
                output.Add(new HighlightedToken(runningText, runningCategory));
                runningText = token.Token;
                runningCategory = token.ClassOrConfidence;
            }
        }

        if (runningText is not null)
        {
            output.Add(new HighlightedToken(runningText, runningCategory));
        }

        return output;
    }

    /// <summary>A token as it is sent, with its whitespace kept or trimmed.</summary>
    private string Normalize(string token) => ShowWhitespaces ? token : token.Trim();

    /// <summary>One entity found in a text: where it starts, where it ends, and its category.</summary>
    /// <param name="Start">The index of its first character.</param>
    /// <param name="End">The index just past its last character.</param>
    /// <param name="Category">Its category, or <see langword="null"/> for none.</param>
    private sealed record Entity(int Start, int End, string? Category);
}

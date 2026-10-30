using System.Globalization;
using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A classification label, with the confidence of the top categories below it.
/// </summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. The value has three shapes: a bare label, a map of
/// labels to confidences, or the document the frontend draws — and the first two are turned into
/// the third on the way out.
/// </para>
/// <para>
/// The map is sorted by confidence before it is sent, because the frontend draws the bars in the
/// order it receives them and the top class is the first one. <see cref="NumTopClasses"/> cuts the
/// list, which is what keeps a thousand-class model from drawing a thousand bars.
/// </para>
/// </remarks>
[GradioBlock("label")]
[HandlerValue("object?", "change", "select")]
public sealed partial class Label : Component
{
    internal Label()
    {
    }

    private object? _value;

    /// <summary>The label the component starts with.</summary>
    /// <value>
    /// Text, a number, a map of labels to confidences, the path of a JSON file holding one of those,
    /// or <see langword="null"/> for an empty label.
    /// </value>
    public object? Value
    {
        get => Postprocess(_value);
        set => _value = value;
    }

    /// <summary>How many of the most confident classes are shown, or <see langword="null"/> for all of them.</summary>
    public int? NumTopClasses { get; set; }

    /// <summary>The label drawn above the classification.</summary>
    /// <remarks>
    /// Named for what it is rather than for the Python version's parameter, because the parameter's
    /// own name is the class's: <c>Label.Label</c> is not a member C# allows.
    /// </remarks>
    [UpstreamParameter("label")]
    public string? LabelText { get; set; }

    /// <summary>The relative width of the classification next to its neighbours.</summary>
    public int? Scale { get; set; }

    /// <summary>The width below which the classification wraps.</summary>
    public int? MinWidth { get; set; } = 160;

    /// <summary>The background colour of the label, as a CSS colour name or a hex code.</summary>
    public string? Color { get; set; }

    /// <summary>Whether the heading is drawn above a map of confidences.</summary>
    /// <remarks>A bare label is drawn whatever this says: it is the whole of what there is to show.</remarks>
    public bool ShowHeading { get; set; } = true;

    /// <summary>The buttons drawn in the component's corner.</summary>
    /// <value>No buttons unless the application asks for one.</value>
    /// <remarks>
    /// Names only, for now: a custom <c>Button</c> is a block, and the button family is its own
    /// batch (ticket 15). The Python version's default is an empty list rather than a missing key.
    /// </remarks>
    public IReadOnlyList<string> Buttons { get; set; } = [];

    /// <inheritdoc />
    public override JsonObject ApiInfo => DataSchemas.LabelSchema();

    /// <inheritdoc />
    public override JsonNode? ExampleInputs => new JsonObject
    {
        ["label"] = "Cat",
        ["confidences"] = new JsonArray(
            new JsonObject { ["label"] = "cat", ["confidence"] = 0.9 },
            new JsonObject { ["label"] = "dog", ["confidence"] = 0.1 }),
    };

    /// <summary>The classification a handler reads.</summary>
    /// <param name="value">What the client sent: a label, and the scores below it.</param>
    /// <returns>
    /// The map of labels to confidences, or the bare label when there are no confidences, or
    /// <see langword="null"/> for an empty classification.
    /// </returns>
    public override object? Preprocess(JsonNode? value)
    {
        if (value is not JsonObject document)
        {
            return null;
        }

        if (document["confidences"] is not JsonArray confidences)
        {
            return JsonValues.ToObject(document["label"]);
        }

        var scores = new Dictionary<string, double?>(StringComparer.Ordinal);

        foreach (var confidence in confidences.OfType<JsonObject>())
        {
            scores[JsonValues.Text(confidence["label"]) ?? string.Empty] =
                JsonValues.Number(confidence["confidence"]);
        }

        return scores;
    }

    /// <summary>The classification the client receives from what a handler returned.</summary>
    /// <param name="value">A label, a number, a map of labels to confidences, a JSON path, or none.</param>
    /// <returns>The document the frontend draws, or an empty one for an empty value.</returns>
    /// <exception cref="ArgumentException"><paramref name="value"/> is of a shape a label cannot be made from.</exception>
    public override JsonNode? Postprocess(object? value)
    {
        switch (value)
        {
            case null:
                return new JsonObject();

            case string path when path.EndsWith(".json", StringComparison.Ordinal) && System.IO.File.Exists(path):
                return JsonNode.Parse(System.IO.File.ReadAllText(path))?.DeepClone() ?? new JsonObject();

            case string or int or long or float or double or decimal:
                return new JsonObject
                {
                    ["label"] = Convert.ToString(value, CultureInfo.InvariantCulture),
                    ["confidences"] = null,
                };

            case JsonObject document:
                return FromMap(JsonValues.ToObject(document) as IReadOnlyDictionary<string, object?>
                    ?? new Dictionary<string, object?>());
        }

        if (value is not IReadOnlyDictionary<string, object?> map)
        {
            throw new ArgumentException(
                "The Label output expects one of: a string label, an int label, a float label, or a "
                + $"dictionary whose keys are labels and values are confidences; got a "
                + $"{value.GetType()}.",
                nameof(value));
        }

        if (map.Count == 0)
        {
            return new JsonObject();
        }

        // A document that already carries a nested map of confidences is the shape a classification
        // model returns when it wraps its scores in a key of their own.
        if (map.TryGetValue("confidences", out var nested)
            && nested is IReadOnlyDictionary<string, object?> scores)
        {
            return FromMap(scores);
        }

        return FromMap(map);
    }

    /// <summary>A map of labels to confidences, sorted and cut to the top classes.</summary>
    /// <param name="scores">The map.</param>
    /// <returns>The document the frontend draws.</returns>
    private JsonObject FromMap(IReadOnlyDictionary<string, object?> scores)
    {
        if (scores.Count == 0)
        {
            return new JsonObject();
        }

        var sorted = scores
            .Select(pair => (Label: pair.Key, Confidence: Convert.ToDouble(pair.Value, CultureInfo.InvariantCulture)))
            .OrderByDescending(pair => pair.Confidence)
            .ToList();

        if (NumTopClasses is { } top && top >= 0)
        {
            sorted = [.. sorted.Take(top)];
        }

        return new JsonObject
        {
            ["label"] = sorted[0].Label,
            ["confidences"] = new JsonArray([.. sorted.Select(pair => (JsonNode?)new JsonObject
            {
                ["label"] = pair.Label,
                ["confidence"] = pair.Confidence,
            })]),
        };
    }
}

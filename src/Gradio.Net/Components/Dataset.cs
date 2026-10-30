using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A set of examples the user can page through and pick one of.
/// </summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. A dataset draws its samples with other components
/// rather than with a renderer of its own: each sample is a row of values, one per component, and
/// the config carries a copy of those components' props so the frontend can draw a row without
/// asking the server what a textbox looks like.
/// </para>
/// <para>
/// That copy is the one place in this framework where a component reads another one's props. It is
/// made from the in-memory shape — nulls in place — because that is what the Python version copies,
/// and it is written last, after the name and the flags an event binding sets, because that is
/// where the Python version's <c>get_config</c> override appends it.
/// </para>
/// <para>
/// The components are added to the page by the application, before the dataset that draws them.
/// The Python version's constructor registers them as it takes them; a C# component is added to an
/// application by <c>Blocks.Add</c>, so the order the application writes is the order the page has.
/// </para>
/// </remarks>
[GradioBlock("dataset")]
[HandlerValue("object?", "change", "click", "select")]
public sealed partial class Dataset : Component
{
    internal Dataset()
    {
    }

    /// <summary>What clicking a sample hands a handler.</summary>

    private IReadOnlyList<Component> _components = [];
    private IReadOnlyList<JsonObject>? _componentProps;
    private IReadOnlyList<IReadOnlyList<object?>>? _samples;
    private IReadOnlyList<string>? _headers;

    /// <summary>The label drawn above the dataset.</summary>
    public override string? Label { get; set; }

    /// <summary>The components that draw a sample, one value each.</summary>
    /// <remarks>
    /// Not written to the config as it is: the Python version's props carry a list of names and
    /// class ids, which is what <see cref="WriteExtraProps"/> appends. The parameter is declared
    /// because the Python version declares it, and the application sets it to the components it
    /// wants drawn.
    /// </remarks>
    [OmitFromConfig]
    public IReadOnlyList<Component> Components
    {
        get => _components;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            _components = value;
            _componentProps = null;
        }
    }

    /// <summary>How each component draws a sample, as a copy of its props.</summary>
    /// <value>
    /// The application's list, or — when it names none — one entry per component, copied from the
    /// component's own props with the value left out.
    /// </value>
    public IReadOnlyList<JsonObject>? ComponentProps
    {
        get => _componentProps ?? [.. Components.Select(PropsOf)];
        set => _componentProps = value;
    }

    /// <summary>The samples, as the frontend receives them.</summary>
    /// <value>
    /// One row per sample, each cell postprocessed by the component that draws it. The application
    /// sets the raw values, which is what <see cref="Preprocess"/> hands back.
    /// </value>
    public IReadOnlyList<IReadOnlyList<object?>> Samples
    {
        get
        {
            var raw = _samples ?? [[]];
            var processed = new List<IReadOnlyList<object?>>(raw.Count);

            foreach (var row in raw)
            {
                var cells = new List<object?>(row.Count);

                for (var index = 0; index < row.Count && index < Components.Count; index++)
                {
                    cells.Add(Components[index].Postprocess(row[index]));
                }

                processed.Add(cells);
            }

            return processed;
        }

        set => _samples = value;
    }

    /// <summary>The column headers.</summary>
    /// <value>
    /// The application's headers, or — when it names none — the components' labels, with an empty
    /// string for a component that has none. A dataset whose components are all unlabelled has no
    /// headers at all, which is the Python version's answer.
    /// </value>
    public IReadOnlyList<string> Headers
    {
        get
        {
            if (_headers is not null)
            {
                return _headers;
            }

            var labels = Components.Select(LabelOf).ToArray();

            return labels.All(label => label is null)
                ? []
                : [.. labels.Select(label => label ?? string.Empty)];
        }

        set => _headers = value;
    }

    /// <summary>What clicking a sample hands a handler.</summary>
    /// <value><c>values</c> for the sample's values, <c>index</c> for its position, or <c>tuple</c> for both.</value>
    /// <remarks>
    /// Not validated, because the Python version does not validate it: the value is stored and
    /// compared against the three names where it is used, and a name that is none of them hands the
    /// handler nothing. Refusing it here would make a page the Python version serves unreachable.
    /// </remarks>
    public string Type { get; set; } = "values";

    /// <summary>How the samples are drawn.</summary>
    /// <value>
    /// <c>gallery</c> for one component per card, <c>table</c> for one sample per row, or
    /// <see langword="null"/> for the frontend's choice — a gallery for one component, a table for
    /// more.
    /// </value>
    public string? Layout { get; set; }

    /// <summary>How many samples one page holds.</summary>
    public int SamplesPerPage { get; set; } = 10;

    /// <summary>The relative width of the dataset next to its neighbours.</summary>
    public int? Scale { get; set; }

    /// <summary>The width below which the dataset wraps.</summary>
    public int? MinWidth { get; set; } = 160;

    /// <summary>The address of the application this dataset was loaded from, if it was.</summary>
    /// <remarks>The Python version sets it when an application is loaded from another; it is never
    /// set by hand.</remarks>
    public string? ProxyUrl { get; set; }

    /// <summary>The labels shown instead of a sample's values.</summary>
    public IReadOnlyList<string>? SampleLabels { get; set; }

    /// <inheritdoc />
    public override JsonObject ApiInfo => new()
    {
        ["type"] = "integer",
        ["description"] = "index of selected example",
    };

    /// <inheritdoc />
    public override JsonNode? ExampleInputs => JsonValue.Create(0);

    /// <summary>What clicking a sample hands a handler.</summary>
    /// <param name="value">The index of the sample the user picked.</param>
    /// <returns>
    /// The sample's values, its index, or both — depending on <see cref="Type"/> — or
    /// <see langword="null"/> when nothing was picked.
    /// </returns>
    /// <exception cref="ArgumentException">The value is not an index.</exception>
    /// <exception cref="ArgumentOutOfRangeException">There is no sample at that index.</exception>
    public override object? Preprocess(JsonNode? value)
    {
        if (value is null)
        {
            return null;
        }

        var index = JsonValues.Int32(value)
            ?? throw new ArgumentException(
                $"A dataset's value is the index of the sample picked; the client sent "
                + $"'{value.ToJsonString()}'.",
                nameof(value));

        // The Python version answers an index with itself before it looks the sample up, so a type that
        // wants only the index does not fail on an index the dataset no longer holds.
        if (Type == "index")
        {
            return index;
        }

        var sample = RawSample(index);

        return Type switch
        {
            "tuple" => (index, sample),
            "values" => sample,
            _ => null,
        };
    }

    /// <summary>The index of the sample a handler returned.</summary>
    /// <param name="value">An index, a sample's values, or none.</param>
    /// <returns>The index, or <see langword="null"/> when the sample is not one the dataset holds.</returns>
    /// <remarks>
    /// The Python version warns that a dataset cannot be updated by handing it a set of values; the
    /// answer is <see langword="null"/> and the application is expected to return a new dataset
    /// instead.
    /// </remarks>
    public override JsonNode? Postprocess(object? value)
    {
        if (value is null)
        {
            return null;
        }

        if (value is int or long)
        {
            return JsonValue.Create(Convert.ToInt32(value, System.Globalization.CultureInfo.InvariantCulture));
        }

        if (value is not IReadOnlyList<object?> sample)
        {
            return null;
        }

        var processed = Samples;

        for (var index = 0; index < processed.Count; index++)
        {
            if (Same(processed[index], sample))
            {
                return JsonValue.Create(index);
            }
        }

        return null;
    }

    /// <summary>Appends the components and their ids, as the Python version's config override
    /// does.</summary> <param name="props">The props being built.</param>
    protected internal override void WriteExtraProps(PropWriter props)
    {
        base.WriteExtraProps(props);

        var components = new JsonArray();

        foreach (var component in Components)
        {
            components.Add(new JsonObject
            {
                ["name"] = component.BlockClass,
                ["class_id"] = null,
            });
        }

        props.Add("components", components);
        props.Add("component_ids", new JsonArray(
            [.. Components.Select(component => (JsonNode?)JsonValue.Create(component.Id))]));
    }

    /// <summary>The raw sample at an index.</summary>
    /// <param name="index">The index.</param>
    /// <returns>The values the application wrote for that sample.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The dataset holds no sample at that index.</exception>
    private IReadOnlyList<object?> RawSample(int index)
    {
        var samples = _samples ?? [[]];

        if (index < 0 || index >= samples.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(index),
                index,
                $"The dataset holds {samples.Count} sample(s); there is none at that index.");
        }

        return samples[index];
    }

    /// <summary>A component's props, with the value left out.</summary>
    /// <param name="component">The component.</param>
    /// <returns>The copy a dataset carries.</returns>
    /// <remarks>
    /// The Python version copies the props its signature declares and drops <c>value</c>: a dataset
    /// draws the sample it was given rather than the component's own default.
    /// </remarks>
    private static JsonObject PropsOf(Component component)
    {
        var props = component.AllProps();
        props.Remove("value");

        return props;
    }

    /// <summary>A component's label, or <see langword="null"/> when it has none.</summary>
    /// <param name="component">The component.</param>
    private static string? LabelOf(Component component) =>
        component.AllProps()["label"] is JsonValue label && label.TryGetValue<string>(out var text)
            ? text
            : null;

    /// <summary>Whether two samples hold the same values.</summary>
    private static bool Same(IReadOnlyList<object?> left, IReadOnlyList<object?> right)
    {
        if (left.Count != right.Count)
        {
            return false;
        }

        for (var index = 0; index < left.Count; index++)
        {
            if (!JsonNode.DeepEquals(
                JsonValues.FromAnyObject(left[index]),
                JsonValues.FromAnyObject(right[index])))
            {
                return false;
            }
        }

        return true;
    }
}

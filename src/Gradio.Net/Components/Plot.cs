using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A plot as the frontend receives it: which library drew it, and the drawing itself.
/// </summary>
/// <param name="Type">The library: <c>altair</c>, <c>bokeh</c>, <c>plotly</c> or <c>matplotlib</c>.</param>
/// <param name="Plot">
/// The drawing, as that library's own JSON — or, for a matplotlib figure, the image's base64.
/// </param>
/// <remarks>
/// <para>
/// The Python version's <c>PlotData</c> in <c>plot.py</c>: a pydantic model with these two fields.
/// It is a record here rather than a bare dictionary so that a handler can return one and a handler
/// can be handed one, and so that the library's name is checked once rather than at every use.
/// </para>
/// <para>
/// This is what makes the component a passthrough: the Python version converts a matplotlib,
/// plotly, altair or bokeh object into this shape on the server, and this framework has none of
/// those libraries — an application that has a drawing already brings it in this shape.
/// </para>
/// </remarks>
public sealed record PlotData(string Type, string Plot);

/// <summary>
/// A plot drawn from a drawing an application already has.
/// </summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. The Python version turns a figure from one of four
/// Python plotting libraries into the JSON the frontend draws; there are no such libraries here, so
/// the value is that JSON already — a <see cref="PlotData"/>, or an object with its two fields.
/// </para>
/// <para>
/// What is not carried over is the conversion itself: a matplotlib figure is encoded to an image
/// with <c>format</c> as its file type, a plotly or altair chart is asked for its own JSON, and a
/// bokeh model is embedded with its library's version. Those are the plotting libraries' work, and
/// an application on this side does it where it built the chart.
/// </para>
/// </remarks>
[GradioBlock("plot")]
[HandlerValue("PlotData?", "change")]
public sealed partial class Plot : Component
{
    internal Plot()
    {
    }

    private object? _value;

    /// <summary>The drawing the plot starts with.</summary>
    /// <value>A <see cref="PlotData"/>, an object with its two fields, or <see langword="null"/>.</value>
    public object? Value
    {
        get => Postprocess(_value);
        set => _value = value;
    }

    /// <summary>
    /// The file format a figure is sent in.
    /// </summary>
    /// <value>
    /// <c>webp</c>, which is the Python version's default for speed; it is read when a figure is
    /// encoded to an image, which this framework leaves to the application.
    /// </value>
    public string Format { get; set; } = "webp";

    /// <summary>The label drawn above the plot.</summary>
    public override string? Label { get; set; }

    /// <summary>The relative width of the plot next to its neighbours.</summary>
    public int? Scale { get; set; }

    /// <summary>The width below which the plot wraps.</summary>
    public int? MinWidth { get; set; } = 160;

    /// <summary>The buttons drawn in the plot's corner.</summary>
    /// <value>No buttons unless the application asks for one.</value>
    /// <remarks>
    /// Names only, for now: the Python version takes <c>gr.Button</c> instances here, and a button
    /// is a block whose events are its own batch. A name the frontend knows is written as it is,
    /// which is what a plot with a fullscreen or export affordance needs.
    /// </remarks>
    public IReadOnlyList<string> Buttons { get; set; } = [];

    /// <summary>
    /// The version of the bokeh library the frontend loads to draw a bokeh plot.
    /// </summary>
    /// <value>
    /// <see langword="null"/>, which is what the Python version reports when bokeh is not installed
    /// — and there is no bokeh here to install.
    /// </value>
    /// <remarks>
    /// Not a constructor parameter: the Python version's <c>get_config</c> adds it after the
    /// parameters, which is where this build writes it too. It stays a member rather than being
    /// dropped because the frontend reads it to decide whether it can draw a bokeh plot at all, and
    /// an application that knows which version its frontend should load can say so.
    /// </remarks>
    public string? BokehVersion { get; set; }

    /// <inheritdoc />
    public override JsonObject ApiInfo => DataSchemas.PlotSchema();

    /// <summary>The drawing a handler reads.</summary> <param name="value">What the client sent:
    /// the two fields of a plot.</param> <returns>The drawing, or <see langword="null"/> when the
    /// plot holds none.</returns> <exception cref="ArgumentException">The library is not one the
    /// Python version supports.</exception>
    public override object? Preprocess(JsonNode? value) => value is JsonObject document
        ? new PlotData(Kind(JsonValues.Text(document["type"])), JsonValues.Text(document["plot"]) ?? string.Empty)
        : null;

    /// <summary>The drawing the client receives from what a handler returned.</summary>
    /// <param name="value">A <see cref="PlotData"/>, an object with its two fields, or none.</param>
    /// <returns>The drawing as the protocol carries it, or <see langword="null"/> for none.</returns>
    /// <exception cref="ArgumentException">
    /// The value is of another shape, or names a library the Python version does not draw.
    /// </exception>
    public override JsonNode? Postprocess(object? value) => value switch
    {
        null => null,
        PlotData plot => Write(plot),
        JsonObject document => Write(new PlotData(
            Kind(JsonValues.Text(document["type"])),
            JsonValues.Text(document["plot"]) ?? string.Empty)),
        _ => throw new ArgumentException(
            "A plot is a PlotData — the library that drew it and the drawing itself — or an object "
            + $"with those two fields. It was handed a {value.GetType()}.",
            nameof(value)),
    };

    /// <summary>A drawing as the protocol carries it.</summary> <param name="plot">The
    /// drawing.</param> <returns>The two fields the Python version's model declares, in its
    /// order.</returns> <exception cref="ArgumentException">The library is not one of the four the
    /// Python version draws.</exception>
    private static JsonObject Write(PlotData plot) => new()
    {
        ["type"] = Kind(plot.Type),
        ["plot"] = plot.Plot,
    };

    /// <summary>The library a plot names, checked against the four the Python version
    /// draws.</summary> <param name="type">The library's name, or <see langword="null"/> when none
    /// was given.</param> <returns>The name.</returns> <exception cref="ArgumentException">The name
    /// is not one of the four.</exception>
    private static string Kind(string? type) => type is "altair" or "bokeh" or "plotly" or "matplotlib"
        ? type
        : throw new ArgumentException(
            $"A plot is drawn by 'altair', 'bokeh', 'plotly' or 'matplotlib'; it named '{type}'.",
            nameof(type));

    /// <summary>The bokeh version, written after the parameters as the Python version writes
    /// it.</summary> <param name="props">The props being built.</param>
    protected internal override void WriteExtraProps(PropWriter props)
    {
        ArgumentNullException.ThrowIfNull(props);

        props.Add("bokeh_version", BokehVersion);
    }
}

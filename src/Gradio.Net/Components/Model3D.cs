using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A 3D model: uploaded, or shown in a viewer the user can turn around.
/// </summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. Three of them describe the scene rather than the file:
/// the way it is drawn, the colour behind it, and where the camera starts — which is a triple whose
/// entries may each be absent, because an application that only wants to set the radius should not
/// have to invent two angles.
/// </para>
/// <para>
/// The component takes a path and gives one back: a model is not decoded, converted or validated
/// here, and the viewer does that in the browser.
/// </para>
/// </remarks>
[GradioBlock("model3d")]
[HandlerValue("string", "change", "upload", "edit", "clear")]
public sealed partial class Model3D : Component
{
    internal Model3D()
    {
    }

    /// <summary>The sample model the Python version's examples point at.</summary>
    private const string SampleModel =
        "https://raw.githubusercontent.com/gradio-app/gradio/main/gradio/media_assets/models3d/Fox.gltf";

    private object? _value;

    /// <summary>The model the component starts with.</summary>
    /// <value>A path or a URL; read back as the postprocessed value — a file object.</value>
    public object? Value
    {
        get => Postprocess(_value);
        set => _value = value;
    }

    /// <summary>How the model is drawn.</summary>
    /// <value><c>solid</c>, <c>point_cloud</c>, <c>wireframe</c>, or <see langword="null"/> for the viewer's own.</value>
    public string? DisplayMode { get; set; }

    /// <summary>The colour behind the model, as red, green, blue and alpha between 0 and 1.</summary>
    public IReadOnlyList<double> ClearColor { get; set; } = [0, 0, 0, 0];

    /// <summary>The camera's starting position: two angles in degrees and a distance.</summary>
    /// <value>Three entries, any of which may be absent.</value>
    public IReadOnlyList<double?> CameraPosition { get; set; } = [null, null, null];

    /// <summary>How fast the wheel zooms.</summary>
    public double ZoomSpeed { get; set; } = 1;

    /// <summary>How fast dragging pans.</summary>
    public double PanSpeed { get; set; } = 1;

    /// <summary>The height of the component, in pixels or as a CSS length.</summary>
    public object? Height { get; set; }

    /// <summary>The label drawn above the component.</summary>
    public override string? Label { get; set; }

    /// <summary>The relative width of the component next to its neighbours.</summary>
    public int? Scale { get; set; }

    /// <summary>The width below which the component wraps.</summary>
    public int? MinWidth { get; set; } = 160;

    /// <summary>The buttons drawn in the component's corner.</summary>
    public IReadOnlyList<string> Buttons { get; set; } = [];

    /// <inheritdoc />
    public override JsonObject ApiInfo => MediaSchemas.FileSchema();

    /// <inheritdoc />
    public override JsonNode? ExampleInputs => new JsonObject
    {
        ["path"] = SampleModel,
        ["meta"] = new JsonObject { ["_type"] = FileData.FileMetaType },
        ["orig_name"] = "Fox.gltf",
        ["url"] = SampleModel,
    };

    /// <inheritdoc />
    protected internal override IEnumerable<string> KeptFiles => Paths(Value);

    /// <summary>
    /// The value a handler reads from this component.
    /// </summary>
    /// <param name="value">What the client sent: a file object, or a path.</param>
    /// <returns>The path of the cached model, or <see langword="null"/> when there is none.</returns>
    public override object? Preprocess(JsonNode? value) => MediaFiles.PathOf(value, MediaFolder, Root);

    /// <summary>
    /// The value this component receives from what a handler returned.
    /// </summary>
    /// <param name="value">A path, a URL, the model's bytes, a stream of them, or a file object.</param>
    /// <returns>The model as the protocol carries it, or <see langword="null"/> for none.</returns>
    public override JsonNode? Postprocess(object? value)
    {
        var described = MediaFiles.From(value, MediaFolder, "model.glb", "A model component");

        if (described?.Path is not { Length: > 0 })
        {
            return null;
        }

        return MediaFiles.Describe(described, MediaFolder, Root, measure: false).ToJson();
    }

    /// <summary>The paths a value names, for the files that must outlive reclamation.</summary>
    private static IEnumerable<string> Paths(object? value) => value switch
    {
        JsonObject file when JsonValues.Text(file["path"]) is { Length: > 0 } path
            && !MediaCache.NamesALocation(path) => [path],
        _ => [],
    };
}

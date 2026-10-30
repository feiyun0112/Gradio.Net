using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// How the webcam is opened: whether its preview is mirrored, and the constraints it asks for.
/// </summary>
/// <remarks>
/// The Python version's dataclass of the same name, serialized into the config as the object the
/// frontend reads. The constraints are passed through as JSON because they are the browser's
/// <c>MediaTrackConstraints</c> rather than anything this framework interprets.
/// </remarks>
public sealed class WebcamOptions : IConfigValue
{
    internal WebcamOptions()
    {
    }

    /// <summary>Whether the preview is mirrored, which is what makes a selfie look like one.</summary>
    public bool Mirror { get; set; } = true;

    /// <summary>The constraints the camera is opened with, or <see langword="null"/> for the defaults.</summary>
    public JsonNode? Constraints { get; set; }

    /// <inheritdoc />
    public JsonNode ToConfigValue() => new JsonObject
    {
        ["mirror"] = Mirror,
        ["constraints"] = Constraints?.DeepClone(),
    };
}

/// <summary>
/// What an image's watermark looks like and where it goes.
/// </summary>
/// <remarks>
/// The Python version's dataclass of the same name. The watermark is a path here rather than an
/// image object, because a config carries a path and a path is what an application writes; drawing
/// one is the image layer's job, and it happens when a component writes an image out.
/// </remarks>
public sealed class WatermarkOptions : IConfigValue
{
    /// <summary>The positions a watermark may be placed at, by name.</summary>
    public static readonly IReadOnlyList<string> Positions =
    [
        "top-left",
        "top-right",
        "bottom-left",
        "bottom-right",
    ];

    private string _position = "bottom-right";

    /// <summary>The image to draw, or <see langword="null"/> for none.</summary>
    public string? Watermark { get; set; }

    /// <summary>Where the watermark is drawn.</summary>
    /// <value>
    /// One of <see cref="Positions"/>; <c>bottom-right</c> unless the application says otherwise.
    /// </value>
    /// <remarks>
    /// The Python version also accepts a pixel position as a pair of integers. The config carries
    /// the named corners, which is what an application writes there; the pair is a position like
    /// any other to the drawing code — <c>RasterImage.Watermark</c> reads <c>"x,y"</c> — so a value
    /// that names one is applied rather than refused.
    /// </remarks>
    public string Position
    {
        get => _position;
        set
        {
            if (!Positions.Contains(value, StringComparer.Ordinal))
            {
                throw new ArgumentException(
                    $"A watermark's position must be one of {string.Join(", ", Positions)}; got '{value}'.",
                    nameof(value));
            }

            _position = value;
        }
    }

    /// <inheritdoc />
    public JsonNode ToConfigValue() => new JsonObject
    {
        ["watermark"] = Watermark,
        ["position"] = Position,
    };
}

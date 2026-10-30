using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// How wide the eraser draws, in pixels or as the automatic size.
/// </summary>
/// <remarks>
/// The Python version's <c>gr.Eraser</c>. The size is either a number or the word <c>auto</c>,
/// which the frontend turns into a fraction of the image's smaller side — a union the config
/// carries as it stands, which is why the value is <see cref="object"/> rather than a number with a
/// sentinel.
/// </remarks>
public sealed class EraserOptions : IConfigValue
{
    internal EraserOptions()
    {
    }

    /// <summary>The word that asks for a size derived from the image.</summary>
    public const string Auto = "auto";

    /// <summary>The radius the eraser starts at, in pixels or <see cref="Auto"/>.</summary>
    public object DefaultSize { get; set; } = Auto;

    /// <inheritdoc />
    public JsonNode ToConfigValue() => new JsonObject { ["default_size"] = Size(DefaultSize) };

    /// <summary>A size as the config writes it: a number, or the word the frontend understands.</summary>
    /// <param name="size">The size.</param>
    /// <returns>The JSON value.</returns>
    internal static JsonNode? Size(object? size) => size switch
    {
        null => null,
        int number => JsonValue.Create(number),
        double number => JsonValue.Create(number),
        string text => JsonValue.Create(text),
        _ => throw new ArgumentException(
            $"A tool's size is a number of pixels or '{Auto}'; got a {size.GetType()}.",
            nameof(size)),
    };
}

/// <summary>
/// How the brush draws: its width, the colours offered, and whether the user may pick another.
/// </summary>
/// <remarks>
/// The Python version's <c>gr.Brush</c>, which extends the eraser's options with a palette. The
/// palette and the colour it starts on are computed when the application sets neither, so an
/// application that only says <c>new Brush()</c> gets the Python version's five colours and the
/// first of them.
/// </remarks>
public sealed class BrushOptions : IConfigValue
{
    /// <summary>The colours the Python version offers when the application names none.</summary>
    public static readonly IReadOnlyList<string> DefaultColors =
    [
        "rgb(204, 50, 50)",
        "rgb(173, 204, 50)",
        "rgb(50, 204, 112)",
        "rgb(50, 112, 204)",
        "rgb(173, 50, 204)",
    ];

    private IReadOnlyList<object?>? _colors;
    private object? _defaultColor;

    /// <summary>The radius the brush starts at, in pixels or <see cref="EraserOptions.Auto"/>.</summary>
    public object DefaultSize { get; set; } = EraserOptions.Auto;

    /// <summary>The colours offered, each a CSS colour or a colour and an opacity.</summary>
    /// <value>The Python version's five colours when the application names none.</value>
    public IReadOnlyList<object?> Colors
    {
        get => _colors ?? [.. DefaultColors];
        set => _colors = value;
    }

    /// <summary>The colour the brush starts with.</summary>
    /// <value>The first colour of <see cref="Colors"/> unless the application says otherwise.</value>
    public object? DefaultColor
    {
        get => _defaultColor ?? (Colors.Count > 0 ? Colors[0] : null);
        set => _defaultColor = value;
    }

    /// <summary>Whether the user may pick a colour of their own, or only the ones offered.</summary>
    /// <value><c>defaults</c> to offer the palette and a picker, <c>fixed</c> to offer only the palette.</value>
    public string ColorMode { get; set; } = "defaults";

    /// <inheritdoc />
    public JsonNode ToConfigValue() => new JsonObject
    {
        ["default_size"] = EraserOptions.Size(DefaultSize),
        ["colors"] = Palette(Colors),
        ["default_color"] = EraserOptions.Size(DefaultColor),
        ["color_mode"] = ColorMode,
    };

    /// <summary>The palette as the config writes it.</summary>
    private static JsonArray Palette(IReadOnlyList<object?> colors)
    {
        var array = new JsonArray();

        foreach (var color in colors)
        {
            array.Add(EraserOptions.Size(color));
        }

        return array;
    }
}

/// <summary>
/// The layer tool's options: whether layers may be added, and which ones there are.
/// </summary>
/// <remarks>
/// The Python version's <c>gr.LayerOptions</c>. An application that passes <c>layers=True</c> gets
/// the default options and one layer named <c>Layer 1</c>; passing <c>False</c> gets the same
/// options with the tool disabled, which is why <see cref="Hidden"/> is an instance rather than a
/// null.
/// </remarks>
public sealed class LayerOptions : IConfigValue
{
    private IReadOnlyList<string>? _layers;

    /// <summary>Whether the user may add layers of their own.</summary>
    public bool AllowAdditionalLayers { get; set; } = true;

    /// <summary>The layers the editor starts with.</summary>
    /// <value>One layer named <c>Layer 1</c> when the application names none or names an empty list.</value>
    public IReadOnlyList<string> Layers
    {
        get => _layers is { Count: > 0 } ? _layers : ["Layer 1"];
        set => _layers = value;
    }

    /// <summary>Whether the layer tool is hidden.</summary>
    public bool Disabled { get; set; }

    /// <summary>The options of a hidden layer tool, which is what <c>layers=False</c> means.</summary>
    public static LayerOptions Hidden => new() { Disabled = true };

    /// <inheritdoc />
    public JsonNode ToConfigValue() => new JsonObject
    {
        ["allow_additional_layers"] = AllowAdditionalLayers,
        ["layers"] = new JsonArray([.. Layers.Select(layer => JsonValue.Create(layer))]),
        ["disabled"] = Disabled,
    };
}

/// <summary>
/// How an audio component draws its waveform and what its skip buttons skip.
/// </summary>
/// <remarks>
/// The Python version's <c>gr.WaveformOptions</c>. The three colours are left unset by default,
/// which the config writes as null: the frontend then paints them from the theme.
/// </remarks>
public sealed class WaveformOptions : IConfigValue
{
    /// <summary>The colour of the whole waveform, or <see langword="null"/> for the theme's.</summary>
    public string? WaveformColor { get; set; }

    /// <summary>The colour the played part of the waveform is filled with.</summary>
    public string? WaveformProgressColor { get; set; }

    /// <summary>The colour of the region being trimmed.</summary>
    public string? TrimRegionColor { get; set; }

    /// <summary>Whether a waveform is drawn while recording, rather than the browser's own player.</summary>
    public bool ShowRecordingWaveform { get; set; } = true;

    /// <summary>The share of the audio the skip buttons move, as a percentage.</summary>
    public double SkipLength { get; set; } = 5;

    /// <summary>The sample rate the audio is rendered at, in hertz.</summary>
    public int SampleRate { get; set; } = 44100;

    /// <inheritdoc />
    public JsonNode ToConfigValue() => new JsonObject
    {
        ["waveform_color"] = WaveformColor,
        ["waveform_progress_color"] = WaveformProgressColor,
        ["trim_region_color"] = TrimRegionColor,
        ["show_recording_waveform"] = ShowRecordingWaveform,
        ["skip_length"] = SkipLength,
        ["sample_rate"] = SampleRate,
    };
}

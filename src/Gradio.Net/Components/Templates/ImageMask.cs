namespace Gradio.Net;
/// <summary>
/// The image component the Python version calls <c>ImageMask</c>: an editor whose value is a mask
/// drawn over a background.
/// </summary>
/// <remarks>
/// Three defaults apart from <see cref="ImageEditor"/>: the sources are all three, the brush is one
/// fixed black colour, and layers start off — a mask is one surface, not a stack of them.
/// </remarks>
[GradioBlock("imagemask")]
public sealed partial class ImageMask : ImageEditor
{
    /// <summary>Builds the editor as a single masked surface with a fixed black brush.</summary>
    internal ImageMask()
    {
        Sources = ["upload", "webcam", "clipboard"];
        Brush = new BrushOptions { Colors = ["#000000"], ColorMode = "fixed" };

        // The Python version's `layers=False`: one surface, and the config carries the flag rather than a
        // list of layers, which is what the frontend reads it as.
        Layers = false;
    }

    /// <summary>Whether the block is drawn by its parent's component.</summary>
    protected override bool IsTemplate => true;
}

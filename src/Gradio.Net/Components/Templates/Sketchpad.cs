namespace Gradio.Net;
/// <summary>
/// The image component the Python version calls <c>Sketchpad</c>: an editor that starts from
/// nothing and draws with a fixed black brush.
/// </summary>
/// <remarks>
/// Two defaults apart from <see cref="ImageEditor"/>: <c>sources</c> is empty, and the brush is one
/// black colour the user cannot change — which is what makes a drawing a drawing rather than a
/// painting.
/// </remarks>
[GradioBlock("sketchpad")]
public sealed partial class Sketchpad : ImageEditor
{
    /// <summary>Builds the editor with no source to start from, and one black brush.</summary>
    internal Sketchpad()
    {
        Sources = [];
        Brush = new BrushOptions { Colors = ["#000000"], ColorMode = "fixed" };
    }

    /// <summary>Whether the block is drawn by its parent's component.</summary>
    protected override bool IsTemplate => true;
}

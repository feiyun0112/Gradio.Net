namespace Gradio.Net;
/// <summary>
/// The image component the Python version calls <c>Paint</c>: an editor that starts from nothing
/// and offers no background.
/// </summary>
/// <remarks>
/// One default apart from <see cref="ImageEditor"/>: <c>sources</c> is empty, so there is no upload,
/// no webcam and no clipboard — the canvas is the point.
/// </remarks>
[GradioBlock("paint")]
public sealed partial class Paint : ImageEditor
{
    /// <summary>Builds the editor with no source to start from.</summary>
    internal Paint() => Sources = [];

    /// <summary>Whether the block is drawn by its parent's component.</summary>
    protected override bool IsTemplate => true;
}

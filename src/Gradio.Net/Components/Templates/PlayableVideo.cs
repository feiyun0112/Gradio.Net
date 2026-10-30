namespace Gradio.Net;
/// <summary>
/// The video component the Python version calls <c>PlayableVideo</c>: one that plays what it is
/// given rather than offering a camera.
/// </summary>
/// <remarks>
/// Two defaults apart from <see cref="Video"/>: the format is <c>mp4</c>, and the only source is an
/// upload — which is the whole difference between a video you watch and a video you record. The
/// Python version goes one step further and overwrites the source its caller wrote; here the
/// constructor starts from one upload and a caller that names <see cref="Video.Sources"/> gets what
/// it asked for, because a parameter that is silently ignored is worse than one that is honoured.
/// </remarks>
[GradioBlock("playablevideo")]
public sealed partial class PlayableVideo : Video
{
    /// <summary>Builds the component so that it plays an uploaded file.</summary>
    internal PlayableVideo()
    {
        Format = "mp4";
        Sources = ["upload"];
    }

    /// <summary>Whether the block is drawn by its parent's component.</summary>
    protected override bool IsTemplate => true;
}

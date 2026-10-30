namespace Gradio.Net;
/// <summary>
/// The audio component the Python version calls <c>Microphone</c>: one that records rather than
/// uploads
/// </summary>
/// <remarks>
/// Two defaults apart from <see cref="Audio"/>: <c>sources</c> is the microphone alone, so the
/// component offers recording and nothing else, and the file it writes is a wav — the Python
/// version's <c>format</c> for a recording, where an audio component's own default is to keep what
/// it was given. The Python version exports it twice — <c>Mic</c> is the same class — and both
/// spellings build this.
/// </remarks>
[GradioBlock("microphone")]
public sealed partial class Microphone : Audio
{
    /// <summary>Builds the component so that it records.</summary>
    internal Microphone()
    {
        Sources = ["microphone"];
        Format = "wav";
    }

    /// <summary>Whether the block is drawn by its parent's component.</summary>
    protected override bool IsTemplate => true;
}

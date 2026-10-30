namespace Gradio.Net;
/// <summary>
/// The file component the Python version calls <c>Files</c>: one that takes several files
/// </summary>
/// <remarks>
/// <para>
/// One default apart from <see cref="File"/>: <c>file_count</c> is <c>multiple</c>, so the handler
/// is given a list rather than a single file. The Python version spells the pair <c>File</c> and
/// <c>Files</c> because the difference is the shape of the value, and a port that reads either name
/// gets the shape it asked for.
/// </para>
/// <para>
/// The parameters are the base's, served in this class's own order, which is what the parameter
/// manifest records for <c>files</c>.
/// </para>
/// </remarks>
[GradioBlock("files")]
public sealed partial class Files : File
{
    /// <summary>Builds the component so that it takes several files.</summary>
    internal Files() => FileCount = "multiple";

    /// <summary>Whether the block is drawn by its parent's component.</summary>
    protected override bool IsTemplate => true;
}

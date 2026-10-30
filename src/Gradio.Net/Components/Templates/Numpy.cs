namespace Gradio.Net;
/// <summary>
/// The table component the Python version calls <c>Numpy</c>: a dataframe whose value is a table of
/// numbers
/// </summary>
/// <remarks>
/// <para>
/// One default apart from <see cref="Dataframe"/>: <c>type</c> is <c>numpy</c>. The Python
/// version's three table templates exist because the type decides what the handler is given — a
/// table of numbers here, an array of arrays for <see cref="Matrix"/> and <see cref="List"/> — and
/// the rest of the component is the same one.
/// </para>
/// <para>
/// This build's tables carry rows and columns as arrays either way, because the library types are
/// excluded, so the difference a port can observe is the <c>type</c> the config carries.
/// </para>
/// </remarks>
[GradioBlock("numpy")]
public sealed partial class Numpy : Dataframe
{
    /// <summary>Builds the table so that it reads as an array of arrays.</summary>
    internal Numpy() => Type = "numpy";

    /// <summary>Whether the block is drawn by its parent's component.</summary>
    protected override bool IsTemplate => true;
}

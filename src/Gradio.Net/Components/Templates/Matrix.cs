namespace Gradio.Net;
/// <summary>
/// The table component the Python version calls <c>Matrix</c>: a dataframe read as an array of
/// arrays
/// </summary>
/// <remarks>
/// One default apart from <see cref="Dataframe"/>: <c>type</c> is <c>array</c>. It is the same
/// template as <see cref="Numpy"/> with the other spelling of the value, which is why the Python
/// version has both rather than one component with a flag.
/// </remarks>
[GradioBlock("matrix")]
public sealed partial class Matrix : Dataframe
{
    /// <summary>Builds the table so that it reads as an array of arrays.</summary>
    internal Matrix() => Type = "array";

    /// <summary>Whether the block is drawn by its parent's component.</summary>
    protected override bool IsTemplate => true;
}

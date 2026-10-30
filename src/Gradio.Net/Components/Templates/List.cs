namespace Gradio.Net;
/// <summary>
/// The table component the Python version calls <c>List</c>: a dataframe with exactly one column
/// </summary>
/// <remarks>
/// <para>
/// Two defaults apart from <see cref="Dataframe"/>: <c>type</c> is <c>array</c> and there is one
/// column, which is what makes a table a list of rows.
/// </para>
/// <para>
/// The name is the Python version's and is kept, so a port that writes <c>gr.List(...)</c> writes
/// the same thing here. That is why the templates live in a namespace of their own: a class called
/// <c>List</c> beside the components would take <see cref="System.Collections.Generic.List{T}"/>
/// away from every file in that namespace, and the collision would be reported as a hundred
/// unrelated errors. Here it costs a <c>using</c>.
/// </para>
/// </remarks>
[GradioBlock("list")]
public sealed partial class List : Dataframe
{
    /// <summary>Builds the table with one column, read as an array of arrays.</summary>
    internal List()
    {
        Type = "array";
        ColCount = 1;
    }

    /// <summary>Whether the block is drawn by its parent's component.</summary>
    protected override bool IsTemplate => true;
}

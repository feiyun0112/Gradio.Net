namespace Gradio.Net;
/// <summary>
/// A layout that draws its children one above another.
/// </summary>
/// <remarks>
/// <para>
/// The parameters are the Python version's, and the props are generated from them in the Python
/// version's order. <see cref="Scale"/> and <see cref="MinWidth"/> are what a row divides its width
/// by: two columns of scale 2 and 1 are two thirds and one third wide, unless the narrower one
/// would fall below its minimum, in which case the minimum wins and the row wraps.
/// </para>
/// <para>
/// A <see cref="Variant"/> of <c>compact</c> switches off the grouping of form components, for the
/// same reason a compact row does.
/// </para>
/// </remarks>
[GradioBlock("column")]
public sealed partial class Column : BlockContext
{
    internal Column()
    {
    }

    /// <summary>The width this column takes next to its neighbours in a row.</summary>
    public int Scale { get; set; } = 1;

    /// <summary>The width below which the column wraps.</summary>
    public int MinWidth { get; set; } = 320;

    /// <summary>How the column is drawn: <c>default</c>, <c>panel</c> or <c>compact</c>.</summary>
    public string Variant { get; set; } = "default";

    /// <summary>Whether a progress animation is shown while a child is updated.</summary>
    public bool ShowProgress { get; set; }

    /// <inheritdoc />
    protected internal override bool AllowsExpectedParents => Variant != "compact";
}

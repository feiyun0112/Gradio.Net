namespace Gradio.Net;
/// <summary>
/// A layout that draws its children side by side.
/// </summary>
/// <remarks>
/// <para>
/// The parameters are the Python version's, and the props are generated from them in the Python
/// version's order. <see cref="Scale"/> is the height this row takes next to its neighbours rather
/// than the width a column takes, and a child column expands to fill it.
/// </para>
/// <para>
/// A <see cref="Variant"/> of <c>compact</c> switches off the grouping of form components: the
/// Python version draws a compact row as a single block, and a form inside it would be a second
/// border.
/// </para>
/// </remarks>
[GradioBlock("row")]
public sealed partial class Row : BlockContext
{
    internal Row()
    {
    }

    /// <summary>How the row is drawn: <c>default</c>, <c>panel</c> or <c>compact</c>.</summary>
    public string Variant { get; set; } = "default";

    /// <summary>The height the row takes next to its neighbours.</summary>
    public int? Scale { get; set; }

    /// <summary>The row's height: a number of pixels, or a CSS length.</summary>
    public object? Height { get; set; }

    /// <summary>The height the row grows to at most: a number of pixels, or a CSS length.</summary>
    public object? MaxHeight { get; set; }

    /// <summary>The height the row shrinks to at least: a number of pixels, or a CSS length.</summary>
    public object? MinHeight { get; set; }

    /// <summary>Whether every child is drawn at the same height.</summary>
    public bool EqualHeight { get; set; }

    /// <summary>Whether a progress animation is shown while a child is updated.</summary>
    public bool ShowProgress { get; set; }

    /// <inheritdoc />
    protected internal override bool AllowsExpectedParents => Variant != "compact";
}

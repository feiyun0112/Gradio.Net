namespace Gradio.Net;
/// <summary>
/// A collapsible panel that draws its children down one side of the page.
/// </summary>
/// <remarks>
/// <para>
/// The parameters are the Python version's; the props are generated from them in the Python
/// version's order. The label names the panel for the application rather than for the user: the
/// Python version does not draw it.
/// </para>
/// <para>
/// The two signals the frontend raises when the user opens or closes it are bindable like a
/// button's click — <c>sidebar.Expand(fn, inputs, outputs)</c> — and the trigger itself is
/// <c>sidebar.Events.Expand</c>.
/// </para>
/// </remarks>
[GradioBlock("sidebar")]
public sealed partial class Sidebar : BlockContext
{
    internal Sidebar()
    {
    }

    /// <summary>The panel's name, which is not shown to the user.</summary>
    public string? Label { get; set; }

    /// <summary>Whether the panel is open when the page loads.</summary>
    public bool Open { get; set; } = true;

    /// <summary>The panel's width: a number of pixels, or a CSS length.</summary>
    public object? Width { get; set; } = 320;

    /// <summary>Which side of the page the panel is drawn on: <c>left</c> or <c>right</c>.</summary>
    public string Position { get; set; } = "left";
}

namespace Gradio.Net;
/// <summary>
/// A layout whose children the user can drag into a different order.
/// </summary>
/// <remarks>
/// <para>
/// The parameters are the Python version's; the props are generated from them in the Python
/// version's order. A child that is itself a layout is dragged as one piece, so a dashboard of rows
/// or groups reorders as a dashboard rather than as a pile of components.
/// </para>
/// <para>
/// The new order is the frontend's: the block carries no value and raises no signal, so nothing on
/// the server has to be told about a drag.
/// </para>
/// <para>
/// The Python version ships this layout in <c>gradio.layouts.draggable</c> but does not export it
/// from the <c>gradio</c> namespace — the release's export list has no <c>Draggable</c> name —
/// so there is no <c>gr.Draggable()</c> to mirror and this port deliberately has no
/// <c>gr.Draggable()</c> factory either. The type is public so the frontend's component
/// description can be produced; its constructor is <see langword="internal"/> because a layout is
/// built by a factory that also enters its scope, and this one has no public factory to do that.
/// Adding one would put a name on <c>gr</c> that the Python version does not have, which the
/// surface comparison would rightly report as a difference.
/// </para>
/// </remarks>
[GradioBlock("draggable")]
public sealed partial class Draggable : BlockContext
{
    internal Draggable()
    {
    }

    /// <summary>How the children are arranged: <c>column</c> or <c>row</c>.</summary>
    public string Orientation { get; set; } = "column";
}

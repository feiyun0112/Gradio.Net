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

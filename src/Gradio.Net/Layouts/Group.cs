namespace Gradio.Net;
/// <summary>
/// A layout that draws its children as one block, without padding or margin between them.
/// </summary>
/// <remarks>
/// The parameters are the Python version's; the props are generated from them in the Python
/// version's order. A group holds no value and raises no signal of its own, so it is only ever a
/// node in the layout tree.
/// </remarks>
[GradioBlock("group")]
public sealed partial class Group : BlockContext
{
    internal Group()
    {
    }

}

namespace Gradio.Net;
/// <summary>
/// The text field the Python version calls <c>TextArea</c>: a textbox that is seven lines tall
/// </summary>
/// <remarks>
/// <para>
/// The Python version keeps these in <c>gradio.templates</c> rather than in
/// <c>gradio.components</c>, and they are what that module is: a component whose constructor sets
/// one or two of the values its base takes. This one differs from <see cref="Textbox"/> in two
/// defaults and in nothing else — the height it is drawn at, and how far it grows.
/// </para>
/// <para>
/// It is a template in the config's sense as well: the frontend draws it with the textbox
/// component, so <c>props.name</c> and <c>type</c> are <c>textbox</c> while the class is this one
/// (see <see cref="Block.BlockClass"/>).
/// </para>
/// <para>
/// The parameters are the base's, and the order they are served in is this class's own — the Python
/// version serialises a block by walking the parameters of the class's own constructor, and this
/// one takes <c>type</c> later in its list than the textbox does. That order is what the parameter
/// manifest records for <c>textarea</c>, and it is what the generated factory and the config both
/// follow.
/// </para>
/// </remarks>
[GradioBlock("textarea")]
public sealed partial class TextArea : Textbox
{
    /// <summary>Builds the field at the height the Python version draws it.</summary>
    internal TextArea()
    {
        Lines = 7;
        MaxLines = 20;
    }

    /// <summary>Whether the block is drawn by its parent's component.</summary>
    protected override bool IsTemplate => true;
}

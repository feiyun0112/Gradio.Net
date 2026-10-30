namespace Gradio.Net;
/// <summary>
/// A container that groups form components so that they are drawn and sized as one block.
/// </summary>
/// <remarks>
/// <para>
/// A form component written straight into a container is grouped into one of these at the end of
/// the render, so an application that never mentions <see cref="Form"/> still serves one — which
/// is why this layout exists in the batch that introduces the first form component rather than
/// with the rest of the layouts.
/// </para>
/// <para>
/// A form is a layout node and nothing else. It carries no events, it holds no value, and grouping
/// neither adds nor removes a binding: what makes several fields "submitted together" is one binding
/// that names all of them as its inputs. The form decides only where those fields are drawn and how
/// wide the block around them is.
/// </para>
/// </remarks>
[GradioBlock("form")]
public sealed partial class Form : BlockContext
{
    internal Form()
    {
    }

    /// <summary>The relative width of the form next to its neighbours.</summary>
    /// <remarks>
    /// A synthesised form starts at zero and grows by the width of the components it holds, which
    /// is how the Python version sizes one.
    /// </remarks>
    public int Scale { get; set; }

    /// <summary>The width below which the form wraps.</summary>
    public int MinWidth { get; set; }

    /// <summary>
    /// Adds a component the grouping put in this form, and widens the form by it.
    /// </summary>
    /// <param name="child">The component being grouped.</param>
    /// <remarks>
    /// The Python version's <c>Form.add_child</c>. A form has no width of its own: it is as wide as
    /// the components it was created for, which is why a form synthesised inside a row takes a
    /// share of that row rather than the whole of it. The three cases are the Python version's — in
    /// a row, both widths count; in a column of an equal-height row, only the scale does, because
    /// the row has already fixed the widths; directly in an application that fills its height, the
    /// scale does and an unstated one contributes nothing.
    /// </remarks>
    internal override void AddGroupedChild(Block child)
    {
        var sized = child as FormComponent;

        switch (Parent)
        {
            case Row:
                Scale += sized?.Scale ?? 1;
                MinWidth += sized?.MinWidth ?? 0;
                break;

            case Column { Parent: Row { EqualHeight: true } }:
                Scale += sized?.Scale ?? 1;
                break;

            case Blocks { FillHeight: true }:
                Scale += sized?.Scale ?? 0;
                break;
        }

        base.AddGroupedChild(child);
    }
}

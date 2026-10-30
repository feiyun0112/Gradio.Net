namespace Gradio.Net;
/// <summary>
/// A component that belongs in a form: written directly into a container, it is grouped into a
/// synthesised form rather than laid out on its own.
/// </summary>
/// <remarks>
/// <para>
/// This is the Python version's grouping rule, and it is why a demo that writes two textboxes
/// straight into a <c>Blocks</c> serves a config with a <c>form</c> in it that the author never
/// wrote. The grouping itself runs when the render ends
/// (<see cref="BlockContext.FillExpectedParents"/>).
/// </para>
/// <para>
/// A component with <see cref="Component.Container"/> turned off expects no container, which is
/// how a component opts out of being grouped.
/// </para>
/// </remarks>
public abstract class FormComponent : Component
{
    /// <summary>
    /// The share of the width around it this component takes next to its neighbours.
    /// </summary>
    /// <value>
    /// <see langword="null"/> — "whatever one share is" — which is what the Python version reads a
    /// component that says nothing as. It is declared here because the form the grouping
    /// synthesises sizes itself out of the components it holds, and a form can only hold form
    /// components.
    /// </value>
    public virtual int? Scale { get; set; }

    /// <summary>The width below which this component wraps.</summary>
    /// <value><see langword="null"/> for a component that names no minimum.</value>
    public virtual int? MinWidth { get; set; }

    /// <inheritdoc />
    protected internal override string? GetExpectedParentKind() => Container ? "form" : null;
}

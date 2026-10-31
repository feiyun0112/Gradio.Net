namespace Gradio.Net;
/// <summary>
/// A layout whose content is shown or hidden by a header the user can click.
/// </summary>
/// <remarks>
/// <para>
/// The parameters are the Python version's; the props are generated from them in the Python
/// version's order. <see cref="Open"/> decides whether the content is shown when the page loads,
/// and the height bounds apply to the content rather than to the header, so an accordion whose
/// content is taller than <see cref="MaxHeight"/> scrolls.
/// </para>
/// <para>
/// The two signals the frontend raises when the user opens or closes it are bindable like a
/// button's click — <c>accordion.Expand(fn, inputs, outputs)</c> — and the trigger itself is
/// <c>accordion.Events.Expand</c>.
/// </para>
/// </remarks>
[GradioBlock("accordion")]
public sealed partial class Accordion : BlockContext
{
    internal Accordion()
    {
    }

    /// <summary>The name shown in the header, or <see langword="null"/> for a header with no name.</summary>
    public string? Label { get; set; }

    /// <summary>Whether the content is shown when the page loads.</summary>
    public bool Open { get; set; } = true;

    /// <summary>The content's height: a number of pixels, or a CSS length.</summary>
    public object? Height { get; set; }

    /// <summary>The content's maximum height: a number of pixels, or a CSS length.</summary>
    public object? MaxHeight { get; set; }
}

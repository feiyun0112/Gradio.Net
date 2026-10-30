namespace Gradio.Net;
public static partial class gr
{
    /// <summary>Creates an application, and opens the scope its blocks are written into.</summary>
    /// <param name="title">The title the page carries, or <see langword="null"/> for the Python
    /// version's.</param> <returns>The application itself, not a wrapper around it.</returns>
    /// <remarks>
    /// <para>
    /// The return value is a <see cref="Blocks"/>, which is what lets the Python version's
    /// <c>with gr.Blocks() as demo:</c> land on <c>using (var demo = gr.Blocks())</c> without a
    /// second thing to unwrap: the object written into is the object launched, and leaving the
    /// <c>using</c> block closes the render scopes rather than destroying the application
    /// (see <see cref="Blocks.Dispose"/> for why that is the meaning it has).
    /// </para>
    /// <para>
    /// The render is opened here rather than by the caller, because that is the half of the Python
    /// version's <c>with</c> the <c>using</c> cannot spell: <c>using</c> says what happens at the
    /// closing brace, and nothing about the opening one. So the factory opens it, and what is
    /// written between the braces is written into this application — <c>gr.Textbox(...)</c> is a
    /// line of the page, and <c>using (gr.Row())</c> is a layout and the lines under it — with no
    /// second call to make. A caller that wants to write more after the block has ended asks for
    /// <see cref="Blocks.BeginRender"/> itself, which is what writing a second time means.
    /// </para>
    /// <para>
    /// The cost of that is the one a <c>using</c> always has: an application nobody disposes keeps
    /// its render open for the rest of the flow, and the ambient context with it. The Python this
    /// mirrors has the same shape — a <c>with</c> that is never entered never ends — and the remedy
    /// is the same one: the block.
    /// </para>
    /// <para>
    /// The accessor is this application's own, and <see cref="Launch"/> hands the same one to the
    /// host it starts. Two accessors would be two ambient contexts, and a block written into one
    /// would be invisible to the other.
    /// </para>
    /// <para>
    /// The Python version's other constructor parameters — the theme, the stylesheets, the
    /// analytics switch — are ticket 44's; this one takes the title because a page without one is
    /// hard to tell from another.
    /// </para>
    /// </remarks>
    public static Blocks Blocks(string? title = null)
    {
        // The container factory is what `AddGradio` registers for a host, and an application built
        // outside a host has to supply it itself: without it a form component has no form to be
        // grouped into, and the render fails when the scope closes rather than when it is written.
        var application = new Blocks(new RenderContextAccessor(), new BlockContainerFactory());

        if (title is not null)
        {
            application.Title = title;
        }

        // The Python version's `with`, whose opening brace is this line. The handle is the application's to
        // close: `Blocks.Dispose` ends every render still open, which is the closing brace. The
        // render is also what makes the factories place what they build — `gr.Textbox(...)` is a
        // line on this page for as long as it is the page being written (see `Ambient`).
        application.BeginRender();
        Ambient.Open(application.ContextAccessor);

        return application;
    }
}

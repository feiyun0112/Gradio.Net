namespace Gradio.Net;
public static partial class gr
{
    /// <summary>Builds the Python version's <c>navbar</c>: a <see cref="Navbar"/>, with the Python
    /// version's parameters in the Python version's order.</summary> <returns>The block, which is
    /// the core type rather than a wrapper around it.</returns>
    /// <remarks>
    /// <para>
    /// Written by hand rather than generated, and it is the only factory that is: the generated
    /// ones are named after the names in <c>gradio.__all__</c>, and this one is not in it. The
    /// Python version has the class on the module all the same — <c>gr.Navbar()</c> is the line its
    /// own multipage guide writes, and the multipage page is built with it — so leaving it out
    /// would leave a port with no way to say what a page's navigation is.
    /// </para>
    /// <para>
    /// The signature is compared to the recorded one exactly as the generated ones are
    /// (<c>GrParametersTests</c>, <c>GrDefaultsTests</c>), which is what keeps a hand-written list
    /// honest: a parameter the Python version adds fails a test rather than going missing.
    /// </para>
    /// <para>
    /// The block is placed on the page being written, like every other factory — the Python
    /// version's call is a line on the page, not a value.
    /// </para>
    /// </remarks>
    public static Navbar Navbar(
        IReadOnlyList<NavbarLink>? value = null,
        object? visible = null,
        object? mainPageName = null,
        string? elemId = null,
        object? elemClasses = null,
        bool render = true,
        object? key = null)
    {
        var block = new Navbar();

        if (value is { } valueGiven)
        {
            block.Value = valueGiven;
        }

        if (visible is { } visibleGiven)
        {
            block.Visible = Values.VisibilityOf(visibleGiven);
        }

        if (mainPageName is { } mainPageNameGiven)
        {
            block.MainPageName = mainPageNameGiven;
        }

        if (elemId is { } elemIdGiven)
        {
            block.ElemId = elemIdGiven;
        }

        if (elemClasses is { } elemClassesGiven)
        {
            block.ElemClasses = Values.Strings(elemClassesGiven);
        }

        block.Render = render;

        if (key is { } keyGiven)
        {
            block.Key = keyGiven;
        }

        return Ambient.Place(block);
    }
}

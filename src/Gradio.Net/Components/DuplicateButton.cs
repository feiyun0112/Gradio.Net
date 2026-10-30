namespace Gradio.Net;
/// <summary>
/// A button that asks Hugging Face to duplicate the Space the application is running in.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's own documentation says what this does: nothing locally, and on a Space it
/// opens the duplication page in a new tab. The button binds that click for itself, and only when
/// the process says it is running in a Space — <c>SPACE_ID</c> — which is the same test the Python
/// version makes. A Gradio.Net application is not served by Hugging Face Spaces, so in practice the
/// binding is never made and the button is what its name says and nothing more; it exists because
/// an application ported from Python may still write one.
/// </para>
/// <para>
/// <c>_activate</c> turns the binding off. The Python version has it so that a framework can build
/// the button and decide later; it is not a prop of the config, because the Python version takes it
/// and never stores it.
/// </para>
/// </remarks>
[GradioBlock("duplicatebutton")]
public sealed partial class DuplicateButton : Button
{
    internal DuplicateButton()
    {
    }

    /// <summary>Whether the button binds its own click when it is added to an application.</summary>
    /// <value><see langword="true"/>, which is what makes the button do anything at all.</value>
    [UpstreamParameter("_activate")]
    [OmitFromConfig]
    public bool AutoActivate { get; set; } = true;

    /// <summary>Whether the block is drawn by its parent's component.</summary> <remarks>The Python
    /// version marks this button a template: the frontend draws it with the button component, and
    /// the config names that component rather than this class.</remarks>
    protected override bool IsTemplate => true;

    /// <summary>The text drawn when the application names none.</summary>
    protected override string? DefaultValue => "Duplicate Space";

    /// <summary>The variant drawn when the application names none.</summary>
    protected override string DefaultVariant => "huggingface";

    /// <summary>The size drawn when the application names none.</summary>
    protected override string DefaultSize => "sm";

    /// <summary>The width taken when the application names none.</summary>
    /// <value>Zero, which is what makes the button as wide as its text.</value>
    protected override int? DefaultScale => 0;

    /// <inheritdoc />
    protected internal override void OnAddedTo(Blocks root)
    {
        base.OnAddedTo(root);

        if (AutoActivate)
        {
            Activate();
        }
    }

    /// <summary>Binds the click that opens the duplication page, when the application runs in a Space.</summary>
    /// <remarks>
    /// <para>
    /// The address is the Space's own, so the binding is a fragment of JavaScript rather than a
    /// handler: the server has nothing to do with the duplication, and the browser is the one that
    /// opens the tab.
    /// </para>
    /// <para>
    /// Whether the process is a Space is the Python version's test and both halves of it are
    /// needed: the platform says <c>SYSTEM=spaces</c>, and <c>SPACE_ID</c> names the Space. A host
    /// that sets only the second is not a Space, and the Python version does not bind anything for
    /// it either.
    /// </para>
    /// </remarks>
    public void Activate()
    {
        if (Environment.GetEnvironmentVariable("SYSTEM") != "spaces"
            || Environment.GetEnvironmentVariable("SPACE_ID") is not { Length: > 0 } space)
        {
            return;
        }

        Click(
            fn: null,
             js: $"() => {{ window.open(`https://huggingface.co/spaces/{space}?duplicate=true`, '_blank') }}");
    }
}

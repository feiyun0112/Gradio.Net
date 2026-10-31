namespace Gradio.Net;
/// <summary>
/// One tab of a <see cref="Tabs"/>: the label on its button, and the content shown when it is
/// selected.
/// </summary>
/// <remarks>
/// <para>
/// The parameters are the Python version's; the props are generated from them in the Python
/// version's order. The block's type in the config is <c>tabitem</c> rather than <c>tab</c>, which
/// is the Python version's spelling of it: the frontend dispatches on that name, and it is not the
/// class name the rest of the props report.
/// </para>
/// <para>
/// <see cref="RenderChildren"/> asks the frontend to render the content while the tab is inactive,
/// so that a video or an audio player inside it has loaded by the time the user clicks. What this
/// release has to do is carry the parameter to the frontend unchanged, and that is all it does: the
/// Python version's own 6.30.0 bundle never reads it — the parameter exists on the Python side and
/// in the config, and the frontend of that release has no code that consults it — so "the same
/// behaviour as the Python version" is exactly "the prop reaches the frontend". The case records
/// both a tab with it and a tab without, so the field is compared in both states.
/// </para>
/// <para>
/// The tab raises one signal, <c>select</c>, and it binds the way every other signal does:
/// <c>tab.Select(fn, inputs, outputs)</c> binds it and <c>tab.Events.Select</c> is the trigger. What
/// the handler is told about the pick — the label, and whether the tab became selected — arrives as
/// a <see cref="SelectData"/>, which is the Python version's payload for the event.
/// </para>
/// </remarks>
[GradioBlock("tab")]
[HandlerValue("SelectData", EventNames.Select)]
public sealed partial class Tab : BlockContext
{
    internal Tab()
    {
    }

    private string _alignment = "left";

    /// <summary>The label drawn on the tab's button.</summary>
    public string? Label { get; set; }

    /// <summary>Whether the user can click the tab.</summary>
    public bool Interactive { get; set; } = true;

    /// <summary>
    /// The identifier an application selects this tab by, or <see langword="null"/> for none.
    /// </summary>
    /// <remarks>
    /// Named apart from <see cref="Block.Id"/>, which is the id the render gives the block: a tab
    /// that is selected from a handler needs an id its author chose, and the two are different
    /// numbers in the same config entry.
    /// </remarks>
    [UpstreamParameter("id")]
    public object? TabId { get; set; }

    /// <summary>Which side of the tab bar the tab is drawn on: <c>left</c> or <c>right</c>.</summary>
    /// <exception cref="ArgumentException">The value is neither.</exception>
    public string Alignment
    {
        get => _alignment;
        set
        {
            if (value is not ("left" or "right"))
            {
                throw new ArgumentException(
                    "The alignment must be either 'left' or 'right'.",
                    nameof(Alignment));
            }

            _alignment = value;
        }
    }

    /// <summary>The tab's size next to its neighbours.</summary>
    public int? Scale { get; set; }

    /// <summary>The content's height: a number of pixels, or a CSS length.</summary>
    public object? Height { get; set; }

    /// <summary>The content's maximum height: a number of pixels, or a CSS length.</summary>
    public object? MaxHeight { get; set; }

    /// <summary>Whether the content is rendered while the tab is inactive.</summary>
    public bool RenderChildren { get; set; }

    /// <inheritdoc />
    public override string BlockType => "tabitem";

    /// <inheritdoc />
    protected internal override string? GetExpectedParentKind() => "tabs";
}

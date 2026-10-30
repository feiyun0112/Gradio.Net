namespace Gradio.Net;
/// <summary>
/// A layout that draws one of its tabs at a time, and a bar to choose which.
/// </summary>
/// <remarks>
/// <para>
/// The parameters are the Python version's; the props are generated from them in the Python
/// version's order. <see cref="Selected"/> names the tab shown when the page loads — by its label
/// or by the id the tab was given — and defaults to the first one.
/// </para>
/// <para>
/// A tab written straight into an application is grouped into a tabs of its own, which is why this
/// layout is a grouping target: the grouping rule asks a container to be the kind its child
/// expects, and the kind a tab expects is this one.
/// </para>
/// </remarks>
[GradioBlock("tabs")]
public sealed partial class Tabs : BlockContext
{
    internal Tabs()
    {
    }

    private string _overflowBehavior = "menu";

    /// <summary>The tab shown when the page loads, by label or id; the first one by default.</summary>
    public object? Selected { get; set; }

    /// <summary>How tabs that do not fit are shown: <c>menu</c> or <c>wrap</c>.</summary>
    /// <exception cref="ArgumentException">The value is neither.</exception>
    /// <remarks>
    /// Refused where it is set rather than where the config is written, which is the Python
    /// version's behaviour: an application that asked for a third behaviour would otherwise serve a
    /// config the frontend cannot honour, and nothing would say so until someone looked at the
    /// page.
    /// </remarks>
    public string OverflowBehavior
    {
        get => _overflowBehavior;
        set
        {
            if (value is not ("menu" or "wrap"))
            {
                throw new ArgumentException(
                    "The overflow behaviour must be either 'menu' or 'wrap'.",
                    nameof(OverflowBehavior));
            }

            _overflowBehavior = value;
        }
    }

    /// <summary>The signal raised when the selected tab changes.</summary>
    public EventTrigger Change => new(this, EventNames.Change);

    /// <summary>The signal raised when the user selects a tab.</summary>
    public EventTrigger Select => new(this, EventNames.Select);
}

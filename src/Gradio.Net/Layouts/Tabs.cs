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
/// <para>
/// The two signals the frontend raises are bindable like a button's click, which is the Python
/// version's <c>tabs.select(fn, ...)</c>: <c>tabs.Select(...)</c> binds the user's pick and
/// <c>tabs.Change(...)</c> binds the selection changing at all. The triggers themselves are
/// <c>tabs.Events.Select</c> and <c>tabs.Events.Change</c>.
/// </para>
/// </remarks>
[GradioBlock("tabs")]
[HandlerValue("SelectData", EventNames.Select)]
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
}

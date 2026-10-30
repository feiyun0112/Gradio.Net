namespace Gradio.Net;
/// <summary>
/// A layout that walks the user through a series of steps, one at a time.
/// </summary>
/// <remarks>
/// <para>
/// The parameters are the Python version's; the props are generated from them in the Python
/// version's order. <see cref="Selected"/> names the step shown when the page loads, by the number
/// the step was given, and defaults to the first one.
/// </para>
/// <para>
/// A step written straight into an application is grouped into a walkthrough of its own, for the
/// same reason a tab is grouped into a tabs.
/// </para>
/// </remarks>
[GradioBlock("walkthrough")]
public sealed partial class Walkthrough : BlockContext
{
    internal Walkthrough()
    {
    }

    /// <summary>The step shown when the page loads, by number; the first one by default.</summary>
    public int? Selected { get; set; }

    /// <summary>The signal raised when the selected step changes.</summary>
    public EventTrigger Change => new(this, EventNames.Change);

    /// <summary>The signal raised when the user selects a step.</summary>
    public EventTrigger Select => new(this, EventNames.Select);
}

namespace Gradio.Net;
/// <summary>
/// One step of a <see cref="Walkthrough"/>.
/// </summary>
/// <remarks>
/// <para>
/// The parameters are the Python version's; the props are generated from them in the Python
/// version's order. The block is the Python version's <c>Step</c>: its props report <c>step</c> and
/// its type in the config is <c>walkthroughstep</c>. The C# type is named after the layout it
/// belongs to rather than after the word itself, because <c>Step</c> is a reserved word in some
/// .NET languages and a type named that way is a type those languages cannot spell.
/// </para>
/// <para>
/// A step written straight into an application is grouped into a walkthrough of its own, which is
/// why it names the container it expects.
/// </para>
/// </remarks>
[GradioBlock("step")]
public sealed partial class WalkthroughStep : BlockContext
{
    internal WalkthroughStep()
    {
    }

    /// <summary>The label drawn on the step's button.</summary>
    public string? Label { get; set; }

    /// <summary>Whether the user can click the step.</summary>
    public bool Interactive { get; set; } = true;

    /// <summary>
    /// The number an application selects this step by, or <see langword="null"/> for none.
    /// </summary>
    /// <remarks>Named apart from <see cref="Block.Id"/> for the same reason a tab's is.</remarks>
    [UpstreamParameter("id")]
    public object? StepId { get; set; }

    /// <summary>The step's size next to its neighbours.</summary>
    public int? Scale { get; set; }

    /// <inheritdoc />
    public override string BlockClass => "step";

    /// <inheritdoc />
    public override string BlockType => "walkthroughstep";

    /// <summary>The signal raised when the user selects the step.</summary>
    public EventTrigger Select => new(this, EventNames.Select);

    /// <inheritdoc />
    protected internal override string? GetExpectedParentKind() => "walkthrough";
}

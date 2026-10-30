namespace Gradio.Net;
/// <summary>
/// Names the Python block a class implements, so that the build can hold its parameter mapping
/// against the parameters the Python version declares.
/// </summary>
/// <remarks>
/// <para>
/// The name is the Python version's — <c>textbox</c>, <c>number</c>, <c>form</c> — and it is the
/// same name the config spells the block's type with, so the annotation cannot disagree with the
/// config it produces.
/// </para>
/// <para>
/// The parameter mapping itself is generated at compile time from the recorded Python manifest,
/// which is why this attribute exists rather than the class name being taken as the block name: the
/// annotation is where a mistake — a component this build has that the Python version does not, or
/// a name spelled differently — becomes a build error instead of a config that is quietly missing a
/// parameter.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
internal sealed class GradioBlockAttribute : Attribute
{
    /// <summary>Declares which the Python version block a class implements.</summary>
    /// <param name="block">The block name as the Python version spells it, e.g.
    /// <c>textbox</c>.</param>
    public GradioBlockAttribute(string block) => Block = block;

    /// <summary>The block name as the Python version spells it.</summary>
    public string Block { get; }
}

namespace Gradio.Net;
/// <summary>
/// Declares that a constructor parameter the Python version declares is not part of the config this
/// block serves, even though the block has to declare it.
/// </summary>
/// <remarks>
/// <para>
/// The Python version removes a few parameters from a block's props in the block's own
/// <c>get_config</c> — <c>State</c> deletes <c>value</c>, because a state's value belongs to the
/// session rather than to the page. The parameter still has to exist here: the build holds every
/// parameter the Python version declares against a member, and a component that simply omitted the
/// member would fail that check for the right reason but for the wrong outcome.
/// </para>
/// <para>
/// So the member exists and says the generated mapping does not write it. That is not the same as
/// saying the key is never served: <c>Block.WriteExtraProps</c> is where a block appends a key its
/// parameters do not carry, and a block may append the very key its member omits when the shape it
/// serves is not the shape the member holds — a dataset's <c>components</c> names a class rather
/// than carrying a component's props. What the Python version's removal means is recorded next to
/// the member it applies to, which is where a reviewer looking at the parameter list will see it.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
internal sealed class OmitFromConfigAttribute : Attribute;

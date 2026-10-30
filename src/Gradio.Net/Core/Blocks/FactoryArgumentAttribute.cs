namespace Gradio.Net;
/// <summary>
/// The shape of a parameter that is not the shape of the member it is written into.
/// </summary>
/// <remarks>
/// <para>
/// A <c>gr</c> factory's parameters are generated from the member each one writes: the parameter
/// takes the member's type, so a parameter list cannot disagree with the block it builds. Four of
/// the Python version's parameters are wider than that — <c>inputs</c> takes one component or
/// several, <c>elem_classes</c> and <c>preserved_by_key</c> take a string or a list of them, and
/// <c>sources</c> takes one source or a list — and the wider shape is what a port writes, so the
/// parameter has to keep it.
/// </para>
/// <para>
/// The wider shape is declared here, on the member, rather than inferred by the generator: a rule
/// that lived in the generator would be a rule nobody reading the component would find, and the
/// next member that needs it would be added without one.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property, Inherited = false)]
internal sealed class FactoryArgumentAttribute : Attribute
{
    /// <summary>Declares the shape a factory's parameter for this member takes.</summary>
    /// <param name="kind">The shape.</param>
    public FactoryArgumentAttribute(FactoryArgumentKind kind) => Kind = kind;

    /// <summary>The shape a factory's parameter for this member takes.</summary>
    public FactoryArgumentKind Kind { get; }
}

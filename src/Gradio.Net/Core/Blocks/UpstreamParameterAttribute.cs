namespace Gradio.Net;
/// <summary>
/// Names the Python constructor parameter a member stands for, when the member cannot carry its
/// name.
/// </summary>
/// <remarks>
/// <para>
/// The parameter mapping is generated from the Python version's parameter list, and a parameter is
/// mapped to the member of the same name in Pascal case. That rule has one blind spot: a parameter
/// whose name is already taken by the kernel. A <c>Tab</c> takes an <c>id</c> — the identifier an
/// application uses to select it — while <see cref="Block.Id"/> is the id the render gave the
/// block, and the two are different numbers that both appear in the config.
/// </para>
/// <para>
/// The attribute says which parameter a differently named member stands for, so that the mapping
/// stays complete and the collision is resolved by naming rather than by dropping the parameter.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class UpstreamParameterAttribute : Attribute
{
    /// <summary>Declares which the Python version parameter a member stands for.</summary>
    /// <param name="name">The parameter's name, as the Python version spells it.</param>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is
    /// <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="name"/> is empty or
    /// whitespace.</exception>
    public UpstreamParameterAttribute(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
    }

    /// <summary>The parameter's name, as the Python version spells it.</summary>
    public string Name { get; }
}

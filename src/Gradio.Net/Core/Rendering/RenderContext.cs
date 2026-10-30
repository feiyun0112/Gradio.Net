namespace Gradio.Net;
/// <summary>
/// The state of one render: which <see cref="Blocks"/> is currently being built.
/// </summary>
/// <remarks>
/// <para>
/// The context is ambient rather than a parameter. Passing it explicitly would put it into the
/// constructor of every component and layout, and would not survive the <c>using</c>-block style
/// the framework's API is modelled on.
/// </para>
/// <para>
/// It is carried by <see cref="IRenderContextAccessor"/> and lives only as long as the render
/// scope that established it, which is what lets several <see cref="Blocks"/> instances coexist
/// in one process without seeing each other's blocks.
/// </para>
/// </remarks>
public sealed class RenderContext
{
    /// <summary>Creates a context for a render that is building <paramref name="root"/>.</summary>
    /// <param name="root">The render root being built.</param>
    /// <exception cref="ArgumentNullException"><paramref name="root"/> is <see langword="null"/>.</exception>
    public RenderContext(Blocks root)
    {
        ArgumentNullException.ThrowIfNull(root);

        Root = root;
        Container = root;
    }

    /// <summary>The render root being built.</summary>
    public Blocks Root { get; }

    /// <summary>
    /// The container new blocks are added to: the innermost block whose scope is open — a layout, or
    /// a component that holds children — or the root when none is.
    /// </summary>
    /// <remarks>
    /// The Python version keeps the same thing on its render context — a stack of contexts, of
    /// which the innermost receives what is rendered next. It lives here rather than on the root
    /// because it is a property of the render in progress, and because a nested render (a scope
    /// entered inside another one) starts from its own root rather than inheriting the outer
    /// container.
    /// </remarks>
    public Block Container { get; internal set; }
}

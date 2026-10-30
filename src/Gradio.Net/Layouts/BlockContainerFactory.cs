namespace Gradio.Net;
/// <summary>
/// Turns the container kind a component names into the layout that provides it.
/// </summary>
/// <remarks>
/// This is the one place that knows both halves of the grouping rule: the kernel asks for a
/// container by name, and the layouts answer. Keeping the name in the kernel is what lets the
/// components that name a container stay free of the layouts.
/// </remarks>
public sealed class BlockContainerFactory : IBlockContainerFactory
{
    /// <inheritdoc />
    /// <exception cref="NotSupportedException"><paramref name="kind"/> is not a layout of this framework.</exception>
    /// <remarks>
    /// The kinds are the layouts a block can name as the container it expects: a form component
    /// expects a form, a tab expects tabs, a step expects a walkthrough. Every other layout is only
    /// ever created by the application that writes it.
    /// </remarks>
    public BlockContext Create(string kind) => kind switch
    {
        "form" => new Form(),
        "tabs" => new Tabs(),
        "walkthrough" => new Walkthrough(),
        _ => throw new NotSupportedException(
            $"A block expects a '{kind}' container, which is not a layout of this framework. "
            + "The layouts that can group components are named by the components that expect them."),
    };
}

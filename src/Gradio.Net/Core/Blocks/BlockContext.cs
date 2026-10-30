namespace Gradio.Net;
/// <summary>
/// A block that only holds other blocks: it carries no value, so it is never an event input or
/// output, and the config describes it as a node in the layout tree.
/// </summary>
/// <remarks>
/// <para>
/// The children are the render order, which is also the order the config lists them in. A container
/// the Python version synthesises to group form components is one of these too — it is created with
/// <c>render=False</c> and holds the components it was created for.
/// </para>
/// <para>
/// A container is <see cref="IDisposable"/> because entering it is a scope: disposing it leaves the
/// scope, which is what closing the Python version's <c>with gr.Row():</c> does. It is the same
/// meaning <see cref="Blocks.Dispose"/> has — "leave the scope", not "destroy the block" — and it
/// is what lets a caller write <c>using (gr.Row())</c> instead of holding the handle
/// <see cref="Blocks.Enter"/> returns. A container that was never entered has nothing to close, so
/// disposing it does nothing.
/// </para>
/// </remarks>
public abstract class BlockContext : Block, IDisposable
{
    private readonly List<Block> _children = [];
    private IDisposable? _openScope;

    /// <summary>The blocks directly inside this one, in render order.</summary>
    public IReadOnlyList<Block> Children => _children;

    /// <inheritdoc />
    protected internal override IReadOnlyList<Block>? LayoutChildren => _children;

    /// <summary>
    /// The parameters a re-render preserves.
    /// </summary>
    /// <value>Nothing, which is the Python version's default for a container: it holds no value to
    /// keep.</value>
    public IReadOnlyList<string> PreservedByKey { get; set; } = [];

    /// <summary>The kind of container this is, as a component names the container it expects.</summary>
    /// <value>
    /// The block class for a layout that can be a grouping target; <see langword="null"/> for one
    /// that cannot.
    /// </value>
    protected internal virtual string? ContainerKind => BlockClass;

    /// <inheritdoc />
    internal override bool HoldsChildren => true;

    /// <summary>
    /// Whether the components written straight into this container are grouped into the containers
    /// they expect.
    /// </summary>
    /// <value>
    /// <see langword="true"/> for every layout but one that says otherwise: the Python version's
    /// compact row and column switch the grouping off, because a compact layout is drawn as one
    /// block and a form inside it would be a second border.
    /// </value>
    protected internal override bool AllowsExpectedParents => true;

    /// <summary>Leaves the scope this container was entered with, if it is still open.</summary>
    /// <remarks>
    /// Closing is where the container's own grouping runs, and where adding goes back to the block
    /// around it. A container nobody entered is not a scope that was left open, so disposing it does
    /// nothing — which is what makes <c>using (gr.Group())</c> and a group that is only ever a node
    /// in the tree the same thing.
    /// </remarks>
    public virtual void Dispose()
    {
        var scope = _openScope;
        _openScope = null;
        scope?.Dispose();

        GC.SuppressFinalize(this);
    }

    /// <summary>Remembers the scope this container was entered with.</summary>
    /// <param name="scope">The handle <see cref="Blocks.Enter"/> handed out.</param>
    /// <remarks>
    /// Kept here rather than returned to the caller alone, because the container is the thing a
    /// caller has: <c>using (gr.Row())</c> disposes the row, and the row is what knows which scope
    /// that was.
    /// </remarks>
    internal void OpenedBy(IDisposable scope) => _openScope = scope;

    /// <summary>Forgets the scope, which has closed.</summary>
    internal void Closed() => _openScope = null;

    /// <summary>Adds a block as the last child of this one.</summary>
    /// <param name="child">The block to add.</param>
    /// <exception cref="ArgumentNullException"><paramref name="child"/> is <see langword="null"/>.</exception>
    internal override void AppendChild(Block child)
    {
        ArgumentNullException.ThrowIfNull(child);

        _children.Add(child);
    }

    /// <summary>
    /// Adds a block the grouping put here, rather than one the application wrote here.
    /// </summary>
    /// <param name="child">The block being grouped.</param>
    /// <remarks>
    /// The Python version has two ways in for the same reason: a layout may care about the
    /// components it is given by the grouping rule and not about the ones an application placed in
    /// it. A form is the one that does — its width is the sum of the components it holds — and the
    /// difference between "the application sized this form" and "the grouping sized it" is exactly
    /// the difference between the two methods.
    /// </remarks>
    internal virtual void AddGroupedChild(Block child) => AppendChild(child);

    /// <summary>
    /// Groups the children that expect a container into one, the way the Python version's
    /// <c>fill_expected_parents</c> does.
    /// </summary>
    /// <param name="createContainer">Makes a container of the expected kind and gives it its id.</param>
    /// <remarks>
    /// The rule itself is <see cref="ExpectedParents"/>'s, because a component that holds children
    /// runs it too. What is here is the answer a layout gives: its own kind, so that it does not
    /// group into itself, and its own children.
    /// </remarks>
    internal override void FillExpectedParents(Func<string, BlockContext> createContainer) =>
        ExpectedParents.Fill(_children, ContainerKind, this, createContainer);
}

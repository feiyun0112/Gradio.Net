namespace Gradio.Net;
/// <summary>
/// The grouping a container runs when its scope closes: the components that expect a container are
/// moved into one, the way the Python version's <c>fill_expected_parents</c> does.
/// </summary>
/// <remarks>
/// <para>
/// The Python version runs this when a context closes: a <c>Textbox</c> written directly inside a
/// <c>Blocks</c> is a form component, so it ends up inside a synthesised <c>Form</c> rather than at
/// the top level. The container is created once and the run of components that expect it is moved
/// into it.
/// </para>
/// <para>
/// A child that needs no container ends the run when it breaks grouping — which is what keeps a
/// button between two textboxes from swallowing the second one into the first's form.
/// </para>
/// <para>
/// A run never spans two pages. The Python version groups every child of a context in one pass, so
/// two components written on two different pages become one form that belongs to the first of them,
/// and the second page's component is served inside a container its page does not name. A page is
/// what the frontend renders and what a URL serves, so a container is made per page here; the
/// single-page case is untouched.
/// </para>
/// <para>
/// It lives here rather than on one container because two kinds of block run it: a layout, and a
/// component that holds children — the Python version's HTML is both a <c>BlockContext</c> and a
/// component, and its children are grouped exactly as a layout's are.
/// </para>
/// </remarks>
internal static class ExpectedParents
{
    /// <summary>Regroups a container's children in place.</summary>
    /// <param name="children">The children, which are replaced by the regrouped list.</param>
    /// <param name="containerKind">The kind of container this one is, so it does not group into itself.</param>
    /// <param name="parent">The container the new ones belong to.</param>
    /// <param name="createContainer">Makes a container of the expected kind and gives it its id.</param>
    internal static void Fill(
        List<Block> children,
        string? containerKind,
        Block parent,
        Func<string, BlockContext> createContainer)
    {
        ArgumentNullException.ThrowIfNull(children);
        ArgumentNullException.ThrowIfNull(parent);
        ArgumentNullException.ThrowIfNull(createContainer);

        var regrouped = new List<Block>(children.Count);
        BlockContext? pending = null;
        string? pendingKind = null;

        foreach (var child in children)
        {
            var expected = child.GetExpectedParentKind();
            if (expected is null || containerKind == expected)
            {
                if (child.BreaksGrouping)
                {
                    pending = null;
                    pendingKind = null;
                }

                regrouped.Add(child);
                continue;
            }

            if (pending is not null && pendingKind == expected && pending.PagePath == child.PagePath)
            {
                pending.AddGroupedChild(child);
                child.MarkParent(pending);
                continue;
            }

            var container = createContainer(expected);
            container.PagePath = child.PagePath;

            // The container is inside this one before it holds anything, because a layout may read
            // its own parent when it is handed a child — a form sizes itself against the row it was
            // put in, and the row is this container.
            container.MarkParent(parent);
            container.AddGroupedChild(child);
            child.MarkParent(container);
            regrouped.Add(container);

            pending = container;
            pendingKind = expected;
        }

        children.Clear();
        children.AddRange(regrouped);
    }
}

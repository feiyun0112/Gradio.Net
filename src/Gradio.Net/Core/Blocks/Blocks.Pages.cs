using System.Collections.ObjectModel;

namespace Gradio.Net;
/// <summary>
/// The pages of an application: declaring one, being on one, and the rules that say what one may
/// be called.
/// </summary>
/// <remarks>
/// <para>
/// A page is application data rather than framework data — an application with one page declares
/// nothing and still has the home page — so the parts of the root that exist because of pages live
/// here rather than beside the render. What they share with the rest of the root is the id counter
/// and the block list: a page's blocks are numbered in the same sequence as everyone else's.
/// </para>
/// <para>
/// A binding belongs to a page too, and its page is the one that was current when it was set up.
/// That is why a page scope can be entered outside a render: an application that binds its events
/// after building its UI still has to say which page each endpoint is on.
/// </para>
/// </remarks>
public sealed partial class Blocks
{
    private readonly List<Page> _pages = [Page.Home];
    private ReadOnlyCollection<Page>? _pagesView;

    /// <summary>The pages of this application, the home page first.</summary>
    /// <remarks>
    /// The Python version's <c>Blocks.pages</c>: what the config's <c>pages</c> array carries, and
    /// what the host serves a page endpoint for. The home page is always there and always first,
    /// which is why an application that never calls <see cref="Route"/> still has one page.
    /// </remarks>
    public IReadOnlyList<Page> Pages => _pagesView ??= _pages.AsReadOnly();

    /// <summary>The path of the page blocks are currently being added to.</summary>
    /// <value>The empty string — the home page — until <see cref="Route"/> says otherwise.</value>
    /// <remarks>
    /// A block records the page it was added on, and the config buckets components, bindings and
    /// the layout tree by that. The value is what <see cref="Route"/> sets for the length of its
    /// scope; it is not a property of the block, because the same block cannot be on two pages.
    /// </remarks>
    public string CurrentPagePath { get; private set; } = string.Empty;

    /// <summary>
    /// Adds a page to this application, and makes it the page blocks are added to.
    /// </summary>
    /// <param name="name">The name the navbar shows.</param>
    /// <param name="path">
    /// The URL suffix the page is served at. Derived from <paramref name="name"/> when omitted:
    /// lower-cased, with spaces turned into hyphens.
    /// </param>
    /// <param name="showInNavbar">
    /// Whether the navbar offers the page. A page that is not offered is still served at its path.
    /// </param>
    /// <returns>A scope that ends the page: adding goes back to the page that was current before.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// The name derives no path, or the path contains a character a URL suffix cannot carry, or it
    /// names one of the application's own routes.
    /// </exception>
    /// <remarks>
    /// <para>
    /// The Python version's <c>Blocks.route</c>. A page is a name, a URL suffix and whether the
    /// navbar lists it; the blocks written while it is current are the page's, and the config
    /// buckets the components, the bindings and the layout tree by it.
    /// </para>
    /// <para>
    /// The scope is also how a binding joins a page: a binding is set up on the page that is
    /// current, which is how an application says that an endpoint belongs to one page rather than
    /// to the application. A page scope may therefore be entered outside a render — after the
    /// render, where bindings usually are — and nothing is added to the tree by doing so.
    /// </para>
    /// <para>
    /// The Python version refuses <c>route()</c> inside a <c>Blocks</c> context, because there the
    /// page is written after the application's own block has ended. Here it is the opposite: blocks
    /// can only be added while a render is open, so a page is written inside one — the shape is
    /// <c>using (blocks.BeginRender()) { …; using (blocks.Route("About")) { … } }</c>. What is
    /// being built is the same application, page for page.
    /// </para>
    /// <para>
    /// The page stays current after its scope ends, which is what the Python version does:
    /// <c>route</c> sets the current page and nothing resets it, so a binding set up after the last
    /// page is written belongs to that page — on both sides. Ending a scope therefore does not mean
    /// "back to the application": it ends the page's grouping (see <see cref="ClosePage"/>) and
    /// gives back the layout that was open, and the page a block belongs to is whatever was
    /// declared last.
    /// </para>
    /// <para>
    /// A path that collides with an existing page is prefixed with an underscore rather than
    /// refused, which is the Python version's rule. A path the application gave that names one of
    /// its own routes is refused, because the application cannot serve a page there; a path
    /// <em>derived</em> from the name is prefixed instead, which is also the Python version's rule
    /// — a page called "Login" is served at <c>/_login</c> rather than refused.
    /// </para>
    /// </remarks>
    public IDisposable Route(string name, string? path = null, bool showInNavbar = true)
    {
        ArgumentNullException.ThrowIfNull(name);
        ThrowIfSealed();

        var suffix = path is null ? PageRoutes.FromName(name) : PageRoutes.FromPath(path);

        if (suffix.Length == 0)
        {
            throw new ArgumentException(
                $"The route '{name}' derives no URL suffix; give it a name of letters and digits, "
                + "or pass the path explicitly.",
                nameof(name));
        }
        // A path the application named is refused when the application already serves it: no
        // amount of renaming would make the page reachable there. A path derived from the name is
        // prefixed instead, which is the Python version's rule for the same collision.
        if (path is not null && PageRoutes.IsInternalRoute(suffix))
        {
            throw new ArgumentException(
                $"The path '{suffix}' is a route this application already serves, so a page cannot "
                + "be served there.",
                nameof(path));
        }

        // Entering a page that already exists is not declaring it twice: the page is the one it
        // already was, and the scope says which page what follows belongs to. The Python version has no such
        // case because a page is entered once, while it is built; here a page is also entered to
        // bind an event on it, which may happen after everything has been rendered.
        var existing = _pages.FirstOrDefault(page => page.Path == suffix && page.Name == name);
        if (existing is not null)
        {
            ClosePage();
            CurrentPagePath = existing.Path;

            return new PageHandle(this);
        }

        while (_pages.Any(page => page.Path == suffix)
            || (path is null && PageRoutes.IsInternalRoute(suffix)))
        {
            suffix = "_" + suffix;
        }

        ClosePage();

        _pages.Add(new Page(suffix, name, showInNavbar));
        CurrentPagePath = suffix;

        return new PageHandle(this);
    }

    /// <summary>
    /// Ends a page: groups what was written straight into the root for it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Python version runs the root's grouping at the end of every <c>with</c> block on the
    /// application, and a page is one of them — which is why the form a page's trailing form
    /// components are grouped into is numbered where that page ends rather than after the whole
    /// application. Nothing here is a scope of its own, so the boundary is the page's: entering a
    /// page closes the one that was being written, and leaving it closes it in turn.
    /// </para>
    /// <para>
    /// Running it twice is running it once: a container it already made has no expected parent of
    /// its own, so a second pass finds nothing left to group.
    /// </para>
    /// </remarks>
    private void ClosePage()
    {
        if (_rendered || !ReferenceEquals(_accessor.Current?.Root, this))
        {
            return;
        }

        FillExpectedParents(CreateContainer);
    }

    /// <summary>
    /// Refuses a second block on one page where the frontend can only draw one.
    /// </summary>
    /// <remarks>
    /// The Python version's <c>validate_navbar_settings</c>, said in terms of the rule rather than
    /// of the component: the navbar configures the page it is on, so two of them would be two
    /// answers to one question. The block that says it may only appear once is the one that knows
    /// it.
    /// </remarks>
    /// <exception cref="InvalidOperationException">A page holds two blocks that allow only one.</exception>
    private void CheckSinglePerPageBlocks()
    {
        var seen = new HashSet<(string Page, Type Block)>();

        foreach (var block in _blocks)
        {
            if (!block.SinglePerPage)
            {
                continue;
            }

            if (!seen.Add((block.PagePath, block.GetType())))
            {
                throw new InvalidOperationException(
                    $"Only one {block.GetType().Name} can exist per page, and page "
                    + $"'{PageLabel(block.PagePath)}' has more than one.");
            }
        }
    }

    /// <summary>A page's name for a message: the home page has none, so it is called what it is.</summary>
    private static string PageLabel(string page) => page.Length == 0 ? "Home" : page;

    /// <summary>The scope <see cref="Route"/> hands out: it ends the page when it ends.</summary>
    /// <remarks>
    /// It does not put the page back. The Python version's <c>with</c> block leaves the page it
    /// entered current, so what is written after the last page belongs to that page, and a scope
    /// that restored the page would describe a different application for the same code.
    /// </remarks>
    private sealed class PageHandle : IDisposable
    {
        private readonly Blocks _owner;
        private bool _disposed;

        internal PageHandle(Blocks owner) => _owner = owner;

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            // Leaving a page ends it, the way the Python version's `with` block does: what was written
            // straight into the root for it is grouped before the next page's blocks are numbered.
            _owner.ClosePage();
        }
    }
}

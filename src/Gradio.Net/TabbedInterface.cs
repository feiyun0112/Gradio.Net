using System.Globalization;
using System.Text;
namespace Gradio.Net;
/// <summary>
/// One tab of a <see cref="TabbedInterface"/>: how its content is drawn, and whether that content
/// fills the height of the page.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>interface_list</c> holds the applications themselves, because a Python
/// <c>Interface</c> is a <c>Blocks</c> that can be rendered into another one after the fact. Here a
/// container hosts one application, so a tab holds the <em>recipe</em> for its content
/// instead: an action that draws the tab's blocks into the application being built.
/// </para>
/// <para>
/// <see cref="FillHeight"/> is the Python version's <c>interface.fill_height</c>: a tab whose
/// content fills the height is given a scale of 1, and one whose content does not is given none, so
/// that the tab bar shares the height between them the way the Python version's does. The Python
/// version reads that off the application the tab holds; the content here is a recipe, so what it
/// says is read off the application it drew into, and <see langword="null"/> — the default — is
/// "whatever the content said".
/// </para>
/// </remarks>
public sealed class TabContent
{
    /// <summary>Describes one tab's content.</summary>
    /// <param name="render">Draws the tab's blocks into the application being built.</param>
    /// <param name="fillHeight">
    /// Whether the content fills the height of the page, or <see langword="null"/> to ask the
    /// content, which is the Python version's reading of <c>interface.fill_height</c>.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="render"/> is <see langword="null"/>.</exception>
    public TabContent(Action<Blocks> render, bool? fillHeight = null)
    {
        ArgumentNullException.ThrowIfNull(render);

        Render = render;
        FillHeight = fillHeight;
    }

    /// <summary>Draws this tab's blocks into the application being built.</summary>
    public Action<Blocks> Render { get; }

    /// <summary>Whether this tab's content fills the height of the page.</summary>
    /// <value>
    /// <see langword="null"/> to ask the content, which is where an application normally leaves it:
    /// an interface or a chat that fills the height says so where it is built, and that is what the
    /// tab reports. A value here is for content that cannot say — blocks written straight into the
    /// tab — or that has to be sized against what it draws.
    /// </value>
    public bool? FillHeight { get; }
}

/// <summary>
/// Several applications behind one tab bar, so that they can be compared side by side
/// </summary>
/// <remarks>
/// <para>
/// This is the Python version's <c>gr.TabbedInterface</c>: a heading, a <see cref="Tabs"/> holding
/// one <see cref="Tab"/> per application, and the applications themselves drawn inside those tabs.
/// Everything it draws is an ordinary block, so an application that outgrows it can be rebuilt by
/// hand and will describe itself the same way.
/// </para>
/// <para>
/// It renders into an application rather than being one, for the reason <see cref="Interface"/>
/// does: the render root is resolved from the container — one container hosting one application
/// — so the composition is handed the application to render into.
/// </para>
/// <para>
/// The order of what it draws is the Python version's: the heading first, then the tab bar, then
/// each tab followed by the blocks written into it. What it cannot reproduce is the Python
/// version's numbering: a Python interface is a <c>Blocks</c> of its own and is numbered before the
/// tabs that hold it, while here the tabs and their contents are numbered as they are drawn. The
/// config therefore describes the same application with the same blocks in the same order, under
/// ids of its own.
/// </para>
/// </remarks>
public sealed class TabbedInterface
{
    /// <summary>The heading drawn above the tabs, in the Python version's own markup.</summary>
    private static readonly CompositeFormat Heading =
        CompositeFormat.Parse("<h1 style='text-align: center; margin-bottom: 1rem'>{0}</h1>");

    /// <summary>The name a tab is given when the application names none.</summary>
    /// <remarks>
    /// The Python version numbers these from zero — <c>f"Tab {i}" for i in range(...)</c> — so the
    /// first tab of an unnamed set says "Tab 0". Reproduced as it is, because the name is served.
    /// </remarks>
    private static readonly CompositeFormat DefaultTabName = CompositeFormat.Parse("Tab {0}");

    /// <summary>Builds the tab bar and draws each tab's content into <paramref name="blocks"/>.</summary>
    /// <param name="blocks">The application to render into.</param>
    /// <param name="contents">
    /// One entry per tab, in the order the tabs are drawn. Each draws one application's blocks.
    /// </param>
    /// <param name="tabNames">
    /// The label of each tab, or <see langword="null"/> for "Tab 0", "Tab 1", and so on. Fewer
    /// names than contents means the tabs left without one are not drawn, which is the Python
    /// version's <c>zip</c>; more names than contents means the extra names are ignored.
    /// </param>
    /// <param name="title">
    /// The heading drawn above the tab bar, which is also the page's title. <see langword="null"/>
    /// or empty for no heading, and for the page title the Python version's default — "Gradio".
    /// </param>
    /// <param name="tabs">
    /// The tab bar to use, configured the way the application wants it — the Python version's
    /// <c>tabs_kwargs</c> — or <see langword="null"/> for a default one.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="blocks"/> or <paramref name="contents"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">An entry of <paramref name="contents"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// <para>
    /// The application's own settings — what kind of application it is, what it is called, and that
    /// it fills the height — are written after the tabs are drawn rather than before, because each
    /// tab's content is an application in its own right and claims those settings for the one it
    /// was built into. The last word belongs to the composition, which is the Python version's
    /// outcome too: its tabs were separate applications, so they never had a say in what the tabbed
    /// one is.
    /// </para>
    /// <para>
    /// A tab's scale is read while the tabs are being drawn, because it is what that tab's content
    /// said about the height: the Python version asks the interface it holds, and here the question
    /// is asked of the application the content drew into. The application's own value is therefore
    /// cleared before each tab's content is drawn and read once it is done, so that a tab which
    /// says nothing about the height does not inherit the answer the tab before it gave. What is
    /// read is then thrown away with the rest: the composition claims the height for the whole
    /// application at the end.
    /// </para>
    /// <para>
    /// Two of those settings are the composition's silence rather than its word: a tab's content can
    /// also choose the temporary-file schedule and whether telemetry is on, and the tabbed interface
    /// has no opinion about either. Those are read before the tabs are drawn and put back
    /// afterwards, so that a tab which scheduled its own cleanup does not start cleaning up the
    /// whole application.
    /// </para>
    /// <para>
    /// A tabbed interface is rendered once, when it is constructed: the application is built by the
    /// call, so a second one would describe a second application.
    /// </para>
    /// </remarks>
    internal TabbedInterface(
        Blocks blocks,
        IReadOnlyList<TabContent> contents,
        IReadOnlyList<string>? tabNames = null,
        string? title = null,
        Tabs? tabs = null)
    {
        ArgumentNullException.ThrowIfNull(blocks);
        ArgumentNullException.ThrowIfNull(contents);
        CheckContents(contents);

        Render(blocks, contents, tabNames, title, tabs);
    }

    /// <summary>Draws the heading, the tab bar, and every tab with its content.</summary>
    /// <param name="blocks">The application being built.</param>
    /// <param name="contents">One entry per tab.</param>
    /// <param name="tabNames">The label of each tab, or <see langword="null"/> for the defaults.</param>
    /// <param name="title">The heading, or <see langword="null"/> for none.</param>
    /// <param name="tabs">The tab bar to use, or <see langword="null"/> for a default one.</param>
    private static void Render(
        Blocks blocks,
        IReadOnlyList<TabContent> contents,
        IReadOnlyList<string>? tabNames,
        string? title,
        Tabs? tabs)
    {
        // A composition nested inside another render joins it rather than opening a second scope:
        // the outermost scope that closes is the one that finishes the render.
        using var scope = blocks.IsRendering ? null : blocks.BeginRender();

        // The application's settings are read before the tabs are drawn, because a tab's content
        // writes them: each interface or chat claims the application it is built into, which is
        // right when it is built alone and wrong inside a tab. The Python version never sees this — its tabs
        // are applications of their own and write to themselves — so the composition keeps what it
        // was told and drops what a tab said, for the settings it does not set itself.
        var analytics = blocks.AnalyticsEnabled;
        var deleteCache = blocks.DeleteCache;

        if (!string.IsNullOrEmpty(title))
        {
            blocks.Add(new Markdown
            {
                Value = string.Format(CultureInfo.InvariantCulture, Heading, title),
            });
        }

        var bar = blocks.Add(tabs ?? new Tabs());

        using (blocks.Enter(bar))
        {
            // The Python version zips the applications with the names, so the shorter of the two decides how
            // many tabs there are.
            var count = tabNames is null ? contents.Count : Math.Min(contents.Count, tabNames.Count);

            for (var index = 0; index < count; index++)
            {
                var content = contents[index];
                var tab = blocks.Add(new Tab
                {
                    Label = tabNames is null
                        ? string.Format(CultureInfo.InvariantCulture, DefaultTabName, index)
                        : tabNames[index],
                });

                using (blocks.Enter(tab))
                {
                    // The tab's scale is the Python version's `1 if interface.fill_height else None`, read off
                    // the application the tab holds rather than stated by the caller. The content
                    // here is a recipe drawn into the application being built, so what it says is
                    // whatever it wrote there — and the application's value is cleared first, so that
                    // a tab which does not mention the height does not inherit the answer the tab
                    // before it gave.
                    blocks.FillHeight = false;
                    content.Render(blocks);

                    tab.Scale = (content.FillHeight ?? blocks.FillHeight) ? 1 : null;
                }
            }
        }

        ClaimApplication(blocks, title);

        blocks.AnalyticsEnabled = analytics;
        blocks.DeleteCache = deleteCache;
    }

    /// <summary>Says what the composed application is, once its tabs have had their say.</summary>
    /// <param name="blocks">The application being built.</param>
    /// <param name="title">The heading the application was given, or <see langword="null"/>.</param>
    /// <remarks>
    /// <para>
    /// The Python version's tabbed interface fills the height and does not fill the width, and has
    /// no description of its own: a description belongs to an interface, and the interfaces here
    /// are the tabs rather than the application.
    /// </para>
    /// <para>
    /// Only the settings the Python version's <c>TabbedInterface</c> states for itself are stated
    /// here. The two it leaves alone — analytics and the temporary-file schedule — are restored by
    /// the caller to what the application was told before the tabs were drawn, rather than written
    /// here, so that the composition does not invent a value for something it has no opinion about.
    /// </para>
    /// </remarks>
    private static void ClaimApplication(Blocks blocks, string? title)
    {
        blocks.Mode = "tabbed_interface";
        blocks.Title = string.IsNullOrEmpty(title) ? "Gradio" : title;
        blocks.Description = string.Empty;
        blocks.FillHeight = true;
        blocks.FillWidth = false;
    }

    /// <summary>Refuses a list that holds something that is not a tab's content.</summary>
    /// <param name="contents">The list.</param>
    /// <exception cref="ArgumentException">An entry is <see langword="null"/>.</exception>
    private static void CheckContents(IReadOnlyList<TabContent> contents)
    {
        foreach (var content in contents)
        {
            if (content is null)
            {
                throw new ArgumentException("A tab's content cannot be null.", nameof(contents));
            }
        }
    }
}

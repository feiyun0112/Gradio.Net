using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Text;
namespace Gradio.Net;
/// <summary>
/// A render root: the tree a render builds into, and the owner of the block ids inside it.
/// </summary>
/// <remarks>
/// <para>
/// Two <see cref="Blocks"/> instances in one process keep separate id sequences and separate
/// children, so neither can see the other's blocks. That is the point of the type: the Python
/// version keeps the current root and the running id in a process-wide <c>Context</c>, which is
/// what made two demos in one process impossible.
/// </para>
/// <para>
/// Ids start at 0 for the root itself and at 1 for the first block added to it, matching the ids
/// the Python version assigns from its process-wide counter to the first app in a process.
/// </para>
/// <para>
/// Rendering one instance is single-threaded: a root is built by one flow. Rendering two instances
/// concurrently is fine, and is what the ambient context exists to support.
/// </para>
/// </remarks>
public sealed partial class Blocks : BlockContext, IDisposable
{
    private readonly IRenderContextAccessor _accessor;
    private readonly IBlockContainerFactory? _containers;
    private readonly List<Block> _blocks = [];
    private readonly ReadOnlyCollection<Block> _blocksView;
    private readonly List<Block> _reserved = [];
    private readonly List<RenderHandle> _openRenders = [];
    private readonly HashSet<int> _loadEventsAttached = [];
    private Theme _theme = new DefaultTheme();
    private int _nextId;
    private bool _rendered;
    private bool _sealed;
    private IReadOnlyList<string> _allowedPaths = [];
    private IReadOnlyList<string> _blockedPaths = [];

    /// <summary>Creates a render root that takes its render context from <paramref name="accessor"/>.</summary>
    /// <param name="accessor">The accessor the render scopes of this root are entered through.</param>
    /// <param name="containers">
    /// Where the containers that group form components come from. Optional so that a caller with
    /// nothing to group — the kernel's own tests, say — can leave it out; rendering a component
    /// that expects a container without one fails with a message saying so.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="accessor"/> is <see langword="null"/>.</exception>
    internal Blocks(IRenderContextAccessor accessor, IBlockContainerFactory? containers = null)
    {
        ArgumentNullException.ThrowIfNull(accessor);

        _accessor = accessor;
        _containers = containers;
        _blocksView = _blocks.AsReadOnly();
        _dependenciesView = _dependencies.AsReadOnly();

        // The Python version allocates the root's own id before its children's, so a root is id 0 and the
        // first block added to it is id 1.
        MarkRendered(_nextId++, parent: null);
    }

    /// <summary>The render context accessor this application was built with.</summary>
    /// <remarks>
    /// Internal because it is the kernel's wiring rather than the application's: a host that is
    /// handed an application that already exists has to register the accessor it was built with, or
    /// the two would carry different ambient contexts and a block written into one would be
    /// invisible to the other. The one caller outside the kernel is the <c>gr</c> layer's
    /// <c>Launch</c>, which starts a host around an application the author wrote.
    /// </remarks>
    internal IRenderContextAccessor ContextAccessor => _accessor;

    /// <summary>The blocks rendered into this root, in the order they were rendered.</summary>
    /// <remarks>
    /// This is the list the config is built from, not a hook for tests: it is what the Python
    /// version keeps on every <c>BlockContext</c> and walks to serve <c>/config</c>.
    /// </remarks>
    public IReadOnlyList<Block> Registered => _blocksView;

    /// <summary>The title the application page and the OpenAPI document carry.</summary>
    public string Title { get; set; } = "Gradio";

    /// <summary>
    /// The icon the browser shows for the application, or <see langword="null"/> for the one
    /// the Python version uses.
    /// </summary>
    /// <value>
    /// <see langword="null"/>, which is what an application that names none carries — and what
    /// the Python version's config carries in that case.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is the <c>favicon_path</c> launch parameter in the Python version 6.x, kept on the
    /// application for the reason <see cref="Css"/> is: <see cref="Blocks"/> is what an application
    /// here is configured through, and the launch entry point forwards to it. One value serves
    /// three addresses — <c>/favicon.ico</c>, <c>/pwa_icon</c> and the manifest that names the
    /// second — because a browser that found a different icon at each of them would show one in the
    /// tab and another on the home screen.
    /// </para>
    /// <para>
    /// The file is read when it is asked for rather than when it is named, so an icon replaced on
    /// disk is served as it is now; a path that does not name a file is refused rather than skipped,
    /// for the reason <see cref="CssPaths"/> gives about a stylesheet that is not there.
    /// </para>
    /// <para>
    /// What each of the three addresses makes of the file — a vector served as it is at any size, a
    /// raster scaled — is the icon's own rule rather than this property's: this property names a
    /// file, and the routes decide what to do with it.
    /// </para>
    /// </remarks>
    public string? FaviconPath { get; set; }

    /// <summary>
    /// What kind of application this is, as the config's <c>mode</c> says it.
    /// </summary>
    /// <value>
    /// <c>blocks</c>, which is what an application assembled out of blocks is. An
    /// <c>Interface</c> is <c>interface</c> and a <c>TabbedInterface</c> is <c>tabbed_interface</c>;
    /// the frontend reads the value to decide which shape it is drawing.
    /// </value>
    /// <remarks>
    /// The Python version keeps this on the application rather than deriving it from what was
    /// rendered, because the same blocks can be assembled into either: it is the class that was
    /// constructed that says which one this is.
    /// </remarks>
    public string Mode { get; set; } = "blocks";

    /// <summary>
    /// The components an interface was constructed from, or <see langword="null"/> for an
    /// application that is not one.
    /// </summary>
    /// <value>
    /// <see langword="null"/>, which is what a page of blocks carries: it knows which components its
    /// bindings name, not which columns it was given.
    /// </value>
    /// <remarks>
    /// <para>
    /// The Python version's <c>Blocks.input_components</c>, which an <c>Interface</c> sets on the
    /// object it is — the Python version's <c>Interface</c> is a <c>Blocks</c>, so the list lands
    /// there. Here the two are separate types and the interface hands the application the list as
    /// it writes it, which is the same statement in the shape this framework's two types have.
    /// </para>
    /// <para>
    /// It exists because one thing asks for it: the launch report describes an interface by the
    /// components it was given and a page of blocks by the ones its bindings name (see
    /// <c>ApplicationAnalytics</c>). Nothing renders from it, so an application that sets nothing
    /// loses a description rather than a page.
    /// </para>
    /// </remarks>
    internal IReadOnlyList<Component>? InputComponents { get; set; }

    /// <summary>The components an interface was constructed to write, or <see langword="null"/>.</summary>
    /// <value><see langword="null"/>, for the reason <see cref="InputComponents"/> gives.</value>
    internal IReadOnlyList<Component>? OutputComponents { get; set; }

    /// <summary>The description the OpenAPI document carries.</summary>
    /// <value>
    /// An empty string for an application that says nothing — which is what the Python version
    /// serves for one — and <see langword="null"/> for an application that says it has none, which
    /// a chat does when it was given no description.
    /// </value>
    public string? Description { get; set; } = string.Empty;

    /// <summary>The theme the application is dressed in.</summary>
    /// <value>
    /// <see cref="DefaultTheme"/>, which is what the Python version uses when an application names
    /// none.
    /// </value>
    /// <remarks>
    /// <para>
    /// A theme is an object rather than a name because it is editable: an application can start
    /// from a preset, set a few variables, and hand the result back. <see cref="ThemePresets.Get"/>
    /// turns a name into a preset for the case where the name comes from configuration.
    /// </para>
    /// <para>
    /// The config carries the theme's <see cref="Theme.Name"/> rather than the object, which is the
    /// Python version's shape: the frontend reads the name to know which preset it is looking at,
    /// and gets the variables themselves from <c>/theme.css</c>.
    /// </para>
    /// <para>
    /// Assigning <see langword="null"/> is refused rather than served: the Python version's
    /// <c>get_theme</c> turns "no theme" into the default one, so an application that names none
    /// still has a theme, and one that sets this to null would otherwise fail on the first request
    /// that renders it.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">The value is <see langword="null"/>.</exception>
    public Theme Theme
    {
        get => _theme;
        set => _theme = value ?? throw new ArgumentNullException(
            nameof(Theme),
            "An application always has a theme. Use ThemePresets.Default() for the one it gets when it names none.");
    }

    /// <summary>
    /// The stylesheet the page loads from <c>/theme.css</c>.
    /// </summary>
    /// <value>
    /// The theme rendered into CSS: the font rules, the light variables, the dark variables, and the
    /// theme's own custom CSS. Empty only if the theme itself renders nothing.
    /// </value>
    /// <remarks>
    /// Rendered on each request rather than cached, because the theme is mutable: an application may
    /// set a variable after handing the theme over, and a cached stylesheet would serve the old one.
    /// The work is a few string joins over 325 variables, which is cheaper than the bookkeeping a
    /// cache would need to stay correct.
    /// </remarks>
    public string ThemeCss => Theme.ToCss();

    /// <summary>The hash of <see cref="ThemeCss"/>, which the config carries as <c>theme_hash</c>.</summary>
    /// <value>The lower-case hex SHA-256 of the stylesheet's UTF-8 bytes.</value>
    /// <remarks>
    /// The Python version sends it so that a client can tell whether the theme changed between two
    /// loads; it is the same digest over the same bytes, so the two implementations agree on it.
    /// </remarks>
    public string ThemeHash =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(ThemeCss))).ToLowerInvariant();

    /// <summary>
    /// Custom CSS the application adds to the page.
    /// </summary>
    /// <value>
    /// <see langword="null"/>, which is what an application that adds none carries — and what
    /// the Python version's config carries in that case.
    /// </value>
    /// <remarks>
    /// This is the <c>css</c> launch parameter in the Python version 6.x, kept on the application
    /// because <see cref="Blocks"/> is what an application here is configured through; the launch
    /// entry point forwards to it (ticket 23). CSS that belongs to the theme rather than to the
    /// application goes in <see cref="Theme.CustomCss"/>, which <c>/theme.css</c> serves.
    /// </remarks>
    public string? Css { get; set; }

    /// <summary>
    /// CSS files whose text the page carries after <see cref="Css"/>.
    /// </summary>
    /// <value>
    /// The paths, in the order the application named them, or <see langword="null"/> for an
    /// application that names none.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is the <c>css_paths</c> launch parameter in the Python version 6.x, kept beside
    /// <see cref="Css"/> for the same reason: <see cref="Blocks"/> is what an application here is
    /// configured through, and the launch entry point — which does not exist yet — forwards to it
    /// (ticket 23).
    /// </para>
    /// <para>
    /// The Python version takes one path as readily as a list. A list is what this takes, for the
    /// reason the raw bindings take one: a parameter that is sometimes a value and sometimes a
    /// sequence is a shape C# would have to be told twice, and a one-element list is a list.
    /// </para>
    /// <para>
    /// Each file is read when the config is built, which is once per page rather than once per
    /// launch: a file edited between two loads is served as it is now, which is the same choice
    /// <see cref="ThemeCss"/> makes about a theme the application changed after starting. A path
    /// that does not name a file is not skipped — the read raises, and the request fails, because an
    /// application that named a stylesheet it does not have has a mistake rather than a default.
    /// </para>
    /// </remarks>
    public IReadOnlyList<string>? CssPaths { get; set; }

    /// <summary>
    /// The translations the application offers, or <see langword="null"/> for an application that
    /// offers none.
    /// </summary>
    /// <value>
    /// <see langword="null"/>, which is what an application that says nothing carries — and what
    /// the Python version's config carries in that case. An instance holding no locales is a
    /// different answer, and is served as an empty dictionary.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is the <c>i18n</c> launch parameter in the Python version 6.x, kept on the application
    /// for the reason <see cref="Css"/> is: <see cref="Blocks"/> is what an application here is
    /// configured through, and the launch entry point forwards to it.
    /// </para>
    /// <para>
    /// A component's text is written from the catalogue rather than from a separate type —
    /// <c>new Textbox { Label = i18n["name_label"] }</c> — so the props stay strings and the
    /// frontend translates what it finds. The config carries the catalogues themselves as
    /// <c>i18n_translations</c>, which is where the frontend reads them.
    /// </para>
    /// </remarks>
    public I18n? I18n { get; set; }

    /// <summary>
    /// Whether the application offers itself to a browser as an installable app.
    /// </summary>
    /// <value>
    /// <see langword="true"/> when the application is running on a Hugging Face Space, and
    /// <see langword="false"/> everywhere else — which is the Python version's default.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is the <c>pwa</c> launch parameter in the Python version 6.x, kept on the application
    /// for the reason <see cref="Css"/> is: <see cref="Blocks"/> is what an application here is
    /// configured through, and the launch entry point forwards to it.
    /// </para>
    /// <para>
    /// On a Space an application is something a visitor keeps rather than visits, which is why the
    /// default follows the platform rather than being off everywhere.
    /// </para>
    /// <para>
    /// It is the config's <c>pwa</c> field, which the frontend reads, so it is a property of the
    /// application rather than a container option: the page and the manifest have to agree, and one
    /// value is how they do. It does not gate the manifest or the icon — those answer either way,
    /// because a browser asks for them on every load and a 404 there is a console error on every
    /// load.
    /// </para>
    /// </remarks>
    public bool Pwa { get; set; } = PwaFromEnvironment();

    /// <summary>
    /// Whether the application offers itself to agents over the Model Context Protocol.
    /// </summary>
    /// <value>
    /// <see langword="false"/>, unless the environment turns it on for every application at once
    /// with <c>GRADIO_MCP_SERVER</c>, which is what the Python version reads when a launch says
    /// nothing.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is the <c>mcp_server</c> launch parameter in the Python version 6.x, kept on the
    /// application for the reason <see cref="Css"/> is: <see cref="Blocks"/> is what an application
    /// here is configured through, and the launch entry point forwards to it.
    /// </para>
    /// <para>
    /// It is also the config's <c>mcp_server</c> field, which the frontend reads to show the
    /// endpoint's address — so turning it on changes two things that must not disagree: the
    /// endpoints that exist and what the page says about them.
    /// </para>
    /// </remarks>
    public bool McpServer { get; set; } = McpServerFromEnvironment();

    /// <summary>
    /// Whether the application reports usage.
    /// </summary>
    /// <value>
    /// <see langword="false"/>, deliberately, where the Python version defaults to
    /// <see langword="true"/>: nothing leaves the operator's network unless they opt in. The
    /// difference is declared in the config comparison rather than hidden.
    /// </value>
    /// <remarks>
    /// <para>
    /// Turning it on does not make the framework send anything: what it turns on is the composing of
    /// the five events and their being handed to <see cref="IAnalyticsSink"/>, whose default does
    /// nothing with them. An application that wants them somewhere registers a sink.
    /// </para>
    /// <para>
    /// The environment can turn it on for every application at once with
    /// <c>GRADIO_ANALYTICS_ENABLED</c>, which is the Python version's variable. Its default differs
    /// with the property's — unset is off here and on there — and it is read as a word rather than
    /// as the Python version's exact <c>"True"</c>, because a variable whose default already
    /// differs gains nothing from also refusing <c>true</c>.
    /// </para>
    /// </remarks>
    public bool AnalyticsEnabled { get; set; } = AnalyticsFromEnvironment();

    /// <summary>
    /// Whether a run that crashed sends its message to the client, or only to the operator's log.
    /// </summary>
    /// <value>
    /// <see langword="false"/>, which is what the Python version serves: its <c>Blocks</c> starts
    /// with the switch on, but <c>launch()</c> defaults it to off, and every application served
    /// here was launched. The config carries the value either way.
    /// </value>
    /// <remarks>
    /// <para>
    /// It governs crashes only. A handler that raises <see cref="Error"/> is shown to the user
    /// whatever this says, because that is the application saying "no" rather than falling over —
    /// the Python version's <c>show_error or isinstance(error, AppError)</c>, and the reason a
    /// validator does not need the switch turned on to be read.
    /// </para>
    /// <para>
    /// This is the Python version's <c>show_error</c> launch parameter in 6.x, kept on the
    /// application for the reason <see cref="Css"/> is: <see cref="Blocks"/> is what an application
    /// here is configured through. What it changes is the failure payload of a run — the queue's
    /// completion message and the direct route's answer — and nothing else.
    /// </para>
    /// </remarks>
    public bool ShowError { get; set; }

    /// <summary>
    /// Whether this application records its runs, and offers the page that lists them.
    /// </summary>
    /// <value>
    /// <see langword="true"/> unless the application says otherwise — the Python version's default,
    /// and what its config says. The environment can turn it off for every application at once with
    /// <c>GRADIO_RUN_HISTORY</c>, which is what a deployment that must not write anything wants.
    /// </value>
    /// <remarks>
    /// A run is recorded only when it is a public endpoint's final answer, which is the Python
    /// version's rule: a private binding is one the application did not mean to expose, and half a
    /// stream is not an answer. Turning this off also drops the link to the page from the footer,
    /// because a link to a page that refuses is worse than no link.
    /// </remarks>
    public bool RunHistory { get; set; } = RunHistoryFromEnvironment();

    /// <summary>
    /// Whether this application offers the Vibe editor: the panel that rewrites the application's own
    /// source from a sentence, and the routes it asks for the code.
    /// </summary>
    /// <value>
    /// <see langword="false"/> unless <c>GRADIO_VIBE_MODE</c> is set to something — which is the
    /// Python version's test, and it reads a variable as a truth value rather than as a word: any
    /// value at all, including <c>"false"</c>, turns the editor on.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is the config's <c>vibe_mode</c> field, which the frontend reads to decide whether to draw
    /// the panel, so it is a property of the application rather than a container option: the page and
    /// the routes have to agree, and one value is how they do.
    /// </para>
    /// <para>
    /// The rest of what the editor needs — the file it rewrites and the model it asks — is
    /// <see cref="VibeOptions"/>, because none of it appears in the config. The editor is a
    /// development tool: it rewrites a file on the server, so turning it on for a deployment is a
    /// decision to make deliberately.
    /// </para>
    /// </remarks>
    public bool VibeMode { get; set; } = VibeModeFromEnvironment();

    /// <summary>
    /// Whether this application offers the monitoring routes, and with them the panel that reports
    /// its traffic and its errors.
    /// </summary>
    /// <value>
    /// <see langword="true"/>, which is the Python version's default: a launch that says nothing
    /// about monitoring enables it, and only an explicit "off" turns it off.
    /// </value>
    /// <remarks>
    /// <para>
    /// The routes answer <c>403</c> rather than disappearing when this is off, which is the Python
    /// version's answer for the same application — and the panel's address is not published
    /// anywhere, so an application that turns it off is not advertising a page it refuses.
    /// </para>
    /// <para>
    /// This is not the same switch as <see cref="AnalyticsEnabled"/>: that one is about reporting
    /// usage to somebody else, and this one is about reporting it to the operator.
    /// </para>
    /// </remarks>
    public bool EnableMonitoring { get; set; } = true;

    /// <summary>
    /// The secret the monitoring panel is opened with.
    /// </summary>
    /// <value>
    /// A token drawn per application, as the Python version draws it — so that two applications in
    /// one process have two addresses, and so that the address is not something a visitor can
    /// guess.
    /// </value>
    /// <remarks>
    /// The panel is the one route the Python version does not put behind its login check; the key
    /// in the URL is what stands in for it. That is why it is drawn from a cryptographic source and
    /// compared in constant time rather than being a name.
    /// </remarks>
    public string AnalyticsKey { get; } = MonitoringKey.New();

    /// <summary>Whether the page is sized to fill the viewport.</summary>
    public bool FillHeight { get; set; }

    /// <summary>Whether the page is sized to fill the viewport's width.</summary>
    public bool FillWidth { get; set; }

    /// <summary>
    /// The directory this application keeps its files in: uploads, handler outputs, and the files a
    /// component's value names.
    /// </summary>
    /// <value>
    /// <see cref="MediaCache.DefaultFolder"/> — <c>GRADIO_TEMP_DIR</c>, or the system's temporary
    /// directory joined with <c>gradio</c> — unless the application says otherwise.
    /// </value>
    /// <remarks>
    /// One directory per application, because that is the directory the file endpoint will serve
    /// from: a file written outside it is a file the application does not expose. The Python
    /// version has one per process, taken from the environment; making it a property is what lets
    /// two applications in one process keep their files apart.
    /// </remarks>
    public string UploadFolder { get; set; } = MediaCache.DefaultFolder;

    /// <summary>
    /// The largest file the application accepts, in bytes, or <see langword="null"/> for no limit.
    /// </summary>
    /// <value>
    /// <see langword="null"/>, which is the Python version's default: an application that says
    /// nothing accepts a file of any size, and the operator's reverse proxy is what limits it.
    /// </value>
    /// <remarks>
    /// Enforced on both routes that take a file: an upload is measured while it is being read, so a
    /// file over the limit costs no more disk than the limit itself, and a component's own server
    /// function is measured before the file is copied into memory. The second route's body is parsed
    /// by the framework before the application sees it, so what it can promise is the refusal
    /// rather than the saved copy.
    /// </remarks>
    public long? MaxFileSize { get; set; }

    /// <summary>
    /// The directories the application serves besides the two it owns, or empty for none.
    /// </summary>
    /// <value>
    /// Empty, which is the Python version's default: an application exposes what it wrote and
    /// nothing else.
    /// </value>
    /// <remarks>
    /// <para>
    /// The Python version's <c>allowed_paths</c>, and its warning applies: a directory here makes
    /// every file in it and below it readable by anyone who can reach the application.
    /// </para>
    /// <para>
    /// The Python version also reads <c>GRADIO_ALLOWED_PATHS</c> when the launch argument is empty.
    /// That is deliberately not done here: the variable is process-wide and this setting is the
    /// application's — two applications in one process would share an allow-list, which is the same
    /// reason <see cref="UploadFolder"/> is a property rather than the environment's folder.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">The value is <see langword="null"/>.</exception>
    public IReadOnlyList<string> AllowedPaths
    {
        get => _allowedPaths;
        set
        {
            ArgumentNullException.ThrowIfNull(value);

            _allowedPaths = Normalized(value);
        }
    }

    /// <summary>
    /// The directories the application refuses to serve, or empty for none.
    /// </summary>
    /// <value>Empty, which is the Python version's default.</value>
    /// <remarks>
    /// The Python version's <c>blocked_paths</c>, and it takes precedence over everything the
    /// application would otherwise serve — including its own folders. A block that could be undone
    /// by an allow would not be a block.
    /// </remarks>
    /// <exception cref="ArgumentNullException">The value is <see langword="null"/>.</exception>
    public IReadOnlyList<string> BlockedPaths
    {
        get => _blockedPaths;
        set
        {
            ArgumentNullException.ThrowIfNull(value);

            _blockedPaths = Normalized(value);
        }
    }

    /// <summary>The entries of a path list, made absolute and stripped of blanks.</summary>
    /// <param name="paths">What the application gave.</param>
    /// <returns>The paths, in the order they were given.</returns>
    /// <remarks>
    /// A blank entry is dropped rather than kept: the Python version splits a comma-separated
    /// variable and trims it, and an empty entry would otherwise match every path.
    /// </remarks>
    private static IReadOnlyList<string> Normalized(IEnumerable<string> paths) =>
        [.. paths.Where(path => !string.IsNullOrWhiteSpace(path)).Select(Path.GetFullPath)];

    /// <summary>The identifier the frontend reports the application under.</summary> <remarks>Drawn
    /// per process, as the Python version draws it, so it identifies a run rather than a
    /// build.</remarks>
    public long AppId { get; } = Random.Shared.NextInt64();

    /// <summary>Whether the environment turns reporting on for every application.</summary>
    /// <returns><see langword="true"/> only when the variable says so.</returns>
    /// <remarks>
    /// The Python version reads <c>GRADIO_ANALYTICS_ENABLED</c> with <c>True</c> as its default and
    /// compares it with <c>== "True"</c>. The default here is the other one, and the comparison is
    /// case-insensitive: a variable set to <c>True</c> means what it means in the Python version, and
    /// one set to <c>true</c> means the same thing rather than being silently ignored.
    /// </remarks>
    private static bool AnalyticsFromEnvironment() =>
        string.Equals(
            Environment.GetEnvironmentVariable("GRADIO_ANALYTICS_ENABLED"),
            "true",
            StringComparison.OrdinalIgnoreCase);

    /// <summary>Whether the environment turns run history off for every application.</summary>
    /// <returns><see langword="false"/> only when the variable says so.</returns>
    /// <remarks>
    /// The Python version reads <c>GRADIO_RUN_HISTORY</c> the same way. Only "false" turns it off,
    /// so that a variable set to anything else — "1", "yes", a typo — leaves the default in place
    /// rather than silently changing behaviour.
    /// </remarks>
    private static bool RunHistoryFromEnvironment()
    {
        var configured = Environment.GetEnvironmentVariable("GRADIO_RUN_HISTORY");

        return !string.Equals(configured, "false", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>Whether the environment says this application is running on a Space.</summary>
    /// <returns><see langword="true"/> when the platform and a Space name are both there.</returns>
    /// <remarks>
    /// The Python version's <c>utils.get_space()</c>: the platform announces itself with
    /// <c>SYSTEM</c> and names the Space with <c>SPACE_ID</c>, and the test is that the name is not
    /// <see langword="null"/> — which a name set to the empty string is not. So the two halves are
    /// read the way the Python version reads them: <c>SYSTEM</c> exactly, and <c>SPACE_ID</c> as
    /// present rather than as non-empty.
    /// </remarks>
    private static bool PwaFromEnvironment() =>
        string.Equals(Environment.GetEnvironmentVariable("SYSTEM"), "spaces", StringComparison.Ordinal)
        && Environment.GetEnvironmentVariable("SPACE_ID") is not null;

    /// <summary>Whether the environment turns the MCP endpoint on for every application.</summary>
    /// <returns><see langword="true"/> only when the variable says so.</returns>
    /// <remarks>
    /// The Python version reads <c>GRADIO_MCP_SERVER</c> as an exact "true", which is the opposite
    /// default to <see cref="RunHistoryFromEnvironment"/> and deliberately so: an application that
    /// says nothing offers nothing to agents, because an endpoint a machine can call is a decision
    /// the operator makes rather than one they inherit.
    /// </remarks>
    private static bool McpServerFromEnvironment() =>
        string.Equals(
            Environment.GetEnvironmentVariable("GRADIO_MCP_SERVER"),
            "true",
            StringComparison.OrdinalIgnoreCase);

    /// <summary>Whether the environment turns the Vibe editor on for every application.</summary>
    /// <returns><see langword="true"/> when the variable is set to anything but the empty string.</returns>
    /// <remarks>
    /// The Python version's test is <c>bool(os.getenv("GRADIO_VIBE_MODE", ""))</c>, which is a
    /// truth-value test on a string and therefore true for every value a variable can hold —
    /// <c>"false"</c> and <c>"0"</c> included. Reading it as a word here would make an application
    /// that is in Vibe mode the Python version not in Vibe mode here, so the test is the same test.
    /// </remarks>
    private static bool VibeModeFromEnvironment() =>
        Environment.GetEnvironmentVariable("GRADIO_VIBE_MODE") is { Length: > 0 };

    /// <summary>
    /// Makes this root the ambient render context for the current flow, so that <see cref="Add{TBlock}"/>
    /// can attach blocks to it.
    /// </summary>
    /// <returns>
    /// A scope that ends the render and releases the context when it is disposed. Disposing it
    /// twice is safe.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Scopes nest, so a render can be embedded in another one; the outer context is restored when
    /// the inner scope ends. The render ends when the outermost scope does — that is when the
    /// grouping of form components into their container runs, because it needs the whole run of
    /// children to decide what belongs together.
    /// </para>
    /// <para>
    /// The handle is kept rather than only handed out, because a render that is never closed by its
    /// caller is closed by <see cref="Dispose"/> — and a render closed that way has to end the same
    /// way, with its grouping and its load events, or an application whose <c>using</c> block
    /// swallowed an exception would serve a page that was never grouped.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">This application is already being served.</exception>
    /// <remarks>
    /// Internal because a page is written by calling the compatibility layer's factories, not by
    /// opening a render by hand: <c>gr.Blocks()</c> opens this scope and the <c>using</c> that closes
    /// it ends the render, so a ported file never writes this line. It stays visible
    /// inside this assembly because the layer and the page builders are the callers that build a
    /// tree without going through a factory.
    /// </remarks>
    internal IDisposable BeginRender()
    {
        ThrowIfSealed();

        var scope = _accessor.Enter(new RenderContext(this));
        var render = new RenderHandle(this, scope);
        _openRenders.Add(render);

        return render;
    }

    /// <summary>
    /// Adds a block to this root and gives it its id.
    /// </summary>
    /// <typeparam name="TBlock">The block's type, so that callers can keep the concrete type.</typeparam>
    /// <param name="block">The block to add.</param>
    /// <returns><paramref name="block"/>, with its <see cref="Block.Id"/> and <see cref="Block.Parent"/> set.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="block"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">
    /// This root is not the ambient render context — the caller is outside a <see cref="BeginRender"/>
    /// scope of this instance, or is inside the scope of a different one — or the block already
    /// belongs to a root.
    /// </exception>
    /// <exception cref="InvalidOperationException">This application is already being served.</exception>
    /// <remarks>
    /// <para>
    /// Internal for the same reason <see cref="BeginRender"/> is: <c>gr.Textbox(...)</c> is a line on
    /// the page rather than a value, so the factory is what places the block and a ported file never
    /// names this method. <c>demo.Add(gr.Textbox(...))</c> is the shape that sentence
    /// exists to keep out of a port.
    /// </para>
    /// <para>
    /// A block belongs to exactly one root, and to one place in it. Ids are allocated per root, so
    /// letting a block move between roots would let two roots hand out the same id; the Python
    /// version can afford to be looser because its counter is process-wide, which is the state this
    /// design removed.
    /// </para>
    /// <para>
    /// A block whose <see cref="Block.Render"/> is <see langword="false"/> is given its id and
    /// nothing else: it is not attached, and the config does not describe it until
    /// <see cref="Attach{TBlock}"/> is called. That is the Python version's <c>render=False</c>,
    /// and it is what an <c>Interface</c> needs — the components it was handed are constructed
    /// before the layout that holds them, so they are numbered before it and placed afterwards.
    /// </para>
    /// </remarks>
    internal TBlock Add<TBlock>(TBlock block)
        where TBlock : Block
    {
        // The seal is checked by Reserve, which is the half of this that every write goes through.
        var context = AmbientContext();

        Reserve(block);

        if (block.Render)
        {
            Place(block, context);
        }

        return block;
    }

    /// <summary>
    /// Gives a block its id without giving it a place in the tree.
    /// </summary>
    /// <typeparam name="TBlock">The block's type, so that callers can keep the concrete type.</typeparam>
    /// <param name="block">The block to number.</param>
    /// <returns><paramref name="block"/>, with its <see cref="Block.Id"/> set and no parent.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="block"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">
    /// This root is not the ambient render context, or the block already belongs to a root.
    /// </exception>
    /// <exception cref="InvalidOperationException">This application is already being served.</exception>
    /// <remarks>
    /// <para>
    /// The half of <see cref="Add{TBlock}"/> that decides a block's identity, for the caller that
    /// knows the identity before it knows the place. The Python version splits the two the same
    /// way: a block is numbered when it is constructed and enters the tree when it is rendered,
    /// which is why a Python interface's components are numbered before the layout that holds them.
    /// </para>
    /// <para>
    /// A block numbered here is not described by the config until <see cref="Attach{TBlock}"/> places
    /// it, and a block that is never placed is never described.
    /// </para>
    /// <para>
    /// Internal with <see cref="Add{TBlock}"/>, whose first half it is: a page is written through the
    /// factories, so the two moments of a block's life are the kernel's business rather than a
    /// caller's.
    /// </para>
    /// </remarks>
    internal TBlock Reserve<TBlock>(TBlock block)
        where TBlock : Block
    {
        ArgumentNullException.ThrowIfNull(block);
        ThrowIfSealed();

        _ = AmbientContext();

        if (block.Id != UnrenderedId)
        {
            throw new InvalidOperationException(
                $"The block already belongs to a render root as id {block.Id}; a block can only be rendered once.");
        }

        // The root is the parent until the block is placed, which is what lets it bind its own
        // events before it has a place in the tree: a binding has to name its application, and a
        // block that belongs to one and is drawn nowhere still belongs to it.
        block.MarkRendered(_nextId++, this);
        _reserved.Add(block);

        return block;
    }

    /// <summary>
    /// Places a block that was added with <see cref="Block.Render"/> turned off, and describes it in
    /// the config.
    /// </summary>
    /// <typeparam name="TBlock">The block's type, so that callers can keep the concrete type.</typeparam>
    /// <param name="block">The block to render.</param>
    /// <returns><paramref name="block"/>, now with its place in the tree.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="block"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">
    /// This root is not the ambient render context, or <paramref name="block"/> is not a block this
    /// root reserved — it belongs to another application, was never added, or is already rendered.
    /// </exception>
    /// <exception cref="InvalidOperationException">This application is already being served.</exception>
    /// <remarks>
    /// <para>
    /// The Python version's <c>Block.render()</c>. The id was given when the block was added, so
    /// the block keeps the number it was constructed under while it enters the tree where the code
    /// that places it says — which is the whole point of separating the two moments.
    /// </para>
    /// <para>
    /// Only a block this root reserved can be rendered here, and only once: a block rendered twice
    /// would be described twice.
    /// </para>
    /// <para>
    /// Internal with <see cref="Add{TBlock}"/>, whose second half it is.
    /// </para>
    /// </remarks>
    internal TBlock Attach<TBlock>(TBlock block)
        where TBlock : Block
    {
        ArgumentNullException.ThrowIfNull(block);
        ThrowIfSealed();

        var context = AmbientContext();

        if (!_reserved.Remove(block))
        {
            throw new InvalidOperationException(
                $"'{block.GetType().Name}' is not waiting to be rendered by this application; it was "
                + "never added here, belongs to another render root, or has already been rendered.");
        }

        Place(block, context);

        return block;
    }

    /// <summary>
    /// Whether this root is the ambient render context for the current flow.
    /// </summary>
    /// <remarks>
    /// What a helper that may be used either inside a render or on its own asks before opening a
    /// scope of its own: nesting one would end the render early, because the outermost scope that
    /// closes is the one that finishes it.
    /// </remarks>
    public bool IsRendering => _accessor.Current is { } context && ReferenceEquals(context.Root, this);

    /// <summary>Whether this application has been written, which is when its first render finished.</summary>
    /// <value>
    /// <see langword="false"/> for a root nothing was rendered into, and for one whose render is
    /// still open.
    /// </value>
    /// <remarks>
    /// This is what a readiness probe asks: a host that has been started but whose application was
    /// never written is a host that answers requests with an empty page, and the window between
    /// starting and writing is exactly the window an orchestrator should not send traffic into.
    /// </remarks>
    public bool IsRendered => _rendered;

    /// <summary>Gives a block its place in the tree and describes it.</summary>
    /// <param name="block">The block, already numbered.</param>
    /// <param name="context">The ambient render context, which says where it goes.</param>
    /// <remarks>
    /// The two halves of what <see cref="Add{TBlock}"/> does after numbering: this is the part a
    /// block that was reserved goes through when it is finally rendered, and the reason both go
    /// through it is that a block must not be able to arrive in the tree half-registered.
    /// </remarks>
    private void Place(Block block, RenderContext context)
    {
        // The innermost open layout, or the root: a block is added where the code that writes it
        // is, which is what makes a layout's scope mean anything.
        var parent = context.Container;

        block.MarkParent(parent);
        block.PagePath = CurrentPagePath;
        _blocks.Add(block);
        parent.AppendChild(block);

        // A file the block carries as part of itself — a component's default value — is kept out of
        // reclamation. Recorded here rather than by the component because this is where the block
        // and the application meet: before this it had no application to record anything with.
        foreach (var file in block.KeptFiles)
        {
            KeepTempFile(file);
        }

        // A block that binds its own events does it now, because this is the moment it has the id
        // those bindings name — see Block.OnAddedTo for why that is not the constructor here.
        block.OnAddedTo(this);
    }

    /// <summary>
    /// Makes a layout the place blocks are added to, for the length of the scope.
    /// </summary>
    /// <param name="container">The block whose children the next blocks become.</param>
    /// <returns>
    /// A scope that ends the container: its own grouping runs then, and adding goes back to the
    /// block around it.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="container"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">
    /// The block does not hold children, this root is not the ambient render context, the block does
    /// not belong to this root, or it is not a child of the container currently open.
    /// </exception>
    /// <exception cref="InvalidOperationException">This application is already being served.</exception>
    /// <remarks>
    /// <para>
    /// The Python version's <c>with gr.Row():</c>: what is written inside the block belongs to the
    /// row. The scope is what makes the tree, and it is also what makes the ids: the Python version
    /// groups a container's children when the container closes, so a container the grouping
    /// synthesises is numbered after the children it holds and before the siblings that follow — a
    /// container that was only grouped once the whole render had finished would be numbered after
    /// those siblings instead, and the config would describe a different application.
    /// </para>
    /// <para>
    /// A layout entered without being added to the application is refused rather than silently
    /// detached: its children would have nowhere to be in the tree.
    /// </para>
    /// <para>
    /// Internal because the scope is opened by disposing the layout the factory returned —
    /// <c>using (gr.Row())</c> — rather than by asking the application to enter it.
    /// </para>
    /// </remarks>
    internal IDisposable Enter(Block container)
    {
        ArgumentNullException.ThrowIfNull(container);
        ThrowIfSealed();

        var context = AmbientContext();

        if (!container.HoldsChildren)
        {
            throw new InvalidOperationException(
                $"'{container.GetType().Name}' does not hold other blocks, so there is no scope to "
                + "enter: only a layout, or a component whose template has a place for children, can "
                + "hold what is written inside it.");
        }

        if (container.Id == UnrenderedId || !ReferenceEquals(container.Root, this))
        {
            throw new InvalidOperationException(
                $"'{container.GetType().Name}' is not part of this application; add it before "
                + "entering it, so that the blocks written inside it have a place in the tree.");
        }

        if (!ReferenceEquals(container.Parent, context.Container))
        {
            throw new InvalidOperationException(
                $"'{container.GetType().Name}' is not inside the container currently open, so "
                + "entering it would move blocks into a branch they are not written under.");
        }

        var previous = context.Container;
        context.Container = container;

        var handle = new ContainerHandle(this, context, container, previous);

        // The container remembers its scope, so that `using (row)` and `using (demo.Enter(row))` are
        // the same scope closed two ways rather than two scopes.
        if (container is BlockContext entered)
        {
            entered.OpenedBy(handle);
        }

        return handle;
    }


    /// <summary>
    /// Ends every render scope this root still has open, and finishes each of them.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <strong>This is not "destroy the application".</strong> It releases the render scopes that
    /// were still open, and nothing else: the application that comes out of a <c>using</c> block is
    /// the one that went in, ready to be written further, rendered and served. That is what lets
    /// the Python version's <c>with gr.Blocks() as demo:</c> land on <c>using</c> unchanged, which
    /// is the one place this framework's meaning of <see cref="IDisposable"/> is deliberately not
    /// the .NET intuition. A container disposing a service it owns therefore does no harm, which is
    /// the second reason not to give this method a destructive meaning.
    /// </para>
    /// <para>
    /// Closing a render is finishing it — the grouping of what it wrote, and the load events its
    /// components asked for — because that is what leaving the Python version's <c>with</c> block
    /// does. A render that was left open, because the code that entered it threw, is closed here
    /// rather than left half-finished; the ambient context it established is released with it.
    /// </para>
    /// <para>
    /// Finishing can refuse: an application that wrote two blocks a page may only hold once is told
    /// so here, at the closing brace, rather than on the first request that builds the config —
    /// which is where the Python version's <c>__exit__</c> raises the same complaint. That is also
    /// why this method is not the place to swallow anything: an application that is wrong is wrong
    /// whether the flow around it threw.
    /// </para>
    /// <para>
    /// "No longer accepts new blocks" is a state of its own, <see cref="Seal"/>, which the host sets
    /// when it starts serving. Disposing a root does not set it.
    /// </para>
    /// <para>
    /// A layout that was entered and never left is not closed here: the render it was entered in is,
    /// and releasing that is what makes the ambient context unreadable, so a layout whose scope was
    /// left open stops receiving what is written afterwards. Disposing the layout itself
    /// (<see cref="BlockContext.Dispose"/>) is the way to close it while the render continues.
    /// </para>
    /// </remarks>
    public override void Dispose()
    {
        for (var index = _openRenders.Count - 1; index >= 0; index--)
        {
            _openRenders[index].Dispose();
        }

        _openRenders.Clear();

        // The root's own container scope, which only exists if somebody entered the application as a
        // container — it cannot be done through `Enter`, so in practice this closes nothing. The
        // call is what the rule asks for, and it costs one null check.
        base.Dispose();
    }

    /// <summary>
    /// Makes this application stop accepting blocks and bindings, which is what a host does before
    /// it serves it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The page a browser sees is the page that was written before the host started listening: a
    /// block or a binding added afterwards would be in the config of one request and not of another,
    /// and the frontend would be holding a document that no longer describes the server. Sealing is
    /// idempotent, and the host applies it rather than the application, because only the host knows
    /// when serving begins. It is public because the host that calls it is in another layer and
    /// because a test can then say what it means; an application author has no reason to call it.
    /// </para>
    /// <para>
    /// After this, every way of writing into the application — <see cref="BeginRender"/>,
    /// <see cref="Add{TBlock}"/>, <see cref="Reserve{TBlock}"/>, <see cref="Attach{TBlock}"/>,
    /// <see cref="Enter"/>, <see cref="Route"/> and the binding entry point behind every
    /// <c>On…</c> — is refused with an <see cref="InvalidOperationException"/> that says so, rather
    /// than with the <see cref="ObjectDisposedException"/> a caller would expect from a disposed
    /// object: the application is not disposed, it is being served. Properties that shape the
    /// application rather than its blocks (a title, a theme) are not gated: they are read when the
    /// config is built, so a host that sets one late changes every subsequent answer rather than
    /// leaving two documents that disagree.
    /// </para>
    /// </remarks>
    public void Seal() => _sealed = true;

    /// <summary>Whether this application has stopped accepting blocks.</summary>
    /// <remarks>Set by <see cref="Seal"/>, which the host calls when it starts serving.</remarks>
    public bool IsSealed => _sealed;

    /// <summary>Refuses a write into an application that is already being served.</summary>
    /// <exception cref="InvalidOperationException">This application has been sealed.</exception>
    private void ThrowIfSealed()
    {
        if (!_sealed)
        {
            return;
        }

        throw new InvalidOperationException(
            "This application is already being served, so it no longer accepts blocks: the page a "
            + "browser sees is the one written before the host started. Write the UI before starting "
            + "the host — the host seals the application when the first request arrives.");
    }

    /// <summary>
    /// Ends a render: validates what the application may only have one of, and groups what the
    /// render wrote into the containers its components expect.
    /// </summary>
    /// <remarks>
    /// This runs at the end of every render, not only the first. An application can be written in
    /// more than one go — that is what leaving a <c>using</c> block and writing more means — and a
    /// second go is grouped the way the first one was, or a form component written after the block
    /// would sit in the application where the first go's form component sits in a form. Grouping
    /// twice is grouping once: a container the first pass made has no expected parent of its own,
    /// so the second pass finds nothing left to do.
    /// </remarks>
    private void FinishRender()
    {
        _rendered = true;

        CheckSinglePerPageBlocks();

        FillExpectedParents(CreateContainer);

        AttachLoadEvents();
    }

    /// <summary>
    /// Binds the load event of every component that asked for one.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Python version's <c>attach_load_events</c>, which its root runs when the <c>with</c>
    /// block closes. A render ending is the same moment, and it is not the moment the component is
    /// added: a value may be computed from components written after it, so the binding is made here
    /// rather than there.
    /// </para>
    /// <para>
    /// An application can be written in more than one go, so a render ends more than once and this
    /// runs each time. A component already bound is skipped — two bindings for one load event
    /// would run the function twice per page — and one whose inputs were written later is left for
    /// the render that writes them, because a trigger naming a block that has no id yet would
    /// never fire.
    /// </para>
    /// <para>
    /// A caller asks for it directly when the dependency has to sit at a particular place: a
    /// workflow wants its canvas's load to be the first dependency of the application, which is
    /// where the Python version puts it by closing its <c>with</c> block before the endpoints are
    /// registered. Asking twice is asking once, for the reason above.
    /// </para>
    /// </remarks>
    internal void AttachLoadEvents()
    {
        foreach (var component in _blocks.OfType<Component>())
        {
            if (component.LoadFn is null || _loadEventsAttached.Contains(component.Id))
            {
                continue;
            }

            if (component.AttachLoadEvent(Loaded))
            {
                _loadEventsAttached.Add(component.Id);
            }
        }
    }

    /// <summary>
    /// Makes a container of the kind a component expects, and gives it its id.
    /// </summary>
    /// <param name="kind">The container kind, as the component names it.</param>
    /// <returns>The container, with its id and its place in the block list.</returns>
    /// <exception cref="NotSupportedException">This root was built without a container factory.</exception>
    /// <remarks>
    /// The id is allocated where the Python version allocates it — at the moment the grouping
    /// creates the container, which is after the blocks it holds and before whatever comes next.
    /// </remarks>
    private BlockContext CreateContainer(string kind)
    {
        var factory = _containers ?? throw new NotSupportedException(
            $"A block needs a '{kind}' container to be grouped into, but this render root was "
            + "built without a container factory. Register one (AddGradio does) or render the "
            + "block inside a container of its own.");

        var container = factory.Create(kind);
        container.MarkRendered(_nextId++, this);
        _blocks.Add(container);

        return container;
    }

    /// <summary>The ambient render context, when it is this root's.</summary>
    /// <exception cref="InvalidOperationException">This root is not the ambient render context.</exception>
    private RenderContext AmbientContext()
    {
        var context = _accessor.Current;
        if (context is null || !ReferenceEquals(context.Root, this))
        {
            throw new InvalidOperationException(
                $"A block can only be added while this {nameof(Blocks)} is the ambient render context; " +
                $"call {nameof(BeginRender)} on it first.");
        }

        return context;
    }


    /// <summary>
    /// The scope <see cref="BeginRender"/> hands out: it ends a render when it ends.
    /// </summary>
    /// <remarks>
    /// Ending is where the render finishes — the grouping and the load events — and it is also what
    /// takes this handle off the root's list, so that closing a render twice closes it once.
    /// </remarks>
    private sealed class RenderHandle : IDisposable
    {
        private readonly Blocks _owner;
        private readonly IDisposable _scope;
        private bool _disposed;

        internal RenderHandle(Blocks owner, IDisposable scope)
        {
            _owner = owner;
            _scope = scope;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _scope.Dispose();
            _owner._openRenders.Remove(this);
            _owner.FinishRender();
        }
    }

    /// <summary>
    /// The scope <see cref="Enter"/> hands out: it closes a container when it ends.
    /// </summary>
    /// <remarks>
    /// Closing is where a container's own grouping runs — the Python version does it when the
    /// context exits — and it is what restores the container that was open before, so a nested tree
    /// is built by nesting scopes rather than by remembering to go back.
    /// </remarks>
    private sealed class ContainerHandle : IDisposable
    {
        private readonly Blocks _owner;
        private readonly RenderContext _context;
        private readonly Block _container;
        private readonly Block _previous;
        private bool _disposed;

        internal ContainerHandle(Blocks owner, RenderContext context, Block container, Block previous)
        {
            _owner = owner;
            _context = context;
            _container = container;
            _previous = previous;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            // A scope closes only while its render is open and it is the container that render has
            // open: a layout kept in a field and disposed after the `using` has ended belongs to a
            // render that is over, and grouping it now would add nodes to an application that has
            // already been written — one that may already be serving.
            if (!ReferenceEquals(_owner.ContextAccessor.Current, _context)
                || !ReferenceEquals(_context.Container, _container))
            {
                Forget();
                return;
            }

            try
            {
                // The scope closing is where a container groups what was written into it, which is
                // the Python version's `__exit__`. A layout does it, and so does a component that holds
                // children: the rule belongs to the container's kind, so the container is asked
                // rather than the type being tested here.
                if (_container.AllowsExpectedParents)
                {
                    _container.FillExpectedParents(_owner.CreateContainer);
                }
            }
            finally
            {
                // Restored even when the grouping refused: the container that was open before is
                // still the one the next block belongs to, and leaving it pointing at a container
                // whose scope failed would put every later block in the wrong place.
                _context.Container = _previous;
                Forget();
            }
        }

        /// <summary>Tells the container its scope is over.</summary>
        private void Forget()
        {
            // The container forgot its scope before it closes, so that disposing the container
            // afterwards is the no-op it should be rather than a second close.
            if (_container is BlockContext entered)
            {
                entered.Closed();
            }
        }
    }
}

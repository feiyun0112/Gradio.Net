using System.Text;
using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// The document the application serves about itself: what the frontend boots from.
/// </summary>
/// <remarks>
/// It lives apart from the render root's own code because it changes for its own reason. The root
/// changes when rendering does; this changes when the Python version's config does, which is a
/// different question asked on a different day, and the table has the same shape for the same
/// reason.
/// </remarks>
public sealed partial class Blocks
{
    /// <summary>The decoder a stylesheet is read with: UTF-8, and strict about it.</summary>
    /// <remarks>
    /// Strict because a file that is not UTF-8 is a mistake rather than something to paper over
    /// with replacement characters, which is what the Python version's read does with one too.
    /// </remarks>
    private static readonly UTF8Encoding _utf8 = new(
        encoderShouldEmitUTF8Identifier: false,
        throwOnInvalidBytes: true);

    /// <summary>
    /// The document <c>/config</c> serves, as the Python version builds it.
    /// </summary>
    /// <returns>The config, with everything the endpoint adds — <c>username</c>, <c>root</c> — left out.</returns>
    /// <remarks>
    /// <para>
    /// The key set and the values are the Python version's (<c>Blocks.get_config_file</c> plus
    /// <c>BlocksConfig.get_config</c>), because the vendored frontend reads them unchanged.
    /// </para>
    /// <para>
    /// Where it produces a value the Python version would not by default — analytics — the field
    /// carries whatever the application set, which is why a test sets it explicitly.
    /// </para>
    /// <para>
    /// Building it can touch the disk: an application that named CSS files has them read here, so a
    /// path that does not exist fails this call rather than being skipped.
    /// </para>
    /// </remarks>
    public JsonObject GetConfig()
    {
        var components = new JsonArray();
        var componentIds = new JsonArray();
        var dependencies = new JsonArray();
        var dependencyIds = new JsonArray();
        var layout = LayoutOf(this);

        // One entry per page, each with the layout tree narrowed to its own children — the Python version
        // builds the same map before it fills anything in, because a page with nothing on it still
        // has a layout (the root's) and an empty list of components.
        var pages = new JsonObject();
        var entries = new Dictionary<string, JsonObject>(StringComparer.Ordinal);
        var pageChildren = new Dictionary<string, JsonArray>(StringComparer.Ordinal);

        foreach (var page in _pages)
        {
            var children = new JsonArray();
            var entry = new JsonObject
            {
                ["layout"] = new JsonObject
                {
                    ["id"] = Id,
                    ["children"] = children,
                },
                ["components"] = new JsonArray(),
                ["dependencies"] = new JsonArray(),
            };

            pages[page.Path] = entry;
            entries[page.Path] = entry;
            pageChildren[page.Path] = children;
        }

        foreach (var child in Children)
        {
            ChildrenOf(pageChildren, child.PagePath).Add(LayoutOf(child));
        }

        foreach (var block in _blocks)
        {
            components.Add(block.ToConfigEntry());
            componentIds.Add(block.Id);
            ListOf(entries, block.PagePath, "components").Add(block.Id);
        }

        foreach (var dependency in _dependencies)
        {
            dependencies.Add(dependency.ToConfigEntry());
            dependencyIds.Add(dependency.Id);
            ListOf(entries, dependency.PagePath, "dependencies").Add(dependency.Id);
        }

        var pageList = new JsonArray();
        foreach (var page in _pages)
        {
            pageList.Add(page.ToConfigEntry());
        }

        return new JsonObject
        {
            ["version"] = GradioVersion.Upstream,
            ["api_prefix"] = "/gradio_api",
            ["mode"] = Mode,
            ["app_id"] = AppId,
            ["dev_mode"] = false,
            ["vibe_mode"] = VibeMode,
            ["analytics_enabled"] = AnalyticsEnabled,
            ["components"] = components,
            ["css"] = ConfigCss(),
            ["connect_heartbeat"] = NeedsHeartbeat(),
            ["js"] = null,
            ["head"] = null,
            ["title"] = Title,
            ["space_id"] = null,
            ["enable_queue"] = true,
            ["show_error"] = ShowError,
            ["footer_links"] = FooterLinks(),
            ["run_history"] = RunHistory,
            ["is_colab"] = false,
            ["max_file_size"] = MaxFileSize,
            ["stylesheets"] = Stylesheets(),
            ["theme"] = Theme.Name,

            // The protocol version the frontend negotiates against; the Python version advertises sse_v3.
            ["protocol"] = "sse_v3",

            // The four values the page paints itself with before the stylesheet arrives. They are
            // computed rather than referenced: a `var(--...)` here would be a reference into a
            // stylesheet the page has not loaded yet.
            ["body_css"] = BodyCss(),
            ["fill_height"] = FillHeight,
            ["fill_width"] = FillWidth,
            ["theme_hash"] = ThemeHash,
            ["pwa"] = Pwa,

            ["pages"] = pageList,
            ["page"] = pages,
            ["mcp_server"] = McpServer,
            ["i18n_translations"] = I18n?.ToConfig(),
            ["dependencies"] = dependencies,
            ["layout"] = layout,
        };
    }

    /// <summary>
    /// The document a page carries instead of the application's config when nobody has signed in.
    /// </summary>
    /// <param name="root">The address the application is being served from.</param>
    /// <returns>The config a page renders the sign-in form from.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="root"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// <para>
    /// This is not the application's config narrowed or filtered — it is a different document with
    /// nine keys, which is what the Python version serves. Nothing about the application leaks
    /// through it: no component, no binding, no title. The frontend reads <c>auth_required</c> and
    /// draws the sign-in form instead of the page.
    /// </para>
    /// <para>
    /// <c>pages</c> is the one key whose shape is the Python version's rather than this
    /// framework's: the Python version writes a list holding the home page's path, where a normal
    /// config carries one triple per page. It is reproduced as the Python version has it, because a
    /// frontend that reads it is reading the Python version's shape.
    /// </para>
    /// </remarks>
    public JsonObject GetAuthRequiredConfig(string root)
    {
        ArgumentNullException.ThrowIfNull(root);

        return new JsonObject
        {
            ["auth_required"] = true,
            ["auth_message"] = AuthMessage,
            ["space_id"] = null,
            ["root"] = root,
            ["page"] = new JsonObject
            {
                [string.Empty] = new JsonObject { ["layout"] = new JsonObject() },
            },
            ["pages"] = new JsonArray(string.Empty),
            ["components"] = new JsonArray(),
            ["dependencies"] = new JsonArray(),
            ["current_page"] = string.Empty,
        };
    }

    /// <summary>
    /// The CSS the config carries: the application's own, then the text of the files it named.
    /// </summary>
    /// <returns>The stylesheet text, or <see langword="null"/> when there is none.</returns>
    /// <remarks>
    /// The Python version's concatenation: each file's text follows the last with a newline in
    /// front of it, and an application that set no CSS of its own starts with that newline rather
    /// than with nothing. Nothing about CSS needs the newline — two rules may touch — so it is the
    /// Python version's byte choice rather than a requirement, reproduced because the served text
    /// is compared as text.
    /// </remarks>
    /// <exception cref="IOException">A path does not name a file that can be read.</exception>
    private string? ConfigCss()
    {
        if (CssPaths is not { Count: > 0 } paths)
        {
            return Css;
        }

        var text = new StringBuilder(Css ?? string.Empty);

        foreach (var path in paths)
        {
            text.Append('\n').Append(ReadStylesheet(path));
        }

        return text.ToString();
    }

    /// <summary>Reads a stylesheet the way the Python version's text-mode read does.</summary>
    /// <param name="path">The file's path.</param>
    /// <returns>The text.</returns>
    /// <exception cref="IOException">The file cannot be read.</exception>
    /// <exception cref="ArgumentException">The file is not UTF-8.</exception>
    /// <remarks>
    /// Two things <see cref="System.IO.File.ReadAllText(string)"/> does differently would show in the served
    /// text, so the bytes are read and decoded here instead. Python's text mode has universal
    /// newlines — a carriage return, with or without a line feed, arrives as a line feed — so a
    /// stylesheet written on Windows is served with the bytes a stylesheet written anywhere else
    /// has. And it keeps a byte-order mark as the first character, because the codec the Python
    /// version names is <c>utf-8</c> rather than <c>utf-8-sig</c>; .NET's reader detects and drops
    /// one. A file that is not UTF-8 raises, which is also what the Python version does.
    /// </remarks>
    private static string ReadStylesheet(string path) =>
        _utf8.GetString(System.IO.File.ReadAllBytes(path))
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n');

    /// <summary>
    /// The four theme values the page paints itself with, as the Python version's <c>body_css</c>.
    /// </summary>
    /// <returns>The computed values of the body's background and text colour, light and dark.</returns>
    /// <remarks>
    /// The page embeds these in a <c>:root</c> block of its own, so that the first paint is in the
    /// application's colours rather than the browser's default white — the stylesheet arrives a
    /// moment later. The Python version computes the same four, in the same order.
    /// </remarks>
    private JsonObject BodyCss() => new()
    {
        ["body_background_fill"] = Theme.GetComputedValue("body_background_fill"),
        ["body_text_color"] = Theme.GetComputedValue("body_text_color"),
        ["body_background_fill_dark"] = Theme.GetComputedValue("body_background_fill_dark"),
        ["body_text_color_dark"] = Theme.GetComputedValue("body_text_color_dark"),
    };

    /// <summary>The stylesheet URLs the theme's fonts need.</summary>
    /// <returns>The URLs, in the order the theme declared its fonts.</returns>
    private JsonArray Stylesheets()
    {
        var sheets = new JsonArray();

        foreach (var sheet in Theme.Stylesheets)
        {
            sheets.Add(sheet);
        }

        return sheets;
    }

    /// <summary>
    /// Whether the frontend must keep a connection open to the server for as long as the page is.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> when the page holds state the server owns, when a binding listens for a
    /// value the client sends a chunk at a time, when the application runs something as the visitor
    /// leaves, or when a binding is cached per session.
    /// </returns>
    /// <remarks>
    /// <para>
    /// The Python version's <c>utils.connect_heartbeat</c>, which asks one question: does anything
    /// on the page outlive a single run? A stateful component does — its value is kept by the
    /// session, so a session that closed while its value was alive would leak it, and the heartbeat
    /// is what tells the server the browser is gone. A <c>stream</c> binding does — the client
    /// feeds it chunk by chunk, so there is no single request whose end could mean "done".
    /// </para>
    /// <para>
    /// The Python version asks <c>isinstance(block, State)</c> because Python has no interface to
    /// ask for; <see cref="IStatefulComponent"/> is that same question here, and the answer is the
    /// same for every component this framework has — a <c>BrowserState</c> keeps its value on the
    /// client, so neither side counts it.
    /// </para>
    /// <para>
    /// The two cases the framework adds are both about work the server does when the session ends:
    /// an <c>unload</c> binding, which is the connection ending being the only thing that tells the
    /// server the visitor has gone, and a per-session cache, whose entries are forgotten when the
    /// session ends. A page with either is served the connection; without it the work would have no
    /// moment to happen at.
    /// </para>
    /// </remarks>
    private bool NeedsHeartbeat() =>
        _blocks.Any(block => block is IStatefulComponent)
        || _dependencies.Any(dependency => dependency.Connection == Dependency.StreamConnection)
        || BindsUnload
        || _dependencies.Any(dependency => dependency.Handler?.Target is ICachedHandler { Cache.PerSession: true });

    /// <summary>The links the page's footer carries, as the Python version's four.</summary>
    /// <returns>The links, in the order the footer draws them.</returns>
    /// <remarks>
    /// The link to the run history page is dropped when the application does not keep one: the
    /// Python version makes the same choice, and a footer link to a page that refuses is worse than
    /// no link.
    /// </remarks>
    private JsonArray FooterLinks()
    {
        var links = new JsonArray("api", "gradio", "settings");

        if (RunHistory)
        {
            links.Add("runs");
        }

        return links;
    }

    /// <summary>
    /// The config one page of the application carries, as the page itself embeds it.
    /// </summary>
    /// <param name="page">The page's path; the home page is spelled as the empty string.</param>
    /// <param name="components">
    /// The components to describe instead of the application's own — a deep link's — or
    /// <see langword="null"/> to describe the page's.
    /// </param>
    /// <returns>The config the page's own script element carries.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="page"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="page"/> is not a page this application has.
    /// </exception>
    /// <remarks>
    /// <para>
    /// This is the Python version's <c>get_page_config</c>: everything the whole application's
    /// config carries, with the components, bindings and layout narrowed to the ones this page
    /// names, and with <c>current_page</c> saying which page the frontend is rendering. It is not
    /// the same document as <see cref="GetConfig"/> — the page carries <c>current_page</c>, and it
    /// lists the three narrowed keys last, which is the order the Python version builds it in.
    /// </para>
    /// <para>
    /// The narrowing keeps the application's order rather than the page's: a page's components are
    /// the entries of the whole list that it names, in the order that list has them, which is what
    /// makes the same component's id mean the same thing in both documents.
    /// </para>
    /// </remarks>
    public JsonObject GetPageConfig(string page, JsonArray? components = null)
    {
        ArgumentNullException.ThrowIfNull(page);

        if (!_pages.Any(candidate => candidate.Path == page))
        {
            throw new ArgumentException(
                $"The application has no page named '{PageLabel(page)}'; it has "
                + string.Join(", ", _pages.Select(candidate => $"'{PageLabel(candidate.Path)}'")) + ".",
                nameof(page));
        }

        var config = GetConfig();
        var pageConfig = new JsonObject();

        // The Python version copies everything but the three narrowed keys, then adds them back at the end.
        foreach (var (key, value) in config)
        {
            if (key is not ("components" or "dependencies" or "layout"))
            {
                pageConfig[key] = value?.DeepClone();
            }
        }

        var pageEntry = config["page"]![page]!;

        // A deep link names the components the page should describe — the ones a session had, with
        // the values the user filled in — so they replace the page's own list rather than joining it.
        // They are still narrowed to this page, because a link is a link to one page.
        pageConfig["components"] = components is null
            ? Named(config["components"]!.AsArray(), pageEntry["components"]!.AsArray())
            : OnPage(components, pageEntry["components"]!.AsArray());
        pageConfig["dependencies"] = Named(config["dependencies"]!.AsArray(), pageEntry["dependencies"]!.AsArray());
        pageConfig["layout"] = pageEntry["layout"]!.DeepClone();
        pageConfig["current_page"] = page;

        return pageConfig;
    }

    /// <summary>The entries of a given list that a page names, in the given list's order.</summary>
    /// <param name="entries">The components to narrow — a deep link's.</param>
    /// <param name="ids">The ids the page names.</param>
    /// <returns>The entries the page describes, in the order they were given.</returns>
    /// <remarks>
    /// The order is the given list's rather than the application's: a deep link's components are the
    /// ones a session had, and the frontend renders them in the order the file holds them.
    /// </remarks>
    private static JsonArray OnPage(JsonArray entries, JsonArray ids)
    {
        var named = new HashSet<int>(ids.Select(id => id!.GetValue<int>()));
        var kept = new JsonArray();

        foreach (var entry in entries)
        {
            if (entry?["id"] is JsonValue id && named.Contains(id.GetValue<int>()))
            {
                kept.Add(entry.DeepClone());
            }
        }

        return kept;
    }

    /// <summary>The entries of a list that a page names, in the list's own order.</summary>
    /// <param name="entries">The application's entries — components or bindings.</param>
    /// <param name="ids">The ids the page names.</param>
    private static JsonArray Named(JsonArray entries, JsonArray ids)
    {
        var named = new HashSet<int>(ids.Select(id => id!.GetValue<int>()));
        var kept = new JsonArray();

        foreach (var entry in entries)
        {
            if (named.Contains(entry!["id"]!.GetValue<int>()))
            {
                kept.Add(entry.DeepClone());
            }
        }

        return kept;
    }

    /// <summary>One page's layout children, which the page's layout node carries.</summary>
    /// <param name="children">The children lists being built, by page.</param>
    /// <param name="page">The page's path.</param>
    /// <exception cref="InvalidOperationException">A block names a page the application does not have.</exception>
    private static JsonArray ChildrenOf(Dictionary<string, JsonArray> children, string page) =>
        children.TryGetValue(page, out var list)
            ? list
            : throw new InvalidOperationException(
                $"A block belongs to the page '{page}', which the application does not have.");

    /// <summary>One page's list of components or bindings, which the page's entry carries.</summary>
    /// <param name="entries">The page map being built.</param>
    /// <param name="page">The page's path.</param>
    /// <param name="key">The list to reach for: <c>components</c> or <c>dependencies</c>.</param>
    /// <exception cref="InvalidOperationException">A block names a page the application does not have.</exception>
    private static JsonArray ListOf(Dictionary<string, JsonObject> entries, string page, string key) =>
        entries.TryGetValue(page, out var entry)
            ? entry[key]!.AsArray()
            : throw new InvalidOperationException(
                $"A block belongs to the page '{page}', which the application does not have.");

    private static JsonObject LayoutOf(Block block)
    {
        var node = new JsonObject { ["id"] = block.Id };

        if (block.LayoutChildren is not { } context)
        {
            return node;
        }

        var children = new JsonArray();
        foreach (var child in context)
        {
            children.Add(LayoutOf(child));
        }

        node["children"] = children;

        return node;
    }
}

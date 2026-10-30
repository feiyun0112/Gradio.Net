using System.Globalization;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace Gradio.Net;
/// <summary>
/// Serves the application page and everything it loads: the vendored frontend's assets, the theme,
/// the manifest, the robots file and the icon.
/// </summary>
/// <remarks>
/// <para>
/// This is the capability that makes protocol-layer alignment a fact rather than an intention: the
/// page is the Python version's own compiled frontend, rendered from the Python version's own
/// template and served byte for byte as it is checked in, so a browser runs the same client code
/// against this server that it would run against the Python version. Nothing here rewrites, patches
/// or bundles anything — that is why the assets are an immutable, hash-verified tree.
/// </para>
/// <para>
/// The routes are the ones the table declares, and the refusals are the Python version's: an empty
/// asset path is a malformed request, a path that would leave the tree is forbidden, a directory is
/// not a file, and a path that names nothing is missing. The page answers both GET and HEAD,
/// because the Python version registers both and a client that asks HEAD should learn the page's
/// size.
/// </para>
/// <para>
/// The icon routes are the one place where a refusal is this framework's rather than the Python
/// version's, and it is the one place the Python version answers a size it cannot draw by raising:
/// see <see cref="PwaIconSizedAsync"/> for what is answered instead and why. The manifest is not a
/// refusal at all — it is a document, and which icon it names is <see cref="AppIcon"/>'s rule.
/// </para>
/// </remarks>
internal sealed partial class GradioFrontendEndpoints : IGradioEndpoint
{
    private const string PagePath = "/";
    private const string PageTemplatePath = "/{page}";
    private const string ThemeCssPath = "/theme.css";
    private const string ApiThemeCssPath = "/gradio_api/theme.css";
    private const string FaviconPath = "/favicon.ico";
    private const string ManifestPath = "/manifest.json";
    private const string RobotsPath = "/robots.txt";
    private const string AssetsPath = "/assets/{path:path}";
    private const string StaticPath = "/static/{path:path}";
    private const string SveltePath = "/svelte/{path:path}";
    // The manifest names this route, so how it is spelled is AppIcon's to say rather than a second
    // copy here.
    private const string PwaIconPath = AppIcon.IconPath;
    private const string PwaIconSizedPath = "/pwa_icon/{size}";
    private const string RunHistoryPath = "/gradio_api/runs";

    /// <summary>The document's relative URLs resolve against the page itself.</summary>
    private const string PageBaseUrl = "./";

    /// <summary>
    /// The same document served from a path one level deeper resolves its URLs one level up.
    /// </summary>
    /// <remarks>
    /// The run history page is at <c>/gradio_api/runs</c> and loads the same assets as the home page,
    /// which are served from the root: a base of <c>./</c> there would ask for
    /// <c>/gradio_api/assets/…</c>, which is a path no route answers.
    /// </remarks>
    private const string NestedPageBaseUrl = "../";

    private const string RobotsText = "User-agent: *\nDisallow: ";

    /// <summary>What the sized icon route says about a file that is not an image.</summary>
    private const string NotAnImage = "The icon is not an image, so it cannot be scaled.";

    private static readonly byte[] _robots = Encoding.UTF8.GetBytes(RobotsText);

    private readonly Blocks _blocks;
    private readonly FrontendAssets _assets;
    private readonly PageTemplate _page;
    private readonly GradioAuthentication _authentication;
    private readonly GradioLoginGate _gate;
    private readonly GradioOAuthSession _oauth;

    /// <summary>Creates the capability.</summary>
    /// <param name="blocks">The application whose page is served.</param>
    /// <param name="assets">The vendored frontend the page is built from.</param>
    /// <param name="authentication">Who is asking, for the config the page embeds.</param>
    /// <param name="gate">The refusal a host that authenticates its own requests answers with.</param>
    /// <param name="oauth">Who is signed in with an identity provider, for the label they see.</param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    public GradioFrontendEndpoints(
        Blocks blocks,
        FrontendAssets assets,
        GradioAuthentication authentication,
        GradioLoginGate gate,
        GradioOAuthSession oauth)
    {
        ArgumentNullException.ThrowIfNull(blocks);
        ArgumentNullException.ThrowIfNull(assets);
        ArgumentNullException.ThrowIfNull(authentication);
        ArgumentNullException.ThrowIfNull(gate);
        ArgumentNullException.ThrowIfNull(oauth);

        _blocks = blocks;
        _assets = assets;
        _authentication = authentication;
        _gate = gate;
        _oauth = oauth;
        _page = new PageTemplate(assets);
    }

    /// <summary>
    /// The route an application's other pages are served on, registered only by an application that
    /// has one.
    /// </summary>
    /// <remarks>
    /// The Python version registers a literal route per page (<c>/about</c>) at launch. One
    /// template answers the same set of URLs from the client's side and keeps the endpoint count
    /// independent of the number of pages; a path that names no page is refused, which is what the
    /// Python version's missing route would have done. It is not part of <see cref="Routes"/>
    /// because whether it exists is a property of the application rather than of this capability —
    /// see <see cref="GradioEndpointRouteBuilderExtensions.MapGradio"/>.
    /// </remarks>
    public const string PagesRoute = PageTemplatePath;

    /// <inheritdoc />
    public IReadOnlyCollection<string> Routes { get; } =
    [
        new GradioRoute("GET", PagePath).Key,
        new GradioRoute("HEAD", PagePath).Key,
        new GradioRoute("GET", ThemeCssPath).Key,
        new GradioRoute("GET", ApiThemeCssPath).Key,
        new GradioRoute("GET", FaviconPath).Key,
        new GradioRoute("GET", ManifestPath).Key,
        new GradioRoute("GET", RobotsPath).Key,
        new GradioRoute("GET", AssetsPath).Key,
        new GradioRoute("GET", StaticPath).Key,
        new GradioRoute("GET", SveltePath).Key,
        new GradioRoute("GET", PwaIconPath).Key,
        new GradioRoute("GET", PwaIconSizedPath).Key,
        new GradioRoute("GET", RunHistoryPath).Key,
    ];

    /// <inheritdoc />
    /// <remarks>
    /// The dispatch is on the route the request reached rather than on the request's path, because
    /// three of these routes are templates: <c>/assets/{path:path}</c> and its siblings would never
    /// equal the path a browser asks for.
    /// </remarks>
    public Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var route = GradioRouteMetadata.PathOf(context);

        return route == PageTemplatePath
            ? OtherPageAsync(context)
            : route switch
            {
                PagePath => PageAsync(context, string.Empty),
                RunHistoryPath => RunHistoryAsync(context),
                ThemeCssPath or ApiThemeCssPath => ThemeCssAsync(context),
                FaviconPath => FaviconAsync(context),
                ManifestPath => ManifestAsync(context),
                RobotsPath => RobotsAsync(context),
                AssetsPath => AssetRouteAsync(context, "assets/"),
                StaticPath => AssetRouteAsync(context, "static/"),

                // The Python version serves the compiled Svelte components from a directory beside the
                // bundle's assets; this release ships none, so every path under it is a miss.
                SveltePath => AssetRouteAsync(context, "assets/svelte/"),
                PwaIconPath => PwaIconAsync(context),
                PwaIconSizedPath => PwaIconSizedAsync(context),
                _ => NotFoundAsync(context),
            };
    }

    /// <summary>
    /// Serves one of the application's other pages, or refuses a path that names none.
    /// </summary>
    /// <param name="context">The request being answered.</param>
    /// <remarks>
    /// A page written with a trailing slash is redirected to the same page without one, which is
    /// what the Python version registers beside every page route: the address a page is served at
    /// is the one the navbar links to, and a second address for the same document is a second thing
    /// to keep right. The redirect is permanent and relative, as the Python version's is.
    /// <para>
    /// The page is looked up first, because the Python version registers the redirect only for
    /// pages it has: a path that names none is a miss, with or without its slash.
    /// </para>
    /// </remarks>
    private Task OtherPageAsync(HttpContext context)
    {
        var requested = context.Request.RouteValues["page"] as string ?? string.Empty;
        var page = Uri.UnescapeDataString(requested);

        if (!_blocks.Pages.Any(candidate => candidate.Path == page))
        {
            return NotFoundAsync(context);
        }

        if (context.Request.Path.Value?.EndsWith('/') == true)
        {
            context.Response.StatusCode = StatusCodes.Status301MovedPermanently;
            context.Response.Headers.Location = "/" + requested;

            return Task.CompletedTask;
        }

        return PageAsync(context, page);
    }

    /// <summary>Serves a page: the Python version's template, this application's config and API
    /// info.</summary> <param name="context">The request being answered.</param>
    /// <param name="page">The page to serve; the empty string is the home page.</param>
    /// <param name="baseUrl">
    /// What the document's relative URLs resolve against, or <see langword="null"/> for the page's
    /// own path.
    /// </param>
    /// <remarks>
    /// A page carries the config narrowed to its own components and bindings and the API info of
    /// its own endpoints, which is what makes the frontend render that page rather than the whole
    /// application: the document is the same template, and everything in it is about one page.
    /// </remarks>
    private async Task PageAsync(HttpContext context, string page, string? baseUrl = null)
    {
        var root = RootUrl.Of(context);
        var userName = await _authentication.UserNameOfAsync(context).ConfigureAwait(false);

        // An application that asks someone to sign in still serves its page, because the page is
        // where the sign-in form lives: what it carries is a config that describes no components at
        // all, and the frontend draws the form instead. An application that defers to its host's
        // authentication has no form to draw, so a request that host did not authenticate is
        // refused outright — which is the Python version's division between the two.
        if (_blocks.RequiresLogin && userName is null && _blocks.AuthDependency is not null)
        {
            await _gate.RefuseAsync(context).ConfigureAwait(false);

            return;
        }

        JsonObject config;

        if (_blocks.RequiresLogin && userName is null)
        {
            config = _blocks.GetAuthRequiredConfig(root);
        }
        else
        {
            // A share link names the state it was made from, and a page opened from one describes
            // that state rather than the application's own: the components are the session's, with
            // the values the user had filled in. A link the server cannot use is not an error — the
            // page is served as it would have been, and the config says the link was invalid.
            var (components, deepLinkState) = DeepLinkOf(context);

            config = _blocks.GetPageConfig(page, components);

            // The fields that belong to the request rather than to the application, in the order
            // the Python version appends them: who is asking, whether the link was a deep link, and where the
            // page is being served from. The description endpoint appends the same ones to /config.
            RequestScopedConfig.Append(config, context, userName, deepLinkState);
            SignedInLabel.Apply(config, _blocks, _oauth.ProfileOf(context));
        }

        var html = _page.Render(baseUrl ?? PageBaseUrl, config, _blocks.GetApiInfo(root, page));
        var body = Encoding.UTF8.GetBytes(html);

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "text/html; charset=utf-8";
        context.Response.ContentLength = body.Length;

        if (!HttpMethods.IsHead(context.Request.Method))
        {
            await context.Response.Body.WriteAsync(body, context.RequestAborted).ConfigureAwait(false);
        }
    }

    /// <summary>Serves the page that lists the application's runs, or refuses when it keeps none.</summary>
    /// <param name="context">The request being answered.</param>
    /// <returns>A task that completes once the answer is written.</returns>
    /// <remarks>
    /// The Python version serves its main page here: the frontend is one document that draws a
    /// different screen depending on the path it was loaded from, so there is no second template to
    /// render. An application that keeps no history has no such page, and the Python version
    /// answers 404 for it.
    /// </remarks>
    private Task RunHistoryAsync(HttpContext context)
    {
        if (!_blocks.RunHistory)
        {
            return NotFoundAsync(context);
        }

        return PageAsync(context, string.Empty, NestedPageBaseUrl);
    }

    /// <summary>The state a request's share link names, and whether the server could read it.</summary>
    /// <param name="context">The request.</param>
    /// <returns>The components to describe, and the state to report.</returns>
    /// <remarks>
    /// A request that names no link reports <c>none</c> rather than saying nothing, which is what
    /// the Python version's page does: the frontend reads the field to decide whether it is looking
    /// at a restored page. <c>/config</c> is the one that leaves the field out when it has nothing
    /// to say.
    /// </remarks>
    private (JsonArray? Components, string State) DeepLinkOf(HttpContext context)
    {
        if (!context.Request.Query.TryGetValue("deep_link", out var named) || named.Count == 0)
        {
            return (null, DeepLinkState.None);
        }

        var name = named.ToString();

        return name.Length == 0
            ? (null, DeepLinkState.None)
            : DeepLinkState.Load(_blocks, name);
    }

    /// <summary>
    /// Serves the CSS the page loads from <c>/theme.css</c>.
    /// </summary>
    /// <remarks>
    /// The theme system is its own batch (ticket 22), so the application's CSS is empty until it
    /// lands; the endpoint, its type and its path are the Python version's, and the frontend loads
    /// it either way.
    /// </remarks>
    private Task ThemeCssAsync(HttpContext context) =>
        WriteTextAsync(context, _blocks.ThemeCss, "text/css; charset=utf-8");

    /// <summary>
    /// Serves the web app manifest, which names the icon and the sizes it is good at.
    /// </summary>
    /// <param name="context">The request being answered.</param>
    /// <remarks>
    /// The document is <see cref="AppIcon"/>'s, because which icon a browser is offered is a rule
    /// about the icon rather than about this answer; what is decided here is the type it is sent as,
    /// which is the one a browser reads a manifest from and is not <c>application/json</c>.
    /// </remarks>
    private Task ManifestAsync(HttpContext context) =>
        WriteTextAsync(
            context,
            JsonText.Serialize(AppIcon.ManifestOf(_blocks)),
            "application/manifest+json");

    /// <summary>
    /// Serves the icon the application named, or the Python version's own logo when it names none.
    /// </summary>
    /// <param name="context">The request being answered.</param>
    /// <remarks>
    /// A page asks for this on every load, which is why an application that named no icon still
    /// gets one: a 404 here is a console error on every load of every page.
    /// </remarks>
    private Task FaviconAsync(HttpContext context) =>
        AppIcon.NamedBy(_blocks) is { } icon
            ? SendIconAsync(context, icon)
            : SendAssetAsync(context, FrontendAssets.DefaultFaviconAsset);

    /// <summary>Serves the application's icon at the size it already is.</summary>
    /// <param name="context">The request being answered.</param>
    /// <remarks>
    /// The manifest names this address for a vector, whose own size is what it is drawn at; for a
    /// raster the manifest names the two sized addresses instead. An application that named no icon
    /// has nothing here, and the Python version answers the same 404.
    /// </remarks>
    private Task PwaIconAsync(HttpContext context) =>
        AppIcon.NamedBy(_blocks) is { } icon ? SendIconAsync(context, icon) : NotFoundAsync(context);

    /// <summary>Serves the application's icon scaled to the size the request named.</summary>
    /// <param name="context">The request being answered.</param>
    /// <remarks>
    /// <para>
    /// A raster is scaled to the size asked for and sent as a PNG, whatever the file was: the
    /// manifest says the two entries are PNGs, so they have to be. A vector is drawn at any size
    /// and is sent as it is — the Python version tries to raster it and raises, which is a dropped
    /// connection rather than an answer, so the difference is declared in the ledger.
    /// </para>
    /// <para>
    /// A size that is not a whole number is refused with the document FastAPI answers for a path
    /// parameter that did not parse. Zero, negatives and sizes past <see cref="AppIcon.MaxSize"/>
    /// are refused the same way: the Python version raises for the first two and would allocate for
    /// the third, and neither is an answer a client can act on.
    /// </para>
    /// </remarks>
    private async Task PwaIconSizedAsync(HttpContext context)
    {
        // The size is parsed before anything else, because that is where the Python version's framework does
        // it: FastAPI validates a path parameter before the handler runs, so an application with no
        // icon still answers 422 for a size that is not a number rather than 404 for the icon.
        var requested = context.Request.RouteValues["size"] as string ?? string.Empty;

        if (!int.TryParse(requested, NumberStyles.Integer, CultureInfo.InvariantCulture, out var size))
        {
            await Answers.JsonAsync(
                context,
                StatusCodes.Status422UnprocessableEntity,
                ValidationError(
                    "int_parsing",
                    "Input should be a valid integer, unable to parse string as an integer",
                    requested))
                .ConfigureAwait(false);

            return;
        }

        if (AppIcon.NamedBy(_blocks) is not { } icon)
        {
            await NotFoundAsync(context).ConfigureAwait(false);

            return;
        }

        if (BoundsRefusal(size) is { } refusal)
        {
            await Answers.JsonAsync(context, StatusCodes.Status422UnprocessableEntity, refusal)
                .ConfigureAwait(false);

            return;
        }

        var content = await ReadIconAsync(icon, context.RequestAborted).ConfigureAwait(false);
        if (content is null)
        {
            await NotFoundAsync(context).ConfigureAwait(false);

            return;
        }

        if (AppIcon.IsVector(icon))
        {
            await WriteBytesAsync(context, content, ContentTypes.Of(icon)).ConfigureAwait(false);

            return;
        }

        byte[] scaled;
        try
        {
            scaled = RasterImage.Open(content).Resize(size, size).ToBytes("png");
        }
        catch (ArgumentException)
        {
            // The file is there and is not an image. The application named an icon that cannot be
            // made into one, which is a request that cannot be satisfied rather than a file that is
            // missing - and the Python version answers it by raising.
            await Answers.JsonAsync(
                context,
                StatusCodes.Status422UnprocessableEntity,
                new JsonObject { ["detail"] = NotAnImage })
                .ConfigureAwait(false);

            return;
        }

        await WriteBytesAsync(context, scaled, AppIcon.RasterType).ConfigureAwait(false);
    }

    /// <summary>Serves a file as the icon it was named as.</summary>
    /// <param name="context">The request being answered.</param>
    /// <param name="path">The icon's path.</param>
    /// <returns>A task that completes once the answer is written.</returns>
    /// <remarks>
    /// The type comes from the file's own extension rather than from the address it was asked for,
    /// which is what makes <c>/favicon.ico</c> answer a PNG as a PNG: the address is where a browser
    /// looks, and the file is what it gets.
    /// </remarks>
    private static async Task SendIconAsync(HttpContext context, string path)
    {
        var content = await ReadIconAsync(path, context.RequestAborted).ConfigureAwait(false);

        if (content is null)
        {
            await NotFoundAsync(context).ConfigureAwait(false);

            return;
        }

        await WriteBytesAsync(context, content, ContentTypes.Of(path)).ConfigureAwait(false);
    }

    /// <summary>Reads an icon file, or says it cannot be read.</summary>
    /// <param name="path">The icon's path.</param>
    /// <param name="cancellationToken">Cancels the read.</param>
    /// <returns>The bytes, or <see langword="null"/> when the file is not there or not readable.</returns>
    /// <remarks>
    /// An application that named an icon it does not have has a mistake rather than a default, so
    /// the answer is a refusal rather than a silent fallback to the Python version's logo — the
    /// choice <see cref="Blocks.CssPaths"/> makes about a stylesheet. The Python version raises
    /// here instead, which is a dropped connection, so the status is this port's and the ledger
    /// says so.
    /// </remarks>
    private static async Task<byte[]?> ReadIconAsync(string path, CancellationToken cancellationToken)
    {
        try
        {
            return await System.IO.File.ReadAllBytesAsync(path, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException)
        {
            return null;
        }
    }

    /// <summary>Why a size cannot be made into an icon, or <see langword="null"/> when it can.</summary>
    /// <param name="size">The size, already a whole number.</param>
    /// <returns>The refusal to answer with, or <see langword="null"/> when there is none.</returns>
    /// <remarks>
    /// Two rules: it has to be positive, and it has to be no larger than
    /// <see cref="AppIcon.MaxSize"/>. Both are where the Python version raises rather than answers,
    /// so the refusals are written in the document FastAPI uses for a size that did not parse at
    /// all.
    /// </remarks>
    private static JsonObject? BoundsRefusal(int size)
    {
        if (size <= 0)
        {
            return ValidationError(
                "greater_than",
                "Input should be greater than 0",
                size,
                new JsonObject { ["gt"] = 0 });
        }

        return size > AppIcon.MaxSize
            ? ValidationError(
                "less_than_equal",
                $"Input should be less than or equal to {AppIcon.MaxSize}",
                size,
                new JsonObject { ["le"] = AppIcon.MaxSize })
            : null;
    }

    /// <summary>
    /// The document FastAPI answers a request whose path parameter did not validate.
    /// </summary>
    /// <param name="type">The error's kind, as pydantic spells it.</param>
    /// <param name="message">The sentence a client reads.</param>
    /// <param name="input">What was asked for.</param>
    /// <param name="bounds">The bounds the rule had, or <see langword="null"/> when it had none.</param>
    /// <returns>The document: one entry under <c>detail</c>, which is FastAPI's shape.</returns>
    private static JsonObject ValidationError(
        string type,
        string message,
        JsonNode? input,
        JsonObject? bounds = null)
    {
        var error = new JsonObject
        {
            ["type"] = type,
            ["loc"] = new JsonArray("path", "size"),
            ["msg"] = message,
            ["input"] = input,
        };

        if (bounds is not null)
        {
            error["ctx"] = bounds;
        }

        return new JsonObject { ["detail"] = new JsonArray(error) };
    }

    private static Task RobotsAsync(HttpContext context) =>
        WriteBytesAsync(context, _robots, "text/plain; charset=utf-8");

    /// <summary>
    /// Serves the asset a route's path parameter names, under <paramref name="prefix"/>.
    /// </summary>
    /// <param name="context">The request.</param>
    /// <param name="prefix">The directory the route serves from, e.g. <c>assets/</c>.</param>
    /// <remarks>
    /// <para>
    /// The parameter is checked before the prefix is put in front of it, because the Python version
    /// checks the path the client asked for before it joins it to the tree: a request for
    /// <c>/assets/</c> asks the route itself for a file (a malformed request, not a missing one),
    /// and a path that names a URL, a UNC share or an absolute location is an attempt to make the
    /// server fetch something (forbidden, not missing).
    /// </para>
    /// <para>
    /// The value is unescaped first because the URL-shaped path is only visible once it is:
    /// ASP.NET Core leaves <c>%2F</c> escaped in the path, so
    /// <c>/assets/http%3A%2F%2Fexample.com%2Fa.png</c> would otherwise arrive as a name nothing
    /// could have on disk and be answered "missing".
    /// </para>
    /// </remarks>
    private Task AssetRouteAsync(HttpContext context, string prefix)
    {
        var path = context.Request.RouteValues["path"] as string ?? string.Empty;

        if (path.Length == 0)
        {
            return RefuseAsync(context, AssetStatus.EmptyPath);
        }

        var requested = Uri.UnescapeDataString(path);

        return NamesALocation(requested)
            ? RefuseAsync(context, AssetStatus.OutsideRoot)
            : SendAssetAsync(context, prefix + requested);
    }

    /// <summary>
    /// Whether a path the client asked for names a location rather than a file in the tree.
    /// </summary>
    /// <param name="path">The path as it arrived, before the route's own prefix is joined to it.</param>
    /// <returns><see langword="true"/> when the path is absolute or starts with a scheme.</returns>
    /// <remarks>
    /// The Python version's <c>starts_with_protocol</c>: a scheme followed by <c>://</c>, a
    /// protocol-relative <c>//</c>, or the <c>\\</c> / <c>\/</c> / <c>/\</c> spellings Windows
    /// reads as SMB paths. An absolute path is in the same family — joining it to a directory does
    /// not keep it under that directory — and the Python version refuses it too, by way of the join
    /// rather than by this rule.
    /// </remarks>
    private static bool NamesALocation(string path) =>
        path.StartsWith('/')
        || ProtocolPrefix().IsMatch(path);

    /// <summary>A scheme, a protocol-relative path, or a Windows UNC spelling.</summary>
    [GeneratedRegex(@"^(?:[a-zA-Z][a-zA-Z0-9+\-.]*://|//|\\\\|\\/|/\\)", RegexOptions.CultureInvariant)]
    private static partial Regex ProtocolPrefix();

    private async Task SendAssetAsync(HttpContext context, string assetPath)
    {
        var status = _assets.Open(assetPath, out var content, out var contentType);
        if (status != AssetStatus.Found || content is null)
        {
            await RefuseAsync(context, status).ConfigureAwait(false);

            return;
        }

        await using (content.ConfigureAwait(false))
        {
            context.Response.StatusCode = StatusCodes.Status200OK;
            context.Response.ContentType = contentType;
            context.Response.ContentLength = content.Length;

            if (!HttpMethods.IsHead(context.Request.Method))
            {
                await content.CopyToAsync(context.Response.Body, context.RequestAborted).ConfigureAwait(false);
            }
        }
    }

    private static Task RefuseAsync(HttpContext context, AssetStatus status) => status switch
    {
        AssetStatus.EmptyPath => ProblemAsync(context, StatusCodes.Status400BadRequest, "Bad Request"),
        AssetStatus.OutsideRoot or AssetStatus.Directory =>
            ProblemAsync(context, StatusCodes.Status403Forbidden, "Forbidden"),
        _ => NotFoundAsync(context),
    };

    private static Task NotFoundAsync(HttpContext context) =>
        ProblemAsync(context, StatusCodes.Status404NotFound, "Not Found");

    /// <summary>Answers the way the Python version's HTTP layer answers a refusal.</summary>
    private static Task ProblemAsync(HttpContext context, int status, string detail)
    {
        var body = JsonText.Serialize(new JsonObject { ["detail"] = detail });
        var bytes = Encoding.UTF8.GetBytes(body);

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";
        context.Response.ContentLength = bytes.Length;

        return HttpMethods.IsHead(context.Request.Method)
            ? Task.CompletedTask
            : context.Response.Body.WriteAsync(bytes, context.RequestAborted).AsTask();
    }

    private static Task WriteTextAsync(HttpContext context, string text, string contentType) =>
        WriteBytesAsync(context, Encoding.UTF8.GetBytes(text), contentType);

    private static Task WriteBytesAsync(HttpContext context, byte[] body, string contentType)
    {
        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = contentType;
        context.Response.ContentLength = body.Length;

        return HttpMethods.IsHead(context.Request.Method)
            ? Task.CompletedTask
            : context.Response.Body.WriteAsync(body, context.RequestAborted).AsTask();
    }
}

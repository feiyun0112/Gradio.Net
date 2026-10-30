using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;

namespace Gradio.Net;
/// <summary>
/// Serves the three documents that describe an application: its config, its API info, and its
/// OpenAPI schema.
/// </summary>
/// <remarks>
/// <para>
/// They are one capability because they are one subject — what the application is and what it can
/// be asked to do — and because they share the two things that are awkward to get right: the
/// application comes from the container rather than from the request, and the root URL is derived
/// from the request rather than configured.
/// </para>
/// <para>
/// The config is the document the vendored frontend boots from, so it is served exactly as the
/// Python version builds it. The other two are derived from the same application, which is why an
/// application that binds no event describes no endpoint on either side.
/// </para>
/// </remarks>
internal sealed class GradioDescriptionEndpoints : IGradioEndpoint
{
    private const string ConfigPath = "/config";
    private const string InfoPath = "/gradio_api/info";
    private const string OpenApiPath = "/gradio_api/openapi.json";

    private readonly Blocks _blocks;
    private readonly GradioAuthentication _authentication;
    private readonly GradioOAuthSession _oauth;

    /// <summary>Creates the capability.</summary>
    /// <param name="blocks">The application to describe.</param>
    /// <param name="authentication">Who is asking, for the config's <c>username</c>.</param>
    /// <param name="oauth">Who is signed in with an identity provider, for the label a signed-in user sees.</param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    public GradioDescriptionEndpoints(
        Blocks blocks,
        GradioAuthentication authentication,
        GradioOAuthSession oauth)
    {
        ArgumentNullException.ThrowIfNull(blocks);
        ArgumentNullException.ThrowIfNull(authentication);
        ArgumentNullException.ThrowIfNull(oauth);

        _blocks = blocks;
        _authentication = authentication;
        _oauth = oauth;
    }

    /// <inheritdoc />
    /// <remarks>
    /// Built through <see cref="GradioRoute.Key"/> rather than spelled out, so that the claim
    /// cannot drift from the table it is a claim about: the key is how the mapping loop looks a
    /// route up, and a second spelling of it here would be a second thing to keep in step.
    /// </remarks>
    public IReadOnlyCollection<string> Routes { get; } =
    [
        new GradioRoute("GET", ConfigPath).Key,
        new GradioRoute("GET", InfoPath).Key,
        new GradioRoute("GET", OpenApiPath).Key,
    ];

    /// <inheritdoc />
    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var document = context.Request.Path.Value switch
        {
            ConfigPath => await ConfigAsync(context).ConfigureAwait(false),

            // Both carry the address the application is being served from — the API info in the
            // example calls it shows, the OpenAPI document not at all — so both are built against
            // the request rather than against the application alone.
            InfoPath => _blocks.GetApiInfo(RootUrl.Of(context), PageOf(context), AllEndpointsOf(context)),
            OpenApiPath => _blocks.GetOpenApiDocument(RootUrl.Of(context)),
            _ => null,
        };

        // Reaching here on another path would mean the mapping loop handed this capability a route
        // it does not claim; answering 404 says so without pretending to describe anything.
        if (document is null)
        {
            await NotFoundAsync(context).ConfigureAwait(false);

            return;
        }

        await WriteAsync(context, document).ConfigureAwait(false);
    }

    /// <summary>The config a request asks for, narrowed to a page and restored from a link.</summary>
    /// <param name="context">The request.</param>
    /// <returns>The config.</returns>
    /// <remarks>
    /// <para>
    /// Two query parameters change the answer, and both are the Python version's. <c>page</c>
    /// narrows the document to one page's components and bindings — a client that knows which page
    /// it is asking about asks about that one. <c>deep_link</c> replaces the components with the
    /// ones a session saved, which is how a client opens the application where a link left it.
    /// </para>
    /// <para>
    /// A page the application does not have is not an error: the whole document is served, which is
    /// what the Python version does when the page it was asked about is not one it has.
    /// </para>
    /// </remarks>
    private async Task<JsonObject> ConfigAsync(HttpContext context)
    {
        var page = PageOf(context);
        var selected = page is not null && _blocks.Pages.Any(candidate => candidate.Path == page)
            ? page
            : null;

        var (components, state) = DeepLinkOf(context);

        var config = selected is null ? _blocks.GetConfig() : _blocks.GetPageConfig(selected, components);

        // Who is asking and where the application is served from are properties of the request,
        // not of the application; the page appends the same fields to the config it embeds.
        var userName = await _authentication.UserNameOfAsync(context).ConfigureAwait(false);

        RequestScopedConfig.Append(config, context, userName);
        SignedInLabel.Apply(config, _blocks, _oauth.ProfileOf(context));

        // A whole document has no page to narrow, so the link's components replace its own list.
        // The state is reported only for a request that named a link, which is the Python version's rule: a
        // client that never asked for one is not told about one.
        if (state is not null)
        {
            if (selected is null && components is not null)
            {
                config["components"] = components.DeepClone();
            }

            config["deep_link_state"] = state;
        }

        return config;
    }

    /// <summary>The state a request's share link names, and whether the server could read it.</summary>
    /// <param name="context">The request.</param>
    /// <returns>The components to describe, and the state to report, or nothing for a request that named no link.</returns>
    private (JsonArray? Components, string? State) DeepLinkOf(HttpContext context)
    {
        if (!context.Request.Query.TryGetValue("deep_link", out var named) || named.Count == 0)
        {
            return (null, null);
        }

        var name = named.ToString();

        return name.Length == 0
            ? (null, DeepLinkState.Invalid)
            : DeepLinkState.Load(_blocks, name);
    }

    /// <summary>The page an API-info request asks about, or <see langword="null"/> for the whole application.</summary>
    /// <param name="context">The request being answered.</param>
    /// <returns>The page's path, as the query names it.</returns>
    /// <remarks>
    /// The Python version's <c>/gradio_api/info?page=about</c>: the same document, narrowed to the
    /// endpoints that belong to one page — which is the question a client asks when it knows which
    /// page it is looking at. A page the application does not have describes no endpoint, which is
    /// what the Python version answers too: the narrowing is a filter over the bindings, not a
    /// lookup that can fail.
    /// </remarks>
    private static string? PageOf(HttpContext context) =>
        context.Request.Query.TryGetValue("page", out var page) ? page.ToString() : null;

    /// <summary>Whether the request asks for the endpoints the documentation leaves out.</summary>
    /// <param name="context">The request being answered.</param>
    /// <returns>Whether <c>all_endpoints</c> was named at all.</returns>
    /// <remarks>
    /// Present means yes, whatever it says: the Python version reads the parameter's presence
    /// rather than its value, so <c>?all_endpoints=false</c> asks for every endpoint there too.
    /// </remarks>
    private static bool AllEndpointsOf(HttpContext context) =>
        context.Request.Query.ContainsKey("all_endpoints");

    private static Task NotFoundAsync(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;

        return Task.CompletedTask;
    }

    private static Task WriteAsync(HttpContext context, JsonNode document)
    {
        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "application/json; charset=utf-8";

        // The document is already a JSON tree with the Python version's key names, so it is written as the
        // tree has it: a naming policy or a camel-cased contract applied on the way out would
        // rename exactly the keys the comparison is about. The options are the ones every served
        // document uses, which is what keeps this byte for byte the Python version's (JsonText).
        return context.Response.WriteAsync(JsonText.Serialize(document), context.RequestAborted);
    }
}

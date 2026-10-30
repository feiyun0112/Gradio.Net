using System.Security.Cryptography;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;

namespace Gradio.Net;
/// <summary>
/// Serves the compiled frontend a custom component brings with it.
/// </summary>
/// <remarks>
/// <para>
/// This is the other half of <see cref="ICustomComponentBundle"/>: a component that ships its own
/// bundle names a directory and an identity, the config carries the identity to the browser, and
/// the frontend — which has never heard of the component — asks this route for
/// <c>&lt;variant&gt;/index.js</c> and its neighbours. Nothing here builds or inspects a bundle; it
/// is the application's artifact, served as it lies on disk.
/// </para>
/// <para>
/// The refusals are the Python version's, checked against it rather than read off its source: an
/// environment other than <c>client</c>/<c>server</c> is refused by the framework before the
/// handler runs, as FastAPI's validation of the route's <c>Literal</c> does; an identity no
/// component claims, and a path that climbs out of the component's directory, are both "Component
/// not found."; a file that is not there is Starlette's own "Not Found"; and a directory is
/// "Forbidden". The Python version's source reads otherwise — it has an <c>if environment not in
/// [...]</c> that its own validation makes unreachable, and it merges the last two refusals into
/// one <c>HTTPException</c> it never raises — so what is matched here is what the server answers,
/// not what the function appears to say.
/// </para>
/// <para>
/// The identity is resolved against the application's blocks rather than against a registry of its
/// own. That is narrower than the Python version, which scans every loaded component class: a class
/// that is in the process but not in the application cannot be served, which keeps this route from
/// becoming a way to read a directory the application never published.
/// </para>
/// </remarks>
internal sealed class GradioCustomComponentEndpoint : IGradioEndpoint
{
    /// <summary>The route, as the Python version's route table spells it.</summary>
    private const string RoutePath =
        "/gradio_api/custom_component/{id}/{environment}/{type}/{file_name}";

    /// <summary>The two environments the Python version's <c>Literal</c> accepts, and the only two
    /// a browser asks for.</summary>
    private static readonly string[] _environments = ["client", "server"];

    /// <summary>
    /// What the Python version's <c>Literal["client", "server"]</c> makes the framework answer for
    /// anything else — a validation document rather than a sentence, because FastAPI validates
    /// before the handler and never reaches the sentence the handler would have said.
    /// </summary>
    private const string ExpectedEnvironment = "'client' or 'server'";

    /// <summary>The refusal an identity no component claims gets, and a path that leaves the bundle.</summary>
    private const string ComponentNotFound = "Component not found.";

    /// <summary>What Starlette answers for a file that is not there.</summary>
    private const string FileNotFound = "Not Found";

    /// <summary>What Starlette answers for a path that names a directory.</summary>
    private const string IsADirectory = "Forbidden";

    /// <summary>
    /// What the Python version's file response says about a bundle: revalidate every time, and
    /// revalidate against the hash rather than the date.
    /// </summary>
    private const string CacheControl = "max-age=0, must-revalidate";

    private readonly Blocks _blocks;

    /// <summary>Creates the capability.</summary>
    /// <param name="blocks">The application whose components are served.</param>
    /// <exception cref="ArgumentNullException"><paramref name="blocks"/> is <see langword="null"/>.</exception>
    public GradioCustomComponentEndpoint(Blocks blocks)
    {
        ArgumentNullException.ThrowIfNull(blocks);

        _blocks = blocks;
    }

    /// <inheritdoc />
    public IReadOnlyCollection<string> Routes { get; } = [new GradioRoute("GET", RoutePath).Key];

    /// <inheritdoc />
    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var environment = RouteValue(context, "environment");
        if (!_environments.Contains(environment, StringComparer.Ordinal))
        {
            await Answers.JsonAsync(context, StatusCodes.Status422UnprocessableEntity, LiteralError(environment))
                .ConfigureAwait(false);

            return;
        }

        var directory = BundleDirectoryOf(RouteValue(context, "id"));
        if (directory is null
            || !TryResolve(directory, RouteValue(context, "type"), RouteValue(context, "file_name"), out var path))
        {
            await Answers.JsonAsync(context, StatusCodes.Status404NotFound, Detail(ComponentNotFound))
                .ConfigureAwait(false);

            return;
        }

        // A directory is not a file the browser can use, and the Python version refuses it rather than letting
        // the read fail: the request named something the bundle has, so the answer says what is
        // wrong with the request rather than pretending the component is unknown.
        if (Directory.Exists(path))
        {
            await Answers.JsonAsync(context, StatusCodes.Status403Forbidden, Detail(IsADirectory))
                .ConfigureAwait(false);

            return;
        }

        byte[] content;
        try
        {
            content = await System.IO.File.ReadAllBytesAsync(path, context.RequestAborted).ConfigureAwait(false);
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException)
        {
            // A file that was there a moment ago and is not readable now is, from the browser's
            // side, a component it cannot load - which is the same answer as one that is missing.
            await Answers.JsonAsync(context, StatusCodes.Status404NotFound, Detail(FileNotFound))
                .ConfigureAwait(false);

            return;
        }

        // The Python version revalidates on the file's contents, and it hashes the file as text because the
        // assets it serves are text. These are the application's files, which may be fonts or
        // images, so the hash is taken over the bytes: a text round-trip would rewrite line endings
        // and refuse a bundle with a binary asset in it.
        var etag = Convert.ToHexString(SHA256.HashData(content)).ToLowerInvariant();

        context.Response.Headers.ETag = etag;
        context.Response.Headers.CacheControl = CacheControl;

        if (string.Equals(context.Request.Headers.IfNoneMatch.ToString(), etag, StringComparison.Ordinal))
        {
            context.Response.StatusCode = StatusCodes.Status304NotModified;

            return;
        }

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = ContentTypes.Of(path);
        context.Response.ContentLength = content.Length;

        await context.Response.Body.WriteAsync(content, context.RequestAborted).ConfigureAwait(false);
    }

    /// <summary>The directory the component that claims <paramref name="id"/> names.</summary>
    /// <param name="id">The identity the request asked for.</param>
    /// <returns>The directory, or <see langword="null"/> when no component in the application claims it.</returns>
    /// <remarks>
    /// The first claim wins, which is also what the Python version's scan does: two components that
    /// declare the same identity are one component as far as the browser is concerned, and the
    /// application that did it has a mistake this route cannot diagnose.
    /// </remarks>
    private string? BundleDirectoryOf(string id)
    {
        foreach (var block in _blocks.Registered)
        {
            if (block is ICustomComponentBundle custom
                && string.Equals(custom.ComponentClassId, id, StringComparison.Ordinal))
            {
                return custom.TemplateDirectory;
            }
        }

        return null;
    }

    /// <summary>Resolves a variant and a file name under a component's own directory.</summary>
    /// <param name="directory">The directory the component names.</param>
    /// <param name="variant">The variant the frontend asked for, e.g. <c>component</c>.</param>
    /// <param name="fileName">The file within it, e.g. <c>index.js</c>.</param>
    /// <param name="path">The absolute path, when the request stays inside the directory.</param>
    /// <returns><see langword="true"/> when the path is inside <paramref name="directory"/>.</returns>
    /// <remarks>
    /// <para>
    /// The Python version's <c>safe_join</c> with one addition: a backslash is refused whatever the
    /// platform, because a bundle built on Windows and served from Linux is a directory name there
    /// rather than a separator, and the request that used it would read a file the component never
    /// meant to publish. A colon is refused for the same reason on Windows, where it names a drive
    /// or an alternate data stream.
    /// </para>
    /// <para>
    /// The check is made on the resolved path as well as on the segments, because the segments are
    /// not the only way out: a path that survived them can still land outside once the platform has
    /// resolved its own separators. What is compared is the path's text, not the file system's
    /// answer — a symbolic link inside the bundle is followed as the operating system follows it,
    /// which is right: the directory is the application's own, so a link in it is the application's
    /// statement about what it ships.
    /// </para>
    /// </remarks>
    private static bool TryResolve(string directory, string variant, string fileName, out string path)
    {
        path = string.Empty;

        if (!IsSingleSegment(variant) || !IsSingleSegment(fileName))
        {
            return false;
        }

        string root;
        try
        {
            root = Path.GetFullPath(directory);
        }
        catch (Exception error) when (error is ArgumentException or NotSupportedException or PathTooLongException)
        {
            return false;
        }

        var candidate = Path.GetFullPath(Path.Combine(root, variant, fileName));
        var prefix = root.EndsWith(Path.DirectorySeparatorChar)
            ? root
            : root + Path.DirectorySeparatorChar;

        if (!candidate.StartsWith(prefix, StringComparison.Ordinal))
        {
            return false;
        }

        path = candidate;

        return true;
    }

    /// <summary>Whether a route value can be a single path segment on every platform.</summary>
    /// <param name="value">The value the router handed over.</param>
    /// <returns><see langword="true"/> when it holds no separator of any platform.</returns>
    private static bool IsSingleSegment(string value) =>
        !value.Contains('/') && !value.Contains('\\') && !value.Contains(':');

    /// <summary>One parameter of the route, which ASP.NET Core has already unescaped.</summary>
    /// <param name="context">The request.</param>
    /// <param name="name">The parameter's name.</param>
    /// <returns>The value, or the empty string when the request did not carry one.</returns>
    private static string RouteValue(HttpContext context, string name) =>
        context.Request.RouteValues[name] as string ?? string.Empty;

    /// <summary>FastAPI's answer for a path parameter outside the values its <c>Literal</c> allows.</summary>
    /// <param name="input">What the request carried instead.</param>
    /// <returns>The document, with pydantic's fields in the order it writes them.</returns>
    /// <remarks>
    /// A client written against the Python server reads this document, so its shape is the contract
    /// rather than a message this framework chose — the same reason <see cref="Answers"/> carries
    /// the two body-field entries.
    /// </remarks>
    private static JsonObject LiteralError(string input) => new()
    {
        ["detail"] = new JsonArray(new JsonObject
        {
            ["type"] = "literal_error",
            ["loc"] = new JsonArray("path", "environment"),
            ["msg"] = $"Input should be {ExpectedEnvironment}",
            ["input"] = input,
            ["ctx"] = new JsonObject { ["expected"] = ExpectedEnvironment },
        }),
    };

    /// <summary>The Python version's refusal body, which is one sentence under
    /// <c>detail</c>.</summary> <param name="detail">The sentence.</param> <returns>The
    /// document.</returns>
    private static JsonObject Detail(string detail) => new() { ["detail"] = detail };
}

using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;

namespace Gradio.Net;
/// <summary>
/// The endpoint a share button asks for a link to: it writes the session's state under a name the
/// link can carry, and answers with that name.
/// </summary>
/// <remarks>
/// <para>
/// The name is the whole of the link: the page's own address plus <c>?deep_link=&lt;name&gt;</c>, and
/// the state that address stands for is a file the server wrote. The other half is
/// <see cref="DeepLinkState"/>, which the page and <c>/config</c> read the file back through: a
/// request that names a link is described with the components the file holds, so the page opens
/// where the link left off.
/// </para>
/// <para>
/// A session the server does not hold answers with the empty string, which is the Python version's
/// answer: the script that asked reads it as "no link" and copies the page's address alone. A
/// request with no session at all answers the same way.
/// </para>
/// <para>
/// The digest is this build's own rather than the Python version's: the Python version hashes the
/// state with blake2b and this framework has no blake2b, so the name is a truncated SHA-256 in the
/// same URL-safe alphabet. Nothing depends on the value — it is an opaque token that names a file —
/// and the length is the same, so a client that reads it sees the same shape.
/// </para>
/// </remarks>
internal sealed class GradioDeepLinkEndpoint : IGradioEndpoint
{
    private const string RoutePath = "/gradio_api/deep_link";

    /// <summary>How many bytes of the digest the name keeps, which is the Python version's
    /// eight.</summary>
    private const int TokenBytes = 8;

    /// <summary>The refusal a request without a session gets, spelled as FastAPI spells it.</summary>
    private const string MissingSession =
        """{"detail":[{"type":"missing","loc":["query","session_hash"],"msg":"Field required","input":null}]}""";

    private readonly Blocks _blocks;
    private readonly ISessionStore _sessions;

    /// <summary>Creates the capability.</summary>
    /// <param name="blocks">The application whose sessions are linked to.</param>
    /// <param name="sessions">The sessions that may be linked to.</param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    public GradioDeepLinkEndpoint(Blocks blocks, ISessionStore sessions)
    {
        ArgumentNullException.ThrowIfNull(blocks);
        ArgumentNullException.ThrowIfNull(sessions);

        _blocks = blocks;
        _sessions = sessions;
    }

    /// <inheritdoc />
    public IReadOnlyCollection<string> Routes { get; } = [new GradioRoute("GET", RoutePath).Key];

    /// <inheritdoc />
    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        // The session is a required query parameter in the Python version, so a request that names none is
        // refused before anything is looked up — with FastAPI's own answer, which a client written
        // against the Python server reads.
        if (!context.Request.Query.ContainsKey("session_hash"))
        {
            context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            context.Response.ContentType = "application/json";

            await context.Response
                .WriteAsync(MissingSession, context.RequestAborted)
                .ConfigureAwait(false);

            return;
        }

        var sessionHash = context.Request.Query["session_hash"].ToString();

        if (sessionHash.Length == 0 || !_sessions.TryGet(sessionHash, out var session))
        {
            await AnswerAsync(context, string.Empty).ConfigureAwait(false);

            return;
        }

        var state = JsonText.Serialize(ComponentsOf(session));
        var name = NameOf(state);
        var directory = Path.Combine(_blocks.UploadFolder, "deep_links", name);

        Directory.CreateDirectory(directory);
        await System.IO.File.WriteAllTextAsync(
            Path.Combine(directory, "state.json"),
            state,
            context.RequestAborted).ConfigureAwait(false);

        await AnswerAsync(context, name).ConfigureAwait(false);
    }

    /// <summary>The session's components, as the config describes them.</summary>
    /// <param name="session">The session being linked to.</param>
    /// <returns>The components, in the order the application rendered them.</returns>
    /// <remarks>
    /// A stateful component's value is the session's rather than the application's, which is why it
    /// is written in here: a link to a session that has filled a state is a link to the filled one.
    /// </remarks>
    private JsonArray ComponentsOf(Session session)
    {
        var components = new JsonArray();

        foreach (var block in _blocks.Registered)
        {
            var entry = block.ToConfigEntry();

            if (entry["props"] is not JsonObject props)
            {
                components.Add(entry);
                continue;
            }

            // A state's value is the session's. Everything else belongs to the browser, except that
            // the server remembers the last value it saw each component carry — the input of a run
            // and the output of one — which is what a link restores.
            if (block is IStatefulComponent stateful && session.State.TryGet(block.Id, out var value))
            {
                props["value"] = value?.DeepClone();
            }
            else if (session.State.TryRecall(block.Id, out var shown))
            {
                props["value"] = shown?.DeepClone();
            }

            components.Add(entry);
        }

        return components;
    }

    /// <summary>The name a state is filed under.</summary>
    /// <param name="state">The state's JSON.</param>
    /// <returns>A URL-safe name.</returns>
    private static string NameOf(string state)
    {
        var digest = SHA256.HashData(Encoding.UTF8.GetBytes(state))[..TokenBytes];

        return Convert.ToBase64String(digest)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }

    /// <summary>Answers with a string, the way the Python version's JSON response carries
    /// one.</summary> <param name="context">The request and the response to write.</param>
    /// <param name="value">The string to answer with.</param>
    private static async Task AnswerAsync(HttpContext context, string value)
    {
        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "application/json";

        await context.Response
            .WriteAsync(JsonText.Serialize(JsonValue.Create(value)!), context.RequestAborted)
            .ConfigureAwait(false);
    }
}

using System.Text;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;

namespace Gradio.Net;
/// <summary>
/// Who is calling, and signing in and out: the six routes that answer those questions.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>/login</c>, <c>/logout</c>, <c>/gradio_api/user</c>,
/// <c>/gradio_api/login_check</c>, <c>/gradio_api/token</c> and <c>/gradio_api/app_id</c>. They are
/// one capability because they share one answer — the caller's identity — and because the frontend
/// calls them together: the page asks who it is looking at, the form posts a password, and the
/// button that has become a sign-out link goes to the route that ends the session.
/// </para>
/// <para>
/// Two of them answer with a bare JSON value rather than a document, which is the Python version's
/// shape: <c>/user</c> answers the name or nothing, and <c>/login_check</c> answers nothing at all
/// when the caller may look. A client that parses either of them expects exactly that.
/// </para>
/// <para>
/// An application that signs users in with an identity provider gets its <c>/logout</c> from
/// <see cref="GradioOAuthEndpoints"/> instead, which is the Python version's arrangement: a
/// signed-in session lives in a different cookie, so signing out means something else.
/// </para>
/// </remarks>
internal sealed class GradioAuthEndpoints : IGradioEndpoint
{
    private const string LoginPath = "/login";
    private const string LogoutPath = "/logout";
    private const string UserPath = "/gradio_api/user";
    private const string LoginCheckPath = "/gradio_api/login_check";
    private const string TokenPath = "/gradio_api/token";
    private const string AppIdPath = "/gradio_api/app_id";

    private readonly Blocks _blocks;
    private readonly GradioAuthentication _authentication;
    private readonly GradioLoginGate _gate;
    private readonly GradioOAuthSession _oauth;

    /// <summary>Creates the capability.</summary>
    /// <param name="blocks">The application whose users are signed in.</param>
    /// <param name="authentication">Where sessions are kept.</param>
    /// <param name="gate">The refusal an unauthenticated caller gets.</param>
    /// <param name="oauth">The session an application that signs users in with an identity provider keeps.</param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    public GradioAuthEndpoints(
        Blocks blocks,
        GradioAuthentication authentication,
        GradioLoginGate gate,
        GradioOAuthSession oauth)
    {
        ArgumentNullException.ThrowIfNull(blocks);
        ArgumentNullException.ThrowIfNull(authentication);
        ArgumentNullException.ThrowIfNull(gate);
        ArgumentNullException.ThrowIfNull(oauth);

        _blocks = blocks;
        _authentication = authentication;
        _gate = gate;
        _oauth = oauth;
    }

    /// <inheritdoc />
    public IReadOnlyCollection<string> Routes { get; } =
    [
        new GradioRoute("POST", LoginPath).Key,
        new GradioRoute("GET", LogoutPath).Key,
        new GradioRoute("GET", UserPath).Key,
        new GradioRoute("GET", LoginCheckPath).Key,
        new GradioRoute("GET", TokenPath).Key,
        new GradioRoute("GET", AppIdPath).Key,
    ];

    /// <inheritdoc />
    public Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var route = GradioRouteMetadata.PathOf(context);

        return route switch
        {
            LoginPath => LoginAsync(context),
            LogoutPath => LogoutAsync(context),
            UserPath => UserAsync(context),
            LoginCheckPath => LoginCheckAsync(context),
            TokenPath => TokenAsync(context),
            AppIdPath => AppIdAsync(context),
            _ => Answers.NotFoundAsync(context),
        };
    }

    /// <summary>Signs someone in, or says the credentials are wrong.</summary>
    /// <param name="context">The request being answered.</param>
    /// <returns>A task that completes once the answer is written.</returns>
    /// <remarks>
    /// <para>
    /// The form is read before anything else, because the Python version reads it before anything
    /// else: a request without one is refused as malformed even by an application that asks nobody
    /// to sign in, which is what a client written against FastAPI expects.
    /// </para>
    /// <para>
    /// An application that asks nobody to sign in answers the sign-in form by sending the browser
    /// back to the page, which is the Python version's answer and what makes the form harmless to
    /// post twice.
    /// </para>
    /// </remarks>
    private async Task LoginAsync(HttpContext context)
    {
        var form = context.Request.HasFormContentType
            ? await context.Request.ReadFormAsync(context.RequestAborted).ConfigureAwait(false)
            : null;

        // A field that was not sent is a malformed request; a field sent empty is a credential that
        // is wrong, which is the answer below. FastAPI draws the same line, because its form model
        // asks whether the field is there rather than whether it says anything.
        var sentUserName = form is not null && form.ContainsKey("username");
        var sentPassword = form is not null && form.ContainsKey("password");

        if (!sentUserName || !sentPassword)
        {
            await Answers
                .JsonAsync(
                    context,
                    StatusCodes.Status422UnprocessableEntity,
                    MissingCredentials(sentUserName, sentPassword))
                .ConfigureAwait(false);

            return;
        }

        var userName = form!["username"].ToString().Trim();
        var password = form["password"].ToString();

        if (_blocks.Auth is not { } auth)
        {
            Answers.Redirect(context, RootUrl.Of(context));

            return;
        }

        if (!await auth.VerifyAsync(userName, password, context.RequestAborted).ConfigureAwait(false))
        {
            await Answers
                .JsonAsync(
                    context,
                    StatusCodes.Status400BadRequest,
                    new JsonObject { ["detail"] = "Incorrect credentials." })
                .ConfigureAwait(false);

            return;
        }

        _authentication.SignIn(context, userName);

        await Answers.JsonAsync(context, StatusCodes.Status200OK, new JsonObject { ["success"] = true })
            .ConfigureAwait(false);
    }

    /// <summary>Signs the caller out and sends the browser back to the page.</summary>
    /// <param name="context">The request being answered.</param>
    /// <returns>A task that completes once the answer is written.</returns>
    /// <remarks>
    /// An application that signs users in with an identity provider keeps its session in that
    /// provider's cookie, so signing out means dropping the user from it rather than deleting a
    /// token: only the signed-in user is removed, and the rest of the session survives, which is
    /// the Python version's <c>pop("oauth_info")</c>.
    /// </remarks>
    private Task LogoutAsync(HttpContext context)
    {
        if (_blocks.ExpectsOAuth)
        {
            var session = _oauth.Read(context);

            GradioOAuthSession.Forget(session);
            _oauth.Write(context, session);

            Answers.Redirect(context, OAuthRedirect.TargetOf(context));

            return Task.CompletedTask;
        }

        _authentication.SignOut(context, AllSessions(context));
        Answers.Redirect(context, RootUrl.Of(context));

        return Task.CompletedTask;
    }

    /// <summary>Answers who is calling.</summary>
    /// <param name="context">The request being answered.</param>
    /// <returns>A task that completes once the answer is written.</returns>
    private async Task UserAsync(HttpContext context)
    {
        var userName = await _authentication.UserNameOfAsync(context).ConfigureAwait(false);

        await Answers
            .JsonAsync(
                context,
                StatusCodes.Status200OK,
                userName is null ? "null" : JsonText.Serialize(JsonValue.Create(userName)))
            .ConfigureAwait(false);
    }

    /// <summary>Answers whether the caller may look at the application.</summary>
    /// <param name="context">The request being answered.</param>
    /// <returns>A task that completes once the answer is written.</returns>
    /// <remarks>
    /// Nothing at all is the answer when the caller may look — <c>null</c>, which is the Python
    /// version's <c>None</c> — so a client distinguishes the two by the status rather than by the
    /// body.
    /// </remarks>
    private async Task LoginCheckAsync(HttpContext context)
    {
        if (await _gate.AllowsAsync(context).ConfigureAwait(false))
        {
            await Answers.JsonAsync(context, StatusCodes.Status200OK, "null").ConfigureAwait(false);

            return;
        }

        await _gate.RefuseAsync(context).ConfigureAwait(false);
    }

    /// <summary>Answers the token the caller's browser carries.</summary>
    /// <param name="context">The request being answered.</param>
    /// <returns>A task that completes once the answer is written.</returns>
    /// <remarks>
    /// The token is answered even when it names no sign-in, which is the Python version's shape: a
    /// client that holds a stale cookie is told which token it holds and that nobody is signed in
    /// with it. Only the marked cookie is read here, where <c>/user</c> falls back to the plain one
    /// — the Python version is asymmetric in exactly this way, and a client that reads both routes
    /// is reading both answers.
    /// </remarks>
    private async Task TokenAsync(HttpContext context)
    {
        var token = _authentication.SecureTokenOf(context);

        await Answers
            .JsonAsync(
                context,
                StatusCodes.Status200OK,
                new JsonObject
                {
                    ["token"] = token,
                    ["user"] = _authentication.NameOfToken(token),
                })
            .ConfigureAwait(false);
    }

    /// <summary>Answers the identifier the frontend reports the application under.</summary>
    /// <param name="context">The request being answered.</param>
    /// <returns>A task that completes once the answer is written.</returns>
    private Task AppIdAsync(HttpContext context) =>
        Answers.JsonAsync(context, StatusCodes.Status200OK, new JsonObject { ["app_id"] = _blocks.AppId });

    /// <summary>Whether signing out ends every session the user has.</summary>
    /// <param name="context">The request.</param>
    /// <returns>
    /// <see langword="true"/> unless the request says otherwise — the Python version's default, and
    /// the one that makes signing out mean something on a shared machine.
    /// </returns>
    private static bool AllSessions(HttpContext context) =>
        !bool.TryParse(context.Request.Query["all_session"], out var asked) || asked;

    /// <summary>The Python version's refusal for a sign-in form that does not carry a credential at
    /// all.</summary> <param name="sentUserName">Whether the form carried a username field.</param>
    /// <param name="sentPassword">Whether the form carried a password field.</param> <returns>The
    /// document FastAPI answers with.</returns>
    private static JsonObject MissingCredentials(bool sentUserName, bool sentPassword)
    {
        var detail = new JsonArray();

        if (!sentUserName)
        {
            detail.Add(Answers.MissingBodyField("username"));
        }

        if (!sentPassword)
        {
            detail.Add(Answers.MissingBodyField("password"));
        }

        return new JsonObject { ["detail"] = detail };
    }
}

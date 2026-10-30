using Microsoft.AspNetCore.Http;

namespace Gradio.Net;
/// <summary>
/// Answers who is calling, on behalf of a host that authenticates its own requests.
/// </summary>
/// <param name="context">The request being answered.</param>
/// <param name="cancellationToken">Cancels the answer.</param>
/// <returns>
/// The caller's name, or <see langword="null"/> when the request is not authenticated — which is a
/// refusal, not an error.
/// </returns>
/// <remarks>
/// <para>
/// The Python version's <c>auth_dependency</c>, which takes the request and returns a username or
/// nothing. The request is the input because that is where a host's own authentication has already
/// put the answer: a cookie, a bearer token, a header its middleware understood — all of it has
/// been read by the time this is called.
/// </para>
/// <para>
/// It is a delegate rather than an interface so that hooking an existing scheme is one line in the
/// host's startup, and it can be asynchronous because asking whether a token is still valid may mean
/// asking somewhere else.
/// </para>
/// </remarks>
public delegate Task<string?> GradioAuthDependency(HttpContext context, CancellationToken cancellationToken);

/// <summary>
/// How an application is protected, and what a browser is told when it is not.
/// </summary>
/// <remarks>
/// <para>
/// These are the Python version's three launch parameters that decide who may open the application
/// — <c>auth</c>, <c>auth_message</c> and <c>auth_dependency</c> — kept on the application for the
/// same reason the theme and the translations are: both hosting entry points configure the
/// application rather than the host, so the two cannot disagree about who may look at it.
/// </para>
/// <para>
/// <c>auth</c> and <c>auth_dependency</c> are two answers to the same question and are mutually
/// exclusive, which is the Python version's rule: one admits a username and password this
/// application holds, the other hands the question to the host's own authentication. Setting both
/// is refused when the application is first described, rather than quietly preferring one.
/// </para>
/// </remarks>
public sealed partial class Blocks
{
    /// <summary>Why one of the two rules has to be unset before the other is.</summary>
    private const string OneRuleOnly =
        "An application is protected by `Auth` or by `AuthDependency`, not both: one checks a password "
        + "this application holds, the other asks the host's own authentication (the Python version refuses the "
        + "pair at launch for the same reason).";

    private GradioAuth? _auth;
    private GradioAuthDependency? _authDependency;

    /// <summary>
    /// The accounts, or the rule, that admit someone to this application, or <see langword="null"/>
    /// for an application anyone may open.
    /// </summary>
    /// <value><see langword="null"/>, which is the Python version's default.</value>
    /// <remarks>
    /// A protected application still serves its page: the browser is given a page that says it must
    /// sign in, and every route that would describe or run the application answers 401 until it
    /// does. That is the Python version's behaviour, and it is what lets the sign-in form be part
    /// of the application rather than a page this framework invents.
    /// </remarks>
    /// <exception cref="InvalidOperationException"><see cref="AuthDependency"/> is already set.</exception>
    public GradioAuth? Auth
    {
        get => _auth;
        set
        {
            _auth = value;

            if (value is not null && _authDependency is not null)
            {
                throw new InvalidOperationException(OneRuleOnly);
            }
        }
    }

    /// <summary>
    /// Who is calling, when the host authenticates its own requests, or <see langword="null"/> for
    /// an application that authenticates nobody.
    /// </summary>
    /// <value><see langword="null"/>, which is the Python version's default.</value>
    /// <remarks>
    /// <para>
    /// A request this answers nothing for is refused with 401 everywhere, including the page: the
    /// application does not know who is asking, and unlike <see cref="Auth"/> there is no form it
    /// could offer, because signing in is the host's business.
    /// </para>
    /// <para>
    /// It is separate from <see cref="Auth"/> rather than a fallback for it, and setting both is
    /// refused: an application that has its own accounts and also defers to the host would answer
    /// two different questions with one request.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException"><see cref="Auth"/> is already set.</exception>
    public GradioAuthDependency? AuthDependency
    {
        get => _authDependency;
        set
        {
            if (value is not null && _auth is not null)
            {
                throw new InvalidOperationException(OneRuleOnly);
            }

            _authDependency = value;
        }
    }

    /// <summary>
    /// What the sign-in page says to someone who has not signed in yet.
    /// </summary>
    /// <value><see langword="null"/>, which is the Python version's default and leaves the page's
    /// own text.</value> <remarks>The Python version renders it as HTML, so an application can put
    /// a link or a logo in it.</remarks>
    public string? AuthMessage { get; set; }

    /// <summary>Whether anyone must sign in before the application answers.</summary>
    /// <value>
    /// <see langword="true"/> when <see cref="Auth"/> or <see cref="AuthDependency"/> is set.
    /// </value>
    public bool RequiresLogin => _auth is not null || _authDependency is not null;

    /// <summary>Whether the application signs users in with an identity provider.</summary>
    /// <value>
    /// <see langword="true"/> when a block that signs users in — a <c>LoginButton</c> — was rendered.
    /// </value>
    /// <remarks>
    /// The Python version's <c>expects_oauth</c>, and it decides which sign-in routes the host
    /// serves: an application with such a button is sent to its identity provider, and one without
    /// it is sent to the username-and-password form. Both are routes of the same table; this is
    /// what tells them apart.
    /// </remarks>
    public bool ExpectsOAuth => _blocks.Any(block => block is IOAuthLoginBlock);
}

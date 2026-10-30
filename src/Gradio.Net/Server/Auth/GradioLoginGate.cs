using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;

namespace Gradio.Net;
/// <summary>
/// The check a guarded route answers through, and the refusal an unauthenticated caller gets.
/// </summary>
/// <remarks>
/// <para>
/// The Python version attaches <c>login_check</c> as a dependency to every route that would
/// describe or run the application, and to the page's assets it attaches nothing: the page itself
/// is served, and it draws the sign-in form instead of the application. Which routes those are is
/// <see cref="GradioRoute.RequiresLogin"/>'s business — the route table declares it, this asks it —
/// so that there is one list rather than a list beside a list.
/// </para>
/// <para>
/// What is not guarded is as deliberate as what is: the page, its assets, the theme, the heartbeat,
/// the cancel route, the streams and the run-history page are all reachable without signing in,
/// because the Python version does not guard them. The page is where the sign-in form lives, and a
/// heartbeat from a page that has not signed in yet is still a live browser.
/// </para>
/// <para>
/// A route whose capability has not landed yet is guarded too, and refuses before it answers the
/// not-implemented placeholder: the Python version's dependency runs before its handler, so a
/// signed-out visitor learns that they are not signed in rather than that the route is unbuilt.
/// </para>
/// </remarks>
internal sealed class GradioLoginGate
{
    private readonly Blocks _blocks;
    private readonly GradioAuthentication _authentication;

    /// <summary>Creates the gate.</summary>
    /// <param name="blocks">The application whose routes are guarded.</param>
    /// <param name="authentication">Where the caller's identity comes from.</param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    public GradioLoginGate(Blocks blocks, GradioAuthentication authentication)
    {
        ArgumentNullException.ThrowIfNull(blocks);
        ArgumentNullException.ThrowIfNull(authentication);

        _blocks = blocks;
        _authentication = authentication;
    }

    /// <summary>Whether the request may reach a guarded route.</summary>
    /// <param name="context">The request.</param>
    /// <returns><see langword="true"/> when the application asks nobody to sign in, or the caller did.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
    internal async Task<bool> AllowsAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        return !_blocks.RequiresLogin || await _authentication.UserNameOfAsync(context).ConfigureAwait(false) is not null;
    }

    /// <summary>Answers a request that may not reach the route.</summary>
    /// <param name="context">The request being answered.</param>
    /// <returns>A task that completes once the refusal is written.</returns>
    /// <remarks>
    /// The Python version's 401, body and all: a client written against the Python server reads
    /// <c>detail.auth_message</c> to know what the application wants to say about signing in.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
    internal Task RefuseAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        return Answers.JsonAsync(
            context,
            StatusCodes.Status401Unauthorized,
            new JsonObject
            {
                ["detail"] = new JsonObject
                {
                    ["error"] = "Not authenticated",
                    ["auth_message"] = _blocks.AuthMessage,
                },
            });
    }
}

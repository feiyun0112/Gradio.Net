namespace Gradio.Net;
/// <summary>
/// A block that signs the user in with the application's identity provider, and out again.
/// </summary>
/// <remarks>
/// <para>
/// The Python version asks <c>isinstance(block, components.LoginButton)</c> to decide whether an
/// application expects OAuth at all: a page with such a button is served the sign-in routes, and
/// one without it is served the username-and-password routes. The question is the same here, asked
/// through an interface rather than a type, because the block lives in a layer this one cannot see
/// .
/// </para>
/// <para>
/// It is also where the label a signed-in user sees comes from: the Python version relabels the
/// button from a load event, and this framework reads the label while it describes the page,
/// because a handler cannot see the request that carries the session. What the user sees is the
/// same either way.
/// </para>
/// </remarks>
public interface IOAuthLoginBlock
{
    /// <summary>The label to draw for a user who is signed in.</summary>
    /// <param name="userName">The name the identity provider gave for them.</param>
    /// <returns>The label, which names them and offers to sign them out.</returns>
    string SignedInLabel(string userName);
}

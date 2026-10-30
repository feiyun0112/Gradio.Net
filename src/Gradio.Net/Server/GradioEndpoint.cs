namespace Gradio.Net;
/// <summary>
/// One endpoint as a running host registers it.
/// </summary>
/// <param name="Method">The HTTP method.</param>
/// <param name="Path">
/// The Python path template, so that the endpoint can be compared with the Python route table.
/// </param>
/// <param name="RegisteredPattern">
/// The ASP.NET Core route pattern the host registered, kept for diagnosis: it is what a reader has
/// to see to understand why <see cref="Path"/> and the pattern are not spelled the same way.
/// </param>
/// <param name="RequiresLogin">
/// Whether the host put the endpoint behind the sign-in gate, as the mark the mapping loop stamps
/// on it says. Reported so that the set comparison can check the guards as well as the routes: a
/// route that is registered and guarded differently from the Python version's is a request that is
/// refused or served on the wrong side of a sign-in, which a comparison of methods and paths cannot
/// see.
/// </param>
public readonly record struct GradioEndpoint(
    string Method,
    string Path,
    string RegisteredPattern,
    bool RequiresLogin);

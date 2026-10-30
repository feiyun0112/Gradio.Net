namespace Gradio.Net;
/// <summary>
/// One endpoint of the Python route table, in the Python version's own template syntax.
/// </summary>
/// <remarks>
/// <para>
/// The route table is the authority for what this framework exposes, and it is expressed in the
/// Python version's syntax rather than ASP.NET Core's so that it can be compared, entry for entry,
/// against the table extracted from the Python release. Where the two template languages cannot
/// express the same thing, <see cref="RegisteredPattern"/> records the form this framework actually
/// registers and <see cref="Path"/> keeps the Python form.
/// </para>
/// </remarks>
public sealed record GradioRoute
{
    /// <summary>Declares a route from its Python method and path template.</summary>
    /// <param name="method">The HTTP method, upper case, as the Python version spells it.</param>
    /// <param name="path">The Python path template, e.g.
    /// <c>/gradio_api/proxy={url_path:path}</c>.</param>
    /// <exception cref="ArgumentNullException">Either argument is
    /// <see langword="null"/>.</exception>
    public GradioRoute(string method, string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(method);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        Method = method.ToUpperInvariant();
        Path = path;
        RegisteredPattern = RouteTemplate.ToAspNetCore(path);
    }

    /// <summary>The HTTP method.</summary>
    public string Method { get; }

    /// <summary>The path template as the Python version declares it, including its converter
    /// syntax.</summary>
    public string Path { get; }

    /// <summary>The ASP.NET Core route pattern this route is registered at.</summary>
    /// <remarks>
    /// For most routes this is <see cref="Path"/> with the converters dropped. It differs where the
    /// Python version puts a catch-all converter inside a segment that also has a literal prefix
    /// (<c>/gradio_api/proxy={url_path:path}</c>): ASP.NET Core cannot match a catch-all that does
    /// not make up a whole segment, so the literal prefix becomes a route constraint instead, and
    /// the endpoint still matches exactly the URLs the Python version matches and nothing else.
    /// </remarks>
    public string RegisteredPattern { get; }

    /// <summary>Whether the route answers only to someone who has signed in.</summary>
    /// <value>
    /// <see langword="false"/> for most routes: the page, its assets, the theme and the sign-in
    /// routes themselves are reachable without signing in.
    /// </value>
    /// <remarks>
    /// <para>
    /// The Python version states this per route, as a <c>Depends(login_check)</c> on the ones that
    /// describe or run the application. It is a property of the route here for the same reason the
    /// method and the path are: it is part of what the Python version declares, and a second list
    /// kept beside the table would be a second thing to keep in step — one whose drift would let an
    /// unauthenticated visitor call something the application meant to protect.
    /// </para>
    /// <para>
    /// Whether the application asks anyone to sign in at all is a separate question, asked of the
    /// application: an application with no <c>Auth</c> and no <c>AuthDependency</c> lets every
    /// request through, guarded route or not.
    /// </para>
    /// </remarks>
    public bool RequiresLogin { get; init; }

    /// <summary>The comparison key: method plus path with the parameter syntax normalised.</summary>
    public string Key => $"{Method} {RouteTemplate.Normalize(Path)}";
}

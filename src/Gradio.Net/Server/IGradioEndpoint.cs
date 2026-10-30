namespace Gradio.Net;
/// <summary>
/// A capability's answer to one or more routes of the table.
/// </summary>
/// <remarks>
/// <para>
/// The route table stays the authority for what this framework exposes: a capability
/// does not register anything itself, it says which table entries it answers and the mapping loop
/// hands those to it. Everything else keeps answering the not-implemented placeholder, so "built"
/// and "not built yet" stay distinguishable.
/// </para>
/// <para>
/// A capability that claims a route the table does not have is a mistake rather than an extra
/// route, and the mapping loop says so.
/// </para>
/// </remarks>
internal interface IGradioEndpoint
{
    /// <summary>The table entries this capability answers, by their comparison key.</summary>
    /// <value>
    /// Keys as <see cref="GradioRoute.Key"/> spells them, e.g. <c>GET /config</c>.
    /// </value>
    IReadOnlyCollection<string> Routes { get; }

    /// <summary>Answers a request.</summary>
    /// <param name="context">The request and the response to write.</param>
    /// <returns>A task that completes when the response has been written.</returns>
    Task InvokeAsync(HttpContext context);
}

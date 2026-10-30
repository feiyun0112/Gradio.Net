using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// The navigation bar of a multipage application: the pages it offers, and the name the
/// application's home page is shown under.
/// </summary>
/// <remarks>
/// <para>
/// The parameters are the Python version's; the props are generated from them in the Python
/// version's order. The navbar is a component rather than a layout because it carries a value — the
/// extra links it adds — and because an application may bind its <c>change</c> event. It draws
/// nothing of the page's own content; it describes the page's navigation, and it may be written
/// anywhere on the page.
/// </para>
/// <para>
/// <see cref="Value"/> adds links beside the ones the application's own pages already give the
/// navbar. A link whose path is a relative one names another page of the application; an absolute
/// URL is an external link, and the frontend leaves the application to open it.
/// </para>
/// <para>
/// Only one navbar may exist per page — two would be two answers to what the page's navigation is —
/// which the render refuses. That rule is why <see cref="SinglePerPage"/> is turned on.
/// </para>
/// </remarks>
[GradioBlock("navbar")]
public sealed partial class Navbar : Component
{
    internal Navbar()
    {
    }

    /// <summary>The links this navbar adds to the pages the application already has.</summary>
    /// <remarks>
    /// <see langword="null"/> — no links of its own, so the navbar offers the application's pages
    /// and nothing else — which is the Python version's default and what it writes as an absent
    /// prop.
    /// </remarks>
    public IReadOnlyList<NavbarLink>? Value { get; set; }

    /// <summary>
    /// The name the home page is shown under, or <see langword="false"/> to leave the home page out
    /// of the navbar.
    /// </summary>
    /// <remarks>
    /// A string or a boolean, because that is what the Python version accepts: <c>"Home"</c> by
    /// default, and <c>false</c> for an application whose home page is not something to navigate
    /// to.
    /// </remarks>
    public object? MainPageName { get; set; } = "Home";

    /// <inheritdoc />
    public override JsonObject ApiInfo => new();

    /// <inheritdoc />
    protected internal override bool SinglePerPage => true;
}

/// <summary>
/// One link a navbar adds: the name it is shown under, and the page or address it leads to.
/// </summary>
/// <param name="Name">The name the navbar shows.</param>
/// <param name="Path">A page of this application, or an absolute URL.</param>
/// <remarks>
/// The Python version's value is a list of <c>(name, path)</c> pairs, which the config carries as
/// an array of two-element arrays; this type is that pair, and writing it is the only thing it
/// does.
/// </remarks>
public sealed record NavbarLink(string Name, string Path) : IConfigValue
{
    /// <inheritdoc />
    public JsonNode ToConfigValue() => new JsonArray(Name, Path);
}

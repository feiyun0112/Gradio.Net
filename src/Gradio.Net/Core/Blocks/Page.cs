using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// One page of an application: the URL suffix it is served at, the name it is shown under, and
/// whether the navbar offers it.
/// </summary>
/// <param name="Path">The URL suffix, without a leading slash; the home page's is empty.</param>
/// <param name="Name">The name the navbar shows.</param>
/// <param name="ShowInNavbar">
/// Whether the page appears in the navbar. A page that is not shown is still served at its URL.
/// </param>
/// <remarks>
/// <para>
/// The Python version keeps the same three fields in a tuple (<c>Blocks.pages</c>), which is what
/// the config's <c>pages</c> array carries — one <c>[path, name, show_in_navbar]</c> triple per
/// page. A type rather than a tuple because the tuple is the wire form, and a second implementation
/// that passed tuples around would be describing its own JSON rather than the application.
/// </para>
/// <para>
/// The home page is always first and always named by the empty string, which is the Python
/// version's spelling of "the application's root".
/// </para>
/// </remarks>
public sealed record Page(string Path, string Name, bool ShowInNavbar)
{
    /// <summary>The home page: the empty path, shown as <c>Home</c>.</summary>
    public static Page Home { get; } = new(string.Empty, "Home", ShowInNavbar: true);

    /// <summary>Whether this is the application's home page.</summary>
    public bool IsHome => Path.Length == 0;

    /// <summary>This page as the config's <c>pages</c> array spells it.</summary>
    /// <returns>The <c>[path, name, show_in_navbar]</c> triple.</returns>
    internal JsonArray ToConfigEntry() => new(Path, Name, ShowInNavbar);
}

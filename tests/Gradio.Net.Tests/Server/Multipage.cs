using Gradio.Net;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The multipage application the page tests are about.
/// </summary>
/// <remarks>
/// <para>
/// Three pages: the home page, one the navbar offers, and one that is served at its URL and left
/// out of the navbar. The pages are written in the same order and with the same blocks as the
/// case's the Python version demo, because the served document is compared field by field: the
/// order is what makes a block's id, and so the layout tree and the page map, line up between the
/// two.
/// </para>
/// <para>
/// Each of the first two pages binds an event, and each binding is set up while its page is the
/// current one. That is what puts it on that page in the config — and what makes the page's own
/// API info leave the other page's endpoint out.
/// </para>
/// </remarks>
internal static class Multipage
{
    /// <summary>The title the application and the OpenAPI document carry.</summary>
    internal const string Title = "Multipage app";

    /// <summary>Builds the application into a render root.</summary>
    /// <param name="blocks">The application to render into.</param>
    internal static void Build(Blocks blocks)
    {
        blocks.Title = Title;
        blocks.AnalyticsEnabled = true;

        Textbox name;
        Textbox greeting;
        Textbox about;

        using (blocks.BeginRender())
        {
            // The home page's navbar names the home page "Start" and adds a link of its own: a page
            // that is not one of the application's, reached by URL rather than by a route.
            blocks.Add(new Navbar
            {
                Value = [new NavbarLink("Docs", "https://example.com/docs")],
                MainPageName = "Start",
            });

            name = blocks.Add(new Textbox { Label = "Name" });
            greeting = blocks.Add(new Textbox { Label = "Greeting" });

            name.Change(Shout, [name], [greeting], apiName: "shout", parameterNames: ["value"]);

            using (blocks.Route("About"))
            {
                // The second page configures the navbar differently: the home page is not offered,
                // and the navbar is not drawn at all.
                blocks.Add(new Navbar { MainPageName = false, Visible = false });

                var row = blocks.Add(new Row());

                using (blocks.Enter(row))
                {
                    var column = blocks.Add(new Column { Scale = 2, MinWidth = 200 });

                    using (blocks.Enter(column))
                    {
                        blocks.Add(new Textbox { Label = "About the row" });
                    }
                }

                about = blocks.Add(new Textbox { Label = "About" });

                about.Change(Copy, [about], [about], apiName: "about_changed", parameterNames: ["value"]);
            }

            // Served at its URL, left out of the navbar.
            using (blocks.Route("Internal", showInNavbar: false))
            {
                blocks.Add(new Textbox { Label = "Internal" });
            }
        }
    }

    private static object? Shout(string value) => value.ToUpperInvariant();

    private static object? Copy(string value) => value;
}

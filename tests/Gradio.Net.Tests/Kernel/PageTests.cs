using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Kernel;

/// <summary>
/// Pages as the application and the config see them: what belongs to which page, and what a page
/// is served.
/// </summary>
/// <remarks>
/// The config is one document for the application and one for each page, and the two have to
/// agree: the page's copy is the application's with everything that is not about that page taken
/// out. These tests hold the bucketing and the narrowing, which is what the frontend navigates on.
/// </remarks>
public sealed class PageTests
{
    [Fact]
    public void A_single_page_application_declares_the_home_page()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        var page = Assert.Single(blocks.Pages);

        Assert.True(page.IsHome);
        Assert.Equal("Home", page.Name);
        Assert.True(page.ShowInNavbar);
    }

    [Fact]
    public void An_application_carries_its_title_and_its_fill_settings_into_the_page()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        blocks.Title = "A page of its own";
        blocks.FillHeight = true;
        blocks.FillWidth = true;

        var config = blocks.GetConfig();

        // The three are the page's rather than a block's: the template writes the title into the
        // document, and the frontend sizes the page from the two flags. Both read them from the
        // config, which is why they are fields of it rather than props of a component.
        Assert.Equal("A page of its own", config["title"]!.GetValue<string>());
        Assert.True(config["fill_height"]!.GetValue<bool>());
        Assert.True(config["fill_width"]!.GetValue<bool>());

        // And an application that says nothing is served the defaults rather than the last one's
        // settings: the page is described per application, not per process.
        using var other = provider.CreateBlocks();

        Assert.Equal("Gradio", other.GetConfig()["title"]!.GetValue<string>());
        Assert.False(other.GetConfig()["fill_height"]!.GetValue<bool>());
    }

    [Fact]
    public void A_page_holds_the_blocks_written_on_it_and_the_config_buckets_them()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        Textbox home;
        Textbox about;
        Textbox contact;

        using (blocks.BeginRender())
        {
            home = blocks.Add(new Textbox { Label = "Home" });

            using (blocks.Route("About"))
            {
                about = blocks.Add(new Textbox { Label = "About" });
            }

            using (blocks.Route("Contact", "/contact"))
            {
                contact = blocks.Add(new Textbox { Label = "Contact" });
            }
        }

        Assert.Equal(["", "about", "contact"], blocks.Pages.Select(page => page.Path));
        Assert.Equal("About", blocks.Pages[1].Name);

        var config = blocks.GetConfig();
        var pages = config["page"]!.AsObject();

        // A page's component list holds the containers the grouping synthesised on it as well as
        // the components themselves: the Python version lists every block that belongs to the page.
        var homeIds = Ids(pages[""]!["components"]!);
        var aboutIds = Ids(pages["about"]!["components"]!);
        var contactIds = Ids(pages["contact"]!["components"]!);

        Assert.Contains(home.Id, homeIds);
        Assert.DoesNotContain(about.Id, homeIds);
        Assert.Contains(about.Id, aboutIds);
        Assert.DoesNotContain(contact.Id, aboutIds);
        Assert.Contains(contact.Id, contactIds);

        // The pages array is what the navbar reads: one [path, name, shown] triple per page.
        Assert.Equal(
            "[[\"\",\"Home\",true],[\"about\",\"About\",true],[\"contact\",\"Contact\",true]]",
            config["pages"]!.ToJsonString());

        // The whole application's layout still holds every page's tree, which is what /config is.
        Assert.Equal(3, config["layout"]!["children"]!.AsArray().Count);
    }

    [Fact]
    public void A_page_that_is_not_shown_is_still_a_page()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        using (blocks.BeginRender())
        {
            using (blocks.Route("Internal", showInNavbar: false))
            {
                blocks.Add(new Textbox { Label = "Internal" });
            }
        }

        Assert.False(blocks.Pages[1].ShowInNavbar);
        Assert.Contains("internal", blocks.GetConfig()["page"]!.AsObject().Select(pair => pair.Key));
    }

    [Fact]
    public void A_binding_belongs_to_the_page_it_was_bound_on()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        Textbox home;
        Textbox about;

        // Each binding is set up while its page is the current one, which is what says which page
        // it belongs to: a binding made after the last page would belong to that page, as the Python version's
        // does.
        using (blocks.BeginRender())
        {
            home = blocks.Add(new Textbox { Label = "Home" });
            home.Change(value => value, [home], [home], apiName: "home_changed");

            using (blocks.Route("About"))
            {
                about = blocks.Add(new Textbox { Label = "About" });
                about.Change(value => value, [about], [about], apiName: "about_changed");
            }
        }

        var pages = blocks.GetConfig()["page"]!.AsObject();

        Assert.Equal([0], Ids(pages[""]!["dependencies"]!));
        Assert.Equal([1], Ids(pages["about"]!["dependencies"]!));

        // The API info is the same question asked of the same bindings, with the page as a filter
        // rather than as a scope: asked without one, it describes the whole application, and asked
        // for a page, it describes what can be called from that page.
        Assert.Equal(
            ["/home_changed", "/about_changed"],
            EndpointNames(blocks.GetApiInfo("http://127.0.0.1")));
        Assert.Equal(["/home_changed"], EndpointNames(blocks.GetApiInfo("http://127.0.0.1", string.Empty)));
        Assert.Equal(["/about_changed"], EndpointNames(blocks.GetApiInfo("http://127.0.0.1", "about")));
    }

    [Fact]
    public void A_page_config_carries_that_page_and_says_which_page_it_is()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        Textbox home;
        Textbox about;

        using (blocks.BeginRender())
        {
            var row = blocks.Add(new Row());

            using (blocks.Enter(row))
            {
                home = blocks.Add(new Textbox { Label = "Home" });
            }

            using (blocks.Route("About"))
            {
                about = blocks.Add(new Textbox { Label = "About" });
            }
        }

        var page = blocks.GetPageConfig("about");

        Assert.Equal("about", page["current_page"]!.GetValue<string>());
        // A page's copy of the document carries the components themselves, not their ids — that
        // is what the frontend boots from, and it is the application's list narrowed to this page.
        Assert.Contains(about.Id, EntryIds(page["components"]!));
        Assert.DoesNotContain(home.Id, EntryIds(page["components"]!));

        // The page's layout is the application's root with only that page's children under it —
        // the form the grouping synthesised on the page, in this case, rather than the textbox in
        // it, because the form is what sits under the root.
        var aboutChild = Assert.Single(blocks.Children, child => child.PagePath == "about");

        Assert.Equal(
            [aboutChild.Id],
            page["layout"]!["children"]!.AsArray().Select(child => child!["id"]!.GetValue<int>()));

        // Everything else is the application's, including the whole map of pages: the frontend
        // navigates with it, and a page that only knew about itself could not.
        Assert.Equal(blocks.GetConfig()["pages"]!.ToJsonString(), page["pages"]!.ToJsonString());
        Assert.Equal(2, page["page"]!.AsObject().Count);

        // The home page is a page like any other.
        Assert.Contains(home.Id, EntryIds(blocks.GetPageConfig("")["components"]!));
    }

    [Fact]
    public void A_page_the_application_does_not_have_is_refused()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        var refused = Assert.Throws<ArgumentException>(() => blocks.GetPageConfig("missing"));

        Assert.Contains("missing", refused.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void A_route_derives_its_path_from_its_name()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        using (blocks.BeginRender())
        {
            using (blocks.Route("  My Dashboard!  "))
            {
                blocks.Add(new Textbox { Label = "Dashboard" });
            }
        }

        // The Python version's slugify: folded to ASCII, lower-cased, stripped of everything a URL suffix
        // cannot hold, with runs of spaces and hyphens collapsed.
        Assert.Equal("my-dashboard", blocks.Pages[1].Path);
    }

    [Fact]
    public void A_route_refuses_a_path_the_application_already_serves()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        using (blocks.BeginRender())
        {
            Assert.Throws<ArgumentException>(() => blocks.Route("Configuration", "/config"));
            Assert.Throws<ArgumentException>(() => blocks.Route("Odd", "/a b"));
        }
    }

    /// <summary>
    /// A name that derives one of the application's own routes is prefixed rather than refused.
    /// </summary>
    /// <remarks>
    /// The difference between a path the application gave and one derived from a name is the Python
    /// version's: a path it was given is refused, because no renaming would make the page reachable
    /// there, and a derived one is prefixed, because the application only asked for a page called
    /// "Login".
    /// </remarks>
    [Fact]
    public void A_route_whose_derived_path_is_taken_by_the_application_is_prefixed()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        using (blocks.BeginRender())
        {
            using (blocks.Route("Login"))
            {
                blocks.Add(new Textbox { Label = "Sign in" });
            }
        }

        Assert.Equal([string.Empty, "_login"], blocks.Pages.Select(page => page.Path));
    }

    [Fact]
    public void A_route_whose_path_is_taken_is_prefixed_rather_than_refused()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        using (blocks.BeginRender())
        {
            using (blocks.Route("About"))
            {
                blocks.Add(new Textbox { Label = "First" });
            }

            using (blocks.Route("About again", "about"))
            {
                blocks.Add(new Textbox { Label = "Second" });
            }
        }

        // The Python version prefixes a colliding path with an underscore instead of refusing it: two pages
        // may be called the same thing, and the second one still has to be reachable.
        Assert.Equal([string.Empty, "about", "_about"], blocks.Pages.Select(page => page.Path));
    }

    [Fact]
    public void A_page_stays_current_after_its_scope_ends()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        Textbox after;

        using (blocks.BeginRender())
        {
            using (blocks.Route("About"))
            {
                blocks.Add(new Textbox { Label = "About" });
            }

            // The Python version sets the current page and never resets it, so what is written after the
            // last page belongs to that page. Restoring the application's page here would give the
            // same code a different config than the Python version gives it.
            after = blocks.Add(new Textbox { Label = "After" });
        }

        Assert.Equal("about", blocks.CurrentPagePath);
        Assert.Equal("about", after.PagePath);
    }

    /// <summary>
    /// A page's containers are numbered where the page ends, not after the whole application.
    /// </summary>
    /// <remarks>
    /// The Python version runs the root's grouping at the end of every <c>with</c> block on the
    /// application, and a page is one of them. The form a page's trailing form components are
    /// grouped into is therefore numbered before the next page's blocks, id for id. Numbering it at
    /// the end of the render instead would move every later id by one and describe a different
    /// application.
    /// </remarks>
    [Fact]
    public void A_page_groups_what_it_holds_when_it_ends()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        using (blocks.BeginRender())
        {
            blocks.Add(new Textbox { Label = "Home" });

            using (blocks.Route("About"))
            {
                blocks.Add(new Textbox { Label = "About" });
            }
        }

        // Home's textbox, the form the home page was closed with, then the about page's two blocks
        // and the form that page was closed with.
        Assert.Equal(
            [typeof(Textbox), typeof(Form), typeof(Textbox), typeof(Form)],
            blocks.Registered.Select(block => block.GetType()));

        Assert.Equal([1, 2, 3, 4], blocks.Registered.Select(block => block.Id));

        // Each page's layout holds the form that page was closed with, and the form holds the field
        // that was written on the page.
        Assert.Equal([2], blocks.Children.Where(child => child.PagePath.Length == 0).Select(child => child.Id));
        Assert.Equal([4], blocks.Children.Where(child => child.PagePath == "about").Select(child => child.Id));

        var homeForm = Assert.IsType<Form>(blocks.Registered[1]);
        var aboutForm = Assert.IsType<Form>(blocks.Registered[3]);

        Assert.Equal([1], homeForm.Children.Select(child => child.Id));
        Assert.Equal([3], aboutForm.Children.Select(child => child.Id));
    }

    [Fact]
    public void A_page_scope_outside_a_render_belongs_to_that_page()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        Textbox about;

        using (blocks.BeginRender())
        {
            using (blocks.Route("About"))
            {
                about = blocks.Add(new Textbox { Label = "About" });
            }
        }

        var registered = blocks.Registered.Count;

        // Bindings are usually set up after the render, and a page scope is how one of them says
        // which page it belongs to; entering a page that already exists adds nothing to the tree,
        // which is what makes the scope usable a second time.
        using (blocks.Route("About"))
        {
            about.Change(value => value, [about], [about], apiName: "about_changed");
        }

        Assert.Equal("about", Assert.Single(blocks.Dependencies).PagePath);
        Assert.Equal(registered, blocks.Registered.Count);
        Assert.Equal([string.Empty, "about"], blocks.Pages.Select(page => page.Path));
    }

    [Fact]
    public void A_page_can_be_given_more_blocks_after_the_render_that_made_it()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        Textbox first;
        Textbox added;

        using (blocks.BeginRender())
        {
            using (blocks.Route("About"))
            {
                first = blocks.Add(new Textbox { Label = "first" });
            }
        }

        var before = PageComponents(blocks, "about").Length;

        // A second render is how an application adds to itself at run time — the dynamic render —
        // and a page scope says which page the addition belongs to. It is the same page, not a new
        // one: a page is declared once and entered again.
        using (blocks.BeginRender())
        {
            using (blocks.Route("About"))
            {
                added = blocks.Add(new Textbox { Label = "added" });
            }
        }

        var after = PageComponents(blocks, "about");

        Assert.Equal("about", added.PagePath);
        Assert.Contains(first.Id, after);
        Assert.Contains(added.Id, after);

        // The field and the form it was grouped into: a form component written straight onto a
        // page ends up in a form, whichever render wrote it.
        Assert.Equal(before + 2, after.Length);
        Assert.IsType<Form>(added.Parent);
        Assert.Equal([string.Empty, "about"], blocks.Pages.Select(page => page.Path));
    }

    /// <summary>The ids one page's entry names, in the order the config lists them.</summary>
    private static int[] PageComponents(Blocks blocks, string page) =>
        [.. blocks.GetConfig()["page"]![page]!["components"]!.AsArray().Select(id => id!.GetValue<int>())];

    [Fact]
    public void A_second_navbar_on_one_page_is_refused()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        var render = blocks.BeginRender();
        blocks.Add(new Navbar());
        blocks.Add(new Navbar());

        var refused = Assert.Throws<InvalidOperationException>(() => render.Dispose());

        Assert.Contains("Navbar", refused.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void One_navbar_on_each_page_is_allowed()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        using (blocks.BeginRender())
        {
            blocks.Add(new Navbar());

            using (blocks.Route("About"))
            {
                blocks.Add(new Navbar());
            }
        }

        Assert.Equal(2, blocks.Registered.OfType<Navbar>().Count());
    }

    [Fact]
    public void A_navbar_carries_its_links_and_the_name_of_the_home_page()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        Navbar navbar;

        using (blocks.BeginRender())
        {
            navbar = blocks.Add(new Navbar
            {
                Value = [new NavbarLink("Analytics", "analytics"), new NavbarLink("Twitter", "https://example.com")],
                MainPageName = false,
            });
        }

        var props = blocks.GetConfig()["components"]!
            .AsArray()
            .Select(component => component!.AsObject())
            .Single(component => component["id"]!.GetValue<int>() == navbar.Id)["props"]!
            .AsObject();

        Assert.Equal("navbar", props["name"]!.GetValue<string>());
        Assert.Equal(
            "[[\"Analytics\",\"analytics\"],[\"Twitter\",\"https://example.com\"]]",
            props["value"]!.ToJsonString());

        // The home page's name is a string or `false`, and `false` is what "do not offer the home
        // page" is spelled as.
        Assert.False(props["main_page_name"]!.GetValue<bool>());
    }

    private static int[] Ids(JsonNode node) =>
        node.AsArray().Select(item => item!.GetValue<int>()).ToArray();

    private static string[] EndpointNames(JsonObject apiInfo) =>
        apiInfo["named_endpoints"]!.AsObject().Select(pair => pair.Key).ToArray();

    private static int[] EntryIds(JsonNode node) =>
        node.AsArray().Select(item => item!["id"]!.GetValue<int>()).ToArray();
}

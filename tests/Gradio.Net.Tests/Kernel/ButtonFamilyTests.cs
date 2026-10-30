using System.Text.Json.Nodes;
using Gradio.Net;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gradio.Net.Tests.Kernel;

/// <summary>
/// The button family as the config and the frontend see it: what each button says about itself, and
/// what the four that bind their own events bind.
/// </summary>
/// <remarks>
/// <para>
/// The manifest page compares the props of every button against the Python version, so nothing here
/// asserts what a default button's config carries. What it asserts is the half a config comparison
/// cannot isolate: that the four derived buttons are templates drawn by the button component, that
/// each one's own defaults are the ones the Python version gives it, and that the bindings a clear
/// or share button makes for itself are the ones the Python version makes — including the name a
/// second binding under the same name ends up with.
/// </para>
/// <para>
/// The two media buttons are here for their conversions: what a handler is handed, and what a file
/// looks like on the way out. Both are pure computations, and both differ between the two.
/// </para>
/// </remarks>
public sealed class ButtonFamilyTests
{
    [Fact]
    public void A_button_is_served_as_a_button_and_not_as_an_endpoint()
    {
        using var page = Page();

        var entry = Entry(page.Blocks, page.Add(new Button()));

        Assert.Equal("button", entry["type"]!.GetValue<string>());
        Assert.True(entry["skip_api"]!.GetValue<bool>());
        Assert.Null(entry["api_info"]);

        var props = entry["props"]!.AsObject();

        Assert.Equal("Run", props["value"]!.GetValue<string>());
        Assert.Equal("secondary", props["variant"]!.GetValue<string>());
        Assert.Equal("lg", props["size"]!.GetValue<string>());
        Assert.Equal("_self", props["link_target"]!.GetValue<string>());
        Assert.True(props["visible"]!.GetValue<bool>());
        Assert.True(props["interactive"]!.GetValue<bool>());
        Assert.Empty(props["elem_classes"]!.AsArray());
        Assert.Equal(["value"], props["preserved_by_key"]!.AsArray().Select(item => item!.GetValue<string>()));
        Assert.False(props["_selectable"]!.GetValue<bool>());

        // Nothing the application did not ask for is written, and that includes the icon: the Python version's
        // button has no default one, and neither does this one.
        Assert.Null(props["link"]);
        Assert.Null(props["elem_id"]);
        Assert.Null(props["scale"]);
        Assert.Null(props["min_width"]);
        Assert.Null(props["icon"]);
    }

    [Fact]
    public void Every_button_that_is_a_template_is_drawn_by_the_button_component()
    {
        using var page = Page();

        foreach (var button in new Button[]
        {
            new ClearButton(),
            new DuplicateButton(),
            new LoginButton(),
            new DeepLinkButton(),
        })
        {
            page.Add(button);

            var entry = Entry(page.Blocks, button);

            // The Python version marks these four as templates, and a template reports the class it was
            // derived from rather than its own: the frontend draws them all with the button.
            Assert.Equal("button", entry["type"]!.GetValue<string>());
            Assert.Equal("button", entry["props"]!["name"]!.GetValue<string>());
            Assert.True(entry["skip_api"]!.GetValue<bool>());
        }

        // The two that carry a file are components of their own, and are in the API.
        var download = page.Add(new DownloadButton());
        var upload = page.Add(new UploadButton());

        Assert.Equal("downloadbutton", Entry(page.Blocks, download)["type"]!.GetValue<string>());
        Assert.Equal("uploadbutton", Entry(page.Blocks, upload)["type"]!.GetValue<string>());
        Assert.False(Entry(page.Blocks, download)["skip_api"]!.GetValue<bool>());
        Assert.False(Entry(page.Blocks, upload)["skip_api"]!.GetValue<bool>());
    }

    [Fact]
    public void Each_template_button_keeps_its_own_defaults()
    {
        using var page = Page();

        var clear = page.Add(new ClearButton());
        var duplicate = page.Add(new DuplicateButton { AutoActivate = false });
        var login = page.Add(new LoginButton());
        var share = page.Add(new DeepLinkButton());

        Assert.Equal("Clear", Props(page.Blocks, clear)["value"]!.GetValue<string>());

        var duplicateProps = Props(page.Blocks, duplicate);
        Assert.Equal("Duplicate Space", duplicateProps["value"]!.GetValue<string>());
        Assert.Equal("huggingface", duplicateProps["variant"]!.GetValue<string>());
        Assert.Equal("sm", duplicateProps["size"]!.GetValue<string>());
        Assert.Equal(0, duplicateProps["scale"]!.GetValue<int>());

        var loginProps = Props(page.Blocks, login);
        Assert.Equal("Sign in with Hugging Face", loginProps["value"]!.GetValue<string>());
        Assert.Equal("Logout ({})", loginProps["logout_value"]!.GetValue<string>());
        Assert.Equal("huggingface", loginProps["variant"]!.GetValue<string>());

        var shareProps = Props(page.Blocks, share);
        Assert.Equal("Share via Link", shareProps["value"]!.GetValue<string>());
        Assert.Equal("Link Copied!", shareProps["copied_value"]!.GetValue<string>());
    }

    [Fact]
    public void A_clear_button_binds_the_javascript_that_empties_its_components()
    {
        using var page = Page();

        var text = page.Add(new Textbox { Label = "Text" });
        var count = page.Add(new Number { Label = "Count" });
        var agree = page.Add(new Checkbox { Label = "Agree" });

        page.Add(new ClearButton { Components = [text, count, agree] });

        var binding = Dependencies(page.Blocks).Single();

        // The values are each component's own answer to being emptied, spelled the way Python's
        // json.dumps spells them — a space after each comma — because the script is served and
        // compared as text.
        Assert.Equal("() => [null, null, false]", binding["js"]!.GetValue<string>());
        Assert.Equal([text.Id, count.Id, agree.Id], Ids(binding["outputs"]!));
        Assert.Empty(binding["inputs"]!.AsArray());
        Assert.False(binding["backend_fn"]!.GetValue<bool>());

        // Nothing to run means nothing to queue, and the name is the one a frontend-only binding
        // gets — under a visibility that keeps it out of the API.
        Assert.False(binding["queue"]!.GetValue<bool>());
        Assert.Equal("js_fn", binding["api_name"]!.GetValue<string>());
        Assert.Equal("private", binding["api_visibility"]!.GetValue<string>());
    }

    [Fact]
    public void A_clear_button_resets_a_state_on_the_server()
    {
        using var page = Page();

        var text = page.Add(new Textbox { Label = "Text" });
        var state = page.Add(new State { Value = JsonValue.Create(42) });

        page.Add(new ClearButton { Components = [text, state] });

        var dependencies = Dependencies(page.Blocks);

        Assert.Equal(2, dependencies.Length);

        // A state is not the client's to clear: its value is the server's, so the second binding is
        // a handler that returns the value the session started from.
        var reset = dependencies[1];

        Assert.True(reset["backend_fn"]!.GetValue<bool>());
        Assert.Equal([state.Id], Ids(reset["outputs"]!));
        Assert.Equal("lambda", reset["api_name"]!.GetValue<string>());
        Assert.Equal("undocumented", reset["api_visibility"]!.GetValue<string>());
        Assert.Null(reset["js"]);
    }

    [Fact]
    public void A_clear_button_binds_two_endpoints_under_one_name()
    {
        using var page = Page();

        var text = page.Add(new Textbox { Label = "Text" });
        var state = page.Add(new State());

        page.Add(new ClearButton
        {
            Components = [text, state],
            ApiName = "clear_it",
            ApiVisibility = "public",
        });

        var dependencies = Dependencies(page.Blocks);

        // The Python version suffixes a name that is already in use, whether it derived the name or was given
        // it — which is how one button that binds twice serves two endpoints.
        Assert.Equal("clear_it", dependencies[0]["api_name"]!.GetValue<string>());
        Assert.Equal("private", dependencies[0]["api_visibility"]!.GetValue<string>());
        Assert.Equal("clear_it_1", dependencies[1]["api_name"]!.GetValue<string>());
        Assert.Equal("public", dependencies[1]["api_visibility"]!.GetValue<string>());
    }

    [Fact]
    public void A_clear_button_with_nothing_to_clear_binds_nothing()
    {
        using var page = Page();

        page.Add(new ClearButton());

        Assert.Empty(Dependencies(page.Blocks));
    }

    [Fact]
    public void A_clear_button_given_components_before_it_is_rendered_binds_when_it_is_added()
    {
        using var page = Page();

        var text = page.Add(new Textbox { Label = "Text" });
        var button = new ClearButton().Add(text);

        // Nothing to bind to yet: the button has no application, and a binding names one.
        Assert.Empty(Dependencies(page.Blocks));

        page.Add(button);

        var binding = Dependencies(page.Blocks).Single();

        Assert.Equal("() => [null]", binding["js"]!.GetValue<string>());
        Assert.Equal([text.Id], Ids(binding["outputs"]!));
    }

    [Fact]
    public void A_download_button_describes_its_file_without_measuring_it()
    {
        using var page = Page();
        var file = TemporaryFile();

        var button = page.Add(new DownloadButton { Label = "Get", Value = file });

        var props = Props(page.Blocks, button);
        var value = props["value"]!.AsObject();

        // The file is copied into the application's cache, and the config carries the copy.
        Assert.True(MediaCache.IsInFolder(value["path"]!.GetValue<string>(), page.Blocks.UploadFolder));
        Assert.Equal(Path.GetFileName(file), value["orig_name"]!.GetValue<string>());
        Assert.Equal(MediaCache.UrlOf(value["path"]!.GetValue<string>()), value["url"]!.GetValue<string>());

        // The Python version's download button describes the file without its size: the endpoint measures the
        // bytes when it serves them.
        Assert.Null(value["size"]);
        Assert.Equal("gradio.FileData", value["meta"]!["_type"]!.GetValue<string>());
    }

    [Fact]
    public void A_download_button_hands_a_handler_the_files_path()
    {
        using var page = Page();
        var file = TemporaryFile();
        var button = page.Add(new DownloadButton { Value = file });

        var sent = Props(page.Blocks, button)["value"]!.DeepClone();

        Assert.Equal(sent["path"]!.GetValue<string>(), button.Preprocess(sent));
    }

    [Fact]
    public void An_upload_button_hands_a_handler_a_path_or_the_bytes()
    {
        using var page = Page();
        var file = TemporaryFile();
        var sent = new JsonObject { ["path"] = file, ["meta"] = new JsonObject { ["_type"] = "gradio.FileData" } };

        var paths = page.Add(new UploadButton());
        var bytes = page.Add(new UploadButton { Type = "binary" });
        var many = page.Add(new UploadButton { FileCount = "multiple" });

        Assert.Equal(file, paths.Preprocess(sent.DeepClone()));
        Assert.Equal(System.IO.File.ReadAllBytes(file), bytes.Preprocess(sent.DeepClone()));

        var list = many.Preprocess(new JsonArray(sent.DeepClone())) as string[];

        Assert.NotNull(list);
        Assert.Equal(new[] { file }, list!);
    }

    [Fact]
    public void An_upload_button_describes_its_files_with_their_size()
    {
        using var page = Page();
        var file = TemporaryFile();

        var one = page.Add(new UploadButton { Value = file });
        var many = page.Add(new UploadButton { FileCount = "multiple", Value = new JsonArray(file) });

        var single = Props(page.Blocks, one)["value"]!.AsObject();
        Assert.Equal(new FileInfo(file).Length, single["size"]!.GetValue<long>());
        Assert.Equal(Path.GetFileName(file), single["orig_name"]!.GetValue<string>());

        var list = Props(page.Blocks, many)["value"]!.AsArray();
        Assert.Single(list);
        Assert.Equal(Path.GetFileName(file), list[0]!["orig_name"]!.GetValue<string>());
    }

    [Fact]
    public void An_upload_button_describes_itself_with_the_schema_of_what_it_takes()
    {
        var one = new UploadButton();
        var many = new UploadButton { FileCount = "multiple" };

        // The Python version's upload button returns the model's own schema rather than the one its component
        // base builds, so the docstring is under `description` and there is no
        // `additional_description` at all.
        Assert.NotNull(one.ApiInfo["description"]);
        Assert.Null(one.ApiInfo["additional_description"]);
        Assert.Null(many.ApiInfo["description"]);
        Assert.Null(many.ApiInfo["additional_description"]);
        Assert.Equal("array", many.ApiInfo["type"]!.GetValue<string>());
        Assert.Equal("FileData", one.ApiInfo["title"]!.GetValue<string>());
    }

    [Fact]
    public void An_upload_button_refuses_a_type_or_a_file_count_it_does_not_know()
    {
        Assert.Throws<ArgumentException>(() => new UploadButton { Type = "bytes" });
        Assert.Throws<ArgumentException>(() => new UploadButton { FileCount = "several" });
    }

    /// <summary>
    /// An upload button refuses a file its kinds do not allow — but only when it hands over a path.
    /// </summary>
    /// <remarks>
    /// The Python version checks the kinds inside the branch that hands a handler a path, and not
    /// the one that hands it bytes: a button that reads the bytes has already decided the file is
    /// wanted, while one that passes a path on can still say no. The list is the frontend's
    /// <c>accept</c> attribute, and a client is not obliged to honour it, which is why the check
    /// exists at all.
    /// </remarks>
    [Fact]
    public void An_upload_button_refuses_a_file_its_kinds_do_not_allow()
    {
        using var page = Page();
        var text = TemporaryFile(".txt");
        var binary = TemporaryFile(".bin");
        var sent = new JsonObject { ["path"] = text };

        var button = page.Add(new UploadButton { FileTypes = [".txt", ".csv"] });

        Assert.Equal(text, button.Preprocess(sent.DeepClone()));

        var refused = Assert.Throws<ArgumentException>(
            () => button.Preprocess(new JsonObject { ["path"] = binary }));

        Assert.StartsWith(
            "Invalid file type. Please upload a file that is one of these formats: ['.txt', '.csv']",
            refused.Message,
            StringComparison.Ordinal);

        // The same file, through a button that hands over bytes: no check, which is the Python version's rule.
        var bytes = page.Add(new UploadButton { Type = "binary", FileTypes = [".txt"] });

        Assert.Equal(System.IO.File.ReadAllBytes(binary), bytes.Preprocess(new JsonObject { ["path"] = binary }));
    }

    [Fact]
    public void The_share_button_numbers_its_element_per_application()
    {
        using var page = Page();

        var first = page.Add(new DeepLinkButton());
        var second = page.Add(new DeepLinkButton());

        // The Python version numbers the elements from a counter it keeps for the whole process; this build
        // numbers them per application, which is what keeps two applications apart.
        Assert.Equal("gradio-share-link-button-0", Props(page.Blocks, first)["elem_id"]!.GetValue<string>());
        Assert.Equal("gradio-share-link-button-1", Props(page.Blocks, second)["elem_id"]!.GetValue<string>());
    }

    [Fact]
    public void The_share_button_binds_the_copy_and_the_restore()
    {
        using var page = Page();

        page.Add(new DeepLinkButton { Value = "Share", CopiedValue = "Copied" });

        var dependencies = Dependencies(page.Blocks);

        Assert.Equal(2, dependencies.Length);

        var copy = dependencies[0];
        var script = copy["js"]!.GetValue<string>();

        Assert.StartsWith("\n() => {\n    const sessionHash", script);
        Assert.Contains("fetch(`gradio_api/deep_link?session_hash=${sessionHash}`)", script);
        Assert.EndsWith("\n\n    return \"Copied\";\n}\n", script);
        Assert.Equal("js_fn", copy["api_name"]!.GetValue<string>());
        Assert.Equal("private", copy["api_visibility"]!.GetValue<string>());
        Assert.False(copy["backend_fn"]!.GetValue<bool>());

        // The label goes back a second later, and that run does not go through the queue.
        var restore = dependencies[1];

        Assert.True(restore["backend_fn"]!.GetValue<bool>());
        Assert.False(restore["queue"]!.GetValue<bool>());
        Assert.Equal("lambda", restore["api_name"]!.GetValue<string>());
        Assert.Equal("undocumented", restore["api_visibility"]!.GetValue<string>());
    }

    [Fact]
    public void The_sign_in_button_sends_the_browser_to_the_sign_in_route()
    {
        using var page = Page();

        var login = page.Add(new LoginButton());

        var binding = Dependencies(page.Blocks).Single();
        var script = binding["js"]!.GetValue<string>();

        // The label is compared with the one the button started with, and the address is the page's
        // own — a page at the root returns to the root.
        Assert.Contains("uri = buttonValue === \"Sign in with Hugging Face\"", script);
        Assert.Contains("'/login/huggingface?_target_url=/'", script);
        Assert.Contains("'/logout?_target_url=/'", script);
        Assert.Equal([login.Id], Ids(binding["inputs"]!));
        Assert.Empty(binding["outputs"]!.AsArray());
        Assert.Equal("private", binding["api_visibility"]!.GetValue<string>());
    }

    [Fact]
    public void The_duplicate_button_does_nothing_outside_a_space()
    {
        using var page = Page();

        page.Add(new DuplicateButton());

        // The Python version's own words: it does nothing locally. Nothing is bound, so the button is a label.
        Assert.Empty(Dependencies(page.Blocks));
    }

    [Fact]
    public void A_button_describes_its_icon_as_a_file_the_browser_can_fetch()
    {
        using var page = Page();
        var file = TemporaryFile();

        var local = page.Add(new Button { Icon = file });
        var remote = page.Add(new Button { Icon = "https://example.com/icon.svg" });

        var icon = Props(page.Blocks, local)["icon"]!.AsObject();

        Assert.True(MediaCache.IsInFolder(icon["path"]!.GetValue<string>(), page.Blocks.UploadFolder));
        Assert.Equal(MediaCache.UrlOf(icon["path"]!.GetValue<string>()), icon["url"]!.GetValue<string>());

        // An icon is not an upload: the Python version describes it with its path and nothing else.
        Assert.Null(icon["orig_name"]);
        Assert.Null(icon["size"]);

        // A URL is served as it is, because the server never holds the bytes.
        var address = Props(page.Blocks, remote)["icon"]!.AsObject();

        Assert.Equal("https://example.com/icon.svg", address["path"]!.GetValue<string>());
        Assert.Equal("https://example.com/icon.svg", address["url"]!.GetValue<string>());
    }

    [Fact]
    public void A_button_writes_whatever_a_handler_returned_as_its_label()
    {
        var button = new Button();

        Assert.Equal("3", button.Postprocess(3)!.GetValue<string>());

        // Even nothing: the Python version writes the label with str(), and str(None) is the word. A clear
        // button given another button therefore clears it to that word, which is why the difference
        // between the two buttons' conversions is asserted rather than assumed.
        Assert.Equal("None", button.Postprocess(null)!.GetValue<string>());
        Assert.Null(new ClearButton().Postprocess(null));
    }

    /// <summary>The served entry of one component, read out of the application's config.</summary>
    private static JsonObject Entry(Blocks blocks, Block block) =>
        blocks.GetConfig()["components"]!.AsArray()
            .Select(entry => entry!.AsObject())
            .Single(entry => entry["id"]!.GetValue<int>() == block.Id);

    /// <summary>The props of one component, read out of the application's config.</summary>
    private static JsonObject Props(Blocks blocks, Block block) => Entry(blocks, block)["props"]!.AsObject();

    /// <summary>The served entries of the application's bindings.</summary>
    private static JsonObject[] Dependencies(Blocks blocks) =>
        [.. blocks.GetConfig()["dependencies"]!.AsArray().Select(entry => entry!.AsObject())];

    private static int[] Ids(JsonNode node) =>
        [.. node.AsArray().Select(value => value!.GetValue<int>())];

    /// <summary>A file outside the cache, so that describing it copies it in.</summary>
    /// <param name="extension">The extension the name carries, which is what a kinds list matches.</param>
    private static string TemporaryFile(string extension = ".txt")
    {
        var path = Path.Combine(Path.GetTempPath(), $"gradio-button-{Guid.NewGuid():N}{extension}");
        System.IO.File.WriteAllText(path, "hello");

        return path;
    }

    /// <summary>An application with a render scope around it.</summary>
    private sealed class PageScope : IDisposable
    {
        private readonly ServiceProvider _provider;
        private readonly IDisposable _render;

        internal PageScope()
        {
            _provider = KernelHost.BuildProvider();
            Blocks = _provider.CreateBlocksWithContainers();
            Blocks.UploadFolder = Path.Combine(Path.GetTempPath(), $"gradio-buttons-{Guid.NewGuid():N}");

            _render = Blocks.BeginRender();
        }

        internal Blocks Blocks { get; }

        internal TBlock Add<TBlock>(TBlock block)
            where TBlock : Block =>
            Blocks.Add(block);

        public void Dispose()
        {
            _render.Dispose();
            Blocks.Dispose();
            _provider.Dispose();

            if (Directory.Exists(Blocks.UploadFolder))
            {
                Directory.Delete(Blocks.UploadFolder, recursive: true);
            }
        }
    }

    private static PageScope Page() => new();
}

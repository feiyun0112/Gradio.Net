using System.Net;
using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The three documents that describe an application, observed the way the frontend and a client
/// observe them: over HTTP, against a real host (the one seam the spec names).
/// </summary>
/// <remarks>
/// These assert the shape the documents have to have, field by field, against the document an
/// application declares.
/// </remarks>
public sealed class DescriptionEndpointTests
{
    private static readonly (int Id, string Type)[] _componentShape =
    [
        (1, "textbox"),
        (2, "textbox"),
        (3, "number"),
        (4, "number"),
        (5, "file"),
        (6, "file"),
        (7, "image"),
        (8, "imageeditor"),
        (9, "imageslider"),
        (10, "video"),
        (11, "audio"),
        (12, "html"),
        (13, "gallery"),
        (14, "fileexplorer"),
        (15, "model3d"),
        (16, "chatbot"),
        (17, "chatbot"),
        (18, "markdown"),
        (19, "markdown"),
        (20, "dialogue"),
        (21, "dialogue"),
        (22, "simpletextbox"),
        (23, "slider"),
        (24, "slider"),
        (25, "checkbox"),
        (26, "checkboxgroup"),
        (27, "checkboxgroup"),
        (28, "radio"),
        (29, "dropdown"),
        (30, "dropdown"),
        (31, "simpledropdown"),
        (32, "multimodaltextbox"),
        (33, "colorpicker"),
        (34, "datetime"),
        (35, "code"),
        (36, "html"),
        (37, "button"),
        (38, "button"),
        (39, "button"),
        (40, "button"),
        (41, "downloadbutton"),
        (42, "uploadbutton"),
        (43, "uploadbutton"),
        (44, "button"),
        (45, "dataframe"),
        (46, "dataframe"),
        (47, "textbox"),
        (48, "number"),
        (49, "dataset"),
        (50, "json"),
        (51, "paramviewer"),
        (52, "highlightedtext"),
        (53, "annotatedimage"),
        (54, "label"),
        (55, "nativeplot"),
        (56, "nativeplot"),
        (57, "nativeplot"),
        (58, "nativeplot"),
        (59, "plot"),
        (60, "plot"),
        (61, "row"),
        (62, "textbox"),
        (63, "column"),
        (64, "group"),
        (65, "accordion"),
        (66, "textbox"),
        (67, "form"),
        (68, "sidebar"),
        (69, "number"),
        (70, "form"),
        (71, "column"),
        (72, "textbox"),
        (73, "textbox"),
        (74, "form"),
        (75, "draggable"),
        (76, "group"),
        (77, "group"),
        (78, "tabs"),
        (79, "tabitem"),
        (80, "textbox"),
        (81, "form"),
        (82, "tabitem"),
        (83, "number"),
        (84, "form"),
        (85, "walkthrough"),
        (86, "walkthroughstep"),
        (87, "textbox"),
        (88, "form"),
        (89, "walkthroughstep"),
        (90, "number"),
        (91, "form"),
        (92, "html"),
        (93, "html"),
        (94, "api"),
        (95, "api"),
        (96, "fallback"),
        (97, "fallback"),
        (98, "timer"),
        (99, "timer"),
        (100, "workflowcanvas"),
        (101, "workflowcanvas"),
        (102, "form"),
        (103, "form"),
        (104, "form"),
        (105, "form"),
    ];

    private static readonly int[] _componentIds =
        [.. Enumerable.Range(1, 105)];

    /// <summary>The components the first run of fields was grouped into, in the order written.</summary>
    private static readonly int[] _formChildren = [1, 2, 3, 4];

    /// <summary>The text and number family, grouped into a form of its own.</summary>
    /// <remarks>
    /// The colour picker, the code editor and the coloured checkbox group are not form components —
    /// the Python version's own class for each of them says so — so each ends the run of fields
    /// that get grouped, which is why the family appears as two forms with a field of its own
    /// between them.
    /// </remarks>
    private static readonly int[] _inputFormChildren = [22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32];

    /// <summary>The date field, which follows the colour picker and is grouped on its own.</summary>
    private static readonly int[] _dateFormChildren = [34];

    /// <summary>The root's children, in the order the config lists them.</summary>
    /// <remarks>
    /// Four synthesised forms and the components that broke the runs between them, then the plot
    /// family — six components that are not form components, so each stands on its own — then the
    /// four layouts (the row, the draggable, the tabs and the walkthrough), the two HTML components,
    /// which are containers but not layouts, and last the utility components, none of which is a
    /// form component.
    /// </remarks>
    private static readonly int[] _rootChildren =
        [102, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 103, 33, 104, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 105, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 75, 78, 85, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101];

    /// <summary>The row's children: the form it grouped, and the two columns written into it.</summary>
    private static readonly int[] _rowChildren = [74, 63, 71];

    private static readonly string[] _preservedByKey = ["value"];

    private static readonly string[] _infoGroups = ["named_endpoints", "unnamed_endpoints"];

    private static readonly string[] _uploadPath = ["/gradio_api/upload"];

    [Fact]
    public async Task The_config_describes_the_page_the_application_built()
    {
        var config = await GetDocumentAsync("/config");

        Assert.Equal(GradioVersion.Upstream, config["version"]!.GetValue<string>());
        Assert.Equal("/gradio_api", config["api_prefix"]!.GetValue<string>());
        Assert.Equal("blocks", config["mode"]!.GetValue<string>());
        Assert.Equal("sse_v3", config["protocol"]!.GetValue<string>());
        Assert.Equal(ManifestPage.Title, config["title"]!.GetValue<string>());

        // The page turns analytics on, as the case's the Python version half does: this framework defaults
        // it off, so leaving it to the default would compare two defaults rather than
        // the field. Turning it on is what makes the field comparable at all.
        Assert.True(config["analytics_enabled"]!.GetValue<bool>());

        // A single-page application: the Python version spells the home page as an empty name.
        var pages = Assert.IsType<JsonArray>(config["pages"]);
        var home = Assert.Single(pages)!.AsArray();
        Assert.Equal(string.Empty, home[0]!.GetValue<string>());
        Assert.Equal("Home", home[1]!.GetValue<string>());
        Assert.True(home[2]!.GetValue<bool>());

        // The four value components are on the page twice each, and the form they are grouped into
        // is synthesised after them — which is why it has an id above theirs. The media components
        // come after the numbers and are not grouped: none of them is a form component.
        var components = Assert.IsType<JsonArray>(config["components"]);
        Assert.Equal(
            _componentShape,
            components.Select(component => (
                component!["id"]!.GetValue<int>(),
                component["type"]!.GetValue<string>())));

        Assert.Equal(
            _componentIds,
            Assert.IsType<JsonArray>(config["page"]![""]!["components"]).Select(id => id!.GetValue<int>()));

        // Three buttons bind their own events when they are added: the clear button the script
        // that empties three fields, and the share button the two clicks that copy a link and put
        // its label back. Nothing else on the page binds anything, so the list is exactly those.
        var dependencies = Assert.IsType<JsonArray>(config["dependencies"]);
        Assert.Equal(
            ["clear_fields", "js_fn", "lambda"],
            dependencies.Select(entry => entry!["api_name"]!.GetValue<string>()));
        Assert.Equal(
            [0, 1, 2],
            Assert.IsType<JsonArray>(config["page"]![""]!["dependencies"]).Select(id => id!.GetValue<int>()));
    }

    [Fact]
    public async Task The_config_carries_the_parameters_the_application_asked_for()
    {
        var config = await GetDocumentAsync("/config");
        var components = Assert.IsType<JsonArray>(config["components"]);

        var notes = Props(components, id: 2);
        Assert.Equal("Notes", notes["label"]!.GetValue<string>());
        Assert.Equal(4, notes["lines"]!.GetValue<int>());
        Assert.Equal("Anything else", notes["placeholder"]!.GetValue<string>());

        // The optional parameters the application did not set are left out rather than sent as
        // null, which is what the Python version does with them.
        Assert.False(notes.ContainsKey("max_lines"));
        Assert.False(notes.ContainsKey("value"));

        var score = Props(components, id: 4);
        Assert.Equal(1.5, score["value"]!.GetValue<double>());
        Assert.Equal(2, score["precision"]!.GetValue<int>());
        Assert.Equal(0, score["minimum"]!.GetValue<double>());
        Assert.Equal(10, score["maximum"]!.GetValue<double>());

        // The parameters every component of the kind carries, at the Python version's defaults.
        Assert.Equal("text", notes["type"]!.GetValue<string>());
        Assert.True(notes["show_label"]!.GetValue<bool>());
        Assert.True(notes["container"]!.GetValue<bool>());
        Assert.Equal(160, notes["min_width"]!.GetValue<int>());
        Assert.Equal(
            _preservedByKey,
            notes["preserved_by_key"]!.AsArray().Select(item => item!.GetValue<string>()));
    }

    [Fact]
    public async Task The_layout_puts_the_components_inside_the_synthesised_form()
    {
        var config = await GetDocumentAsync("/config");

        var layout = config["layout"]!;
        Assert.Equal(0, layout["id"]!.GetValue<int>());

        // The root holds the forms the grouping synthesised, the components that ended each run,
        // and the layouts, in the order the page writes them.
        var rootChildren = Assert.IsType<JsonArray>(layout["children"]);
        Assert.Equal(
            _rootChildren,
            rootChildren.Select(child => child!["id"]!.GetValue<int>()));

        // The first run is the four value components of the first batch, in the order they were
        // written; the next is the text and number family; the last is the date field that follows
        // the colour picker. The media components in between are not grouped, so each of them sits
        // on the root rather than in a form of its own.
        Assert.Equal(
            _formChildren,
            Assert.IsType<JsonArray>(rootChildren[0]!["children"]).Select(child => child!["id"]!.GetValue<int>()));
        Assert.Equal(
            _inputFormChildren,
            Assert.IsType<JsonArray>(rootChildren[18]!["children"]).Select(child => child!["id"]!.GetValue<int>()));
        Assert.Equal(
            _dateFormChildren,
            Assert.IsType<JsonArray>(rootChildren[20]!["children"]).Select(child => child!["id"]!.GetValue<int>()));

        // The row is the shape the nesting exists for: the form the grouping made inside it is a
        // child of the row rather than of the page, and it is numbered after the columns that
        // follow the field it holds, because the row groups when it closes. It is the first of the
        // four layouts, which follow the plot family on the root.
        var row = rootChildren[46]!;
        Assert.Equal(
            _rowChildren,
            Assert.IsType<JsonArray>(row["children"]).Select(child => child!["id"]!.GetValue<int>()));

        // The page carries the same tree as the document's own layout, as the Python version serves it.
        Assert.True(JsonNode.DeepEquals(config["page"]![""]!["layout"], layout));
    }

    [Fact]
    public async Task The_api_info_describes_no_endpoint_for_an_application_that_binds_none()
    {
        var info = await GetDocumentAsync("/gradio_api/info");

        Assert.Equal(
            _infoGroups,
            info.Select(pair => pair.Key).Order(StringComparer.Ordinal));
        Assert.Empty(Assert.IsType<JsonObject>(info["named_endpoints"]));
        Assert.Empty(Assert.IsType<JsonObject>(info["unnamed_endpoints"]));
    }

    [Fact]
    public async Task The_openapi_document_describes_the_upload_path_unconditionally()
    {
        var schema = await GetDocumentAsync("/gradio_api/openapi.json");

        Assert.Equal("3.0.2", schema["openapi"]!.GetValue<string>());
        Assert.Equal(ManifestPage.Title, schema["info"]!["title"]!.GetValue<string>());
        Assert.Equal(string.Empty, schema["info"]!["description"]!.GetValue<string>());
        Assert.Equal(GradioVersion.Upstream, schema["info"]!["version"]!.GetValue<string>());

        // A file input is served by the upload endpoint rather than by the application's own, so
        // the Python version describes it whether or not the application binds an event.
        var paths = Assert.IsType<JsonObject>(schema["paths"]);
        Assert.Equal(_uploadPath, paths.Select(pair => pair.Key));
        Assert.NotNull(paths["/gradio_api/upload"]!["post"]!["requestBody"]);
    }

    /// <summary>
    /// The API info describes the endpoints the documentation leaves out when it is asked for them.
    /// </summary>
    /// <remarks>
    /// The share button's second click is <c>undocumented</c>: callable, and left out of the
    /// description unless the documentation page asks for every endpoint. A plain request does not,
    /// which is the Python version's rule and the reason the two answers differ here.
    /// </remarks>
    [Fact]
    public async Task The_api_info_describes_the_endpoints_the_documentation_leaves_out_when_asked()
    {
        var plain = await GetDocumentAsync("/gradio_api/info");
        var everything = await GetDocumentAsync("/gradio_api/info?all_endpoints=true");

        var described = Assert.IsType<JsonObject>(plain["named_endpoints"]);
        var all = Assert.IsType<JsonObject>(everything["named_endpoints"]);

        // The share button's second click, and nothing else.
        Assert.Equal(
            ["/lambda"],
            all.Select(pair => pair.Key).Except(described.Select(pair => pair.Key)));

        // A private binding is never described, whichever document is asked for: the clear button's
        // script and the share button's first click are both private.
        Assert.DoesNotContain("/js_fn", all.Select(pair => pair.Key));
        Assert.DoesNotContain("/clear_fields", all.Select(pair => pair.Key));
    }

    private static JsonObject Props(JsonArray components, int id) =>
        components
            .Single(component => component!["id"]!.GetValue<int>() == id)!
            ["props"]!
            .AsObject();

    private static async Task<JsonObject> GetDocumentAsync(string path)
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            ManifestPage.Build);

        using var response = await host.Client.GetAsync(path, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        return JsonNode.Parse(body)!.AsObject();
    }
}

using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// A workflow application: what it draws from a graph, what it draws from bound functions alone,
/// and what its two kinds of endpoint actually run.
/// </summary>
/// <remarks>
/// <para>
/// What is asserted here is the shape of the graph's document: the draft an application without a
/// file gets, the endpoints a run goes through, the answers the canvas asks the server for, and the
/// refusals that stop a workflow from being built at all.
/// </para>
/// <para>
/// The graphs are written to temporary files because a graph is a file: an application reads it
/// when it is built and again for every page it serves, which is the behaviour being exercised.
/// </para>
/// </remarks>
public sealed class WorkflowTests
{
    [Fact]
    public async Task A_workflow_is_drawn_from_its_graph()
    {
        await using var host = await StartAsync(Pipeline);

        var config = await GetAsync(host, "/config");

        Assert.Equal("workflow", config["mode"]!.GetValue<string>());
        Assert.Equal("Shout pipeline", config["title"]!.GetValue<string>());

        var components = Assert.IsType<JsonArray>(config["components"]);

        // The canvas first, then the column the app view goes in: the canvas is what an author
        // arranges, and the column is what a visitor sees instead of it.
        Assert.Equal("workflowcanvas", TypeOf(components[0]));
        Assert.Equal("workflow-app", components[0]!["props"]!["app_view"]!.GetValue<string>());
        Assert.Equal("column", TypeOf(components[1]));
        Assert.Equal("workflow-app", components[1]!["props"]!["elem_id"]!.GetValue<string>());

        // Then the two blocks each bound function is reached by — one argument, one answer — and
        // only then the app view, which is why its blocks are numbered after them.
        Assert.Equal("api", TypeOf(components[2]));
        Assert.Equal("string", components[2]!["props"]!["_api_info"]!["type"]!.GetValue<string>());
        Assert.Equal("api", TypeOf(components[3]));

        // The app view: a heading, one panel, two inputs, a button, and one component per output.
        Assert.Equal("column", TypeOf(components[4]));
        Assert.Equal("workflow-app-body", components[4]!["props"]!["elem_classes"]![0]!.GetValue<string>());
        Assert.Equal("# Shout pipeline", components[5]!["props"]!["value"]!.GetValue<string>());
        Assert.Equal("row", TypeOf(components[6]));

        var labels = components
            .Where(component => TypeOf(component) == "textbox")
            .Select(component => component!["props"]!["label"]!.GetValue<string>());

        Assert.Equal(["Prompt", "Suffix", "Result", "Echo"], labels);

        // The inputs are pre-filled from the graph, which is what makes a saved workflow callable
        // with no arguments.
        Assert.Equal("hello world", components[8]!["props"]!["value"]!.GetValue<string>());
        Assert.Equal("!", components[9]!["props"]!["value"]!.GetValue<string>());
        Assert.Equal("Run", components[10]!["props"]!["value"]!.GetValue<string>());
    }

    [Fact]
    public async Task The_config_binds_the_load_the_button_and_the_two_endpoints()
    {
        await using var host = await StartAsync(Pipeline);

        var dependencies = Assert.IsType<JsonArray>((await GetAsync(host, "/config"))["dependencies"]);

        // The load comes first because it is attached when the first render closes — the canvas and
        // the column its app view goes in are written, and everything after that, the endpoints and
        // the blocks they fill, is written afterwards. The Python version closes its own `with` block at the
        // same point, so its load is the first dependency too.
        Assert.Equal(
            ["_load_initial", "predict_fn_shout", "result"],
            dependencies.Select(entry => entry!["api_name"]!.GetValue<string>()));

        var load = dependencies[0]!;
        Assert.Equal(["0:load"], ConfigDocument.Targets(load));
        Assert.Equal("full", load["show_progress"]!.GetValue<string>());

        // The load takes nothing and fills the canvas, which is the block it is bound to.
        Assert.Empty(Assert.IsType<JsonArray>(load["inputs"]));
        Assert.Equal([1], ConfigDocument.Ids(load["outputs"]));

        // A bound function is reached by a client rather than raised by the page: no target, and
        // left out of the documentation because it is a step of a pipeline rather than an endpoint.
        // Its one argument is the JSON text the canvas sends, so it is bound to the api block.
        var bound = dependencies[1]!;
        Assert.Empty(Assert.IsType<JsonArray>(bound["targets"]));
        Assert.Equal("hidden", bound["show_progress"]!.GetValue<string>());
        Assert.Equal("undocumented", bound["api_visibility"]!.GetValue<string>());
        Assert.Equal([3], ConfigDocument.Ids(bound["inputs"]));
        Assert.Equal([4], ConfigDocument.Ids(bound["outputs"]));

        // The app view's button raises the endpoint that runs the pipeline, with the two inputs it
        // reads and the two outputs it fills.
        var run = dependencies[2]!;
        Assert.Equal(["11:click"], ConfigDocument.Targets(run));
        Assert.Equal([9, 10], ConfigDocument.Ids(run["inputs"]));
        Assert.Equal([14, 15], ConfigDocument.Ids(run["outputs"]));

    }

    [Fact]
    public async Task The_app_view_runs_the_pipeline_the_graph_describes()
    {
        await using var host = await StartAsync(Pipeline);

        var answer = await RunAsync(host, "/gradio_api/run/result", new JsonArray("hello", "!"));

        // One pass over the shared nodes: the operator produced the first answer and the reference
        // relayed the second.
        Assert.Equal("""["HELLO!","hello"]""", answer["data"]!.ToJsonString());
    }

    /// <summary>
    /// A page that opens later reads the graph file again, which is what makes an edit visible
    /// without a restart.
    /// </summary>
    /// <remarks>
    /// The canvas's value is computed once, when the application is built, and that copy is what the
    /// config carries. The load event exists for the pages opened after it: each one asks for the
    /// text again, so a file that changed on disk — by an editor, by a save from the canvas, by
    /// anything — is what the next page draws. An implementation that answered from the value it
    /// read at construction would look identical until the file changed, which is why the file is
    /// changed here rather than only inspected.
    /// </remarks>
    [Fact]
    public async Task A_page_that_opens_later_reads_the_graph_file_again()
    {
        var path = GraphFile(Pipeline);
        await using var host = await StartAsync(Pipeline, graphPath: path);

        var drawn = (await GetAsync(host, "/config"))["components"]![0]!["props"]!["value"]!.GetValue<string>();

        Assert.Equal(Pipeline, drawn);

        // The author edits the file while the application is running.
        const string Edited = """{"schema_version":"2","name":"Edited","references":[],"operators":[],"subjects":[],"edges":[]}""";

        await System.IO.File.WriteAllTextAsync(path, Edited, TestContext.Current.CancellationToken);

        var reloaded = await RunAsync(host, "/gradio_api/run/_load_initial", []);

        Assert.Equal(Edited, reloaded["data"]![0]!.GetValue<string>());
    }

    /// <summary>
    /// An application that registers an operator can run a node this build has no caller for.
    /// </summary>
    /// <remarks>
    /// `space`, `model` and `dataset` nodes are not run by the framework: each is an integration
    /// with a service, and which client to use is the application's decision. What the
    /// framework owes is the seam — a node of that kind, with the values its ports were given,
    /// handed to the operator the application registered under that kind — and this is the test
    /// that the seam is reachable from a request rather than only from the executor's own tests.
    /// </remarks>
    [Fact]
    public async Task An_application_that_registers_an_operator_runs_a_node_of_that_kind()
    {
        await using var host = await StartAsync(
            ModelPipeline,
            operators: new Dictionary<string, WorkflowOperator>(StringComparer.Ordinal)
            {
                ["model"] = (node, arguments, _) => ValueTask.FromResult<IReadOnlyList<object?>>(
                    [$"{node.Raw["model_id"]} answered {arguments[0]}"]),
            });

        var answer = await RunAsync(host, "/gradio_api/run/result", new JsonArray("hello"));

        Assert.Equal("""["black-forest-labs/FLUX.1-schnell answered hello"]""", answer["data"]!.ToJsonString());
    }

    [Fact]
    public async Task A_bound_function_answers_the_canvas_through_its_own_endpoint()
    {
        await using var host = await StartAsync(Pipeline);

        var answer = await RunAsync(
            host,
            "/gradio_api/run/predict_fn_shout",
            new JsonArray("""["hello","!"]"""));

        // The answer is the list of values the node produced, written as JSON text: the canvas
        // reads it as the node's outputs rather than as one value.
        Assert.Equal("""["HELLO!"]""", answer["data"]![0]!.GetValue<string>());
    }

    [Fact]
    public async Task The_canvas_is_told_it_may_not_change_the_workflow()
    {
        await using var host = await StartAsync(Pipeline);

        // The Python version answers this per visitor, and only the holder of the launch-time write token is
        // allowed to edit. This build has no write path, so every visitor is a read-only one — which
        // is the answer the canvas acts on rather than a client-side assumption.
        Assert.Equal("false", await CanvasFunctionAsync(host, "get_write_access"));
    }

    [Fact]
    public async Task The_canvas_keys_a_visitors_arrangement_by_the_file_it_was_built_from()
    {
        // The path is written here rather than by the helper, because the key is the path hashed:
        // an assertion about it has to hash the same path the workflow was given.
        var path = GraphFile(Pipeline);
        await using var host = await StartAsync(Pipeline, graphPath: path);

        var key = await CanvasFunctionAsync(host, "get_workflow_key");
        var digest = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(Path.GetFullPath(path))));

        // The path is hashed rather than sent, so that a visitor's browser storage does not carry
        // the author's directory layout. Sixteen hex digits of sha256, which is the Python version's length.
        Assert.Equal("file:" + digest[..16].ToLowerInvariant(), key);
    }

    [Fact]
    public async Task The_canvas_is_told_which_app_view_version_it_holds()
    {
        await using var host = await StartAsync(Pipeline);

        // A page compares this against the copy it already has to know whether it is stale. Nothing
        // has rebuilt the app view since it was drawn, so the answer is the initial version — a
        // second number appears only after a save, which this build has no write path for.
        Assert.Equal("1.0", await CanvasFunctionAsync(host, "get_app_version"));
    }

    [Fact]
    public async Task The_canvas_is_told_which_functions_it_may_add_as_nodes()
    {
        await using var host = await StartAsync(Pipeline);

        var templates = JsonNode.Parse(await CanvasFunctionAsync(host, "list_bound_fns"))!.AsArray();

        var template = Assert.Single(templates)!;
        Assert.Equal("shout", template["fn"]!.GetValue<string>());
        Assert.Equal(
            ["in_text", "in_suffix"],
            template["inputs"]!.AsArray().Select(port => port!["id"]!.GetValue<string>()));
        Assert.Equal("out_0", template["outputs"]![0]!["id"]!.GetValue<string>());
    }

    [Fact]
    public async Task The_canvas_is_told_which_endpoints_the_workflow_exposes()
    {
        await using var host = await StartAsync(Pipeline);

        var document = JsonNode.Parse(await CanvasFunctionAsync(host, "get_workflow_api"))!;
        var endpoint = Assert.Single(document["endpoints"]!.AsArray())!;

        Assert.Equal("/result", endpoint["api_name"]!.GetValue<string>());
        Assert.Equal("Result", endpoint["label"]!.GetValue<string>());
        Assert.Equal(
            ["in_0", "in_1"],
            endpoint["parameters"]!.AsArray().Select(parameter => parameter!["parameter_name"]!.GetValue<string>()));
        Assert.Equal(
            ["Result", "Echo"],
            endpoint["returns"]!.AsArray().Select(value => value!["label"]!.GetValue<string>()));
    }

    /// <summary>
    /// The canvas is offered the templates the Python version ships, in the order it ranks them.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The list is the Python version's own snapshot, vendored byte for byte and verified by the
    /// build, so what is asserted here is the ranking and the shape rather than the data: the ids
    /// in the order the canvas shows them, and the fields its cards read.
    /// </para>
    /// <para>
    /// A space's card carries three constants — no likes, no downloads, and a runtime that is always
    /// running — because a curated entry has none of them. The number it does carry is the latency of
    /// its last check, which is also what ranks it: the templates that run without a GPU first, then
    /// the featured ones, then the fastest.
    /// </para>
    /// </remarks>
    [Fact]
    public async Task The_canvas_is_offered_the_templates_upstream_ships()
    {
        await using var host = await StartAsync(Pipeline);

        var spaces = JsonNode.Parse(await CanvasFunctionAsync(host, "search_spaces", ["trending"]))!.AsArray();

        Assert.Equal(
            [
                "hf-audio/whisper-large-v3-turbo",
                "briaai/BRIA-RMBG-2.0",
                "black-forest-labs/FLUX.1-schnell",
                "coqui/xtts",
                "stabilityai/stable-diffusion-3.5-large",
                "tencent/Hunyuan3D-2",
                "Wan-AI/Wan2.2-TI2V-5B",
                "multimodalart/face-to-all",
                "facebook/MusicGen",
            ],
            spaces.Select(space => space!["id"]!.GetValue<string>()));

        var first = spaces[0]!;

        Assert.Equal(0, first["likes"]!.GetValue<int>());
        Assert.Equal(900, first["trendingScore"]!.GetValue<int>());
        Assert.Equal("RUNNING", first["runtime"]!["stage"]!.GetValue<string>());
        Assert.Equal("automatic-speech-recognition", first["pipeline_tag"]!.GetValue<string>());
        Assert.Equal(["zero-gpu"], first["tags"]!.AsArray().Select(tag => tag!.GetValue<string>()));
        Assert.Equal("gradio", first["cardData"]!["sdk"]!.GetValue<string>());
        Assert.Equal("Whisper Large v3 Turbo", first["cardData"]!["title"]!.GetValue<string>());
        Assert.True(first["_curated"]!.GetValue<bool>());
        Assert.Null(first["_thumbnail"]);

        var models = JsonNode.Parse(await CanvasFunctionAsync(host, "search_models", ["trending"]))!.AsArray();

        Assert.Equal(
            [
                "black-forest-labs/FLUX.1-schnell",
                "stabilityai/stable-diffusion-3.5-large-turbo",
                "openai/whisper-large-v3-turbo",
                "meta-llama/Llama-3.2-3B-Instruct",
                "facebook/detr-resnet-50",
                "facebook/sam-vit-base",
                "depth-anything/Depth-Anything-V2-Small-hf",
                "Qwen/Qwen2.5-VL-7B-Instruct",
                "Qwen/Qwen2.5-7B-Instruct",
                "google/vit-base-patch16-224",
                "facebook/bart-large-cnn",
                "Helsinki-NLP/opus-mt-en-fr",
                "deepset/roberta-base-squad2",
            ],
            models.Select(model => model!["id"]!.GetValue<string>()));

        // A model's card carries a download count where a space's carries a runtime, and neither
        // carries a description: the canvas draws the two kinds differently.
        Assert.Equal(0, models[0]!["downloads"]!.GetValue<int>());
        Assert.True(models[0]!["_featured"]!.GetValue<bool>());
        Assert.Null(models[0]!["runtime"]);
    }

    /// <summary>
    /// A search narrows by what was typed, by the task, by the modality, and by the GPU it needs.
    /// </summary>
    /// <remarks>
    /// The filters are the canvas's four dropdowns and its one switch, and the order they are
    /// applied in is the Python version's: a task wins over a modality, and "runs without a GPU" is
    /// applied to what is left. "New" is a different ordering rather than a different filter — the
    /// same templates, newest first.
    /// </remarks>
    [Fact]
    public async Task A_search_narrows_by_query_task_modality_and_gpu()
    {
        await using var host = await StartAsync(Pipeline);

        Assert.Equal(
            ["black-forest-labs/FLUX.1-schnell"],
            await IdsAsync(host, "search_spaces", ["trending", "flux"]));

        Assert.Equal(
            ["black-forest-labs/FLUX.1-schnell", "stabilityai/stable-diffusion-3.5-large"],
            await IdsAsync(host, "search_spaces", ["trending", "", "text-to-image"]));

        // The same modality without the switch keeps every image template; with it, only the ones
        // that answer without a GPU.
        Assert.Equal(
            [
                "briaai/BRIA-RMBG-2.0",
                "black-forest-labs/FLUX.1-schnell",
                "stabilityai/stable-diffusion-3.5-large",
                "multimodalart/face-to-all",
            ],
            await IdsAsync(host, "search_spaces", ["trending", "", "", "image", true]));

        // Newest first is the same nine templates in the order the list records them.
        Assert.Equal(
            [
                "multimodalart/face-to-all",
                "facebook/MusicGen",
                "tencent/Hunyuan3D-2",
                "Wan-AI/Wan2.2-TI2V-5B",
                "black-forest-labs/FLUX.1-schnell",
                "stabilityai/stable-diffusion-3.5-large",
                "briaai/BRIA-RMBG-2.0",
                "hf-audio/whisper-large-v3-turbo",
                "coqui/xtts",
            ],
            await IdsAsync(host, "search_spaces", ["new"]));

        Assert.Equal(
            ["Qwen/Qwen2.5-VL-7B-Instruct", "Qwen/Qwen2.5-7B-Instruct"],
            await IdsAsync(host, "search_models", ["trending", "qwen"]));
    }

    /// <summary>
    /// A repository is answered as one of the templates or as none, and the kind has to match.
    /// </summary>
    /// <remarks>
    /// The canvas asks before offering "edit this Space" or "edit this model": a template it already
    /// knows is opened as one, and anything else is loaded as a repository. The id alone is enough —
    /// the same repository is a template in both roles — which is why the kind is optional.
    /// </remarks>
    [Fact]
    public async Task A_repository_is_answered_as_a_template_or_as_none()
    {
        await using var host = await StartAsync(Pipeline);

        Assert.Equal(
            """{"curated": true, "status": "ok", "featured": true}""",
            await CanvasFunctionAsync(host, "is_curated", ["black-forest-labs/FLUX.1-schnell"]));

        Assert.Equal(
            """{"curated": true, "status": "ok", "featured": true}""",
            await CanvasFunctionAsync(host, "is_curated", ["black-forest-labs/FLUX.1-schnell", "model"]));

        Assert.Equal(
            """{"curated": false}""",
            await CanvasFunctionAsync(host, "is_curated", ["nobody/nothing"]));
    }

    /// <summary>
    /// The two lists the canvas filters by are what the templates cover.
    /// </summary>
    /// <remarks>
    /// The tasks are a union of two fields rather than one: a model names a task and a Space names a
    /// category, and the canvas offers both in one dropdown. Both lists are sorted, and both leave out
    /// a template whose last check failed — a filter that offered a broken template would be a filter
    /// that leads nowhere.
    /// </remarks>
    [Fact]
    public async Task The_canvas_is_told_which_modalities_and_tasks_the_templates_cover()
    {
        await using var host = await StartAsync(Pipeline);

        Assert.Equal(
            ["3d", "audio", "image", "text", "video"],
            await NamesAsync(host, "curated_modalities"));

        Assert.Equal(
            [
                "3d-modeling", "automatic-speech-recognition", "depth-estimation", "image-classification",
                "image-editing", "image-generation", "image-segmentation", "image-text-to-text",
                "image-to-3d", "image-to-image", "music-generation", "object-detection",
                "question-answering", "speech-synthesis", "summarization", "text-generation",
                "text-to-audio", "text-to-image", "text-to-speech", "text-to-video", "translation",
                "video-generation",
            ],
            await NamesAsync(host, "curated_modality_tasks"));

        Assert.Equal(
            [
                "depth-estimation", "image-classification", "image-editing", "image-generation",
                "image-segmentation", "image-text-to-text", "image-to-image", "object-detection",
                "text-to-image",
            ],
            await NamesAsync(host, "curated_modality_tasks", ["image"]));
    }

    [Fact]
    public async Task A_workflow_with_no_file_draws_its_bound_functions_as_a_draft()
    {
        await using var host = await StartAsync(graph: null, bind: Shout);

        var config = await GetAsync(host, "/config");
        var components = Assert.IsType<JsonArray>(config["components"]);

        // The draft is the canvas's first schema: functions and wires, with no positions and no
        // settings. Only a saved graph can be run as an endpoint, so an application that has not
        // saved one yet draws the canvas and says there is nothing to run.
        Assert.Equal(Draft, components[0]!["props"]!["value"]!.GetValue<string>());
        Assert.Equal("Gradio", config["title"]!.GetValue<string>());
        Assert.Equal(
            "This workflow doesn't have any outputs yet. Open the workflow and add an output node to turn it into an app.",
            components[6]!["props"]!["value"]!.GetValue<string>());
    }

    [Fact]
    public async Task Two_bound_names_that_become_one_endpoint_name_are_refused()
    {
        var error = await Assert.ThrowsAnyAsync<ArgumentException>(() => StartAsync(
            graph: null,
            bind: new Dictionary<string, Delegate>(StringComparer.Ordinal)
            {
                ["a.b"] = new Func<string, string>(value => value),
                ["a_b"] = new Func<string, string>(value => value),
            }));

        Assert.Contains("both become the endpoint name 'a_b'", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task An_edge_that_names_a_function_that_is_not_bound_is_refused()
    {
        var error = await Assert.ThrowsAnyAsync<ArgumentException>(() => StartAsync(
            graph: null,
            bind: Shout,
            edges: [new WorkflowEdgeSpec("shout", "missing")]));

        Assert.Contains("no function 'missing'", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task A_workflow_of_several_pipelines_is_drawn_as_one_tab_each()
    {
        await using var host = await StartAsync(TwoPipelines);

        var components = Assert.IsType<JsonArray>((await GetAsync(host, "/config"))["components"]);

        var types = components.Select(TypeOf).ToList();
        Assert.Contains("tabs", types);
        Assert.Equal(2, types.Count(type => type == "tabitem"));

        // Each tab is named after its pipeline's output, which is also the name of its endpoint.
        var tabs = components
            .Where(component => TypeOf(component) == "tabitem")
            .Select(component => component!["props"]!["label"]!.GetValue<string>());

        Assert.Equal(["First", "Second"], tabs);
    }

    [Fact]
    public async Task A_file_that_is_not_a_saved_graph_leaves_the_app_view_empty()
    {
        // Version 1 is the draft's schema. A workflow that reads one has no outputs to expose, and
        // says so rather than drawing a panel it cannot fill.
        await using var host = await StartAsync("""{"version": "1", "name": "Draft", "nodes": []}""");

        var config = await GetAsync(host, "/config");
        var components = Assert.IsType<JsonArray>(config["components"]);

        Assert.Equal("Gradio", config["title"]!.GetValue<string>());

        // The canvas, the column, the bound function's endpoint, the body and its two notes: no
        // panel, because a draft has no outputs to draw one from.
        Assert.Equal("markdown", TypeOf(components[^1]));
        Assert.DoesNotContain(components, component => TypeOf(component) == "button");
    }

    private static readonly Dictionary<string, Delegate> Shout = new(StringComparer.Ordinal)
    {
        ["shout"] = new Func<string, string, string>((text, suffix) => text.ToUpperInvariant() + suffix),
    };

    /// <summary>The draft a bound function and a wire produce, spelled the way the Python version
    /// spells it.</summary>
    private const string Draft =
        """{"version": "1", "name": "Workflow", "nodes": [{"id": "fn_shout", "source": "fn", "fn": "shout", "kind": "transform", "label": "shout", "width": 220, "inputs": [{"id": "in_text", "label": "text", "type": "text"}, {"id": "in_suffix", "label": "suffix", "type": "text"}], "outputs": [{"id": "out_0", "label": "output", "type": "text"}], "data": {}}], "edges": []}""";

    private static Task<GradioTestHost> StartAsync(
        string? graph,
        IReadOnlyDictionary<string, Delegate>? bind = null,
        IReadOnlyList<WorkflowEdgeSpec>? edges = null,
        string? graphPath = null,
        IReadOnlyDictionary<string, WorkflowOperator>? operators = null) =>
        GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks =>
            {
                _ = new Workflow(
                    blocks,
                    graph: graphPath ?? (graph is null ? MissingFile() : GraphFile(graph)),
                    bind: bind ?? Shout,
                    edges: edges,
                    operators: operators);
            });

    /// <summary>Writes a graph to a file, because a graph is read from one.</summary>
    private static string GraphFile(string json)
    {
        var directory = Directory.CreateTempSubdirectory("gradio-workflow-");
        var path = Path.Combine(directory.FullName, "workflow.json");
        System.IO.File.WriteAllText(path, json);

        return path;
    }

    /// <summary>A path no file is at, so that the workflow starts with the draft it can generate.</summary>
    private static string MissingFile() =>
        Path.Combine(Path.GetTempPath(), $"gradio-workflow-{Guid.NewGuid():N}", "workflow.json");

    private static async Task<JsonObject> GetAsync(GradioTestHost host, string path)
    {
        using var response = await host.Client.GetAsync(path, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        return JsonNode.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))!
            .AsObject();
    }

    private static async Task<JsonObject> RunAsync(GradioTestHost host, string path, JsonArray data)
    {
        using var content = new StringContent(
            new JsonObject { ["data"] = data }.ToJsonString(),
            Encoding.UTF8,
            "application/json");

        using var response = await host.Client.PostAsync(path, content, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        return JsonNode.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))!
            .AsObject();
    }

    /// <summary>Calls one of the canvas's own functions, the way the browser does.</summary>
    /// <param name="host">The host.</param>
    /// <param name="function">The function's name.</param>
    /// <param name="data">The arguments the browser sends, or <see langword="null"/> for none.</param>
    /// <returns>The function's answer, as the string it returned.</returns>
    private static async Task<string> CanvasFunctionAsync(
        GradioTestHost host,
        string function,
        JsonArray? data = null)
    {
        using var content = new StringContent(
            new JsonObject
            {
                ["session_hash"] = "workflow",
                ["component_id"] = 1,
                ["fn_name"] = function,
                ["data"] = data?.DeepClone(),
            }.ToJsonString(),
            Encoding.UTF8,
            "application/json");

        using var response = await host.Client.PostAsync(
            "/gradio_api/component_server",
            content,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)).Trim();

        // The function answered with a string, which reaches the browser as a JSON string.
        return JsonNode.Parse(body)!.GetValue<string>();
    }

    private static string TypeOf(JsonNode? component) => component!["type"]!.GetValue<string>();

    /// <summary>The ids a template search answers with, in the order it answered them.</summary>
    /// <param name="host">The host.</param>
    /// <param name="function">The search.</param>
    /// <param name="data">Its arguments.</param>
    /// <returns>The ids.</returns>
    private static async Task<string[]> IdsAsync(GradioTestHost host, string function, JsonArray data) =>
        JsonNode.Parse(await CanvasFunctionAsync(host, function, data))!
            .AsArray()
            .Select(entry => entry!["id"]!.GetValue<string>())
            .ToArray();

    /// <summary>The names one of the filter lists answers with, in order.</summary>
    /// <param name="host">The host.</param>
    /// <param name="function">The list.</param>
    /// <param name="data">Its arguments, or <see langword="null"/> for none.</param>
    /// <returns>The names.</returns>
    private static async Task<string[]> NamesAsync(
        GradioTestHost host,
        string function,
        JsonArray? data = null) =>
        JsonNode.Parse(await CanvasFunctionAsync(host, function, data))!
            .AsArray()
            .Select(name => name!.GetValue<string>())
            .ToArray();

    /// <summary>A graph whose operator is a kind this build has no caller for.</summary>
    /// <remarks>
    /// One reference the caller supplies, one <c>model</c> operator, one subject. Nothing about the
    /// graph is special: what makes it a test is that the operator's kind is not <c>fn</c>, so the
    /// only way the pipeline can run is through an operator the application registered.
    /// </remarks>
    private const string ModelPipeline = """
        {
          "schema_version": "2",
          "name": "Model pipeline",
          "references": [
            {"id": "prompt", "label": "Prompt", "asset_type": "text",
             "inputs": [{"id": "in", "label": "Text", "type": "text"}],
             "outputs": [{"id": "out", "label": "Text", "type": "text"}],
             "data": {"out": "hello"}}
          ],
          "operators": [
            {"id": "image", "kind": "model", "model_id": "black-forest-labs/FLUX.1-schnell",
             "label": "Image",
             "inputs": [{"id": "prompt", "label": "Prompt", "type": "text", "required": true}],
             "outputs": [{"id": "out_0", "label": "Image", "type": "text", "output_index": 0}],
             "data": {}}
          ],
          "subjects": [
            {"id": "result", "label": "Result", "asset_type": "text",
             "inputs": [{"id": "in", "label": "Text", "type": "text"}],
             "outputs": [{"id": "out", "label": "Text", "type": "text"}],
             "data": {}}
          ],
          "edges": [
            {"id": "e1", "from_node_id": "prompt", "from_port_id": "out", "to_node_id": "image", "to_port_id": "prompt", "type": "text"},
            {"id": "e2", "from_node_id": "image", "from_port_id": "out_0", "to_node_id": "result", "to_port_id": "in", "type": "text"}
          ]
        }
        """;

    private const string Pipeline = """
        {
          "schema_version": "2",
          "name": "Shout pipeline",
          "references": [
            {"id": "prompt", "label": "Prompt", "asset_type": "text",
             "inputs": [{"id": "in", "label": "Text", "type": "text"}],
             "outputs": [{"id": "out", "label": "Text", "type": "text"}],
             "data": {"out": "hello world"}},
            {"id": "suffix", "label": "Suffix", "asset_type": "text",
             "inputs": [{"id": "in", "label": "Text", "type": "text"}],
             "outputs": [{"id": "out", "label": "Text", "type": "text"}],
             "data": {"out": "!"}}
          ],
          "operators": [
            {"id": "shout", "kind": "fn", "fn": "shout", "label": "shout",
             "inputs": [{"id": "in_text", "label": "text", "type": "text"}, {"id": "in_suffix", "label": "suffix", "type": "text"}],
             "outputs": [{"id": "out_0", "label": "output", "type": "text"}],
             "data": {}}
          ],
          "subjects": [
            {"id": "result", "label": "Result", "asset_type": "text",
             "inputs": [{"id": "in", "label": "Text", "type": "text"}],
             "outputs": [{"id": "out", "label": "Text", "type": "text"}],
             "data": {}},
            {"id": "echo", "label": "Echo", "asset_type": "text",
             "inputs": [{"id": "in", "label": "Text", "type": "text"}],
             "outputs": [{"id": "out", "label": "Text", "type": "text"}],
             "data": {}}
          ],
          "edges": [
            {"id": "e1", "from_node_id": "prompt", "from_port_id": "out", "to_node_id": "shout", "to_port_id": "in_text", "type": "text"},
            {"id": "e2", "from_node_id": "suffix", "from_port_id": "out", "to_node_id": "shout", "to_port_id": "in_suffix", "type": "text"},
            {"id": "e3", "from_node_id": "shout", "from_port_id": "out_0", "to_node_id": "result", "to_port_id": "in", "type": "text"},
            {"id": "e4", "from_node_id": "prompt", "from_port_id": "out", "to_node_id": "echo", "to_port_id": "in", "type": "text"}
          ]
        }
        """;

    private const string TwoPipelines = """
        {
          "schema_version": "2",
          "name": "Two pipelines",
          "subjects": [
            {"id": "one", "label": "First", "asset_type": "text",
             "inputs": [{"id": "in", "label": "Text", "type": "text"}], "data": {}},
            {"id": "two", "label": "Second", "asset_type": "text",
             "inputs": [{"id": "in", "label": "Text", "type": "text"}], "data": {}}
          ]
        }
        """;
}

using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// An endpoint derived from a handler's signature, observed the way a client observes it: over HTTP,
/// against a real host (the one seam the spec names).
/// </summary>
/// <remarks>
/// Four things have to be true of such an endpoint and each is a test here: the config describes the
/// blocks the signature produced, the API info describes the arguments it takes, the OpenAPI
/// document carries the paths a generic client generates a call from, and a call actually runs the
/// handler with the types it declared. The fourth is the one that would fail silently if the
/// conversion were wrong — a JSON number is a <c>double</c>, and a delegate cannot be called with
/// one where it declared an <c>int</c>.
/// </remarks>
public sealed class ApiEndpointTests
{
    [Fact]
    public async Task The_config_describes_a_binding_with_no_trigger()
    {
        var config = await GetAsync("/config", mcp: false);
        var dependencies = Assert.IsType<JsonArray>(config["dependencies"]);

        var add = dependencies.Single(entry => entry!["api_name"]!.GetValue<string>() == "add")!;

        // Nothing in the page raises a derived endpoint: a client reaches it by name, which is what
        // an empty target list says.
        Assert.Empty(Assert.IsType<JsonArray>(add["targets"]));
        Assert.True(add["backend_fn"]!.GetValue<bool>());
        Assert.True(add["queue"]!.GetValue<bool>());

        // Three of the Python version's binding parameters are the endpoint's rather than the caller's.
        Assert.Equal("hidden", add["show_progress"]!.GetValue<string>());
        Assert.False(add["scroll_to_output"]!.GetValue<bool>());
        Assert.Equal("once", add["trigger_mode"]!.GetValue<string>());
        Assert.Equal("sse", add["connection"]!.GetValue<string>());
        Assert.Equal("public", add["api_visibility"]!.GetValue<string>());
        Assert.False(add["types"]!["generator"]!.GetValue<bool>());
    }

    [Fact]
    public async Task The_config_describes_one_block_per_argument_and_per_return_value()
    {
        var config = await GetAsync("/config", mcp: false);
        var components = Assert.IsType<JsonArray>(config["components"]);
        var dependencies = Assert.IsType<JsonArray>(config["dependencies"]);

        var slice = dependencies.Single(entry => entry!["api_name"]!.GetValue<string>() == ApiPage.SliceName)!;
        var inputs = Ids(slice["inputs"]!.AsArray());
        var outputs = Ids(slice["outputs"]!.AsArray());

        // Three arguments and a tuple of two, so three blocks in and two out.
        Assert.Equal(3, inputs.Length);
        Assert.Equal(2, outputs.Length);

        var a = Props(components, inputs[0]);
        Assert.Equal("1st", a["label"]!.GetValue<string>());
        Assert.Equal("integer", a["_api_info"]!["type"]!.GetValue<string>());

        // A collection is described by its elements, and the name is the handler's own.
        var items = Props(components, inputs[2]);
        Assert.Equal("3rd", items["label"]!.GetValue<string>());
        Assert.Equal("array", items["_api_info"]!["type"]!.GetValue<string>());
        Assert.Equal("integer", items["_api_info"]!["items"]!["type"]!.GetValue<string>());

        // A tuple's two values are two outputs, each described by its own type.
        Assert.Equal("integer", Props(components, outputs[0])["_api_info"]!["type"]!.GetValue<string>());
        Assert.Equal("string", Props(components, outputs[1])["_api_info"]!["type"]!.GetValue<string>());
    }

    [Fact]
    public async Task The_api_info_describes_the_arguments_the_handler_declares()
    {
        var info = await GetAsync("/gradio_api/info", mcp: false);
        var endpoints = Assert.IsType<JsonObject>(info["named_endpoints"]);

        var add = endpoints["/add"]!;
        var parameters = Assert.IsType<JsonArray>(add["parameters"]);

        Assert.Equal(["a", "b"], parameters.Select(parameter => parameter!["parameter_name"]!.GetValue<string>()));

        // An argument with no default is one a client has to send, and the schema says what it is.
        Assert.False(parameters[0]!["parameter_has_default"]!.GetValue<bool>());
        Assert.Equal("integer", parameters[0]!["type"]!["type"]!.GetValue<string>());
        Assert.Equal("int", parameters[0]!["python_type"]!["type"]!.GetValue<string>());

        // A tuple's two values are described as two returns.
        var slice = endpoints[$"/{ApiPage.SliceName}"]!;
        Assert.Equal(2, Assert.IsType<JsonArray>(slice["returns"]).Count);
        Assert.Equal("string", slice["returns"]![1]!["type"]!["type"]!.GetValue<string>());

        // A declared default is what makes an argument optional, and it is carried, not implied.
        var summarise = endpoints["/Summarise"]!;
        Assert.True(summarise["parameters"]![1]!["parameter_has_default"]!.GetValue<bool>());
        Assert.Equal(20, summarise["parameters"]![1]!["parameter_default"]!.GetValue<int>());
    }

    [Fact]
    public async Task The_openapi_document_carries_the_derived_endpoint()
    {
        var schema = await GetAsync("/gradio_api/openapi.json", mcp: false);
        var paths = Assert.IsType<JsonObject>(schema["paths"]);

        Assert.True(paths.ContainsKey("/gradio_api/call/v2/add"));
        Assert.True(paths.ContainsKey("/gradio_api/call/add/{event_id}"));

        // The request body is what a client generates a call from, so it names the arguments and
        // their types rather than the components that stand for them.
        var properties = paths["/gradio_api/call/v2/add"]!["post"]!["requestBody"]!["content"]!
            ["application/json"]!["schema"]!["properties"]!.AsObject();

        Assert.Equal(["a", "b"], properties.Select(pair => pair.Key));
        Assert.Equal("integer", properties["a"]!["type"]!.GetValue<string>());
    }

    [Fact]
    public async Task A_private_endpoint_is_hidden_from_every_document()
    {
        var plain = Assert.IsType<JsonObject>((await GetAsync("/gradio_api/info", mcp: false))["named_endpoints"]);
        var everything = Assert.IsType<JsonObject>(
            (await GetAsync("/gradio_api/info?all_endpoints=true", mcp: false))["named_endpoints"]);

        Assert.False(plain.ContainsKey($"/{ApiPage.HiddenName}"));
        Assert.False(everything.ContainsKey($"/{ApiPage.HiddenName}"));

        var schema = await GetAsync("/gradio_api/openapi.json", mcp: false);
        Assert.False(schema["paths"]!.AsObject().ContainsKey($"/gradio_api/call/v2/{ApiPage.HiddenName}"));
    }

    [Fact]
    public async Task An_undocumented_endpoint_is_described_only_when_the_documentation_asks()
    {
        var plain = Assert.IsType<JsonObject>((await GetAsync("/gradio_api/info", mcp: false))["named_endpoints"]);
        var everything = Assert.IsType<JsonObject>(
            (await GetAsync("/gradio_api/info?all_endpoints=true", mcp: false))["named_endpoints"]);

        Assert.False(plain.ContainsKey($"/{ApiPage.QuietName}"));
        Assert.True(everything.ContainsKey($"/{ApiPage.QuietName}"));

        // Callable and left out of the description are the two halves of the name, so the OpenAPI
        // document — which is the description — leaves it out as well.
        var schema = await GetAsync("/gradio_api/openapi.json", mcp: false);
        Assert.False(schema["paths"]!.AsObject().ContainsKey($"/gradio_api/call/v2/{ApiPage.QuietName}"));
    }

    [Fact]
    public async Task A_derived_endpoint_runs_with_the_types_its_signature_declared()
    {
        var answer = await PostAsync(
            $"/gradio_api/run/{ApiPage.SliceName}",
            new JsonObject { ["data"] = new JsonArray(3, 5, new JsonArray(1, 2, 3, 4, 5)) });

        // The handler added two integers — which it could only do if it was handed integers — and
        // sliced the list with them.
        Assert.Equal("[8,\"4,5\"]", answer["data"]!.ToJsonString());
    }

    /// <summary>
    /// A batched endpoint is described by the list its handler declared, and called with one sample.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Python version builds its <c>Api</c> block from the annotation and leaves batching to
    /// the run, so the documents say <c>array</c> for a parameter declared as a list — which is
    /// what the oracle answers for a batched <c>gr.api</c> function. The sample is what a client
    /// actually sends and what the handler reads, and it is the half that used to be broken: the
    /// value was converted as though it were the whole list, so no batched derived endpoint could
    /// run at all.
    /// </para>
    /// <para>
    /// Both halves are asserted here because they pull in opposite directions — the document is the
    /// list, the conversion is the element — and a build that got either one alone would still pass
    /// the other document's comparison.
    /// </para>
    /// </remarks>
    [Fact]
    public async Task A_batched_endpoint_is_described_by_the_list_and_called_with_one_sample()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks =>
            {
                using (blocks.BeginRender())
                {
                    blocks.Api(
                        new Func<int[], int[]>(Doubled),
                        apiName: "doubled", batch: true);
                }
            });

        var config = JsonNode.Parse(
            await host.Client.GetStringAsync("/config", TestContext.Current.CancellationToken))!.AsObject();

        var components = Assert.IsType<JsonArray>(config["components"]);

        // The one argument and the one return, each described by the list the handler declared.
        Assert.Equal(
            """{"type":"array","items":{"type":"integer"}}""",
            Props(components, 1)["_api_info"]!.ToJsonString());
        Assert.Equal(
            """{"type":"array","items":{"type":"integer"}}""",
            Props(components, 2)["_api_info"]!.ToJsonString());

        var info = JsonNode.Parse(
            await host.Client.GetStringAsync("/gradio_api/info", TestContext.Current.CancellationToken))!.AsObject();

        var endpoint = info["named_endpoints"]!["/doubled"]!;
        Assert.Equal("array", endpoint["parameters"]![0]!["type"]!["type"]!.GetValue<string>());
        Assert.Equal("array", endpoint["returns"]![0]!["type"]!["type"]!.GetValue<string>());

        // One sample in, one sample out: the run hands the handler a batch of one, which is what a
        // client's single call is.
        using var content = new StringContent(
            """{"data":[3]}""",
            System.Text.Encoding.UTF8,
            "application/json");

        using var response = await host.Client.PostAsync(
            "/gradio_api/run/doubled",
            content,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var answer = JsonNode.Parse(
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))!;

        Assert.Equal("[6]", answer["data"]!.ToJsonString());
    }

    /// <summary>Doubles every sample of a batch.</summary>
    /// <param name="values">The samples.</param>
    /// <returns>Each sample, doubled.</returns>
    private static int[] Doubled(int[] values) => [.. values.Select(value => value * 2)];

    [Fact]
    public async Task An_application_that_derives_no_endpoint_offers_none()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks =>
            {
                using (blocks.BeginRender())
                {
                    blocks.Add(new Textbox { Label = "Nothing" });
                }
            });

        using var response = await host.Client.GetAsync("/gradio_api/info", TestContext.Current.CancellationToken);
        var info = JsonNode.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))!;

        Assert.Empty(Assert.IsType<JsonObject>(info["named_endpoints"]));
    }

    private static int[] Ids(JsonArray entries) => [.. entries.Select(entry => entry!.GetValue<int>())];

    private static JsonObject Props(JsonArray components, int id) =>
        components
            .Single(component => component!["id"]!.GetValue<int>() == id)!
            ["props"]!
            .AsObject();

    private static async Task<JsonObject> GetAsync(string path, bool mcp)
    {
        await using var host = await StartAsync(mcp);
        using var response = await host.Client.GetAsync(path, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        return JsonNode.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))!
            .AsObject();
    }

    private static async Task<JsonObject> PostAsync(string path, JsonObject body)
    {
        await using var host = await StartAsync(mcp: false);

        using var content = new StringContent(body.ToJsonString(), Encoding.UTF8, "application/json");
        using var response = await host.Client.PostAsync(path, content, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        return JsonNode.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))!
            .AsObject();
    }

    private static Task<GradioTestHost> StartAsync(bool mcp) =>
        GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => ApiPage.Build(blocks, mcp));
}

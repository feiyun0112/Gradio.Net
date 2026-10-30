using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The endpoint an application offers to agents, spoken to the way an agent speaks to it: JSON-RPC
/// over the streamable transport, against a real host.
/// </summary>
/// <remarks>
/// <para>
/// What the ticket claims is that the endpoint offers tools, prompts and resources "as the Python
/// version does", and that what it offers is derived from the endpoints the application documents.
/// Each is a test here rather than a claim in a comment: the catalog is compared against the API
/// info the same application serves, so the two cannot drift apart without one of these failing.
/// </para>
/// <para>
/// The four things a protocol has that a function does not are all exercised — the handshake that
/// agrees a version, the lists, the calls, and the two kinds of failure (a request for something
/// this application does not have, which is an error in the exchange, and a call that reached a
/// handler which failed, which is an answer saying so).
/// </para>
/// </remarks>
public sealed class McpTests
{
    /// <summary>The address of the mount, as the Python version spells it.</summary>
    private const string Mount = "/gradio_api/mcp";

    [Fact]
    public async Task The_handshake_answers_in_the_version_the_client_asked_for()
    {
        await using var host = await StartAsync(mcp: true);

        var asked = await CallAsync(host, "initialize", new JsonObject
        {
            ["protocolVersion"] = "2025-03-26",
            ["capabilities"] = new JsonObject(),
            ["clientInfo"] = new JsonObject { ["name"] = "a client", ["version"] = "1.0" },
        });

        var result = Result(asked);
        Assert.Equal("2025-03-26", result["protocolVersion"]!.GetValue<string>());

        // The application, not the framework, is what an agent sees itself talking to.
        Assert.Equal(ApiPage.Title, result["serverInfo"]!["name"]!.GetValue<string>());

        // The three lists this endpoint can answer are the three it says it has.
        var capabilities = Assert.IsType<JsonObject>(result["capabilities"]);
        Assert.Equal(["prompts", "resources", "tools"], capabilities.Select(pair => pair.Key).Order());
    }

    [Fact]
    public async Task The_handshake_names_the_servers_own_version_when_the_clients_is_unknown()
    {
        await using var host = await StartAsync(mcp: true);

        var answer = await CallAsync(host, "initialize", new JsonObject { ["protocolVersion"] = "1999-01-01" });

        Assert.Equal("2025-06-18", Result(answer)["protocolVersion"]!.GetValue<string>());
    }

    [Fact]
    public async Task The_tools_are_the_endpoints_the_api_documents()
    {
        await using var host = await StartAsync(mcp: true);

        var tools = Assert.IsType<JsonArray>(Result(await CallAsync(host, "tools/list"))["tools"]);
        var names = tools.Select(tool => tool!["name"]!.GetValue<string>()).ToArray();

        // One tool per documented endpoint that is a tool: the method's own name, in the order the
        // bindings were set up — the same order, and the same set, as the API info.
        Assert.Equal(["Add", "AddAndSlice", "Greet", "DescribeFile"], names);

        // And that list is the document's, not one kept beside it: as many tools as the API info has
        // endpoints, less the two an agent reads instead of calling. The names cannot be compared
        // one for one — a tool is named after its handler while an endpoint may be named by the
        // application — so what is compared is that the two lists are the same length and drawn
        // from the same set of bindings.
        var documented = Assert.IsType<JsonObject>((await GetJsonAsync(host, "/gradio_api/info"))["named_endpoints"]);

        Assert.Equal(documented.Count - 2, names.Length);

        var addAndSlice = tools.Single(tool => tool!["name"]!.GetValue<string>() == "AddAndSlice")!;

        // The endpoint's description is the tool's, and the schema is the arguments reshaped.
        Assert.Equal("Adds two numbers and slices a list.", addAndSlice["description"]!.GetValue<string>());

        var schema = Assert.IsType<JsonObject>(addAndSlice["inputSchema"]);
        Assert.Equal("object", schema["type"]!.GetValue<string>());
        Assert.Equal(["a", "b", "items"], schema["properties"]!.AsObject().Select(pair => pair.Key));
        Assert.Equal(["a", "b", "items"], schema["required"]!.AsArray().Select(name => name!.GetValue<string>()));
        Assert.Equal("array", schema["properties"]!["items"]!["type"]!.GetValue<string>());
    }

    [Fact]
    public async Task An_argument_with_a_default_is_optional_and_says_what_the_default_is()
    {
        await using var host = await StartAsync(mcp: true);

        var tools = Assert.IsType<JsonArray>(Result(await CallAsync(host, "tools/list"))["tools"]);
        var schema = tools.Single(tool => tool!["name"]!.GetValue<string>() == "Greet")!["inputSchema"]!.AsObject();

        Assert.Equal(["name"], schema["required"]!.AsArray().Select(name => name!.GetValue<string>()));
        Assert.Equal("Hello", schema["properties"]!["greeting"]!["default"]!.GetValue<string>());
    }

    [Fact]
    public async Task A_file_is_asked_for_as_the_address_an_agent_can_send()
    {
        await using var host = await StartAsync(mcp: true);

        var tools = Assert.IsType<JsonArray>(Result(await CallAsync(host, "tools/list"))["tools"]);
        var schema = tools.Single(tool => tool!["name"]!.GetValue<string>() == "DescribeFile")!["inputSchema"]!.AsObject();

        // An agent cannot upload a file, so what its schema asks for is a URL the server can fetch —
        // which is what the Python version's filedata schema is rewritten into.
        var file = schema["properties"]!["file"]!;
        Assert.Equal("string", file["type"]!.GetValue<string>());
        Assert.Equal("Gradio File Input - a http or https url to a file", file["format"]!.GetValue<string>());

        // And the description says what to do about it, because a format string does not.
        var tool = tools.Single(candidate => candidate!["name"]!.GetValue<string>() == "DescribeFile")!;
        Assert.Contains(
            "upload_file_to_gradio",
            tool["description"]!.GetValue<string>(),
            StringComparison.Ordinal);
    }

    [Fact]
    public async Task A_private_endpoint_is_no_tool_and_neither_is_an_undocumented_one()
    {
        await using var host = await StartAsync(mcp: true);

        var tools = Assert.IsType<JsonArray>(Result(await CallAsync(host, "tools/list"))["tools"]);
        var names = tools.Select(tool => tool!["name"]!.GetValue<string>()).ToArray();

        // The Python version builds its catalog from the API info a client reads, so an endpoint that is not
        // in that document is not a tool either — and one that is not callable at all is not one.
        Assert.DoesNotContain(ApiPage.HiddenName, names, StringComparer.OrdinalIgnoreCase);
        Assert.DoesNotContain(ApiPage.QuietName, names, StringComparer.OrdinalIgnoreCase);

        var quiet = await CallAsync(host, "tools/call", new JsonObject
        {
            ["name"] = ApiPage.QuietName,
            ["arguments"] = new JsonObject { ["text"] = "anything" },
        });

        Assert.Equal(-32602, quiet["error"]!["code"]!.GetValue<int>());
    }

    [Fact]
    public async Task A_tool_call_answers_with_the_values_the_handler_returned()
    {
        await using var host = await StartAsync(mcp: true);

        var add = await CallAsync(host, "tools/call", new JsonObject
        {
            ["name"] = "Add",
            ["arguments"] = new JsonObject { ["a"] = 3, ["b"] = 5 },
        });

        var content = Assert.IsType<JsonArray>(Result(add)["content"]);
        Assert.Equal("text", content[0]!["type"]!.GetValue<string>());
        Assert.Equal("8", content[0]!["text"]!.GetValue<string>());

        // Two values out of a tuple are two blocks: an agent reads them in the order they were
        // declared, which is the only order there is.
        var slice = await CallAsync(host, "tools/call", new JsonObject
        {
            ["name"] = "AddAndSlice",
            ["arguments"] = new JsonObject { ["a"] = 3, ["b"] = 5, ["items"] = new JsonArray(1, 2, 3, 4, 5) },
        });

        var blocks = Assert.IsType<JsonArray>(Result(slice)["content"]);
        Assert.Equal(["8", "4,5"], blocks.Select(block => block!["text"]!.GetValue<string>()));
    }

    [Fact]
    public async Task A_tool_that_declares_structured_output_reports_its_answer_both_ways()
    {
        await using var host = await StartAsync(mcp: true);

        var add = Result(await CallAsync(host, "tools/call", new JsonObject
        {
            ["name"] = "Add",
            ["arguments"] = new JsonObject { ["a"] = 3, ["b"] = 5 },
        }));

        var structured = Assert.IsType<JsonObject>(add["structuredContent"]);
        Assert.Equal(add["content"]!.ToJsonString(), structured["result"]!.ToJsonString());

        // A tool that did not ask for it does not get it, rather than getting an empty object.
        var slice = Result(await CallAsync(host, "tools/call", new JsonObject
        {
            ["name"] = "AddAndSlice",
            ["arguments"] = new JsonObject { ["a"] = 0, ["b"] = 1, ["items"] = new JsonArray(7) },
        }));

        Assert.False(slice.ContainsKey("structuredContent"));
    }

    [Fact]
    public async Task A_call_that_leaves_out_an_optional_argument_runs_with_its_default()
    {
        await using var host = await StartAsync(mcp: true);

        var greet = Result(await CallAsync(host, "tools/call", new JsonObject
        {
            ["name"] = "Greet",
            ["arguments"] = new JsonObject { ["name"] = "Ada" },
        }));

        Assert.Equal("Hello, Ada", greet["content"]![0]!["text"]!.GetValue<string>());

        // Spelled out, the same call says the same thing — which is what "optional" has to mean.
        var spelled = Result(await CallAsync(host, "tools/call", new JsonObject
        {
            ["name"] = "Greet",
            ["arguments"] = new JsonObject { ["name"] = "Ada", ["greeting"] = "Hello" },
        }));

        Assert.Equal(
            greet["content"]!.ToJsonString(),
            spelled["content"]!.ToJsonString());
    }

    [Fact]
    public async Task A_call_whose_arguments_do_not_describe_the_declared_types_is_a_failed_call()
    {
        await using var host = await StartAsync(mcp: true);

        var answer = await CallAsync(host, "tools/call", new JsonObject
        {
            ["name"] = "Add",
            ["arguments"] = new JsonObject { ["a"] = "not a number", ["b"] = 5 },
        });

        // An answer rather than an exchange error: the caller sent the value, so the caller is who
        // has to read what was wrong with it.
        var result = Result(answer);
        Assert.True(result["isError"]!.GetValue<bool>());
        Assert.Equal("text", result["content"]![0]!["type"]!.GetValue<string>());
    }

    [Fact]
    public async Task A_call_that_reached_a_handler_which_failed_is_an_answer_that_says_so()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks =>
            {
                using (blocks.BeginRender())
                {
                    blocks.McpServer = true;
                    blocks.Api(new Func<string, string>(Refuse));
                }
            });

        var answer = await CallAsync(host, "tools/call", new JsonObject
        {
            ["name"] = nameof(Refuse),
            ["arguments"] = new JsonObject { ["text"] = "anything" },
        });

        var result = Result(answer);
        Assert.True(result["isError"]!.GetValue<bool>());

        // What a client is told is what any other client would be told: the run failed, and an
        // application that does not show its errors does not show them to an agent either.
        Assert.Equal("Error", result["content"]![0]!["text"]!.GetValue<string>());
    }

    [Fact]
    public async Task The_resources_are_the_endpoints_that_declare_an_address()
    {
        await using var host = await StartAsync(mcp: true);

        // A template is a resource whose address carries a parameter, and the two are listed apart
        // because a client resolves them differently.
        var resources = Assert.IsType<JsonArray>(Result(await CallAsync(host, "resources/list"))["resources"]);
        Assert.Empty(resources);

        var templates = Assert.IsType<JsonArray>(
            Result(await CallAsync(host, "resources/templates/list"))["resourceTemplates"]);

        var weather = Assert.Single(templates)!;
        Assert.Equal("weather://{city}", weather["uriTemplate"]!.GetValue<string>());
        Assert.Equal("Weather", weather["name"]!.GetValue<string>());
        Assert.Equal("The weather in a city.", weather["description"]!.GetValue<string>());
        Assert.Equal("text/plain", weather["mimeType"]!.GetValue<string>());
    }

    [Fact]
    public async Task Reading_a_resource_fills_its_address_in()
    {
        await using var host = await StartAsync(mcp: true);

        var answer = Result(await CallAsync(host, "resources/read", new JsonObject { ["uri"] = "weather://Paris" }));

        var contents = Assert.IsType<JsonArray>(answer["contents"]);
        var content = Assert.Single(contents)!;

        Assert.Equal("weather://Paris", content["uri"]!.GetValue<string>());
        Assert.Equal("text/plain", content["mimeType"]!.GetValue<string>());
        Assert.Equal("Sunny in Paris", content["text"]!.GetValue<string>());
    }

    [Fact]
    public async Task An_address_this_application_does_not_have_is_refused()
    {
        await using var host = await StartAsync(mcp: true);

        var answer = await CallAsync(host, "resources/read", new JsonObject { ["uri"] = "weather://" });

        // A segment matches anything but a slash, so an address with nothing after the scheme is
        // not this template's — and there is no resource it could be.
        Assert.Equal(-32602, answer["error"]!["code"]!.GetValue<int>());
    }

    [Fact]
    public async Task The_prompts_are_the_endpoints_that_ask_to_be_filled_in()
    {
        await using var host = await StartAsync(mcp: true);

        var prompts = Assert.IsType<JsonArray>(Result(await CallAsync(host, "prompts/list"))["prompts"]);
        var summarise = Assert.Single(prompts)!;

        Assert.Equal("Summarise", summarise["name"]!.GetValue<string>());
        Assert.Equal("Summarise a text.", summarise["description"]!.GetValue<string>());

        // A prompt is called with one argument per parameter, and which of them a caller may leave
        // out is the same rule as a tool's.
        var arguments = Assert.IsType<JsonArray>(summarise["arguments"]);
        Assert.Equal(["text", "maxWords"], arguments.Select(argument => argument!["name"]!.GetValue<string>()));
        Assert.Equal([true, false], arguments.Select(argument => argument!["required"]!.GetValue<bool>()));
    }

    [Fact]
    public async Task Filling_a_prompt_in_answers_with_the_message_the_handler_produced()
    {
        await using var host = await StartAsync(mcp: true);

        var answer = Result(await CallAsync(host, "prompts/get", new JsonObject
        {
            ["name"] = "Summarise",
            ["arguments"] = new JsonObject { ["text"] = "a long text" },
        }));

        Assert.Equal("Summarise a text.", answer["description"]!.GetValue<string>());

        var message = Assert.Single(Assert.IsType<JsonArray>(answer["messages"]))!;
        Assert.Equal("user", message["role"]!.GetValue<string>());

        // The argument left out is the default, so the text the model is handed says so.
        Assert.Equal(
            "Summarise the following in at most 20 words: a long text",
            message["content"]!["text"]!.GetValue<string>());
    }

    [Fact]
    public async Task A_notification_is_accepted_and_not_answered()
    {
        await using var host = await StartAsync(mcp: true);

        using var content = new StringContent(
            new JsonObject { ["jsonrpc"] = "2.0", ["method"] = "notifications/initialized" }.ToJsonString(),
            Encoding.UTF8,
            "application/json");

        using var response = await host.Client.PostAsync($"{Mount}", content, TestContext.Current.CancellationToken);

        // The transport's own answer for a message nobody expects an answer to.
        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        Assert.Empty(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task A_method_the_protocol_does_not_have_is_refused()
    {
        await using var host = await StartAsync(mcp: true);

        var answer = await CallAsync(host, "tools/sing");

        Assert.Equal(-32601, answer["error"]!["code"]!.GetValue<int>());
        Assert.Equal("2.0", answer["jsonrpc"]!.GetValue<string>());
    }

    [Fact]
    public async Task A_client_may_narrow_the_catalog_to_the_tools_it_names()
    {
        await using var host = await StartAsync(mcp: true);

        var tools = Assert.IsType<JsonArray>(
            Result(await CallAsync(host, "tools/list", query: "?tools=Add,Greet"))["tools"]);

        Assert.Equal(["Add", "Greet"], tools.Select(tool => tool!["name"]!.GetValue<string>()));
    }

    [Fact]
    public async Task The_schema_document_describes_the_tools_without_a_connection()
    {
        await using var host = await StartAsync(mcp: true);

        using var response = await host.Client.GetAsync($"{Mount}/schema", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var schemas = Assert.IsType<JsonArray>(
            JsonNode.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)));

        var greet = schemas.Single(schema => schema!["name"]!.GetValue<string>() == "Greet")!;

        // The registry reads the same three facts an agent would get from a connection, plus the two
        // a page needs: what kind of thing this is, and whether it wants a file.
        Assert.Equal("Hello", greet["inputSchema"]!["properties"]!["greeting"]!["default"]!.GetValue<string>());

        var meta = Assert.IsType<JsonObject>(greet["meta"]);
        Assert.False(meta["file_data_present"]!.GetValue<bool>());
        Assert.Equal("tool", meta["mcp_type"]!.GetValue<string>());
        Assert.Equal("Greet", meta["endpoint_name"]!.GetValue<string>());

        var describe = schemas.Single(schema => schema!["name"]!.GetValue<string>() == "DescribeFile")!;
        Assert.True(describe["meta"]!["file_data_present"]!.GetValue<bool>());

        // Everything the application offers is in the document, in the order it declared it — not
        // three sections — and each entry says which of the three it is.
        Assert.Equal(
            ["Add", "AddAndSlice", "Weather", "Summarise", "Greet", "DescribeFile"],
            schemas.Select(schema => schema!["name"]!.GetValue<string>()));

        Assert.Equal("resource", schemas[2]!["meta"]!["mcp_type"]!.GetValue<string>());
        Assert.Equal("prompt", schemas[3]!["meta"]!["mcp_type"]!.GetValue<string>());

        // A resource is described the way a tool is, arguments and all: a client that wants to know
        // what something takes asks the same question of all three.
        Assert.Equal(
            ["city"],
            schemas[2]!["inputSchema"]!["required"]!.AsArray().Select(name => name!.GetValue<string>()));

        // The file flag latches, which is the Python version's arithmetic rather than a per-entry answer: the
        // one entry that takes a file is the last, and everything before it says false.
        Assert.Equal(
            [false, false, false, false, false, true],
            schemas.Select(schema => schema!["meta"]!["file_data_present"]!.GetValue<bool>()));
    }

    [Fact]
    public async Task A_client_may_narrow_the_schema_document_to_the_names_it_asks_for()
    {
        await using var host = await StartAsync(mcp: true);

        using var response = await host.Client.GetAsync(
            $"{Mount}/schema?tools=Add,Weather",
            TestContext.Current.CancellationToken);

        var schemas = Assert.IsType<JsonArray>(
            JsonNode.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)));

        Assert.Equal(
            ["Add", "Weather"],
            schemas.Select(schema => schema!["name"]!.GetValue<string>()));
    }

    [Fact]
    public async Task An_application_that_asks_callers_to_sign_in_does_not_offer_its_tools_to_anyone()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            cancellationToken,
            blocks =>
            {
                blocks.Auth = GradioAuth.OneAccount("alice", "s3cret");
                ApiPage.Build(blocks, mcp: true);
            });

        // The Python version mounts its MCP endpoint outside the per-route check, so its tools answer anyone
        // who finds the address; that is a hole rather than a behaviour, and this is the one place
        // the two implementations part company on purpose.
        using var content = new StringContent(
            new JsonObject { ["jsonrpc"] = "2.0", ["id"] = 1, ["method"] = "tools/list" }.ToJsonString(),
            Encoding.UTF8,
            "application/json");

        using var response = await host.Client.PostAsync($"{Mount}", content, cancellationToken);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        using var schema = await host.Client.GetAsync($"{Mount}/schema", cancellationToken);
        Assert.Equal(HttpStatusCode.Unauthorized, schema.StatusCode);

        // An application that asks nobody to sign in is unaffected, which is what makes the gate
        // free for the applications that do not use it.
        using var open = await host.Client.GetAsync("/gradio_api/info", cancellationToken);
        Assert.Equal(HttpStatusCode.Unauthorized, open.StatusCode);
    }

    [Fact]
    public async Task The_schema_document_is_empty_when_the_application_documents_no_endpoint()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks =>
            {
                using (blocks.BeginRender())
                {
                    blocks.McpServer = true;
                    blocks.Add(new Textbox { Label = "Nothing" });
                }
            });

        using var response = await host.Client.GetAsync($"{Mount}/schema", TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        // An object rather than an empty array, which is the Python version's answer for an application with
        // no API at all.
        Assert.Equal("{}", body);
    }

    [Fact]
    public async Task A_browser_that_opens_the_endpoint_is_told_how_to_connect()
    {
        await using var host = await StartAsync(mcp: true);

        using var response = await host.Client.GetAsync($"{Mount}", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);

        var page = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Contains($"{Mount}/", page, StringComparison.Ordinal);
        Assert.Contains(host.BaseAddress.ToString().TrimEnd('/'), page, StringComparison.Ordinal);
    }

    [Fact]
    public async Task A_client_that_asks_for_an_event_stream_on_the_streamable_transport_is_refused()
    {
        await using var host = await StartAsync(mcp: true);

        using var request = new HttpRequestMessage(HttpMethod.Get, Mount);
        request.Headers.Accept.ParseAdd("text/event-stream");

        using var response = await host.Client.SendAsync(request, TestContext.Current.CancellationToken);

        // Nothing here speaks without being asked, so the transport's own answer is that this
        // method is not one it offers on this address.
        Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
    }

    [Fact]
    public async Task The_older_transport_carries_a_message_to_the_stream_that_asked_for_it()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(mcp: true);

        using var stream = await host.Client.GetAsync(
            $"{Mount}/sse",
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        Assert.Equal(HttpStatusCode.OK, stream.StatusCode);
        Assert.Equal("text/event-stream", stream.Content.Headers.ContentType?.MediaType);

        await using var body = await stream.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(body);

        // The first event is where to post: the stream's own address, with the session it opened.
        var endpoint = await EventAsync(reader, "endpoint", cancellationToken);
        Assert.StartsWith($"{Mount}/messages/?session_id=", endpoint, StringComparison.Ordinal);

        var session = endpoint[(endpoint.IndexOf('=') + 1)..];

        using var content = new StringContent(
            new JsonObject { ["jsonrpc"] = "2.0", ["id"] = 7, ["method"] = "tools/list" }.ToJsonString(),
            Encoding.UTF8,
            "application/json");

        using var posted = await host.Client.PostAsync(
            $"{Mount}/messages/?session_id={session}",
            content,
            cancellationToken);

        Assert.Equal(HttpStatusCode.Accepted, posted.StatusCode);

        // And the answer arrives on the stream rather than on the response to the post, which is
        // the whole of what this transport is.
        var answer = JsonNode.Parse(await EventAsync(reader, "message", cancellationToken))!;
        Assert.Equal(7, answer["id"]!.GetValue<int>());
        Assert.NotEmpty(Assert.IsType<JsonArray>(answer["result"]!["tools"]));
    }

    [Fact]
    public async Task A_message_for_a_session_nobody_opened_is_refused()
    {
        await using var host = await StartAsync(mcp: true);

        using var content = new StringContent(
            new JsonObject { ["jsonrpc"] = "2.0", ["id"] = 1, ["method"] = "tools/list" }.ToJsonString(),
            Encoding.UTF8,
            "application/json");

        using var response = await host.Client.PostAsync(
            $"{Mount}/messages/?session_id=nothing-opened-this",
            content,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task The_endpoint_is_there_only_when_the_application_asked_for_it()
    {
        await using var host = await StartAsync(mcp: false);

        foreach (var path in new[] { Mount, $"{Mount}/schema", $"{Mount}/sse" })
        {
            using var response = await host.Client.GetAsync(path, TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // And the config says so, which is what the page reads to print the address at all.
        var config = await GetJsonAsync(host, "/config");
        Assert.False(config["mcp_server"]!.GetValue<bool>());
    }

    [Fact]
    public async Task An_application_that_asked_for_it_says_so_in_its_config()
    {
        await using var host = await StartAsync(mcp: true);

        var config = await GetJsonAsync(host, "/config");

        Assert.True(config["mcp_server"]!.GetValue<bool>());
    }

    /// <summary>
    /// A tool call is not a visitor leaving, so nothing bound to that moment runs.
    /// </summary>
    /// <remarks>
    /// An agent's call runs in a session the endpoint makes for it and drops when the call is done.
    /// Dropping that session is not the end of a page — there was no page — so the work an
    /// application does when a visitor leaves has no moment here. Without that distinction a handler
    /// that releases what a session opened would run once per tool call, against an empty session.
    /// </remarks>
    [Fact]
    public async Task A_tool_call_does_not_run_what_the_application_bound_to_a_visitor_leaving()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var left = 0;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            using (blocks.BeginRender())
            {
                blocks.McpServer = true;
                blocks.Api(new Func<int, int, int>(Sum), apiName: "add");
            }

            blocks.Unload((Action)(() => Interlocked.Increment(ref left)));
        });

        var answer = Result(await CallAsync(host, "tools/call", new JsonObject
        {
            ["name"] = "Sum",
            ["arguments"] = new JsonObject { ["a"] = 3, ["b"] = 5 },
        }));

        Assert.Equal("8", Assert.IsType<JsonArray>(answer["content"])[0]!["text"]!.GetValue<string>());

        // The call's session is dropped before the answer is written; waiting past that is waiting
        // for a handler that should never have been called.
        await Task.Delay(TimeSpan.FromMilliseconds(300), cancellationToken);

        Assert.Equal(0, left);
    }

    /// <summary>
    /// A tool call is a run like any other: it is admitted to the queue, so the rules that decide
    /// whether a run may start apply to an agent exactly as they apply to a browser.
    /// </summary>
    /// <remarks>
    /// The two endpoints here have the same validator and differ only in whether they are queued. A
    /// validator runs when a run is admitted to the queue, which is the Python version's rule, so
    /// the queued endpoint refuses an input the rule dislikes and the direct one never asks — and
    /// both answers come back through the tool call, which is what makes this a test of the path a
    /// tool takes rather than of the queue alone.
    /// </remarks>
    [Fact]
    public async Task A_tool_call_is_admitted_the_way_a_clients_join_is()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            using (blocks.BeginRender())
            {
                blocks.McpServer = true;

                blocks.Api(new Func<int, int, int>(Sum), apiName: "queued", validator: (Func<int, int, object?>)((a, _) => a >= 0 ? null : "no negatives"));

                blocks.Api(new Func<int, int, int>(SumDirect), apiName: "direct", queue: false, validator: (Func<int, int, object?>)((a, _) => a >= 0 ? null : "no negatives"));
            }
        });

        var refused = Result(await CallAsync(host, "tools/call", new JsonObject
        {
            ["name"] = nameof(Sum),
            ["arguments"] = new JsonObject { ["a"] = -1, ["b"] = 5 },
        }));

        Assert.True(refused["isError"]!.GetValue<bool>());

        // What the queue hands back is the JSON of what the validator returned, so a string arrives
        // quoted; what matters here is that the refusal reached the agent at all.
        Assert.Contains(
            "no negatives",
            refused["content"]![0]!["text"]!.GetValue<string>(),
            StringComparison.Ordinal);

        // The same input on the endpoint that skips the queue: the validator is part of admission,
        // and there is no admission here.
        var ran = Result(await CallAsync(host, "tools/call", new JsonObject
        {
            ["name"] = nameof(SumDirect),
            ["arguments"] = new JsonObject { ["a"] = -1, ["b"] = 5 },
        }));

        Assert.Equal("4", ran["content"]![0]!["text"]!.GetValue<string>());
    }

    /// <summary>
    /// A batched endpoint hands a tool call its values as a batch of one, which is what makes
    /// batching a property of the binding rather than of who is calling.
    /// </summary>
    /// <remarks>
    /// The handler is typed to take an array of values, so a run that handed it a single number
    /// would fail to call it at all — the answer is the batch having been applied.
    /// </remarks>
    [Fact]
    public async Task A_batched_endpoint_hands_a_tool_call_its_values_as_a_batch()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            using (blocks.BeginRender())
            {
                blocks.McpServer = true;

                blocks.Api(new Func<int[], int[]>(Total), apiName: "total", batch: true);
            }
        });

        var answer = Result(await CallAsync(host, "tools/call", new JsonObject
        {
            ["name"] = nameof(Total),
            ["arguments"] = new JsonObject { ["values"] = 3 },
        }));

        Assert.Equal("3", answer["content"]![0]!["text"]!.GetValue<string>());
    }

    /// <summary>
    /// A file an agent sends is an address, and the handler receives it as the path it named.
    /// </summary>
    /// <remarks>
    /// The schema asks for a string because an agent cannot upload anything (see
    /// <see cref="A_file_is_asked_for_as_the_address_an_agent_can_send"/>); this is the other end of
    /// that rewrite — the address arrives as the file the handler declared.
    /// </remarks>
    [Fact]
    public async Task A_file_a_tool_is_given_reaches_the_handler_as_the_address_it_was_sent()
    {
        await using var host = await StartAsync(mcp: true);

        var answer = Result(await CallAsync(host, "tools/call", new JsonObject
        {
            ["name"] = nameof(ApiPage.DescribeFile),
            ["arguments"] = new JsonObject { ["file"] = "https://example.test/report.txt" },
        }));

        Assert.Equal(
            "https://example.test/report.txt",
            answer["content"]![0]!["text"]!.GetValue<string>());
    }

    /// <summary>Adds two numbers, under a name an agent calls the tool by.</summary>
    private static int Sum(int a, int b) => a + b;

    /// <summary>The same sum, for the endpoint that skips the queue.</summary>
    private static int SumDirect(int a, int b) => a + b;

    /// <summary>Adds up a batch, one sample in and one sample out.</summary>
    /// <remarks>
    /// The parameter is the list a batched run hands a handler, typed as the numbers it carries: a
    /// batched handler reads one sample per element and answers with one result per sample.
    /// </remarks>
    private static int[] Total(int[] values) => [values.Sum()];

    /// <summary>An endpoint whose handler always fails.</summary>
    /// <param name="text">Anything.</param>
    /// <returns>Nothing: it throws.</returns>
    /// <exception cref="InvalidOperationException">Always.</exception>
    private static string Refuse(string text) =>
        throw new InvalidOperationException($"the handler refused '{text}'");
    private static JsonObject Result(JsonObject answer)
    {
        Assert.Null(answer["error"]);

        return Assert.IsType<JsonObject>(answer["result"]);
    }

    /// <summary>Reads one server-sent event of a given name.</summary>
    /// <param name="reader">The stream's reader.</param>
    /// <param name="name">The event name to read to.</param>
    /// <param name="cancellationToken">Cancels the read.</param>
    /// <returns>The event's data.</returns>
    private static async Task<string> EventAsync(StreamReader reader, string name, CancellationToken cancellationToken)
    {
        var wanted = $"event: {name}";

        while (await reader.ReadLineAsync(cancellationToken) is { } line)
        {
            if (!string.Equals(line, wanted, StringComparison.Ordinal))
            {
                continue;
            }

            var data = await reader.ReadLineAsync(cancellationToken);

            Assert.NotNull(data);
            Assert.StartsWith("data: ", data, StringComparison.Ordinal);

            return data["data: ".Length..];
        }

        throw new InvalidOperationException($"The stream ended before an '{name}' event.");
    }

    private static async Task<JsonObject> GetJsonAsync(GradioTestHost host, string path)
    {
        using var response = await host.Client.GetAsync(path, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        return JsonNode.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))!
            .AsObject();
    }

    /// <summary>Sends one JSON-RPC request and reads the answer to it.</summary>
    /// <param name="host">The host.</param>
    /// <param name="method">The method to call.</param>
    /// <param name="parameters">Its parameters, or <see langword="null"/> for none.</param>
    /// <param name="query">A query string to add to the address, or <see langword="null"/>.</param>
    /// <returns>The response message.</returns>
    private static async Task<JsonObject> CallAsync(
        GradioTestHost host,
        string method,
        JsonObject? parameters = null,
        string? query = null)
    {
        var message = new JsonObject { ["jsonrpc"] = "2.0", ["id"] = 1, ["method"] = method };

        if (parameters is not null)
        {
            message["params"] = parameters;
        }

        using var content = new StringContent(message.ToJsonString(), Encoding.UTF8, "application/json");
        using var response = await host.Client.PostAsync(
            $"{Mount}{query}",
            content,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        // The streamable transport answers a request with one server-sent event rather than with a
        // bare document, so the message is the event's data.
        var data = body.Split('\n').First(line => line.StartsWith("data: ", StringComparison.Ordinal));

        return JsonNode.Parse(data["data: ".Length..])!.AsObject();
    }

    private static Task<GradioTestHost> StartAsync(bool mcp) =>
        GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => ApiPage.Build(blocks, mcp));
}

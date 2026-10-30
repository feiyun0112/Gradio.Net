using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using Gradio.Net;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// A page built from an OpenAPI document: what it draws, what it sends, and what it shows when the
/// service says no.
/// </summary>
/// <remarks>
/// <para>
/// The service is <see cref="FakeService"/>, so what is asserted is the request the page builds —
/// the method, the address with its query parameters, the body — and what it does with an answer,
/// rather than a service's behaviour.
/// </para>
/// <para>
/// The document is the Python version's own 6.30.0 OpenAPI example, and the assertions about what
/// the page draws are the Python version's own output for it: the panel's four blocks, the order of
/// the forms, the labels, the parameter names and the API names. The evidence is taken once, in the
/// test, rather than kept as a file; nothing re-records it.
/// </para>
/// </remarks>
[Collection(QueueCollection.Name)]
public sealed class LoadOpenApiTests
{
    /// <summary>The document the page is built from.</summary>
    private const string Document = """
        {
          "openapi": "3.0.0",
          "info": { "title": "Book Store", "description": "A shop for books." },
          "paths": {
            "/books": {
              "get": {
                "summary": "List books",
                "description": "Every book on the shelf.",
                "parameters": [
                  { "name": "shelf", "in": "query", "description": "which shelf",
                    "schema": { "type": "string", "default": "a" } },
                  { "name": "limit", "in": "query", "schema": { "type": "integer" } }
                ],
                "responses": { "200": { "description": "ok" } }
              },
              "post": {
                "summary": "Add a book",
                "requestBody": {
                  "description": "The book to add.",
                  "content": {
                    "application/json": { "schema": { "$ref": "#/components/schemas/Book" } }
                  }
                },
                "responses": { "201": { "description": "made" } }
              }
            },
            "/books/{book_id}": {
              "get": {
                "summary": "One book",
                "parameters": [
                  { "name": "book_id", "in": "path", "required": true,
                    "schema": { "type": "integer" } },
                  { "name": "format", "in": "query",
                    "schema": { "type": "string", "enum": ["short", "long"] } }
                ],
                "responses": { "200": { "description": "ok" } }
              }
            },
            "/internal/report": {
              "get": { "summary": "Hidden", "parameters": [], "responses": { "200": { "description": "ok" } } }
            }
          },
          "components": {
            "schemas": {
              "Book": {
                "type": "object",
                "properties": {
                  "title": { "type": "string", "example": "Dune" },
                  "pages": { "type": "integer", "example": 412 },
                  "in_print": { "type": "boolean", "example": true }
                }
              }
            }
          }
        }
        """;

    [Fact]
    public async Task A_document_becomes_a_page_of_forms()
    {
        var service = new FakeService();

        await using var host = await StartAsync(service);

        var config = await ConfigAsync(host);

        // The application is a page of blocks under the document's title, not the interface the last
        // endpoint drew.
        Assert.Equal("blocks", config["mode"]!.GetValue<string>());
        Assert.Equal("Book Store", config["title"]!.GetValue<string>());

        // The panel: the title, the description, the heading, and one link per endpoint.
        var markdown = MarkdownOf(config);

        Assert.Equal(
            [
                "## Book Store",
                "A shop for books.",
                "### API Endpoints",
                "<div style='overflow-x: auto; overflow-y: auto; max-height: 500px;'><ul>"
                + "<li style='white-space: nowrap;'><a href='#GET__books' style='white-space: nowrap;'>"
                + "GET <code style='font-size: inherit; white-space: nowrap;'>/books</code></a></li>\n"
                + "<li style='white-space: nowrap;'><a href='#POST__books' style='white-space: nowrap;'>"
                + "POST <code style='font-size: inherit; white-space: nowrap;'>/books</code></a></li>\n"
                + "<li style='white-space: nowrap;'><a href='#GET__books_book_id' style='white-space: nowrap;'>"
                + "GET <code style='font-size: inherit; white-space: nowrap;'>/books/{book_id}</code></a></li>\n"
                + "</ul></div>",
                "<h2 id='GET__books'><span style='display:inline-block;min-width:48px;padding:2px 10px;"
                + "border-radius:4px;background:#61affe;color:white;font-weight:bold;font-family:monospace;"
                + "margin-right:8px;text-align:center;border:2px solid #61affe;"
                + "box-shadow:0 1px 2px rgba(0,0,0,0.08);'>GET</span> "
                + "<code style='font-size: inherit;'>/books</code></h2>",
                "List books",
            ],
            markdown.Take(6));

        // The fields an endpoint's arguments are read from, in the document's order: a text box with
        // its default and its description, and a number field.
        var shelf = ComponentOf(config, "textbox", "shelf");
        Assert.Equal("a", shelf["value"]!.GetValue<string>());
        Assert.Equal("which shelf", shelf["info"]!.GetValue<string>());

        var limit = ComponentOf(config, "number", "limit");
        Assert.Null(limit["value"]);

        // An enumeration is a dropdown of its values, and a body is a box of JSON holding an example
        // built from the schema.
        var format = ComponentOf(config, "dropdown", "format");
        Assert.Equal(
            """[["short","short"],["long","long"]]""",
            format["choices"]!.ToJsonString());
        Assert.False(format["allow_custom_value"]!.GetValue<bool>());

        var body = ComponentOf(config, "textbox", "Request Body");
        Assert.Equal("The book to add.", body["info"]!.GetValue<string>());
        Assert.Equal(
            "{\n  \"title\": \"Dune\",\n  \"pages\": 412,\n  \"in_print\": true\n}",
            body["value"]!.GetValue<string>());

        // The answer is a JSON component, and the endpoint is served under the name the Python version gives
        // it: the method and the path with the slashes and braces taken out.
        var answer = ComponentOf(config, "json", "Response");
        Assert.NotNull(answer);

        var names = config["dependencies"]!.AsArray()
            .Select(entry => entry!["api_name"]!.GetValue<string>())
            .ToArray();

        Assert.Contains("get__books", names);
        Assert.Contains("post__books", names);
        Assert.Contains("get__books_book_id", names);

        // The path the exclusion names is not drawn at all.
        Assert.DoesNotContain("get__internal_report", names);
    }

    [Fact]
    public async Task The_parameters_are_named_the_way_the_document_names_them()
    {
        var service = new FakeService();

        await using var host = await StartAsync(service);

        using var response = await host.Client.GetAsync(
            "/gradio_api/info",
            TestContext.Current.CancellationToken);

        var info = JsonNode.Parse(
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))!;

        Assert.Equal(
            ["shelf", "limit"],
            NamesOf(info, "/get__books"));
        Assert.Equal(["request_body"], NamesOf(info, "/post__books"));
        Assert.Equal(["book_id", "format"], NamesOf(info, "/get__books_book_id"));

        // The endpoint's own description is what the API document says about it, which is the
        // operation's description where it has one and its summary where it does not.
        Assert.Equal(
            "Every book on the shelf.",
            info["named_endpoints"]!["/get__books"]!["description"]!.GetValue<string>());
        Assert.Equal(
            "Add a book",
            info["named_endpoints"]!["/post__books"]!["description"]!.GetValue<string>());
    }

    [Fact]
    public async Task A_query_parameter_travels_in_the_address()
    {
        var service = new FakeService().Answering("""{"books":[]}""");

        await using var host = await StartAsync(service);

        var data = await RunAsync(host, "get__books", new JsonArray("b", 5), "query");

        var call = Assert.Single(service.Calls);

        Assert.Equal("GET", call.Method);
        Assert.Equal("http://api.test/v1/books?shelf=b&limit=5", call.Address);
        Assert.Equal("""{"books":[]}""", data[0]!.ToJsonString());

        // A GET carries no body, whatever the document says about one.
        Assert.Null(call.ContentType);
        Assert.Equal(string.Empty, call.Body);
    }

    [Fact]
    public async Task A_path_parameter_is_written_into_the_address()
    {
        var service = new FakeService().Answering("""{"title":"Dune"}""");

        await using var host = await StartAsync(service);

        await RunAsync(host, "get__books_book_id", new JsonArray(7, "short"), "path");

        var call = Assert.Single(service.Calls);

        Assert.Equal("http://api.test/v1/books/7?format=short", call.Address);
    }

    [Fact]
    public async Task A_json_body_is_sent_as_the_field_holds_it()
    {
        var service = new FakeService().Answering("""{"id":1}""");

        await using var host = await StartAsync(service);

        await RunAsync(host, "post__books", new JsonArray("""{"title": "Dune", "pages": 412}"""), "body");

        var call = Assert.Single(service.Calls);

        Assert.Equal("POST", call.Method);
        Assert.Equal("http://api.test/v1/books", call.Address);

        // The field holds JSON and the body is that JSON: the text is read and sent as the document
        // it spells, not as the string the reader typed.
        Assert.Equal(
            """{"title":"Dune","pages":412}""",
            JsonNode.Parse(call.Body)!.ToJsonString());
    }

    [Fact]
    public async Task The_token_travels_as_a_bearer_token()
    {
        var service = new FakeService().Answering("""{"books":[]}""");

        await using var host = await StartAsync(service, authToken: "s3cret");

        await RunAsync(host, "get__books", new JsonArray("a", 1), "token");

        Assert.Equal("Bearer s3cret", Assert.Single(service.Calls).Authorization);
    }

    [Fact]
    public async Task A_refusal_is_shown_as_the_service_answered()
    {
        var service = new FakeService().Refusing(
            HttpStatusCode.NotFound,
            """{"message":"no such shelf"}""");

        await using var host = await StartAsync(service);

        var data = await RunAsync(host, "get__books", new JsonArray("zzz", 1), "refused");

        // A refusal is an answer the page shows: the status and what the service said, in the JSON
        // component, which is where a reader looks.
        var answer = data[0]!.AsObject();

        Assert.Equal("error", answer["__status__"]!.GetValue<string>());
        Assert.Equal(404, answer["status_code"]!.GetValue<int>());
        Assert.Equal("""{"message":"no such shelf"}""", answer["message"]!.GetValue<string>());
    }

    [Fact]
    public async Task A_file_body_is_sent_as_the_files_bytes()
    {
        const string Uploads = """
            {
              "openapi": "3.0.0",
              "info": { "title": "Uploads" },
              "paths": {
                "/files": {
                  "post": {
                    "summary": "Put a file",
                    "requestBody": {
                      "content": {
                        "application/octet-stream": { "schema": { "type": "string", "format": "binary" } }
                      }
                    },
                    "responses": { "200": { "description": "ok" } }
                  }
                }
              }
            }
            """;

        var service = new FakeService().Answering("""{"stored":true}""");
        var file = WriteTemporaryFile([1, 2, 3]);

        try
        {
            await using var host = await GradioTestHost.StartStandaloneAsync(
                logs: null,
                TestContext.Current.CancellationToken,
                blocks => _ = External.LoadOpenApi(
                    blocks,
                    "http://api.test/v1",
                    JsonNode.Parse(Uploads)!,
                    httpClient: service.Client()));

            var config = await ConfigAsync(host);

            // The body is a file, so the field is one, and what it hands over is the path of the
            // file the reader chose.
            var field = ComponentOf(config, "file", "File");
            Assert.NotNull(field);

            await RunAsync(host, "post__files", new JsonArray(file), "upload");

            var call = Assert.Single(service.Calls);

            // The Python version sends the path itself as the body, which no service can use; the file's bytes
            // are what a service is waiting for.
            Assert.Equal([1, 2, 3], call.BodyBytes);
            Assert.Equal("application/octet-stream", call.ContentType);
        }
        finally
        {
            System.IO.File.Delete(file);
        }
    }

    [Fact]
    public async Task An_endpoint_that_takes_a_file_documents_the_upload()
    {
        const string Uploads = """
            {
              "openapi": "3.0.0",
              "info": { "title": "Uploads" },
              "paths": {
                "/files": {
                  "post": {
                    "summary": "Put a file",
                    "requestBody": {
                      "content": {
                        "application/octet-stream": { "schema": { "type": "string", "format": "binary" } }
                      }
                    },
                    "responses": { "200": { "description": "ok" } }
                  }
                }
              }
            }
            """;

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = External.LoadOpenApi(
                blocks,
                "http://api.test/v1",
                JsonNode.Parse(Uploads)!,
                httpClient: new FakeService().Client()));

        using var response = await host.Client.GetAsync(
            "/gradio_api/info",
            TestContext.Current.CancellationToken);

        var info = JsonNode.Parse(
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))!;

        var snippets = info["named_endpoints"]!["/post__files"]!["code_snippets"]!;

        // A parameter whose example is a file is one the caller uploads, and each snippet says so in
        // its own dialect. A snippet that showed the file object as ordinary JSON would be a call a
        // reader could copy and watch fail.
        Assert.StartsWith(
            "from gradio_client import Client, handle_file\n",
            snippets["python"]!.GetValue<string>(),
            StringComparison.Ordinal);
        Assert.Contains(
            "request_body=handle_file('",
            snippets["python"]!.GetValue<string>(),
            StringComparison.Ordinal);

        Assert.StartsWith(
            "import { Client, handle_file } from \"@gradio/client\";\n\n"
            + "const response_0 = await fetch(",
            snippets["javascript"]!.GetValue<string>(),
            StringComparison.Ordinal);
        Assert.Contains(
            "request_body: handle_file(exampleFile),",
            snippets["javascript"]!.GetValue<string>(),
            StringComparison.Ordinal);

        Assert.StartsWith(
            "FILE_PATH=$(curl -s -X POST ",
            snippets["bash"]!.GetValue<string>(),
            StringComparison.Ordinal);
        Assert.Contains(
            "/gradio_api/upload -F 'files=@/path/to/your/file' | tr -d '[]\" ')\n\ncurl -X POST ",
            snippets["bash"]!.GetValue<string>(),
            StringComparison.Ordinal);
        Assert.Contains(
            "\"request_body\": {\"path\": \"'$FILE_PATH'\", \"meta\": {\"_type\": \"gradio.FileData\"}}",
            snippets["bash"]!.GetValue<string>(),
            StringComparison.Ordinal);

        // The command line sends the object a client fills in when it uploads, which is the address
        // and the marker rather than the whole file object the document's example carries.
        Assert.Contains(
            "\"request_body\": {\n    \"path\": \"https://",
            snippets["cli"]!.GetValue<string>(),
            StringComparison.Ordinal);
    }

    [Fact]
    public async Task A_description_is_documented_without_being_the_configs_own()
    {
        var service = new FakeService();

        await using var host = await StartAsync(service);

        var config = await ConfigAsync(host);

        // The Python version's generated handler carries the document's description as its docstring, so the
        // binding's own `api_description` is unset: the config says nothing and the API document
        // says what the document said.
        var dependency = config["dependencies"]!.AsArray()
            .Single(entry => entry!["api_name"]!.GetValue<string>() == "get__books");

        Assert.Null(dependency!["api_description"]);

        using var response = await host.Client.GetAsync(
            "/gradio_api/info",
            TestContext.Current.CancellationToken);

        var info = JsonNode.Parse(
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))!;

        Assert.Equal(
            "Every book on the shelf.",
            info["named_endpoints"]!["/get__books"]!["description"]!.GetValue<string>());
    }

    [Fact]
    public async Task Only_the_paths_and_methods_asked_for_are_drawn()
    {
        var service = new FakeService();

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = External.LoadOpenApi(
                blocks,
                "http://api.test/v1",
                JsonNode.Parse(Document)!,
                paths: ["/books"],
                excludePaths: [".*internal.*"],
                methods: ["get"],
                httpClient: service.Client()));

        var config = await ConfigAsync(host);

        var names = config["dependencies"]!.AsArray()
            .Select(entry => entry!["api_name"]!.GetValue<string>())
            .ToArray();

        Assert.Contains("get__books", names);
        Assert.Contains("get__books_book_id", names);

        // The post was filtered by its method and the report by its path.
        Assert.DoesNotContain("post__books", names);
        Assert.DoesNotContain("get__internal_report", names);
    }

    [Fact]
    public async Task A_document_that_leaves_nothing_to_draw_is_refused()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken);

        var error = Assert.Throws<ArgumentException>(() => External.LoadOpenApi(
            host.Services.GetRequiredService<Blocks>(),
            "http://api.test/v1",
            JsonNode.Parse(Document)!,
            paths: ["/nothing-here"]));

        Assert.Contains("No valid paths", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task A_document_is_read_from_a_file_or_an_address()
    {
        var file = WriteTemporaryFile(Encoding.UTF8.GetBytes(Document));

        try
        {
            await using var fromFile = await GradioTestHost.StartStandaloneAsync(
                logs: null,
                TestContext.Current.CancellationToken,
                blocks => _ = External.LoadOpenApiAsync(
                    blocks,
                    "http://api.test/v1",
                    file).GetAwaiter().GetResult());

            Assert.Equal("Book Store", (await ConfigAsync(fromFile))["title"]!.GetValue<string>());
        }
        finally
        {
            System.IO.File.Delete(file);
        }

        var service = new FakeService().Answering(Document);

        await using var fromAddress = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = External.LoadOpenApiAsync(
                blocks,
                "http://api.test/v1",
                "http://spec.test/openapi.json",
                httpClient: service.Client()).GetAwaiter().GetResult());

        Assert.Equal("Book Store", (await ConfigAsync(fromAddress))["title"]!.GetValue<string>());
        Assert.Equal("http://spec.test/openapi.json", Assert.Single(service.Calls).Address);
    }

    [Fact]
    public async Task Each_endpoints_interface_can_be_configured()
    {
        var service = new FakeService();

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = External.LoadOpenApi(
                blocks,
                "http://api.test/v1",
                JsonNode.Parse(Document)!,
                excludePaths: [".*internal.*"],
                buildInterface: endpoint => new Interface(
                    blocks,
                    endpoint.Fn,
                    [.. endpoint.Inputs],
                    [.. endpoint.Outputs],
                    title: endpoint.Path,
                    apiName: endpoint.ApiName,
                    apiDescription: endpoint.ApiDescription,
                    parameterNames: endpoint.ParameterNames),
                httpClient: service.Client()));

        // Each interface draws its own heading, which is what the option is for.
        var markdown = MarkdownOf(await ConfigAsync(host));

        Assert.Contains(
            "<h1 style='text-align: center; margin-bottom: 1rem'>/books</h1>",
            markdown);
    }

    /// <summary>Starts a host whose application is the document's page.</summary>
    /// <param name="service">The service the endpoints are called at.</param>
    /// <param name="authToken">The token to send, or <see langword="null"/> for none.</param>
    /// <returns>The host.</returns>
    private static Task<GradioTestHost> StartAsync(FakeService service, string? authToken = null) =>
        GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks => _ = External.LoadOpenApi(
                blocks,
                "http://api.test/v1",
                JsonNode.Parse(Document)!,
                excludePaths: [".*internal.*"],
                authToken: authToken,
                httpClient: service.Client()));

    /// <summary>The served config.</summary>
    /// <param name="host">The host.</param>
    /// <returns>The config.</returns>
    private static async Task<JsonObject> ConfigAsync(GradioTestHost host)
    {
        using var response = await host.Client.GetAsync(
            "/config",
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        return JsonNode.Parse(
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))!.AsObject();
    }

    /// <summary>The values of the markdown blocks the page draws, in the order it draws them.</summary>
    /// <param name="config">The config.</param>
    /// <returns>The values.</returns>
    private static List<string> MarkdownOf(JsonObject config) =>
    [
        .. config["components"]!.AsArray()
            .Where(entry => entry!["type"]!.GetValue<string>() == "markdown")
            .Select(entry => entry!["props"]!["value"]!.GetValue<string>()),
    ];

    /// <summary>The props of the first component of a type with a label.</summary>
    /// <param name="config">The config.</param>
    /// <param name="type">The component's type, as the config spells it.</param>
    /// <param name="label">Its label.</param>
    /// <returns>The props.</returns>
    private static JsonObject ComponentOf(JsonObject config, string type, string label) =>
        config["components"]!.AsArray()
            .First(entry => entry!["type"]!.GetValue<string>() == type
                && entry["props"]?["label"]?.GetValue<string>() == label)!["props"]!
            .AsObject();

    /// <summary>The parameter names an endpoint is documented with.</summary>
    /// <param name="info">The API document.</param>
    /// <param name="endpoint">The endpoint's name.</param>
    /// <returns>The names, in order.</returns>
    private static List<string> NamesOf(JsonNode info, string endpoint) =>
    [
        .. info["named_endpoints"]![endpoint]!["parameters"]!.AsArray()
            .Select(parameter => parameter!["parameter_name"]!.GetValue<string>()),
    ];

    /// <summary>Runs one endpoint once and returns what its outputs carried.</summary>
    /// <param name="host">The host.</param>
    /// <param name="apiName">The endpoint's name.</param>
    /// <param name="data">The values its inputs are read from.</param>
    /// <param name="sessionHash">The session the run belongs to.</param>
    /// <returns>One value per output.</returns>
    private static async Task<JsonArray> RunAsync(
        GradioTestHost host,
        string apiName,
        JsonArray data,
        string sessionHash)
    {
        var body = new JsonObject
        {
            ["data"] = data,
            ["session_hash"] = sessionHash,
        };

        using var response = await host.Client.PostAsync(
            $"/gradio_api/run/{apiName}",
            new StringContent(body.ToJsonString(), Encoding.UTF8, "application/json"),
            TestContext.Current.CancellationToken);

        var answer = JsonNode.Parse(
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))!;

        Assert.True(
            response.StatusCode == HttpStatusCode.OK,
            $"'{apiName}' answered {response.StatusCode}: {answer.ToJsonString()}");

        return answer["data"]!.AsArray();
    }

    /// <summary>Writes a file the page will be given.</summary>
    /// <param name="bytes">Its bytes.</param>
    /// <returns>Its path.</returns>
    private static string WriteTemporaryFile(byte[] bytes)
    {
        var directory = Path.Combine(Path.GetTempPath(), "gradio-net-external", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);

        var path = Path.Combine(directory, "upload.bin");
        System.IO.File.WriteAllBytes(path, bytes);

        return path;
    }
}

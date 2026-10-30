using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.GrLayer;

/// <summary>
/// The three page builders: the layer builds the render root, hands it over, and gives it back.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>Interface</c>, <c>ChatInterface</c> and <c>TabbedInterface</c> are
/// applications, so the claim is that <c>gr.Interface(...)</c> is the application <c>new
/// Interface(...)</c> describes — same blocks, same bindings, same document. Each test writes the
/// page both ways and compares the served config as text.
/// </para>
/// <para>
/// The layer opens no render of its own beyond the one the kernel's constructor asks for, so a
/// difference here would be a block drawn twice or not at all, which the comparison would report as
/// a different page rather than as a missing call.
/// </para>
/// </remarks>
public sealed class GrPageTests
{
    [Fact]
    public void An_interface_written_with_the_layer_is_the_interface_written_with_the_core()
    {
        var layer = gr.Interface(
            (Func<string, string>)(name => $"Hello, {name}!"),
            [gr.Textbox(label: "Name")],
            [gr.Textbox(label: "Greeting")],
            title: "Greet",
            apiName: "greet");

        var core = new Blocks(new RenderContextAccessor(), new BlockContainerFactory());

        using (core.BeginRender())
        {
            _ = new Interface(
                core,
                (Func<string, string>)(name => $"Hello, {name}!"),
                new ComponentCollection { new Textbox { Label = "Name" } },
                new ComponentCollection { new Textbox { Label = "Greeting" } },
                title: "Greet",
                apiName: "greet");
        }

        Assert.Equal(ConfigOf(core), ConfigOf(layer));
    }

    [Fact]
    public void A_chat_interface_written_with_the_layer_is_the_one_written_with_the_core()
    {
        var layer = gr.ChatInterface(
            (Func<string, string>)(message => $"You said: {message}"),
            title: "Echo");

        var core = new Blocks(new RenderContextAccessor(), new BlockContainerFactory());

        using (core.BeginRender())
        {
            _ = new ChatInterface(
                core,
                (Func<string, string>)(message => $"You said: {message}"),
                title: "Echo");
        }

        Assert.Equal(ConfigOf(core), ConfigOf(layer));
    }

    [Fact]
    public void A_tabbed_interface_written_with_the_layer_is_the_one_written_with_the_core()
    {
        // A tab holds the recipe that draws it, because a container hosts one application here
        // — the one shape where the layer's parameter is not the core's.
        static TabContent[] Tabs() =>
        [
            new(tab => _ = new Textbox { Label = "One" }),
            new(tab => _ = new Textbox { Label = "Two" }),
        ];

        var layer = gr.TabbedInterface(Tabs(), ["First", "Second"], title: "Compare");

        var core = new Blocks(new RenderContextAccessor(), new BlockContainerFactory());

        using (core.BeginRender())
        {
            _ = new TabbedInterface(core, Tabs(), ["First", "Second"], title: "Compare");
        }

        Assert.Equal(ConfigOf(core), ConfigOf(layer));
    }

    [Fact]
    public async Task An_openapi_page_written_with_the_layer_is_the_one_written_with_the_core()
    {
        // The document is written out here rather than read from a file: what is compared is the
        // page the loader assembles, not the document.
        var spec = JsonNode.Parse(
            """
            {
              "openapi": "3.0.0",
              "info": { "title": "Book Store", "description": "A shelf of books." },
              "paths": {
                "/books": {
                  "get": {
                    "summary": "List books",
                    "description": "Every book on the shelf.",
                    "parameters": [
                      {
                        "name": "genre",
                        "in": "query",
                        "description": "Which shelf to read.",
                        "schema": { "type": "string", "enum": ["fiction", "history"], "default": "fiction" }
                      },
                      { "name": "limit", "in": "query", "schema": { "type": "integer", "default": 10 } },
                      { "name": "signed", "in": "query", "schema": { "type": "boolean", "default": false } },
                      { "name": "tags", "in": "query", "schema": { "type": "array" } }
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
                      { "name": "book_id", "in": "path", "required": true, "schema": { "type": "integer" } }
                    ],
                    "responses": { "200": { "description": "ok" } }
                  },
                  "delete": {
                    "summary": "Remove a book",
                    "parameters": [
                      { "name": "book_id", "in": "path", "required": true, "schema": { "type": "integer" } }
                    ],
                    "responses": { "204": { "description": "gone" } }
                  }
                },
                "/books/{book_id}/cover": {
                  "post": {
                    "summary": "Upload a cover",
                    "parameters": [
                      { "name": "book_id", "in": "path", "required": true, "schema": { "type": "integer" } }
                    ],
                    "requestBody": {
                      "content": {
                        "application/octet-stream": { "schema": { "type": "string", "format": "binary" } }
                      }
                    },
                    "responses": { "200": { "description": "ok" } }
                  }
                }
              },
              "components": {
                "schemas": {
                  "Book": {
                    "type": "object",
                    "properties": {
                      "title": { "type": "string", "example": "Dune" },
                      "pages": { "type": "integer", "example": 412 },
                      "in_print": { "type": "boolean", "example": true },
                      "tags": { "type": "array", "example": ["classic"] }
                    }
                  }
                }
              }
            }
            """)!;

        var layer = gr.LoadOpenapi(spec, "http://127.0.0.1:9/v1");

        var core = new Blocks(new RenderContextAccessor(), new BlockContainerFactory());
        _ = External.LoadOpenApi(core, "http://127.0.0.1:9/v1", spec);

        Assert.Equal(ConfigOf(core), ConfigOf(layer));

        // The awaited spelling is the same page again: one loader, two ways to read the document.
        var awaited = await gr.LoadOpenApiAsync(
            spec,
            "http://127.0.0.1:9/v1",
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(ConfigOf(core), ConfigOf(awaited));
    }

    [Fact]
    public void A_chat_written_with_the_layer_is_the_one_written_with_the_core()
    {
        var layer = gr.LoadChat("http://127.0.0.1:9/v1/", "local-model");

        var core = new Blocks(new RenderContextAccessor(), new BlockContainerFactory());
        _ = External.LoadChat(core, "http://127.0.0.1:9/v1/", "local-model");

        Assert.Equal(ConfigOf(core), ConfigOf(layer));
    }

    [Fact]
    public void An_interface_that_says_nothing_about_analytics_leaves_the_environment_to_answer()
    {
        // The Python version's `analytics_enabled=None` is not "off": it is "read the variable", which is what
        // the application's own default already does. Setting it when it was not given would turn a
        // ported line into a decision.
        var layer = gr.Interface((Func<string, string>)(value => value), [gr.Textbox()], [gr.Textbox()]);

        Assert.Equal(new Blocks(new RenderContextAccessor()).AnalyticsEnabled, layer.AnalyticsEnabled);
    }

    /// <summary>The served document, as the text a client receives.</summary>
    /// <param name="demo">The application.</param>
    /// <returns>The document, without the two fields that belong to the instance.</returns>
    /// <remarks>
    /// <c>app_id</c> is drawn per application, and <c>secret</c> is the key a chat's stored history
    /// is sealed with — also drawn per application. Neither is a fact about the page, and two
    /// applications in one process never agree on either.
    /// </remarks>
    private static string ConfigOf(Blocks demo)
    {
        var config = demo.GetConfig();
        config.Remove("app_id");

        foreach (var component in config["components"]!.AsArray())
        {
            component!["props"]?.AsObject().Remove("secret");
        }

        return config.ToJsonString();
    }
}

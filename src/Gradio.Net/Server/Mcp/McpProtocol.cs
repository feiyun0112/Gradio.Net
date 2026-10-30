using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;

namespace Gradio.Net;
/// <summary>
/// The exchange an MCP client has with an application: one JSON-RPC message in, one out
/// </summary>
/// <remarks>
/// <para>
/// The protocol is JSON-RPC 2.0 with a fixed method set, and the methods are the four lists an
/// agent asks for and the three calls it can make: <c>tools/list</c> and <c>tools/call</c>,
/// <c>resources/list</c>, <c>resources/templates/list</c> and <c>resources/read</c>,
/// <c>prompts/list</c> and <c>prompts/get</c>, and <c>initialize</c> before any of them.
/// </para>
/// <para>
/// What each answer is built from is the application's own description: the same endpoint list the
/// API info serves, read through <see cref="McpCatalog"/>. That is what keeps a callable endpoint
/// and an agent's tool the same thing rather than two lists that agree today.
/// </para>
/// <para>
/// Two kinds of failure are told apart, because a client treats them differently. A request that
/// names something this application does not have is an error in the exchange — a JSON-RPC error
/// response — while a call that reached a handler which failed is an answer that says the call
/// failed, which is what lets an agent read the failure and try something else.
/// </para>
/// </remarks>
internal sealed class McpProtocol
{
    /// <summary>The protocol version this implementation answers with when a client names none it knows.</summary>
    internal const string LatestVersion = "2025-06-18";

    /// <summary>The versions a client may name and be answered in.</summary>
    /// <remarks>
    /// The three revisions MCP has published. A client that asks for one of these is answered in it,
    /// which is what the protocol's negotiation asks for: the server accepts the client's version
    /// when it can, and names its own when it cannot.
    /// </remarks>
    private static readonly string[] _versions = ["2024-11-05", "2025-03-26", LatestVersion];

    private readonly Blocks _blocks;
    private readonly McpToolRunner _runner;
    private readonly GradioAuthentication _authentication;

    /// <summary>Creates the protocol handler.</summary>
    /// <param name="blocks">The application an agent sees.</param>
    /// <param name="runner">What runs an endpoint for a call.</param>
    /// <param name="authentication">Who is calling, which a run carries to its handler.</param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    public McpProtocol(Blocks blocks, McpToolRunner runner, GradioAuthentication authentication)
    {
        ArgumentNullException.ThrowIfNull(blocks);
        ArgumentNullException.ThrowIfNull(runner);
        ArgumentNullException.ThrowIfNull(authentication);

        _blocks = blocks;
        _runner = runner;
        _authentication = authentication;
    }

    /// <summary>Handles one message.</summary>
    /// <param name="message">The JSON-RPC message.</param>
    /// <param name="context">The request it arrived on, which names the application's address.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>The response, or <see langword="null"/> for a notification, which has none.</returns>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    internal async Task<JsonObject?> HandleAsync(
        JsonObject message,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentNullException.ThrowIfNull(context);

        var id = message["id"]?.DeepClone();
        var method = JsonValues.Text(message["method"]) ?? string.Empty;
        var parameters = message["params"] as JsonObject ?? new JsonObject();

        // A notification is a message the client does not expect an answer to, which is the one
        // thing JSON-RPC says about it. The protocol's own notifications are the client saying it
        // is ready and that a call it started is no longer wanted, and neither changes anything
        // here: a run's lifetime is its request's.
        if (id is null)
        {
            return null;
        }

        var catalog = McpCatalog.Of(_blocks);

        try
        {
            return method switch
            {
                "initialize" => Answer(id, Initialize(parameters)),
                "ping" => Answer(id, new JsonObject()),
                "tools/list" => Answer(id, ListTools(catalog, Selected(context))),
                "tools/call" => await CallToolAsync(id, catalog, parameters, context, cancellationToken)
                    .ConfigureAwait(false),
                "resources/list" => Answer(id, ListResources(catalog, Selected(context))),
                "resources/templates/list" => Answer(id, ListResourceTemplates(catalog, Selected(context))),
                "resources/read" => await ReadResourceAsync(id, catalog, parameters, context, cancellationToken)
                    .ConfigureAwait(false),
                "prompts/list" => Answer(id, ListPrompts(catalog, Selected(context))),
                "prompts/get" => await GetPromptAsync(id, catalog, parameters, context, cancellationToken)
                    .ConfigureAwait(false),
                _ => Refuse(id, -32601, $"Method not found: {method}"),
            };
        }
        catch (McpCallFailure failure)
        {
            return Refuse(id, -32603, failure.Message);
        }
    }

    /// <summary>The handshake, which is where the protocol version is agreed.</summary>
    /// <param name="parameters">What the client said about itself.</param>
    /// <returns>The server's half of the handshake.</returns>
    private JsonObject Initialize(JsonObject parameters)
    {
        var asked = JsonValues.Text(parameters["protocolVersion"]);

        return new JsonObject
        {
            ["protocolVersion"] = asked is not null && _versions.Contains(asked, StringComparer.Ordinal)
                ? asked
                : LatestVersion,
            ["capabilities"] = new JsonObject
            {
                ["tools"] = new JsonObject { ["listChanged"] = false },
                ["resources"] = new JsonObject { ["subscribe"] = false, ["listChanged"] = false },
                ["prompts"] = new JsonObject { ["listChanged"] = false },
            },
            ["serverInfo"] = new JsonObject
            {
                ["name"] = string.IsNullOrEmpty(_blocks.Title) ? "Gradio App" : _blocks.Title,
                ["version"] = GradioVersion.PackageVersion,
            },
        };
    }

    /// <summary>The tools this application offers.</summary>
    /// <param name="catalog">The application as an agent sees it.</param>
    /// <param name="selected">The tools the request asked for, or <see langword="null"/> for all.</param>
    /// <returns>The list result.</returns>
    private static JsonObject ListTools(McpCatalog catalog, IReadOnlyList<string>? selected)
    {
        var tools = new JsonArray();

        foreach (var tool in catalog.Tools)
        {
            if (selected is not null && !selected.Contains(tool.Name, StringComparer.Ordinal))
            {
                continue;
            }

            tools.Add(new JsonObject
            {
                ["name"] = tool.Name,
                ["description"] = tool.Description,
                ["inputSchema"] = tool.InputSchema.DeepClone(),
            });
        }

        return new JsonObject { ["tools"] = tools };
    }

    /// <summary>Calls a tool, and answers with what the endpoint returned.</summary>
    /// <param name="id">The request's id.</param>
    /// <param name="catalog">The application as an agent sees it.</param>
    /// <param name="parameters">The call's name and arguments.</param>
    /// <param name="context">The request it arrived on.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>The response.</returns>
    private async Task<JsonObject> CallToolAsync(
        JsonNode? id,
        McpCatalog catalog,
        JsonObject parameters,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var name = JsonValues.Text(parameters["name"]) ?? string.Empty;
        var arguments = parameters["arguments"] as JsonObject ?? new JsonObject();

        if (catalog.Tool(name) is not { } tool)
        {
            return Refuse(id, -32602, $"Unknown tool for this Gradio app: {name}");
        }

        JsonArray data;
        try
        {
            data = await _runner.RunAsync(
                tool.Dependency,
                tool.ParameterNames,
                arguments,
                await _authentication.UserNameOfAsync(context).ConfigureAwait(false),
                cancellationToken)
                .ConfigureAwait(false);
        }
        catch (McpCallFailure failure)
        {
            // A call that reached a handler which failed is an answer rather than an exchange
            // error: an agent reads the text, and a client is not left wondering whether the
            // connection is still good.
            return Answer(id, new JsonObject
            {
                ["content"] = new JsonArray(new JsonObject { ["type"] = "text", ["text"] = failure.Message }),
                ["isError"] = true,
            });
        }

        var content = McpContent.Of(data, RootUrl.Of(context));

        var result = new JsonObject
        {
            ["content"] = content,
        };

        if (tool.StructuredOutput)
        {
            result["structuredContent"] = new JsonObject { ["result"] = content.DeepClone() };
        }

        return Answer(id, result);
    }

    /// <summary>The resources whose address is fixed.</summary>
    /// <param name="catalog">The application as an agent sees it.</param>
    /// <param name="selected">The names the request asked for, or <see langword="null"/> for all.</param>
    /// <returns>The list result.</returns>
    private static JsonObject ListResources(McpCatalog catalog, IReadOnlyList<string>? selected)
    {
        var resources = new JsonArray();

        foreach (var resource in catalog.Resources)
        {
            if (selected is not null && !selected.Contains(resource.Name, StringComparer.Ordinal))
            {
                continue;
            }

            resources.Add(new JsonObject
            {
                ["uri"] = resource.UriTemplate,
                ["name"] = resource.Name,
                ["description"] = resource.Description,
                ["mimeType"] = resource.MimeType,
            });
        }

        return new JsonObject { ["resources"] = resources };
    }

    /// <summary>The resources whose address carries a parameter.</summary>
    /// <param name="catalog">The application as an agent sees it.</param>
    /// <param name="selected">The names the request asked for, or <see langword="null"/> for all.</param>
    /// <returns>The list result.</returns>
    private static JsonObject ListResourceTemplates(McpCatalog catalog, IReadOnlyList<string>? selected)
    {
        var templates = new JsonArray();

        foreach (var template in catalog.ResourceTemplates)
        {
            if (selected is not null && !selected.Contains(template.Name, StringComparer.Ordinal))
            {
                continue;
            }

            templates.Add(new JsonObject
            {
                ["uriTemplate"] = template.UriTemplate,
                ["name"] = template.Name,
                ["description"] = template.Description,
                ["mimeType"] = template.MimeType,
            });
        }

        return new JsonObject { ["resourceTemplates"] = templates };
    }

    /// <summary>Reads a resource by address.</summary>
    /// <param name="id">The request's id.</param>
    /// <param name="catalog">The application as an agent sees it.</param>
    /// <param name="parameters">The address.</param>
    /// <param name="context">The request it arrived on.</param>
    /// <param name="cancellationToken">Cancels the read.</param>
    /// <returns>The response.</returns>
    private async Task<JsonObject> ReadResourceAsync(
        JsonNode? id,
        McpCatalog catalog,
        JsonObject parameters,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var uri = JsonValues.Text(parameters["uri"]) ?? string.Empty;

        foreach (var resource in catalog.Resources.Concat(catalog.ResourceTemplates))
        {
            if (McpCatalog.Match(resource.UriTemplate, uri) is not { } values)
            {
                continue;
            }

            var arguments = new JsonObject();
            foreach (var (name, value) in values)
            {
                arguments[name] = value;
            }

            var names = ArgumentNames(resource.Dependency);
            var data = await _runner.RunAsync(
                resource.Dependency,
                names,
                arguments,
                await _authentication.UserNameOfAsync(context).ConfigureAwait(false),
                cancellationToken)
                .ConfigureAwait(false);

            var text = McpContent.TextOf(data.Count > 0 ? data[0] : null);
            var content = new JsonObject
            {
                ["uri"] = uri,
                ["mimeType"] = resource.MimeType,
            };

            // A media type that is not text says the handler answered with the bytes, base64
            // encoded — which is the shape a resource's binary content travels in, so it is what
            // the block's value already is.
            if (resource.MimeType.StartsWith("text/", StringComparison.OrdinalIgnoreCase))
            {
                content["text"] = text;
            }
            else
            {
                content["blob"] = text;
            }

            return Answer(id, new JsonObject { ["contents"] = new JsonArray(content) });
        }

        return Refuse(id, -32602, $"Resource not found: {uri}");
    }

    /// <summary>The prompts this application offers.</summary>
    /// <param name="catalog">The application as an agent sees it.</param>
    /// <param name="selected">The names the request asked for, or <see langword="null"/> for all.</param>
    /// <returns>The list result.</returns>
    private static JsonObject ListPrompts(McpCatalog catalog, IReadOnlyList<string>? selected)
    {
        var prompts = new JsonArray();

        foreach (var prompt in catalog.Prompts)
        {
            if (selected is not null && !selected.Contains(prompt.Name, StringComparer.Ordinal))
            {
                continue;
            }

            var arguments = new JsonArray();
            foreach (var argument in prompt.Arguments)
            {
                arguments.Add(new JsonObject
                {
                    ["name"] = argument.Name,
                    ["description"] = string.Empty,
                    ["required"] = argument.Required,
                });
            }

            prompts.Add(new JsonObject
            {
                ["name"] = prompt.Name,
                ["description"] = prompt.Description,
                ["arguments"] = arguments,
            });
        }

        return new JsonObject { ["prompts"] = prompts };
    }

    /// <summary>Fills a prompt in, and answers with the message it produced.</summary>
    /// <param name="id">The request's id.</param>
    /// <param name="catalog">The application as an agent sees it.</param>
    /// <param name="parameters">The prompt's name and arguments.</param>
    /// <param name="context">The request it arrived on.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>The response.</returns>
    private async Task<JsonObject> GetPromptAsync(
        JsonNode? id,
        McpCatalog catalog,
        JsonObject parameters,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        var name = JsonValues.Text(parameters["name"]) ?? string.Empty;
        var arguments = parameters["arguments"] as JsonObject ?? new JsonObject();

        if (catalog.Prompt(name) is not { } prompt)
        {
            return Refuse(id, -32602, $"Prompt not found: {name}");
        }

        var data = await _runner.RunAsync(
            prompt.Dependency,
            [.. prompt.Arguments.Select(argument => argument.Name)],
            arguments,
            await _authentication.UserNameOfAsync(context).ConfigureAwait(false),
            cancellationToken).ConfigureAwait(false);

        return Answer(id, new JsonObject
        {
            ["description"] = prompt.Description,
            ["messages"] = new JsonArray(new JsonObject
            {
                ["role"] = "user",
                ["content"] = new JsonObject
                {
                    ["type"] = "text",
                    ["text"] = McpContent.TextOf(data.Count > 0 ? data[0] : null),
                },
            }),
        });
    }

    /// <summary>The parameter names a binding's endpoint describes, in order.</summary>
    /// <param name="dependency">The binding.</param>
    /// <returns>The names.</returns>
    /// <remarks>
    /// Read from the application rather than from the catalog, because a resource is not listed
    /// with an input schema: its arguments come from its address, and the order they are read in is
    /// the order the endpoint declares them.
    /// </remarks>
    private IReadOnlyList<string> ArgumentNames(Dependency dependency)
    {
        foreach (var (candidate, endpoint) in _blocks.DocumentedEndpoints())
        {
            if (!ReferenceEquals(candidate, dependency))
            {
                continue;
            }

            return [.. endpoint["parameters"]!.AsArray()
                .Select(parameter => parameter!["parameter_name"]!.GetValue<string>())];
        }

        return [];
    }

    /// <summary>The names a request asked for, or <see langword="null"/> for all of them.</summary>
    /// <param name="context">The request.</param>
    /// <returns>The names, or <see langword="null"/>.</returns>
    /// <remarks>
    /// The Python version lets a client narrow the whole catalog with a <c>tools</c> query
    /// parameter, which is how one application is offered to an agent as a subset of what it can
    /// do. The parameter is named after tools because tools came first, and it narrows the
    /// resources and the prompts too: the Python version reads it in one place and every list is
    /// filtered by it.
    /// </remarks>
    internal static IReadOnlyList<string>? Selected(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var selected = context.Request.Query["tools"].ToString();

        return selected.Length == 0
            ? null
            : [.. selected.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)];
    }

    /// <summary>A response that carries a result.</summary>
    /// <param name="id">The request's id.</param>
    /// <param name="result">The result.</param>
    /// <returns>The response.</returns>
    private static JsonObject Answer(JsonNode? id, JsonObject result) => new()
    {
        ["jsonrpc"] = "2.0",
        ["id"] = id?.DeepClone(),
        ["result"] = result,
    };

    /// <summary>A response that carries an error.</summary>
    /// <param name="id">The request's id.</param>
    /// <param name="code">The JSON-RPC error code.</param>
    /// <param name="message">What went wrong.</param>
    /// <returns>The response.</returns>
    private static JsonObject Refuse(JsonNode? id, int code, string message) => new()
    {
        ["jsonrpc"] = "2.0",
        ["id"] = id?.DeepClone(),
        ["error"] = new JsonObject
        {
            ["code"] = code,
            ["message"] = message,
        },
    };
}

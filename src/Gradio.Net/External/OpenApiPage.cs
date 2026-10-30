using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
namespace Gradio.Net;
/// <summary>
/// The page a service's OpenAPI document is turned into: a panel listing the endpoints, and then one
/// interface per endpoint, each a form that calls it.
/// </summary>
/// <remarks>
/// <para>
/// This is the Python version's <c>load_openapi</c>. A document describes an HTTP service in enough
/// detail to build a form for it — a parameter's name, where it goes, what it may hold; a request
/// body's shape — and the page is that reading of it: a text box per parameter, a box of JSON per
/// body, and a JSON component for the answer.
/// </para>
/// <para>
/// The components are built before the heading that introduces them, because that is the order the
/// Python version builds them in and the config numbers what is built. The heading, the summary and
/// the response component follow, and then the interface draws itself.
/// </para>
/// <para>
/// Three things are done differently from the Python version 6.30.0, all of them cases where its
/// own code cannot produce what it says. A document with a path-level <c>parameters</c> or
/// <c>summary</c> key makes it treat that key as an HTTP method and fail; here only the methods the
/// protocol defines are endpoints. A file upload sends the file's bytes; the Python version sends
/// the file's path as the body, which no service accepts. And a call that fails is left to fail —
/// the Python version answers with the text "Error: ...", which its own JSON output then refuses to
/// parse — so the run reports the failure the way any other handler's does.
/// </para>
/// </remarks>
internal sealed partial class OpenApiPage
{
    /// <summary>The methods a path item may hold, which are the ones the protocol defines.</summary>
    private static readonly IReadOnlyList<string> HttpMethods =
    [
        "get", "put", "post", "delete", "options", "head", "patch", "trace",
    ];

    /// <summary>The content types that mean "the body is a file".</summary>
    private static readonly IReadOnlyList<string> FileContentTypes =
    [
        "application/octet-stream", "multipart/form-data",
    ];

    /// <summary>The statuses the Python version treats as an answer rather than a
    /// refusal.</summary>
    private static readonly IReadOnlyList<int> AnsweredStatuses = [200, 201, 202, 204];

    private readonly Blocks _blocks;
    private readonly JsonObject _spec;
    private readonly string _baseUrl;
    private readonly string? _authToken;
    private readonly HttpClient _client;
    private readonly Func<OpenApiEndpoint, Interface> _build;
    private readonly string _title;

    /// <summary>Describes the page a document is turned into.</summary>
    /// <param name="blocks">The application to render into.</param>
    /// <param name="spec">The document.</param>
    /// <param name="baseUrl">The address the endpoints are called at.</param>
    /// <param name="authToken">The bearer token to send, or <see langword="null"/> for none.</param>
    /// <param name="client">The client to call the service with.</param>
    /// <param name="build">How each endpoint's interface is built.</param>
    internal OpenApiPage(
        Blocks blocks,
        JsonObject spec,
        string baseUrl,
        string? authToken,
        HttpClient client,
        Func<OpenApiEndpoint, Interface> build)
    {
        _blocks = blocks;
        _spec = spec;
        _baseUrl = baseUrl;
        _authToken = authToken;
        _client = client;
        _build = build;
        _title = JsonValues.Text(spec["info"]?["title"]) ?? "OpenAPI Interface";
    }

    /// <summary>Draws the panel and one interface per endpoint.</summary>
    /// <param name="paths">The paths to keep, as patterns, or <see langword="null"/> for all of them.</param>
    /// <param name="excludePaths">The paths to leave out, as patterns, or <see langword="null"/> for none.</param>
    /// <param name="methods">The methods to keep, or <see langword="null"/> for all of them.</param>
    /// <exception cref="ArgumentException">The document holds no endpoint the filters keep.</exception>
    internal void Render(
        IReadOnlyList<string>? paths,
        IReadOnlyList<string>? excludePaths,
        IReadOnlyList<string>? methods)
    {
        var endpoints = EndpointsOf(paths, excludePaths, methods);

        if (endpoints.Count == 0)
        {
            throw new ArgumentException(
                "No valid paths found in the OpenAPI specification.",
                nameof(paths));
        }

        // A page nested inside another render joins it rather than opening a second scope: the
        // outermost scope that closes is the one that finishes the render.
        using var scope = _blocks.IsRendering ? null : _blocks.BeginRender();

        // The application's own description is read before the forms are drawn, because a form
        // claims the application it is built into — which is right when it is built alone and wrong
        // for a page that holds several. The title is not read: the Python version's page is titled by the
        // document, and so is this one.
        var description = _blocks.Description;

        DrawPanel(endpoints);

        foreach (var (path, method, operation) in endpoints)
        {
            DrawEndpoint(path, method, operation);
        }

        // The application says what it is after the interfaces have had their say. What it keeps is
        // what the Python version's `gr.Blocks` is — a page of blocks, under the document's title, described
        // the way the application described itself. The Python version's page is a fresh application, so the
        // description it keeps is the default; here the application is the caller's, so the
        // description kept is the caller's.
        _blocks.Mode = "blocks";
        _blocks.Title = _title;
        _blocks.Description = description;
    }

    /// <summary>Draws the panel that lists the endpoints.</summary>
    /// <param name="endpoints">The endpoints the page holds.</param>
    private void DrawPanel(IReadOnlyList<(string Path, string Method, JsonObject Operation)> endpoints)
    {
        var sidebar = _blocks.Add(new Sidebar());

        using (_blocks.Enter(sidebar))
        {
            _blocks.Add(new Markdown { Value = $"## {_title}" });
            _blocks.Add(new Markdown { Value = JsonValues.Text(_spec["info"]?["description"]) ?? string.Empty });
            _blocks.Add(new Markdown { Value = "### API Endpoints" });

            var links = new StringBuilder(
                "<div style='overflow-x: auto; overflow-y: auto; max-height: 500px;'><ul>");

            foreach (var (path, method, _) in endpoints)
            {
                links.Append(CultureInfo.InvariantCulture, $"<li style='white-space: nowrap;'>")
                    .Append(CultureInfo.InvariantCulture, $"<a href='#{AnchorOf(method, path)}' style='white-space: nowrap;'>")
                    .Append(CultureInfo.InvariantCulture, $"{method.ToUpperInvariant()} ")
                    .Append(CultureInfo.InvariantCulture, $"<code style='font-size: inherit; white-space: nowrap;'>{path}</code>")
                    .Append("</a></li>\n");
            }

            links.Append("</ul></div>");

            _blocks.Add(new Markdown { Value = links.ToString() });
        }
    }

    /// <summary>Draws one endpoint: its heading, its summary, and the form that calls it.</summary>
    /// <param name="path">The path.</param>
    /// <param name="method">The method.</param>
    /// <param name="operation">The operation.</param>
    private void DrawEndpoint(string path, string method, JsonObject operation)
    {
        // The inputs are built before the heading that introduces them, because that is the order
        // the Python version builds them in and the config numbers what is built.
        var inputs = new List<Component>();
        var names = new List<string>();

        foreach (var parameter in ParametersOf(operation))
        {
            inputs.Add(ComponentOf(parameter));
            names.Add(JsonValues.Text(parameter["name"]) ?? string.Empty);
        }

        var body = operation["requestBody"] as JsonObject;

        if (body is not null && BodyComponentOf(body) is { } bodyComponent)
        {
            inputs.Add(bodyComponent);
            names.Add("request_body");
        }

        _blocks.Add(new Markdown { Value = HeadingOf(path, method) });

        if (JsonValues.Text(operation["summary"]) is { Length: > 0 } summary)
        {
            _blocks.Add(new Markdown { Value = summary });
        }

        var outputs = new List<Component> { new JSON { Label = "Response" } };

        var endpoint = new OpenApiEndpoint(
            path,
            method,
            HandlerOf(path, method, operation, inputs.Count),
            inputs,
            outputs,
            ApiNameOf(path, method),
            DescriptionOf(operation),
            names);

        _build(endpoint);
    }

    /// <summary>The endpoints the filters keep, in the document's order.</summary>
    /// <param name="paths">The paths to keep, as patterns, or <see langword="null"/> for all of them.</param>
    /// <param name="excludePaths">The paths to leave out, as patterns, or <see langword="null"/> for none.</param>
    /// <param name="methods">The methods to keep, or <see langword="null"/> for all of them.</param>
    /// <returns>One entry per endpoint.</returns>
    private List<(string Path, string Method, JsonObject Operation)> EndpointsOf(
        IReadOnlyList<string>? paths,
        IReadOnlyList<string>? excludePaths,
        IReadOnlyList<string>? methods)
    {
        var endpoints = new List<(string Path, string Method, JsonObject Operation)>();

        foreach (var (path, item) in SelectedPaths(paths, excludePaths))
        {
            if (item is not JsonObject operations)
            {
                continue;
            }

            foreach (var (method, node) in operations)
            {
                if (!HttpMethods.Contains(method, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (methods is not null
                    && !methods.Any(kept => string.Equals(kept, method, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                if (node is JsonObject operation)
                {
                    endpoints.Add((path, method.ToLowerInvariant(), operation));
                }
            }
        }

        return endpoints;
    }

    /// <summary>The paths of the document the filters keep.</summary>
    /// <param name="paths">The paths to keep, as patterns, or <see langword="null"/> for all of them.</param>
    /// <param name="excludePaths">The paths to leave out, as patterns, or <see langword="null"/> for none.</param>
    /// <returns>The path items, in the document's order.</returns>
    /// <remarks>
    /// A pattern is matched at the start of the path and nowhere else, which is what the Python
    /// version's <c>re.match</c> does: <c>/books</c> keeps <c>/books</c> and <c>/books/{id}</c> and
    /// leaves <c>/shelves/books</c> alone. The exclusions are applied first, so an excluded path
    /// stays excluded however it is asked for.
    /// </remarks>
    private IEnumerable<(string Path, JsonNode? Item)> SelectedPaths(
        IReadOnlyList<string>? paths,
        IReadOnlyList<string>? excludePaths)
    {
        if (_spec["paths"] is not JsonObject items)
        {
            yield break;
        }

        foreach (var (path, item) in items)
        {
            if (excludePaths is not null && excludePaths.Any(pattern => Matches(pattern, path)))
            {
                continue;
            }

            if (paths is not null && !paths.Any(pattern => Matches(pattern, path)))
            {
                continue;
            }

            yield return (path, item);
        }
    }

    /// <summary>Whether a pattern matches the start of a path.</summary>
    /// <param name="pattern">The pattern.</param>
    /// <param name="path">The path.</param>
    /// <returns>Whether it matches.</returns>
    private static bool Matches(string pattern, string path) =>
        Regex.IsMatch(path, "\\A(?:" + pattern + ")", RegexOptions.CultureInvariant);

    /// <summary>The component a parameter's argument is read from.</summary>
    /// <param name="parameter">The parameter.</param>
    /// <returns>The component.</returns>
    /// <remarks>
    /// The Python version's <c>component_from_parameter_schema</c>: an enumeration is a dropdown, a
    /// number a number field, a flag a checkbox, and everything else a text box — with an array
    /// read as JSON, because a list of values is not something one field can hold otherwise.
    /// </remarks>
    private static Component ComponentOf(JsonObject parameter)
    {
        var name = JsonValues.Text(parameter["name"]);
        var description = JsonValues.Text(parameter["description"]);
        var schema = parameter["schema"] as JsonObject ?? [];
        var type = JsonValues.Text(schema["type"]);
        var defaultValue = JsonValues.ToObject(schema["default"]);

        if (schema["enum"] is JsonArray values)
        {
            return new Dropdown
            {
                Choices = [.. values.Select(value => new Choice(JsonValues.ToObject(value) ?? string.Empty))],
                Label = name,
                Value = defaultValue,
                AllowCustomValue = false,
                Info = description,
            };
        }

        return type switch
        {
            "number" or "integer" => new Number
            {
                Label = name,
                Value = JsonValues.Number(schema["default"]),
                Info = description,
            },

            "boolean" => new Checkbox
            {
                Label = name,
                Value = schema["default"] is JsonValue flag && flag.TryGetValue<bool>(out var on) && on,
                Info = description,
            },

            "array" => new Textbox
            {
                Label = $"{name} (JSON array)",
                Value = "[]",
                Info = description,
            },

            _ => new Textbox
            {
                Label = name,
                Value = TextOf(defaultValue),
                Info = description,
            },
        };
    }

    /// <summary>The component a request body is typed into, or <see langword="null"/> for none.</summary>
    /// <param name="requestBody">The request body.</param>
    /// <returns>The component.</returns>
    /// <remarks>
    /// The Python version's <c>component_from_request_body_schema</c>. A body that is a file is a
    /// file field; a body that is JSON is a text box holding an example of it, built from the
    /// schema's own examples when it has them and from the shape of its properties when it does
    /// not.
    /// </remarks>
    private Component? BodyComponentOf(JsonObject requestBody)
    {
        if (requestBody["content"] is not JsonObject content)
        {
            return null;
        }

        foreach (var (contentType, entry) in content)
        {
            if (!FileContentTypes.Contains(contentType, StringComparer.Ordinal))
            {
                continue;
            }

            var schema = Resolve(entry?["schema"] as JsonObject ?? []);

            if (JsonValues.Text(schema["type"]) == "string" && JsonValues.Text(schema["format"]) == "binary")
            {
                return new File { Label = "File" };
            }
        }

        var json = content["application/json"] as JsonObject
            ?? content.FirstOrDefault(entry => entry.Key.StartsWith("application/", StringComparison.Ordinal)).Value as JsonObject;

        if (json is null)
        {
            return null;
        }

        var example = ExampleOf(Resolve(json["schema"] as JsonObject ?? []));

        return new Textbox
        {
            Label = "Request Body",
            Value = IndentedText(example),
            Info = JsonValues.Text(requestBody["description"]) ?? "Request Body",
        };
    }

    /// <summary>The example a schema describes, which the body's field starts with.</summary>
    /// <param name="schema">The schema.</param>
    /// <returns>The example.</returns>
    /// <remarks>
    /// A schema's own example is used as it stands. One without an example is filled in property by
    /// property, each with its own example or with the empty value its type has, which is what
    /// the Python version does to give the reader something to edit rather than an empty box.
    /// </remarks>
    private JsonNode ExampleOf(JsonObject schema)
    {
        if (schema["example"] is { } example && !IsEmpty(example))
        {
            return example.DeepClone();
        }

        if (JsonValues.Text(schema["type"]) != "object")
        {
            return new JsonObject();
        }

        var filled = new JsonObject();

        if (schema["properties"] is not JsonObject properties)
        {
            return filled;
        }

        foreach (var (name, node) in properties)
        {
            var property = Resolve(node as JsonObject ?? []);

            filled[name] = JsonValues.Text(property["type"]) switch
            {
                "string" => property["example"]?.DeepClone() ?? JsonValue.Create(string.Empty),
                "number" or "integer" => property["example"]?.DeepClone() ?? JsonValue.Create(0),
                "boolean" => property["example"]?.DeepClone() ?? JsonValue.Create(false),
                "array" => property["example"]?.DeepClone() ?? new JsonArray(),
                "object" => property["example"]?.DeepClone() ?? new JsonObject(),
                _ => property["example"]?.DeepClone(),
            };
        }

        return filled;
    }

    /// <summary>Whether an example says nothing, which is what the Python version's own emptiness
    /// test asks.</summary> <param name="example">The example.</param> <returns>Whether it is an
    /// empty object, an empty list, an empty text, zero, false or nothing.</returns>
    private static bool IsEmpty(JsonNode example) => example switch
    {
        JsonObject document => document.Count == 0,
        JsonArray array => array.Count == 0,
        JsonValue value when value.TryGetValue<string>(out var text) => text.Length == 0,
        JsonValue value when value.TryGetValue<double>(out var number) => number == 0,
        JsonValue value when value.TryGetValue<bool>(out var flag) => !flag,
        _ => false,
    };

    /// <summary>The schema a reference names, or the schema itself when it is not one.</summary>
    /// <param name="schema">The schema, or a reference to it.</param>
    /// <returns>The schema.</returns>
    /// <remarks>
    /// A reference is resolved within the document, which is where every reference in a document
    /// this page can use points. One that points elsewhere is left as it is, and the field then
    /// starts empty rather than the page failing to load.
    /// </remarks>
    private JsonObject Resolve(JsonObject schema)
    {
        if (JsonValues.Text(schema["$ref"]) is not { } reference)
        {
            return schema;
        }

        JsonNode? current = _spec;

        foreach (var step in reference.TrimStart('#').TrimStart('/').Split('/'))
        {
            if (string.IsNullOrEmpty(step))
            {
                continue;
            }

            current = current?[step.Replace("~1", "/", StringComparison.Ordinal).Replace("~0", "~", StringComparison.Ordinal)];
        }

        return current as JsonObject ?? schema;
    }

    /// <summary>The handler that calls one endpoint.</summary>
    /// <param name="path">The path.</param>
    /// <param name="method">The method.</param>
    /// <param name="operation">The operation.</param>
    /// <param name="arity">How many arguments the interface hands it.</param>
    /// <returns>The handler.</returns>
    private Delegate HandlerOf(string path, string method, JsonObject operation, int arity) =>
        CollectingDelegate.OfAsync(arity, values => CallAsync(path, method, operation, values));

    /// <summary>Calls the service and answers with what it said.</summary>
    /// <param name="path">The path.</param>
    /// <param name="method">The method.</param>
    /// <param name="operation">The operation, which says where each argument goes.</param>
    /// <param name="values">The arguments, in the order the components are drawn.</param>
    /// <returns>The answer, as JSON.</returns>
    /// <exception cref="HttpRequestException">The service could not be reached.</exception>
    /// <remarks>
    /// The arguments are read in the order the document declares them, and each goes where the
    /// document says: a query parameter onto the address, a path parameter into the path's own
    /// placeholder, and a body into the request. An argument the document does not place — a header,
    /// a cookie — is read and dropped, because a form cannot say what to do with it.
    /// </remarks>
    private async Task<object?> CallAsync(
        string path,
        string method,
        JsonObject operation,
        object?[] values)
    {
        var address = _baseUrl.TrimEnd('/') + path;
        var query = new List<KeyValuePair<string, string>>();
        var next = 0;

        foreach (var parameter in ParametersOf(operation))
        {
            if (next >= values.Length)
            {
                break;
            }

            var value = values[next++];
            var name = JsonValues.Text(parameter["name"]) ?? string.Empty;

            switch (JsonValues.Text(parameter["in"]))
            {
                case "query" when value is not null:
                    query.Add(new KeyValuePair<string, string>(name, TextOf(value)!));
                    break;

                case "path":
                    address = address.Replace($"{{{name}}}", TextOf(value), StringComparison.Ordinal);
                    break;

                default:
                    break;
            }
        }

        using var request = new HttpRequestMessage(
            new HttpMethod(method.ToUpperInvariant()),
            Address(address, query));

        var body = operation["requestBody"] as JsonObject;
        var upload = body is not null && IsUpload(body);

        if (!string.IsNullOrEmpty(_authToken))
        {
            request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {_authToken}");
        }

        // A GET and a DELETE carry no body however the document describes one, so an upload's type
        // has nothing to describe on those and the request is sent as it is.
        if (body is not null && next < values.Length && method is "post" or "put" or "patch")
        {
            request.Content = upload
                ? BinaryContent(await Attachments
                    .BytesAsync(TextOf(values[next]) ?? string.Empty, _client, CancellationToken.None)
                    .ConfigureAwait(false))
                : new StringContent(JsonOf(values[next]).ToJsonString(), Encoding.UTF8, "application/json");
        }

        using var response = await _client
            .SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None)
            .ConfigureAwait(false);

        var text = await response.Content.ReadAsStringAsync(CancellationToken.None).ConfigureAwait(false);
        var status = (int)response.StatusCode;

        if (AnsweredStatuses.Contains(status))
        {
            // A service that answered with nothing to say — 204, or a 200 with an empty body — is an
            // empty document rather than a failure.
            return text.Length == 0 ? new JsonObject() : JsonNode.Parse(text);
        }

        return new JsonObject
        {
            ["__status__"] = "error",
            ["status_code"] = status,
            ["message"] = text,
        };
    }

    /// <summary>Whether a request body is a file rather than JSON.</summary>
    /// <param name="body">The request body.</param>
    /// <returns>Whether one of its content types is a file's.</returns>
    private static bool IsUpload(JsonObject body) =>
        body["content"] is JsonObject content
        && content.Any(entry => FileContentTypes.Contains(entry.Key, StringComparer.Ordinal));

    /// <summary>A body that is a file, with the type the document says it is.</summary>
    /// <param name="bytes">The file's bytes.</param>
    /// <returns>The content.</returns>
    /// <remarks>
    /// The Python version sends the path itself as the body, which is the one thing a service
    /// cannot use. The file's bytes are sent here instead — and, like every other file this
    /// framework handles, a value that is an address rather than a path is fetched.
    /// </remarks>
    private static ByteArrayContent BinaryContent(byte[] bytes)
    {
        var content = new ByteArrayContent(bytes);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        return content;
    }

    /// <summary>The document a body argument holds.</summary>
    /// <param name="value">The text the field holds.</param>
    /// <returns>The document.</returns>
    /// <exception cref="System.Text.Json.JsonException">The text is not JSON.</exception>
    private static JsonNode JsonOf(object? value) =>
        JsonNode.Parse(TextOf(value) ?? string.Empty) ?? new JsonObject();

    /// <summary>The address a call goes to, with its query parameters.</summary>
    /// <param name="address">The path's address.</param>
    /// <param name="query">The query parameters.</param>
    /// <returns>The address.</returns>
    private static string Address(string address, List<KeyValuePair<string, string>> query)
    {
        if (query.Count == 0)
        {
            return address;
        }

        var builder = new StringBuilder(address).Append('?');

        for (var index = 0; index < query.Count; index++)
        {
            if (index > 0)
            {
                builder.Append('&');
            }

            builder.Append(Uri.EscapeDataString(query[index].Key))
                .Append('=')
                .Append(Uri.EscapeDataString(query[index].Value));
        }

        return builder.ToString();
    }

    /// <summary>An argument as the text a request carries.</summary>
    /// <param name="value">The value.</param>
    /// <returns>The text.</returns>
    /// <remarks>
    /// A number is written the way .NET writes it rather than the way Python does — <c>5</c> rather
    /// than <c>5.0</c> — because the text is what a service matches a path on.
    /// </remarks>
    private static string? TextOf(object? value) => value switch
    {
        null => null,
        string text => text,
        bool flag => flag ? "True" : "False",
        IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
        _ => value.ToString(),
    };

    /// <summary>The parameters an operation declares.</summary>
    /// <param name="operation">The operation.</param>
    /// <returns>The parameters.</returns>
    private static List<JsonObject> ParametersOf(JsonObject operation) =>
        operation["parameters"] is JsonArray parameters
            ? [.. parameters.OfType<JsonObject>()]
            : [];

    /// <summary>The name the endpoint is served under.</summary>
    /// <param name="path">The path.</param>
    /// <param name="method">The method.</param>
    /// <returns>The name.</returns>
    /// <remarks>
    /// The Python version names the endpoint after the function it builds, which it names after the
    /// method and the path with the slashes and braces taken out: GET <c>/books/{id}</c> is
    /// <c>get__books_id</c>.
    /// </remarks>
    private static string ApiNameOf(string path, string method) => $"{method}_{SlugOf(path)}";

    /// <summary>The path as a name: its slashes and braces are not part of one.</summary>
    /// <param name="path">The path.</param>
    /// <returns>The name.</returns>
    /// <remarks>
    /// The Python version writes it three times — in the endpoint's name, in its heading's anchor
    /// and in the panel's links — and the three have to agree or a link leads nowhere.
    /// </remarks>
    private static string SlugOf(string path) =>
        path.Replace("/", "_", StringComparison.Ordinal)
            .Replace("{", string.Empty, StringComparison.Ordinal)
            .Replace("}", string.Empty, StringComparison.Ordinal);

    /// <summary>What the API document says about the endpoint.</summary>
    /// <param name="operation">The operation.</param>
    /// <returns>The description.</returns>
    private static string DescriptionOf(JsonObject operation) =>
        JsonValues.Text(operation["description"])
        ?? JsonValues.Text(operation["summary"])
        ?? string.Empty;

    /// <summary>The anchor the panel's link and the heading share.</summary>
    /// <param name="method">The method.</param>
    /// <param name="path">The path.</param>
    /// <returns>The anchor.</returns>
    private static string AnchorOf(string method, string path) =>
        $"{method.ToUpperInvariant()}_{SlugOf(path)}";

    /// <summary>The heading that introduces an endpoint, in the Python version's own
    /// markup.</summary> <param name="path">The path.</param> <param name="method">The
    /// method.</param> <returns>The markdown.</returns>
    private static string HeadingOf(string path, string method) =>
        $"<h2 id='{AnchorOf(method, path)}'>{MethodBox(method)} "
        + $"<code style='font-size: inherit;'>{path}</code></h2>";

    /// <summary>The method's name in a box coloured by what it does, as the Python version draws
    /// it.</summary> <param name="method">The method.</param> <returns>The markup.</returns>
    private static string MethodBox(string method)
    {
        var colour = method.ToUpperInvariant() switch
        {
            "GET" => "#61affe",
            "POST" => "#49cc90",
            "PUT" => "#fca130",
            "DELETE" => "#f93e3e",
            "PATCH" => "#50e3c2",
            _ => "#999",
        };

        return "<span style='display:inline-block;min-width:48px;padding:2px 10px;border-radius:4px;"
            + $"background:{colour};color:white;font-weight:bold;font-family:monospace;"
            + $"margin-right:8px;text-align:center;border:2px solid {colour};"
            + "box-shadow:0 1px 2px rgba(0,0,0,0.08);'>"
            + $"{method.ToUpperInvariant()}</span>";
    }

    /// <summary>An example body as the text its field holds.</summary>
    /// <param name="example">The example.</param>
    /// <returns>The text.</returns>
    /// <remarks>
    /// The Python version writes it with <c>json.dumps(indent=2)</c>, so the reader gets a document
    /// they can read rather than one line of it — with the line breaks that function writes, which
    /// are <c>\n</c> on every platform. A text box holding <c>\r\n</c> would serve a document the
    /// Python version does not.
    /// </remarks>
    private static string IndentedText(JsonNode example) =>
        example.ToJsonString(IndentedOptions).Replace("\r\n", "\n", StringComparison.Ordinal);

    /// <summary>How an example is written: indented, and escaped the way every served document is.</summary>
    private static readonly JsonSerializerOptions IndentedOptions = new()
    {
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true,
    };
}

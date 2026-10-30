using System.Collections.Frozen;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// The machine-readable descriptions of the application: what it can be asked to do, and the
/// OpenAPI document that describes the same thing to a client that has never heard of Gradio.
/// </summary>
/// <remarks>
/// <para>
/// Both are built from the application's event bindings, which is why they arrive with them: an
/// application that binds no event describes no endpoint, and one that binds three describes three.
/// The two read the same binding through <see cref="EndpointOf"/> and the same parameter and return
/// helpers, so an endpoint cannot be described as taking one thing in the API info and another in
/// the OpenAPI document.
/// </para>
/// <para>
/// The shape is the Python version's, because the clients read it: the <c>gradio_client</c>
/// packages parse these documents to build their calls, so a field that is present here and absent
/// there is a client that cannot call the application.
/// </para>
/// </remarks>
public sealed partial class Blocks
{
    /// <summary>
    /// The document <c>/gradio_api/info</c> serves: the application's callable endpoints.
    /// </summary>
    /// <param name="root">The address the application is served from.</param>
    /// <param name="page">
    /// The path of the page whose endpoints to describe, or <see langword="null"/> for the
    /// application's.
    /// </param>
    /// <param name="allEndpoints">
    /// Whether to describe the endpoints the documentation leaves out — the Python version's
    /// <c>all_endpoints</c>, which the API documentation page asks for so that an
    /// <c>undocumented</c> endpoint still appears in it.
    /// </param>
    /// <returns>
    /// The Python version's two groups. Every binding that is an endpoint has an entry in the
    /// first, keyed by its name with a leading slash; the second is empty, because the Python
    /// version only fills it for a binding whose name is not a name, and this framework has no such
    /// case.
    /// </returns>
    /// <remarks>
    /// <para>
    /// A page is served the endpoints bound on it rather than the application's, because that is
    /// what the page's own copy of the document carries: a client that reads it is being told what
    /// it can call from the page it is looking at.
    /// </para>
    /// <para>
    /// A page this application does not have describes the whole application instead, which is the
    /// Python version's answer: the page argument narrows the document when it names something, and
    /// is ignored when it does not.
    /// </para>
    /// </remarks>
    public JsonObject GetApiInfo(string root, string? page = null, bool allEndpoints = false)
    {
        ArgumentNullException.ThrowIfNull(root);

        var narrowed = page is not null && _pages.Any(candidate => candidate.Path == page);
        var named = new JsonObject();

        foreach (var (dependency, endpoint) in EndpointEntries(allEndpoints))
        {
            if (narrowed && dependency.PagePath != page)
            {
                continue;
            }

            // The example calls belong to this document rather than to the endpoint: they show a
            // client how to reach the address the application is being served from, which the host
            // chooses and this document is built against.
            endpoint["code_snippets"] = CodeSnippets(
                $"/{dependency.ApiName}",
                root,
                endpoint["parameters"]!.AsArray());

            named[$"/{dependency.ApiName}"] = endpoint;
        }

        return new JsonObject
        {
            ["named_endpoints"] = named,
            ["unnamed_endpoints"] = new JsonObject(),
        };
    }

    /// <summary>
    /// The application's endpoints, each with the document that describes it.
    /// </summary>
    /// <param name="allEndpoints">
    /// Whether to include the endpoints the documentation leaves out — the Python version's
    /// <c>all_endpoints</c>.
    /// </param>
    /// <returns>One entry per endpoint, in the order the bindings were set up.</returns>
    /// <remarks>
    /// The API info and the MCP catalog are built from this list rather than from the bindings
    /// directly, which is what makes the two agree: an endpoint a client can call is an endpoint an
    /// agent can call, and one the application hid is hidden from both. The endpoint objects are
    /// freshly built for each caller, so a caller that adds to one — the example calls, the tool's
    /// input schema — is not changing what another sees.
    /// </remarks>
    internal IReadOnlyList<(Dependency Dependency, JsonObject Endpoint)> DocumentedEndpoints(bool allEndpoints = false) =>
        [.. EndpointEntries(allEndpoints)];

    /// <summary>The endpoints, lazily.</summary>
    /// <param name="allEndpoints">Whether to include the ones the documentation leaves out.</param>
    /// <returns>One entry per endpoint.</returns>
    private IEnumerable<(Dependency Dependency, JsonObject Endpoint)> EndpointEntries(bool allEndpoints)
    {
        foreach (var dependency in _dependencies)
        {
            if (EndpointOf(dependency, allEndpoints) is { } endpoint)
            {
                yield return (dependency, endpoint);
            }
        }
    }

    /// <summary>
    /// The document <c>/gradio_api/openapi.json</c> serves.
    /// </summary>
    /// <returns>
    /// The OpenAPI description of the application's HTTP API, with one pair of paths per endpoint —
    /// the one that starts a run and the one that collects its result. The upload path is part of
    /// it whether or not the application binds an event: the Python version describes it
    /// unconditionally, because a file input is served by that endpoint rather than by the
    /// application's own.
    /// </returns>
    public JsonObject GetOpenApiDocument(string root)
    {
        ArgumentNullException.ThrowIfNull(root);

        var paths = new JsonObject
        {
            ["/gradio_api/upload"] = UploadPath(),
        };

        foreach (var dependency in _dependencies)
        {
            if (EndpointOf(dependency, allEndpoints: false) is not { } endpoint)
            {
                continue;
            }

            var name = dependency.ApiName;

            // The example calls are the API info's alone: the OpenAPI document describes what an
            // endpoint takes and returns rather than how to call it from one client or another.
            paths[$"/gradio_api/call/v2/{name}"] = CallPath(
                name,
                endpoint["description"]!.GetValue<string>(),
                endpoint["parameters"]!.AsArray(),
                endpoint["returns"]!.AsArray());

            paths[$"/gradio_api/call/{name}/{{event_id}}"] = ResultPath(name);
        }

        return new JsonObject
        {
            ["openapi"] = "3.0.2",
            ["info"] = new JsonObject
            {
                ["title"] = Title,
                ["description"] = Description,
                ["version"] = GradioVersion.Upstream,
            },
            ["paths"] = paths,
            ["components"] = new JsonObject
            {
                ["schemas"] = new JsonObject(),
            },
        };
    }

    /// <summary>
    /// One endpoint's entry in the API info, or <see langword="null"/> when the binding is not an
    /// endpoint.
    /// </summary>
    /// <remarks>
    /// A binding with no handler has nothing to call, and a <c>private</c> one is deliberately not
    /// described. An <c>undocumented</c> one is described only when the caller asks for every
    /// endpoint, which is what the documentation page does and what a plain request does not: the
    /// name means exactly that — reachable, and left out of the description unless it is asked for.
    /// What an endpoint takes and returns is the same in both documents; only the API info adds the
    /// example calls, which need the address the application is served from.
    /// </remarks>
    private static JsonObject? EndpointOf(Dependency dependency, bool allEndpoints)
    {
        if (!dependency.BackendFunction)
        {
            return null;
        }

        if (dependency.ApiVisibility == ApiVisibility.Private
            || (dependency.ApiVisibility == ApiVisibility.Undocumented && !allEndpoints))
        {
            return null;
        }

        // A component that is not part of the API is not part of the description of it either: a
        // state carries a value between the steps of a chain, and a client neither sends nor reads
        // one. The Python version skips them where it builds both documents, which is why a chat's endpoint
        // takes the message and not the conversation it is added to.
        var parameters = new JsonArray();
        var position = 0;
        foreach (var input in dependency.Inputs)
        {
            if (!input.SkipApi)
            {
                parameters.Add(ParameterOf(input, position, dependency.ParameterNames));
            }

            position++;
        }

        var returns = new JsonArray();
        foreach (var output in dependency.Outputs)
        {
            if (!output.SkipApi)
            {
                returns.Add(ReturnOf(output));
            }
        }

        return new JsonObject
        {
            ["parameters"] = parameters,
            ["returns"] = returns,
            ["api_visibility"] = Dependency.ApiVisibilityName(dependency.ApiVisibility),
            ["description"] = dependency.DocumentationDescription,
        };
    }

    /// <summary>One input component's entry in an endpoint's parameters.</summary>
    /// <param name="block">The component.</param>
    /// <param name="index">Its position, which names it when the binding gives it no name.</param>
    /// <param name="names">The names the binding gives its inputs, or <see langword="null"/> for none.</param>
    private static JsonObject ParameterOf(Block block, int index, IReadOnlyList<string?>? names)
    {
        var component = (Component)block;
        var info = component.ApiInfoAsInput;
        var props = PropsOf(component);

        // The Python version names an input after the handler's own parameter, which a .NET delegate does not
        // carry — reflection over one has no source to read the names from. A binding that wants its
        // endpoint called by name gives them through ParameterNames; one that does not is named by
        // position, which is the Python version's own fallback for a handler whose parameters cannot be read.
        var name = names is not null && index < names.Count && !string.IsNullOrEmpty(names[index])
            ? names[index]!
            : $"param_{index}";

        // A parameter is optional when its component already holds a value, because that value is
        // what a client leaves the argument out for. The Python version reads the value out of the component's
        // props, which is why an empty conversation counts as a default and an empty field does not.
        var value = props["value"];

        return new JsonObject
        {
            ["label"] = LabelOf(props, $"parameter_{block.Id}"),
            ["parameter_name"] = name,
            ["parameter_has_default"] = value is not null,
            ["parameter_default"] = value?.DeepClone(),
            ["type"] = info.DeepClone(),
            ["python_type"] = new JsonObject
            {
                ["type"] = JsonSchemaTypes.PythonType(info),

                // The Python version reports the schema's own description here, which is absent for most
                // components and explicitly null for a conversation: the schema pops the docstring
                // and writes it back under this name whether or not there was one.
                ["description"] = info.TryGetPropertyValue("additional_description", out var description)
                    ? description?.DeepClone()
                    : JsonValue.Create(string.Empty),
            },
            ["component"] = Capitalize(component.BlockClass),
            ["example_input"] = component.ExampleInputs?.DeepClone(),
        };
    }

    /// <summary>One output component's entry in an endpoint's returns.</summary>
    /// <param name="block">The component.</param>
    private static JsonObject ReturnOf(Block block)
    {
        var component = (Component)block;
        var info = component.ApiInfoAsOutput;

        return new JsonObject
        {
            ["label"] = LabelOf(PropsOf(component), $"value_{block.Id}"),
            ["type"] = info.DeepClone(),
            ["python_type"] = new JsonObject
            {
                ["type"] = JsonSchemaTypes.PythonType(info),
                ["description"] = info["description"]?.DeepClone() ?? JsonValue.Create(string.Empty),
            },
            ["component"] = Capitalize(component.BlockClass),
        };
    }

    /// <summary>The props a component describes itself with, as the config serves them.</summary>
    /// <param name="component">The component.</param>
    /// <returns>The props.</returns>
    /// <remarks>
    /// The API documents read a component the way the config does — through the props it wrote —
    /// because that is what the Python version reads: both documents are built from the served
    /// config, so a value the config carries and the documents do not is a value no client would
    /// see.
    /// </remarks>
    private static JsonObject PropsOf(Component component)
    {
        var props = new PropWriter();
        component.WriteProps(props);

        return props.Build();
    }

    /// <summary>
    /// A component's label, or the fallback the Python version uses for one that has none.
    /// </summary>
    /// <param name="props">The props the component wrote.</param>
    /// <param name="fallback">The name to use when the component carries no label at all.</param>
    /// <returns>The label: the text, the translation a default label names, or the fallback.</returns>
    /// <remarks>
    /// <para>
    /// The label is a prop rather than a member of every component — a number field and a textbox
    /// both have one, a plot does not — so it is read back out of the props rather than reached for
    /// through an interface that not every component implements.
    /// </para>
    /// <para>
    /// A default label is served here as the translation metadata it is, while the config serves
    /// the same label as the string the browser translates. That is the Python version's split, not
    /// an accident of this implementation: the API documents are built before the config is
    /// serialized, so the object is still an object, and a client reading the document is told
    /// which message the label names rather than which message to show.
    /// </para>
    /// <para>
    /// The fallback is used only when the key is absent. A component that carries a label of
    /// <see langword="null"/> has a label, and it is null — which is what the Python version's
    /// <c>dict.get</c> does with a key that is there.
    /// </para>
    /// </remarks>
    private static JsonNode? LabelOf(JsonObject props, string fallback)
    {
        if (!props.TryGetPropertyValue("label", out var label))
        {
            return JsonValue.Create(fallback);
        }

        if (label is JsonValue text && text.TryGetValue<string>(out var value)
            && TranslationKey.TryParse(value, out var key))
        {
            return new JsonObject
            {
                ["key"] = key,
                ["_type"] = "translation_metadata",
            };
        }

        return label?.DeepClone();
    }

    /// <summary>What an endpoint's summary adds when it takes a file, in the Python version's
    /// words.</summary>
    private const string FileUploadNote =
        ". File inputs must first be uploaded via POST /gradio_api/upload (multipart/form-data with"
        + " a \"files\" field). Use the returned path in the request body as {\"path\":"
        + " \"<uploaded_path>\", \"meta\": {\"_type\": \"gradio.FileData\"}}.";

    /// <summary>The path that starts a run, as the OpenAPI document describes it.</summary>
    /// <param name="name">The endpoint's name, without a leading slash.</param>
    /// <param name="description">The endpoint's own description, which may be empty.</param>
    /// <param name="parameters">The endpoint's parameters, as the API info describes them.</param>
    /// <param name="returns">The endpoint's returns, as the API info describes them.</param>
    /// <remarks>
    /// The request body is the API info's parameters with their references resolved and their files
    /// described the way a caller sends one, which is the Python version's <c>_condense_info</c>: a
    /// client generating a call from this document wants the shape it has to send, not the shape
    /// the server keeps. The summary falls back to naming the endpoint, because an endpoint with no
    /// description still has to be called something in a generated client.
    /// </remarks>
    private static JsonObject CallPath(string name, string description, JsonArray parameters, JsonArray returns)
    {
        var properties = new JsonObject();
        var takesAFile = false;

        foreach (var parameter in parameters)
        {
            var parameterName = parameter!["parameter_name"]!.GetValue<string>();
            var schema = RequestSchemaOf(parameter["type"]);

            properties[parameterName] = schema;

            // A file is rewritten as the string a caller sends, and the format is what says so —
            // the Python version reads the simplified schema, where a file is still named as one.
            takesAFile |= JsonValues.Text(schema?["format"]) == "filepath";
        }

        var content = new JsonObject
        {
            ["schema"] = new JsonObject
            {
                ["type"] = "object",
                ["properties"] = properties,
            },
        };

        var described = new StringBuilder(description)
            .Append("Output must be fetched from GET /gradio_api/call/")
            .Append(name)
            .Append("/{event_id}. Returns an array of ")
            .Append(returns.Count)
            .Append(" elements of the following format: ")
            .Append(ReturnsOf(returns))
            .ToString();

        // An endpoint that takes a file is called with a path the caller has already uploaded, which
        // is a thing a generated client cannot guess from a string schema — so the summary says it.
        var summary = description.Length > 0 ? description : $"Endpoint /{name}";

        if (takesAFile)
        {
            summary += FileUploadNote;
        }

        return new JsonObject
        {
            ["post"] = new JsonObject
            {
                ["summary"] = summary,
                ["description"] = described,
                ["operationId"] = name,
                ["requestBody"] = new JsonObject
                {
                    ["required"] = true,
                    ["content"] = new JsonObject
                    {
                        ["application/json"] = content,
                    },
                },
                ["responses"] = new JsonObject
                {
                    ["200"] = new JsonObject
                    {
                        ["description"] = "Successful response",
                        ["content"] = new JsonObject
                        {
                            ["application/json"] = new JsonObject
                            {
                                ["schema"] = new JsonObject
                                {
                                    ["type"] = "object",
                                    ["properties"] = new JsonObject
                                    {
                                        ["event_id"] = new JsonObject { ["type"] = "string" },
                                    },
                                },
                            },
                        },
                    },
                },
            },
        };
    }

    /// <summary>One parameter's schema, as the request body describes it.</summary>
    /// <param name="type">The parameter's schema, as the API info carries it.</param>
    /// <returns>The schema a caller sends.</returns>
    /// <remarks>
    /// The Python version's last three adjustments, in its order: the description the schema keeps
    /// for the API info is dropped, an object that names only its properties is given its type, and
    /// a file is described as the string a caller passes rather than as the object the server
    /// keeps.
    /// </remarks>
    private static JsonObject? RequestSchemaOf(JsonNode? type)
    {
        if (JsonSchemaTypes.Simplify(type, isInput: true) is not JsonObject schema)
        {
            return null;
        }

        schema.Remove("additional_description");

        if (schema.ContainsKey("properties") && !schema.ContainsKey("type"))
        {
            schema["type"] = "object";
        }

        if (JsonValues.Text(schema["type"]) == "filepath")
        {
            schema["type"] = "string";
            schema["format"] = "filepath";
        }

        return schema;
    }

    /// <summary>
    /// How the Python version spells the shape of an endpoint's result in the call description.
    /// </summary>
    /// <remarks>
    /// The result is a JSON array of strings, and each schema inside one is rendered the way Python
    /// renders a dictionary — single-quoted keys, a space after each colon — because that is what
    /// the description is: a Python value interpolated into a string, which a client reading the
    /// description sees verbatim. The schemas are the ones a caller receives, so a file in a result
    /// is described as the address it is fetched from rather than as the object.
    /// </remarks>
    private static string ReturnsOf(JsonArray returns)
    {
        var described = new List<string>(returns.Count);

        for (var index = 0; index < returns.Count; index++)
        {
            var name = index == 0 ? "output" : $"output_{index}";
            var type = JsonSchemaTypes.Simplify(returns[index]!["type"], isInput: false)?.AsObject();

            described.Add(
                type is null
                    ? $"{name} ({{}})"
                    : $"{name} ({{{string.Join(", ", type.Select(pair => $"'{pair.Key}': {PythonLiteral(pair.Value)}"))}}})");
        }

        // Spelled out rather than serialized: the default serializer escapes the single quotes the
        // schemas are rendered with, and what a client reads is the text of the description, not a
        // JSON value it will be parsed back out of.
        return "[" + string.Join(", ", described.Select(item => $"\"{item}\"")) + "]";
    }

    /// <summary>A JSON value as Python would print it inside a dictionary.</summary>
    private static string PythonLiteral(JsonNode? value) => value switch
    {
        null => "None",
        JsonValue json when json.TryGetValue<string>(out var text) => $"'{text}'",
        JsonValue json when json.TryGetValue<bool>(out var flag) => flag ? "True" : "False",
        _ => value.ToJsonString(),
    };

    /// <summary>The path a run's result is collected from.</summary>
    private static JsonObject ResultPath(string name) => new()
    {
        ["get"] = new JsonObject
        {
            ["summary"] = $"Fetch results for /{name}",
            ["description"] = "Returns a stream of server-sent events (SSE). The final event has `event: complete` with `data` containing a JSON array of outputs.",
            ["operationId"] = $"{name}_get",
            ["parameters"] = new JsonArray
            {
                new JsonObject
                {
                    ["name"] = "event_id",
                    ["in"] = "path",
                    ["required"] = true,
                    ["schema"] = new JsonObject { ["type"] = "string" },
                    ["description"] = "The event_id returned by the POST request",
                },
            },
            ["responses"] = new JsonObject
            {
                ["200"] = new JsonObject
                {
                    ["description"] = "SSE stream with event: complete containing a JSON array of outputs",
                    ["content"] = new JsonObject
                    {
                        ["text/event-stream"] = new JsonObject
                        {
                            ["schema"] = new JsonObject { ["type"] = "string" },
                        },
                    },
                },
            },
        },
    };

    /// <summary>
    /// The snippets the API documentation shows a caller, one per client.
    /// </summary>
    /// <param name="name">The endpoint's name, with a leading slash.</param>
    /// <param name="root">The address the application is served from.</param>
    /// <param name="parameters">The endpoint's parameters, read for their names and examples.</param>
    /// <returns>The snippets, keyed by client.</returns>
    /// <remarks>
    /// The address in them is the address the application is being served from, which the host
    /// chooses, so it belongs to the run rather than to the application.
    /// </remarks>
    private static JsonObject CodeSnippets(string name, string root, JsonArray parameters)
    {
        var pairs = new List<SnippetArgument>();
        var cliPairs = new List<(string Name, JsonNode? Value)>();

        foreach (var parameter in parameters)
        {
            var parameterName = parameter!["parameter_name"]!.GetValue<string>();
            var pythonType = JsonValues.Text(parameter["python_type"]?["type"]);

            pairs.Add(new SnippetArgument(
                parameterName,
                ValueOf(parameter),
                pythonType,
                JsonValues.Text(parameter["component"]),
                parameter["example_input"]?.DeepClone()));

            cliPairs.Add((parameterName, CliValueOf(parameter)));
        }

        // A parameter whose example is a file is one the caller uploads rather than one it sends as
        // JSON, and every snippet says so in its own dialect: the Python client wraps the address in
        // `handle_file`, the JavaScript one fetches the file into a blob first, the shell uploads it
        // and sends the name back, and the command line simplifies the object to its path.
        //
        // The example decides this, and the value the snippet shows is the default when there is one
        // — the Python version's own split, which is why a parameter whose default is a file but whose example
        // is not can be shown with `handle_file` in a snippet that did not import it. The quirk is
        // the Python version's and is reproduced rather than tidied: a reader comparing the two documents
        // would otherwise find a difference where the applications are the same.
        var fileNames = pairs
            .Where(pair => HasFileData(pair.Example))
            .Select(pair => pair.Name)
            .ToHashSet(StringComparer.Ordinal);

        var blobs = pairs
            .Where(pair => pair.Component is { } component && BlobComponents.Contains(component))
            .ToList();

        var pythonArguments = pairs.Count == 0
            ? $"\tapi_name=\"{name}\",\n"
            : "\t" + string.Join(
                ",\n\t",
                pairs.Select(pair => $"{pair.Name}={Represent(pair, SnippetLanguage.Python)}"))
                + $",\n\tapi_name=\"{name}\",\n";

        var javascriptArguments = pairs.Count == 0
            ? "\n"
            : "\n\t\t" + string.Join(
                ",\n\t\t",
                pairs.Select(pair => $"{pair.Name}: {JavaScriptValue(pair)}")) + ",\n";

        var body = "{"
            + string.Join(
                ", ",
                pairs.Select(pair => fileNames.Contains(pair.Name)
                    ? $"\"{pair.Name}\": {FilePlaceholder}"
                    : $"\"{pair.Name}\": {Represent(pair, SnippetLanguage.Bash)}"))
            + "}";

        // Every parameter that carries a file sends the same object, so one substitution fills them
        // all: the shell snippet uploads once and the address is what the body names.
        body = body.Replace(FilePlaceholder, UploadedFile, StringComparison.Ordinal);

        // The command-line snippet is the one that shows the body as a reader would type it, so it
        // is the one that is written out indented rather than on one line — and written the way
        // Python writes it, because a value in it can be any text an application asked for: a
        // quote inside a label reaches the reader as \" rather than as \u0022.
        var cliBody = cliPairs.Count == 0
            ? "{}"
            : PythonJson.Dump(
                new JsonObject([.. cliPairs.Select(pair => KeyValuePair.Create(pair.Name, SimplifyFiles(pair.Value)))]),
                indent: 2);

        var pythonImports = fileNames.Count == 0
            ? "from gradio_client import Client\n"
            : "from gradio_client import Client, handle_file\n";

        var javascriptImports = blobs.Count == 0
            ? "import { Client } from \"@gradio/client\";\n\n"
            : "import { Client, handle_file } from \"@gradio/client\";\n\n"
                + string.Concat(blobs.Select((blob, index) =>
                    $"const response_{index} = await fetch(\"{FileAddress(blob.Example)}\");\n"
                    + $"const example{blob.Component} = await response_{index}.blob();\n"))
                + "\n";

        var upload = fileNames.Count == 0
            ? string.Empty
            : "FILE_PATH=$(curl -s -X POST " + root + "/gradio_api/upload"
                + " -F 'files=@/path/to/your/file'"
                + " | tr -d '[]\" ')\n\n";

        return new JsonObject
        {
            ["python"] =
                pythonImports
                + "\nclient = Client(\"" + root + "\")\n"
                + "result = client.predict(\n"
                + pythonArguments
                + ")\nprint(result)",
            ["javascript"] =
                javascriptImports
                + "const client = await Client.connect(\"" + root + "\");\n"
                + "const result = await client.predict(\"" + name + "\", {" + javascriptArguments + "});\n\n"
                + "console.log(result.data);",
            ["bash"] =
                upload
                + "curl -X POST " + root + "/gradio_api/call/v2/" + name.TrimStart('/') + " -s -H \"Content-Type: application/json\" \\\n"
                + "  -d '" + body + "' \\\n"
                + "  | awk -F'\"' '{ print $4}' \\\n"
                + "  | read EVENT_ID; curl -N " + root + "/gradio_api/call/" + name.TrimStart('/') + "/$EVENT_ID",
            ["cli"] = "{command} predict " + root + " " + name + " '" + cliBody + "'\n",
        };
    }

    /// <summary>The components whose value is a file the caller uploads.</summary>
    /// <remarks>
    /// The Python version's <c>BLOB_COMPONENTS</c>, which is what decides whether the JavaScript
    /// snippet fetches a blob before it calls the endpoint. The list is the components that carry a
    /// file rather than a value, spelled as the config spells them.
    /// </remarks>
    private static readonly FrozenSet<string> BlobComponents =
        new[] { "Audio", "DownloadButton", "File", "Image", "ImageSlider", "Model3D", "UploadButton", "Video" }
            .ToFrozenSet(StringComparer.Ordinal);

    /// <summary>Where a shell snippet's uploaded file is put, before it is replaced by its name.</summary>
    private const string FilePlaceholder = "FILE_PATH_PLACEHOLDER";

    /// <summary>What a shell snippet sends in place of a file parameter.</summary>
    private const string UploadedFile = "{\"path\": \"'$FILE_PATH'\", \"meta\": {\"_type\": \"gradio.FileData\"}}";

    /// <summary>One argument as the JavaScript client spells it.</summary>
    /// <param name="argument">The argument.</param>
    /// <returns>The value, as the reader would type it.</returns>
    /// <remarks>
    /// A component that carries a file is called with the blob the snippet fetched above rather
    /// than with its value: the client uploads what it is handed, so handing it the address of the
    /// example would be the wrong call. Two file parameters of the same component would be given
    /// the same variable name, which is the Python version's spelling of the same snippet.
    /// </remarks>
    private static string JavaScriptValue(SnippetArgument argument) =>
        argument.Component is { } component && BlobComponents.Contains(component)
            ? $"handle_file(example{component})"
            : Represent(argument, SnippetLanguage.JavaScript);

    /// <summary>The address a file object names.</summary>
    /// <param name="value">The value.</param>
    /// <returns>The address, or the empty string when the value names none.</returns>
    private static string FileAddress(JsonNode? value) =>
        value is JsonObject file ? JsonValues.Text(file["url"]) ?? string.Empty : string.Empty;

    /// <summary>A value with every file object replaced by the path it names.</summary>
    /// <param name="value">The value.</param>
    /// <returns>The value, with its files reduced.</returns>
    /// <remarks>
    /// The command line's body is a JSON document a caller retypes, and a file in it is the address
    /// it was fetched from — the rest of the object is what a client fills in when it uploads.
    /// </remarks>
    private static JsonNode? SimplifyFiles(JsonNode? value) => value switch
    {
        JsonObject file when IsFileData(file) => new JsonObject
        {
            ["path"] = FileAddress(file),
            ["meta"] = new JsonObject { ["_type"] = "gradio.FileData" },
        },
        JsonObject map => new JsonObject([.. map.Select(pair => KeyValuePair.Create(pair.Key, SimplifyFiles(pair.Value)))]),
        JsonArray list => new JsonArray([.. list.Select(SimplifyFiles)]),
        _ => value?.DeepClone(),
    };

    /// <summary>Whether a value is a file object, as the protocol carries one.</summary>
    /// <param name="value">The value.</param>
    /// <returns>Whether it names a file.</returns>
    private static bool IsFileData(JsonNode? value) =>
        value is JsonObject file
        && JsonValues.Text(file["url"]) is { Length: > 0 }
        && file["meta"] is JsonObject meta
        && JsonValues.Text(meta["_type"]) == "gradio.FileData";

    /// <summary>Whether a value holds a file object anywhere inside it.</summary>
    /// <param name="value">The value.</param>
    /// <returns>Whether it names a file.</returns>
    private static bool HasFileData(JsonNode? value) => value switch
    {
        null => false,
        JsonObject file when IsFileData(file) => true,
        JsonObject map => map.Any(pair => HasFileData(pair.Value)),
        JsonArray list => list.Any(HasFileData),
        _ => false,
    };

    /// <summary>The value a snippet shows for one parameter.</summary>
    /// <param name="parameter">The parameter, as the API info describes it.</param>
    /// <returns>The value a caller would send.</returns>
    /// <remarks>
    /// A parameter that has a default shows that default, because it is what a caller who says
    /// nothing sends; one that has none shows the component's example. The Python version makes the
    /// same choice, which is why a chat's endpoint is documented as being called with an empty
    /// conversation and a text field's with its example text.
    /// </remarks>
    private static JsonNode? ValueOf(JsonNode parameter) =>
        parameter["parameter_has_default"]?.GetValue<bool>() == true
            ? parameter["parameter_default"]?.DeepClone()
            : parameter["example_input"]?.DeepClone();

    /// <summary>The value the command-line snippet shows for one parameter.</summary>
    /// <param name="parameter">The parameter, as the API info describes it.</param>
    /// <returns>The value a caller would send.</returns>
    /// <remarks>
    /// The command-line snippet is generated by a different part of the Python version's tooling,
    /// and it picks the example whenever the default is empty — an empty conversation is a default
    /// that says nothing about how the endpoint is called, so the example is what a reader is
    /// shown.
    /// </remarks>
    private static JsonNode? CliValueOf(JsonNode parameter) =>
        ValueOf(parameter) is { } value && !IsEmpty(value)
            ? value
            : parameter["example_input"]?.DeepClone();

    /// <summary>Whether a value is one Python would treat as absent.</summary>
    /// <param name="value">The value.</param>
    /// <returns>Whether it is empty.</returns>
    private static bool IsEmpty(JsonNode value) => value switch
    {
        JsonArray array => array.Count == 0,
        JsonObject document => document.Count == 0,
        JsonValue scalar when scalar.TryGetValue<bool>(out var flag) => !flag,
        JsonValue scalar when scalar.TryGetValue<string>(out var text) => text.Length == 0,
        JsonValue scalar when scalar.TryGetValue<double>(out var number) => number == 0,
        _ => false,
    };

    /// <summary>
    /// One argument of a snippet: what a caller sends for it, and the type the API info says it is.
    /// </summary>
    /// <param name="Name">The parameter's name.</param>
    /// <param name="Value">The value the snippet shows.</param>
    /// <param name="PythonType">
    /// The parameter's Python type, as the API info spells it, or <see langword="null"/> for one it
    /// does not name.
    /// </param>
    /// <param name="Component">
    /// The component the argument is read from, as the API info names it, or <see langword="null"/>
    /// for one it does not.
    /// </param>
    /// <param name="Example">
    /// The value the API info shows as an example, which is where a file argument is recognised.
    /// </param>
    /// <remarks>
    /// The type is what decides how a value is written, which is the Python version's rule and the
    /// reason it is carried here rather than inferred from the value: a text field's example is
    /// written in quotes because a caller sends a string, while an endpoint derived from
    /// annotations shows <c>...</c> bare because the argument is not text at all.
    /// </remarks>
    private readonly record struct SnippetArgument(
        string Name,
        JsonNode? Value,
        string? PythonType,
        string? Component,
        JsonNode? Example);

    /// <summary>The languages a snippet is written in.</summary>
    private enum SnippetLanguage
    {
        /// <summary>Python, where a value that is not there is <c>None</c> and a flag is
        /// <c>True</c>.</summary>
        Python,

        /// <summary>JavaScript, where those two are <c>null</c> and <c>true</c>.</summary>
        JavaScript,

        /// <summary>The shell, where the body is JSON.</summary>
        Bash,
    }

    /// <summary>One argument as a snippet spells it.</summary>
    /// <param name="argument">The argument.</param>
    /// <param name="language">The language the snippet is written in.</param>
    /// <returns>The value, as the reader would type it.</returns>
    /// <remarks>
    /// <para>
    /// The Python version's <c>_represent_value</c>. A value is written for its type rather than
    /// for its shape: a string is quoted, a number is bare, a flag is spelled the way the language
    /// spells one, and anything else is left as the text it is — which is how the placeholder
    /// <c>...</c> reaches a reader without quotes around it.
    /// </para>
    /// <para>
    /// A value that is not there is <c>None</c> in Python and <c>null</c> everywhere else, and an
    /// empty string is treated as one, because that is what the Python version does with the empty
    /// default a component happens to carry.
    /// </para>
    /// </remarks>
    private static string Represent(SnippetArgument argument, SnippetLanguage language)
    {
        var absent = language == SnippetLanguage.Python ? "None" : "null";
        var type = argument.PythonType;

        if (type is null || argument.Value is null)
        {
            return absent;
        }

        if (type is "string" or "str")
        {
            return $"\"{Text(argument.Value)}\"";
        }

        if (type is "number")
        {
            return Text(argument.Value);
        }

        if (type is "boolean" or "bool")
        {
            var flag = argument.Value.GetValue<bool>();

            return language == SnippetLanguage.Python
                ? (flag ? "True" : "False")
                : (flag ? "true" : "false");
        }

        if (type == "List[str]")
        {
            return Spaced(argument.Value);
        }

        if (type.StartsWith("Literal['", StringComparison.Ordinal))
        {
            return $"\"{Text(argument.Value)}\"";
        }

        if (argument.Value is JsonValue scalar && scalar.TryGetValue<string>(out var text))
        {
            return text.Length == 0 ? absent : text;
        }

        // A value that names a file is written in the dialect of the client that can upload one:
        // the Python client wraps the address in the call that sends it, the shell sends the object
        // with its address, and the others send the object as the protocol carries it.
        if (HasFileData(argument.Value))
        {
            return language switch
            {
                SnippetLanguage.Python => PythonSnippetValue(argument.Value),
                SnippetLanguage.Bash => Spaced(SimplifyFiles(argument.Value)),
                _ => Spaced(argument.Value),
            };
        }

        return Spaced(argument.Value);
    }

    /// <summary>A value as Python writes it into a call, with each file written as its
    /// upload.</summary> <param name="value">The value.</param> <returns>The text.</returns>
    /// <remarks>
    /// The Python version's <c>_stringify_py</c> after its files have been replaced: a JSON
    /// document with Python's spellings for the three values it does not share with JSON, and a
    /// file written as the call that uploads it rather than as the object that describes it.
    /// </remarks>
    private static string PythonSnippetValue(JsonNode? value) => value switch
    {
        null => "None",
        JsonObject file when IsFileData(file) => $"handle_file('{FileAddress(file)}')",
        JsonObject map => "{"
            + string.Join(", ", map.Select(pair => $"\"{pair.Key}\": {PythonSnippetValue(pair.Value)}"))
            + "}",
        JsonArray list => "[" + string.Join(", ", list.Select(PythonSnippetValue)) + "]",
        JsonValue scalar when scalar.TryGetValue<bool>(out var flag) => flag ? "True" : "False",
        JsonValue scalar when scalar.TryGetValue<string>(out var text) => JsonText.Serialize(JsonValue.Create(text)),
        _ => value.ToJsonString(),
    };

    /// <summary>A value's own text, for the types whose spelling is the value itself.</summary>
    /// <param name="value">The value.</param>
    /// <returns>The text, unescaped and unquoted.</returns>
    private static string Text(JsonNode value) =>
        value is JsonValue scalar && scalar.TryGetValue<string>(out var text)
            ? text
            : value.ToJsonString();

    /// <summary>A value as JSON, with the spaces Python's serializer puts after each
    /// separator.</summary> <param name="value">The value.</param> <returns>The text.</returns>
    private static string Spaced(JsonNode? value) =>
        value?.ToJsonString() is { } json
            ? SpaceSeparators(json)
            : "null";

    /// <summary>Adds the space Python writes after a colon and a comma, outside strings.</summary>
    /// <param name="json">The compact JSON.</param>
    /// <returns>The JSON as Python writes it.</returns>
    private static string SpaceSeparators(string json)
    {
        var spaced = new StringBuilder(json.Length + 16);
        var quoted = false;
        var escaped = false;

        foreach (var character in json)
        {
            if (quoted)
            {
                spaced.Append(character);
                escaped = !escaped && character == '\\';

                if (!escaped && character == '"')
                {
                    quoted = false;
                }

                continue;
            }

            if (character == '"')
            {
                quoted = true;
                escaped = false;
            }
            else if (character is ':' or ',')
            {
                spaced.Append(character).Append(' ');

                continue;
            }

            spaced.Append(character);
        }

        return spaced.ToString();
    }

    private static JsonObject UploadPath() => new()
    {
        ["post"] = new JsonObject
        {
            ["summary"] = "Upload File",
            ["operationId"] = "upload_file_upload_post",
            ["parameters"] = new JsonArray
            {
                new JsonObject
                {
                    ["name"] = "upload_id",
                    ["in"] = "query",
                    ["required"] = false,
                    ["schema"] = new JsonObject
                    {
                        ["type"] = "string",
                        ["nullable"] = true,
                        ["default"] = null,
                    },
                    ["description"] = "Optional ID to track upload progress",
                },
            },
            ["requestBody"] = new JsonObject
            {
                ["required"] = true,
                ["content"] = new JsonObject
                {
                    ["multipart/form-data"] = new JsonObject
                    {
                        ["schema"] = new JsonObject
                        {
                            ["type"] = "object",
                            ["properties"] = new JsonObject
                            {
                                ["files"] = new JsonObject
                                {
                                    ["type"] = "array",
                                    ["items"] = new JsonObject
                                    {
                                        ["type"] = "string",
                                        ["format"] = "binary",
                                    },
                                    ["description"] = "One or more files to upload",
                                },
                            },
                            ["required"] = new JsonArray("files"),
                        },
                    },
                },
            },
            ["responses"] = new JsonObject
            {
                ["200"] = JsonResponse(
                    "List of file paths where the uploaded files were saved",
                    "application/json",
                    new JsonObject
                    {
                        ["type"] = "array",
                        ["items"] = new JsonObject { ["type"] = "string" },
                    }),
                ["400"] = JsonResponse(
                    "Invalid content type or invalid file name",
                    "text/plain",
                    new JsonObject { ["type"] = "string" }),
                ["413"] = JsonResponse(
                    "File exceeds maximum allowed size",
                    "text/plain",
                    new JsonObject { ["type"] = "string" }),
            },
            ["security"] = new JsonArray
            {
                new JsonObject { ["login_check"] = new JsonArray() },
            },
        },
    };

    /// <summary>Capitalizes the first letter, which is how a component's name is spelled there.</summary>
    private static string Capitalize(string value) =>
        value.Length == 0 ? value : char.ToUpperInvariant(value[0]) + value[1..];

    private static JsonObject JsonResponse(string description, string mediaType, JsonNode schema) => new()
    {
        ["description"] = description,
        ["content"] = new JsonObject
        {
            [mediaType] = new JsonObject { ["schema"] = schema },
        },
    };
}

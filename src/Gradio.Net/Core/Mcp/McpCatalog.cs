using System.Reflection;
using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// What one of an application's handlers is to an agent, as the attributes on it say.
/// </summary>
/// <remarks>
/// A record rather than a set of fields on <see cref="Dependency"/> because it is a property of the
/// handler rather than of the binding: the same method bound twice is the same kind of thing to an
/// agent both times, and a binding that is not an endpoint at all still has a kind.
/// </remarks>
internal sealed record McpDescriptor(
    McpKind Kind,
    string? Name,
    string? Description,
    string? UriTemplate,
    string? MimeType,
    bool StructuredOutput)
{
    /// <summary>The descriptor a handler carries.</summary>
    /// <param name="handler">The handler.</param>
    /// <returns>What the handler's attributes say, or a plain tool for one that says nothing.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="handler"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// The method behind the delegate rather than the delegate's type: an attribute is written on a
    /// method, and a method group handed to the framework keeps the one it was declared on. A
    /// lambda has none to keep, which is why the default is the answer for one.
    /// </remarks>
    internal static McpDescriptor Of(Delegate handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        var method = handler.Method;

        if (method.GetCustomAttribute<McpResourceAttribute>() is { } resource)
        {
            return new McpDescriptor(
                McpKind.Resource,
                resource.Name,
                resource.Description,
                resource.UriTemplate,
                resource.MimeType ?? "text/plain",
                StructuredOutput: false);
        }

        if (method.GetCustomAttribute<McpPromptAttribute>() is { } prompt)
        {
            return new McpDescriptor(McpKind.Prompt, prompt.Name, prompt.Description, null, null, StructuredOutput: false);
        }

        var tool = method.GetCustomAttribute<McpToolAttribute>();

        return new McpDescriptor(
            McpKind.Tool,
            tool?.Name,
            tool?.Description,
            null,
            null,
            tool?.StructuredOutput ?? false);
    }
}

/// <summary>
/// One tool an agent may call: an endpoint, and the schema of the arguments it takes
/// </summary>
/// <param name="Name">The name the tool is listed under, which is what a call names.</param>
/// <param name="Dependency">The binding the call runs.</param>
/// <param name="Description">The description the tool is listed with.</param>
/// <param name="InputSchema">The JSON schema of the arguments.</param>
/// <param name="ParameterNames">
/// The endpoint's parameters, in the order a call's arguments are read — the order the handler's
/// signature declares them.
/// </param>
/// <param name="StructuredOutput">Whether the answer is also reported as structured content.</param>
internal sealed record McpTool(
    string Name,
    Dependency Dependency,
    string Description,
    JsonObject InputSchema,
    IReadOnlyList<string> ParameterNames,
    bool StructuredOutput);

/// <summary>
/// One resource an agent may read, or the pattern of one
/// </summary>
/// <param name="Name">The name the resource is listed under.</param>
/// <param name="Dependency">The binding a read runs.</param>
/// <param name="UriTemplate">The address, or the address pattern.</param>
/// <param name="Description">The description the resource is listed with.</param>
/// <param name="MimeType">The media type of what a read returns.</param>
internal sealed record McpResource(
    string Name,
    Dependency Dependency,
    string UriTemplate,
    string Description,
    string MimeType);

/// <summary>
/// One entry of the schema document an agent reads without opening a connection
/// </summary>
/// <param name="Name">The name the entry is listed under.</param>
/// <param name="Kind">What kind of thing it is, which the document reports as <c>mcp_type</c>.</param>
/// <param name="Description">The description it is listed with.</param>
/// <param name="InputSchema">The JSON schema of the arguments it takes.</param>
/// <param name="EndpointName">The API name of the endpoint behind it.</param>
/// <remarks>
/// A list of its own rather than a walk over the three kinds, because the document lists them in
/// the order the application declared them and the three kinds are three lists: a client reading it
/// sees one application, not three sections.
/// </remarks>
internal sealed record McpSchemaEntry(
    string Name,
    McpKind Kind,
    string Description,
    JsonObject InputSchema,
    string EndpointName);

/// <summary>
/// One prompt an agent may fill in.
/// </summary>
/// <param name="Name">The name the prompt is listed under.</param>
/// <param name="Dependency">The binding a fill-in runs.</param>
/// <param name="Description">The description the prompt is listed with.</param>
/// <param name="Arguments">The arguments it accepts, in the order its handler declares them.</param>
internal sealed record McpPrompt(
    string Name,
    Dependency Dependency,
    string Description,
    IReadOnlyList<McpPromptArgument> Arguments);

/// <summary>One argument of a prompt.</summary>
/// <param name="Name">The name the argument is given by.</param>
/// <param name="Required">Whether a caller has to provide it.</param>
internal sealed record McpPromptArgument(string Name, bool Required);

/// <summary>
/// The application as an agent sees it: its tools, its resources and its prompts
/// </summary>
/// <remarks>
/// <para>
/// The Python version builds this once, at launch, from the API info it had then. Here it is built
/// per request from the same list the API info is built from, which is what makes the two agree
/// without either having to be told about the other: an endpoint a client can call is a tool an
/// agent can call, and one the application hid from clients is hidden from agents too.
/// </para>
/// <para>
/// The split between tools, resources and prompts is the handler's own declaration — an attribute,
/// or nothing at all for a tool — rather than something derived from the endpoint's name or shape.
/// </para>
/// </remarks>
internal sealed class McpCatalog
{
    private McpCatalog(
        IReadOnlyList<McpTool> tools,
        IReadOnlyList<McpResource> resources,
        IReadOnlyList<McpResource> resourceTemplates,
        IReadOnlyList<McpPrompt> prompts,
        IReadOnlyList<McpSchemaEntry> entries)
    {
        Tools = tools;
        Resources = resources;
        ResourceTemplates = resourceTemplates;
        Prompts = prompts;
        Entries = entries;
    }

    /// <summary>The tools an agent may call.</summary>
    internal IReadOnlyList<McpTool> Tools { get; }

    /// <summary>The resources whose address is fixed.</summary>
    internal IReadOnlyList<McpResource> Resources { get; }

    /// <summary>The resources whose address carries a parameter.</summary>
    internal IReadOnlyList<McpResource> ResourceTemplates { get; }

    /// <summary>The prompts an agent may fill in.</summary>
    internal IReadOnlyList<McpPrompt> Prompts { get; }

    /// <summary>
    /// Everything the application offers, in the order it declared it, for the schema document.
    /// </summary>
    internal IReadOnlyList<McpSchemaEntry> Entries { get; }

    /// <summary>Reads an application as an agent sees it.</summary>
    /// <param name="blocks">The application.</param>
    /// <returns>The catalog.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="blocks"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// The list is the API info's own, without <c>all_endpoints</c>: an <c>undocumented</c>
    /// endpoint is absent here for the reason it is absent there. The Python version builds its
    /// catalog from <c>get_api_info()</c> and nothing else, so a client that is not told about an
    /// endpoint is not offered it as a tool either — and a <c>private</c> one, which that list
    /// never holds, is not offered at all.
    /// </remarks>
    internal static McpCatalog Of(Blocks blocks)
    {
        ArgumentNullException.ThrowIfNull(blocks);

        var tools = new List<McpTool>();
        var resources = new List<McpResource>();
        var templates = new List<McpResource>();
        var prompts = new List<McpPrompt>();
        var entries = new List<McpSchemaEntry>();
        var used = new HashSet<string>(StringComparer.Ordinal);

        foreach (var (dependency, endpoint) in blocks.DocumentedEndpoints())
        {
            if (dependency.Handler is not { } handler)
            {
                continue;
            }

            var descriptor = McpDescriptor.Of(handler);
            var (schema, names) = InputSchemaOf(endpoint);
            var name = Unique(descriptor.Name ?? Blocks.NameOf(handler), used);

            // The description is the same one for all three kinds, because it is read the same way:
            // the Python version asks the endpoint what it does and adds the sentence a file input calls for.
            var description = Describe(
                descriptor.Description ?? JsonValues.Text(endpoint["description"]) ?? string.Empty,
                schema);

            switch (descriptor.Kind)
            {
                case McpKind.Resource:
                {
                    var templateParameters = ParametersOf(descriptor.UriTemplate!);
                    var resource = new McpResource(
                        name,
                        dependency,
                        descriptor.UriTemplate!,
                        description,
                        descriptor.MimeType ?? "text/plain");

                    (templateParameters.Count > 0 ? templates : resources).Add(resource);
                    break;
                }

                case McpKind.Prompt:
                    prompts.Add(new McpPrompt(name, dependency, description, ArgumentsOf(endpoint)));
                    break;

                default:
                    tools.Add(new McpTool(
                        name,
                        dependency,
                        description,
                        schema,
                        names,
                        descriptor.StructuredOutput));
                    break;
            }

            entries.Add(new McpSchemaEntry(name, descriptor.Kind, description, schema, dependency.ApiName));
        }

        return new McpCatalog(tools, resources, templates, prompts, entries);
    }

    /// <summary>The tool a name names, or <see langword="null"/> for one this application has not.</summary>
    /// <param name="name">The tool's name.</param>
    /// <returns>The tool.</returns>
    internal McpTool? Tool(string name) =>
        Tools.FirstOrDefault(tool => string.Equals(tool.Name, name, StringComparison.Ordinal));

    /// <summary>The prompt a name names, or <see langword="null"/> for one this application has not.</summary>
    /// <param name="name">The prompt's name.</param>
    /// <returns>The prompt.</returns>
    internal McpPrompt? Prompt(string name) =>
        Prompts.FirstOrDefault(prompt => string.Equals(prompt.Name, name, StringComparison.Ordinal));

    /// <summary>
    /// The schema document the Python version serves at the MCP endpoint's <c>/schema</c> address.
    /// </summary>
    /// <param name="selected">The entries the request asked for, or <see langword="null"/> for all.</param>
    /// <returns>One entry per thing the application offers, tools, resources and prompts alike.</returns>
    /// <remarks>
    /// <para>
    /// The document exists for a client that wants to know what an application offers without
    /// opening an MCP connection — the Hugging Face MCP server reads it that way. Its field names
    /// are the Python version's, <c>meta</c> included, and so is its habit of describing a resource
    /// and a prompt exactly as it describes a tool: a reader that wants to know what arguments
    /// something takes asks the same question of all three.
    /// </para>
    /// <para>
    /// <c>file_data_present</c> latches. The Python version sets it once, when it meets the first
    /// entry that takes a file, and never clears it — so every entry after that one reports
    /// <see langword="true"/> whether or not it takes a file. That reads as a property of the
    /// application rather than of the entry, which is what the field is for (it decides whether a
    /// page offers an upload tool), so it is reproduced rather than corrected: an agent reading the
    /// entry's own schema is told the truth by the schema.
    /// </para>
    /// </remarks>
    internal JsonArray Schema(IReadOnlyList<string>? selected = null)
    {
        var schemas = new JsonArray();
        var fileDataPresent = false;

        foreach (var entry in Entries)
        {
            if (selected is not null && !selected.Contains(entry.Name, StringComparer.Ordinal))
            {
                continue;
            }

            fileDataPresent |= ContainsFile(entry.InputSchema);

            schemas.Add(new JsonObject
            {
                ["name"] = entry.Name,
                ["description"] = entry.Description,
                ["inputSchema"] = entry.InputSchema.DeepClone(),
                ["meta"] = new JsonObject
                {
                    ["file_data_present"] = fileDataPresent,
                    ["mcp_type"] = KindOf(entry.Kind),
                    ["endpoint_name"] = entry.EndpointName,
                },
            });
        }

        return schemas;
    }

    /// <summary>What a kind of thing is called in the schema document.</summary>
    /// <param name="kind">The kind.</param>
    /// <returns>The name the Python version spells it with.</returns>
    private static string KindOf(McpKind kind) => kind switch
    {
        McpKind.Resource => "resource",
        McpKind.Prompt => "prompt",
        _ => "tool",
    };

    /// <summary>The description a tool is listed with.</summary>
    /// <param name="description">The endpoint's description.</param>
    /// <param name="schema">The tool's input schema.</param>
    /// <returns>The description, with the sentence a file input calls for.</returns>
    /// <remarks>
    /// A tool that takes a file says so, in the Python version's words: an agent cannot upload
    /// anything, and the schema alone — a string with an unusual format — does not tell it what to
    /// do about that. The sentence names the upload tool "if present", which is the Python
    /// version's hedge and this implementation's answer both: the tool it names is the one the
    /// excluded CLI provides.
    /// </remarks>
    private static string Describe(string description, JsonObject schema) =>
        ContainsFile(schema) ? description + FileInputSentence : description;

    /// <summary>The sentence a tool with a file input is described with.</summary>
    private const string FileInputSentence =
        " If a user passes a file as an input, use the upload_file_to_gradio tool, if present, to"
        + " upload the file to the gradio app and create a Gradio File Input. Then use the returned"
        + " path as the input to the tool";

    /// <summary>The arguments an endpoint takes, as a tool's input schema describes them.</summary>
    /// <param name="endpoint">The endpoint, as the API info describes it.</param>
    /// <returns>The schema, and the parameters' names in the order a call reads them.</returns>
    /// <remarks>
    /// The schema is the API info's own, reshaped the way the Python version reshapes it for MCP:
    /// the parameters become the object's properties, the ones without a default become its
    /// required list, and a file becomes the address a caller passes rather than the object the
    /// server keeps.
    /// </remarks>
    private static (JsonObject Schema, IReadOnlyList<string> Names) InputSchemaOf(JsonObject endpoint)
    {
        var properties = new JsonObject();
        var required = new JsonArray();
        var names = new List<string>();

        foreach (var parameter in endpoint["parameters"]!.AsArray())
        {
            var name = parameter!["parameter_name"]!.GetValue<string>();
            var property = parameter["type"]?.DeepClone() as JsonObject ?? [];

            // The Python version describes every parameter, including the ones it has nothing to say about:
            // the description it writes comes from the handler's docstring, and a parameter the
            // docstring does not mention — which, for a .NET delegate, is all of them — gets an
            // empty one. Writing the key rather than leaving it out keeps the two catalogs the same
            // document instead of one that differs by a field a reader would read as the same.
            property["description"] = string.Empty;

            if (parameter["parameter_has_default"]?.GetValue<bool>() == true
                && parameter["parameter_default"] is { } fallback)
            {
                property["default"] = fallback.DeepClone();
            }

            properties[name] = property;
            names.Add(name);

            if (parameter["parameter_has_default"]?.GetValue<bool>() != true)
            {
                required.Add(name);
            }
        }

        var schema = new JsonObject
        {
            ["type"] = "object",
            ["properties"] = properties,
        };

        if (required.Count > 0)
        {
            schema["required"] = required;
        }

        return ((JsonObject)FileAddresses(schema)!, names);
    }

    /// <summary>Rewrites every file in a schema as the address a caller sends.</summary>
    /// <param name="schema">The schema.</param>
    /// <returns>The schema.</returns>
    /// <remarks>
    /// <para>
    /// The Python version's <c>simplify_filedata_schema</c>. An agent has no way to upload a file
    /// to the application, so what its tool's schema asks for is an address the server can fetch —
    /// and the value it sends is read back as a file by the block that receives it.
    /// </para>
    /// <para>
    /// The description and the file's own <c>required</c> list are carried over because the Python
    /// version carries them: its rewrite drops a file's properties and keeps everything else, so a
    /// string schema ends up with a list of fields it does not have. That list says nothing a
    /// client can act on — a <c>required</c> outside an object is ignored — and leaving it out
    /// would make this catalog differ from the Python version's by a field, which is the one thing
    /// the two must not do.
    /// </para>
    /// </remarks>
    private static JsonNode? FileAddresses(JsonNode? schema)
    {
        if (schema is JsonArray array)
        {
            return new JsonArray([.. array.Select(FileAddresses)]);
        }

        if (schema is not JsonObject document)
        {
            return schema?.DeepClone();
        }

        if (JsonSchemaTypes.IsFileSchema(document))
        {
            var address = new JsonObject
            {
                ["type"] = "string",
                ["format"] = TypeSchema.FileInputFormat,
            };

            if (document["required"] is { } required)
            {
                address["required"] = required.DeepClone();
            }

            address["description"] = document["description"]?.DeepClone() ?? JsonValue.Create(string.Empty);

            return address;
        }

        var rewritten = new JsonObject();
        foreach (var (key, value) in document)
        {
            rewritten[key] = FileAddresses(value);
        }

        return rewritten;
    }

    /// <summary>Whether a schema still describes a file anywhere in it.</summary>
    /// <param name="schema">The schema.</param>
    /// <returns>Whether it does.</returns>
    /// <remarks>
    /// Asked of the rewritten schema rather than of the one it came from, because that is the
    /// question the Python version answers: whether a client reading this tool has to send an
    /// address.
    /// </remarks>
    private static bool ContainsFile(JsonNode? schema) => schema switch
    {
        JsonArray array => array.Any(ContainsFile),
        JsonObject document => JsonValues.Text(document["format"]) == TypeSchema.FileInputFormat
            || document.Any(pair => ContainsFile(pair.Value)),
        _ => false,
    };

    /// <summary>The arguments a prompt's handler takes.</summary>
    /// <param name="endpoint">The endpoint, as the API info describes it.</param>
    /// <returns>The arguments, in the order the handler declares them.</returns>
    private static List<McpPromptArgument> ArgumentsOf(JsonObject endpoint)
    {
        var arguments = new List<McpPromptArgument>();

        foreach (var parameter in endpoint["parameters"]!.AsArray())
        {
            arguments.Add(new McpPromptArgument(
                parameter!["parameter_name"]!.GetValue<string>(),
                Required: parameter["parameter_has_default"]?.GetValue<bool>() != true));
        }

        return arguments;
    }

    /// <summary>The names a resource's address template takes.</summary>
    /// <param name="template">The address pattern.</param>
    /// <returns>The names, in the order they appear.</returns>
    private static List<string> ParametersOf(string template)
    {
        var parameters = new List<string>();
        var start = 0;

        while (template.IndexOf('{', start) is var open and >= 0)
        {
            var close = template.IndexOf('}', open + 1);
            if (close < 0)
            {
                return parameters;
            }

            parameters.Add(template[(open + 1)..close]);
            start = close + 1;
        }

        return parameters;
    }

    /// <summary>The names a resource's address template takes, and what they match.</summary>
    /// <param name="template">The address pattern.</param>
    /// <param name="uri">The address a caller asked for.</param>
    /// <returns>The values, by name, or <see langword="null"/> when the address is not this template's.</returns>
    /// <remarks>
    /// A segment matches anything but a slash, which is how the Python version matches one: a
    /// resource address names a thing, and a thing's name does not span segments.
    /// </remarks>
    internal static Dictionary<string, string>? Match(string template, string uri)
    {
        var parameters = ParametersOf(template);
        var pattern = new System.Text.StringBuilder("^");

        var start = 0;
        foreach (var name in parameters)
        {
            var open = template.IndexOf('{' + name + '}', start, StringComparison.Ordinal);
            pattern.Append(System.Text.RegularExpressions.Regex.Escape(template[start..open]));
            pattern.Append("(?<").Append(name).Append(">[^/]+)");
            start = open + name.Length + 2;
        }

        pattern.Append(System.Text.RegularExpressions.Regex.Escape(template[start..])).Append('$');

        var match = System.Text.RegularExpressions.Regex.Match(uri, pattern.ToString());
        if (!match.Success)
        {
            return null;
        }

        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var name in parameters)
        {
            values[name] = match.Groups[name].Value;
        }

        return values;
    }

    /// <summary>
    /// A name made unique among the ones already in the list, and legal as a tool name.
    /// </summary>
    /// <param name="name">The name asked for.</param>
    /// <param name="used">The names already taken, which this adds to.</param>
    /// <returns>The name to use.</returns>
    /// <remarks>
    /// The Python version's <c>valid_and_unique_tool_name</c>: a name may only hold letters, digits
    /// and underscores, and two endpoints backed by the same method — a button that binds one
    /// handler twice, say — would otherwise be one tool with two meanings.
    /// </remarks>
    private static string Unique(string name, HashSet<string> used)
    {
        var sanitized = new string([.. name.Select(character =>
            char.IsAsciiLetterOrDigit(character) ? character : '_')]);

        if (sanitized.Length == 0)
        {
            sanitized = "tool";
        }

        sanitized = sanitized[..Math.Min(sanitized.Length, 120)];

        var candidate = sanitized;
        for (var suffix = 1; !used.Add(candidate); suffix++)
        {
            candidate = $"{sanitized}_{suffix}";
        }

        return candidate;
    }
}

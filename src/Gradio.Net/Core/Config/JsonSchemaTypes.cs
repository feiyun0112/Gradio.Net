using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// What the two API documents make of a component's JSON schema: the Python type a client shows,
/// and the schema with its references resolved.
/// </summary>
/// <remarks>
/// <para>
/// The Python version serves a component's typing twice over. The API info shows it as a Python
/// type hint — <c>list[dict(role: str, ...)]</c> for a conversation — which is what the
/// documentation page and the clients display, while the OpenAPI document shows it as a schema a
/// client can generate a call from. Both are derived from the same JSON schema, by two functions
/// that live in the client libraries the Python version ships; they are written out here because
/// those libraries are Python.
/// </para>
/// <para>
/// The two are not the same simplification, which is why they are two methods rather than one. The
/// type hint walks the schema and renders it as a string, following references but keeping the
/// nested shape. The OpenAPI schema inlines every reference and replaces a file's schema with the
/// sentence that tells a caller how to upload one — a document a client generates code from wants
/// the file described, not the object the server happens to keep.
/// </para>
/// </remarks>
internal static class JsonSchemaTypes
{
    /// <summary>The schema type the Python version writes for a value that is any JSON at
    /// all.</summary>
    private const string AnyJson = "str | float | bool | list | dict";

    /// <summary>What a client is told about a file it is sending.</summary>
    private const string FileInputDescription =
        "Must include {\"path\": \"url\", \"meta\": {\"_type\": \"gradio.FileData\"}}. "
        + "The meta key signals that the file will be uploaded to the remote server.";

    /// <summary>What a client is told about a file it is receiving.</summary>
    private const string FileOutputDescription = "JSON object with url key to fetch the output";

    /// <summary>The media type a file object names itself with.</summary>
    private const string FileMetaType = "gradio.FileData";

    /// <summary>A schema as the Python type hint the Python version's clients show.</summary>
    /// <param name="schema">The schema.</param>
    /// <returns>The type hint, spelled the way Python spells one.</returns>
    /// <remarks>
    /// The Python version's <c>json_schema_to_python_type</c>, including what it does before
    /// rendering an object at all: a schema that describes a file is answered with <c>filepath</c>,
    /// because that is what a caller actually passes. The test comes first rather than as a rewrite
    /// of the rendered dictionary, because the rendering depends on the order a schema happens to
    /// list its nullable fields in — and a rendering that drifted would silently stop being
    /// recognised.
    /// </remarks>
    internal static string PythonType(JsonNode? schema)
    {
        var document = schema as JsonObject;

        return PythonTypeOf(document, document?["$defs"] as JsonObject);
    }

    /// <summary>
    /// A schema with its references inlined and its files replaced by how a caller sends one.
    /// </summary>
    /// <param name="schema">The schema.</param>
    /// <param name="isInput">Whether the schema describes a value a caller sends rather than receives.</param>
    /// <returns>The simplified schema.</returns>
    /// <remarks>
    /// The Python version's <c>simplify_json_schema</c>: resolve every <c>$ref</c> from the root's
    /// <c>$defs</c>, drop the <c>$defs</c> themselves, and then replace every file schema with the
    /// sentence that tells a caller what to upload. A client that generated a call from the
    /// unreduced document would generate a call that takes a file object rather than a file.
    /// </remarks>
    internal static JsonNode? Simplify(JsonNode? schema, bool isInput)
    {
        var resolved = Resolve(schema, (schema as JsonObject)?["$defs"] as JsonObject);

        return Walk(resolved, _ => FileOf(isInput));
    }

    /// <summary>A schema with every reference replaced by what it names.</summary>
    /// <param name="schema">The schema.</param>
    /// <param name="defs">The definitions its references are resolved from.</param>
    /// <returns>The schema.</returns>
    private static JsonNode? Resolve(JsonNode? schema, JsonObject? defs)
    {
        if (schema is JsonArray array)
        {
            return new JsonArray([.. array.Select(item => Resolve(item, defs))]);
        }

        if (schema is not JsonObject document)
        {
            return schema?.DeepClone();
        }

        if (document["$ref"] is JsonValue reference
            && reference.TryGetValue<string>(out var path)
            && defs?[NameOf(path)] is { } named)
        {
            return Resolve(named, defs);
        }

        var resolved = new JsonObject();
        foreach (var (key, value) in document)
        {
            if (key == "$defs")
            {
                continue;
            }

            resolved[key] = Resolve(value, defs);
        }

        return resolved;
    }

    /// <summary>The name a reference points at, which is the last segment of its path.</summary>
    /// <param name="path">The reference.</param>
    /// <returns>The name.</returns>
    private static string NameOf(string path) => path[(path.LastIndexOf('/') + 1)..];

    /// <summary>Replaces every node a test accepts with what the replacement makes of it.</summary>
    /// <param name="schema">The schema.</param>
    /// <param name="replace">What to do with a node the test accepts.</param>
    /// <returns>The schema.</returns>
    private static JsonNode? Walk(JsonNode? schema, Func<JsonObject, JsonNode?> replace)
    {
        if (schema is JsonArray array)
        {
            return new JsonArray([.. array.Select(item => Walk(item, replace))]);
        }

        if (schema is not JsonObject document)
        {
            return schema?.DeepClone();
        }

        if (IsFileSchema(document))
        {
            return replace(document);
        }

        var walked = new JsonObject();
        foreach (var (key, value) in document)
        {
            walked[key] = Walk(value, replace);
        }

        return walked;
    }

    /// <summary>Whether a schema describes a file.</summary>
    /// <param name="schema">The schema.</param>
    /// <param name="defs">
    /// The definitions its references are resolved from, or <see langword="null"/> for a schema that
    /// has already had them inlined.
    /// </param>
    /// <returns>Whether it is the object a file is sent as.</returns>
    /// <remarks>
    /// The Python version's <c>is_gradio_filedata</c>, which resolves a reference before asking: a
    /// component's schema names its <c>FileData</c> once and refers to it by <c>$ref</c> everywhere
    /// else, so a test that only looked at the object in front of it would answer no for every file
    /// a conversation carries.
    /// </remarks>
    internal static bool IsFileSchema(JsonObject schema, JsonObject? defs = null)
    {
        if (Dereference(schema, defs) is not { } document
            || document["properties"] is not JsonObject properties
            || !properties.ContainsKey("path")
            || !properties.ContainsKey("meta"))
        {
            return false;
        }

        return Dereference(properties["meta"], defs) is JsonObject meta
            && (meta["properties"]?["_type"]?["const"]?.GetValue<string>() == FileMetaType
                || meta["default"]?["_type"]?.GetValue<string>() == FileMetaType);
    }

    /// <summary>The schema a reference names, or the schema itself when it is not a reference.</summary>
    /// <param name="schema">The schema.</param>
    /// <param name="defs">The definitions its references are resolved from.</param>
    /// <returns>The schema to read, or <see langword="null"/> for one that resolves to nothing.</returns>
    private static JsonObject? Dereference(JsonNode? schema, JsonObject? defs) =>
        schema is JsonObject document
        && document["$ref"] is JsonValue reference
        && reference.TryGetValue<string>(out var path)
        && defs?[NameOf(path)] is JsonObject named
            ? named
            : schema as JsonObject;

    /// <summary>What the OpenAPI document says about a file.</summary>
    /// <param name="isInput">Whether the file is one a caller sends.</param>
    /// <returns>The schema.</returns>
    private static JsonObject FileOf(bool isInput) => isInput
        ? new JsonObject { ["type"] = "filepath", ["description"] = FileInputDescription }
        : new JsonObject { ["type"] = "object", ["description"] = FileOutputDescription };

    /// <summary>A schema rendered as a Python type hint.</summary>
    /// <param name="schema">The schema.</param>
    /// <param name="defs">The definitions its references are resolved from.</param>
    /// <returns>The type hint.</returns>
    private static string PythonTypeOf(JsonNode? schema, JsonObject? defs)
    {
        if (schema is not JsonObject document || document.Count == 0)
        {
            return "Any";
        }

        if (document["const"] is { } constant)
        {
            return $"Literal[{Scalar(constant)}]";
        }

        if (document["enum"] is JsonArray choices)
        {
            return "Literal[" + string.Join(", ", choices.Select(choice => $"'{Scalar(choice)}'")) + "]";
        }

        if (document["$ref"] is JsonValue reference
            && reference.TryGetValue<string>(out var path)
            && defs?[NameOf(path)] is { } named)
        {
            return PythonTypeOf(named, defs);
        }

        if (document["oneOf"] is JsonArray oneOf)
        {
            return string.Join(" | ", oneOf.Select(option => PythonTypeOf(option, defs)));
        }

        if (document["anyOf"] is JsonArray anyOf)
        {
            return string.Join(" | ", anyOf.Select(option => PythonTypeOf(option, defs)));
        }

        if (document["allOf"] is JsonArray allOf)
        {
            return "All[" + string.Join(", ", allOf.Select(option => PythonTypeOf(option, defs))) + "]";
        }

        return document["type"] is JsonValue type && type.TryGetValue<string>(out var name)
            ? Named(name, document, defs)
            : AnyJsonOrAny(document);
    }

    /// <summary>A schema that names its type.</summary>
    /// <param name="name">The type.</param>
    /// <param name="schema">The schema.</param>
    /// <param name="defs">The definitions its references are resolved from.</param>
    /// <returns>The type hint.</returns>
    private static string Named(string name, JsonObject schema, JsonObject? defs) => name switch
    {
        "null" => "None",
        "integer" => "int",
        "string" => "str",
        "boolean" => "bool",
        "number" => "float",
        "array" => ArrayOf(schema, defs),
        "object" => ObjectOf(schema, defs),
        _ => AnyJsonOrAny(schema),
    };

    /// <summary>An array's type hint: the element's, in a list or a tuple.</summary>
    /// <param name="schema">The schema.</param>
    /// <param name="defs">The definitions its references are resolved from.</param>
    /// <returns>The type hint.</returns>
    private static string ArrayOf(JsonObject schema, JsonObject? defs)
    {
        if (schema["items"] is JsonObject items && items["prefixItems"] is JsonArray fixedItems)
        {
            return "tuple[" + string.Join(", ", fixedItems.Select(item => PythonTypeOf(item, defs))) + "]";
        }

        if (schema["prefixItems"] is JsonArray prefixItems)
        {
            return "tuple[" + string.Join(", ", prefixItems.Select(item => PythonTypeOf(item, defs))) + "]";
        }

        return $"list[{PythonTypeOf(schema["items"], defs)}]";
    }

    /// <summary>An object's type hint: its fields, by name and by type.</summary>
    /// <param name="schema">The schema.</param>
    /// <param name="defs">The definitions its references are resolved from.</param>
    /// <returns>The type hint.</returns>
    private static string ObjectOf(JsonObject schema, JsonObject? defs)
    {
        if (IsFileSchema(schema, defs))
        {
            return "filepath";
        }

        var described = new List<string>();

        if (schema["properties"] is JsonObject properties)
        {
            foreach (var (name, property) in properties)
            {
                if (name == "$defs")
                {
                    continue;
                }

                var description = JsonValues.Text(property?["description"]);
                described.Add(
                    $"{name}: {PythonTypeOf(property, defs)}"
                    + (string.IsNullOrEmpty(description) ? string.Empty : $" ({description})"));
            }
        }

        if (schema["additionalProperties"] is { } additional)
        {
            described.Add(
                additional is JsonValue flag && flag.TryGetValue<bool>(out var allowed) && allowed
                    ? "str, Any"
                    : $"str, {PythonTypeOf(additional, defs)}");
        }

        return "dict(" + string.Join(", ", described) + ")";
    }

    /// <summary>
    /// A schema that names no type: any JSON when its description says it carries JSON, and
    /// <c>Any</c> otherwise.
    /// </summary>
    /// <param name="schema">The schema.</param>
    /// <returns>The type hint.</returns>
    private static string AnyJsonOrAny(JsonObject schema) =>
        JsonValues.Text(schema["description"])?.Contains("json", StringComparison.Ordinal) == true
            ? AnyJson
            : "Any";

    /// <summary>A value as Python would write it inside a type hint.</summary>
    /// <param name="value">The value.</param>
    /// <returns>The text.</returns>
    private static string Scalar(JsonNode? value) => value switch
    {
        null => "None",
        JsonValue json when json.TryGetValue<bool>(out var flag) => flag ? "True" : "False",
        _ => value.ToString(),
    };
}

using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Gradio.Net;
/// <summary>
/// The JSON schema a CLR type stands for, and the types a handler's signature declares
/// </summary>
/// <remarks>
/// <para>
/// The Python version derives an API endpoint's typing from the handler's type hints rather than
/// from components (<c>gr.api</c>): one <c>Api</c> block per parameter and per return value, each
/// carrying the schema its hint produced. A .NET delegate has types where Python has hints, and
/// this is the mapping between them — the counterpart of the client library's
/// <c>python_type_to_json_schema</c>, case for case.
/// </para>
/// <para>
/// The mapping is deliberately mechanical rather than clever: what a type <em>is</em> decides the
/// schema, so that an application's schema cannot disagree with the values its handler receives.
/// <see cref="TypedValues"/> reads and writes the same shapes with the same naming policy, which is
/// what keeps the two ends of one endpoint in agreement.
/// </para>
/// </remarks>
internal static class TypeSchema
{
    /// <summary>The format the Python version gives a file a caller sends.</summary>
    internal const string FileInputFormat = "Gradio File Input - a http or https url to a file";

    /// <summary>
    /// How a CLR value is spelled as JSON when an endpoint's schema — rather than a component's
    /// shape — decides the mapping.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Snake case because that is what the rest of the protocol speaks: a component's props are
    /// snake case, a binding's fields are snake case, and an endpoint derived from a record whose
    /// properties are <c>FirstName</c> would otherwise describe one document and accept another.
    /// </para>
    /// <para>
    /// Read-only for that reason: the same options serve both directions, and a caller that changed
    /// them would make a schema and the values it describes disagree.
    /// </para>
    /// </remarks>
    internal static readonly JsonSerializerOptions Json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
    };

    /// <summary>The schema a type stands for.</summary>
    /// <param name="type">The CLR type.</param>
    /// <returns>The schema document.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="type"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// A type this mapping does not know is described as any JSON — an empty schema — rather than
    /// as an error: the endpoint is still callable, and claiming a shape the framework did not
    /// derive would be worse than claiming none.
    /// </remarks>
    internal static JsonObject Of(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (Nullable.GetUnderlyingType(type) is { } optional)
        {
            return new JsonObject
            {
                ["oneOf"] = new JsonArray(
                    new JsonObject { ["type"] = "null" },
                    Of(optional)),
            };
        }

        if (type == typeof(void) || type == typeof(object) || type == typeof(JsonNode)
            || type == typeof(JsonElement) || type == typeof(JsonDocument))
        {
            return [];
        }

        if (type == typeof(string) || type == typeof(char))
        {
            return Typed("string");
        }

        if (type == typeof(bool))
        {
            return Typed("boolean");
        }

        if (IsInteger(type))
        {
            return Typed("integer");
        }

        if (IsNumber(type))
        {
            return Typed("number");
        }

        if (type == typeof(byte[]))
        {
            return new JsonObject { ["type"] = "string", ["format"] = "byte" };
        }

        if (WellKnownFormat(type) is { } format)
        {
            return new JsonObject { ["type"] = "string", ["format"] = format };
        }

        if (type.IsEnum)
        {
            return new JsonObject
            {
                ["enum"] = new JsonArray([.. Enum.GetNames(type).Select(name => (JsonNode?)JsonValue.Create(name))]),
            };
        }

        if (type == typeof(FileData))
        {
            return FileSchema();
        }

        if (TupleElements(type) is { } elements)
        {
            return new JsonObject
            {
                ["type"] = "array",
                ["prefixItems"] = new JsonArray([.. elements.Select(Of)]),
                ["minItems"] = elements.Length,
                ["maxItems"] = elements.Length,
            };
        }

        if (DictionaryValue(type) is { } value)
        {
            return new JsonObject
            {
                ["type"] = "object",
                ["additionalProperties"] = Of(value),
            };
        }

        if (ElementType(type) is { } element)
        {
            return new JsonObject
            {
                ["type"] = "array",
                ["items"] = Of(element),
            };
        }

        // A sequence whose elements the type does not name — a plain list, a set of anything.
        if (typeof(IEnumerable).IsAssignableFrom(type))
        {
            return new JsonObject { ["type"] = "array", ["items"] = new JsonObject() };
        }

        return PropertiesOf(type);
    }

    /// <summary>
    /// The types a handler's result carries, one per output.
    /// </summary>
    /// <param name="handler">The handler.</param>
    /// <returns>The declared return types, in order.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="handler"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// <para>
    /// Three shapes, matching the three output shapes. A task is awaited before its
    /// result is read; a tuple is spread across the outputs; an
    /// <see cref="IAsyncEnumerable{T}"/> is one output, because what it streams is one value at a
    /// time rather than one value per output.
    /// </para>
    /// <para>
    /// A handler that answers with nothing — a bare <see cref="Task"/>, or <c>void</c> — has one
    /// output describing any JSON, which is what the Python version does with a function that
    /// returns <c>None</c>: the endpoint still has a result, it just does not say what shape it is.
    /// </para>
    /// </remarks>
    internal static IReadOnlyList<Type> ReturnsOf(Delegate handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        var type = handler.Method.ReturnType;

        if (type.IsGenericType)
        {
            var definition = type.GetGenericTypeDefinition();

            if (definition == typeof(Task<>) || definition == typeof(ValueTask<>))
            {
                type = type.GetGenericArguments()[0];
            }
        }

        if (type == typeof(Task) || type == typeof(ValueTask) || type == typeof(void))
        {
            return [typeof(object)];
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>))
        {
            return [type.GetGenericArguments()[0]];
        }

        return TupleElements(type) is { } elements ? elements : [type];
    }

    /// <summary>The schema a file is sent as, spelled the way the Python version's clients render
    /// it.</summary> <returns>The schema.</returns>
    /// <remarks>
    /// <para>
    /// The field list, the nulls and the <c>meta</c> marker are <see cref="FileData"/>'s own, which
    /// is why this is a hand-written document rather than a reflection over the type: the marker is
    /// what tells a client — and <see cref="JsonSchemaTypes.Simplify"/> — that this object is a
    /// file rather than a dictionary that happens to have a <c>path</c>.
    /// </para>
    /// <para>
    /// Every field is required, the nullable ones included, because that is what the Python
    /// version's generator writes: the client library's reading of a file's shape names all seven,
    /// and a schema that called four of them optional would be describing a different object.
    /// </para>
    /// </remarks>
    private static JsonObject FileSchema() => new()
    {
        ["type"] = "object",
        ["properties"] = new JsonObject
        {
            ["path"] = Typed("string"),
            ["url"] = MayBeAbsent(Typed("string")),
            ["size"] = MayBeAbsent(Typed("integer")),
            ["orig_name"] = MayBeAbsent(Typed("string")),
            ["mime_type"] = MayBeAbsent(Typed("string")),
            ["is_stream"] = Typed("boolean"),
            ["meta"] = new JsonObject
            {
                ["type"] = "object",
                ["properties"] = new JsonObject
                {
                    ["_type"] = new JsonObject { ["const"] = FileData.FileMetaType },
                },
                ["required"] = new JsonArray("_type"),
            },
        },
        ["required"] = new JsonArray("path", "url", "size", "orig_name", "mime_type", "is_stream", "meta"),
    };

    /// <summary>The schema a class or struct stands for: its properties, by name.</summary>
    /// <param name="type">The type.</param>
    /// <returns>The schema, or an empty one for a type that declares no property.</returns>
    private static JsonObject PropertiesOf(Type type)
    {
        var properties = new JsonObject();
        var required = new JsonArray();
        var nullable = new NullabilityInfoContext();

        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!property.CanRead || property.GetIndexParameters().Length > 0)
            {
                continue;
            }

            properties[JsonNameOf(property)] = Of(property.PropertyType);

            if (IsRequired(property, nullable))
            {
                required.Add(JsonNameOf(property));
            }
        }

        if (properties.Count == 0)
        {
            return [];
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

        return schema;
    }

    /// <summary>Whether a property has to be present for the value to be whole.</summary>
    /// <param name="property">The property.</param>
    /// <param name="nullable">The reader for a reference type's nullability.</param>
    /// <returns>Whether it is required.</returns>
    /// <remarks>
    /// A member the language marks <c>required</c>, and a member whose type cannot hold null — a
    /// value type that is not <see cref="Nullable{T}"/>, or a reference type the compiler was told
    /// is never null. Everything else can be left out, which is what a caller omitting an optional
    /// field relies on.
    /// </remarks>
    private static bool IsRequired(PropertyInfo property, NullabilityInfoContext nullable)
    {
        if (property.GetCustomAttribute<RequiredMemberAttribute>() is not null)
        {
            return true;
        }

        var type = property.PropertyType;

        if (Nullable.GetUnderlyingType(type) is not null)
        {
            return false;
        }

        if (type.IsValueType)
        {
            return true;
        }

        return nullable.Create(property).ReadState == NullabilityState.NotNull;
    }

    /// <summary>The name a property is spelled with in JSON.</summary>
    /// <param name="property">The property.</param>
    /// <returns>The name.</returns>
    private static string JsonNameOf(PropertyInfo property) =>
        property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name
        ?? Json.PropertyNamingPolicy!.ConvertName(property.Name);

    /// <summary>The element types of a tuple, or <see langword="null"/> for anything else.</summary>
    /// <param name="type">The type.</param>
    /// <returns>The element types, in order.</returns>
    /// <remarks>
    /// A <see cref="ValueTuple{T1, T2}"/> of any arity, and the reference
    /// <see cref="Tuple{T1, T2}"/>, which a handler may also return. The arity is read from the
    /// generic arguments rather than from the name, because the compiler spells a tuple of more than
    /// seven elements as a nested rest element.
    /// </remarks>
    private static Type[]? TupleElements(Type type)
    {
        if (!type.IsGenericType)
        {
            return null;
        }

        var definition = type.GetGenericTypeDefinition();
        var name = definition.FullName ?? string.Empty;

        if (!name.StartsWith("System.ValueTuple`", StringComparison.Ordinal)
            && !name.StartsWith("System.Tuple`", StringComparison.Ordinal))
        {
            return null;
        }

        return type.GetGenericArguments();
    }

    /// <summary>The value type of a string-keyed dictionary, or <see langword="null"/>.</summary>
    /// <param name="type">The type.</param>
    /// <returns>The value type.</returns>
    private static Type? DictionaryValue(Type type)
    {
        foreach (var candidate in Interfaces(type))
        {
            if (!candidate.IsGenericType)
            {
                continue;
            }

            var definition = candidate.GetGenericTypeDefinition();
            if (definition != typeof(IDictionary<,>) && definition != typeof(IReadOnlyDictionary<,>))
            {
                continue;
            }

            var arguments = candidate.GetGenericArguments();

            return arguments[0] == typeof(string) ? arguments[1] : null;
        }

        return null;
    }

    /// <summary>The element type of a sequence, or <see langword="null"/>.</summary>
    /// <param name="type">The type.</param>
    /// <returns>The element type.</returns>
    /// <remarks>
    /// A set is a sequence too, and the Python version describes one with <c>uniqueItems</c>; the
    /// flag is not written here because a .NET set's JSON is an array a caller may repeat a value
    /// in, and a schema that refused that would reject a value the handler can take.
    /// </remarks>
    internal static Type? ElementType(Type type)
    {
        if (type.IsArray)
        {
            return type.GetElementType();
        }

        foreach (var candidate in Interfaces(type))
        {
            if (candidate.IsGenericType && candidate.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return candidate.GetGenericArguments()[0];
            }
        }

        return null;
    }

    /// <summary>A type and every interface it implements.</summary>
    /// <param name="type">The type.</param>
    /// <returns>The interfaces, the type itself included.</returns>
    private static Type[] Interfaces(Type type) =>
        type.IsInterface ? [type, .. type.GetInterfaces()] : type.GetInterfaces();

    /// <summary>Whether a type is an integer.</summary>
    /// <param name="type">The type.</param>
    /// <returns>Whether it is.</returns>
    private static bool IsInteger(Type type) =>
        type == typeof(int) || type == typeof(long) || type == typeof(short) || type == typeof(byte)
        || type == typeof(sbyte) || type == typeof(uint) || type == typeof(ulong) || type == typeof(ushort);

    /// <summary>Whether a type is a number with a fractional part.</summary>
    /// <param name="type">The type.</param>
    /// <returns>Whether it is.</returns>
    private static bool IsNumber(Type type) =>
        type == typeof(double) || type == typeof(float) || type == typeof(decimal);

    /// <summary>The format a type is known by, or <see langword="null"/> for one that is not.</summary>
    /// <param name="type">The type.</param>
    /// <returns>The format.</returns>
    /// <remarks>
    /// The types that carry a shape a plain string does not say: two kinds of moment, a day, a time
    /// of day, a span, an identifier and an address. Describing them as strings keeps the schema
    /// honest — that is what they travel as — while the format tells a client what to put in the
    /// string. The Python version has no schema for any of them and answers with "any JSON", which
    /// an agent reads as "send whatever"; this is the one place the derivation says more than the
    /// Python one, and it says it in the vocabulary JSON Schema already has.
    /// </remarks>
    private static string? WellKnownFormat(Type type) => type switch
    {
        _ when type == typeof(DateTime) || type == typeof(DateTimeOffset) => "date-time",
        _ when type == typeof(DateOnly) => "date",
        _ when type == typeof(TimeOnly) => "time",
        _ when type == typeof(TimeSpan) => "duration",
        _ when type == typeof(Guid) => "uuid",
        _ when type == typeof(Uri) => "uri",
        _ => null,
    };

    /// <summary>A schema naming one type.</summary>
    /// <param name="name">The type.</param>
    /// <returns>The schema.</returns>
    private static JsonObject Typed(string name) => new() { ["type"] = name };

    /// <summary>A schema for a value that may be absent.</summary>
    /// <param name="schema">The schema of the value when it is there.</param>
    /// <returns>The schema.</returns>
    private static JsonObject MayBeAbsent(JsonObject schema) => new()
    {
        ["oneOf"] = new JsonArray(new JsonObject { ["type"] = "null" }, schema),
    };
}

using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gradio.Net.Utils;

/// <summary>
/// Serializes tuples as JSON arrays, matching the behaviour of the Python implementation,
/// where a tuple is always emitted as an array (e.g. <c>["label", "value"]</c>).
/// </summary>
/// <remarks>
/// Several component properties are declared as tuples, for example
/// <see cref="Gradio.Net.Components.Radio.Choices"/> is a <c>List&lt;(string Name, object Value)&gt;</c>
/// and <see cref="Gradio.Net.Components.Dataframe.RowCount"/> is a <c>(int Count, string Mode)</c>.
/// System.Text.Json ignores <see cref="ValueTuple"/> fields by default, so those values were
/// serialized as empty JSON objects (<c>{}</c>). When a component instance is returned from an event
/// handler the update payload is built by reading the properties back off the component
/// (see <c>BlockHelpers.PostprocessUpdateDict</c>, the port of Python's
/// <c>postprocess_update_dict</c> which uses <c>getattr(block, key)</c>), so the frontend received
/// <c>"choices": [{}, {}]</c> and rendered an empty choice list.
/// </remarks>
public sealed class TupleJsonConverterFactory : JsonConverterFactory
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert is not null && typeof(ITuple).IsAssignableFrom(typeToConvert);
    }

    /// <inheritdoc />
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        return (JsonConverter)Activator.CreateInstance(
            typeof(TupleJsonConverter<>).MakeGenericType(typeToConvert))!;
    }

    private sealed class TupleJsonConverter<TTuple> : JsonConverter<TTuple>
        where TTuple : ITuple
    {
        public override TTuple? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            JsonElement element = document.RootElement;
            if (element.ValueKind != JsonValueKind.Array)
            {
                throw new JsonException($"Cannot convert {element.ValueKind} to {typeToConvert}.");
            }

            Type[] itemTypes = typeToConvert.GetGenericArguments();
            object?[] items = new object?[itemTypes.Length];
            int index = 0;
            foreach (JsonElement item in element.EnumerateArray())
            {
                if (index >= itemTypes.Length)
                {
                    break;
                }

                items[index] = item.Deserialize(itemTypes[index], options);
                index++;
            }

            if (index != itemTypes.Length)
            {
                throw new JsonException(
                    $"Cannot convert a JSON array of {index} item(s) to {typeToConvert}.");
            }

            return (TTuple?)Activator.CreateInstance(typeToConvert, items);
        }

        public override void Write(Utf8JsonWriter writer, TTuple value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            for (int i = 0; i < value.Length; i++)
            {
                JsonSerializer.Serialize(writer, value[i], options);
            }

            writer.WriteEndArray();
        }
    }
}

using Gradio.Net.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gradio.Net;

internal class SSEMessageTypeJsonConverter : JsonConverter<SSEMessageType>
{
    public override SSEMessageType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, SSEMessageType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString().ToSnakeCase().ToLowerInvariant());
    }
}

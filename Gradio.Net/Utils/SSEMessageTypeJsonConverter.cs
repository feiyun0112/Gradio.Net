using Gradio.Net.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Gradio.Net
{
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
}

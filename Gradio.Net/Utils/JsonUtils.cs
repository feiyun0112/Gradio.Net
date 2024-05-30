using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Gradio.Net.Models;

namespace Gradio.Net
{
    internal static class JsonUtils
    {
        private static JsonSerializerOptions _serializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        internal static T Deserialize<T>(string str)
        {
            return JsonSerializer.Deserialize<T>(str, _serializeOptions);
        }

        internal static string Serialize<T>(T obj)
        {
            return JsonSerializer.Serialize(obj, _serializeOptions);
        }
    }
}

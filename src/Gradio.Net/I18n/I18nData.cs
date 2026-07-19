using System.Text.Json.Serialization;

namespace Gradio.Net.I18n;

public class I18nData
{
    public string Key { get; }

    [JsonPropertyName("__type__")]
    public string Type => "translation_metadata";

    public I18nData(string key)
    {
        Key = key;
    }

    public Dictionary<string, object> ToDict()
    {
        return new Dictionary<string, object>
        {
            { "__type__", Type },
            { "key", Key }
        };
    }

    public override string ToString()
    {
        // Python parity: json.dumps uses ", " separator and ": " key-value separator
        // e.g. {"__type__": "translation_metadata", "key": "chat_interface.chatbot"}
        var options = new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = false,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        // Manually build to match Python's json.dumps format with spaces
        return $"__i18n__{{\"__type__\": \"translation_metadata\", \"key\": \"{Key}\"}}";
    }

    public override int GetHashCode()
    {
        return Key.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is I18nData other)
        {
            return Key == other.Key;
        }
        return false;
    }

    public static string operator +(I18nData left, object right)
    {
        return left.ToString() + right?.ToString() ?? string.Empty;
    }

    public static string operator +(object left, I18nData right)
    {
        return left?.ToString() ?? string.Empty + right.ToString();
    }

    public Dictionary<string, object> ToJson()
    {
        return ToDict();
    }
}

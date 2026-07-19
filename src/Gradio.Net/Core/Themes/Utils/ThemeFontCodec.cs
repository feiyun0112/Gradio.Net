using System.Text.Json;

namespace Gradio.Net.Core.Themes.Utils;

public static class ThemeFontCodec
{
    public static object AsFont(JsonElement dct)
    {
        if (!dct.TryGetProperty("__gradio_font__", out _))
            return dct;

        var name = dct.GetProperty("name").GetString() ?? string.Empty;
        var klass = dct.GetProperty("class").GetString() ?? "font";
        var weights = dct.TryGetProperty("weights", out var w) && w.ValueKind == JsonValueKind.Array
            ? w.EnumerateArray().Where(x => x.TryGetInt32(out _)).Select(x => x.GetInt32()).ToArray()
            : null;

        return klass switch
        {
            "google" => new GoogleFont(name, weights),
            "local" => new LocalFont(name, weights),
            _ => new Font(name)
        };
    }

    public static string Encode(Font font)
    {
        var payload = new Dictionary<string, object?>
        {
            ["__gradio_font__"] = true,
            ["name"] = font.Name,
            ["class"] = font is GoogleFont ? "google" : font is LocalFont ? "local" : "font",
            ["weights"] = font is GoogleFont gf ? gf.Weights : font is LocalFont lf ? lf.Weights : null
        };
        return JsonSerializer.Serialize(payload);
    }
}

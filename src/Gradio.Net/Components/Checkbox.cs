namespace Gradio.Net.Components;

[Events.Event("change")]
[Events.Event("input")]
[Events.Event("select")]
public class Checkbox : FormComponent
{
    public Checkbox(
        bool? value = false,
        string? label = null,
        string? info = null,
        object? every = null,
        object? inputs = null,
        bool? showLabel = null,
        bool? container = null,
        int? scale = null,
        int? minWidth = null,
        bool? interactive = null,
        bool visible = true,
        string? elemId = null,
        List<string> elemClasses = null,
        bool render = true,
        object? key = null,
        string? preservedByKey = null)
    {
        Value = value;
        Label = label;
        Info = info;
        ShowLabel = showLabel ?? ShowLabel;
        if (container.HasValue) Container = container.Value;
        Scale = scale;
        MinWidth = minWidth;
        Interactive = interactive;
        Visible = visible;
        ElemId = elemId;
        ElemClasses = elemClasses ?? null;
        Key = key;
        PreservedByKey = preservedByKey != null ? new List<string> { preservedByKey } : new List<string>();
    }

    public bool? Value { get; set; }

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        // Python get_config() forces show_label=true when label is not null
        if (Label != null)
        {
            ShowLabel = true;
        }
        config["label"] = Label;
        config["show_label"] = ShowLabel;
        config["value"] = Value;
        return config;
    }

    public override Dictionary<string, object> ApiInfo()
    {
        return new Dictionary<string, object>
        {
            { "type", "boolean" }
        };
    }

    public override object Preprocess(object input)
    {
        if (input is bool b) return b;
        if (input is System.Text.Json.JsonElement je)
        {
            if (je.ValueKind == System.Text.Json.JsonValueKind.True || je.ValueKind == System.Text.Json.JsonValueKind.False)
            {
                return je.GetBoolean();
            }

            var text = je.ToString();
            if (bool.TryParse(text, out var parsedFromJson)) return parsedFromJson;
        }
        if (input is string s && bool.TryParse(s, out var result)) return result;
        return base.Preprocess(input);
    }

    public override object Postprocess(object output)
    {
        if (output is bool b) return b;
        if (output is System.Text.Json.JsonElement je)
        {
            if (je.ValueKind == System.Text.Json.JsonValueKind.True || je.ValueKind == System.Text.Json.JsonValueKind.False)
            {
                return je.GetBoolean();
            }

            var text = je.ToString();
            if (bool.TryParse(text, out var parsedFromJson)) return parsedFromJson;
        }
        if (output == null) return false;
        if (output is string s && bool.TryParse(s, out var parsed)) return parsed;
        return true;
    }

    public override List<object> ExampleInputs()
    {
        return new List<object> { true };
    }
}

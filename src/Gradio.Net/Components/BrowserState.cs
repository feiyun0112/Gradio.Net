namespace Gradio.Net.Components;

[Events.Event("change")]
public class BrowserState : Component
{
    public BrowserState(
        object? defaultValue = null,
        string? storageKey = null,
        string? secret = null,
        bool render = true)
    {
        DefaultValue = defaultValue;
        Secret = secret ?? global::Gradio.Net.Utils.Utils.GenerateRandomString(16);
        StorageKey = storageKey ?? global::Gradio.Net.Utils.Utils.GenerateRandomString(16);
    }

    public object? DefaultValue { get; set; }
    public string Secret { get; set; }
    public string StorageKey { get; set; }

    public override string GetBlockName() => "browserstate";

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        return new Dictionary<string, object>
        {
            ["default_value"] = DefaultValue,
            ["storage_key"] = StorageKey,
            ["secret"] = Secret,
            ["name"] = GetBlockClass(),
            ["_selectable"] = Selectable ?? false
        };
    }

    public override object? Preprocess(object? payload)
    {
        return payload ?? DefaultValue;
    }

    public override object? Postprocess(object? value)
    {
        return value;
    }

    public override Dictionary<string, object> ApiInfo()
    {
        return new Dictionary<string, object>
        {
            ["type"] = new Dictionary<string, object>(),
            ["description"] = "any json-serializable value"
        };
    }

    public override object ExamplePayload() => "test";

    public override object ExampleValue() => "test";

    public override bool BreaksGrouping() => false;
}

using System.Text.Json;

namespace Gradio.Net.Components;

[Events.Event("change")]
public class State : Component
{
    public double TimeToLive { get; set; }
    public Action<object?> DeleteCallback { get; set; }

    public State(
        object? value = null,
        bool render = true,
        double? timeToLive = null,
        Action<object?>? deleteCallback = null)
    {
        TimeToLive = timeToLive ?? double.PositiveInfinity;
        DeleteCallback = deleteCallback ?? DefaultDeleteCallback;
        Value = DeepCopyOrThrow(value);
    }

    public override string GetBlockName() => "state";

    public override bool Stateful => true;

    public override bool SkipApi => true;

    public override IReadOnlyList<string> KeepNullProps => new[] { "time_to_live", "delete_callback" };

    public override object Preprocess(object input)
    {
        return input;
    }

    public override object Postprocess(object output)
    {
        return output;
    }

    public override Dictionary<string, object> ApiInfo()
    {
        return new Dictionary<string, object>
        {
            ["type"] = new Dictionary<string, object>(),
            ["description"] = "any valid json"
        };
    }

    public override object ExamplePayload() => null!;

    public override object ExampleValue() => null!;

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config.Remove("value");
        config.Remove("Value");
        config.Remove("show_label");
        config.Remove("container");
        config.Remove("min_width");
        config.Remove("ShowLabel");
        config.Remove("Container");
        config.Remove("MinWidth");

        // Keep state-specific constructor keys present in config.
        config["delete_callback"] = "<function default_delete_callback>";
        // System.Text.Json (strict JSON) cannot write non-finite number literals.
        // Keep semantic parity: non-finite TTL means "store indefinitely".
        config["time_to_live"] = double.IsFinite(TimeToLive) ? TimeToLive : null!;
        return config;
    }

    public override bool BreaksGrouping() => false;

    private static void DefaultDeleteCallback(object? x)
    {
        // no-op
    }

    private static object? DeepCopyOrThrow(object? value)
    {
        if (value == null)
        {
            return null;
        }

        var t = value.GetType();
        if (t.IsValueType || value is string)
        {
            return value;
        }

        if (value is ICloneable cloneable)
        {
            return cloneable.Clone();
        }

        try
        {
            var json = JsonSerializer.Serialize(value, t);
            return JsonSerializer.Deserialize(json, t);
        }
        catch (Exception ex)
        {
            throw new TypeLoadException(
                $"The initial value of `gr.State` must be able to be deepcopied. The initial value of type {t} cannot be deepcopied.",
                ex);
        }
    }
}

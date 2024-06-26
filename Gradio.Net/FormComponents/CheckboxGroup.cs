using Gradio.Net.Enums;
using System.ComponentModel;
using System.Text.Json;


namespace Gradio.Net;

public class CheckboxGroup : FormComponent, IHaveChangeEvent, IHaveInputEvent, IHaveSelectEvent
{
    internal CheckboxGroup() { }
    internal IEnumerable<string> Choices { get; set; }
    internal CheckboxGroupType? Type { get; set; }
    static Dictionary<string, object> _defaultProps = new Dictionary<string, object>()
    {
        { nameof(Type),  CheckboxGroupType.Value },
         { nameof(Container), true },
          { nameof(MinWidth), 160 },
           { nameof(Visible), true },
        { nameof(Render), true },
    };
    protected override object? GetDefaultProp(string name) => _defaultProps.ContainsKey(name) ? _defaultProps[name] : null;

    public static IEnumerable<string> Payload(object obj)
    {
        if (obj == null)
        {
            return null;
        }

        if (obj is IEnumerable<string> list)
        {
            return list;
        }

        throw new ArgumentException($"Payload Type expect IEnumerable<string> actual {obj.GetType()}");
    }

    protected override Dictionary<string, object> GetProps(bool useDefaultValue)
    {
        Dictionary<string, object> result = base.GetProps(useDefaultValue);
        if (useDefaultValue || this.Choices != null)
        {
            var choices = this.GetPropertyValue<IEnumerable<string>>(nameof(Choices)) ?? Array.Empty<string>();
            result["choices"] = choices.Select(x => new[] { x, x }).ToArray();
        }
        return result;
    }

    protected override Dictionary<string, object> GetApiInfo()
    {
        return new Dictionary<string, object>() { { "type", "string" } };
    }

    internal override object PreProcess(object data)
    {

        if (data == null)
        {
            return new List<string>();
        }

        if (data is JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.String)
            {
                return new[] { data.ToString() };
            }

            if (element.ValueKind == JsonValueKind.Array)
            {
                return JsonUtils.Deserialize<string[]>(data.ToString());
            }

        }
        string? str = data.ToString();

        string[] choices = JsonUtils.Deserialize<string[]>(str);
        if (choices != null)
        {
            return choices;
        }

        return new[] { str };
    }

    internal override object PostProcess(string rootUrl, object data)
    {
        if (data == null)
        {
            return new List<string>();
        }

        if (data is IEnumerable<string>)
        {
            return data;
        }

        string? str = data.ToString();

        string[] choices = JsonUtils.Deserialize<string[]>(str);
        if (choices != null)
        {
            return choices;
        }

        return new[] { str };
    }
}

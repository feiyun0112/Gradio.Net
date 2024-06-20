using Gradio.Net.Enums;


namespace Gradio.Net;

public class MultimodalTextbox : FormComponent, IHaveChangeEvent, IHaveInputEvent, IHaveSelectEvent, IHaveSubmitEvent, IHaveFocusEvent, IHaveBlurEvent
{
    internal MultimodalTextbox() { }
    internal object SubmitBtn { get; set; }
    internal int? Lines { get; set; }
    internal int? MaxLines { get; set; }
    internal string Placeholder { get; set; }
    internal bool? Autofocus { get; set; }
    internal bool? Autoscroll { get; set; }

    internal TextboxTextAlign? TextAlign { get; set; }

    static Dictionary<string, object> _defaultProps = new Dictionary<string, object>()
    {  { nameof(Lines), 1 },
         { nameof(MaxLines), 20 },
          { nameof(ShowLabel), true },
            { nameof(Container), true },
        { nameof(MinWidth), 160 },
         { nameof(Visible), true},

           { nameof(Autofocus), false },

               { nameof(Autoscroll), true },


           { nameof(Render), true },


            { nameof(TextAlign), TextboxTextAlign.Left },
             { nameof(Rtl), false },
    };
    protected override object? GetDefaultProp(string name) => _defaultProps.ContainsKey(name) ? _defaultProps[name] : null;

    public static MultimodalData Payload(object obj)
    {
        if (obj == null)
        {
            return null;
        }

        if (obj is MultimodalData str)
        {
            return str;
        }

        throw new ArgumentException($"Payload Type expect MultimodalData actual {obj.GetType()}");
    }

    protected override Dictionary<string, object> GetApiInfo()
    {
        return new Dictionary<string, object>() { { "type", "string" } };
    }

    internal override object PreProcess(object data)
    {
        MultimodalData result = null;
        if (data == null)
        {
            return result;
        }

        result = JsonUtils.Deserialize<MultimodalData>(data.ToString());
        return result;
    }

    internal override object PostProcess(string rootUrl, object data)
    {
        MultimodalData result = null;
        if (data == null)
        {
            return result;
        }

        result = JsonUtils.Deserialize<MultimodalData>(data.ToString());
        return result;
    }
}

using Gradio.Net.Enums;


namespace Gradio.Net;

public class Textbox : FormComponent, IHaveChangeEvent, IHaveInputEvent, IHaveSelectEvent, IHaveSubmitEvent, IHaveFocusEvent, IHaveBlurEvent
{
    internal Textbox() { }
    internal int? Lines { get; set; }
    internal int? MaxLines { get; set; }
    internal string Placeholder { get; set; }
    internal bool? Autofocus { get; set; }
    internal bool? Autoscroll { get; set; }
    internal TextboxType? Type { get; set; }
    internal TextboxTextAlign? TextAlign { get; set; }
    internal bool? ShowCopyButton { get; set; }

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
            { nameof(Type), TextboxType.Text },

            { nameof(TextAlign), TextboxTextAlign.Left },
             { nameof(Rtl), false },
               { nameof(ShowCopyButton), false },
    };
    protected override object? GetDefaultProp(string name) => _defaultProps.ContainsKey(name) ? _defaultProps[name] : null;


    public static string Payload(object obj)
    {
        if (obj == null)
        {
            return null;
        }

        if (obj is string str)
        {
            return str;
        }

        throw new ArgumentException($"Payload Type expect string actual {obj.GetType()}");
    }

    protected override Dictionary<string, object> GetApiInfo()
    {
        return new Dictionary<string, object>() { { "type", "string" } };
    }

    internal override object PreProcess(object data)
    {
        string result = null;
        if (data == null)
        {
            return result;
        }

        result = data.ToString();
        return result;
    }

    internal override object PostProcess(string rootUrl, object data)
    {
        if (data == null)
        {
            return null;
        }

        return data.ToString();
    }
}

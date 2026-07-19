using Gradio.Net.Core.Exceptions;

namespace Gradio.Net.Components;

[Events.Event("change")]
[Events.Event("input")]
[Events.Event("select")]
[Events.Event("submit")]
[Events.Event("focus")]
[Events.Event("blur")]
[Events.Event("stop")]
[Events.Event("copy")]
public class Textbox : FormComponent
{
    public string Type { get; set; }
    public int Lines { get; set; }
    public int? MaxLines { get; set; }
    public object Placeholder { get; set; }
    public bool Autofocus { get; set; }
    public bool Autoscroll { get; set; }
    public List<string> Buttons { get; set; }
    public string TextAlign { get; set; }
    public bool Rtl { get; set; }
    public int? MaxLength { get; set; }
    public object SubmitBtn { get; set; }
    public object StopBtn { get; set; }
    public InputHTMLAttributes HtmlAttributes { get; set; }

    public Textbox(
        object value = null,
        string label = null,
        object every = null,
        object inputs = null,
        bool? showLabel = null,
        bool container = true,
        int? scale = null,
        int minWidth = 160,
        bool? interactive = null,
        object visible = null,
        string elemId = null,
        object elemClasses = null,
        bool autofocus = false,
        bool render = true,
        object key = null,
        object preservedByKey = null,
        string type = "text",
        int lines = 1,
        int? maxLines = null,
        object placeholder = null,
        List<string> buttons = null,
        object submitBtn = null,
        object stopBtn = null,
        bool autoscroll = true,
        string textAlign = null,
        bool rtl = false,
        int? maxLength = null,
        InputHTMLAttributes htmlAttributes = null,
        string info = null
    )
    {
        Value = value;
        Label = label;
        Info = info;
        ShowLabel = showLabel ?? true;
        Container = container;
        Scale = scale;
        MinWidth = minWidth;
        Interactive = interactive;
        if (visible is bool vb) Visible = vb;
        ElemId = elemId;

        if (elemClasses is List<string> classesList)
        {
            ElemClasses = classesList;
        }
        else if (elemClasses is string classesStr)
        {
            ElemClasses = new List<string> { classesStr };
        }
        else if (elemClasses is IEnumerable<string> classesEnumerable)
        {
            ElemClasses = classesEnumerable.ToList();
        }

        Key = key;
        if (preservedByKey is List<string> preservedList)
        {
            PreservedByKey = preservedList;
        }
        else if (preservedByKey is string preservedString)
        {
            PreservedByKey = new List<string> { preservedString };
        }
        else if (preservedByKey is IEnumerable<string> preservedEnumerable)
        {
            PreservedByKey = preservedEnumerable.ToList();
        }

        if (!new List<string> { "text", "password", "email" }.Contains(type))
        {
            throw new Exception("`type` must be one of \"text\", \"password\", or \"email\".");
        }

        if (type == "password" || type == "email")
        {
            if (lines != 1)
            {
                lines = 1;
            }
            if (maxLines != null && maxLines != 1)
            {
                maxLines = 1;
            }
        }

        Type = type;
        Lines = lines;
        MaxLines = maxLines;
        Placeholder = placeholder;
        Buttons = buttons;
        SubmitBtn = submitBtn ?? false;   // Python default: submit_btn=False
        StopBtn = stopBtn ?? false;
        Autofocus = autofocus;
        Autoscroll = autoscroll;
        TextAlign = textAlign;
        Rtl = rtl;
        MaxLength = maxLength;
        HtmlAttributes = htmlAttributes;
    }

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);

        config["type"] = Type;
        config["lines"] = Lines;

        if (MaxLines != null)
        {
            config["max_lines"] = MaxLines;
        }
        config["placeholder"] = Placeholder;
        config["autofocus"] = Autofocus;
        config["autoscroll"] = Autoscroll;
        config["buttons"] = Buttons ?? new List<string>();
        config["text_align"] = TextAlign;
        config["rtl"] = Rtl;
        config["max_length"] = MaxLength;
        config["submit_btn"] = SubmitBtn;
        config["stop_btn"] = StopBtn;
        config["html_attributes"] = SerializeHtmlAttributes(HtmlAttributes);

        var ordered = new Dictionary<string, object>
        {
            ["value"] = config.ContainsKey("value") ? config["value"] : Value,
            ["type"] = config.ContainsKey("type") ? config["type"] : Type,
            ["lines"] = config.ContainsKey("lines") ? config["lines"] : Lines,
            ["label"] = config.ContainsKey("label") ? config["label"] : Label,
            ["info"] = config.ContainsKey("info") ? config["info"] : Info,
            ["show_label"] = config.ContainsKey("show_label") ? config["show_label"] : ShowLabel,
            ["container"] = config.ContainsKey("container") ? config["container"] : Container,
            ["scale"] = config.ContainsKey("scale") ? config["scale"] : Scale,
            ["min_width"] = config.ContainsKey("min_width") ? config["min_width"] : MinWidth,
            ["interactive"] = config.ContainsKey("interactive") ? config["interactive"] : Interactive,
            ["visible"] = config.ContainsKey("visible") ? config["visible"] : Visible,
            ["elem_id"] = config.ContainsKey("elem_id") ? config["elem_id"] : ElemId,
            ["autofocus"] = config.ContainsKey("autofocus") ? config["autofocus"] : Autofocus,
            ["autoscroll"] = config.ContainsKey("autoscroll") ? config["autoscroll"] : Autoscroll,
            ["elem_classes"] = config.ContainsKey("elem_classes") ? config["elem_classes"] : (ElemClasses ?? new List<string>()),
            ["key"] = config.ContainsKey("key") ? config["key"] : Key,
            ["preserved_by_key"] = config.ContainsKey("preserved_by_key") ? config["preserved_by_key"] : PreservedByKey,
            ["rtl"] = config.ContainsKey("rtl") ? config["rtl"] : Rtl,
            ["buttons"] = config.ContainsKey("buttons") ? config["buttons"] : (Buttons ?? new List<string>()),
            ["submit_btn"] = config.ContainsKey("submit_btn") ? config["submit_btn"] : SubmitBtn,
            ["stop_btn"] = config.ContainsKey("stop_btn") ? config["stop_btn"] : StopBtn,
            ["name"] = config.ContainsKey("name") ? config["name"] : GetBlockClass(),
            ["_selectable"] = config.ContainsKey("_selectable") ? config["_selectable"] : (Selectable ?? false)
        };

        if (config.ContainsKey("max_lines")) ordered["max_lines"] = config["max_lines"];
        if (config.ContainsKey("placeholder")) ordered["placeholder"] = config["placeholder"];
        if (config.ContainsKey("text_align")) ordered["text_align"] = config["text_align"];
        if (config.ContainsKey("max_length")) ordered["max_length"] = config["max_length"];
        if (config.ContainsKey("html_attributes")) ordered["html_attributes"] = config["html_attributes"];

        return ordered;
    }

    public override Dictionary<string, object> ApiInfo()
    {
        // Python: api_info() -> {"type": "string"}
        return new Dictionary<string, object>
            {
                { "type", "string" }
            };
    }

    public override List<object> ExampleInputs()
    {
        return new List<object> { "Hello!!" };
    }

    internal static Dictionary<string, object>? SerializeHtmlAttributes(InputHTMLAttributes attrs)
    {
        if (attrs == null)
        {
            return null;
        }

        return new Dictionary<string, object>
            {
                { "autocapitalize", attrs.Autocapitalize },
                { "autocorrect", attrs.Autocorrect },
                { "spellcheck", attrs.Spellcheck },
                { "autocomplete", attrs.Autocomplete },
                { "tabindex", attrs.Tabindex },
                { "enterkeyhint", attrs.Enterkeyhint },
                { "lang", attrs.Lang }
            };
    }

    public override object Preprocess(object payload)
    {
        return payload == null ? null : payload.ToString();
    }

    public override object Postprocess(object value)
    {
        return value == null ? null : value.ToString();
    }

    public override object ExamplePayload()
    {
        return "Hello!!";
    }

    public override object ExampleValue()
    {
        return "Hello!!";
    }
}

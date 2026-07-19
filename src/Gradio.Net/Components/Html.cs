
namespace Gradio.Net.Components;

[Events.Event("click")]
[Events.Event("change")]
[Events.Event("input")]
[Events.Event("focus")]
[Events.Event("blur")]
[Events.Event("keydown")]
[Events.Event("keyup")]
[Events.Event("submit")]
[Events.Event("select")]
[Events.Event("upload")]
[Events.Event("delete")]
[Events.Event("clear")]
public class Html : Component
{
    public string HtmlTemplate { get; set; }
    public string CssTemplate { get; set; }
    public string? JsOnLoad { get; set; }
    public bool ApplyDefaultCss { get; set; }
    public int? MinHeight { get; set; }
    public int? MaxHeight { get; set; }
    public bool Padding { get; set; }
    public bool Autoscroll { get; set; }
    public Dictionary<string, object> Props { get; set; }
    public string ComponentClassName { get; set; }

    public Html(
        object? value = null,
        string? label = null,
        string htmlTemplate = "${value}",
        string cssTemplate = "",
        string? jsOnLoad = "element.addEventListener('click', function() { trigger('click') });",
        bool applyDefaultCss = true,
        object? every = null,
        object? inputs = null,
        bool showLabel = false,
        object? visible = null,
        string? elemId = null,
        object? elemClasses = null,
        bool render = true,
        object? key = null,
        object? preservedByKey = null,
        int? minHeight = null,
        int? maxHeight = null,
        bool container = false,
        bool padding = false,
        bool autoscroll = false,
        Dictionary<string, object>? props = null)
    {
        HtmlTemplate = htmlTemplate;
        CssTemplate = cssTemplate;
        JsOnLoad = jsOnLoad;
        ApplyDefaultCss = applyDefaultCss;
        MinHeight = minHeight;
        MaxHeight = maxHeight;
        Padding = padding;
        Autoscroll = autoscroll;
        Props = props ?? new Dictionary<string, object>();
        ComponentClassName = GetType().Name;

        Value = value;
        Label = label;
        ShowLabel = showLabel;
        Container = container;
        if (visible is bool vb) Visible = vb;
        ElemId = elemId;
        ElemClasses = elemClasses switch
        {
            string one => new List<string> { one },
            List<string> many => many,
            _ => ElemClasses
        };
        Key = key;
        PreservedByKey = preservedByKey switch
        {
            string one => new List<string> { one },
            List<string> many => many,
            _ => PreservedByKey
        };
    }

    public override string GetBlockName() => "html";

    public override object ExamplePayload() => "<p>Hello</p>";

    public override object ExampleValue() => "<p>Hello</p>";

    public override object? Preprocess(object input)
    {
        return input?.ToString();
    }

    public override object? Postprocess(object output)
    {
        return output?.ToString();
    }

    public override Dictionary<string, object> ApiInfo()
    {
        return new Dictionary<string, object> { ["type"] = "string" };
    }

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["html_template"] = HtmlTemplate;
        config["css_template"] = CssTemplate;
        config["js_on_load"] = JsOnLoad;
        config["apply_default_css"] = ApplyDefaultCss;
        config["min_height"] = MinHeight;
        config["max_height"] = MaxHeight;
        config["padding"] = Padding;
        config["autoscroll"] = Autoscroll;
        config["props"] = Props;
        config["component_class_name"] = ComponentClassName;
        return config;
    }
}

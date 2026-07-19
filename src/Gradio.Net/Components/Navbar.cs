using Gradio.Net.Core;
using Gradio.Net.Events;

namespace Gradio.Net.Components;

public class Navbar : Component
{
    public static readonly List<string> EVENTS = ["change"];

    public object MainPageName { get; set; }

    public Navbar(
        List<(string, string)>? value = null,
        bool visible = true,
        object? mainPageName = null,
        string? elemId = null,
        object? elemClasses = null,
        bool render = true,
        object? key = null)
        : base()
    {
        List<string>? convertedElemClasses = null;
        if (elemClasses is List<string> list)
            convertedElemClasses = list;
        else if (elemClasses is string str)
            convertedElemClasses = new List<string> { str };

        Key = key;
        Visible = visible;
        Value = value;
        MainPageName = mainPageName ?? "Home";

        if (render) Render();
    }

    protected override string GetComponentClassId() => "navbar";

    public List<(string, string)>? Preprocess(List<(string, string)>? payload) => payload;
    public override object Preprocess(object input) => input;

    public List<(string, string)>? Postprocess(List<(string, string)>? value) => value;
    public override object Postprocess(object output) => output;

    public override Dictionary<string, object> ApiInfo() => new Dictionary<string, object>();

    public List<(string, string)>? ExamplePayload() => null;
    public List<(string, string)>? ExampleValue() => null;

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["visible"] = Visible;
        config["main_page_name"] = MainPageName;
        return config;
    }
}


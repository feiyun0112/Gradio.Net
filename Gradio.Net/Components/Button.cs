using Gradio.Net.Enums;

namespace Gradio.Net;

public class Button : Component, IHaveClickEvent
{
    internal Button() { }

    internal string Icon { get;  set; }
    internal ButtonVariant Variant { get;   set; }
    internal ButtonSize? Size { get;   set; }
    internal string Link { get;   set; }


    protected override Dictionary<string, object> GetApiInfo()
    {
        return new Dictionary<string, object>() { { "type", "string" } };
    }

       }

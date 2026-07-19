using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static Components.Timer Timer(
        double value = 1,
        bool active = true,
        bool render = true)
    {
        return new Components.Timer(
            value: value,
            active: active,
            render: render);
    }
}

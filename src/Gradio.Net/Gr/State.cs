using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static State State(
        object? value = null,
        bool render = true,
        double? timeToLive = null,
        Action<object?>? deleteCallback = null)
    {
        return new State(
            value: value,
            render: render,
            timeToLive: timeToLive,
            deleteCallback: deleteCallback);
    }
}

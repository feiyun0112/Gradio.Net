using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static BrowserState BrowserState(
        object defaultValue = null,
        string storageKey = null,
        string secret = null,
        bool render = true)
    {
        return new BrowserState(
            defaultValue: defaultValue,
            storageKey: storageKey,
            secret: secret,
            render: render);
    }
}

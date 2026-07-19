using Gradio.Net.Components;
using Gradio.Net.Core;

namespace Gradio.Net;

public static partial class gr
{
    public static Dictionary<string, object> Skip()
    {
        return Helpers.Skip();
    }

    public static Dictionary<string, object> Validate(bool isValid, string message)
    {
        return Helpers.Validate(isValid, message);
    }
}

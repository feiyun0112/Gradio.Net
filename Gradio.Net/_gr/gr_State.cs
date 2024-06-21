using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection;
using System;

namespace Gradio.Net;

public static partial class gr
{
    public static State State(object value = null,
        bool? render = null,
        decimal? timeToLive = null,
        Action<object> deleteCallback = null)
    {
        State block = new()
        {
            TimeToLive = timeToLive,
            DeleteCallback = deleteCallback,
            Value = value,
            Render = render
        };
        Context.AddToCurrentBlocks(block);
        return block;
    }
}

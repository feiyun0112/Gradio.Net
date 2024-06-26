
using Gradio.Net.Helpers;

namespace Gradio.Net;

public static partial class gr
{

    public static Progress Progress(int total)
    {
        return new Progress(total);
    }

    public static void Info(string message)
    {
        Context.LogMessageChannel.Writer.TryWrite(LogMessage.Info(message));
    }
    public static void Warning(string message)
    {
        Context.LogMessageChannel.Writer.TryWrite(LogMessage.Warning(message));
    }
}

using Gradio.Net.Core;

namespace Gradio.Net;

public static partial class gr
{
    public static void Warning(
        string message = "Warning issued.",
        double? duration = 10,
        bool visible = true,
        string title = "Warning")
    {
        Helpers.Warning(message, duration, visible, title);
    }

    public static void Info(
        string message = "Info issued.",
        double? duration = 10,
        bool visible = true,
        string title = "Info")
    {
        Helpers.Info(message, duration, visible, title);
    }

    public static Core.Exceptions.Error Error(
        string message = "Error raised.",
        double? duration = 10,
        bool visible = true,
        string title = "Error",
        bool printException = true)
    {
        return new Core.Exceptions.Error(message, duration, visible, title, printException);
    }
}

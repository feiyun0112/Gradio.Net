namespace Gradio.Net.Utils;

public abstract class ServerReloader : BaseReloader
{
    public abstract EventWaitHandle StopEvent { get; }

    public void Stop()
    {
        StopEvent.Set();
    }

    public string GetDemoName(object module, string defaultName)
    {
        return defaultName;
    }
}

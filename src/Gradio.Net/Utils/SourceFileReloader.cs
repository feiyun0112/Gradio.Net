namespace Gradio.Net.Utils;

public class SourceFileReloader : ServerReloader
{
    private readonly object _app;
    private readonly EventWaitHandle _stopEvent;

    public SourceFileReloader(
        object app,
        List<string> watchDirs,
        string watchModuleName,
        string demoFile,
        object watchModule,
        EventWaitHandle stopEvent,
        string demoName,
        string encoding = "utf-8")
    {
        _app = app;
        _stopEvent = stopEvent;
        _ = watchDirs;
        _ = watchModuleName;
        _ = demoFile;
        _ = watchModule;
        _ = GetDemoName(watchModule, demoName);
        _ = encoding;
    }

    public override object RunningApp => _app;

    public override EventWaitHandle StopEvent => _stopEvent;

    public bool ShouldWatch()
    {
        return !_stopEvent.WaitOne(0);
    }

    public void AlertChange(string changeType = "reload")
    {
        _ = changeType;
    }

    public new void SwapBlocks(object demo)
    {
        _ = RunningApp;
        base.SwapBlocks(demo);
        AlertChange("reload");
    }
}

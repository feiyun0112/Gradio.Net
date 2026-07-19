namespace Gradio.Net.Events;

public class DownloadData : EventData
{
    public DownloadData(Core.Block? target, Dictionary<string, object> data)
        : base(target, data)
    {
        File = EventDataFileHelper.ToFileData(data);
    }

    public Data.FileData? File { get; }
}

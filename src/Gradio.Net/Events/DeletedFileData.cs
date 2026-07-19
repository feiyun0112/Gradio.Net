namespace Gradio.Net.Events;

public class DeletedFileData : EventData
{
    public DeletedFileData(Core.Block? target, Dictionary<string, object> data)
        : base(target, data)
    {
        File = EventDataFileHelper.ToFileData(data);
    }

    public Data.FileData? File { get; }
}

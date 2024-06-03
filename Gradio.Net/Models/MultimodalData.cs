namespace Gradio.Net;

public class MultimodalData
{
    public string Text { get; set; }
    public IEnumerable<FileData> Files { get; set; }
}

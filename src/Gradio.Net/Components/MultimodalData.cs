using Gradio.Net.Data;

namespace Gradio.Net.Components;

public class MultimodalData
{
    public string Text { get; set; }
    public List<FileData> Files { get; set; }

    public MultimodalData()
    {
        Text = string.Empty;
        Files = new List<FileData>();
    }

    public MultimodalData(string text, List<FileData> files = null)
    {
        Text = text;
        Files = files ?? new List<FileData>();
    }
}

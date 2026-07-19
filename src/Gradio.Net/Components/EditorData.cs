using Gradio.Net.Data;

namespace Gradio.Net.Components;

public class EditorData : GradioModel
{
    public FileData Background { get; set; } = null;
    public List<FileData> Layers { get; set; } = new List<FileData>();
    public FileData Composite { get; set; } = null;
    public string Id { get; set; } = null;
}

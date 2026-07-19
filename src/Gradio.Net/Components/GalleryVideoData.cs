using Gradio.Net.Data;

namespace Gradio.Net.Components;

public class GalleryVideoData
{
    public FileData Video { get; set; } = new();
    public string? Caption { get; set; }
}

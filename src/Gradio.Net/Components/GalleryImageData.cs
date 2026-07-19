using Gradio.Net.Data;

namespace Gradio.Net.Components;

public class GalleryImageData
{
    public ImageData Image { get; set; } = new();
    public string? Caption { get; set; }
}

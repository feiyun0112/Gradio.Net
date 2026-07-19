using Gradio.Net.Data;

namespace Gradio.Net.Components;

public class EditorDataBlobs : GradioModel
{
    public byte[] Background { get; set; } = null;
    public List<byte[]> Layers { get; set; } = new List<byte[]>();
    public byte[] Composite { get; set; } = null;
}

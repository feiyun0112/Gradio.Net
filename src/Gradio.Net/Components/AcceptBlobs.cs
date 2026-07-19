using Gradio.Net.Data;

namespace Gradio.Net.Components;

public class AcceptBlobs : GradioModel
{
    public BlobData Data { get; set; }
    public List<Tuple<string, byte[]>> Files { get; set; }
}

using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace Gradio.Net.Data;


public class ImageData : GradioModel
{
    public string? Path { get; set; }

    public string? Url { get; set; }

    public int? Size { get; set; }

    public string? OrigName { get; set; }

    public string? MimeType { get; set; }

    public bool IsStream { get; set; } = false;

    public Dictionary<string, object> Meta { get; set; } = new Dictionary<string, object> { { "_type", "gradio.FileData" } };
}

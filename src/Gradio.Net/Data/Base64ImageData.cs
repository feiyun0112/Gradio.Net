using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace Gradio.Net.Data;


public class Base64ImageData : GradioModel
{
    public string Url { get; set; } = string.Empty;
}


namespace Gradio.Net.Data;

public class APIReturnInfo
{
    public string Label { get; set; } = string.Empty;

    public Dictionary<string, object> Type { get; set; } = new Dictionary<string, object>();

    public Dictionary<string, string> PythonType { get; set; } = new Dictionary<string, string>();

    public string Component { get; set; } = string.Empty;
}

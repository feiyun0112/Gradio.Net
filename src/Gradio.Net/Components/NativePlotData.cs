
namespace Gradio.Net.Components;

public class NativePlotData
{
    public List<string> Columns { get; set; } = new();
    public List<List<object?>> Data { get; set; } = new();
    public Dictionary<string, string> Datatypes { get; set; } = new();
    public string Mark { get; set; } = "native";
}

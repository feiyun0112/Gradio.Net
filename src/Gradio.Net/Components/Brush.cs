
namespace Gradio.Net.Components;

public class Brush : Eraser
{
    public List<object> Colors { get; set; }
    public object DefaultColor { get; set; }
    public string ColorMode { get; set; } = "defaults";

    public Brush()
    {
        Colors = new List<object>
        {
            "rgb(204, 50, 50)",
            "rgb(173, 204, 50)",
            "rgb(50, 204, 112)",
            "rgb(50, 112, 204)",
            "rgb(173, 50, 204)"
        };
        DefaultColor = Colors[0];
    }
}

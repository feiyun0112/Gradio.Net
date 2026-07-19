
namespace Gradio.Net.Components;

public class WebcamOptions
{
    public bool Mirror { get; set; } = true;
    public Dictionary<string, object> Constraints { get; set; } = null;
}

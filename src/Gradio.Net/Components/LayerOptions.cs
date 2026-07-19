
namespace Gradio.Net.Components;

public class LayerOptions
{
    public bool AllowAdditionalLayers { get; set; } = true;
    public List<string> Layers { get; set; }
    public bool Disabled { get; set; } = false;

    public LayerOptions()
    {
        Layers = new List<string> { "Layer 1" };
    }
}

namespace Gradio.Net.Components;

public class WatermarkOptions
{
    public object Watermark { get; set; } = null;
    public object Position { get; set; } = "bottom-right";

    public WatermarkOptions() { }

    public WatermarkOptions(object watermark, object position)
    {
        Watermark = watermark;
        Position = position;
    }
}

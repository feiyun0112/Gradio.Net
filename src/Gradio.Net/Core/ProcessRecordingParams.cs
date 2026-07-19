
namespace Gradio.Net.Core;

public class ProcessRecordingParams
{
    public double? RemoveSegmentStart { get; set; }
    public double? RemoveSegmentEnd { get; set; }
    public List<ZoomEffect> ZoomEffects { get; set; }
}

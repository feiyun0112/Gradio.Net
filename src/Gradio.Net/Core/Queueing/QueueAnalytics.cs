namespace Gradio.Net.Core.Queueing;

public class QueueAnalytics
{
    public DateTime Time { get; set; }
    public string Status { get; set; } = "queued";
    public double? ProcessTime { get; set; }
    public string? Function { get; set; }
    public string? SessionHash { get; set; }
}

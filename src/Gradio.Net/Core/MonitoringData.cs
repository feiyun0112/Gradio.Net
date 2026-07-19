namespace Gradio.Net.Core;

public class MonitoringData
{
    public double Time { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Function { get; set; } = string.Empty;
    public double ProcessTime { get; set; }
    public string SessionHash { get; set; } = string.Empty;
}

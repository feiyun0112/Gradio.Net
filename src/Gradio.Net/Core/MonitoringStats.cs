
namespace Gradio.Net.Core;

public class MonitoringStats
{
    public int UniqueUsers { get; set; }
    public int TotalRequests { get; set; }
    public double AvgProcessTime { get; set; }
    public List<MonitoringData> RequestData { get; set; } = new();
}

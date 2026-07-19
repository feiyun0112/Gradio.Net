namespace Gradio.Net.Core.Queueing;

public class QueueStatus
{
    public int QueueSize { get; set; }
    public int ActiveWorkers { get; set; }
    public int MaxWorkers { get; set; }
}

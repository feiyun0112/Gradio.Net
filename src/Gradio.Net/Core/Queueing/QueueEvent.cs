using Gradio.Net.Core;
using Gradio.Net.Data;
using Gradio.Net.Core.Queueing;

namespace Gradio.Net.Core.Queueing;

public class QueueEvent
{
    public string Id { get; } = Guid.NewGuid().ToString("N");
    public string SessionHash { get; set; }
    public string? FnIndex { get; set; }
    public string? ApiName { get; set; }
    public PredictBodyInternal? Data { get; set; }
    public BlockFunction? Fn { get; set; }
    public string? Username { get; set; }
    public string ConcurrencyId { get; set; } = string.Empty;
    public bool Alive { get; set; } = true;
    public bool Closed { get; set; }
    public int CallCount { get; set; }
    public double RunTimeSeconds { get; set; }
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public ProgressMessage? Progress { get; set; }
    public bool ProgressPending { get; set; }
    public SemaphoreSlim Signal { get; } = new(0, 1);

    public QueueEvent(string sessionHash, string concurrencyId, string? username = null)
    {
        SessionHash = sessionHash ?? Id;
        ConcurrencyId = concurrencyId;
        Username = username;
    }

    public TimeSpan? ProcessingTime => CompletedAt.HasValue && StartedAt.HasValue
        ? CompletedAt.Value - StartedAt.Value
        : null;
}

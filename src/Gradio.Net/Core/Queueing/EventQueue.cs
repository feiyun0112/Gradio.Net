
using Gradio.Net.Core;

namespace Gradio.Net.Core.Queueing;

public class EventQueue
{
    public string ConcurrencyId { get; }
    public int? ConcurrencyLimit { get; set; }
    public int CurrentConcurrency { get; set; }
    public List<QueueEvent> QueueEvents => _queueEvents;
    public Dictionary<BlockFunction, HashSet<float>> StartTimesPerFn { get; } = new();
    private readonly List<QueueEvent> _queueEvents = new();
    private readonly object _lock = new();

    public EventQueue(string concurrencyId, int? concurrencyLimit)
    {
        ConcurrencyId = concurrencyId;
        ConcurrencyLimit = concurrencyLimit;
    }

    public void Add(QueueEvent evt)
    {
        lock (_lock)
        {
            _queueEvents.Add(evt);
        }
    }

    public bool TryDequeue(out QueueEvent? evt)
    {
        lock (_lock)
        {
            if (_queueEvents.Count > 0)
            {
                evt = _queueEvents[0];
                _queueEvents.RemoveAt(0);
                return true;
            }
            evt = null;
            return false;
        }
    }

    public int Count
    {
        get
        {
            lock (_lock)
            {
                return _queueEvents.Count;
            }
        }
    }
}

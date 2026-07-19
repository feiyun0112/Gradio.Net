using System.Collections.Concurrent;

namespace Gradio.Net.Core;

public class StateHolder
{
    public int Capacity { get; set; } = 10000;
    private readonly ConcurrentDictionary<string, SessionState> _sessionData = new();
    private readonly ConcurrentDictionary<string, DateTime> _timeLastUsed = new();
    private readonly ReaderWriterLockSlim _lock = new();
    public Blocks? Blocks { get; set; }

    public ConcurrentDictionary<string, SessionState> SessionData => _sessionData;

    public void SetBlocks(Blocks blocks)
    {
        Blocks = blocks;
        blocks.StateHolder = this;
        Capacity = blocks.StateSessionCapacity;
    }

    public SessionState this[string sessionId] => GetSessionState(sessionId);

    public bool Contains(string sessionId) => _sessionData.ContainsKey(sessionId);

    private SessionState GetSessionState(string sessionId)
    {
        if (!_sessionData.TryGetValue(sessionId, out var sessionState))
        {
            if (Blocks == null)
                throw new InvalidOperationException("Blocks has not been set on StateHolder");
            sessionState = new SessionState(Blocks);
            _sessionData[sessionId] = sessionState;
        }
        Update(sessionId);
        _timeLastUsed[sessionId] = DateTime.Now;
        return sessionState;
    }

    public void Update(string sessionId)
    {
        _lock.EnterWriteLock();
        try
        {
            // Simple LRU cleanup when capacity is exceeded
            if (_sessionData.Count > Capacity)
            {
                var oldestSession = _timeLastUsed.OrderBy(kv => kv.Value).FirstOrDefault();
                if (oldestSession.Key != null)
                {
                    _sessionData.TryRemove(oldestSession.Key, out _);
                    _timeLastUsed.TryRemove(oldestSession.Key, out _);
                }
            }
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public void DeleteAllExpiredState()
    {
        foreach (var sessionId in _sessionData.Keys)
            DeleteState(sessionId, expiredOnly: true);
    }

    public void DeleteState(string sessionId, bool expiredOnly = false)
    {
        if (_sessionData.TryGetValue(sessionId, out var sessionState))
            sessionState.DeleteState(expiredOnly);
    }
}

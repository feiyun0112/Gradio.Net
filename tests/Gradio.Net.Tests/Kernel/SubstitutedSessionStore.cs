using System.Diagnostics.CodeAnalysis;
using Gradio.Net;
namespace Gradio.Net.Tests.Kernel;

/// <summary>
/// A stand-in session store, used to check that a consumer's implementation is the one the kernel
/// resolves — which is what makes moving sessions to a distributed store a registration change
/// rather than a rewrite.
/// </summary>
internal sealed class SubstitutedSessionStore : ISessionStore
{
    private readonly Dictionary<string, Session> _sessions = new(StringComparer.Ordinal);

    /// <summary>Every hash this store has been asked for, in order.</summary>
    internal List<string> Requested { get; } = [];

    /// <inheritdoc />
    public event Action<SessionEviction>? Evicted
    {
        add { }
        remove { }
    }

    /// <inheritdoc />
    public int Count => _sessions.Count;

    /// <inheritdoc />
    public Session GetOrAdd(string sessionHash)
    {
        Requested.Add(sessionHash);

        if (_sessions.TryGetValue(sessionHash, out var existing))
        {
            return existing;
        }

        var session = new Session(sessionHash, DateTimeOffset.UnixEpoch);
        _sessions.Add(sessionHash, session);

        return session;
    }

    /// <inheritdoc />
    public bool TryGet(string sessionHash, [NotNullWhen(true)] out Session? session) =>
        _sessions.TryGetValue(sessionHash, out session);

    /// <inheritdoc />
    public bool Remove(string sessionHash) => _sessions.Remove(sessionHash);

    /// <inheritdoc />
    public int RemoveExpired() => 0;

    /// <inheritdoc />
    public IReadOnlyList<Session> Snapshot() => [.. _sessions.Values];

    /// <inheritdoc />
    public void Clear() => _sessions.Clear();
}

using System.Collections.Concurrent;
using System.Text.Json.Nodes;
using System.Threading.Channels;

namespace Gradio.Net;
/// <summary>
/// The open SSE connections of the MCP endpoint, by session.
/// </summary>
/// <remarks>
/// <para>
/// The older of the two transports MCP defines is a pair of requests: a long-lived
/// <c>GET</c> the server writes answers to, and a <c>POST</c> per message that carries one in. The
/// two are joined by a session id the server hands out on the stream's first event, which is what
/// this holds — the channel a session's answers go out on, for as long as its stream is open.
/// </para>
/// <para>
/// A session exists only while its stream does: the stream's own end is what removes it, so a
/// client that disconnects leaves nothing behind. That is the whole of the lifecycle, which is why
/// there is no sweep here — an abandoned entry cannot exist, because the request that owns it has
/// not finished.
/// </para>
/// </remarks>
internal sealed class McpSessions
{
    private readonly ConcurrentDictionary<string, Channel<JsonObject>> _open = new(StringComparer.Ordinal);

    /// <summary>Opens a session and returns the stream its answers go out on.</summary>
    /// <returns>The session's id, and the reader the SSE response forwards from.</returns>
    internal (string Id, ChannelReader<JsonObject> Answers) Open()
    {
        var id = Guid.NewGuid().ToString("N");
        var channel = Channel.CreateUnbounded<JsonObject>(new UnboundedChannelOptions { SingleReader = true });

        _open[id] = channel;

        return (id, channel.Reader);
    }

    /// <summary>Hands an answer to the session it belongs to.</summary>
    /// <param name="id">The session's id.</param>
    /// <param name="message">The answer.</param>
    /// <returns>Whether the session was open to receive it.</returns>
    /// <remarks>
    /// A message for a session nobody opened is dropped rather than opening one: the session is what
    /// a client's stream created, and an answer is not a reason to keep one alive.
    /// </remarks>
    internal bool Answer(string id, JsonObject message) =>
        _open.TryGetValue(id, out var channel) && channel.Writer.TryWrite(message);

    /// <summary>Whether a session is open.</summary>
    /// <param name="id">The session's id.</param>
    /// <returns>Whether it is.</returns>
    internal bool IsOpen(string id) => _open.ContainsKey(id);

    /// <summary>Closes a session, which is what the end of its stream does.</summary>
    /// <param name="id">The session's id.</param>
    /// <returns>Whether a session was open.</returns>
    internal bool Close(string id)
    {
        if (!_open.TryRemove(id, out var channel))
        {
            return false;
        }

        channel.Writer.TryComplete();

        return true;
    }
}

using System.Collections.Concurrent;

namespace Gradio.Net;
/// <summary>One segment of a media stream, with the identity a playlist names it by.</summary>
/// <param name="Id">The segment's name, which the playlist writes and the segment endpoint looks up.</param>
/// <param name="Data">The segment's bytes.</param>
/// <param name="Duration">How long the segment lasts, in seconds.</param>
/// <param name="Extension">The extension it is served as, with the dot.</param>
internal sealed record MediaStreamSegment(string Id, byte[] Data, double Duration, string Extension);

/// <summary>
/// One streamed output of one run: the segments produced so far, and whether it has ended
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>MediaStream</c>. The playlist is derived from this rather than stored: a
/// player re-fetches the playlist as it plays, so what it says has to be what the stream has now,
/// and a second copy of that would be a second thing to keep in step.
/// </para>
/// <para>
/// A stream outlives the run that produced it, deliberately: the client asks for the playlist after
/// the run's last message, and a stream dropped the moment its run ended would answer 404 for the
/// one request the client makes next.
/// </para>
/// </remarks>
internal sealed class MediaStream
{
    private readonly List<MediaStreamSegment> _segments = [];
    private readonly List<Action> _onEnd = [];

    /// <summary>Creates a stream.</summary>
    /// <param name="desiredOutputFormat">
    /// The format the client would like the combined file in, taken from the name of the file the
    /// component named its output with, or <see langword="null"/> for the component's own.
    /// </param>
    internal MediaStream(string? desiredOutputFormat) => DesiredOutputFormat = desiredOutputFormat;

    /// <summary>The segments produced so far, in the order they were produced.</summary>
    internal IReadOnlyList<MediaStreamSegment> Segments
    {
        get
        {
            lock (_segments)
            {
                return [.. _segments];
            }
        }
    }

    /// <summary>The format the client asked for the combined file in, or <see langword="null"/>.</summary>
    internal string? DesiredOutputFormat { get; }

    /// <summary>Whether the stream has ended, which is what closes the playlist.</summary>
    internal bool Ended { get; private set; }

    /// <summary>
    /// The longest segment, rounded up, which the playlist declares as its target duration.
    /// </summary>
    /// <value>One second until a segment says otherwise, which is the Python version's starting
    /// point.</value>
    internal int MaxDuration { get; private set; } = 1;

    /// <summary>The path of the whole stream as one file, once somebody has asked for it.</summary>
    internal string? CombinedFile { get; set; }

    /// <summary>Adds a segment, if the chunk produced one.</summary>
    /// <param name="chunk">The segment, or <see langword="null"/> for a chunk that produced none.</param>
    /// <remarks>
    /// The name is drawn here rather than by the component: a segment is named by the stream that
    /// holds it, and a component that had to name its own would be deciding something about the
    /// playlist.
    /// </remarks>
    internal void Add(MediaStreamChunk? chunk)
    {
        if (chunk is null)
        {
            return;
        }

        lock (_segments)
        {
            _segments.Add(new MediaStreamSegment(
                Guid.NewGuid().ToString("N"),
                chunk.Data,
                chunk.Duration,
                chunk.Extension));

            MaxDuration = Math.Max(MaxDuration, (int)Math.Ceiling(chunk.Duration));
        }
    }

    /// <summary>Adds something to run when the stream ends.</summary>
    /// <param name="cleanup">What to release — a component's encoder, usually.</param>
    internal void OnEnd(Action cleanup)
    {
        ArgumentNullException.ThrowIfNull(cleanup);

        lock (_onEnd)
        {
            _onEnd.Add(cleanup);
        }
    }

    /// <summary>Ends the stream, releasing whatever the components hold for it.</summary>
    /// <remarks>
    /// A cleanup that throws does not stop the others: this is reached from a failure path as well
    /// as a normal one, and a release that fails must not replace the error being described.
    /// </remarks>
    internal void End()
    {
        if (Ended)
        {
            return;
        }

        Ended = true;

        Action[] cleanups;
        lock (_onEnd)
        {
            cleanups = [.. _onEnd];
            _onEnd.Clear();
        }

        foreach (var cleanup in cleanups)
        {
            try
            {
                cleanup();
            }
            catch (Exception)
            {
                // Nobody is left to tell: the stream is over either way.
            }
        }
    }
}

/// <summary>
/// The streams this application is serving, by session, run and output component
/// </summary>
/// <remarks>
/// <para>
/// The Python version keeps this on the application (<c>Blocks.pending_streams</c>), because the
/// route that serves a playlist and the run that produces its segments are different requests and
/// have to meet somewhere. Here it is a service for the same reason, and for one more: the
/// container that owns the application owns the streams, so a second application in the same
/// process cannot serve another's.
/// </para>
/// <para>
/// A stream is keyed by the session, the run and the output component, which together are what its
/// playlist address names. Two runs of the same binding in the same session therefore do not
/// overwrite each other, and a client that is still playing the older one keeps its playlist.
/// </para>
/// </remarks>
internal sealed class PendingStreams
{
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<int, MediaStream>>> _streams =
        new(StringComparer.Ordinal);

    /// <summary>The stream an output of a run is producing, or <see langword="null"/> for none.</summary>
    /// <param name="sessionHash">The session the run belongs to.</param>
    /// <param name="run">The run's identity.</param>
    /// <param name="componentId">The output component.</param>
    /// <returns>The stream, or <see langword="null"/> when that output opened none.</returns>
    internal MediaStream? Find(string sessionHash, string run, int componentId) =>
        _streams.TryGetValue(sessionHash, out var runs)
        && runs.TryGetValue(run, out var outputs)
        && outputs.TryGetValue(componentId, out var stream)
            ? stream
            : null;

    /// <summary>Opens the stream an output is about to produce.</summary>
    /// <param name="sessionHash">The session the run belongs to.</param>
    /// <param name="run">The run's identity.</param>
    /// <param name="componentId">The output component.</param>
    /// <param name="desiredOutputFormat">The format the client asked for, or <see langword="null"/>.</param>
    /// <returns>The new stream.</returns>
    /// <remarks>
    /// A stream already filed under the same three names belonged to a run that ended without being
    /// cleaned up, and it is ended here rather than left holding an encoder.
    /// </remarks>
    internal MediaStream Open(string sessionHash, string run, int componentId, string? desiredOutputFormat)
    {
        var outputs = _streams.GetOrAdd(
            sessionHash,
            _ => new ConcurrentDictionary<string, ConcurrentDictionary<int, MediaStream>>(StringComparer.Ordinal))
            .GetOrAdd(run, _ => new ConcurrentDictionary<int, MediaStream>());

        var stream = new MediaStream(desiredOutputFormat);

        if (outputs.TryRemove(componentId, out var stale))
        {
            stale.End();
        }

        outputs[componentId] = stream;

        return stream;
    }

    /// <summary>Ends every stream a run opened, keeping them so that their playlists still answer.</summary>
    /// <param name="sessionHash">The session the run belongs to.</param>
    /// <param name="run">The run's identity.</param>
    /// <remarks>
    /// Ending is not forgetting: a client fetches the playlist after the run's last message, and the
    /// last segment is only added at the end. The stream is dropped when its session is, which is
    /// what <see cref="EndSession"/> is for.
    /// </remarks>
    internal void EndRun(string sessionHash, string run)
    {
        if (!_streams.TryGetValue(sessionHash, out var runs) || !runs.TryGetValue(run, out var outputs))
        {
            return;
        }

        foreach (var stream in outputs.Values)
        {
            stream.End();
        }
    }

    /// <summary>Ends every stream a session opened, keeping them so that their playlists still answer.</summary>
    /// <param name="sessionHash">The session whose connection went away.</param>
    /// <remarks>
    /// A client fetches the playlist after the run's last message and then closes the connection
    /// that carried the messages, so "the connection is gone" cannot mean "the stream is gone": the
    /// one request the client makes next is for the playlist of a run it has just been told about.
    /// What is released is the encoder, which is what ending a stream does.
    /// </remarks>
    internal void EndStreams(string sessionHash)
    {
        if (!_streams.TryGetValue(sessionHash, out var runs))
        {
            return;
        }

        foreach (var outputs in runs.Values)
        {
            foreach (var stream in outputs.Values)
            {
                stream.End();
            }
        }
    }

    /// <summary>Ends and forgets every stream a session opened.</summary>
    /// <param name="sessionHash">The session, or <see langword="null"/> to forget all of them.</param>
    internal void EndSession(string? sessionHash)
    {
        if (sessionHash is null)
        {
            foreach (var key in _streams.Keys)
            {
                EndSession(key);
            }

            return;
        }

        if (!_streams.TryRemove(sessionHash, out var runs))
        {
            return;
        }

        foreach (var outputs in runs.Values)
        {
            foreach (var stream in outputs.Values)
            {
                stream.End();
            }
        }
    }
}

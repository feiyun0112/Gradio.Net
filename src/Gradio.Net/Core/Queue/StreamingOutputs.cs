namespace Gradio.Net;
/// <summary>
/// Turns the chunks a run produced into segments of a stream, and tells the client where the stream
/// is.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>handle_streaming_outputs</c>, which runs between postprocessing and the
/// message that describes the run. An output that streams does not send its value: the value is one
/// chunk of a stream the client plays, the chunk becomes a segment, and what the payload carries is
/// the file object naming that stream's playlist. The client then fetches the playlist and plays it
/// while the rest of the chunks are still being produced.
/// </para>
/// <para>
/// The stream is filed under the session, the run and the output component, which together are what
/// its address names. A run whose handler returns once rather than yielding opens a stream, adds
/// one segment and ends it in the same pass, which is the Python version's answer too: a stream of
/// one segment is still a stream, and a client that was told to play a playlist gets one.
/// </para>
/// </remarks>
internal sealed class StreamingOutputs
{
    /// <summary>How long a streamed run waits for a chunk before taking its next step.</summary>
    /// <remarks>The Python version's default, used when the binding names no time limit of its
    /// own.</remarks>
    private static readonly TimeSpan DefaultWait = TimeSpan.FromSeconds(30);

    private readonly PendingStreams _streams;

    /// <summary>Creates the pass.</summary>
    /// <param name="streams">The streams this application is serving.</param>
    /// <exception cref="ArgumentNullException"><paramref name="streams"/> is <see langword="null"/>.</exception>
    internal StreamingOutputs(PendingStreams streams)
    {
        ArgumentNullException.ThrowIfNull(streams);

        _streams = streams;
    }

    /// <summary>Files the chunks of a run's streaming outputs, and rewrites their values.</summary>
    /// <param name="output">What the run produced.</param>
    /// <param name="dependency">The binding that ran.</param>
    /// <param name="queued">The event the run belongs to.</param>
    /// <returns>The same output, with a stream's file object in place of each chunk.</returns>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    internal async Task<RunOutput> ApplyAsync(RunOutput output, Dependency dependency, QueueEvent queued)
    {
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(dependency);
        ArgumentNullException.ThrowIfNull(queued);

        var final = !output.IsGenerating;
        var outputs = dependency.Outputs;

        for (var index = 0; index < outputs.Count && index < output.Data.Count; index++)
        {
            if (outputs[index] is not IStreamingOutput streaming || !streaming.Streaming)
            {
                continue;
            }

            // An update is not a chunk of the stream. The Python version excludes one from this pass, and
            // without the exclusion the props the handler asked the client to merge would be
            // replaced by the stream's file object: the client would lose the update, and a stream
            // would be opened for a chunk that never existed.
            if (PropertyUpdate.Is(output.Data[index]))
            {
                continue;
            }

            var componentId = outputs[index].Id;
            var stream = _streams.Find(queued.SessionHash, queued.Id, componentId);

            // A final pass over an output that never opened a stream has nothing to finalize: the
            // session may have been dropped, or every chunk before this one may have been an update
            // rather than a value.
            if (final && stream is null)
            {
                continue;
            }

            var first = stream is null;
            var streamId = StreamId(queued, componentId);
            var value = output.StreamValues.TryGetValue(componentId, out var chunk) ? chunk : null;
            var (segment, file) = await streaming
                .StreamOutputAsync(value, streamId, first)
                .ConfigureAwait(false);

            if (first)
            {
                stream = _streams.Open(
                    queued.SessionHash,
                    queued.Id,
                    componentId,
                    DesiredFormatOf(file.OrigName));

                var opened = stream;
                opened.OnEnd(() => streaming.EndStreamOutput(streamId));
            }

            stream!.Add(segment);

            if (final)
            {
                await FinishAsync(streaming, stream, streamId).ConfigureAwait(false);
            }

            output.Data[index] = file.ToJson();
        }

        return output;
    }

    /// <summary>Ends every stream a run opened, keeping them so that their playlists still answer.</summary>
    /// <param name="queued">The event the run belonged to.</param>
    internal void EndRun(QueueEvent queued)
    {
        ArgumentNullException.ThrowIfNull(queued);

        _streams.EndRun(queued.SessionHash, queued.Id);
    }

    /// <summary>How long a streamed run waits for its next chunk.</summary>
    /// <param name="dependency">The binding being run.</param>
    /// <returns>The binding's time limit, or the Python version's default.</returns>
    internal static TimeSpan WaitOf(Dependency dependency)
    {
        ArgumentNullException.ThrowIfNull(dependency);

        return dependency.TimeLimit is { } limit ? TimeSpan.FromSeconds(limit) : DefaultWait;
    }

    /// <summary>The last segment of a stream, and the end of it.</summary>
    /// <param name="streaming">The component that produced the stream.</param>
    /// <param name="stream">The stream.</param>
    /// <param name="streamId">Its identity.</param>
    /// <remarks>
    /// The stream is ended whether or not the flush produced anything: a playlist that never gets
    /// its end marker is one a player polls forever, which is worse than a stream with no last
    /// segment.
    /// </remarks>
    private static async Task FinishAsync(IStreamingOutput streaming, MediaStream stream, string streamId)
    {
        try
        {
            stream.Add(await streaming.FlushStreamOutputAsync(streamId).ConfigureAwait(false));
        }
        finally
        {
            stream.End();
        }
    }

    /// <summary>The address a stream is served at, which is also its identity.</summary>
    /// <param name="queued">The event the run belongs to.</param>
    /// <param name="componentId">The output component.</param>
    /// <returns>The playlist's path, relative to the stream route.</returns>
    private static string StreamId(QueueEvent queued, int componentId) =>
        $"{queued.SessionHash}/{queued.Id}/{componentId}/playlist.m3u8";

    /// <summary>The format the client asked the combined file for, from the name of the stream.</summary>
    /// <param name="origName">The name the component gave the stream's output, or <see langword="null"/>.</param>
    /// <returns>The extension without its dot, or <see langword="null"/> when the name has none.</returns>
    private static string? DesiredFormatOf(string? origName) =>
        origName is { Length: > 0 } && Path.GetExtension(origName) is { Length: > 1 } extension
            ? extension[1..]
            : null;
}

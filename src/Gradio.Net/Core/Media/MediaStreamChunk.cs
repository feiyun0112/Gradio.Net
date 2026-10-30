namespace Gradio.Net;
/// <summary>
/// One piece of a media stream: the bytes of a segment, how long it lasts, and what to call it
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>MediaStreamChunk</c>, which its components produce by encoding what a
/// handler yielded. The duration is the segment's own rather than the stream's: a player places
/// each segment by adding up the ones before it, which is why the playlist writes one
/// <c>#EXTINF</c> line per segment and why a segment that lies about its length makes the whole
/// stream drift.
/// </para>
/// <para>
/// The extension is what the segment is served as — <c>.aac</c> for packed audio, <c>.ts</c> for a
/// video segment — and the segment endpoint refuses anything else, which is the Python version's
/// rule.
/// </para>
/// </remarks>
public sealed class MediaStreamChunk
{
    /// <summary>Creates a chunk.</summary>
    /// <param name="data">The segment's bytes.</param>
    /// <param name="duration">How long the segment lasts, in seconds.</param>
    /// <param name="extension">The file extension it is served as, with the dot.</param>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is negative.</exception>
    /// <exception cref="ArgumentException"><paramref name="extension"/> is not <c>.aac</c> or <c>.ts</c>.</exception>
    public MediaStreamChunk(byte[] data, double duration, string extension)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentOutOfRangeException.ThrowIfNegative(duration);
        ArgumentException.ThrowIfNullOrWhiteSpace(extension);

        if (extension is not (AudioExtension or VideoExtension))
        {
            throw new ArgumentException(
                $"A media stream segment is served as {AudioExtension} or {VideoExtension}; got '{extension}'.",
                nameof(extension));
        }

        Data = data;
        Duration = duration;
        Extension = extension;
    }

    /// <summary>The extension of a packed-audio segment.</summary>
    public const string AudioExtension = ".aac";

    /// <summary>The extension of a video segment.</summary>
    public const string VideoExtension = ".ts";

    /// <summary>The segment's bytes.</summary>
    public byte[] Data { get; }

    /// <summary>How long the segment lasts, in seconds.</summary>
    public double Duration { get; }

    /// <summary>The extension the segment is served as, with the dot.</summary>
    public string Extension { get; }
}

/// <summary>
/// A component whose output can be played while it is still being produced.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>StreamingOutput</c>. A handler that yields chunks from such a component
/// does not send each chunk to the client as a value: the component turns each one into a segment
/// of a stream the client plays, and what the client receives is one file object naming that
/// stream's playlist.
/// </para>
/// <para>
/// The three methods are one lifetime, in order: <see cref="StreamOutputAsync"/> for each chunk
/// (the first one opening the stream), <see cref="FlushStreamOutputAsync"/> once at the end for
/// whatever the component held back, and <see cref="EndStreamOutput"/> when nobody will read the
/// stream again — a normal end, a cancel, or a client that went away.
/// </para>
/// </remarks>
public interface IStreamingOutput
{
    /// <summary>Whether the component is in streaming mode.</summary>
    /// <remarks>
    /// A component that is not streaming sends each chunk as an ordinary value, which is what the
    /// <c>streaming</c> parameter of the component decides and what binding its <c>stream</c> event
    /// turns on.
    /// </remarks>
    bool Streaming { get; }

    /// <summary>Turns one chunk of a handler's output into a segment.</summary>
    /// <param name="value">The chunk the handler yielded.</param>
    /// <param name="streamId">The stream's identity, which is the path of its playlist.</param>
    /// <param name="firstChunk">Whether this is the chunk that opens the stream.</param>
    /// <returns>The segment, or <see langword="null"/> for a chunk that produces none, and the file object the client is told about.</returns>
    Task<(MediaStreamChunk? Chunk, FileData File)> StreamOutputAsync(
        object? value,
        string streamId,
        bool firstChunk);

    /// <summary>Produces the last segment of a stream, or <see langword="null"/> when there is none.</summary>
    /// <param name="streamId">The stream's identity.</param>
    /// <returns>The segment, or <see langword="null"/>.</returns>
    Task<MediaStreamChunk?> FlushStreamOutputAsync(string streamId);

    /// <summary>Releases whatever the component holds for a stream.</summary>
    /// <param name="streamId">The stream's identity.</param>
    void EndStreamOutput(string streamId);
}

namespace Gradio.Net;
/// <summary>
/// What a video component does with the chunks a handler yields when it is streaming.
/// </summary>
/// <remarks>
/// <para>
/// The Python version muxes each chunk into an MPEG-TS segment with ffmpeg, encodes the audio into
/// AAC and keeps the two tracks on one timeline. This framework does not run external processes, so
/// a segment is what the handler produced: a <see cref="MediaStreamChunk"/> is taken as it stands —
/// its bytes, its duration and its extension — and anything else is read as MPEG-TS bytes with no
/// duration to declare, which the playlist then writes as <c>#EXTINF:0.000000</c>.
/// </para>
/// <para>
/// That is the honest half of the feature: the playlist, the segments and the combined file are
/// served by the Python version's rules, and what is missing is the encoder. An application that
/// already produces AAC or MPEG-TS — or that measures its own chunks — gets a stream that plays;
/// one that hands over an MP4 chunk is told why it cannot.
/// </para>
/// </remarks>
public partial class Video
{
    /// <inheritdoc />
    /// <remarks>
    /// The file the client is told about names the stream's playlist rather than a file, which is
    /// how the frontend knows to play it as it arrives: the Python version builds the same path out
    /// of the session, the run and the component's id, and the playlist route parses it back.
    /// </remarks>
    public Task<(MediaStreamChunk? Chunk, FileData File)> StreamOutputAsync(
        object? value,
        string streamId,
        bool firstChunk)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(streamId);

        var file = new FileData
        {
            Path = streamId,
            Url = "/gradio_api/stream/" + streamId,
            OrigName = "video-stream.mp4",
            IsStream = true,
        };

        if (value is null)
        {
            return Task.FromResult<(MediaStreamChunk?, FileData)>((null, file));
        }

        if (value is MediaStreamChunk chunk)
        {
            return Task.FromResult<(MediaStreamChunk?, FileData)>((chunk, file));
        }

        switch (value)
        {
            case string:
            case FileData:
            case byte[]:
            case System.IO.Stream:
                var bytes = StreamBytes.Read(value, "video");
                if (!LooksLikeTransportStream(bytes))
                {
                    throw new NotSupportedException(
                        "A streamed video chunk must be MPEG-TS or a MediaStreamChunk. The Python version "
                        + "converts each chunk with ffmpeg; this framework does not run external "
                        + "processes, so a chunk that is not already in the format the playlist "
                        + "serves cannot be muxed here.");
                }

                return Task.FromResult<(MediaStreamChunk?, FileData)>(
                    (new MediaStreamChunk(bytes, 0, MediaStreamChunk.VideoExtension), file));
        }

        throw new NotSupportedException(
            "A streaming video takes a path, bytes, a stream, or a MediaStreamChunk; it was handed a "
            + $"{value.GetType()}.");
    }

    /// <inheritdoc />
    /// <remarks>Nothing is held back: a segment is what the handler produced, whole.</remarks>
    public Task<MediaStreamChunk?> FlushStreamOutputAsync(string streamId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(streamId);

        return Task.FromResult<MediaStreamChunk?>(null);
    }

    /// <inheritdoc />
    /// <remarks>Nothing is held: there is no encoder to release.</remarks>
    public void EndStreamOutput(string streamId) => ArgumentException.ThrowIfNullOrWhiteSpace(streamId);

    /// <summary>Whether bytes begin the way an MPEG-TS packet stream begins.</summary>
    /// <param name="bytes">The bytes.</param>
    /// <returns>Whether the first byte is the packet stream's sync byte.</returns>
    /// <remarks>
    /// A transport stream is a run of 188-byte packets, each of which starts with <c>0x47</c>. The
    /// check is the sync byte alone, which is what the Python version's own continuity-counter walk
    /// looks at before it trusts a segment to be one.
    /// </remarks>
    private static bool LooksLikeTransportStream(byte[] bytes) => bytes.Length > 0 && bytes[0] == 0x47;
}

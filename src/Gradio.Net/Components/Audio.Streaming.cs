namespace Gradio.Net;
/// <summary>
/// What an audio component does with the chunks a handler yields when it is streaming.
/// </summary>
/// <remarks>
/// <para>
/// The Python version encodes each chunk to AAC with ffmpeg and packs it into one segment. This
/// framework does not run external processes, so the segment is the chunk: a
/// <see cref="MediaStreamChunk"/> is taken as it stands, and a run of ADTS frames has its duration
/// read out of the frames themselves — which is exact, and is the one container this can measure
/// without ffprobe. Bytes of another format are served as they are with no duration to declare, and
/// the playlist writes them as <c>#EXTINF:0.000000</c>.
/// </para>
/// <para>
/// The playlist, the segments and the combined file are served by the Python version's rules either
/// way; what is missing is the encoder, and an application that already has AAC gets a stream that
/// plays.
/// </para>
/// </remarks>
public partial class Audio
{
    /// <inheritdoc />
    /// <remarks>
    /// The file the client is told about names the stream's playlist rather than a file, which is
    /// how the frontend knows to play it as it arrives. The Python version names the whole stream
    /// after the format it would combine into, which is <c>audio-stream.mp3</c> whatever the
    /// segments are.
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
            OrigName = "audio-stream.mp3",
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
                var bytes = StreamBytes.Read(value, "audio");
                var duration = AacFrames.Duration(bytes) ?? 0;

                return Task.FromResult<(MediaStreamChunk?, FileData)>(
                    (new MediaStreamChunk(bytes, duration, MediaStreamChunk.AudioExtension), file));
        }

        throw new NotSupportedException(
            "A streaming audio component takes a path, bytes, a stream, or a MediaStreamChunk; it "
            + $"was handed a {value.GetType()}.");
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
}

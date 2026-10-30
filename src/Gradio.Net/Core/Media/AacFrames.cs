namespace Gradio.Net;
/// <summary>
/// How long a run of AAC frames lasts, read out of the frames themselves.
/// </summary>
/// <remarks>
/// <para>
/// The Python version asks ffprobe. This framework does not run external processes, and the
/// playlist cannot be written without a duration — a player places each segment by adding up the
/// ones before it — so the one container a streamed audio chunk is served as is read here instead.
/// </para>
/// <para>
/// An ADTS stream is a run of frames, each with a 7- or 9-byte header that says how long the frame
/// is and what sample rate it uses. Every frame carries 1024 samples, so the duration of the run is
/// its frame count times 1024, divided by the rate. Bytes that are not a run of ADTS frames have no
/// duration this can read, which is the caller's answer rather than an error: the bytes are still
/// worth serving.
/// </para>
/// </remarks>
internal static class AacFrames
{
    /// <summary>The samples one frame carries.</summary>
    private const int SamplesPerFrame = 1024;

    /// <summary>The sample rates an ADTS header can name, by index.</summary>
    private static readonly int[] _sampleRates =
    [
        96000, 88200, 64000, 48000, 44100, 32000, 24000, 22050, 16000, 12000, 11025, 8000, 7350,
    ];

    /// <summary>How long a run of ADTS frames lasts.</summary>
    /// <param name="data">The bytes.</param>
    /// <returns>
    /// The duration in seconds, or <see langword="null"/> when the bytes are not a run of ADTS
    /// frames this can read.
    /// </returns>
    internal static double? Duration(ReadOnlySpan<byte> data)
    {
        if (data.Length < 7 || data[0] != 0xFF || (data[1] & 0xF0) != 0xF0)
        {
            return null;
        }

        var rate = _sampleRates[(data[2] >> 2) & 0x0F];
        var frames = 0;
        var offset = 0;

        while (offset + 7 <= data.Length)
        {
            if (data[offset] != 0xFF || (data[offset + 1] & 0xF0) != 0xF0)
            {
                return null;
            }

            var length = ((data[offset + 3] & 0x03) << 11)
                | (data[offset + 4] << 3)
                | (data[offset + 5] >> 5);

            if (length < 7 || offset + length > data.Length)
            {
                // A truncated last frame is not a reason to throw the whole run away: what is there
                // is what plays, and the duration of the frames that are whole is the honest answer.
                break;
            }

            frames++;
            offset += length;
        }

        return frames == 0 ? null : (double)frames * SamplesPerFrame / rate;
    }
}

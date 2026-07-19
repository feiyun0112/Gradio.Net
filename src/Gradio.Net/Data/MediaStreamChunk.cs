
namespace Gradio.Net.Data;

public class MediaStreamChunk
{
    public byte[] Data { get; set; } = Array.Empty<byte>();

    public float Duration { get; set; }

    public string Extension { get; set; } = string.Empty;

    public string? Id { get; set; }

    public string? Path { get; set; }

    public string? Url { get; set; }

    public string? Mime { get; set; }

    public string? OrigName { get; set; }
}

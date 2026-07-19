
namespace Gradio.Net.Components;

public interface IStreamingOutput
{
    Task<Dictionary<string, object>> StreamOutput(object value, string outputId, bool firstChunk);
    Task<object> CombineStream(List<byte[]> stream, string? desiredOutputFormat = null, bool onlyFile = false);
}

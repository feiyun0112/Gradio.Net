
namespace Gradio.Net.Components
{
    public class DataframeData
    {
        public List<object> Headers { get; set; } = new();
        public List<List<object?>> Data { get; set; } = new();
        public Dictionary<string, object>? Metadata { get; set; }
    }
}

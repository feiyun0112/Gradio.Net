using Gradio.Net.Data;

namespace Gradio.Net.Components
{
    public class Annotation
    {
        public FileData Image { get; set; }
        public string Label { get; set; }
    }
}

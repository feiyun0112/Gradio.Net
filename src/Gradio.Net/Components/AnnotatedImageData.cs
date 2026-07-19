using Gradio.Net.Data;

namespace Gradio.Net.Components
{
    public class AnnotatedImageData
    {
        public FileData Image { get; set; }
        public List<Annotation> Annotations { get; set; } = new List<Annotation>();
    }
}

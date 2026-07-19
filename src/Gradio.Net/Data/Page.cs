
namespace Gradio.Net.Data;

public class Page
{
    public List<int> Components { get; set; } = new List<int>();

    public List<int> Dependencies { get; set; } = new List<int>();

    public Layout Layout { get; set; } = new Layout();
}

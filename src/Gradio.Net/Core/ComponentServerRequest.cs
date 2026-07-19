
namespace Gradio.Net.Core;

public class ComponentServerRequest
{
    public Dictionary<string, object> Data { get; set; }
    public List<Tuple<string, byte[]>> Files { get; set; }
    public int ComponentId { get; set; }
    public string SessionHash { get; set; }
    public string FnName { get; set; }
}

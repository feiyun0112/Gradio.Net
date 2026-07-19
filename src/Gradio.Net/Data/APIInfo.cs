
namespace Gradio.Net.Data;

public class APIInfo
{
    public Dictionary<string, APIEndpointInfo> NamedEndpoints { get; set; } = new Dictionary<string, APIEndpointInfo>();

    public Dictionary<string, APIEndpointInfo> UnnamedEndpoints { get; set; } = new Dictionary<string, APIEndpointInfo>();
}

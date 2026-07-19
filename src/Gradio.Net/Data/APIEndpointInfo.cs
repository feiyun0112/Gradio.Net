
namespace Gradio.Net.Data;

public class APIEndpointInfo
{
    public string? Description { get; set; }

    public List<object> Parameters { get; set; } = new List<object>();

    public List<APIReturnInfo> Returns { get; set; } = new List<APIReturnInfo>();

    public string ApiVisibility { get; set; } = string.Empty;
}

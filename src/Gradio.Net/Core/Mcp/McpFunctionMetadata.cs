
namespace Gradio.Net.Core.Mcp;

public class McpFunctionMetadata
{
    public string McpType { get; set; }
    public string? McpName { get; set; }
    public string? McpDescription { get; set; }
    public string? McpUriTemplate { get; set; }
    public string? McpMimeType { get; set; }
    public bool McpStructuredOutput { get; set; }
    public Dictionary<string, object>? McpMeta { get; set; }
}

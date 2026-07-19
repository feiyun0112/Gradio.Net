
namespace Gradio.Net.Core.Mcp;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class ResourceAttribute : Attribute
{
    public string UriTemplate { get; }
    public string? Description { get; }
    public string MimeType { get; }

    public ResourceAttribute(string uriTemplate, string? description = null, string? mimeType = null)
    {
        UriTemplate = uriTemplate;
        Description = description;
        MimeType = mimeType ?? "text/plain";
    }
}

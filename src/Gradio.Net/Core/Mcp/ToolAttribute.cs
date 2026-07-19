
namespace Gradio.Net.Core.Mcp;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class ToolAttribute : Attribute
{
    public string? Name { get; }
    public string? Description { get; }
    public bool StructuredOutput { get; }
    public Dictionary<string, object>? Meta { get; }

    public ToolAttribute(string? name = null, string? description = null, bool structuredOutput = false, Dictionary<string, object>? meta = null)
    {
        Name = name;
        Description = description;
        StructuredOutput = structuredOutput;
        Meta = meta;
    }
}

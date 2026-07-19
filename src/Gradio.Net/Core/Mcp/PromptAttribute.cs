
namespace Gradio.Net.Core.Mcp;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class PromptAttribute : Attribute
{
    public string Name { get; }
    public string? Description { get; }

    public PromptAttribute(string? name = null, string? description = null)
    {
        Name = name ?? string.Empty;
        Description = description;
    }
}


namespace Gradio.Net.Core.Mcp;

public static class McpDecorators
{
    public static McpFunctionMetadata? GetMcpMetadata(Delegate function)
    {
        var methodInfo = function.Method;
        return GetMcpMetadata(methodInfo);
    }

    public static McpFunctionMetadata? GetMcpMetadata(System.Reflection.MethodInfo methodInfo)
    {
        var resourceAttr = methodInfo.GetCustomAttributes(typeof(ResourceAttribute), false)
            .FirstOrDefault() as ResourceAttribute;

        if (resourceAttr != null)
        {
            return new McpFunctionMetadata
            {
                McpType = "resource",
                McpName = methodInfo.Name,
                McpDescription = resourceAttr.Description,
                McpUriTemplate = resourceAttr.UriTemplate,
                McpMimeType = resourceAttr.MimeType
            };
        }

        var promptAttr = methodInfo.GetCustomAttributes(typeof(PromptAttribute), false)
            .FirstOrDefault() as PromptAttribute;

        if (promptAttr != null)
        {
            return new McpFunctionMetadata
            {
                McpType = "prompt",
                McpName = string.IsNullOrEmpty(promptAttr.Name) ? methodInfo.Name : promptAttr.Name,
                McpDescription = promptAttr.Description
            };
        }

        var toolAttr = methodInfo.GetCustomAttributes(typeof(ToolAttribute), false)
            .FirstOrDefault() as ToolAttribute;

        if (toolAttr != null)
        {
            return new McpFunctionMetadata
            {
                McpType = "tool",
                McpName = toolAttr.Name ?? methodInfo.Name,
                McpDescription = toolAttr.Description,
                McpStructuredOutput = toolAttr.StructuredOutput,
                McpMeta = toolAttr.Meta
            };
        }

        return null;
    }

    public static Func<T, TResult> Resource<T, TResult>(
        string uriTemplate,
        string? description = null,
        string? mimeType = null
    )
    {
        return (Func<T, TResult>)((Delegate)(object)(Func<object, object>)((input) =>
        {
            throw new NotImplementedException("Resource decorator wrapper not implemented. Use ResourceAttribute instead.");
        }));
    }

    public static Func<T, TResult> Prompt<T, TResult>(
        string? name = null,
        string? description = null
    )
    {
        return (Func<T, TResult>)((Delegate)(object)(Func<object, object>)((input) =>
        {
            throw new NotImplementedException("Prompt decorator wrapper not implemented. Use PromptAttribute instead.");
        }));
    }

    public static Func<T, TResult> Tool<T, TResult>(
        string? name = null,
        string? description = null,
        bool structuredOutput = false,
        Dictionary<string, object>? meta = null
    )
    {
        return (Func<T, TResult>)((Delegate)(object)(Func<object, object>)((input) =>
        {
            throw new NotImplementedException("Tool decorator wrapper not implemented. Use ToolAttribute instead.");
        }));
    }
}

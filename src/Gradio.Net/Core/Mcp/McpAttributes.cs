namespace Gradio.Net;
/// <summary>
/// What a handler is to an agent: a tool it can call, a resource it can read, or a prompt it can
/// fill in.
/// </summary>
/// <remarks>
/// The Python version spells this as a private attribute its decorators set on the function —
/// <c>_mcp_type</c> — and a function with none is a tool. The same default applies here: an
/// endpoint derived from a handler is a tool until the application says otherwise, because that is
/// what an endpoint is.
/// </remarks>
public enum McpKind
{
    /// <summary>Something an agent calls with arguments and gets an answer from.</summary>
    Tool,

    /// <summary>Something an agent reads by address.</summary>
    Resource,

    /// <summary>Something an agent asks to be filled in.</summary>
    Prompt,
}

/// <summary>
/// Marks a handler as a tool an agent may call, and says how it is described.
/// </summary>
/// <remarks>
/// <para>
/// The default already is a tool, so this attribute is only needed to say something the handler's
/// signature cannot: a name other than the method's, a description other than the endpoint's, or
/// that the answer should also be reported as structured content.
/// </para>
/// <para>
/// The Python version's <c>gr.tool</c> also takes a <c>_meta</c> dictionary, which the MCP SDK
/// passes through to a client. An attribute cannot carry a dictionary, so it is not offered here;
/// the field is absent from the descriptor rather than silently dropped from one that was set.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Method)]
public sealed class McpToolAttribute : Attribute
{
    /// <summary>The name the tool is listed under, or <see langword="null"/> for the handler's.</summary>
    public string? Name { get; set; }

    /// <summary>The description the tool is listed with, or <see langword="null"/> for the endpoint's.</summary>
    public string? Description { get; set; }

    /// <summary>
    /// Whether the answer is also reported as structured content — the Python version's
    /// <c>structured_output</c>, which wraps the result in an object under <c>result</c>.
    /// </summary>
    public bool StructuredOutput { get; set; }
}

/// <summary>
/// Marks a handler as a resource an agent may read.
/// </summary>
/// <remarks>
/// <para>
/// The address is a template: a segment written <c>{name}</c> is filled from the handler's
/// parameter of that name. A template with no such segment is a resource, and one with a segment is
/// a resource template — which is the Python version's split, and the reason the two are listed
/// separately.
/// </para>
/// <para>
/// The Python version's <c>resource</c> decorator takes no name, and lists the resource under the
/// handler's — but its <c>tool</c> and <c>prompt</c> decorators do take one, so the three are given
/// the same shape here rather than leaving the odd one out. A name is also the only way a .NET
/// handler can be listed under the name its Python counterpart has.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Method)]
public sealed class McpResourceAttribute : Attribute
{
    /// <summary>Creates the attribute.</summary>
    /// <param name="uriTemplate">The address, or the address pattern, the resource is read at.</param>
    /// <exception cref="ArgumentNullException"><paramref name="uriTemplate"/> is <see langword="null"/>.</exception>
    public McpResourceAttribute(string uriTemplate)
    {
        ArgumentNullException.ThrowIfNull(uriTemplate);

        UriTemplate = uriTemplate;
    }

    /// <summary>The address, or the address pattern, the resource is read at.</summary>
    public string UriTemplate { get; }

    /// <summary>The name the resource is listed under, or <see langword="null"/> for the handler's.</summary>
    public string? Name { get; set; }

    /// <summary>The description the resource is listed with, or <see langword="null"/> for the endpoint's.</summary>
    public string? Description { get; set; }

    /// <summary>The media type of what the resource returns, or <see langword="null"/> for text.</summary>
    public string? MimeType { get; set; }
}

/// <summary>
/// Marks a handler as a prompt an agent may ask to be filled in.
/// </summary>
/// <remarks>
/// A prompt is called like a tool — with arguments, one per parameter — and its answer becomes the
/// message the agent is handed. What distinguishes it is who reads the answer: a prompt's text is
/// for a model, and a tool's is for a caller.
/// </remarks>
[AttributeUsage(AttributeTargets.Method)]
public sealed class McpPromptAttribute : Attribute
{
    /// <summary>The name the prompt is listed under, or <see langword="null"/> for the handler's.</summary>
    public string? Name { get; set; }

    /// <summary>The description the prompt is listed with, or <see langword="null"/> for the endpoint's.</summary>
    public string? Description { get; set; }
}

using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// One call of a block's own server function, as the browser makes it.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>component_server</c> request body. It arrives in one of two shapes, and
/// both end up here: a JSON body carrying the function's one argument, or a multipart body carrying
/// that argument as a form field beside the files it refers to — which is how an image editor
/// uploads the large images it works on without putting their bytes in the request's JSON.
/// </para>
/// <para>
/// The files are the bytes the browser sent rather than paths on disk: a server function is the
/// component's own behaviour, and what it does with them is its business.
/// </para>
/// </remarks>
public sealed class ComponentServerCall
{
    /// <summary>The function's name, as the block declares it.</summary>
    public string FunctionName { get; init; } = string.Empty;

    /// <summary>The function's argument, or <see langword="null"/> when it was sent none.</summary>
    public JsonNode? Data { get; init; }

    /// <summary>The files the browser sent with the call, in the order they arrived.</summary>
    public IReadOnlyList<ComponentServerFile> Files { get; init; } = [];
}

/// <summary>One file sent with a component server call.</summary>
/// <param name="Name">The name the browser gave it.</param>
/// <param name="Bytes">Its bytes.</param>
public sealed record ComponentServerFile(string Name, byte[] Bytes);

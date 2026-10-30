using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A table of parameters with their types, descriptions and defaults, for documenting an API
/// </summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. The value is a map from a parameter's name to its type,
/// description and default, and the frontend draws it as a table with the descriptions linked and
/// the types highlighted.
/// </para>
/// <para>
/// The component is an output in practice, and it is the one component whose signature carries no
/// label, no container and no width: the Python version declares it that way, so the config has
/// none of those keys.
/// </para>
/// </remarks>
[GradioBlock("paramviewer")]
[HandlerValue("object?", "change", "upload")]
public sealed partial class ParamViewer : Component
{
    internal ParamViewer()
    {
    }

    /// <summary>The languages the types may be highlighted as.</summary>

    private string _language = "python";

    /// <summary>The parameters, as a map from a name to its type, description and default.</summary>
    public object? Value { get; set; }

    /// <summary>The language the types are highlighted as.</summary>
    /// <value><c>python</c> or <c>typescript</c>; anything else is refused.</value>
    public string Language
    {
        get => _language;
        set => _language = value;
    }

    /// <summary>The words in a description that are rendered as links.</summary>
    public IReadOnlyList<string>? Linkify { get; set; }

    /// <summary>The heading above the table.</summary>
    /// <value>
    /// <c>Parameters</c> unless the application says otherwise; <see langword="null"/> hides the
    /// heading and the button that opens every row at once.
    /// </value>
    public string? Header { get; set; } = "Parameters";

    /// <summary>Whether each parameter gets an anchor link, or the prefix those links use.</summary>
    /// <value>
    /// <see langword="false"/>, <see langword="true"/>, or a string used as the links' prefix so that
    /// two viewers on one page do not collide.
    /// </value>
    public object? AnchorLinks { get; set; } = false;

    /// <summary>The greatest height of the table, in pixels or as a CSS length, or <see langword="null"/> to fit.</summary>
    public object? MaxHeight { get; set; }

    /// <inheritdoc />
    public override JsonObject ApiInfo => new()
    {
        ["type"] = new JsonObject(),
        ["description"] = "any valid json",
    };

    /// <inheritdoc />
    public override JsonNode? ExampleInputs => new JsonObject
    {
        ["array"] = new JsonObject
        {
            ["type"] = "numpy",
            ["description"] = "any valid json",
            ["default"] = "None",
        },
    };

    /// <summary>The parameters a handler reads.</summary>
    /// <param name="value">What the client sent.</param>
    /// <returns>The map, as CLR values.</returns>
    public override object? Preprocess(JsonNode? value) => JsonValues.ToObject(value);

    /// <summary>The parameters the client receives from what a handler returned.</summary>
    /// <param name="value">The map.</param>
    /// <returns>The same map, as JSON.</returns>
    public override JsonNode? Postprocess(object? value) => JsonValues.FromAnyObject(value);
}

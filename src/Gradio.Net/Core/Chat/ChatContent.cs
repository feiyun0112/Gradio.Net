using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// One block of a chat message's content: text, a file, or a component
/// </summary>
/// <remarks>
/// <para>
/// A message's content is a list of these rather than a string. That is the gradio 6 model: a
/// message can hold a paragraph and a chart, or two files, and the frontend renders each block with
/// the renderer that block names. The type marker is what it dispatches on, which is why every
/// block carries one and why the marker is part of the wire shape rather than of this class.
/// </para>
/// <para>
/// The three blocks are the three the Python version has. A block this framework does not know is
/// read as a component block rather than refused, because a client that sends one is describing
/// something the frontend may well be able to render.
/// </para>
/// </remarks>
public abstract class ChatContent
{
    /// <summary>A block of text.</summary>
    /// <param name="text">The text.</param>
    /// <returns>The block.</returns>
    public static ChatContent TextBlock(string text) => new TextContent(text);

    /// <summary>A block holding a file.</summary>
    /// <param name="file">The file.</param>
    /// <param name="altText">The alternative text a screen reader reads, or <see langword="null"/>.</param>
    /// <returns>The block.</returns>
    public static ChatContent FileBlock(FileData file, string? altText = null) => new FileContent(file, altText);

    /// <summary>A block holding a component, rendered as that component rather than as text.</summary>
    /// <param name="component">The component's name, as the config spells it.</param>
    /// <param name="value">The value it carries.</param>
    /// <param name="constructorArgs">The arguments it was built with.</param>
    /// <param name="props">Its props.</param>
    /// <returns>The block.</returns>
    public static ChatContent ComponentBlock(
        string component,
        JsonNode? value,
        JsonObject? constructorArgs = null,
        JsonObject? props = null) =>
        new ComponentContent(component, value, constructorArgs, props);

    /// <summary>The block as the protocol carries it.</summary>
    /// <returns>A new node; the caller may keep or mutate it.</returns>
    internal abstract JsonObject ToJson();

    /// <summary>Reads a block a client sent.</summary>
    /// <param name="value">The block, as the protocol carried it.</param>
    /// <returns>The block.</returns>
    /// <exception cref="ArgumentException"><paramref name="value"/> is not a block.</exception>
    /// <remarks>
    /// A bare string is read as text because that is one of the shapes a client may send — the
    /// field accepts a string where a list of blocks is expected — and an object naming a path is
    /// read as a file, which is the shape the Python version's older clients produce.
    /// </remarks>
    internal static ChatContent FromJson(JsonNode? value)
    {
        if (value is JsonValue scalar && scalar.TryGetValue<string>(out var text))
        {
            return new TextContent(text);
        }

        if (value is not JsonObject document)
        {
            throw new ArgumentException(
                $"A chat message's content must be a string, a block or a file object; got '{value?.ToJsonString() ?? "null"}'.",
                nameof(value));
        }

        switch (JsonValues.Text(document["type"]))
        {
            case "text":
                return new TextContent(JsonValues.Text(document["text"]) ?? string.Empty);

            case "file":
                return new FileContent(
                    FileData.FromJson(document["file"]) ?? new FileData(),
                    JsonValues.Text(document["alt_text"]));

            case "component":
                return new ComponentContent(
                    JsonValues.Text(document["component"]) ?? string.Empty,
                    document["value"]?.DeepClone(),
                    document["constructor_args"] as JsonObject,
                    document["props"] as JsonObject);
        }

        // An object that names a file without saying it is one is what a client that read an
        // older answer sends; it is a file block with the marker left off.
        if (FileData.FromJson(document) is { Path: not null } file)
        {
            return new FileContent(file, JsonValues.Text(document["alt_text"]));
        }

        throw new ArgumentException(
            $"A chat message's content block must name its type; got '{document.ToJsonString()}'.",
            nameof(value));
    }
}

/// <summary>A block of text, which is what most of a conversation is made of.</summary>
public sealed class TextContent : ChatContent
{
    /// <summary>Creates a block of text.</summary>
    /// <param name="text">The text.</param>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
    public TextContent(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        Text = text;
    }

    /// <summary>The text, as the message carries it.</summary>
    public string Text { get; }

    /// <inheritdoc />
    internal override JsonObject ToJson() => new()
    {
        ["text"] = Text,
        ["type"] = "text",
    };
}

/// <summary>A block holding a file, which the frontend renders by its media type.</summary>
public sealed class FileContent : ChatContent
{
    /// <summary>Creates a block holding a file.</summary>
    /// <param name="file">The file.</param>
    /// <param name="altText">The alternative text a screen reader reads, or <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="file"/> is <see langword="null"/>.</exception>
    public FileContent(FileData file, string? altText = null)
    {
        ArgumentNullException.ThrowIfNull(file);

        File = file;
        AltText = altText;
    }

    /// <summary>The file.</summary>
    public FileData File { get; }

    /// <summary>The alternative text, or <see langword="null"/> for none.</summary>
    public string? AltText { get; }

    /// <inheritdoc />
    internal override JsonObject ToJson() => new()
    {
        ["file"] = File.ToJson(),
        ["alt_text"] = AltText,
        ["type"] = "file",
    };
}

/// <summary>
/// A block holding a component, which the frontend renders with that component's own renderer.
/// </summary>
/// <remarks>
/// The props travel with the message because the frontend builds the component from them: a
/// message that carried only a value would render with the default props rather than the ones the
/// handler asked for.
/// </remarks>
public sealed class ComponentContent : ChatContent
{
    /// <summary>Creates a block holding a component.</summary>
    /// <param name="component">The component's name, as the config spells it.</param>
    /// <param name="value">The value it carries.</param>
    /// <param name="constructorArgs">The arguments it was built with, or <see langword="null"/> for none.</param>
    /// <param name="props">Its props, or <see langword="null"/> for none.</param>
    /// <exception cref="ArgumentNullException"><paramref name="component"/> is <see langword="null"/>.</exception>
    public ComponentContent(
        string component,
        JsonNode? value,
        JsonObject? constructorArgs = null,
        JsonObject? props = null)
    {
        ArgumentNullException.ThrowIfNull(component);

        Component = component;
        Value = value;
        ConstructorArgs = constructorArgs ?? [];
        Props = props ?? [];
    }

    /// <summary>The component's name, as the config spells it.</summary>
    public string Component { get; }

    /// <summary>The value the component carries.</summary>
    public JsonNode? Value { get; }

    /// <summary>The arguments the component was built with.</summary>
    public JsonObject ConstructorArgs { get; }

    /// <summary>The component's props.</summary>
    public JsonObject Props { get; }

    /// <inheritdoc />
    internal override JsonObject ToJson() => new()
    {
        ["component"] = Component,
        ["value"] = Value?.DeepClone(),
        ["constructor_args"] = ConstructorArgs.DeepClone(),
        ["props"] = Props.DeepClone(),
        ["type"] = "component",
    };
}

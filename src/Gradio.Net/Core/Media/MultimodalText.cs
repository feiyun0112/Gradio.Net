using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// What a multimodal textbox carries: the text the user typed and the files they sent with it
/// </summary>
/// <remarks>
/// <para>
/// The Python version's value is a dictionary of two keys, and the config carries it in that shape
/// because the frontend reads both halves: the text it draws in the field and the files it lists
/// beside it. A handler reads the same text and the same files, each file carrying the path the
/// browser sent; the Python version hands the handler the bare paths, and the type here keeps the
/// name and size beside the path because a handler that wants them should not have to go back to
/// the disk for them.
/// </para>
/// <para>
/// The two are always present in the config, even when the field is empty — the Python version's
/// post-processing answers an empty text and no files for an absent value rather than an absent
/// key, and the frontend distinguishes "an empty field" from "no field".
/// </para>
/// </remarks>
public sealed class MultimodalText : IConfigValue
{
    /// <summary>Declares a value that is text and no files.</summary>
    /// <param name="text">The text.</param>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
    public MultimodalText(string text)
        : this(text, null)
    {
    }

    /// <summary>Declares a value with files.</summary>
    /// <param name="text">The text.</param>
    /// <param name="files">The files, or <see langword="null"/> for none.</param>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
    public MultimodalText(string text, IReadOnlyList<FileData>? files)
    {
        ArgumentNullException.ThrowIfNull(text);

        Text = text;
        Files = files ?? [];
    }

    /// <summary>The text the field holds.</summary>
    public string Text { get; }

    /// <summary>The files sent with the text.</summary>
    public IReadOnlyList<FileData> Files { get; }

    /// <summary>The value as the config carries it.</summary>
    /// <returns>A new object with the text and the files.</returns>
    public JsonNode ToConfigValue()
    {
        var files = new JsonArray();

        foreach (var file in Files)
        {
            files.Add(file.ToJson());
        }

        return new JsonObject
        {
            ["text"] = Text,
            ["files"] = files,
        };
    }

    /// <summary>Reads a value a client sent.</summary>
    /// <param name="value">The object the client sent, or <see langword="null"/>.</param>
    /// <returns>The value, or <see langword="null"/> when nothing was sent.</returns>
    /// <exception cref="ArgumentException"><paramref name="value"/> is neither an object nor absent.</exception>
    /// <remarks>
    /// A client that sends no files sends no <c>files</c> key at all — the model's one required
    /// field is the text — so an absent list is read as no files rather than as a malformed value.
    /// </remarks>
    public static MultimodalText? FromJson(JsonNode? value)
    {
        if (value is null)
        {
            return null;
        }

        if (value is not JsonObject document)
        {
            throw new ArgumentException(
                $"A multimodal value is an object with a text and files; got '{value.ToJsonString()}'.",
                nameof(value));
        }

        var text = JsonValues.Text(document["text"]) ?? string.Empty;
        var files = new List<FileData>();

        if (document["files"] is JsonArray list)
        {
            foreach (var entry in list)
            {
                var file = FileData.FromJson(entry);
                if (file is not null)
                {
                    files.Add(file);
                }
            }
        }

        return new MultimodalText(text, files);
    }
}

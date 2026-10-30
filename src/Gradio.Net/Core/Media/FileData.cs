using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// A file as the protocol carries it: where the server keeps it, what to call it, and how a browser
/// reaches it.
/// </summary>
/// <remarks>
/// <para>
/// This is the Python version's <c>FileData</c> pydantic model, and the shape matters because it is
/// what a component sends to the client and receives back from it: the field names, the nulls that
/// are written rather than omitted, and the <c>meta</c> marker that tells the frontend the object
/// is a file and not an arbitrary dictionary. A client that reads <c>path</c> and ignores the rest
/// — the <c>gradio_client</c> packages do — is unaffected by the other fields; a client that
/// round-trips the object needs all of them.
/// </para>
/// <para>
/// <see cref="Url"/> is the address a browser fetches the file from. The Python version spells it
/// <c>/gradio_api/file=&lt;path&gt;</c> and leaves it relative: the frontend resolves it against
/// the address the application is served from, which is why the value carries the same spelling
/// here.
/// </para>
/// </remarks>
public sealed class FileData : GradioModel, IConfigValue
{
    /// <summary>The marker that says an object is a file, which every file's <c>meta</c> carries.</summary>
    public const string FileMetaType = "gradio.FileData";

    /// <summary>Where the server keeps the file.</summary>
    /// <value>
    /// A local path, or an http(s) URL for a file that was never downloaded — a component's value
    /// that points somewhere else, which the browser fetches itself.
    /// </value>
    public string? Path { get; set; }

    /// <summary>The address a browser fetches the file from.</summary>
    public string? Url { get; set; }

    /// <summary>The file's size in bytes, when it is known.</summary>
    public long? Size { get; set; }

    /// <summary>The name the file had before it was uploaded or cached.</summary>
    public string? OrigName { get; set; }

    /// <summary>The file's media type, when it is known.</summary>
    /// <remarks>
    /// The Python version leaves this unset for a file it cached and lets the serving endpoint
    /// guess from the extension, which is why the components here leave it unset too.
    /// </remarks>
    public string? MimeType { get; set; }

    /// <summary>Whether the file is a stream of chunks rather than a whole file.</summary>
    /// <remarks>
    /// Always <see langword="false"/> here: streaming media is its own batch (ticket 16). The field
    /// is written rather than omitted because the frontend reads it.
    /// </remarks>
    public bool IsStream { get; set; }

    /// <inheritdoc />
    public JsonNode ToConfigValue() => ToJson();

    /// <summary>The file as the protocol carries it.</summary>
    /// <returns>
    /// Every field, in the Python version's order, with the nulls written: a client that
    /// round-trips the object expects the keys to be there.
    /// </returns>
    public override JsonObject ToJson() => new()
    {
        ["path"] = Path,
        ["url"] = Url,
        ["size"] = Size,
        ["orig_name"] = OrigName,
        ["mime_type"] = MimeType,
        ["is_stream"] = IsStream,
        ["meta"] = new JsonObject { ["_type"] = FileMetaType },
    };

    /// <summary>
    /// Reads a value the client sent as a file.
    /// </summary>
    /// <param name="value">The value, as the protocol carried it.</param>
    /// <returns>
    /// The file, or <see langword="null"/> when the value names no file — a JSON null, an object
    /// with no path, or a value of another shape.
    /// </returns>
    /// <remarks>
    /// A bare string is accepted as a path because that is one of the shapes a client may send: the
    /// upload endpoint answers with paths, and a caller that skips the metadata hands one back. An
    /// object without a <c>path</c> names nothing rather than naming an empty path, which is the
    /// difference between "no file" and "the working directory".
    /// </remarks>
    public static FileData? FromJson(JsonNode? value)
    {
        if (value is JsonValue scalar && scalar.TryGetValue<string>(out var text))
        {
            return new FileData { Path = text };
        }

        if (value is not JsonObject document)
        {
            return null;
        }

        var path = JsonValues.Text(document["path"]);
        if (path is null)
        {
            return null;
        }

        return new FileData
        {
            Path = path,
            Url = JsonValues.Text(document["url"]),
            Size = JsonValues.Long(document["size"]),
            OrigName = JsonValues.Text(document["orig_name"]),
            MimeType = JsonValues.Text(document["mime_type"]),
            IsStream = JsonValues.Flag(document["is_stream"]),
        };
    }
}

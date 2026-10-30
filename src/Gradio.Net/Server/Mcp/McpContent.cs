using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// The content a tool's answer is made of: the values the endpoint returned, as an agent can read
/// them.
/// </summary>
/// <remarks>
/// <para>
/// MCP carries content, not JSON: a text block, or an image as bytes. The Python version turns
/// every value into a text block except an image, which it sends as an image block followed by the
/// address it can also be fetched from — so an agent that can see the picture sees it, and one that
/// cannot still has somewhere to look.
/// </para>
/// <para>
/// A value that is not text is written as the JSON it is. The Python version writes Python's
/// <c>str()</c>, which for a dictionary is its repr; the JSON rendering says the same thing without
/// asking a reader to parse a Python literal, and it is the same document the endpoint's schema
/// described.
/// </para>
/// </remarks>
internal static class McpContent
{
    /// <summary>The content blocks an answer is made of.</summary>
    /// <param name="data">The values the endpoint returned.</param>
    /// <param name="root">The address the application is served from.</param>
    /// <returns>The blocks, one or two per value.</returns>
    internal static JsonArray Of(JsonArray data, string root)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(root);

        var content = new JsonArray();

        foreach (var value in data)
        {
            if (value is JsonObject file && IsFile(file))
            {
                var path = JsonValues.Text(file["path"]);
                var address = Address(JsonValues.Text(file["url"]) ?? path, root);
                var mediaType = MediaMimeTypes.Guess(path ?? address);

                if (mediaType is not null
                    && mediaType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)
                    && path is not null
                    && System.IO.File.Exists(path))
                {
                    content.Add(new JsonObject
                    {
                        ["type"] = "image",
                        ["data"] = Convert.ToBase64String(System.IO.File.ReadAllBytes(path)),
                        ["mimeType"] = mediaType,
                    });

                    content.Add(Text($"Image URL: {address}"));

                    continue;
                }

                content.Add(Text(address));

                continue;
            }

            content.Add(Text(TextOf(value)));
        }

        return content;
    }

    /// <summary>A value as an agent reads it.</summary>
    /// <param name="value">The value.</param>
    /// <returns>The text.</returns>
    internal static string TextOf(JsonNode? value) => value switch
    {
        null => "null",
        JsonValue scalar when scalar.TryGetValue<string>(out var text) => text,
        _ => value.ToJsonString(),
    };

    /// <summary>Whether a value is the object a file travels as.</summary>
    /// <param name="value">The value.</param>
    /// <returns>Whether it is.</returns>
    private static bool IsFile(JsonObject value) =>
        JsonValues.Text(value["meta"]?["_type"]) == FileData.FileMetaType;

    /// <summary>A file's address, made absolute against the address the application is served from.</summary>
    /// <param name="address">The address the value carries, or <see langword="null"/>.</param>
    /// <param name="root">The address the application is served from.</param>
    /// <returns>The absolute address, or an empty string for a file that has none.</returns>
    /// <remarks>
    /// A path becomes the address the file endpoint serves it at, which is
    /// <see cref="MediaCache.UrlOf"/>'s business rather than this one's: an agent is handed the same
    /// URL a browser would fetch, so the encoding has to be the same encoding.
    /// </remarks>
    private static string Address(string? address, string root) => address switch
    {
        null or "" => string.Empty,
        _ when address.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || address.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            || address.StartsWith("data:", StringComparison.OrdinalIgnoreCase) => address,
        _ when address.StartsWith('/') => root + address,
        _ => root + MediaCache.UrlOf(address),
    };

    /// <summary>A text block.</summary>
    /// <param name="text">The text.</param>
    /// <returns>The block.</returns>
    private static JsonObject Text(string text) => new()
    {
        ["type"] = "text",
        ["text"] = text,
    };
}

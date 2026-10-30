using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// How a JSON document is written when it is sent to a browser.
/// </summary>
/// <remarks>
/// <para>
/// The Python version serializes with orjson, which writes text as text: non-ASCII characters stay
/// characters, and the characters the default .NET encoder escapes for HTML safety — <c>&lt;</c>,
/// <c>&gt;</c>, <c>&amp;</c>, <c>+</c> and a few more — are written as they are. The difference is
/// not cosmetic: a manifest whose <c>image/svg+xml</c> comes out as <c>image/svg\u002Bxml</c> is a
/// different document, byte for byte.
/// </para>
/// <para>
/// The one place that must escape more is the page: the config is embedded in a script element, so
/// <c>&lt;</c> there would end the element early. <c>PageTemplate</c> escapes those four characters
/// on top of these options, which is what the Python version's own template filter does.
/// </para>
/// </remarks>
internal static class JsonText
{
    /// <summary>The options every served document is written with.</summary>
    internal static readonly JsonSerializerOptions Options = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    /// <summary>Writes a document as it goes over the wire.</summary>
    /// <param name="document">The document to write.</param>
    /// <returns>The document's text.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="document"/> is <see langword="null"/>.</exception>
    internal static string Serialize(JsonNode document)
    {
        ArgumentNullException.ThrowIfNull(document);

        return document.ToJsonString(Options);
    }
}

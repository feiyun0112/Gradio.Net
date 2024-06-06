using Winista.Mime;

namespace Gradio.Net;

internal static class ClientUtils
{
    private static MimeTypes _mimeTypes = new MimeTypes();

    internal static string GetMimeType(string filePath)
    {
        return _mimeTypes.GetMimeType(filePath)?.Name ?? "application/octet-stream";
    }

    internal static bool IsUrl(string str)
    {
        return str.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) || str.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase);
    }
}

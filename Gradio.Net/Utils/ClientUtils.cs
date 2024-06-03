using Microsoft.AspNetCore.StaticFiles;

namespace Gradio.Net;

internal static class ClientUtils
{
    internal static string GetMimeType(string filePath)
    {
        const string DefaultContentType = "application/octet-stream";
        FileExtensionContentTypeProvider provider = new();
        if (!provider.TryGetContentType(filePath, out string contentType))
        {
            contentType = DefaultContentType;
        }

        return contentType;
    }

    internal static bool IsUrl(string str)
    {
        return str.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) || str.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase);
    }
}

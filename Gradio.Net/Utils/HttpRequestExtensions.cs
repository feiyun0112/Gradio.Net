using Microsoft.AspNetCore.Http;

namespace Gradio.Net;

internal static class HttpRequestExtensions
{
    public static string GetRootUrl(this HttpRequest httpRequest)
    {
        return $"{httpRequest.Scheme}://{httpRequest.Host}";
    }
}

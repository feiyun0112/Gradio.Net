using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Gradio.Net
{
    internal static class ClientUtils
    {
        internal static string GetMimeType(string filePath)
        {
            const string DefaultContentType = "application/octet-stream";
            var provider = new FileExtensionContentTypeProvider();
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
}

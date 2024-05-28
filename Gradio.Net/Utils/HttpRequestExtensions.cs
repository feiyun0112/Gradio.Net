using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gradio.Net
{
    internal static class HttpRequestExtensions
    {
        public static string GetRootUrl(this HttpRequest httpRequest)
        {
            return $"{httpRequest.Scheme}://{httpRequest.Host}";
        }
    }
}

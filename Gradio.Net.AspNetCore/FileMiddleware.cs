using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gradio.Net.AspNetCore
{
    internal class FileMiddleware
    {
        private readonly RequestDelegate _next;

        public FileMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, GradioApp gradioApp)
        {
            string path = httpContext.Request.Path.ToString();
            const string FILE_URL = "/file=";
            if (path.StartsWith(FILE_URL))
            {
                (string filePath, string contentType) = gradioApp.GetUploadedFile(path.Substring(FILE_URL.Length));
                httpContext.Response.ContentType = contentType;
                using (FileStream fs = new(filePath, FileMode.Open))
                {
                    await fs.CopyToAsync(httpContext.Response.Body);
                }
                return;
            }
            await _next(httpContext);
        }
    }
}

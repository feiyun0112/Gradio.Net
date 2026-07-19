using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace Gradio.Net;

public class RangedStaticFilesMiddleware
{
    private static readonly Regex RangeRegex = new Regex(@"^bytes=(?<start>\d+)-(?<end>\d*)$", RegexOptions.Compiled);
    private readonly RequestDelegate _next;
    private readonly StaticFileOptions _options;
    private readonly IFileProvider _fileProvider;

    public RangedStaticFilesMiddleware(RequestDelegate next, IWebHostEnvironment environment, IOptions<StaticFileOptions> options)
    {
        _next = next;
        _options = options.Value;
        _fileProvider = _options.FileProvider ?? environment.ContentRootFileProvider;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var request = context.Request;
        var response = context.Response;

        if ((request.Method == HttpMethods.Get || request.Method == HttpMethods.Head) && request.Headers.ContainsKey("Range"))
        {
            var path = request.Path;
            var fileInfo = _fileProvider.GetFileInfo(path.Value.TrimStart('/'));

            if (fileInfo.Exists && !fileInfo.IsDirectory)
            {
                var rangeHeader = request.Headers["Range"];
                var match = RangeRegex.Match(rangeHeader);

                if (match.Success)
                {
                    var start = long.Parse(match.Groups["start"].Value);
                    var endGroup = match.Groups["end"];
                    var end = endGroup.Success && !string.IsNullOrEmpty(endGroup.Value) ? long.Parse(endGroup.Value) : (long?)null;

                    var range = new OpenRange(start, end);
                    var contentTypeProvider = new FileExtensionContentTypeProvider();
                    string contentType;
                    if (!contentTypeProvider.TryGetContentType(fileInfo.Name, out contentType))
                    {
                        contentType = "application/octet-stream";
                    }

                    var rangedResponse = new RangedFileResponse(fileInfo, range, contentType);
                    await rangedResponse.ExecuteResultAsync(new ActionContext(context, new RouteData(), new ActionDescriptor()));
                    return;
                }
                else
                {
                    response.StatusCode = 400;
                    return;
                }
            }
        }

        await _next(context);

        if (response.StatusCode == 200 && response.ContentType != null)
        {
            response.Headers.AcceptRanges = "bytes";
        }
    }
}

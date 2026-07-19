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

public class RangedFileResponse : FileResult
{
    private const int ChunkSize = 4096;
    private readonly OpenRange _range;
    private readonly IFileInfo _fileInfo;

    public RangedFileResponse(string filePath, OpenRange range, string contentType = null, string fileDownloadName = null)
        : base(contentType ?? "application/octet-stream")
    {
        FileDownloadName = fileDownloadName;
        _range = range;
        _fileInfo = new Microsoft.Extensions.FileProviders.Physical.PhysicalFileInfo(new FileInfo(filePath));
    }

    public RangedFileResponse(IFileInfo fileInfo, OpenRange range, string contentType = null, string fileDownloadName = null)
        : base(contentType ?? "application/octet-stream")
    {
        FileDownloadName = fileDownloadName;
        _range = range;
        _fileInfo = fileInfo;
    }

    private void SetRangeHeaders(HttpResponse response, ClosedRange range)
    {
        var totalLength = _fileInfo.Length;
        var contentLength = range.Length;
        response.Headers.Add("Content-Range", $"bytes {range.Start}-{range.End}/{totalLength}");
        response.ContentLength = contentLength;
    }

    public override async Task ExecuteResultAsync(ActionContext context)
    {
        if (!_fileInfo.Exists)
        {
            throw new FileNotFoundException($"File at path {_fileInfo.PhysicalPath} does not exist.");
        }

        if (!_fileInfo.IsDirectory)
        {
            var response = context.HttpContext.Response;
            response.StatusCode = 206; // Partial Content
            var sendHeaderOnly = string.Equals(context.HttpContext.Request.Method, HttpMethods.Head, StringComparison.OrdinalIgnoreCase);

            var closedRange = _range.Clamp(0, _fileInfo.Length - 1);
            SetRangeHeaders(response, closedRange);

            if (!string.IsNullOrEmpty(FileDownloadName))
            {
                response.Headers.Add("Content-Disposition", $"attachment; filename=\"{FileDownloadName}\"");
            }

            response.Headers.AcceptRanges = "bytes";
            response.ContentType = ContentType;

            if (sendHeaderOnly)
            {
                return;
            }

            using (var fileStream = _fileInfo.CreateReadStream())
            {
                fileStream.Position = closedRange.Start;
                var remainingBytes = closedRange.Length;

                if (remainingBytes <= 0)
                {
                    await response.Body.WriteAsync(Array.Empty<byte>());
                    return;
                }

                var buffer = new byte[ChunkSize];
                while (remainingBytes > 0)
                {
                    var bytesToRead = (int)Math.Min(ChunkSize, remainingBytes);
                    var bytesRead = await fileStream.ReadAsync(buffer, 0, bytesToRead);
                    if (bytesRead == 0)
                    {
                        break;
                    }

                    await response.Body.WriteAsync(buffer, 0, bytesRead);
                    remainingBytes -= bytesRead;
                }
            }
        }
        else
        {
            throw new InvalidOperationException($"File at path {_fileInfo.PhysicalPath} is not a file.");
        }
    }
}

using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace Gradio.Net.Core;


public class GradioMultiPartParser
{
    private readonly HttpRequest _request;
    private readonly int _maxFiles;
    private readonly int _maxFields;
    private readonly string _uploadId;
    private readonly FileUploadProgress _uploadProgress;
    private readonly long _maxFileSize;
    private readonly int _maxHeaderSize;

    public GradioMultiPartParser(
        HttpRequest request,
        int maxFiles = 1000,
        int maxFields = 1000,
        string uploadId = null,
        FileUploadProgress uploadProgress = null,
        long maxFileSize = 104857600, // 100MB
        int maxHeaderSize = 8192 // 8KB
    )
    {
        _request = request;
        _maxFiles = maxFiles;
        _maxFields = maxFields;
        _uploadId = uploadId;
        _uploadProgress = uploadProgress;
        _maxFileSize = maxFileSize;
        _maxHeaderSize = maxHeaderSize;
    }

    public async Task<IDictionary<string, object>> ParseAsync(CancellationToken cancellationToken = default)
    {
        var formDictionary = new Dictionary<string, object>();
        var formFiles = new List<IFormFile>();
        int fileCount = 0;
        int fieldCount = 0;

        if (!_request.HasFormContentType)
        {
            throw new InvalidOperationException("Request does not have form content type");
        }

        try
        {
            var form = await _request.ReadFormAsync(cancellationToken);

            // Process form fields
            foreach (var field in form)
            {
                if (field.Value.Count > 0)
                {
                    fieldCount++;
                    if (fieldCount > _maxFields)
                    {
                        throw new InvalidOperationException($"Too many fields. Maximum number of fields is {_maxFields}.");
                    }
                    formDictionary[field.Key] = field.Value.ToString();
                }
            }

            // Process form files
            foreach (var file in form.Files)
            {
                fileCount++;
                if (fileCount > _maxFiles)
                {
                    throw new InvalidOperationException($"Too many files. Maximum number of files is {_maxFiles}.");
                }

                if (file.Length > _maxFileSize)
                {
                    throw new InvalidOperationException($"File size exceeded maximum allowed size of {_maxFileSize} bytes.");
                }

                // Create GradioUploadFile and update SHA hash
                var gradioFile = new GradioUploadFile(file);
                if (file.Length > 0)
                {
                    using (var stream = file.OpenReadStream())
                    {
                        var buffer = new byte[8192];
                        int bytesRead;
                        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                        {
                            gradioFile.UpdateSha(buffer.Take(bytesRead).ToArray());

                            // Update upload progress if tracking
                            if (_uploadProgress != null && _uploadId != null)
                            {
                                _uploadProgress.Append(_uploadId, file.FileName, buffer.Take(bytesRead).ToArray());
                            }
                        }
                    }
                }

                formDictionary[file.Name] = gradioFile;
                formFiles.Add(gradioFile);
            }

            // Set done status if tracking upload progress
            if (_uploadProgress != null && _uploadId != null)
            {
                _uploadProgress.SetDone(_uploadId);
            }

            return formDictionary;
        }
        catch (Exception ex)
        {
            // Set done status on error if tracking upload progress
            if (_uploadProgress != null && _uploadId != null)
            {
                _uploadProgress.SetDone(_uploadId);
            }
            throw;
        }
    }
}

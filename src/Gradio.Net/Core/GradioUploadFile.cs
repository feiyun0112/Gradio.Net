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


public class GradioUploadFile : IFormFile
{
    private readonly IFormFile _file;
    public SHA256 Sha { get; }

    public GradioUploadFile(IFormFile file)
    {
        _file = file;
        Sha = SHA256.Create();
        // Update with hash seed if available
        var hashSeed = Encoding.UTF8.GetBytes(ProcessingUtils.GetHashSeed());
        Sha.TransformBlock(hashSeed, 0, hashSeed.Length, hashSeed, 0);
    }

    public string ContentType => _file.ContentType;
    public string ContentDisposition => _file.ContentDisposition;
    public IHeaderDictionary Headers => _file.Headers;
    public long Length => _file.Length;
    public string Name => _file.Name;
    public string FileName => _file.FileName;

    public Stream OpenReadStream() => _file.OpenReadStream();

    public void CopyTo(Stream target) => _file.CopyTo(target);

    public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
        => _file.CopyToAsync(target, cancellationToken);

    // Method to update the SHA hash with file data
    public void UpdateSha(byte[] data)
    {
        Sha.TransformBlock(data, 0, data.Length, data, 0);
    }

    // Method to finalize the SHA hash
    public string GetShaHash()
    {
        Sha.TransformFinalBlock(new byte[0], 0, 0);
        return Convert.ToBase64String(Sha.Hash);
    }
}

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


public class FileUploadProgressUnit
{
    public string Filename { get; set; }
    public int ChunkSize { get; set; }

    public FileUploadProgressUnit(string filename, int chunkSize)
    {
        Filename = filename;
        ChunkSize = chunkSize;
    }
}

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


public class FileUploadProgressTracker
{
    public Queue<FileUploadProgressUnit> Queue { get; set; }
    public bool IsDone { get; set; }

    public FileUploadProgressTracker()
    {
        Queue = new Queue<FileUploadProgressUnit>();
        IsDone = false;
    }
}


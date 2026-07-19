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

public class ClosedRange
{
    public long Start { get; }
    public long End { get; }

    public ClosedRange(long start, long end)
    {
        Start = start;
        End = end;
    }

    public long Length => End - Start + 1;

    public bool IsValid => Length > 0;
}

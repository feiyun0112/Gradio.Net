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

public class OpenRange
{
    public long Start { get; }
    public long? End { get; }

    public OpenRange(long start, long? end = null)
    {
        Start = start;
        End = end;
    }

    public ClosedRange Clamp(long start, long end)
    {
        var begin = Math.Max(this.Start, start);
        var endValue = Math.Min(this.End ?? end, end);

        begin = Math.Min(begin, endValue);
        endValue = Math.Max(begin, endValue);

        return new ClosedRange(begin, endValue);
    }
}

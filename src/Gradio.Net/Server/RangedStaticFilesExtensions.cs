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

public static class RangedStaticFilesExtensions
{
    public static IApplicationBuilder UseRangedStaticFiles(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RangedStaticFilesMiddleware>();
    }

    public static IApplicationBuilder UseRangedStaticFiles(this IApplicationBuilder app, StaticFileOptions options)
    {
        return app.UseMiddleware<RangedStaticFilesMiddleware>(Options.Create(options));
    }
}

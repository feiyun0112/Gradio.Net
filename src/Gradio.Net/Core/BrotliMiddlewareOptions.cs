using Microsoft.AspNetCore.Builder;

namespace Gradio.Net.Core;

public class BrotliMiddlewareOptions
{
    public int Quality { get; set; } = 4;

    public string Mode { get; set; } = "text";

    public int Lgwin { get; set; } = 22;

    public int Lgblock { get; set; } = 0;

    public int MinimumSize { get; set; } = 400;

    public bool GzipFallback { get; set; } = true;

    public List<string>? ExcludedHandlers { get; set; }
}


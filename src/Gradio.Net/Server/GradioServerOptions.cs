
namespace Gradio.Net.Server;

public class GradioServerOptions
{
    public string? ServerName { get; set; } = "127.0.0.1";

    public int? ServerPort { get; set; } = 7860;

    public bool Share { get; set; } = false;

    public bool Debug { get; set; } = false;

    public object? Auth { get; set; } = null;

    public string? AuthMessage { get; set; } = null;

    public bool ShowError { get; set; } = true;

    public int? Height { get; set; } = null;

    public int? Width { get; set; } = null;

    public string? FaviconPath { get; set; } = null;

    public bool SslVerify { get; set; } = true;

    public bool Quiet { get; set; } = false;

    public List<string>? AllowedPaths { get; set; } = null;

    public List<string>? BlockedPaths { get; set; } = null;

    public string? RootPath { get; set; } = null;

    public int StateSessionCapacity { get; set; } = 10000;

    public bool Pwa { get; set; } = false;

    public bool McpServer { get; set; } = false;
}

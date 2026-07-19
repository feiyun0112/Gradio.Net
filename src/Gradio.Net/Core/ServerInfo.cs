using Microsoft.Extensions.Hosting;

namespace Gradio.Net.Core;

public class ServerInfo
{
    public string ServerName { get; set; } = string.Empty;
    public int Port { get; set; }
    public string PathToLocalServer { get; set; } = string.Empty;
    public IHost Host { get; set; } = null!;
    public CancellationTokenSource? CancellationTokenSource { get; set; }
}

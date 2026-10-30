using System.Globalization;
using System.Net;
using System.Text;

namespace Gradio.Net;
/// <summary>
/// The page a browser gets when it opens the MCP endpoint, which is not what the endpoint is for.
/// </summary>
/// <remarks>
/// <para>
/// The streamable transport expects a client that asks for an event stream, so a person who clicked
/// the address printed at launch would otherwise be shown a JSON-RPC refusal. The Python version
/// answers with a page instead, and the useful part of that page is the address itself and the
/// snippets that paste into an agent's configuration — which is what this is.
/// </para>
/// <para>
/// The markup is this framework's rather than the Python version's: the Python version's page is a
/// stylesheet's worth of presentation around the same four facts, and reproducing it byte for byte
/// would be reproducing a design rather than a behaviour. What is reproduced is the behaviour a
/// client of the page sees: the address, and how to connect.
/// </para>
/// </remarks>
internal static class McpLandingPage
{
    /// <summary>The page, for an application served from a given address.</summary>
    /// <param name="root">The address the application is served from.</param>
    /// <returns>The HTML.</returns>
    internal static string For(string root)
    {
        ArgumentNullException.ThrowIfNull(root);

        var address = $"{root}{McpEndpoint.Prefix}/";
        var shown = WebUtility.HtmlEncode(address);
        var quoted = System.Text.Json.JsonSerializer.Serialize(address);

        var cursor = WebUtility.HtmlEncode(
            "{\"mcpServers\":{\"gradio\":{\"url\":" + quoted + "}}}");

        var command = WebUtility.HtmlEncode($"claude mcp add --transport http gradio {address}");

        var page = new StringBuilder();
        page.AppendLine("<!DOCTYPE html>");
        page.AppendLine("<html lang=\"en\">");
        page.AppendLine("<head>");
        page.AppendLine("<meta charset=\"utf-8\" />");
        page.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" />");
        page.AppendLine("<title>Gradio MCP Server</title>");
        page.AppendLine("<style>");
        page.AppendLine("body{font-family:system-ui,-apple-system,'Segoe UI',sans-serif;line-height:1.6;color:#1f2937;background:#f6f7f8;margin:0;padding:3rem 1.25rem}");
        page.AppendLine("main{max-width:44rem;margin:0 auto}");
        page.AppendLine("h1{font-size:1.6rem;margin:0 0 .4rem}");
        page.AppendLine("p.lead{color:#6b7280;margin:0 0 1.75rem}");
        page.AppendLine("section{background:#fff;border:1px solid #e5e7eb;border-radius:12px;padding:1.25rem 1.4rem;margin-bottom:1.1rem}");
        page.AppendLine("h2{font-size:.78rem;letter-spacing:.06em;text-transform:uppercase;color:#ff7c00;margin:0 0 .7rem}");
        page.AppendLine("code{font-family:ui-monospace,SFMono-Regular,Consolas,monospace;font-size:.9rem;word-break:break-all}");
        page.AppendLine("pre{margin:0;background:#0b1221;color:#e2e8f0;border-radius:8px;padding:.9rem 1rem;overflow-x:auto}");
        page.AppendLine("pre code{color:inherit}");
        page.AppendLine("</style>");
        page.AppendLine("</head>");
        page.AppendLine("<body>");
        page.AppendLine("<main>");
        page.AppendLine("<h1>Gradio MCP Server</h1>");
        page.AppendLine("<p class=\"lead\">This is the Model Context Protocol endpoint for a Gradio app. It is meant to be connected to by an MCP client, not opened in a browser.</p>");
        page.AppendLine("<section>");
        page.AppendLine("<h2>MCP Server URL</h2>");
        page.AppendLine(CultureInfo.InvariantCulture, $"<code id=\"address\">{shown}</code>");
        page.AppendLine("</section>");
        page.AppendLine("<section>");
        page.AppendLine("<h2>Cursor</h2>");
        page.AppendLine("<p>Add this to <code>~/.cursor/mcp.json</code>:</p>");
        page.AppendLine(CultureInfo.InvariantCulture, $"<pre><code>{cursor}</code></pre>");
        page.AppendLine("</section>");
        page.AppendLine("<section>");
        page.AppendLine("<h2>Claude Code</h2>");
        page.AppendLine("<p>Run this in your terminal:</p>");
        page.AppendLine(CultureInfo.InvariantCulture, $"<pre><code>{command}</code></pre>");
        page.AppendLine("</section>");
        page.AppendLine("</main>");
        page.AppendLine("</body>");
        page.AppendLine("</html>");

        return page.ToString();
    }
}

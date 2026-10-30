using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// The page an operator opens to see what their application is doing.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's panel is a second Gradio application mounted beside the first, which reads
/// the queue's records out of a dictionary the two share. That shape is not available here: one
/// container hosts one application, so a panel cannot be a second one. What is served instead is a
/// page built from the same two facts the mounted application read — the per-binding summary and
/// the records behind it — so that opening the address the Python version prints shows the same
/// traffic and the same errors.
/// </para>
/// <para>
/// It is a page rather than a charting application on purpose: the numbers are the contract, and a
/// reader who wants them in their own dashboard has the summary route. The page refreshes itself on
/// the interval the Python version's timer uses, so that an operator can leave it open.
/// </para>
/// <para>
/// Every value that came from an application — a binding's name, a session hash — is HTML-encoded
/// here: a binding's name is written by the application's author, and the panel is served from the
/// application's own origin.
/// </para>
/// </remarks>
internal static class MonitoringPage
{
    /// <summary>How often the page reloads itself, in seconds.</summary>
    /// <remarks>The Python version's panel asks its timer for a tick every five seconds.</remarks>
    private const int RefreshSeconds = 5;

    /// <summary>How many records the page lists.</summary>
    internal const int RecentCount = 200;

    /// <summary>Renders the page.</summary>
    /// <param name="title">The application's title, for the heading.</param>
    /// <param name="summary">The per-binding summary, as <c>/monitoring/summary</c> reports it.</param>
    /// <param name="records">The newest records, oldest first.</param>
    /// <returns>The page.</returns>
    internal static string Render(string title, JsonObject summary, IReadOnlyList<JsonObject> records)
    {
        ArgumentNullException.ThrowIfNull(title);
        ArgumentNullException.ThrowIfNull(summary);
        ArgumentNullException.ThrowIfNull(records);

        var page = new StringBuilder();
        var invariant = CultureInfo.InvariantCulture;

        page.Append("<!doctype html><html lang=\"en\"><head><meta charset=\"utf-8\">");
        page.Append("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");
        page.Append(invariant, $"<meta http-equiv=\"refresh\" content=\"{RefreshSeconds}\">");
        page.Append(invariant, $"<title>Monitoring — {Encode(title)}</title>");
        page.Append("<style>");
        page.Append(Styles);
        page.Append("</style></head><body>");
        page.Append(invariant, $"<h1>Monitoring — {Encode(title)}</h1>");

        page.Append("<h2>Endpoints</h2>");
        page.Append("<table><thead><tr><th>Endpoint</th><th>Requests</th><th>Success rate</th>");
        page.Append("<th>p50 (s)</th><th>p90 (s)</th><th>p99 (s)</th></tr></thead><tbody>");

        if (summary["functions"] is not JsonObject functions || functions.Count == 0)
        {
            page.Append("<tr><td colspan=\"6\" class=\"empty\">Nothing has been run yet.</td></tr>");
        }
        else
        {
            foreach (var (name, entry) in functions)
            {
                var percentiles = entry?["process_time_percentiles"] as JsonObject;

                page.Append("<tr>");
                page.Append(invariant, $"<td>{Encode(name)}</td>");
                page.Append(invariant, $"<td>{Number(entry?["total_requests"])}</td>");
                page.Append(invariant, $"<td>{Rate(entry?["success_rate"])}</td>");
                page.Append(invariant, $"<td>{Number(percentiles?["50th"])}</td>");
                page.Append(invariant, $"<td>{Number(percentiles?["90th"])}</td>");
                page.Append(invariant, $"<td>{Number(percentiles?["99th"])}</td>");
                page.Append("</tr>");
            }
        }

        page.Append("</tbody></table>");

        page.Append("<h2>Recent requests</h2>");
        page.Append("<table><thead><tr><th>Time</th><th>Endpoint</th><th>Status</th>");
        page.Append("<th>Process time (s)</th><th>Session</th></tr></thead><tbody>");

        if (records.Count == 0)
        {
            page.Append("<tr><td colspan=\"5\" class=\"empty\">Nothing has been run yet.</td></tr>");
        }
        else
        {
            foreach (var record in records)
            {
                var status = Text(record["status"]);

                page.Append("<tr>");
                page.Append(invariant, $"<td>{Time(record["time"])}</td>");
                page.Append(invariant, $"<td>{Encode(Text(record["function"]))}</td>");
                page.Append(invariant, $"<td class=\"{StatusClass(status)}\">{Encode(status)}</td>");
                page.Append(invariant, $"<td>{Number(record["process_time"])}</td>");
                page.Append(invariant, $"<td>{Encode(Text(record["session_hash"]))}</td>");
                page.Append("</tr>");
            }
        }

        page.Append("</tbody></table>");
        page.Append(invariant, $"<p class=\"note\">Refreshed every {RefreshSeconds} seconds. ");
        page.Append("The same numbers are served as JSON at <code>../monitoring/summary</code>.</p>");
        page.Append("</body></html>");

        return page.ToString();
    }

    /// <summary>A number as the page shows it, or a dash when there is none.</summary>
    /// <param name="value">The value.</param>
    /// <returns>The text.</returns>
    private static string Number(JsonNode? value) =>
        AsNumber(value) is { } number ? number.ToString("0.###", CultureInfo.InvariantCulture) : "—";

    /// <summary>A success rate as a percentage.</summary>
    /// <param name="value">The rate, or <see langword="null"/> when nothing has been decided.</param>
    /// <returns>The text.</returns>
    private static string Rate(JsonNode? value) =>
        AsNumber(value) is { } rate
            ? (rate * 100).ToString("0.#", CultureInfo.InvariantCulture) + "%"
            : "—";

    /// <summary>A wall-clock instant as the page shows it.</summary>
    /// <param name="value">The instant, in seconds since the Unix epoch.</param>
    /// <returns>The text, in UTC.</returns>
    /// <remarks>
    /// UTC rather than the browser's zone: the page is rendered on the server, which does not know
    /// what the operator's clock says, and a timestamp that is labelled is better than one that is
    /// silently local to somewhere else.
    /// </remarks>
    private static string Time(JsonNode? value) =>
        AsNumber(value) is { } seconds
            ? DateTimeOffset
                .FromUnixTimeMilliseconds((long)(seconds * 1000))
                .UtcDateTime
                .ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + "Z"
            : "—";

    /// <summary>A document's text, or the empty string when it holds none.</summary>
    /// <param name="value">The value.</param>
    /// <returns>The text.</returns>
    private static string Text(JsonNode? value) =>
        value is JsonValue entry && entry.TryGetValue<string>(out var text) ? text : string.Empty;

    /// <summary>A document's number, read whichever numeric type it was built from.</summary>
    /// <param name="value">The value.</param>
    /// <returns>The number, or <see langword="null"/> when the value is not one.</returns>
    /// <remarks>
    /// Read through the value's JSON text rather than through <c>GetValue&lt;double&gt;</c>, which
    /// insists on the type the node was created with: a count is an integer and a duration is not,
    /// and both are numbers as far as the page is concerned.
    /// </remarks>
    private static double? AsNumber(JsonNode? value) =>
        double.TryParse(
            value?.ToJsonString(),
            NumberStyles.Float,
            CultureInfo.InvariantCulture,
            out var number)
            ? number
            : null;

    /// <summary>The class that colours a status.</summary>
    /// <param name="status">The status.</param>
    /// <returns>The class name.</returns>
    private static string StatusClass(string status) => status switch
    {
        AnalyticsStatus.Success => "ok",
        AnalyticsStatus.Failed => "bad",
        AnalyticsStatus.Cancelled => "warn",
        _ => "wait",
    };

    /// <summary>Text that is safe to put in the document.</summary>
    /// <param name="text">The text.</param>
    /// <returns>The text with the characters that would end an element escaped.</returns>
    private static string Encode(string text) => WebUtility.HtmlEncode(text);

    /// <summary>The page's styles.</summary>
    /// <remarks>
    /// A light page with a dark variant, matching neither of the application's themes: the panel is
    /// not the application, and a page that dressed itself in the theme would look like part of it.
    /// </remarks>
    private const string Styles = """
        :root { color-scheme: light dark; font-family: system-ui, sans-serif; }
        body { margin: 2rem auto; max-width: 60rem; padding: 0 1rem; line-height: 1.4; }
        h1 { font-size: 1.4rem; margin-bottom: 1.5rem; }
        h2 { font-size: 1.1rem; margin: 2rem 0 .5rem; }
        table { border-collapse: collapse; width: 100%; font-variant-numeric: tabular-nums; }
        th, td { text-align: left; padding: .35rem .6rem; border-bottom: 1px solid #8883; }
        th { font-weight: 600; font-size: .85rem; opacity: .75; }
        td.empty { opacity: .6; font-style: italic; }
        .ok { color: #15803d; }
        .bad { color: #b91c1c; }
        .warn { color: #a16207; }
        .wait { opacity: .7; }
        .note { margin-top: 2rem; font-size: .85rem; opacity: .7; }
        """;
}

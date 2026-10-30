using System.Globalization;
using System.Net;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Gradio.Net;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The monitoring routes seen the way an operator sees them: a run is made, and then the panel's
/// data and the panel itself are asked what they have.
/// </summary>
/// <remarks>
/// <para>
/// The summary document is not only the panel's business — the API page in the vendored frontend
/// fetches it on load and draws a success rate and three percentiles from it — so the shape is
/// asserted field by field rather than "some JSON came back".
/// </para>
/// <para>
/// The queue hands runs to handlers on its own schedule, so the assertions that follow a run wait for
/// the record rather than reading it immediately.
/// </para>
/// </remarks>
[Collection(QueueCollection.Name)]
public sealed class MonitoringTests
{
    [Fact]
    public async Task An_application_that_has_run_nothing_summarises_to_no_endpoints()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(blocks => Textboxes(blocks), cancellationToken);

        using var response = await host.Client.GetAsync("/monitoring/summary", cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("""{"functions":{}}""", body);
    }

    [Fact]
    public async Task A_run_is_counted_under_its_endpoint()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(
            blocks =>
            {
                var (input, output) = Textboxes(blocks);

                input.Change(
                    (Func<string, string>)(value => value.ToUpperInvariant()),
                    [input],
                    [output],
                    apiName: "echo");
            },
            cancellationToken);

        await RunAsync(host, "echo", "hello", "counted", cancellationToken);

        var summary = await WaitForSummaryAsync(
            host.Client,
            functions => functions["echo"]?["success_rate"] is not null,
            cancellationToken);

        var entry = summary["functions"]!["echo"]!;

        Assert.Equal(1.0, entry["success_rate"]!.GetValue<double>());
        Assert.Equal(1, entry["total_requests"]!.GetValue<int>());

        // Every percentile is a number, and it is the one run's duration: a single measurement has
        // the same value at every rank.
        var percentiles = entry["process_time_percentiles"]!;
        var p50 = percentiles["50th"]!.GetValue<double>();

        Assert.True(p50 >= 0);
        Assert.Equal(p50, percentiles["90th"]!.GetValue<double>());
        Assert.Equal(p50, percentiles["99th"]!.GetValue<double>());
    }

    /// <summary>
    /// A poll taken while a run is still going does not cost the run the reading it is owed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The panel refreshes itself every five seconds, so it polls while runs are in flight — and
    /// the summary is a document that costs a sort over every record. It is computed when a run
    /// ends, as the Python version's queue computes it, and a read answers with what that
    /// computation produced.
    /// </para>
    /// <para>
    /// What this catches is the arrangement where a read recomputes: a poll taken mid-run would then
    /// spend the recomputation on "this run is still going", and the run's own end would find
    /// nothing left to spend — the panel would say <c>processing</c> for a run that finished minutes
    /// ago. So the polls below are taken while the handler is held, and the summary is asked again
    /// only after it was let go.
    /// </para>
    /// </remarks>
    [Fact]
    public async Task A_poll_during_a_run_does_not_leave_the_summary_saying_it_is_still_running()
    {
        var probe = new QueueProbe();
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(
            blocks =>
            {
                var (input, output) = Textboxes(blocks);

                input.Change(
                    (Func<string, Task<string>>)probe.Hold,
                    [input],
                    [output],
                    apiName: "held");
            },
            cancellationToken);

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("held", cancellationToken);

        var held = await queue.JoinAsync(fnIndex, ["a"], "polled", cancellationToken);

        await using var stream = await queue.OpenAsync("polled", cancellationToken);

        Assert.NotNull(await stream.UntilAsync(
            message => QueueSession.Is(message, "process_starts", held),
            TimeSpan.FromSeconds(15),
            cancellationToken));

        // Nothing has ended yet, so nothing is summarised — the answer a panel gets while the first
        // run of an application is still going.
        for (var poll = 0; poll < 3; poll++)
        {
            Assert.Equal(
                """{"functions":{}}""",
                await host.Client.GetStringAsync("/monitoring/summary", cancellationToken));
        }

        probe.Release();

        Assert.True(await stream.UntilClosedAsync(TimeSpan.FromSeconds(15), cancellationToken));

        var summary = await WaitForSummaryAsync(
            host.Client,
            functions => functions["held"]?["success_rate"] is not null,
            cancellationToken);

        Assert.Equal(1.0, summary["functions"]!["held"]!["success_rate"]!.GetValue<double>());
    }

    [Fact]
    public async Task A_run_that_raises_lowers_the_success_rate_and_reports_no_duration()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(
            blocks =>
            {
                var (input, output) = Textboxes(blocks);

                input.Change(
                    (Func<string, string>)(_ => throw new InvalidOperationException("no")),
                    [input],
                    [output],
                    apiName: "broken");
            },
            cancellationToken);

        await RunAsync(host, "broken", "hello", "broken", cancellationToken);

        var summary = await WaitForSummaryAsync(
            host.Client,
            functions => functions["broken"]?["success_rate"] is not null,
            cancellationToken);

        var entry = summary["functions"]!["broken"]!;

        Assert.Equal(0.0, entry["success_rate"]!.GetValue<double>());
        Assert.Equal(1, entry["total_requests"]!.GetValue<int>());

        // A failed run reports nothing rather than zero: a duration of zero would be read as an
        // instant answer, and numpy carries the unknown through the percentile the same way.
        Assert.Null(entry["process_time_percentiles"]!["50th"]);
    }

    [Fact]
    public async Task The_address_route_answers_the_page_upstream_answers()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(blocks => Textboxes(blocks), cancellationToken);

        using var response = await host.Client.GetAsync("/monitoring", cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal("See console for monitoring URL.", body);
    }

    [Fact]
    public async Task An_application_that_does_not_offer_monitoring_refuses_all_three_routes()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(
            blocks =>
            {
                blocks.EnableMonitoring = false;
                Textboxes(blocks);
            },
            cancellationToken);

        foreach (var path in new[] { "/monitoring", "/monitoring/summary", "/monitoring/whatever" })
        {
            using var response = await host.Client.GetAsync(path, cancellationToken);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            Assert.Equal(
                """{"detail":"Monitoring is not enabled."}""",
                await response.Content.ReadAsStringAsync(cancellationToken));
        }
    }

    [Fact]
    public async Task The_panel_opens_with_the_application_key_and_refuses_another()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(
            blocks =>
            {
                blocks.Title = "Monitored";

                var (input, output) = Textboxes(blocks);

                input.Change(
                    (Func<string, string>)(value => value),
                    [input],
                    [output],
                    apiName: "echo");
            },
            cancellationToken);

        await RunAsync(host, "echo", "hello", "panel", cancellationToken);

        var key = host.Services.GetRequiredService<Blocks>().AnalyticsKey;

        // The key is drawn, not configured: the Python version draws sixteen random bytes, and a panel whose
        // address could be guessed is a panel with no protection at all.
        Assert.Equal(22, key.Length);

        using var refused = await host.Client.GetAsync("/monitoring/not-the-key", cancellationToken);

        Assert.Equal(HttpStatusCode.Forbidden, refused.StatusCode);
        Assert.Equal(
            """{"detail":"Invalid key."}""",
            await refused.Content.ReadAsStringAsync(cancellationToken));

        using var panel = await host.Client.GetAsync($"/monitoring/{key}", cancellationToken);
        var html = await panel.Content.ReadAsStringAsync(cancellationToken);

        Assert.Equal(HttpStatusCode.OK, panel.StatusCode);
        Assert.Equal("text/html", panel.Content.Headers.ContentType?.MediaType);
        Assert.Contains("Monitoring — Monitored", html, StringComparison.Ordinal);
        Assert.Contains("echo", html, StringComparison.Ordinal);
    }

    /// <summary>
    /// The panel shows both of the things the queue remembers, and keeps itself up to date.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The two sources are asserted apart rather than as "the name appears somewhere": the summary
    /// table carries the success rate and the percentiles, which only the per-binding summary has,
    /// and the records table carries the session that asked, which only a per-event record has. A
    /// panel that drew one of them twice, or drew neither, fails on the field the other one owns.
    /// </para>
    /// <para>
    /// The refresh is part of the page rather than of a script: the Python version's panel ticks on
    /// a timer, and a page an operator leaves open has to pick up new traffic without being
    /// reloaded by hand.
    /// </para>
    /// </remarks>
    [Fact]
    public async Task The_panel_refreshes_itself_and_draws_both_the_summary_and_the_records()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(
            blocks =>
            {
                blocks.Title = "Both sources";

                var (input, output) = Textboxes(blocks);

                input.Change(
                    (Func<string, Task<string>>)(async value =>
                    {
                        await Task.Delay(50, TestContext.Current.CancellationToken);

                        return value;
                    }),
                    [input],
                    [output],
                    apiName: "echo");
            },
            cancellationToken);

        await RunAsync(host, "echo", "hello", "both-sources", cancellationToken);

        var key = host.Services.GetRequiredService<Blocks>().AnalyticsKey;

        var listed = await Eventually.IsTrueAsync(
            async () => (await host.Client.GetStringAsync($"/monitoring/{key}", cancellationToken))
                .Contains("both-sources", StringComparison.Ordinal),
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.True(listed, "The panel never listed the run that happened.");

        var html = await host.Client.GetStringAsync($"/monitoring/{key}", cancellationToken);

        Assert.Contains("<meta http-equiv=\"refresh\" content=\"5\">", html, StringComparison.Ordinal);

        // The summary's row: one request, all of it successful, and the three percentiles that only
        // a per-binding summary has. The percentiles are read as numbers rather than as "a cell is
        // there", because the page draws a dash when it has none — and a summary without its
        // percentiles is the half of the data this test exists for.
        Assert.Contains("<h2>Endpoints</h2>", html, StringComparison.Ordinal);

        var summaryRow = Regex.Match(html, "<tr><td>echo</td>(?:<td>[^<]*</td>){5}</tr>").Value;

        Assert.False(summaryRow.Length == 0, "The panel drew no summary row for the binding that ran.");

        var cells = Regex.Matches(summaryRow, "<td>([^<]*)</td>")
            .Select(match => match.Groups[1].Value)
            .ToArray();

        Assert.Equal("1", cells[1]);
        Assert.Equal("100%", cells[2]);

        foreach (var percentile in cells[3..])
        {
            Assert.True(
                double.TryParse(percentile, NumberStyles.Float, CultureInfo.InvariantCulture, out var value)
                && value > 0,
                $"The panel drew '{percentile}' for a percentile of a run that took time.");
        }

        // The records' row: the session that asked, which only a per-event record carries.
        Assert.Contains("<h2>Recent requests</h2>", html, StringComparison.Ordinal);
        Assert.Contains("<td>both-sources</td>", html, StringComparison.Ordinal);
        Assert.Contains("<td class=\"ok\">success</td>", html, StringComparison.Ordinal);
    }

    /// <summary>Starts a host serving an application this test wrote.</summary>
    /// <param name="build">Writes the application.</param>
    /// <param name="cancellationToken">Cancels the start.</param>
    /// <returns>The host.</returns>
    private static Task<GradioTestHost> StartAsync(Action<Blocks> build, CancellationToken cancellationToken) =>
        GradioTestHost.StartIntegratedAsync(logs: null, cancellationToken, configure: null, build);

    /// <summary>Runs a binding and waits until the client has been told it finished.</summary>
    /// <param name="host">The host to run against.</param>
    /// <param name="apiName">The binding to run.</param>
    /// <param name="value">The value its input carries.</param>
    /// <param name="session">The session to run it in.</param>
    /// <param name="cancellationToken">Cancels the run.</param>
    /// <returns>A task that completes once the run is over.</returns>
    private static async Task RunAsync(
        GradioTestHost host,
        string apiName,
        string value,
        string session,
        CancellationToken cancellationToken)
    {
        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync(apiName, cancellationToken);

        await queue.JoinAsync(fnIndex, [value], session, cancellationToken);

        await using var stream = await queue.OpenAsync(session, cancellationToken);

        Assert.NotNull(await stream.UntilAsync(
            message => QueueSession.Kind(message) == "process_completed",
            TimeSpan.FromSeconds(15),
            cancellationToken));
    }

    /// <summary>Waits for the summary to describe an endpoint, and answers with it.</summary>
    /// <param name="client">The client.</param>
    /// <param name="matches">What the summary has to say.</param>
    /// <param name="cancellationToken">Cancels the wait.</param>
    /// <returns>The summary document.</returns>
    private static async Task<JsonObject> WaitForSummaryAsync(
        HttpClient client,
        Func<JsonObject, bool> matches,
        CancellationToken cancellationToken)
    {
        JsonObject? summary = null;

        var found = await Eventually.IsTrueAsync(
            async () =>
            {
                var body = await client.GetStringAsync("/monitoring/summary", cancellationToken);
                summary = JsonNode.Parse(body)!.AsObject();

                return summary["functions"] is JsonObject functions && matches(functions);
            },
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.True(found, "The summary never described the run.");

        return summary!;
    }

    /// <summary>Adds the pair of textboxes these bindings read and write.</summary>
    /// <param name="blocks">The application.</param>
    /// <returns>The pair.</returns>
    private static (Textbox Input, Textbox Output) Textboxes(Blocks blocks)
    {
        using (blocks.BeginRender())
        {
            var input = blocks.Add(new Textbox { Label = "A" });
            var output = blocks.Add(new Textbox { Label = "Out" });

            return (input, output);
        }
    }
}

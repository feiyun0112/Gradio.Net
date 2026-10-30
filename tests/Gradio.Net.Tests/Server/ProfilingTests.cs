using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Gradio.Net;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The profiling routes seen the way an operator's tooling sees them: a run is made, and then the
/// traces, the summary and the request that forgets them are asked what they have.
/// </summary>
/// <remarks>
/// Profiling is an option the container holds rather than a variable read on every request, so the
/// tests turn it on through the seam an application uses: a registration made before Gradio's own,
/// which the "try add" leaves in place.
/// </remarks>
[Collection(QueueCollection.Name)]
public sealed class ProfilingTests
{
    [Fact]
    public async Task A_profiled_application_reports_one_trace_per_run()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(profiled: true, cancellationToken);

        await RunAsync(host, "profiled", cancellationToken);

        var traces = await Eventually.IsTrueAsync(
            async () => (await TracesAsync(host.Client, null, cancellationToken)).Count == 1,
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.True(traces, "The run was not traced.");

        var trace = (await TracesAsync(host.Client, null, cancellationToken))[0];

        Assert.Equal("echo", trace["fn_name"]!.GetValue<string>());
        Assert.Equal("profiled", trace["session_hash"]!.GetValue<string>());
        Assert.False(string.IsNullOrEmpty(trace["event_id"]!.GetValue<string>()));

        // The phases are the trace's whole point, and the handler's own call is the one a slow
        // endpoint is usually slow in.
        Assert.True(trace["fn_call_ms"]!.GetValue<double>() >= 0);
        Assert.True(trace["total_ms"]!.GetValue<double>() > 0);
        Assert.Equal(1, trace["n_iterations"]!.GetValue<int>());

        // A phase this framework does not measure is carried as zero rather than left out: the
        // document's shape is the contract.
        Assert.Equal(0.0, trace["preprocess_format_image_ms"]!.GetValue<double>());
    }

    [Fact]
    public async Task A_generator_counts_one_iteration_per_chunk()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(profiled: true, cancellationToken);

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("stream", cancellationToken);

        await queue.JoinAsync(fnIndex, ["go"], "generated", cancellationToken);

        await using var stream = await queue.OpenAsync("generated", cancellationToken);
        Assert.NotNull(await stream.UntilAsync(
            message => QueueSession.Kind(message) == "process_completed",
            TimeSpan.FromSeconds(15),
            cancellationToken));

        var counted = await Eventually.IsTrueAsync(
            async () =>
            {
                var traces = await TracesAsync(host.Client, null, cancellationToken);

                return traces.Count == 1 && traces[0]["n_iterations"]!.GetValue<int>() > 1;
            },
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.True(counted, "The streamed run was not counted chunk by chunk.");
    }

    [Fact]
    public async Task The_traces_route_reports_the_newest_runs_on_request()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(profiled: true, cancellationToken);

        await RunAsync(host, "first", cancellationToken);
        await RunAsync(host, "second", cancellationToken);

        var two = await Eventually.IsTrueAsync(
            async () => (await TracesAsync(host.Client, null, cancellationToken)).Count == 2,
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.True(two, "Both runs were not traced.");

        var newest = await TracesAsync(host.Client, 1, cancellationToken);

        Assert.Single(newest);
        Assert.Equal("second", newest[0]["session_hash"]!.GetValue<string>());

        using var refused = await host.Client.GetAsync("/gradio_api/profiling/traces?last_n=soon", cancellationToken);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, refused.StatusCode);
    }

    [Fact]
    public async Task The_summary_reports_the_percentiles_of_every_phase()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(profiled: true, cancellationToken);

        await RunAsync(host, "summary", cancellationToken);

        var summary = await Eventually.IsTrueAsync(
            async () =>
            {
                var body = await host.Client.GetStringAsync("/gradio_api/profiling/summary", cancellationToken);
                var document = JsonNode.Parse(body)!.AsObject();

                return document["count"]!.GetValue<int>() == 1
                    && document["phases"]!["total"]!["p50"]!.GetValue<double>() > 0;
            },
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.True(summary, "The summary never described the run.");

        var phases = JsonNode.Parse(
            await host.Client.GetStringAsync("/gradio_api/profiling/summary", cancellationToken))!["phases"]!;

        foreach (var phase in new[] { "queue_wait", "preprocess", "fn_call", "postprocess", "streaming_diff", "total" })
        {
            Assert.NotNull(phases[phase]);

            foreach (var key in new[] { "p50", "p90", "p95", "p99", "mean", "min", "max" })
            {
                Assert.NotNull(phases[phase]![key]);
            }
        }

        // A run that was not an upload reports no upload block at all, which is what the Python version's
        // summary does when nothing has been uploaded.
        Assert.Null(phases.Parent?["upload"]);
    }

    [Fact]
    public async Task The_summary_of_nothing_reports_no_phases_at_all()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(profiled: true, cancellationToken);

        // Six sets of zeros would say every run costs nothing, which is a different fact from
        // "nothing has run". The Python version answers an empty collector with the count and no phases.
        Assert.Equal(
            """{"count":0,"phases":{}}""",
            await host.Client.GetStringAsync("/gradio_api/profiling/summary", cancellationToken));
    }

    [Fact]
    public async Task A_last_n_of_zero_reports_every_trace()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(profiled: true, cancellationToken);

        await RunAsync(host, "first", cancellationToken);
        await RunAsync(host, "second", cancellationToken);

        var two = await Eventually.IsTrueAsync(
            async () => (await TracesAsync(host.Client, null, cancellationToken)).Count == 2,
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.True(two, "Both runs were not traced.");

        // Zero is the edge of Python's slice rather than its opposite: `traces[-0:]` is `traces[0:]`,
        // which is every trace. Reading it as "none" would answer the newest zero runs with all of
        // them missing.
        Assert.Equal(2, (await TracesAsync(host.Client, 0, cancellationToken)).Count);
    }

    [Fact]
    public async Task An_upload_is_traced_apart_from_the_runs()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();

        await using var host = await StartAsync(profiled: true, cancellationToken, folder.Folder);

        using var body = Multipart("note.txt", "hello"u8.ToArray());
        using var response = await host.Client.PostAsync("/gradio_api/upload", body, cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var uploaded = await Eventually.IsTrueAsync(
            async () =>
            {
                var traces = await TracesAsync(host.Client, null, cancellationToken);

                return traces.Count == 1
                    && traces[0]["fn_name"]!.GetValue<string>() == "gradio_file_upload";
            },
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.True(uploaded, "The upload was not traced.");

        var summary = JsonNode.Parse(
            await host.Client.GetStringAsync("/gradio_api/profiling/summary", cancellationToken))!.AsObject();

        // An upload is not a run: it counts in the upload block and not in the run count, which is
        // the split that makes "what receiving files costs" answerable on its own.
        Assert.Equal(0, summary["count"]!.GetValue<int>());
        Assert.Equal(1, summary["upload"]!["count"]!.GetValue<int>());
        Assert.True(summary["upload"]!["p50"]!.GetValue<double>() >= 0);
    }

    [Fact]
    public async Task Clearing_forgets_every_trace()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(profiled: true, cancellationToken);

        await RunAsync(host, "cleared", cancellationToken);

        var traced = await Eventually.IsTrueAsync(
            async () => (await TracesAsync(host.Client, null, cancellationToken)).Count == 1,
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.True(traced, "The run was not traced.");

        using var cleared = await host.Client.PostAsync("/gradio_api/profiling/clear", content: null, cancellationToken);

        Assert.Equal(HttpStatusCode.OK, cleared.StatusCode);
        Assert.Equal(
            """{"status":"cleared"}""",
            await cleared.Content.ReadAsStringAsync(cancellationToken));

        Assert.Empty(await TracesAsync(host.Client, null, cancellationToken));
    }

    [Fact]
    public async Task An_application_that_is_not_profiled_has_no_profiling_routes()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(profiled: false, cancellationToken);

        foreach (var path in new[]
        {
            "/gradio_api/profiling/traces",
            "/gradio_api/profiling/summary",
        })
        {
            using var response = await host.Client.GetAsync(path, cancellationToken);

            // Not registered the Python version, so not found here — rather than the placeholder, which would
            // say "this framework has not built it" about a capability that exists and is off.
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        using var clear = await host.Client.PostAsync(
            "/gradio_api/profiling/clear",
            content: null,
            cancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, clear.StatusCode);
    }

    /// <summary>Starts a host serving an application with one ordinary and one streamed binding.</summary>
    /// <param name="profiled">Whether the application is profiled.</param>
    /// <param name="cancellationToken">Cancels the start.</param>
    /// <param name="uploadFolder">Where uploads are stored, or <see langword="null"/> for the default.</param>
    /// <returns>The host.</returns>
    private static Task<GradioTestHost> StartAsync(
        bool profiled,
        CancellationToken cancellationToken,
        string? uploadFolder = null) =>
        GradioTestHost.StartIntegratedAsync(
            logs: null,
            cancellationToken,
            services => services.AddSingleton(new ProfilingOptions { Enabled = profiled }),
            blocks =>
            {
                if (uploadFolder is not null)
                {
                    blocks.UploadFolder = uploadFolder;
                }

                using (blocks.BeginRender())
                {
                    var input = blocks.Add(new Textbox { Label = "A" });
                    var output = blocks.Add(new Textbox { Label = "Out" });

                    input.Change(
                        (Func<string, string>)(value => value),
                        [input],
                        [output],
                        apiName: "echo");

                    input.Change(
                        (Func<string, IAsyncEnumerable<string>>)(value => Chunks(value)),
                        [input],
                        [output],
                        apiName: "stream");
                }
            });

    /// <summary>A body carrying one file, as the client library sends an upload.</summary>
    /// <param name="name">The name the file is given.</param>
    /// <param name="content">The bytes.</param>
    /// <returns>The body.</returns>
    private static MultipartFormDataContent Multipart(string name, byte[] content)
    {
        var body = new MultipartFormDataContent();
        var part = new ByteArrayContent(content);

        part.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        body.Add(part, "files", name);

        return body;
    }

    /// <summary>A handler that answers in several chunks, so that a run has several iterations.</summary>
    /// <param name="value">What the input carried.</param>
    /// <returns>The chunks.</returns>
    private static async IAsyncEnumerable<string> Chunks(string value)
    {
        for (var index = 0; index < 3; index++)
        {
            await Task.Yield();

            yield return $"{value}{index}";
        }
    }

    /// <summary>Runs a binding and waits until the client has been told it finished.</summary>
    /// <param name="host">The host to run against.</param>
    /// <param name="session">The session to run it in.</param>
    /// <param name="cancellationToken">Cancels the run.</param>
    /// <returns>A task that completes once the run is over.</returns>
    private static async Task RunAsync(
        GradioTestHost host,
        string session,
        CancellationToken cancellationToken)
    {
        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("echo", cancellationToken);

        await queue.JoinAsync(fnIndex, ["hello"], session, cancellationToken);

        await using var stream = await queue.OpenAsync(session, cancellationToken);

        Assert.NotNull(await stream.UntilAsync(
            message => QueueSession.Kind(message) == "process_completed",
            TimeSpan.FromSeconds(15),
            cancellationToken));
    }

    /// <summary>The traces the application has kept.</summary>
    /// <param name="client">The client.</param>
    /// <param name="lastN">How many of the newest to ask for, or <see langword="null"/> for all.</param>
    /// <param name="cancellationToken">Cancels the read.</param>
    /// <returns>The traces.</returns>
    private static async Task<List<JsonObject>> TracesAsync(
        HttpClient client,
        int? lastN,
        CancellationToken cancellationToken)
    {
        var path = lastN is { } count
            ? $"/gradio_api/profiling/traces?last_n={count}"
            : "/gradio_api/profiling/traces";

        var traces = await client.GetFromJsonAsync<JsonArray>(path, cancellationToken);

        return [.. (traces ?? []).Select(entry => entry!.AsObject())];
    }
}

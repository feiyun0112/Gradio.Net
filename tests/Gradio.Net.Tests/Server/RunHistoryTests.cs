using System.Net;
using System.Text.Json.Nodes;
using Gradio.Net;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using File = System.IO.File;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// Run history seen the way a browser sees it: a run is made, and then the routes that read the
/// history are asked what they have.
/// </summary>
/// <remarks>
/// <para>
/// The store is a folder of the test's own, registered before Gradio's own so that the "try add"
/// leaves it in place. That is also the seam an application uses to move its history somewhere else,
/// so these tests exercise the replacement as well as the feature.
/// </para>
/// <para>
/// Recording happens after the answer has been sent, so a test that looked immediately would be
/// asserting about a task's timing rather than about the history: the assertions wait.
/// </para>
/// </remarks>
[Collection(QueueCollection.Name)]
public sealed class RunHistoryTests : IDisposable
{
    private readonly TempFolder _folder = new();

    [Fact]
    public async Task A_queued_run_is_recorded_and_can_be_read_back()
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

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("echo", cancellationToken);

        await queue.JoinAsync(fnIndex, ["hello"], "history", cancellationToken);

        await using var stream = await queue.OpenAsync("history", cancellationToken);
        Assert.NotNull(await stream.UntilAsync(
            message => QueueSession.Kind(message) == "process_completed",
            TimeSpan.FromSeconds(15),
            cancellationToken));

        var record = await WaitForRecordAsync(host.Client, cancellationToken);

        Assert.Equal("echo", record["endpoint"]!.GetValue<string>());
        Assert.Equal("""["hello"]""", record["inputs"]!.ToJsonString());
        Assert.Equal("""["HELLO"]""", record["outputs"]!.ToJsonString());
        Assert.Equal(2, record["schema_version"]!.GetValue<int>());
        Assert.EndsWith("Z", record["started_at"]!.GetValue<string>(), StringComparison.Ordinal);

        // The id is the nanosecond clock in twenty digits and a random tail, which is the Python version's
        // shape: the record's name is what sorts the history, and it is a name other things may
        // parse rather than an opaque string.
        Assert.Matches("^[0-9]{20}[0-9a-f]{8}$", record["record_id"]!.GetValue<string>());
    }

    [Fact]
    public async Task A_private_binding_is_not_recorded()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(
            blocks =>
            {
                var (input, output) = Textboxes(blocks);

                input.Change(
                    (Func<string, string>)(value => value),
                    [input],
                    [output],
                    apiName: "hidden", apiVisibility: ApiVisibility.Private);
            },
            cancellationToken);

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("hidden", cancellationToken);

        await queue.JoinAsync(fnIndex, ["a"], "private", cancellationToken);

        await using var stream = await queue.OpenAsync("private", cancellationToken);
        Assert.NotNull(await stream.UntilAsync(
            message => QueueSession.Kind(message) == "process_completed",
            TimeSpan.FromSeconds(15),
            cancellationToken));

        // A private binding is one the application did not mean to expose, so what it was given and
        // what it answered are not kept.
        Assert.Empty(await RecordsAsync(host.Client, cancellationToken));
    }

    [Fact]
    public async Task An_application_that_keeps_no_history_has_no_page_and_no_records()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(
            blocks =>
            {
                blocks.RunHistory = false;

                var (input, output) = Textboxes(blocks);
                input.Change((Func<string, string>)(value => value), [input], [output]);
            },
            cancellationToken);

        using var page = await host.Client.GetAsync("/gradio_api/runs", cancellationToken);
        using var records = await host.Client.GetAsync("/gradio_api/run-history/records", cancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, page.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, records.StatusCode);
    }

    [Fact]
    public async Task The_page_that_lists_the_runs_is_served()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(blocks => Textboxes(blocks), cancellationToken);

        using var page = await host.Client.GetAsync("/gradio_api/runs", cancellationToken);
        var html = await page.Content.ReadAsStringAsync(cancellationToken);

        Assert.Equal(HttpStatusCode.OK, page.StatusCode);
        Assert.Equal("text/html", page.Content.Headers.ContentType?.MediaType);

        // The page is the application's own document served one level deeper, so its base resolves
        // one level up: with "./" the assets it loads would be asked for under /gradio_api/assets,
        // which no route answers.
        Assert.Contains("<base href=\"../\"", html, StringComparison.Ordinal);
    }

    [Fact]
    public async Task A_bucket_can_be_connected_and_listed()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(blocks => Textboxes(blocks), cancellationToken);

        using var body = new StringContent("""{"bucket_id":"mine"}""", System.Text.Encoding.UTF8, "application/json");
        using var connected = await host.Client.PostAsync("/gradio_api/run-history/connect", body, cancellationToken);

        Assert.Equal(HttpStatusCode.OK, connected.StatusCode);
        Assert.Equal("""{"ok":true}""", await connected.Content.ReadAsStringAsync(cancellationToken));

        using var listed = await host.Client.GetAsync("/gradio_api/run-history/buckets", cancellationToken);
        var buckets = JsonNode.Parse(await listed.Content.ReadAsStringAsync(cancellationToken))!;

        Assert.Equal("""["mine"]""", buckets["buckets"]!.ToJsonString());
    }

    [Fact]
    public async Task A_bucket_that_is_not_a_name_is_refused()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(blocks => Textboxes(blocks), cancellationToken);

        using var body = new StringContent("""{"bucket_id":"../elsewhere"}""", System.Text.Encoding.UTF8, "application/json");
        using var refused = await host.Client.PostAsync("/gradio_api/run-history/connect", body, cancellationToken);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, refused.StatusCode);
    }

    [Fact]
    public async Task A_file_a_run_produced_travels_with_its_record()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        // An extension in capitals, because the asset is named after the file it holds and the Python version
        // keeps the name as it was written rather than lowering it.
        var produced = _folder.Write("produced.TXT", "the bytes"u8);

        await using var host = await StartAsync(
            blocks =>
            {
                blocks.UploadFolder = _folder.Folder;

                using (blocks.BeginRender())
                {
                    var input = blocks.Add(new Textbox { Label = "A" });
                    var output = blocks.Add(new Image { Label = "Picture" });

                    input.Change(
                        (Func<string, string>)(_ => produced),
                        [input],
                        [output],
                        apiName: "picture");
                }
            },
            cancellationToken);

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("picture", cancellationToken);

        await queue.JoinAsync(fnIndex, ["go"], "asset", cancellationToken);

        await using var stream = await queue.OpenAsync("asset", cancellationToken);
        Assert.NotNull(await stream.UntilAsync(
            message => QueueSession.Kind(message) == "process_completed",
            TimeSpan.FromSeconds(15),
            cancellationToken));

        var record = await WaitForRecordAsync(host.Client, cancellationToken);

        // The record refers to the file by a name rather than by a path, because the file travels
        // with the record: a path would stop working as soon as the file did. The number counts the
        // assets of one record, and the extension is the file's own.
        var reference = record["outputs"]!.AsArray()[0]!["__asset__"]!.GetValue<string>();

        Assert.Equal("a001.TXT", reference);

        using var asset = await host.Client.GetAsync(
            $"/gradio_api/run-history/records/picture/{record["record_id"]!.GetValue<string>()}/assets/{reference}",
            cancellationToken);

        Assert.Equal(HttpStatusCode.OK, asset.StatusCode);
        Assert.Equal("the bytes", await asset.Content.ReadAsStringAsync(cancellationToken));
    }

    /// <summary>Starts a host whose history is kept in this test's folder.</summary>
    /// <param name="build">Writes the application.</param>
    /// <param name="cancellationToken">Cancels the start.</param>
    /// <returns>The host.</returns>
    private Task<GradioTestHost> StartAsync(Action<Blocks> build, CancellationToken cancellationToken) =>
        GradioTestHost.StartIntegratedAsync(
            logs: null,
            cancellationToken,
            services => services.AddSingleton<IRunHistoryStore>(new FileRunHistoryStore(_folder.Folder)),
            build);

    /// <summary>Waits for the run to be recorded, and answers with the record.</summary>
    /// <param name="client">The client.</param>
    /// <param name="cancellationToken">Cancels the wait.</param>
    /// <returns>The record.</returns>
    private static async Task<JsonObject> WaitForRecordAsync(HttpClient client, CancellationToken cancellationToken)
    {
        JsonObject? record = null;

        var recorded = await Eventually.IsTrueAsync(
            async () =>
            {
                var records = await RecordsAsync(client, cancellationToken);
                record = records.FirstOrDefault();

                return record is not null;
            },
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.True(recorded, "The run was not recorded.");

        return record!;
    }

    /// <summary>The records the application's history holds.</summary>
    /// <param name="client">The client.</param>
    /// <param name="cancellationToken">Cancels the read.</param>
    /// <returns>The records.</returns>
    private static async Task<List<JsonObject>> RecordsAsync(HttpClient client, CancellationToken cancellationToken)
    {
        using var response = await client.GetAsync("/gradio_api/run-history/records", cancellationToken);

        response.EnsureSuccessStatusCode();

        var body = JsonNode.Parse(await response.Content.ReadAsStringAsync(cancellationToken))!;

        return [.. body["records"]!.AsArray().Select(entry => entry!.AsObject())];
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

    public void Dispose() => _folder.Dispose();
}

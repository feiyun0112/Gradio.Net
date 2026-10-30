using System.Net;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;
using MediaFile = Gradio.Net.File;
namespace Gradio.Net.Tests.Server;

/// <summary>
/// The whole loop as a browser and a handler see it: a file is uploaded, an event handler reads it,
/// and what the handler produces is served back.
/// </summary>
/// <remarks>
/// The three endpoints are already asserted one by one — the upload by <see cref="FileEndpointTests"/>,
/// the run by the queue's tests, the serving by the same file tests. What only an end-to-end test
/// can show is that the three agree on one file: that the path an upload answers with is a path a
/// handler can read, and that the file a handler produces is at the URL the client is given.
/// </remarks>
public sealed class MediaLoopTests
{
    [Fact]
    public async Task An_uploaded_file_is_read_by_a_handler_and_the_result_is_served_back()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        using var scratch = new TempFolder();

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            blocks.UploadFolder = folder.Folder;

            using (blocks.BeginRender())
            {
                var input = blocks.Add(new MediaFile { Label = "In" });
                var output = blocks.Add(new MediaFile { Label = "Out" });

                // The handler reads the file it is handed and writes what it read into a file of
                // its own, which is the shape a real one takes: the path it was given is a real
                // file, and the path it returns is one the client has never seen.
                input.Upload(
                    (Func<string, string>)(path =>
                    {
                        var text = System.IO.File.ReadAllText(path);
                        var produced = Path.Combine(scratch.Folder, "reply.txt");
                        System.IO.File.WriteAllText(produced, text.ToUpperInvariant());

                        return produced;
                    }),
                    [input],
                    [output],
                    apiName: "echo");
            }
        });

        var uploaded = await UploadAsync(host, "note.txt", "round trip", cancellationToken);

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("echo", cancellationToken);
        var eventId = await queue.JoinAsync(
            fnIndex,
            [new JsonObject
            {
                ["path"] = uploaded,
                ["meta"] = new JsonObject { ["_type"] = FileData.FileMetaType },
                ["orig_name"] = "note.txt",
            }],
            "media",
            cancellationToken);

        await using var stream = await queue.OpenAsync("media", cancellationToken);

        var completed = await stream.UntilAsync(
            message => QueueSession.Kind(message) == "process_completed"
                && message["event_id"]?.GetValue<string>() == eventId,
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.NotNull(completed);
        Assert.True(completed["success"]!.GetValue<bool>());

        // What the client is given is a file object, and its URL is the address the file endpoint
        // answers — which is what makes the result displayable at all.
        var file = Assert.IsType<JsonObject>(completed["output"]!["data"]!.AsArray()[0]);
        var url = file["url"]!.GetValue<string>();
        Assert.Equal("reply.txt", file["orig_name"]!.GetValue<string>());
        Assert.StartsWith("/gradio_api/file=", url, StringComparison.Ordinal);

        using var served = await host.Client.GetAsync(url, cancellationToken);

        Assert.Equal(HttpStatusCode.OK, served.StatusCode);
        Assert.Equal("ROUND TRIP", await served.Content.ReadAsStringAsync(cancellationToken));
    }

    [Fact]
    public async Task A_handler_that_reads_a_file_of_type_binary_gets_the_bytes_the_client_sent()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            blocks.UploadFolder = folder.Folder;

            using (blocks.BeginRender())
            {
                var input = blocks.Add(new MediaFile { Label = "In", Type = "binary" });
                var output = blocks.Add(new Textbox { Label = "Out" });

                input.Change(
                    (Func<byte[], string>)(bytes => string.Join(" ", bytes)),
                    [input],
                    [output],
                    apiName: "describe");
            }
        });

        var uploaded = await UploadAsync(host, "bytes.bin", "abc", cancellationToken);

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("describe", cancellationToken);
        await queue.JoinAsync(
            fnIndex,
            [new JsonObject { ["path"] = uploaded }],
            "binary",
            cancellationToken);

        await using var stream = await queue.OpenAsync("binary", cancellationToken);

        var completed = await stream.UntilAsync(
            message => QueueSession.Kind(message) == "process_completed",
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.NotNull(completed);
        Assert.Equal("97 98 99", completed["output"]!["data"]!.AsArray()[0]!.GetValue<string>());
    }

    /// <summary>Uploads a file the way the frontend does, and returns the path the upload answered with.</summary>
    /// <param name="host">The host to upload to.</param>
    /// <param name="name">The name the client gives the file.</param>
    /// <param name="content">The file's content.</param>
    /// <param name="cancellationToken">Cancels the upload.</param>
    /// <returns>The stored path.</returns>
    private static async Task<string> UploadAsync(
        GradioTestHost host,
        string name,
        string content,
        CancellationToken cancellationToken)
    {
        var part = new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes(content));
        part.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        using var body = new MultipartFormDataContent { { part, "files", name } };
        using var response = await host.Client.PostAsync("/gradio_api/upload", body, cancellationToken);

        response.EnsureSuccessStatusCode();

        var paths = JsonNode.Parse(await response.Content.ReadAsStringAsync(cancellationToken))!.AsArray();

        return Assert.Single(paths)!.GetValue<string>();
    }
}

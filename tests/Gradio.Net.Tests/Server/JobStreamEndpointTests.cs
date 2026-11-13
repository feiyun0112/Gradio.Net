using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The job-style routes as a client reads them: submit, then read the result stream.
/// </summary>
/// <remarks>
/// The Python version's <c>GET /call/{api_name}/{event_id}</c> writes named server-sent events and
/// its OpenAPI document says the final one is <c>event: complete</c>. This route used to forward
/// the queue's own messages instead, so a client that waited for that event waited until the
/// connection closed. What is asserted here is the framing on the wire, not the queue's shape.
/// </remarks>
public sealed class JobStreamEndpointTests
{
    [Fact]
    public async Task A_job_style_call_streams_a_named_complete_event_with_only_the_outputs()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            cancellationToken,
            blocks => Build(blocks));

        var eventId = await SubmitAsync(host, "/gradio_api/call/add", """{"a":2,"b":3}""", cancellationToken);
        var stream = await ReadAsync(host, $"/gradio_api/call/add/{eventId}", cancellationToken);

        Assert.Equal("text/event-stream", stream.ContentType);
        Assert.Contains("event: complete\n", stream.Text, StringComparison.Ordinal);
        Assert.Contains("data: [5]\n\n", stream.Text, StringComparison.Ordinal);

        // Not a single queue message leaked onto this connection: no `msg` field, no estimates and
        // no close_stream, which the Python version's process_msg skips as well.
        Assert.DoesNotContain("\"msg\"", stream.Text, StringComparison.Ordinal);
        Assert.DoesNotContain("close_stream", stream.Text, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Both_call_spellings_answer_with_the_same_named_stream()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            cancellationToken,
            blocks => Build(blocks));

        var eventId = await SubmitAsync(host, "/gradio_api/call/v2/add", """{"data":[4,5]}""", cancellationToken);
        var stream = await ReadAsync(host, $"/gradio_api/call/v2/add/{eventId}", cancellationToken);

        Assert.Contains("event: complete\ndata: [9]", stream.Text, StringComparison.Ordinal);
        Assert.DoesNotContain("\"msg\"", stream.Text, StringComparison.Ordinal);
    }

    [Fact]
    public async Task A_streaming_call_writes_generating_events_before_the_complete_one()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            cancellationToken,
            blocks => Build(blocks));

        var eventId = await SubmitAsync(host, "/gradio_api/call/chunks", """{"data":["abc"]}""", cancellationToken);
        var stream = await ReadAsync(host, $"/gradio_api/call/chunks/{eventId}", cancellationToken);

        Assert.Contains("event: generating\n", stream.Text, StringComparison.Ordinal);
        Assert.Contains("event: complete\n", stream.Text, StringComparison.Ordinal);
        Assert.True(
            stream.Text.IndexOf("event: generating", StringComparison.Ordinal)
            < stream.Text.IndexOf("event: complete", StringComparison.Ordinal));
        Assert.DoesNotContain("\"msg\"", stream.Text, StringComparison.Ordinal);
    }

    [Fact]
    public async Task An_identifier_the_route_never_issued_is_a_named_error()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            cancellationToken,
            blocks => Build(blocks));

        var stream = await ReadAsync(host, "/gradio_api/call/add/not-an-event", cancellationToken);

        Assert.Equal("text/event-stream", stream.ContentType);
        Assert.Equal("event: error\ndata: \"404: Not Found\"\n\n", stream.Text);
    }

    /// <summary>Writes an application with one plain endpoint and one streaming endpoint.</summary>
    /// <param name="blocks">The application to write into.</param>
    private static void Build(Blocks blocks)
    {
        Textbox input;
        Textbox output;

        using (blocks.BeginRender())
        {
            blocks.Api(new Func<int, int, int>(Add), apiName: "add");

            input = blocks.Add(new Textbox { Label = "Input" });
            output = blocks.Add(new Textbox { Label = "Output" });
        }

        input.Change(
            (Func<string, IAsyncEnumerable<string>>)Chunks,
            [input],
            [output],
            apiName: "chunks");
    }

    /// <summary>Adds two numbers.</summary>
    /// <param name="a">The first.</param>
    /// <param name="b">The second.</param>
    /// <returns>The sum.</returns>
    private static int Add(int a, int b) => a + b;

    /// <summary>Produces two chunks of a stream.</summary>
    /// <param name="value">The input.</param>
    /// <returns>The chunks.</returns>
    private static async IAsyncEnumerable<string> Chunks(string value)
    {
        await Task.Yield();

        yield return value[..1];
        yield return value;
    }

    /// <summary>Submits a run and returns the identifier its result is read by.</summary>
    /// <param name="host">The host.</param>
    /// <param name="path">The submission path.</param>
    /// <param name="body">The request body.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    /// <returns>The event id.</returns>
    private static async Task<string> SubmitAsync(
        GradioTestHost host,
        string path,
        string body,
        CancellationToken cancellationToken)
    {
        using var content = new StringContent(body, Encoding.UTF8, "application/json");
        using var response = await host.Client.PostAsync(path, content, cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var answer = JsonNode.Parse(await response.Content.ReadAsStringAsync(cancellationToken))!;

        return answer["event_id"]!.GetValue<string>();
    }

    /// <summary>Reads a result stream to its end.</summary>
    /// <param name="host">The host.</param>
    /// <param name="path">The result path.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    /// <returns>The stream's content type and text.</returns>
    private static async Task<(string ContentType, string Text)> ReadAsync(
        GradioTestHost host,
        string path,
        CancellationToken cancellationToken)
    {
        using var response = await host.Client.GetAsync(
            path,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        return (
            response.Content.Headers.ContentType?.MediaType ?? string.Empty,
            await response.Content.ReadAsStringAsync(cancellationToken));
    }
}

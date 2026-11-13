using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The reason a validator gives for refusing a run, as a client reads it.
/// </summary>
/// <remarks>
/// A string a validator answers with is the message, not a JSON value to be displayed. It used to
/// be serialized on its way into <c>detail</c>, so the client received a quoted, escaped JSON
/// literal — <c>\"\u8fd9\u91cc...\"</c> — instead of the sentence. The Python version's
/// <c>gr.validate()</c> carries the same reason in its <c>message</c> field as plain text.
/// </remarks>
public sealed class ValidatorMessageTests
{
    [Fact]
    public async Task A_validator_that_answers_with_a_string_refuses_with_that_text()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            cancellationToken,
            blocks =>
            {
                using (blocks.BeginRender())
                {
                    blocks.Api(
                        new Func<string, string>(Echo),
                        apiName: "echo",
                        validator: (Func<string, string?>)(value => value == "ok" ? null : "这里只收 ok"));
                }
            });

        using var content = new StringContent("""{"data":["bad"]}""", Encoding.UTF8, "application/json");
        using var response = await host.Client.PostAsync("/gradio_api/call/echo", content, cancellationToken);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        var answer = JsonNode.Parse(await response.Content.ReadAsStringAsync(cancellationToken))!;
        var detail = answer["detail"]!.GetValue<string>();

        Assert.Equal("这里只收 ok", detail);
        Assert.DoesNotContain("\\u", detail, StringComparison.Ordinal);
        Assert.False(detail.StartsWith('"'));
    }

    [Fact]
    public async Task A_validator_that_accepts_lets_the_run_through()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            cancellationToken,
            blocks =>
            {
                using (blocks.BeginRender())
                {
                    blocks.Api(
                        new Func<string, string>(Echo),
                        apiName: "echo",
                        validator: (Func<string, string?>)(value => value == "ok" ? null : "这里只收 ok"));
                }
            });

        using var content = new StringContent("""{"data":["ok"]}""", Encoding.UTF8, "application/json");
        using var response = await host.Client.PostAsync("/gradio_api/call/echo", content, cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(JsonNode.Parse(await response.Content.ReadAsStringAsync(cancellationToken))!["event_id"]);
    }

    /// <summary>Answers with what it was handed.</summary>
    /// <param name="value">The input.</param>
    /// <returns>The input.</returns>
    private static string Echo(string value) => value;
}

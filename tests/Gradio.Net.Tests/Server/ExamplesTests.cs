using System.Net;
using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// What clicking a row of the example gallery hands each of the application's inputs.
/// </summary>
/// <remarks>
/// The gallery renders correctly no matter how many inputs there are — the table is built from the
/// same samples the click reads — so the only way to tell a broken click from a working one is to
/// run its endpoint and look at the values it sends back.
/// </remarks>
public sealed class ExamplesTests : IDisposable
{
    private readonly string _folder = Path.Combine(Path.GetTempPath(), $"gradio-examples-{Guid.NewGuid():N}");

    /// <inheritdoc />
    public void Dispose()
    {
        if (Directory.Exists(_folder))
        {
            Directory.Delete(_folder, recursive: true);
        }
    }

    [Fact]
    public async Task Clicking_an_example_fills_every_input_with_its_own_value()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks =>
            {
                blocks.UploadFolder = _folder;

                _ = new Interface(
                    blocks,
                    (Func<string, string, string>)((text, separator) => text.Replace(
                        separator, "\n", StringComparison.Ordinal)),
                    [new Textbox { Label = "text" }, new Textbox { Label = "separator" }],
                    [new Textbox { Label = "result" }],
                    examples: new object?[]
                    {
                        new object?[] { "a,b,c", "," },
                        new object?[] { "a;b;c", ";" },
                    },
                    preloadExample: null);
            });

        using var response = await host.Client.PostAsync(
            "/gradio_api/run/load_example",
            new StringContent(
                new JsonObject { ["data"] = new JsonArray(1) }.ToJsonString(),
                System.Text.Encoding.UTF8,
                "application/json"),
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = JsonNode.Parse(
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))!;
        var data = body["data"]!.AsArray();

        // One update per input, each carrying that input's own cell — not one value holding the row.
        Assert.Equal(2, data.Count);
        Assert.True(data[0] is JsonObject, body.ToJsonString());
        Assert.True(data[1] is JsonObject, body.ToJsonString());
        Assert.Equal("a;b;c", data[0]!["value"]!.GetValue<string>());
        Assert.Equal(";", data[1]!["value"]!.GetValue<string>());
    }

    [Fact]
    public async Task Clicking_an_example_with_one_input_still_fills_that_input()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            blocks =>
            {
                blocks.UploadFolder = _folder;

                _ = new Interface(
                    blocks,
                    (Func<string, string>)(value => value.ToUpperInvariant()),
                    [new Textbox { Label = "text" }],
                    [new Textbox { Label = "result" }],
                    examples: new object?[] { new object?[] { "a,b,c" } },
                    preloadExample: null);
            });

        using var response = await host.Client.PostAsync(
            "/gradio_api/run/load_example",
            new StringContent(
                new JsonObject { ["data"] = new JsonArray(0) }.ToJsonString(),
                System.Text.Encoding.UTF8,
                "application/json"),
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = JsonNode.Parse(
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))!;
        var data = body["data"]!.AsArray();

        Assert.Single(data);
        Assert.Equal("a,b,c", data[0]!["value"]!.GetValue<string>());
    }
}

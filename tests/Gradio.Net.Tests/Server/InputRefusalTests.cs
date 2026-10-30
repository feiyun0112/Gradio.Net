using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// A value a component refuses fails the run before the handler is called — which is where the
/// Python version refuses one, and what a user sees as an error rather than as a wrong answer.
/// </summary>
/// <remarks>
/// <para>
/// The refusals themselves are pure computations and are asserted, message included, where the
/// component lives — <c>InputFamilyTests</c> for the fields that take a value,
/// <c>ButtonFamilyTests</c> for the upload button's file. What is asserted here is the place they
/// happen at: the run is what fails, the handler never runs, and the client is told the run failed
/// rather than handed a value that was never valid. The Python version does the same, in the
/// preprocessing step of the run, which is why a value a component rejects cannot reach a handler
/// on either side.
/// </para>
/// <para>
/// These drive the queue the way a client does, so they share its collection.
/// </para>
/// </remarks>
[Collection(QueueCollection.Name)]
public sealed class InputRefusalTests
{
    [Fact]
    public async Task A_number_outside_its_range_fails_the_run_without_calling_the_handler()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var logs = new CapturingLoggerProvider();
        var calls = 0;

        await using var host = await GradioTestHost.StartStandaloneAsync(logs, cancellationToken, blocks =>
        {
            Number field;
            Textbox output;

            using (blocks.BeginRender())
            {
                field = blocks.Add(new Number { Label = "Score", Minimum = 0, Maximum = 10 });
                output = blocks.Add(new Textbox { Label = "Out" });
            }

            field.Change(
                (Func<double, object?>)(value =>
                {
                    Interlocked.Increment(ref calls);

                    return value;
                }),
                [field],
                [output],
                apiName: "score");
        });

        var completed = await RunAsync(host, "score", [11], cancellationToken);

        Assert.False(completed["success"]!.GetValue<bool>());
        Assert.Equal(0, calls);

        // And it was refused for that reason rather than for some other: the log carries the message
        // the Python version raises, which is what an operator reads afterwards.
        var failure = logs.Entries.Select(entry => entry.Exception).OfType<Exception>().FirstOrDefault();

        Assert.NotNull(failure);
        Assert.StartsWith(
            "Value 11 is greater than maximum value 10.",
            failure.Message,
            StringComparison.Ordinal);
    }

    [Fact]
    public async Task A_value_the_list_does_not_offer_fails_the_run_without_calling_the_handler()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var calls = 0;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            Dropdown field;
            Textbox output;

            using (blocks.BeginRender())
            {
                field = blocks.Add(new Dropdown
                {
                    Label = "Size",
                    Choices = [new Choice("small"), new Choice("large")],
                });
                output = blocks.Add(new Textbox { Label = "Out" });
            }

            field.Change(
                (Func<object?, object?>)(value =>
                {
                    Interlocked.Increment(ref calls);

                    return value;
                }),
                [field],
                [output],
                apiName: "size");
        });

        var completed = await RunAsync(host, "size", ["medium"], cancellationToken);

        Assert.False(completed["success"]!.GetValue<bool>());
        Assert.Equal(0, calls);
    }

    [Fact]
    public async Task A_file_the_upload_button_does_not_accept_fails_the_run()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var calls = 0;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            UploadButton field;
            Textbox output;

            using (blocks.BeginRender())
            {
                field = blocks.Add(new UploadButton { Label = "Upload", FileTypes = [".txt"] });
                output = blocks.Add(new Textbox { Label = "Out" });
            }

            field.Upload(
                (Func<string, object?>)(path =>
                {
                    Interlocked.Increment(ref calls);

                    return Path.GetFileName(path);
                }),
                [field],
                [output],
                apiName: "uploaded");
        });

        // A client is not obliged to honour the kinds the field offered, so the file it sends is
        // refused on the way in — the same place a value a field will not take is refused.
        var completed = await RunAsync(
            host,
            "uploaded",
            [new JsonObject { ["path"] = "/tmp/notes.bin" }],
            cancellationToken);

        Assert.False(completed["success"]!.GetValue<bool>());
        Assert.Equal(0, calls);
    }

    /// <summary>Runs a binding once over the queue and returns the message that ended it.</summary>
    /// <param name="host">The host serving the application.</param>
    /// <param name="apiName">The binding to run.</param>
    /// <param name="data">The values the client sends.</param>
    /// <param name="cancellationToken">Cancels the test.</param>
    /// <returns>The completion message, successful or not.</returns>
    private static async Task<JsonObject> RunAsync(
        GradioTestHost host,
        string apiName,
        JsonArray data,
        CancellationToken cancellationToken)
    {
        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync(apiName, cancellationToken);
        var eventId = await queue.JoinAsync(fnIndex, data, "refused", cancellationToken);

        await using var stream = await queue.OpenAsync("refused", cancellationToken);

        var completed = await stream.UntilAsync(
            message => QueueSession.Kind(message) == "process_completed"
                && message["event_id"]?.GetValue<string>() == eventId,
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.NotNull(completed);

        return completed;
    }
}

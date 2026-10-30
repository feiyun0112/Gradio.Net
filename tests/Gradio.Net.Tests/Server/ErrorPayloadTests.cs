using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using Gradio.Net;
using Gradio.Net.Tests.Kernel;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// What a failed run tells the client: nothing, the message the application meant for the user, or
/// the text of a crash.
/// </summary>
/// <remarks>
/// <para>
/// The two kinds of failure are the whole of this file. A handler that raises <see cref="Error"/>
/// is the application saying "no", and the message is shown whether or not the application sends
/// error text — the Python version's <c>show_error or isinstance(error, AppError)</c>. A handler
/// that falls over is a crash, and its text goes to the client only when
/// <see cref="Blocks.ShowError"/> says so.
/// </para>
/// <para>
/// Both routes are exercised because both build the payload: the direct route answers with it, and
/// the queue puts it in the completion message. They are two places, and a rule implemented in one
/// of them is a rule half the clients see.
/// </para>
/// </remarks>
public sealed class ErrorPayloadTests
{
    [Fact]
    public async Task A_refusal_is_shown_without_the_application_asking_for_it()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(blocks =>
        {
            var (input, output) = AddTextboxes(blocks);

            input.Change(
                (Func<string, string>)Refuse,
                [input],
                [output],
                apiName: "refuse", queue: false);
        });

        // The Python version's four values, in the Python version's order, with the defaults a refusal raised with no
        // arguments carries.
        Assert.Equal(
            "{\"error\":\"a name is required\",\"duration\":10,\"visible\":true,\"title\":\"Error\"}",
            await FailDirectlyAsync(host, "refuse", cancellationToken));
    }

    [Fact]
    public async Task A_refusal_carries_the_four_values_it_was_raised_with()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(blocks =>
        {
            var (input, output) = AddTextboxes(blocks);

            input.Change(
                (Func<string, string>)RefuseLoudly,
                [input],
                [output],
                apiName: "refuse", queue: false);
        });

        // A duration of null is written rather than left out: the frontend reads the key to decide
        // whether the modal closes by itself.
        Assert.Equal(
            "{\"error\":\"<b>no</b>\",\"duration\":null,\"visible\":false,\"title\":\"Not so fast\"}",
            await FailDirectlyAsync(host, "refuse", cancellationToken));
    }

    [Fact]
    public async Task A_crash_says_nothing_unless_the_application_sends_error_text()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(blocks =>
        {
            var (input, output) = AddTextboxes(blocks);

            input.Change(
                (Func<string, string>)Crash,
                [input],
                [output],
                apiName: "crash", queue: false);
        });

        // The message is in the log, and the client is told that something failed and nothing more.
        Assert.Equal("{\"error\":null}", await FailDirectlyAsync(host, "crash", cancellationToken));
    }

    [Fact]
    public async Task A_crash_carries_its_message_when_the_application_sends_error_text()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(blocks =>
        {
            blocks.ShowError = true;

            var (input, output) = AddTextboxes(blocks);

            input.Change(
                (Func<string, string>)Crash,
                [input],
                [output],
                apiName: "crash", queue: false);
        });

        // `visible` says the text may be drawn, which is what the Python version sends for a crash the
        // application opted into showing.
        Assert.Equal(
            "{\"error\":\"the handler broke\",\"visible\":true}",
            await FailDirectlyAsync(host, "crash", cancellationToken));
    }

    [Fact]
    public async Task The_queue_puts_the_same_payload_in_its_completion()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(blocks =>
        {
            var (input, output) = AddTextboxes(blocks);

            input.Change(
                (Func<string, string>)Refuse,
                [input],
                [output],
                apiName: "refuse");
        });

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("refuse", cancellationToken);
        var eventId = await queue.JoinAsync(fnIndex, ["a"], "errors", cancellationToken);

        await using var stream = await queue.OpenAsync("errors", cancellationToken);

        var completed = await stream.UntilAsync(
            message => QueueSession.Kind(message) == "process_completed"
                && message["event_id"]?.GetValue<string>() == eventId,
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.NotNull(completed);
        Assert.False(completed["success"]!.GetValue<bool>());
        Assert.Equal(
            "{\"error\":\"a name is required\",\"duration\":10,\"visible\":true,\"title\":\"Error\"}",
            completed["output"]!.ToJsonString());
    }

    [Fact]
    public async Task A_refusal_that_said_not_to_be_printed_is_not_logged()
    {
        var logs = new CapturingLoggerProvider();
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(
            blocks =>
            {
                var (input, output) = AddTextboxes(blocks);

                input.Change(
                    (Func<string, string>)RefuseQuietly,
                    [input],
                    [output],
                    apiName: "quiet", queue: false);
            },
            logs);

        await FailDirectlyAsync(host, "quiet", cancellationToken);

        // The user was told; an operator does not need a traceback for every visitor who typed the
        // wrong thing. The Python version's rule, and the reason the flag exists.
        Assert.DoesNotContain(logs.Entries, entry => entry.EventId.Name == "GradioDirectRunFailed");
    }

    [Fact]
    public async Task A_crash_is_logged()
    {
        var logs = new CapturingLoggerProvider();
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await StartAsync(
            blocks =>
            {
                var (input, output) = AddTextboxes(blocks);

                input.Change(
                    (Func<string, string>)Crash,
                    [input],
                    [output],
                    apiName: "crash", queue: false);
            },
            logs);

        await FailDirectlyAsync(host, "crash", cancellationToken);

        Assert.Contains(logs.Entries, entry => entry.EventId.Name == "GradioDirectRunFailed");
    }

    [Fact]
    public void The_switch_is_off_and_carried_by_the_config()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();

        Assert.False(blocks.ShowError);
        Assert.False(blocks.GetConfig()["show_error"]!.GetValue<bool>());

        blocks.ShowError = true;

        Assert.True(blocks.GetConfig()["show_error"]!.GetValue<bool>());
    }

    [Fact]
    public void Every_failure_the_framework_raises_on_purpose_shares_a_base()
    {
        // The distinction a host can act on: the framework refused and said why, or something went
        // wrong. `Error` is the first kind, and the types the framework already raised are under
        // the same base so that one clause catches them.
        Assert.True(typeof(GradioException).IsAssignableFrom(typeof(Error)));
        Assert.True(typeof(GradioException).IsAssignableFrom(typeof(WorkflowExecutionException)));
        Assert.True(typeof(GradioException).IsAssignableFrom(typeof(RunHistoryException)));
        Assert.True(typeof(GradioException).IsAssignableFrom(typeof(OAuthProviderException)));

        // And a crash is not one of them, which is what makes the clause useful.
        Assert.False(typeof(GradioException).IsAssignableFrom(typeof(InvalidOperationException)));
    }

    /// <summary>Starts a host whose application is written by <paramref name="build"/>.</summary>
    /// <param name="build">Writes the application.</param>
    /// <param name="logs">Where the host's log goes, or <see langword="null"/> for nowhere.</param>
    /// <returns>The started host.</returns>
    private static Task<GradioTestHost> StartAsync(Action<Blocks> build, CapturingLoggerProvider? logs = null) =>
        GradioTestHost.StartStandaloneAsync(logs, TestContext.Current.CancellationToken, build);

    /// <summary>Adds the two textboxes these applications are made of.</summary>
    /// <param name="blocks">The application to write into.</param>
    /// <returns>The two textboxes.</returns>
    private static (Textbox Input, Textbox Output) AddTextboxes(Blocks blocks)
    {
        Textbox input;
        Textbox output;

        using (blocks.BeginRender())
        {
            input = blocks.Add(new Textbox { Label = "A" });
            output = blocks.Add(new Textbox { Label = "B" });
        }

        return (input, output);
    }

    /// <summary>Calls a binding directly, asserts that it failed, and reads what the failure said.</summary>
    /// <param name="host">The host serving it.</param>
    /// <param name="apiName">The binding to call.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    /// <returns>The answer's body, as text.</returns>
    private static async Task<string> FailDirectlyAsync(
        GradioTestHost host,
        string apiName,
        CancellationToken cancellationToken)
    {
        using var content = new StringContent("{\"data\":[\"a\"]}", Encoding.UTF8, "application/json");
        using var response = await host.Client.PostAsync($"/gradio_api/run/{apiName}", content, cancellationToken);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    private static string Refuse(string value) => throw new Error("a name is required");

    private static string RefuseLoudly(string value) =>
        throw new Error("<b>no</b>", duration: null, visible: false, title: "Not so fast");

    private static string RefuseQuietly(string value) =>
        throw new Error("a name is required", printException: false);

    private static string Crash(string value) => throw new InvalidOperationException("the handler broke");
}

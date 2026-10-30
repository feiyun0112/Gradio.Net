using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// A component whose value is computed rather than written: the load event it binds, and the value
/// that arrives when the event runs.
/// </summary>
/// <remarks>
/// <para>
/// The Python version calls a component's value when the value is a function, on each browser
/// session and again whenever the components it reads change. That is a binding like any other,
/// made once the application has been written — so a document comparison is what shows its shape,
/// and a run is what shows that the value really lands in the component.
/// </para>
/// <para>
/// The page's own signal is dispatched by the frontend, which <see cref="BrowserEndToEndTests"/>
/// proves for the application's own <c>load</c>; what is asserted here is the dependency the
/// component contributes and what a run of it produces.
/// </para>
/// </remarks>
public sealed class LoadEventTests
{
    [Fact]
    public async Task A_computed_value_binds_the_page_load_and_the_changes_it_reads()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            using (blocks.BeginRender())
            {
                var name = blocks.Add(new Textbox { Label = "Name" });
                var count = blocks.Add(new Number { Label = "Count" });

                blocks.Add(new Textbox
                {
                    Label = "Greeting",
                    LoadFn = new LoadFunction(
                        (Func<string?, double?, string>)((who, times) =>
                            $"{who} ×{(int)(times ?? 0)}"),
                        "greeting"),
                    Inputs = [name, count],
                });
            }
        });

        var config = await ConfigDocument.ReadAsync(host, "/config", cancellationToken);
        var dependency = Assert.Single(Assert.IsType<JsonArray>(config["dependencies"]));

        // Both of the components it reads are triggers as well as inputs: the value is recomputed
        // when either changes, and the function's parameters are filled from them in that order.
        Assert.Equal(["1:change", "2:change", "0:load"], ConfigDocument.Targets(dependency));
        Assert.Equal([1, 2], ConfigDocument.Ids(dependency!["inputs"]));
        Assert.Equal([3], ConfigDocument.Ids(dependency!["outputs"]));

        // A value recomputed whenever a field changes shows no spinner, which is the Python version's rule:
        // one repainted on every keystroke would be a flicker rather than progress.
        Assert.Equal("hidden", dependency!["show_progress"]!.GetValue<string>());
        Assert.Equal("greeting", dependency["api_name"]!.GetValue<string>());
    }

    [Fact]
    public async Task A_value_computed_from_nothing_shows_the_whole_progress()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            using (blocks.BeginRender())
            {
                blocks.Add(new Textbox
                {
                    Label = "Greeting",
                    LoadFn = new LoadFunction((Func<string>)(() => "hello"), "greeting"),
                });
            }
        });

        var config = await ConfigDocument.ReadAsync(host, "/config", cancellationToken);
        var dependency = Assert.Single(Assert.IsType<JsonArray>(config["dependencies"]));

        // The page's opening is the only trigger there is, so the run is the user's whole wait and
        // the animation is shown — the other half of the Python version's rule.
        Assert.Equal(["0:load"], ConfigDocument.Targets(dependency));
        Assert.Empty(Assert.IsType<JsonArray>(dependency!["inputs"]));
        Assert.Equal("full", dependency!["show_progress"]!.GetValue<string>());
    }

    [Fact]
    public async Task The_computed_value_reaches_the_component_when_the_event_runs()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            using (blocks.BeginRender())
            {
                var name = blocks.Add(new Textbox { Label = "Name" });

                blocks.Add(new Textbox
                {
                    Label = "Greeting",
                    LoadFn = new LoadFunction(
                        (Func<string?, string>)(who => $"hello {who}"),
                        "greeting"),
                    Inputs = [name],
                });
            }
        });

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("greeting", cancellationToken);

        // What the client sends is what the handler's parameters are filled from, which is the
        // same path a change trigger takes: the value is read, computed and written back.
        Assert.Equal("hello Ada", await RunAsync(queue, fnIndex, ["Ada"], cancellationToken));
    }

    /// <summary>
    /// A computed value whose fields are written after it binds when they are written.
    /// </summary>
    /// <remarks>
    /// An application can be written in more than one go — leaving a <c>using</c> block and writing
    /// more is what that looks like — so a component can name a field that has no id yet. The
    /// render that ends in between has nothing to bind, and the one that writes the field does; a
    /// binding made too early would name an id no block has and never fire.
    /// </remarks>
    [Fact]
    public async Task A_computed_value_written_before_the_fields_it_reads_binds_when_they_are_written()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        Textbox? later = null;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            using (blocks.BeginRender())
            {
                later = new Textbox { Label = "Later" };

                blocks.Add(new Textbox
                {
                    Label = "Greeting",
                    LoadFn = new LoadFunction((Func<string?, string>)(who => $"hello {who}"), "greeting"),
                    Inputs = [later],
                });
            }

            using (blocks.BeginRender())
            {
                blocks.Add(later);
            }
        });

        var config = await ConfigDocument.ReadAsync(host, "/config", cancellationToken);
        var dependency = Assert.Single(Assert.IsType<JsonArray>(config["dependencies"]));

        // The field that was written second is the one the trigger names, and there is one binding
        // rather than one per render.
        Assert.Equal([$"{later!.Id}:change", "0:load"], ConfigDocument.Targets(dependency));
        Assert.Equal([later!.Id], ConfigDocument.Ids(dependency!["inputs"]));
    }

    /// <summary>
    /// A field that raises no change is read, but does not re-run the value.
    /// </summary>
    /// <remarks>
    /// The Python version keeps an input's <c>change</c> trigger only when the component has one,
    /// and the same filter decides the progress animation: with nothing that can change, the page's
    /// opening is the only trigger and the animation is shown in full. An api block is the
    /// component that raises nothing at all, so it is the case that tells the two rules apart.
    /// </remarks>
    [Fact]
    public async Task A_field_that_raises_no_change_is_read_but_does_not_re_run_the_value()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            using (blocks.BeginRender())
            {
                var argument = blocks.Add(new Api { Schema = new JsonObject { ["type"] = "string" } });

                blocks.Add(new Textbox
                {
                    Label = "Greeting",
                    LoadFn = new LoadFunction((Func<object?, string>)(who => $"hello {who}"), "greeting"),
                    Inputs = [argument],
                });
            }
        });

        var config = await ConfigDocument.ReadAsync(host, "/config", cancellationToken);
        var dependency = Assert.Single(Assert.IsType<JsonArray>(config["dependencies"]));

        Assert.Equal(["0:load"], ConfigDocument.Targets(dependency));

        // Read all the same, and the animation is the whole one because nothing can change.
        Assert.Equal([1], ConfigDocument.Ids(dependency!["inputs"]));
        Assert.Equal("full", dependency!["show_progress"]!.GetValue<string>());
    }

    [Fact]
    public async Task A_component_whose_value_was_written_binds_no_load_event()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            using (blocks.BeginRender())
            {
                blocks.Add(new Textbox { Label = "Written", Value = "here" });
            }
        });

        var config = await ConfigDocument.ReadAsync(host, "/config", cancellationToken);

        // Nothing asked for a computed value, so there is no dependency at all: the attach pass
        // adds one per component that asked, not one per component.
        Assert.Empty(Assert.IsType<JsonArray>(config["dependencies"]));
    }

    /// <summary>Runs a binding once over the queue and returns the value it wrote.</summary>
    /// <param name="queue">The client the run goes through.</param>
    /// <param name="fnIndex">The binding to run.</param>
    /// <param name="data">The values the client sends.</param>
    /// <param name="cancellationToken">Cancels the test.</param>
    /// <returns>The value the component was given, as text.</returns>
    private static async Task<string?> RunAsync(
        QueueClient queue,
        int fnIndex,
        JsonArray data,
        CancellationToken cancellationToken)
    {
        var eventId = await queue.JoinAsync(fnIndex, data, "load", cancellationToken);

        await using var stream = await queue.OpenAsync("load", cancellationToken);

        var completed = await stream.UntilAsync(
            message => QueueSession.Kind(message) == "process_completed"
                && message["event_id"]?.GetValue<string>() == eventId,
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.NotNull(completed);
        Assert.True(completed["success"]!.GetValue<bool>());

        return completed["output"]!["data"]!.AsArray()[0]?.GetValue<string>();
    }
}

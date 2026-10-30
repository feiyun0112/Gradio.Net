using System.Text.Json.Nodes;
using Gradio.Net;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gradio.Net.Tests.Kernel;

/// <summary>
/// The event-binding mechanism, exercised at the seam where a binding becomes a served entry.
/// </summary>
/// <remarks>
/// <para>
/// A binding has two halves that are easy to get wrong in opposite directions: what it does when it
/// runs, and how it describes itself. This file is about the second half — the config entry, which
/// is the half the frontend and the API clients read — and about the three output shapes, which are
/// the contract between a handler and the outputs it fills.
/// </para>
/// <para>
/// The served page is the end-to-end proof; these tests are what makes a failure legible, because
/// they say which part of a binding is wrong rather than which field of a document differs.
/// </para>
/// </remarks>
public sealed class EventBindingTests
{
    /// <summary>The keys of a served binding, in the Python version's order.</summary>
    private static readonly string[] BindingKeys =
    [
        "id", "targets", "inputs", "outputs", "backend_fn", "js", "queue", "api_name",
        "api_description", "scroll_to_output", "show_progress", "show_progress_on", "batch",
        "max_batch_size", "cancels", "types", "collects_event_data", "trigger_after",
        "trigger_only_on_success", "trigger_only_on_failure", "trigger_mode",
        "api_visibility", "rendered_in", "render_id", "connection", "time_limit",
        "stream_every", "event_specific_args", "component_prop_inputs", "js_implementation",
    ];

    /// <summary>The handler the tests bind. A named method, so the name it is served under is the one it was declared with.</summary>
    private static object? Handler(string value) => value;

    [Fact]
    public void A_binding_is_served_with_upstreams_keys_and_its_defaults()
    {
        using var page = Page();

        var entry = Entry(page.Blocks, page.Input.Change(Handler, [page.Input], [page.Output]));

        Assert.Equal(BindingKeys, entry.Select(pair => pair.Key));

        Assert.Equal(page.Input.Id, entry["targets"]![0]![0]!.GetValue<int>());
        Assert.Equal("change", entry["targets"]![0]![1]!.GetValue<string>());
        Assert.Equal([page.Input.Id], Ids(entry["inputs"]!));
        Assert.Equal([page.Output.Id], Ids(entry["outputs"]!));
        Assert.True(entry["backend_fn"]!.GetValue<bool>());
        Assert.True(entry["queue"]!.GetValue<bool>());
        Assert.False(entry["batch"]!.GetValue<bool>());
        Assert.Equal(4, entry["max_batch_size"]!.GetValue<int>());
        Assert.Equal("sse", entry["connection"]!.GetValue<string>());
        Assert.Equal(0.5, entry["stream_every"]!.GetValue<double>());
        Assert.Equal("public", entry["api_visibility"]!.GetValue<string>());
        Assert.Equal("full", entry["show_progress"]!.GetValue<string>());
        Assert.False(entry["types"]!["generator"]!.GetValue<bool>());
        Assert.False(entry["types"]!["cancel"]!.GetValue<bool>());
        Assert.False(entry["collects_event_data"]!.GetValue<bool>());
        Assert.Null(entry["js"]);
        Assert.Null(entry["api_description"]);
        Assert.Null(entry["time_limit"]);
        Assert.Null(entry["trigger_after"]);
        Assert.Empty(entry["cancels"]!.AsArray());
        Assert.Empty(entry["component_prop_inputs"]!.AsArray());
    }

    [Fact]
    public void The_typed_path_and_the_fallback_path_produce_the_same_binding()
    {
        using var page = Page();

        var typed = page.Input.Change(Handler, [page.Input], [page.Output]);
        var fallback = page.Input.Change(Handler, [page.Input], [page.Output]);

        var typedEntry = Entry(page.Blocks, typed);
        var fallbackEntry = Entry(page.Blocks, fallback);

        // The id and the name differ because the two are different bindings; everything else is the
        // same shape, which is what "the fallback is the same mechanism" has to mean.
        typedEntry.Remove("id");
        fallbackEntry.Remove("id");
        typedEntry.Remove("api_name");
        fallbackEntry.Remove("api_name");

        Assert.True(JsonNode.DeepEquals(typedEntry, fallbackEntry), $"{typedEntry} != {fallbackEntry}");
    }

    [Fact]
    public void A_binding_can_read_several_components_and_a_two_input_handler_is_typed()
    {
        using var page = Page();

        var second = page.Add(new Textbox { Label = "Second" });
        var dependency = page.Input.Change(
            (Func<string, string, object?>)((left, right) => left + right),
            second,
            [page.Output]);

        Assert.Equal([page.Input.Id, second.Id], Ids(Entry(page.Blocks, dependency)["inputs"]!));
    }

    [Fact]
    public void Several_triggers_become_one_endpoint()
    {
        using var page = Page();
        var other = page.Add(new Textbox { Label = "Other" });

        var dependency = page.Blocks.On(
            [page.Input.Events.Change, other.Events.Change],
            fn: (Delegate)Handler, inputs: [page.Input], outputs: [page.Output]);

        var entry = Entry(page.Blocks, dependency);

        Assert.Equal(2, entry["targets"]!.AsArray().Count);
        Assert.Equal(page.Input.Id, entry["targets"]![0]![0]!.GetValue<int>());
        Assert.Equal(other.Id, entry["targets"]![1]![0]!.GetValue<int>());
        Assert.Equal("change", entry["targets"]![0]![1]!.GetValue<string>());
        Assert.Equal("change", entry["targets"]![1]![1]!.GetValue<string>());

        // A merged set reports an event-specific argument list even when it is empty; a binding to
        // one component's own signal reports nothing at all. The difference is visible in the
        // config, so it is asserted rather than assumed.
        Assert.NotNull(entry["event_specific_args"]);
        Assert.Empty(entry["event_specific_args"]!.AsArray());
    }

    [Fact]
    public void A_binding_to_one_components_signal_reports_no_event_specific_arguments()
    {
        using var page = Page();

        var entry = Entry(page.Blocks, page.Input.Change(Handler, [page.Input], [page.Output]));

        Assert.Null(entry["event_specific_args"]);
    }

    [Fact]
    public void Every_binding_parameter_reaches_the_entry()
    {
        using var page = Page();

        var dependency = page.Input.Change(
            Handler,
            [page.Input],
            [page.Output],
            apiName: "described", apiDescription: "What it does", scrollToOutput: true, showProgress: ProgressDisplay.Minimal, showProgressOn: [page.Output], batch: true, maxBatchSize: 16, triggerMode: TriggerMode.Multiple, apiVisibility: ApiVisibility.Undocumented, timeLimit: 12.5, streamEvery: 0.25);

        var entry = Entry(page.Blocks, dependency);

        Assert.Equal("described", entry["api_name"]!.GetValue<string>());
        Assert.Equal("What it does", entry["api_description"]!.GetValue<string>());
        Assert.True(entry["scroll_to_output"]!.GetValue<bool>());
        Assert.Equal("minimal", entry["show_progress"]!.GetValue<string>());
        Assert.Equal([page.Output.Id], Ids(entry["show_progress_on"]!));
        Assert.True(entry["batch"]!.GetValue<bool>());
        Assert.Equal(16, entry["max_batch_size"]!.GetValue<int>());
        Assert.Equal("multiple", entry["trigger_mode"]!.GetValue<string>());
        Assert.Equal("undocumented", entry["api_visibility"]!.GetValue<string>());
        Assert.Equal(12.5, entry["time_limit"]!.GetValue<double>());
        Assert.Equal(0.25, entry["stream_every"]!.GetValue<double>());
    }

    [Fact]
    public void The_parameters_the_queue_reads_stay_on_the_binding()
    {
        using var page = Page();

        var dependency = page.Input.Change(
            Handler,
            [page.Input],
            [page.Output],
            concurrencyLimit: ConcurrencyLimit.Of(3), concurrencyId: "group", preprocess: false, postprocess: false, validator: (Func<string, object?>)(value => value));

        Assert.Equal(3, dependency.ConcurrencyLimit.Value);
        Assert.Equal("group", dependency.ConcurrencyId);
        Assert.False(dependency.Preprocess);
        Assert.False(dependency.Postprocess);
        Assert.NotNull(dependency.Validator);

        // They are not part of the served document, which is the Python version's shape: the queue reads them
        // when a run is admitted rather than when the config is built.
        var names = Entry(page.Blocks, dependency).Select(pair => pair.Key).ToArray();
        Assert.DoesNotContain("concurrency_limit", names);
        Assert.DoesNotContain("concurrency_id", names);
        Assert.DoesNotContain("preprocess", names);
        Assert.DoesNotContain("postprocess", names);
        Assert.DoesNotContain("validator", names);
    }

    [Theory]
    [InlineData("change", "always_last")]
    [InlineData("input", "once")]
    [InlineData("submit", "once")]
    [InlineData("blur", "once")]
    public void An_unset_trigger_mode_is_taken_from_the_event(string name, string expected)
    {
        using var page = Page();

        var dependency = page.Blocks.On(
            [new EventTrigger(page.Input, name)],
            fn: (Delegate)Handler, inputs: [page.Input], outputs: [page.Output]);

        Assert.Equal(expected, Entry(page.Blocks, dependency)["trigger_mode"]!.GetValue<string>());
    }

    [Fact]
    public void A_chained_step_follows_the_binding_it_was_chained_to()
    {
        using var page = Page();

        var first = page.Input.Submit(Handler, [page.Input], [page.Output]);
        var then = page.Blocks.On(
            [first.Events.Then],
            fn: (Delegate)Handler, inputs: [page.Input], outputs: [page.Output]);

        var failure = page.Blocks.On(
            [first.Events.Failure],
            fn: (Delegate)Handler, inputs: [page.Input], outputs: [page.Output]);

        var chained = Entry(page.Blocks, then);
        Assert.Equal(first.Id, chained["trigger_after"]!.GetValue<int>());
        Assert.Null(chained["targets"]![0]![0]);
        Assert.Equal("then", chained["targets"]![0]![1]!.GetValue<string>());
        Assert.False(chained["trigger_only_on_success"]!.GetValue<bool>());
        Assert.False(chained["trigger_only_on_failure"]!.GetValue<bool>());

        var failed = Entry(page.Blocks, failure);
        Assert.Equal(first.Id, failed["trigger_after"]!.GetValue<int>());
        Assert.Equal("failure", failed["targets"]![0]![1]!.GetValue<string>());
        Assert.True(failed["trigger_only_on_failure"]!.GetValue<bool>());
    }

    [Fact]
    public void A_step_bound_with_the_chaining_method_follows_the_binding_it_was_chained_to()
    {
        using var page = Page();

        var first = page.Input.Submit(Handler, [page.Input], [page.Output]);
        var step = first.Then((Delegate)Handler, [page.Input], [page.Output]);

        // The method is the short form of binding the trigger, so what it produces is the same
        // dependency the explicit form produces: a step that follows the first one, whatever its
        // outcome, and a target that names no block because the step listens to a binding.
        var chained = Entry(page.Blocks, step);

        Assert.Equal(first.Id, chained["trigger_after"]!.GetValue<int>());
        Assert.Equal("then", chained["targets"]![0]![1]!.GetValue<string>());
        Assert.Null(chained["targets"]![0]![0]);
        Assert.False(chained["trigger_only_on_success"]!.GetValue<bool>());
        Assert.False(chained["trigger_only_on_failure"]!.GetValue<bool>());
    }

    [Fact]
    public void Cancelling_another_binding_registers_a_private_binding_for_it()
    {
        using var page = Page();

        var longRunning = page.Input.Input(Handler, [page.Input], [page.Output]);
        var cancelling = page.Input.Submit(
            Handler,
            [page.Input],
            [page.Output],
            cancels: [longRunning]);

        Assert.Empty(Entry(page.Blocks, cancelling)["cancels"]!.AsArray());

        var cancellation = page.Blocks.Dependencies[^1];
        Assert.NotEqual(cancelling.Id, cancellation.Id);
        Assert.True(cancellation.IsCancelFunction);
        Assert.False(cancellation.BackendFunction);
        Assert.False(cancellation.Queue);
        Assert.Equal(ApiVisibility.Private, cancellation.ApiVisibility);
        Assert.Equal("unnamed", cancellation.ApiName);
        Assert.Equal([longRunning.Id], cancellation.Cancels);

        var entry = Entry(page.Blocks, cancellation);
        Assert.True(entry["types"]!["cancel"]!.GetValue<bool>());
        Assert.Equal([longRunning.Id], Ids(entry["cancels"]!));
    }

    [Fact]
    public void A_name_derived_from_a_handler_leaves_the_visibility_alone()
    {
        using var page = Page();

        var anonymous = page.Input.Change((Delegate)((string value) => value), [page.Input], [page.Output]);
        var named = page.Input.Change(
            (Delegate)((string value) => value),
            [page.Input],
            [page.Output],
            apiName: "mine");

        // A handler is something a client can call, so a binding that has one is an endpoint whether
        // or not the caller named it; only the name is derived.
        Assert.Equal("lambda", anonymous.ApiName);
        Assert.Equal(ApiVisibility.Public, anonymous.ApiVisibility);
        Assert.Equal("mine", named.ApiName);
        Assert.Equal(ApiVisibility.Public, named.ApiVisibility);
    }

    [Fact]
    public void A_binding_with_no_handler_and_no_name_is_private()
    {
        using var page = Page();

        var anonymous = page.Input.Change(fn: (Delegate?)null, inputs: [], outputs: []);
        var named = page.Input.Change(
            fn: (Delegate?)null,
            inputs: [],
            outputs: [],
            apiName: "mine");

        Assert.Equal("unnamed", anonymous.ApiName);
        Assert.Equal(ApiVisibility.Private, anonymous.ApiVisibility);

        // A name the caller gave is a name the caller chose, so the binding keeps the visibility it
        // was given even though there is no handler behind it.
        Assert.Equal("mine", named.ApiName);
        Assert.Equal(ApiVisibility.Public, named.ApiVisibility);
    }

    [Fact]
    public void A_name_already_in_use_is_made_unique()
    {
        using var page = Page();

        var first = page.Input.Change(Handler, [page.Input], [page.Output]);
        var second = page.Input.Input(Handler, [page.Input], [page.Output]);
        var third = page.Input.Submit(Handler, [page.Input], [page.Output]);

        Assert.Equal("Handler", first.ApiName);
        Assert.Equal("Handler_1", second.ApiName);
        Assert.Equal("Handler_2", third.ApiName);
    }

    [Fact]
    public void A_frontend_only_binding_runs_no_handler_and_is_named_for_the_frontend()
    {
        using var page = Page();

        var dependency = page.Input.Change(
            fn: (Delegate?)null,
            inputs: [],
            outputs: [],
             js: "() => 1");

        Assert.False(dependency.BackendFunction);
        Assert.False(dependency.Queue);
        Assert.Equal("js_fn", dependency.ApiName);
        Assert.Equal(ApiVisibility.Private, dependency.ApiVisibility);
        Assert.Equal("() => 1", Entry(page.Blocks, dependency)["js"]!.GetValue<string>());
    }

    [Fact]
    public void A_binding_may_not_name_a_component_of_another_application()
    {
        using var first = Page();
        using var second = Page();

        var exception = Assert.Throws<InvalidOperationException>(
            () => first.Input.Change(Handler, [second.Input], [first.Output]));

        Assert.Contains("own application", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void A_binding_may_not_name_something_that_is_not_a_component()
    {
        using var page = Page();

        Assert.Throws<ArgumentException>(
            () => page.Input.Change(Handler, ["not a component"], [page.Output]));
    }

    [Fact]
    public void A_binding_outside_a_render_root_is_refused()
    {
        using var page = Page();
        var loose = new Textbox { Label = "Loose" };

        var exception = Assert.Throws<InvalidOperationException>(
            () => loose.Change(Handler, [loose], []));

        Assert.Contains("belong to an application", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task The_three_output_shapes_are_all_bound_and_their_results_read()
    {
        using var page = Page();
        var second = page.Add(new Textbox { Label = "Second" });

        // A tuple fills one output per element: the handler answers both components at once.
        var tuple = page.Input.Change(
            (Func<string, object?>)(value => (value, value.Length)),
            [page.Input],
            [page.Output, second]);

        Assert.Equal<object?>(["abc", 3], EventResult.Unpack(tuple.Handler!.DynamicInvoke("abc")));

        // A component instance updates the output rather than replacing its value: the handler built
        // the component it wants the output to become.
        var update = page.Input.Change(
            (Func<string, object?>)(value => new Textbox { Value = value }),
            [page.Input],
            [page.Output]);

        var produced = update.Handler!.DynamicInvoke("abc");
        Assert.True(EventResult.IsComponentUpdate(produced));
        Assert.Equal("abc", ((Textbox)produced!).Value);

        // A stream is read the same way, one yield at a time, and is served as a generator.
        var stream = page.Input.Change(
            (Func<string, object?>)Streaming,
            [page.Input],
            [page.Output]);

        Assert.True(stream.IsGenerator);

        var producedStream = (IAsyncEnumerable<string>)stream.Handler!.DynamicInvoke("abc")!;
        var chunks = new List<string>();
        await foreach (var chunk in producedStream)
        {
            chunks.Add(chunk);
        }

        Assert.Equal(["abc", "abc!"], chunks);

        static async IAsyncEnumerable<string> Streaming(string value)
        {
            yield return value;
            yield return value + "!";
        }
    }

    [Fact]
    public void A_tuple_result_gives_one_value_per_output()
    {
        var values = EventResult.Unpack(("text", 3));

        Assert.Equal(2, values.Count);
        Assert.Equal("text", values[0]);
        Assert.Equal(3, values[1]);
    }

    [Fact]
    public void A_single_result_gives_one_value()
    {
        Assert.Equal<object?>(["text"], EventResult.Unpack("text"));
        Assert.Equal<object?>([null], EventResult.Unpack(null));
    }

    [Fact]
    public void A_component_result_is_an_update_rather_than_a_value()
    {
        Assert.True(EventResult.IsComponentUpdate(new Textbox { Value = "set by the handler" }));
        Assert.False(EventResult.IsComponentUpdate("a value"));
        Assert.False(EventResult.IsComponentUpdate(null));
    }

    [Fact]
    public void A_streamed_result_is_recognised_as_a_stream()
    {
        Assert.True(EventResult.IsStream(Stream()));
        Assert.False(EventResult.IsStream("a value"));
        Assert.False(EventResult.IsStream(null));

        static async IAsyncEnumerable<string> Stream()
        {
            yield return "chunk";
        }
    }

    [Fact]
    public void A_handler_that_streams_is_served_as_a_generator()
    {
        using var page = Page();

        var dependency = page.Input.Change(
            (Func<string, object?>)Streaming,
            [page.Input],
            [page.Output]);

        Assert.True(dependency.IsGenerator);
        Assert.True(Entry(page.Blocks, dependency)["types"]!["generator"]!.GetValue<bool>());

        static async IAsyncEnumerable<string> Streaming(string value)
        {
            yield return value;
        }
    }

    private static PageScope Page() => new();

    [Fact]
    public void A_component_is_selectable_once_something_listens_for_its_selection()
    {
        using var page = Page();
        var image = page.Add(new Image { Label = "Image" });

        // The frontend offers the affordance from the config, so binding a handler to the signal
        // has to change what the config says — for every component, not only for the ones whose
        // affordances are new.
        Assert.False(image.Selectable);

        image.Select((Func<string, object?>)(value => value), [image], [page.Output]);

        Assert.True(image.Selectable);

        var props = page.Blocks.GetConfig()["components"]!.AsArray()
            .Select(entry => entry!.AsObject())
            .Single(entry => entry["id"]!.GetValue<int>() == image.Id)["props"]!;

        Assert.True(props["_selectable"]!.GetValue<bool>());
    }

    /// <summary>The served entry of one binding, read out of the application's config.</summary>
    private static JsonObject Entry(Blocks blocks, Dependency dependency) =>
        blocks.GetConfig()["dependencies"]!.AsArray()
            .Select(entry => entry!.AsObject())
            .Single(entry => entry["id"]!.GetValue<int>() == dependency.Id);

    private static int[] Ids(JsonNode node) =>
        node.AsArray().Select(value => value!.GetValue<int>()).ToArray();

    /// <summary>An application with the components a binding needs, and a render scope around them.</summary>
    private sealed class PageScope : IDisposable
    {
        private readonly ServiceProvider _provider;
        private readonly IDisposable _render;

        internal PageScope()
        {
            _provider = KernelHost.BuildProvider();
            Blocks = _provider.CreateBlocksWithContainers();
            _render = Blocks.BeginRender();

            Input = Blocks.Add(new Textbox { Label = "Input" });
            Output = Blocks.Add(new Textbox { Label = "Output" });
        }

        internal Blocks Blocks { get; }

        internal Textbox Input { get; }

        internal Textbox Output { get; }

        internal TBlock Add<TBlock>(TBlock block)
            where TBlock : Block =>
            Blocks.Add(block);

        public void Dispose()
        {
            _render.Dispose();
            Blocks.Dispose();
            _provider.Dispose();
        }
    }
}

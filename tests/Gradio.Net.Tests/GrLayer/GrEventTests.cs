using System.Reflection;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.GrLayer;

/// <summary>
/// The event half of the layer's acceptance: the bindings, the merged trigger set, the chain, and
/// the two values a handler answers with.
/// </summary>
/// <remarks>
/// <para>
/// The claim being checked is that writing the page with <c>gr</c> changes nothing about what it
/// becomes. The strongest way to say that is to write the same page twice — once each way — and
/// hold the two configs against each other character by character, which is what
/// <see cref="A_page_written_with_the_layer_is_the_page_written_with_the_core"/> does. Everything
/// else here pins one decision the comparison would only report as "different".
/// </para>
/// <para>
/// The message sequence is the other half, and it needs a server: a config that matches says the
/// page is described the same, and only a run says the handler's answer travels the same.
/// <c>GrEventRunTests</c> is where that happens.
/// </para>
/// </remarks>
public sealed class GrEventTests
{
    [Fact]
    public void A_page_written_with_the_layer_is_the_page_written_with_the_core()
    {
        // The ticket's own words: the same page, byte for byte. Not "equivalent" — a field that
        // matched by accident is exactly what this would hide.
        var layer = GrEventPage.Layer();

        var core = new Blocks(new RenderContextAccessor(), new BlockContainerFactory());
        GrEventPage.Core(core);

        Assert.Equal(ConfigOf(core), ConfigOf(layer));
    }

    [Fact]
    public void Merging_two_triggers_registers_one_binding_with_two_targets()
    {
        // The Python version's `gr.on` generates "only a single API endpoint" for every trigger in the list,
        // so the merge is one binding with two targets rather than two bindings that share a
        // handler. Two endpoints would also mean two ids, and the frontend would dispatch to both.
        using var demo = gr.Blocks();

        var name = gr.Textbox(label: "Name");
        var status = gr.Textbox(label: "Status");

        var binding = gr.On(
            [name.Events.Change, name.Events.Submit],
            (Func<string, object?>)(value => value),
            [name],
            [status],
            apiName: "merged");

        var only = Assert.Single(demo.Dependencies);

        Assert.Same(only, binding);
        Assert.Equal(["change", "submit"], only.Targets.Select(target => target.EventName));
        Assert.Equal("merged", only.ApiName);

        // The served dependency carries both, and the empty event-specific argument list that says
        // it was assembled by merging rather than taken from one component — the Python version's distinction,
        // visible in the document.
        var entry = Assert.Single(demo.GetConfig()["dependencies"]!.AsArray())!;

        Assert.Equal("""[[1,"change"],[1,"submit"]]""", entry["targets"]!.ToJsonString());
        Assert.Equal("[]", entry["event_specific_args"]!.ToJsonString());
    }

    [Fact]
    public void A_single_trigger_is_a_list_of_one()
    {
        // `gr.on(btn.click, …)` and `gr.on([btn.click], …)` are the same call in the Python version, so they are
        // the same call here — which is the "one value or a list" reading already applied
        // to `inputs`. One binding, one target, and the same merged marker: going through the merge
        // entry point is what it says, however many triggers went in.
        using var demo = gr.Blocks();

        var button = gr.Button("Greet");
        var status = gr.Textbox(label: "Status");

        var binding = gr.On(
            button.Events.Click,
            (Func<object?>)(() => "clicked"),
            outputs: [status],
             apiName: "single");

        var only = Assert.Single(demo.Dependencies);

        Assert.Same(only, binding);
        Assert.Equal(["click"], only.Targets.Select(target => target.EventName));
        Assert.Equal("[]", demo.GetConfig()["dependencies"]!.AsArray()[0]!["event_specific_args"]!.ToJsonString());
    }

    [Fact]
    public void The_two_spellings_of_a_merged_binding_are_one_binding()
    {
        // The Python version's binding parameters can be written in the call or in the record — the call is the
        // ported line, the record is how the core API writes the same binding — and both have to
        // reach the kernel as the same thing. The call's parameters win where they are given, and a
        // parameter it did not name leaves the record's alone: overwriting it would turn a record
        // carrying the handler into a binding with none, silently.
        Assert.Equal(ConfigOf(Page(positional: true)), ConfigOf(Page(positional: false)));

        static Blocks Page(bool positional)
        {
            using var demo = gr.Blocks();

            var first = gr.Textbox(label: "First");
            var second = gr.Textbox(label: "Second");
            var output = gr.Textbox(label: "Out");

            var triggers = new[] { first.Events.Change, second.Events.Change };

            if (positional)
            {
                gr.On(triggers, Echo, [first], [output], apiName: "merged");
            }
            else
            {
                gr.On(triggers,  fn: Echo, inputs: [first], outputs: [output], apiName: "merged");
            }

            return demo;
        }
    }

    [Fact]
    public void A_merged_binding_is_found_from_its_triggers_alone()
    {
        // The application is not a parameter of `gr.On`, and it is not on the ambient stack either:
        // by the time this runs the `using` block has ended, and a port's last lines are exactly
        // there. A trigger knows its block and the block knows its page, which is what the binding
        // walks up.
        Blocks demo;
        Textbox name;
        Textbox status;
        Button button;

        using (demo = gr.Blocks())
        {
            name = gr.Textbox(label: "Name");
            status = gr.Textbox(label: "Status");
            button = gr.Button("Greet");
        }

        var binding = gr.On(
            [button.Events.Click, name.Events.Submit],
            (Func<string, object?>)(value => value),
            [name],
            [status]);

        Assert.Same(binding, Assert.Single(demo.Dependencies));
    }

    [Fact]
    public void Skipping_leaves_every_output_alone()
    {
        // The Python version's `gr.skip()` is `{"__type__": "update"}` — an update with no props, which the
        // client merges into the output and which therefore changes nothing. Recorded from the
        // release rather than reasoned about: a handler returning it produced exactly this in the
        // completion message.
        Assert.Equal("""{"__type__":"update"}""", gr.Skip().ToJsonString());
    }

    [Fact]
    public void An_update_carries_the_props_it_was_given()
    {
        // The Python version writes the props it was given, in the order it was given them, and the marker
        // last — `gr.update(value="x", label="y")` serves `{"value": "x", "label": "y",
        // "__type__": "update"}`. The keys are the config's, because that is what the client merges.
        var update = gr.Update(new Update
        {
            Value = string.Empty,
            Label = "Reset",
        });

        Assert.Equal("""{"value":"","label":"Reset","__type__":"update"}""", update.ToJson().ToJsonString());
    }

    [Fact]
    public void An_update_writes_a_config_value_the_way_the_config_writes_it()
    {
        // `visible` is the Python version's `bool | Literal["hidden"]`, and the framework has a type for it.
        // A caller holding that type should not have to spell its wire form: serialized as a struct
        // it would be a number, which no client reads.
        var update = gr.Update(new Update { Visible = Visibility.Hidden });

        Assert.Equal("""{"visible":"hidden","__type__":"update"}""", update.ToJson().ToJsonString());
    }

    [Fact]
    public void An_update_can_select_a_tab()
    {
        // The Python version's `gr.update(selected="Two")` on a `gr.Tabs` output is how a handler
        // moves the page to another tab: the client binds the prop to the tab bar's own selection.
        // The key is `selected`, which is what the config calls the parameter.
        var update = gr.Update(new Update { Selected = "Two" });

        Assert.Equal("""{"selected":"Two","__type__":"update"}""", update.ToJson().ToJsonString());
    }

    [Fact]
    public void An_update_can_change_a_prop_the_record_does_not_name()
    {
        // The Python version's `gr.update` takes any keyword, and every keyword it accepts is a prop
        // some block carries. C# has no keyword splat, so the props the record does not name are
        // given by name — which is what makes the two sides accept the same set.
        var update = gr.Update(new Update
        {
            Props = new Dictionary<string, object?>
            {
                ["show_download_button"] = false,
                ["x_bin"] = null,
            },
        });

        // A key that is present is written even when its value is null: "empty this prop" is an
        // instruction, and a map that dropped nulls could not say it.
        Assert.Equal(
            """{"show_download_button":false,"x_bin":null,"__type__":"update"}""",
            update.ToJson().ToJsonString());
    }

    [Fact]
    public void An_empty_update_is_a_skip()
    {
        // `gr.update()` and `gr.skip()` are one document in the Python version, because "change nothing" and
        // "leave it alone" are one instruction. Two names, one value — and this is where that is
        // said rather than assumed.
        Assert.Equal(gr.Skip().ToJsonString(), gr.Update().ToJson().ToJsonString());
        Assert.Equal(gr.Skip().ToJsonString(), gr.Update(new Update()).ToJson().ToJsonString());
    }

    [Fact]
    public void Every_signal_a_component_raises_has_a_binding_with_upstreams_parameter_names()
    {
        // A trigger a caller can name but not bind is half a surface, and a binding whose parameters
        // are spelled differently is a port that has to be read twice. Every event has the binding
        // that takes any handler: the typed ones are for the events whose handler is
        // given the component's value, and which those are is a choice of this build. The two are
        // overloads of one name, so what is looked for is the one whose parameter list is the Python version's.
        //
        // The names are the Python version's: `fn`, `inputs`, `outputs`, and then the keyword list
        // its event methods take — `api_name`, `queue`, `show_progress`, `concurrency_limit` and the
        // rest, spelled the C# way. `parameterNames` is the one this side adds, because a .NET
        // delegate carries no parameter names of its own and the API document still has to name
        // them. The six hand-written overloads that take a second component or no inputs are the
        // other spellings and are not this claim.
        // The Python version's keyword list, in its order, spelled the C# way.
        string[] Expected =
        [
            "fn", "inputs", "outputs", "parameterNames", "preprocess", "postprocess", "scrollToOutput",
            "showProgress", "showProgressOn", "apiName", "apiDescription", "endpointDescription", "js",
            "jsRunsInBrowser", "jsImplementation", "queue", "batch", "maxBatchSize", "triggerMode",
            "concurrencyLimit", "concurrencyId", "apiVisibility", "timeLimit", "streamEvery", "cancels",
            "validator",
        ];

        var missing = new List<string>();

        foreach (var type in ComponentTypes())
        {
            // A component the factories do not offer is one an application cannot write, so it is
            // not part of this comparison.
            if (GrFactories.New(type) is not Component component)
            {
                continue;
            }

            foreach (var trigger in TriggerNames(component))
            {
                var overloads = type
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(method => string.Equals(method.Name, trigger, StringComparison.Ordinal))
                    .ToList();

                var spelling = overloads
                    .Select(method => method.GetParameters().Select(parameter => parameter.Name).ToArray())
                    .FirstOrDefault(names => names.SequenceEqual(Expected, StringComparer.Ordinal));

                if (spelling is null)
                {
                    var found = overloads.Count == 0
                        ? "nothing"
                        : string.Join(" / ", overloads.Select(method =>
                            $"{trigger}({string.Join(", ", method.GetParameters().Select(parameter => parameter.Name))})"));

                    missing.Add($"{type.Name}.{trigger} — found {found}");
                }
            }
        }

        Assert.True(
            missing.Count == 0,
            $"These signals have no binding with the Python version's parameter names: {string.Join("; ", missing)}. "
            + $"Every event a component raises has to be bindable as {string.Join("/", Expected)}.");
    }

    /// <summary>A handler that answers what it read, so that the binding is the only thing compared.</summary>
    /// <param name="value">The value the binding's input carries.</param>
    /// <returns>The same value.</returns>
    private static string Echo(string value) => value;

    /// <summary>The config both pages are compared by, without the one field no two applications share.</summary>
    /// <param name="demo">The application.</param>
    /// <returns>The served document as text.</returns>
    /// <remarks>
    /// <c>app_id</c> is drawn per application, so two applications in one process never agree on it
    /// and it says nothing about the page. Everything else is compared as the bytes a client would
    /// receive.
    /// </remarks>
    private static string ConfigOf(Blocks demo)
    {
        var config = demo.GetConfig();
        config.Remove("app_id");

        return config.ToJsonString();
    }

    /// <summary>The names of the triggers a component's event set exposes, spelled the C# way.</summary>
    /// <param name="component">The component.</param>
    /// <returns>The property names, which are the binding names.</returns>
    /// <remarks>
    /// Read from the properties rather than from a list: <c>Submit</c> and <c>Stream</c> are declared
    /// on the base and answer <see langword="null"/> for a component that raises neither, which is
    /// how they drop out. The set is read once rather than per property, because reading
    /// <c>Events</c> builds a new one every time.
    /// </remarks>
    private static IReadOnlyList<string> TriggerNames(Component component)
    {
        var events = component.Events;

        return
        [
            .. events.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => property.GetIndexParameters().Length == 0)
                .Where(property => property.GetValue(events) is EventTrigger)
                .Select(property => property.Name),
        ];
    }

    private static IEnumerable<Type> ComponentTypes() =>
        typeof(Component).Assembly.GetTypes()
            .Where(type => type.IsPublic && !type.IsAbstract && typeof(Component).IsAssignableFrom(type))
            .OrderBy(type => type.Name, StringComparer.Ordinal);
}

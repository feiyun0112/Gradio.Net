using System.Text.Json;
using System.Text.Json.Nodes;
using Gradio.Net;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gradio.Net.Tests.Kernel;

/// <summary>
/// A <c>State</c> as the application and the config see it: a component whose value belongs to the
/// session, which is why its config entry carries the lifetime and the callback but not the value.
/// </summary>
public sealed class StateComponentTests
{
    [Fact]
    public void The_config_carries_the_lifetime_and_the_callback_but_not_the_value()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();
        State state;

        using (blocks.BeginRender())
        {
            state = blocks.Add(new State { Value = JsonValue.Create("initial") });
        }

        var entry = Entry(blocks, state);

        // The Python version deletes `value` from a state's props: the value belongs to the session, and a
        // page that received it could only send it back.
        Assert.False(entry["props"]!.AsObject().ContainsKey("value"));

        // The lifetime and the callback stay, because the Python version declares them as constructor
        // parameters and only removes `None`: "never expires" is a number orjson writes as null,
        // and a callable is written as null rather than as a Python repr this side cannot make.
        Assert.True(entry["props"]!.AsObject().ContainsKey("time_to_live"));
        Assert.Null(entry["props"]!["time_to_live"]);
        Assert.True(entry["props"]!.AsObject().ContainsKey("delete_callback"));
        Assert.Null(entry["props"]!["delete_callback"]);

        Assert.Equal("state", entry["type"]!.GetValue<string>());
        Assert.True(entry["skip_api"]!.GetValue<bool>());
        Assert.False(entry.ContainsKey("api_info"));
    }

    [Fact]
    public void A_lifetime_is_written_as_the_number_of_seconds_upstream_would_write()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();
        State state;

        using (blocks.BeginRender())
        {
            state = blocks.Add(new State { TimeToLive = 30 });
        }

        Assert.Equal(30, Entry(blocks, state)["props"]!["time_to_live"]!.GetValue<double>());
    }

    [Fact]
    public void A_state_does_not_split_the_form_around_it()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();
        State state;

        using (blocks.BeginRender())
        {
            blocks.Add(new Textbox { Label = "A" });
            state = blocks.Add(new State { Value = JsonValue.Create(1) });
            blocks.Add(new Textbox { Label = "B" });
        }

        // Two textboxes written straight into a root are grouped into one synthesised form. The
        // state sits between them without breaking that grouping, and without being pulled into the
        // form itself: it is invisible, so it belongs to no container (the Python version's
        // `breaks_grouping` is false and its `get_expected_parent` is None).
        var form = Assert.Single(blocks.Children.OfType<Form>());

        Assert.Equal(2, form.Children.Count);
        Assert.All(form.Children, child => Assert.IsType<Textbox>(child));
        Assert.Same(blocks, state.Parent);
    }

    [Fact]
    public void The_initial_value_is_copied_rather_than_held()
    {
        var node = new JsonObject { ["name"] = "first" };
        var state = new State { Value = node };

        node["name"] = "changed";

        // Two sessions start from the same value, so what the application keeps a reference to must
        // not be what a session starts from.
        Assert.Equal("first", state.Value!["name"]!.GetValue<string>());
    }

    [Fact]
    public void A_session_gets_its_own_copy_of_the_initial_value()
    {
        var state = new State { Value = JsonValue.Create("initial") };
        var stateful = (IStatefulComponent)state;

        var first = stateful.CreateState();
        var second = stateful.CreateState();

        Assert.NotSame(first, second);
        Assert.Equal("initial", first!.GetValue<string>());
    }

    [Fact]
    public void A_value_of_any_shape_round_trips_through_the_session()
    {
        var state = new State();
        var stateful = (IStatefulComponent)state;

        // A dictionary is what reading a JSON object out of a session produces, so writing one back
        // has to keep it a dictionary: a state that was read and returned must not become its own
        // type name.
        var written = stateful.WriteState(new Dictionary<string, object?> { ["turns"] = 3, ["topic"] = "state" });

        Assert.Equal(JsonValueKind.Object, written!.GetValueKind());

        var read = stateful.ReadState(written);

        var dictionary = Assert.IsAssignableFrom<IReadOnlyDictionary<string, object?>>(read);
        Assert.Equal(3d, dictionary["turns"]);
        Assert.Equal("state", dictionary["topic"]);
    }

    [Fact]
    public void A_plain_value_round_trips_as_json()
    {
        var stateful = (IStatefulComponent)new State();

        Assert.Equal(1d, Assert.IsType<double>(stateful.ReadState(stateful.WriteState(1))));
        Assert.Equal("text", Assert.IsType<string>(stateful.ReadState(stateful.WriteState("text"))));
        Assert.True(Assert.IsType<bool>(stateful.ReadState(stateful.WriteState(true))));
        Assert.Null(stateful.ReadState(stateful.WriteState(null)));
    }

    [Theory]
    [InlineData(double.PositiveInfinity, null)]
    [InlineData(0d, 0d)]
    [InlineData(90d, 90d)]
    public void A_lifetime_becomes_a_time_span_or_nothing(double seconds, double? expectedSeconds)
    {
        var stateful = (IStatefulComponent)new State { TimeToLive = seconds };

        Assert.Equal(
            expectedSeconds is { } value ? TimeSpan.FromSeconds(value) : null,
            stateful.StateTimeToLive);
    }

    [Fact]
    public void A_state_that_was_given_no_callback_deletes_without_complaining()
    {
        var stateful = (IStatefulComponent)new State();

        stateful.DeleteState(JsonValue.Create("anything"));
    }

    [Fact]
    public void A_callback_is_handed_the_value_as_the_application_stored_it()
    {
        var deleted = new List<object?>();
        var stateful = (IStatefulComponent)new State { DeleteCallback = new Action<object?>(deleted.Add) };

        stateful.DeleteState(JsonValue.Create("gone"));

        Assert.Equal("gone", Assert.Single(deleted));
    }

    [Fact]
    public void A_returned_state_is_read_as_the_value_it_carries()
    {
        var state = new State { Value = JsonValue.Create("updated") };

        var value = Assert.IsAssignableFrom<JsonNode>(((IStatefulComponent)state).ValueOfUpdate(state));

        Assert.Equal("updated", value.GetValue<string>());
    }

    /// <summary>The config entry of one block.</summary>
    private static JsonObject Entry(Blocks blocks, Block block) =>
        blocks.GetConfig()["components"]!.AsArray()
            .Select(entry => entry!.AsObject())
            .Single(entry => entry["id"]!.GetValue<int>() == block.Id);
}

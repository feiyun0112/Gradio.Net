using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Kernel;

/// <summary>
/// The values a session carries: one per component, each with its own lifetime, and each released
/// through the component's callback when it goes.
/// </summary>
/// <remarks>
/// Time is moved by hand so that "expired" is an exact assertion rather than a race with the clock,
/// which is the same reason the session store's tests do it.
/// </remarks>
public sealed class SessionStateTests
{
    private static readonly DateTimeOffset _start = new(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void A_value_is_created_once_and_read_back()
    {
        var state = new SessionState();

        var first = state.GetOrAdd(7, () => JsonValue.Create(1));
        var second = state.GetOrAdd(7, () => JsonValue.Create(2));

        Assert.Equal(1, first!.GetValue<int>());
        Assert.Equal(1, second!.GetValue<int>());

        // The factory runs only for the session that has nothing: a second call must not replace
        // the value the first one created.
        Assert.Equal(1, state.Count);
    }

    [Fact]
    public void Values_of_different_components_do_not_meet()
    {
        var state = new SessionState();

        state.Set(1, JsonValue.Create("one"), null, null, _start);
        state.Set(2, JsonValue.Create("two"), null, null, _start);

        Assert.True(state.TryGet(1, out var first));
        Assert.True(state.TryGet(2, out var second));
        Assert.Equal("one", first!.GetValue<string>());
        Assert.Equal("two", second!.GetValue<string>());
    }

    [Fact]
    public void A_value_that_was_only_read_never_ages_out()
    {
        var state = new SessionState();
        state.GetOrAdd(1, () => JsonValue.Create("read-only"));

        // The Python version dates a value when it is written, not when it is first read, so a state that is
        // only ever read has no lifetime at all — not even the one a closed session imposes.
        state.TryClose();

        Assert.Equal(0, state.RemoveExpired(_start.AddYears(1)));
        Assert.True(state.TryGet(1, out _));
    }

    [Fact]
    public void A_value_with_no_lifetime_is_kept()
    {
        var state = new SessionState();
        state.Set(1, JsonValue.Create("kept"), timeToLive: null, onDelete: null, _start);

        Assert.Equal(0, state.RemoveExpired(_start.AddYears(10)));
        Assert.True(state.TryGet(1, out _));
    }

    [Fact]
    public void A_value_is_reclaimed_once_its_lifetime_elapses()
    {
        var state = new SessionState();
        state.Set(1, JsonValue.Create("brief"), TimeSpan.FromMinutes(5), onDelete: null, _start);

        Assert.Equal(0, state.RemoveExpired(_start.AddMinutes(4)));
        Assert.True(state.TryGet(1, out _));

        Assert.Equal(1, state.RemoveExpired(_start.AddMinutes(5)));
        Assert.False(state.TryGet(1, out _));
        Assert.Equal(0, state.Count);
    }

    [Fact]
    public void Setting_a_value_again_restarts_its_lifetime()
    {
        var state = new SessionState();
        state.Set(1, JsonValue.Create("first"), TimeSpan.FromMinutes(5), onDelete: null, _start);
        state.Set(1, JsonValue.Create("second"), TimeSpan.FromMinutes(5), onDelete: null, _start.AddMinutes(4));

        // Five minutes after the first write, but only one after the second.
        Assert.Equal(0, state.RemoveExpired(_start.AddMinutes(5)));
        Assert.True(state.TryGet(1, out var value));
        Assert.Equal("second", value!.GetValue<string>());
    }

    [Fact]
    public void A_lifetime_is_per_value_rather_than_per_session()
    {
        var state = new SessionState();
        state.Set(1, JsonValue.Create("brief"), TimeSpan.FromMinutes(1), onDelete: null, _start);
        state.Set(2, JsonValue.Create("lasting"), TimeSpan.FromMinutes(10), onDelete: null, _start);

        Assert.Equal(1, state.RemoveExpired(_start.AddMinutes(2)));
        Assert.False(state.TryGet(1, out _));
        Assert.True(state.TryGet(2, out _));
    }

    [Fact]
    public void An_expired_value_is_handed_to_its_callback()
    {
        var state = new SessionState();
        var deleted = new List<string?>();
        state.Set(
            1,
            JsonValue.Create("gone"),
            TimeSpan.FromSeconds(1),
            value => deleted.Add(value?.GetValue<string>()),
            _start);

        state.RemoveExpired(_start.AddSeconds(1));

        Assert.Equal(["gone"], deleted);
    }

    [Fact]
    public void Removing_a_value_hands_it_to_its_callback()
    {
        var state = new SessionState();
        var deleted = new List<string?>();
        state.Set(1, JsonValue.Create("gone"), null, value => deleted.Add(value?.GetValue<string>()), _start);

        Assert.True(state.Remove(1));
        Assert.False(state.Remove(1));

        Assert.Equal(["gone"], deleted);
    }

    [Fact]
    public void Clearing_the_session_hands_every_value_to_its_callback()
    {
        var state = new SessionState();
        var deleted = new List<string?>();
        state.Set(1, JsonValue.Create("one"), null, value => deleted.Add(value?.GetValue<string>()), _start);
        state.Set(2, JsonValue.Create("two"), null, value => deleted.Add(value?.GetValue<string>()), _start);

        Assert.Equal(2, state.Clear());
        Assert.Equal(0, state.Count);

        // The values arrive in the order they were stored, which is what makes a callback that
        // releases resources one by one predictable.
        Assert.Equal(["one", "two"], deleted);
    }

    [Fact]
    public void A_closed_session_keeps_its_values_for_the_closed_lifetime()
    {
        var state = new SessionState();
        state.Set(1, JsonValue.Create("waiting"), TimeSpan.FromSeconds(1), onDelete: null, _start);

        Assert.True(state.TryClose());

        // The value's own lifetime has long passed, but the page may come back: a reload that found
        // nothing would look like the application had forgotten the user.
        Assert.Equal(0, state.RemoveExpired(_start.AddMinutes(5)));
        Assert.True(state.TryGet(1, out _));

        Assert.Equal(1, state.RemoveExpired(_start.Add(state.ClosedTimeToLive)));
        Assert.False(state.TryGet(1, out _));
    }

    [Fact]
    public void A_session_is_closed_once_and_says_which_close_it_was()
    {
        var state = new SessionState();

        // A page holds two connections that can end — the stream carrying its runs and the
        // heartbeat that says it is alive — and both report the end. The first report is the
        // session ending; the ones after it are the same end arriving twice, and the work an
        // application binds to it must not run twice because of that.
        Assert.True(state.TryClose());
        Assert.False(state.TryClose());
        Assert.True(state.IsClosed);
    }

    [Fact]
    public void Two_sessions_do_not_share_a_value()
    {
        var first = new SessionState();
        var second = new SessionState();

        first.Set(1, JsonValue.Create("first"), null, null, _start);
        second.Set(1, JsonValue.Create("second"), null, null, _start);

        Assert.True(first.TryGet(1, out var firstValue));
        Assert.True(second.TryGet(1, out var secondValue));
        Assert.Equal("first", firstValue!.GetValue<string>());
        Assert.Equal("second", secondValue!.GetValue<string>());
    }

    [Fact]
    public void A_session_holds_its_own_state()
    {
        var session = new Session("session-1", _start);
        var other = new Session("session-2", _start);

        session.State.Set(1, JsonValue.Create("mine"), null, null, _start);

        Assert.True(session.State.TryGet(1, out _));
        Assert.False(other.State.TryGet(1, out _));
    }
}

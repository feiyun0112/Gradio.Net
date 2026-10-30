using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Flagging;

/// <summary>
/// What a flag button does when the sample cannot be written: the two answers differ because one of
/// them has a button to say it on.
/// </summary>
public sealed class FlagMethodTests
{
    [Fact]
    public async Task A_failed_flag_says_so_on_the_button()
    {
        var method = new FlagMethod(new RefusingCallback(), "Flag as wrong", "wrong");

        var answer = await method.InvokeAsync([JsonValue.Create("a")]);

        // The click already put the button into "Saving...", so the answer has to be a button
        // update either way: the one that comes back says the sample was not saved.
        Assert.Equal("Error!", Assert.IsType<Button>(answer).Value);
    }

    [Fact]
    public async Task A_sample_a_component_refuses_says_so_on_the_button_too()
    {
        var method = new FlagMethod(new RefusingValueCallback(), "Flag as wrong", "wrong");

        var answer = await method.InvokeAsync([JsonValue.Create("a")]);

        // Anything that can go wrong is the same thing to the person who clicked: the sample was not
        // saved. The Python version catches every exception here for that reason, and a component that refuses
        // the value it was handed is one of the ways a log can fail.
        Assert.Equal("Error!", Assert.IsType<Button>(answer).Value);
    }

    [Fact]
    public async Task A_failed_flag_with_no_button_to_say_it_on_is_the_runs_failure()
    {
        // Automatic flagging has no button, so a broken log would otherwise lose the sample in
        // silence. The Python version swallows the failure in both cases; here the one with no button to say
        // it on reports it to the run instead.
        var method = new FlagMethod(new RefusingCallback(), "Flag as wrong", "wrong", visualFeedback: false);

        await Assert.ThrowsAsync<IOException>(() => method.InvokeAsync([JsonValue.Create("a")]));
    }

    /// <summary>A callback whose log is always unwritable.</summary>
    private sealed class RefusingCallback : FlaggingCallback
    {
        public override void Setup(IReadOnlyList<Component> components, string directory)
        {
        }

        public override int Flag(
            IReadOnlyList<JsonNode?> values,
            string? flagOption = null,
            string? username = null) =>
            throw new IOException("the log is not writable");
    }

    /// <summary>A callback whose component refuses the sample it was handed.</summary>
    private sealed class RefusingValueCallback : FlaggingCallback
    {
        public override void Setup(IReadOnlyList<Component> components, string directory)
        {
        }

        public override int Flag(
            IReadOnlyList<JsonNode?> values,
            string? flagOption = null,
            string? username = null) =>
            throw new InvalidOperationException("this component does not take that value");
    }
}

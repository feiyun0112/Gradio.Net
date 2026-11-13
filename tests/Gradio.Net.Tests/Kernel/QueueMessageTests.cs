using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Kernel;

/// <summary>
/// The message shapes the vendored frontend reads, held against the Python version's models.
/// </summary>
/// <remarks>
/// A message the server writes but the page cannot read is a feature that is not there: the four
/// callouts were sent for several releases and never drawn because their sentence and their level
/// sat in fields the frontend does not look at. These tests name the fields the page does read.
/// </remarks>
public sealed class QueueMessageTests
{
    [Fact]
    public void A_callout_is_the_python_LogMessage_field_for_field()
    {
        var message = QueueMessages.Log("event-1", "info", "这是一条信息", 8);

        Assert.Equal("log", message["msg"]!.GetValue<string>());
        Assert.Equal("event-1", message["event_id"]!.GetValue<string>());

        // The sentence is `log` and the kind is `level`. Before the fix the level was in `log` and
        // the sentence in `message`, which the frontend's handle_log never reads.
        Assert.Equal("这是一条信息", message["log"]!.GetValue<string>());
        Assert.Equal("info", message["level"]!.GetValue<string>());
        Assert.False(message.ContainsKey("message"));

        Assert.Equal(8, message["duration"]!.GetValue<double>());
        Assert.True(message["visible"]!.GetValue<bool>());
        Assert.Equal("Info", message["title"]!.GetValue<string>());
    }

    [Fact]
    public void A_callout_that_names_no_duration_gets_the_python_default()
    {
        var message = QueueMessages.Log("event-1", "warning", "小心", null);

        Assert.Equal(10, message["duration"]!.GetValue<double>());
        Assert.Equal("小心", message["log"]!.GetValue<string>());
    }

    [Theory]
    [InlineData("info", "Info")]
    [InlineData("warning", "Warning")]
    [InlineData("error", "Error")]
    [InlineData("success", "Success")]
    public void Each_level_gets_the_title_python_gives_it(string level, string title)
    {
        var message = QueueMessages.Log("event-1", level, "正文", 4);

        Assert.Equal(level, message["level"]!.GetValue<string>());
        Assert.Equal(title, message["title"]!.GetValue<string>());
    }

    [Fact]
    public void A_browser_fed_chunk_is_process_streaming_with_the_remaining_limit()
    {
        var fed = QueueMessages.ProcessGenerating(
            "event-1",
            new JsonObject(),
            success: true,
            timeLimit: 2.5,
            streaming: true);

        Assert.Equal("process_streaming", fed["msg"]!.GetValue<string>());
        Assert.Equal(2.5, fed["time_limit"]!.GetValue<double>());
    }

    [Fact]
    public void A_chunk_the_queue_advances_itself_has_no_time_limit()
    {
        var advanced = QueueMessages.ProcessGenerating("event-1", new JsonObject(), success: true);

        Assert.Equal("process_generating", advanced["msg"]!.GetValue<string>());
        Assert.Null(advanced["time_limit"]);
    }
}

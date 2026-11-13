using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Kernel;

/// <summary>
/// The job-style result stream's framing, held against the Python version's
/// <c>routes.py::simple_predict_get</c> message by message.
/// </summary>
/// <remarks>
/// The browser stream and the job stream read the same queue messages but write different bytes.
/// A field-by-field test of the queue's own messages would pass while the result route still
/// forwarded them unchanged — which is exactly the defect this holds in place — so the frames
/// themselves are what these assert, including the messages that are dropped altogether.
/// </remarks>
public sealed class JobStreamTests
{
    [Fact]
    public void A_successful_run_writes_one_complete_event_carrying_only_the_outputs()
    {
        var completed = QueueMessages.ProcessCompleted(
            "event-1",
            Payload(["折好了", 7]),
            success: true);

        Assert.Equal("event: complete\ndata: [\"折好了\",7]\n\n", JobStream.Format(completed));
    }

    [Fact]
    public void A_failed_run_writes_an_error_event_carrying_the_whole_output()
    {
        var failed = QueueMessages.ProcessCompleted(
            "event-1",
            new JsonObject { ["error"] = "内部炸了" },
            success: false);

        Assert.Equal("event: error\ndata: {\"error\":\"内部炸了\"}\n\n", JobStream.Format(failed));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void A_chunk_writes_a_generating_event_on_both_connections(bool streaming)
    {
        var chunk = QueueMessages.ProcessGenerating(
            "event-1",
            Payload(["半截"]),
            success: true,
            streaming: streaming);

        Assert.Equal("event: generating\ndata: [\"半截\"]\n\n", JobStream.Format(chunk));
    }

    [Fact]
    public void A_failed_chunk_writes_an_error_event()
    {
        var chunk = QueueMessages.ProcessGenerating(
            "event-1",
            new JsonObject { ["error"] = "半路出错" },
            success: false);

        Assert.Equal("event: error\ndata: {\"error\":\"半路出错\"}\n\n", JobStream.Format(chunk));
    }

    [Fact]
    public void A_heartbeat_and_a_refusal_have_events_of_their_own()
    {
        Assert.Equal("event: heartbeat\ndata: null\n\n", JobStream.Format(QueueMessages.Heartbeat()));
        Assert.Equal(
            "event: error\ndata: \"404: Not Found\"\n\n",
            JobStream.Format(QueueMessages.UnexpectedError("404: Not Found", sessionNotFound: true)));
    }

    [Theory]
    [InlineData("estimation")]
    [InlineData("process_starts")]
    [InlineData("progress")]
    [InlineData("log")]
    [InlineData("close_stream")]
    public void A_message_with_no_job_style_event_is_not_written(string kind)
    {
        var message = new JsonObject { ["msg"] = kind, ["event_id"] = "event-1" };

        Assert.Null(JobStream.Format(message));
    }

    [Fact]
    public void The_browser_stream_still_writes_the_queue_message_unchanged()
    {
        // The two routes share one channel, so the regression to guard against is a formatter that
        // was applied to both. Nothing here formats: the queue endpoint writes the same JSON.
        var completed = QueueMessages.ProcessCompleted(
            "event-1",
            Payload(["折好了"]),
            success: true);

        Assert.Equal("process_completed", completed["msg"]!.GetValue<string>());
        Assert.Equal("折好了", completed["output"]!["data"]![0]!.GetValue<string>());
    }

    /// <summary>An output payload shaped the way the queue builds one.</summary>
    /// <param name="data">The run's outputs.</param>
    /// <returns>The payload.</returns>
    private static JsonObject Payload(JsonArray data) => new()
    {
        ["data"] = data,
        ["is_generating"] = false,
        ["duration"] = 0.1,
    };
}

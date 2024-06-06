using Gradio.Net.Enums;
using Gradio.Net.Helpers;
using System.Text.Json.Serialization;

namespace Gradio.Net.Models;

[JsonDerivedType(typeof(ProcessCompletedMessage))]
[JsonDerivedType(typeof(UnexpectedErrorMessage))]
[JsonDerivedType(typeof(CloseStreamMessage))]
[JsonDerivedType(typeof(HeartbeatMessage))]
[JsonDerivedType(typeof(ProgressMessage))]
[JsonDerivedType(typeof(ProcessGeneratingMessage))]
[JsonDerivedType(typeof(ProcessStartsMessage))]

public abstract class SSEMessage
{
    public SSEMessage(SSEMessageType msg, string message, bool? success)
    {
        Msg = msg;
        Message = message;
        Success = success;
    }
    [JsonConverter(typeof(SSEMessageTypeJsonConverter))]
    public SSEMessageType Msg { get; set; }
    public string Message { get; set; }
    public bool? Success { get; set; }

    public string ProcessMsg()
    {
        return $"data: {JsonUtils.Serialize(this)}\n\n";
    }
}

public class ProcessStartsMessage : SSEMessage
{
    public ProcessStartsMessage(string eventId)
        : base(SSEMessageType.ProcessStarts, null, null) {
        EventId = eventId; 
    }
    public string EventId { get; set; }
    public decimal? Eta { get; set; }
}

public class UnexpectedErrorMessage : SSEMessage
{
    public UnexpectedErrorMessage(string message)
        : base(SSEMessageType.UnexpectedError, message, false) { }
}

public class CloseStreamMessage : SSEMessage
{
    public CloseStreamMessage()
        : base(SSEMessageType.CloseStream, null, null) { }
}

public class HeartbeatMessage : SSEMessage
{
    public HeartbeatMessage()
        : base(SSEMessageType.Heartbeat, null, null) { }
}

public class ProcessCompletedMessage : SSEMessage
{
    public ProcessCompletedMessage(string eventId, Dictionary<string, object> output)
        : base(SSEMessageType.ProcessCompleted, null, true) {
        this.EventId = eventId;
        this.Output = output;
    }

    public string EventId { get; set; }
    public Dictionary<string, object> Output { get; set; }
}

public class ProgressMessage : SSEMessage
{
    public ProgressMessage(string eventId, Progress progress)
        : base(SSEMessageType.Progress, null, null)
    {
        ProgressData = new List<ProgressUnit> {
            new() {
                Index =progress.Index,
                Desc = progress.Desc,
                Length = progress.Length,
                Unit = progress.Unit
            }
        };
    }

    public IEnumerable<ProgressUnit> ProgressData { get; set; }

    public class ProgressUnit
    {
        public int Index { get; set; }
        public int Length { get; set; }
        public string Unit { get; set; }
        public decimal? Progress { get; set; }
        public string Desc { get; set; }
    }
}

public class ProcessGeneratingMessage : SSEMessage
{
    public ProcessGeneratingMessage(string eventId, Dictionary<string, object> output)
        : base(SSEMessageType.ProcessGenerating, null, true)
    {
        this.EventId = eventId;
        this.Output = output;
    }

    public string EventId { get; set; }
    public Dictionary<string, object> Output { get; set; }
    public bool IsGenerating { get; set; } = true;
}

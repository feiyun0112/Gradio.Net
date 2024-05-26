using Gradio.Net.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Gradio.Net.Models
{
    [JsonDerivedType(typeof(ProcessCompletedMessage))]
    [JsonDerivedType(typeof(UnexpectedErrorMessage))]
    [JsonDerivedType(typeof(CloseStreamMessage))]
    [JsonDerivedType(typeof(HeartbeatMessage))]
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

        internal string ProcessMsg()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };
            return $"data: {JsonSerializer.Serialize(this, serializeOptions)}\n\n";
        }
    }

    public class UnexpectedErrorMessage : SSEMessage
    {
        public UnexpectedErrorMessage(string message)
            :base(SSEMessageType.UnexpectedError,message,false){ }
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
        public ProcessCompletedMessage(string eventId,Dictionary<string,object> output)
            : base(SSEMessageType.ProcessCompleted, null, true) {
            this.EventId = eventId;
            this.Output = output;
        }

        public string EventId { get; set; }
        public Dictionary<string, object> Output { get;  set; }
    }
}

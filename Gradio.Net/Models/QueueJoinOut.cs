using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Gradio.Net
{
    public class QueueJoinOut
    {
        [JsonPropertyName("event_id")]
        public string? EventId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gradio.Net.Enums
{
    public enum SSEMessageType
    {
        SendHash =0,
        QueueFull,
        Estimation,
        SendData,
        ProcessStarts,
        ProcessGenerating,
        ProcessCompleted,
        Log,
        Progress,
        Heartbeat,
        ServerStopped,
        UnexpectedError,
        CloseStream
    }
}

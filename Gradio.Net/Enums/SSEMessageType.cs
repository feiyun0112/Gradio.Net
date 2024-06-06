namespace Gradio.Net.Enums;

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
    CloseStream,
    Done
}

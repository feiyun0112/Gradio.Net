namespace Gradio.Net.Core.Queueing;

public class HeartbeatMessage : EventMessage
{
    public HeartbeatMessage()
    {
        Msg = "heartbeat";
    }
}

namespace Gradio.Net.Core.Queueing;

public class CloseStreamMessage : EventMessage
{
    public CloseStreamMessage()
    {
        Msg = "close_stream";
    }
}

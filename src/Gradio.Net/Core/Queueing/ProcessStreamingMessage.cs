namespace Gradio.Net.Core.Queueing;

public class ProcessStreamingMessage : ProcessGeneratingMessage
{
    public ProcessStreamingMessage()
    {
        Msg = "process_streaming";
    }
}

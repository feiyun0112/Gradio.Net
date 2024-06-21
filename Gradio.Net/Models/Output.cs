namespace Gradio.Net;

public class Output
{
    internal Output() { }
    public object[] Data { get; set; }
}

internal class ErrorOutput : Output
{
    internal ErrorOutput(Exception ex)
    {
        Exception = ex;
    }

    public Exception Exception { get; private set; }
}

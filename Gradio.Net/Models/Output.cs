namespace Gradio.Net;

public class Output
{
    public object[] Data { get; set; }
}

internal class ErrorOutput : Output
{ 
    public ErrorOutput(Exception ex) {
        Exception = ex;
    }

    public Exception Exception { get; private set; }
}

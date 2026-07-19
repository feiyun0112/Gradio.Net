namespace Gradio.Net.Core.Exceptions;

public class InvalidComponentError : Exception
{
    public InvalidComponentError() : base()
    {
    }

    public InvalidComponentError(string message) : base(message)
    {
    }
}

namespace Gradio.Net.Core.Exceptions;

public class InvalidApiNameError : Exception
{
    public InvalidApiNameError() : base()
    {
    }

    public InvalidApiNameError(string message) : base(message)
    {
    }
}

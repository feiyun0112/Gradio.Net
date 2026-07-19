namespace Gradio.Net.Core.Exceptions;

public class InvalidBlockError : Exception
{
    public InvalidBlockError() : base()
    {
    }

    public InvalidBlockError(string message) : base(message)
    {
    }
}

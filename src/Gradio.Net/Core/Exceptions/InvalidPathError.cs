namespace Gradio.Net.Core.Exceptions;

public class InvalidPathError : Exception
{
    public InvalidPathError() : base()
    {
    }

    public InvalidPathError(string message) : base(message)
    {
    }
}

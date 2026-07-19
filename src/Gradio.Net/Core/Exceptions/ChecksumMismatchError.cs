namespace Gradio.Net.Core.Exceptions;

public class ChecksumMismatchError : Exception
{
    public ChecksumMismatchError() : base()
    {
    }

    public ChecksumMismatchError(string message) : base(message)
    {
    }
}

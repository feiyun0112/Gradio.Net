namespace Gradio.Net.Core.Exceptions;

public class DuplicateBlockError : Exception
{
    public DuplicateBlockError() : base()
    {
    }

    public DuplicateBlockError(string message) : base(message)
    {
    }
}

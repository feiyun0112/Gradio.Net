namespace Gradio.Net.Core.Exceptions;

// backwards compatibility alias
public class InvalidApiName : InvalidApiNameError
{
    public InvalidApiName() : base()
    {
    }

    public InvalidApiName(string message) : base(message)
    {
    }
}

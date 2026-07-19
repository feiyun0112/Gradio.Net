namespace Gradio.Net.Core.Exceptions;

public class ModelNotFoundError : Exception
{
    public ModelNotFoundError() : base()
    {
    }

    public ModelNotFoundError(string message) : base(message)
    {
    }
}

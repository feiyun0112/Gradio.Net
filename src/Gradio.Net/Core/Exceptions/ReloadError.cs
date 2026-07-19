namespace Gradio.Net.Core.Exceptions;

public class ReloadError : Exception
{
    public ReloadError() : base()
    {
    }

    public ReloadError(string message) : base(message)
    {
    }
}

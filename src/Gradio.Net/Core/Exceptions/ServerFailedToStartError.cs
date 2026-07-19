namespace Gradio.Net.Core.Exceptions;

public class ServerFailedToStartError : Exception
{
    public ServerFailedToStartError() : base()
    {
    }

    public ServerFailedToStartError(string message) : base(message)
    {
    }
}

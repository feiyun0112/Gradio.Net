namespace Gradio.Net.Core.Exceptions;

public class TooManyRequestsError : Error
{
    public TooManyRequestsError() : base("Too many requests. Please try again later.")
    {
    }

    public TooManyRequestsError(string message) : base(message)
    {
    }
}

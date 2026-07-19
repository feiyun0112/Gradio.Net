namespace Gradio.Net.Core.Exceptions;

public class GradioVersionIncompatibleError : Exception
{
    public GradioVersionIncompatibleError() : base()
    {
    }

    public GradioVersionIncompatibleError(string message) : base(message)
    {
    }
}

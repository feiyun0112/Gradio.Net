namespace Gradio.Net;
/// <summary>
/// A workflow node that could not run, with a message fit for whoever asked for the answer
/// </summary>
/// <remarks>
/// <para>
/// The messages are written for a caller rather than for a log: an endpoint reports "&quot;Prompt&quot;
/// is required by &quot;shout&quot; but was not provided", because the caller is the one who can supply it.
/// That is why this is a type of its own rather than a general failure — the application turns it
/// into the error the user reads, and everything else stays an unexpected fault.
/// </para>
/// <para>
/// It is also what a graph raises when it cannot be run at all: a cycle has no order to run in, and
/// that is a fact about the caller's graph rather than a fault in the framework.
/// </para>
/// </remarks>
public sealed class WorkflowExecutionException : GradioException
{
    /// <summary>Creates the error.</summary>
    public WorkflowExecutionException()
        : base("The workflow could not be run.")
    {
    }

    /// <summary>Creates the error with a message.</summary>
    /// <param name="message">What went wrong, in the words the caller should read.</param>
    public WorkflowExecutionException(string message)
        : base(message)
    {
    }

    /// <summary>Creates the error with a message and the failure behind it.</summary>
    /// <param name="message">What went wrong, in the words the caller should read.</param>
    /// <param name="innerException">The failure this one describes.</param>
    public WorkflowExecutionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

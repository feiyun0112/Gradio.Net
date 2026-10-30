namespace Gradio.Net;
/// <summary>
/// A run history operation that failed, with the status a client should be told.
/// </summary>
/// <remarks>
/// The Python version's <c>HistoryError</c>. The status travels with the failure because the store
/// is the only thing that knows what went wrong: a missing bucket and an unreachable one are both
/// "the run could not be read", and a client is owed the difference between "ask for another
/// bucket" and "try again".
/// </remarks>
public sealed class RunHistoryException : GradioException
{
    /// <summary>Creates the failure.</summary>
    /// <param name="message">What went wrong.</param>
    /// <param name="status">The HTTP status a client should be told.</param>
    public RunHistoryException(string message, int status = 500)
        : base(message)
    {
        Status = status;
    }

    /// <summary>Creates the failure with the failure underneath it.</summary>
    /// <param name="message">What went wrong.</param>
    /// <param name="innerException">The failure this one describes.</param>
    /// <param name="status">The HTTP status a client should be told.</param>
    public RunHistoryException(string message, Exception innerException, int status = 500)
        : base(message, innerException)
    {
        Status = status;
    }

    /// <summary>The HTTP status a client should be told.</summary>
    public int Status { get; }
}

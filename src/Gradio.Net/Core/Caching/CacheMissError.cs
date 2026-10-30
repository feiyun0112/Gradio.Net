namespace Gradio.Net;
/// <summary>
/// What a cached function raises when it is asked only whether the answer is already known, and it
/// is not.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>CacheMissError</c>. A miss is not a failure — it is the answer "run it
/// for real" — which is why it is an exception rather than a return value: the check happens deep
/// inside a handler that is being run as if it were going to compute, and the only way out of it
/// without running anything is to throw.
/// </para>
/// <para>
/// The queue is what catches it: a cached binding is first run in probe mode, which answers from the
/// cache when it can and raises this when it cannot, and a run that raises falls through to the
/// ordinary path. An application that calls a cached function itself never sees it, because probe
/// mode is only ever entered by something that is prepared to catch it.
/// </para>
/// </remarks>
public sealed class CacheMissError : Exception
{
    /// <summary>Creates the error.</summary>
    public CacheMissError()
        : base("The answer is not in the cache.")
    {
    }

    /// <summary>Creates the error with a message.</summary>
    /// <param name="message">The message.</param>
    public CacheMissError(string message)
        : base(message)
    {
    }

    /// <summary>Creates the error with a message and an inner failure.</summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The failure this one describes.</param>
    public CacheMissError(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

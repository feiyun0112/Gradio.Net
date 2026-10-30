namespace Gradio.Net;
/// <summary>
/// The base of every failure this framework raises on purpose: the ones a caller is meant to
/// recognise, rather than the ones that mean something went wrong inside.
/// </summary>
/// <remarks>
/// <para>
/// The Python version has no such base — its exceptions are a flat set of <c>ValueError</c>,
/// <c>RuntimeError</c> and plain <c>Exception</c> subclasses, and the only distinction it draws is
/// the one that matters to a client: <c>gr.Error</c> is the application saying "no", and everything
/// else is a crash. That distinction is kept here as <see cref="Error"/>, which
/// derives from this type.
/// </para>
/// <para>
/// The base earns its place by making "the framework refused, and said why" catchable in one
/// clause. A host that wants to answer a refusal differently from a crash — a status, a log level,
/// a page — catches this and lets the rest through. A framework failure that is not a refusal
/// (an unreachable run-history store, a workflow graph that cannot run) is under it too, because
/// the alternative is a caller enumerating every type the framework happens to have.
/// </para>
/// <para>
/// It is abstract: there is no such thing as a failure of this type, only one of the kinds beneath
/// it.
/// </para>
/// </remarks>
public abstract class GradioException : Exception
{
    /// <summary>Creates a failure.</summary>
    /// <param name="message">What went wrong, in the words a caller reads.</param>
    protected GradioException(string message)
        : base(message)
    {
    }

    /// <summary>Creates a failure with the failure underneath it.</summary>
    /// <param name="message">What went wrong, in the words a caller reads.</param>
    /// <param name="innerException">The failure this one describes.</param>
    protected GradioException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

namespace Gradio.Net;
/// <summary>
/// The application saying "no": a failure a handler raises to tell the user what is wrong, in words
/// meant to be read rather than a stack trace meant to be debugged.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>gr.Error</c>. What makes it different from every other exception is not
/// that it carries a message — all of them do — but that the message is addressed to the person
/// using the application: the frontend draws it in a modal, with a title and for a length of time,
/// instead of the generic "there was an error" a crashed handler produces. That is also why it is
/// shown even when the application has not turned error text on: an application that said "no"
/// wanted it said.
/// </para>
/// <para>
/// The four values beside the message are the ones the modal is drawn with, and they travel in the
/// failure payload exactly as the Python version's do. <see cref="PrintException"/> is the fifth,
/// and it is about the operator rather than the visitor: an application that raises a refusal as
/// part of working — a validator, a form that is not filled in — does not want a traceback in its
/// log for every one of them.
/// </para>
/// <para>
/// The Python version's <c>str()</c> of one of these is <c>repr(message)</c>, which puts quotes
/// around it; the <see cref="Exception.Message"/> here is the message itself, because the only
/// thing the Python version does with that string is print it and this framework logs the message
/// directly. The payload carries the message either way, so nothing a client reads changes.
/// </para>
/// </remarks>
public sealed class Error : GradioException
{
    /// <summary>The message an error raised with none carries.</summary> <remarks>The Python
    /// version's default, kept because a client that groups these reads it.</remarks>
    public const string DefaultMessage = "Error raised.";

    /// <summary>The title an error raised with none is drawn under.</summary>
    public const string DefaultTitle = "Error";

    /// <summary>How long the modal is drawn for when the application says nothing.</summary>
    public const double DefaultDuration = 10;

    /// <summary>Creates the refusal.</summary>
    /// <param name="message">
    /// What to tell the user. May be HTML, which the modal renders rather than escapes.
    /// </param>
    /// <param name="duration">
    /// How long the modal stays for, in seconds, or <see langword="null"/> (or zero) for until the
    /// user closes it.
    /// </param>
    /// <param name="visible">Whether the message is drawn at all.</param>
    /// <param name="title">The title drawn above it.</param>
    /// <param name="printException">Whether the failure is written to the log as well.</param>
    public Error(
        string message = DefaultMessage,
        double? duration = DefaultDuration,
        bool visible = true,
        string title = DefaultTitle,
        bool printException = true)
        : base(message)
    {
        Duration = duration;
        Visible = visible;
        Title = title;
        PrintException = printException;
    }

    /// <summary>How long the modal stays for, in seconds, or <see langword="null"/> for until closed.</summary>
    public double? Duration { get; }

    /// <summary>Whether the message is drawn.</summary>
    public bool Visible { get; }

    /// <summary>The title drawn above the message.</summary>
    public string Title { get; }

    /// <summary>Whether the failure is written to the log.</summary>
    /// <remarks>
    /// A refusal that is part of the application working — a form that is not filled in — sets this
    /// to <see langword="false"/>: the user was told, and an operator does not need a traceback for
    /// every visitor who typed the wrong thing.
    /// </remarks>
    public bool PrintException { get; }
}

using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// What a failed run tells the client: nothing, a message the application meant for the user, or the
/// text of whatever went wrong.
/// </summary>
/// <remarks>
/// <para>
/// This is the Python version's <c>error_payload</c>, and the rule it encodes is the whole of what
/// <c>show_error</c> means. A run that failed with an <see cref="Error"/> says so to the user
/// whatever the setting is — the application raised it to be read. Anything else is a crash: the
/// payload carries no text unless the application turned error text on, in which case the message
/// is sent and marked visible. The keys that are absent are as much a part of it as the ones that
/// are: a client reading <c>duration</c> finds nothing when the failure was not a refusal.
/// </para>
/// <para>
/// The text is the exception's message rather than a stack trace, which is the Python version's
/// choice and the right one for a payload: a trace names the machine's directories and the
/// framework's internals, and the operator already has it in the log.
/// </para>
/// </remarks>
internal static class ErrorPayloads
{
    /// <summary>The payload for a failed run.</summary>
    /// <param name="error">What the handler threw, or <see langword="null"/> when nothing did.</param>
    /// <param name="showError">Whether the application sends the text of a crash to the client.</param>
    /// <returns>The payload.</returns>
    /// <remarks>
    /// An <see cref="Error"/> is shown whether or not <paramref name="showError"/> is set, which is
    /// the Python version's <c>show_error or isinstance(error, AppError)</c>: the switch is about
    /// crashes, and a refusal is not one.
    /// </remarks>
    internal static JsonObject Of(Exception? error, bool showError)
    {
        var payload = new JsonObject { ["error"] = null };

        if (error is null)
        {
            return payload;
        }

        if (error is Error refusal)
        {
            payload["error"] = refusal.Message;
            payload["duration"] = refusal.Duration;
            payload["visible"] = refusal.Visible;
            payload["title"] = refusal.Title;

            return payload;
        }

        if (showError)
        {
            payload["error"] = error.Message;
            payload["visible"] = true;
        }

        return payload;
    }

    /// <summary>Whether a failure is one the operator's log should carry.</summary>
    /// <param name="error">What the handler threw.</param>
    /// <returns>
    /// <see langword="false"/> only for a refusal that said it was not to be printed; <see
    /// langword="true"/> for everything else.
    /// </returns>
    /// <remarks>
    /// The Python version's <c>if not isinstance(error, Error) or error.print_exception:
    /// traceback.print_exc()</c>. The refusal is already on its way to the user; a traceback for it
    /// would be noise, and an application that validates a form would produce one per visitor.
    /// </remarks>
    internal static bool IsLogWorthy(Exception error) => error is not Error { PrintException: false };
}

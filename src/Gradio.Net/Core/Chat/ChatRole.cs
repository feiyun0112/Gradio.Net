namespace Gradio.Net;
/// <summary>
/// The roles a chat message may have, as the Python version spells them.
/// </summary>
/// <remarks>
/// A role is not free text: the frontend aligns a message by it, and a fourth value would be a
/// message it cannot place. The Python version's model accepts exactly these three, and so does
/// this one.
/// </remarks>
public static class ChatRole
{
    /// <summary>The person using the application.</summary>
    public const string User = "user";

    /// <summary>The application, or the model behind it.</summary>
    public const string Assistant = "assistant";

    /// <summary>An instruction the conversation is prefixed with.</summary>
    public const string System = "system";

    /// <summary>Whether a role is one the Python version knows.</summary> <param name="role">The
    /// role.</param> <returns>Whether it is <see cref="User"/>, <see cref="Assistant"/> or
    /// <see cref="System"/>.</returns>
    public static bool IsKnown(string role) =>
        role is User or Assistant or System;
}

/// <summary>
/// The status a thought carries, as the Python version spells it.
/// </summary>
/// <remarks>
/// It says whether the work behind a message is still going: <see cref="Pending"/> makes the
/// frontend show a spinner and open the message, <see cref="Done"/> closes it. A message without a
/// status is open and has no spinner.
/// </remarks>
public static class MessageStatus
{
    /// <summary>The work is still running.</summary>
    public const string Pending = "pending";

    /// <summary>The work is finished.</summary>
    public const string Done = "done";

    /// <summary>Whether a status is one the Python version knows.</summary>
    /// <param name="status">The status.</param>
    /// <returns>Whether it is <see cref="Pending"/> or <see cref="Done"/>.</returns>
    public static bool IsKnown(string status) => status is Pending or Done;
}

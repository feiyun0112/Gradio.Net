namespace Gradio.Net;
/// <summary>
/// What kind of file a loaded chat accepts from the user.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>file_types</c> is <c>"text_encoded"</c>, <c>"image"</c>, a list of
/// those, or <c>None</c>. The two kinds are not the same thing to the endpoint that receives them:
/// a text file is read and appended to the prompt, and an image is sent as an image. Asking for
/// both is asking for both, so the parameter is a set rather than a list, and <see cref="None"/> is
/// the empty set — a chat with no uploads at all.
/// </para>
/// <para>
/// The Python version refuses a kind it does not know at run time, because a string can say
/// anything. Here the kinds are the enum's members, so a name that is not one of them cannot be
/// written — and a value built by casting a number the enum does not define is refused where the
/// chat is loaded, rather than becoming a field that accepts nothing.
/// </para>
/// </remarks>
[Flags]
public enum ChatFileType
{
    /// <summary>No files at all: the field takes text, as a chat without uploads does.</summary>
    None = 0,

    /// <summary>
    /// Any text-encoded file, which is read and appended to the prompt rather than uploaded.
    /// </summary>
    /// <remarks>
    /// The extensions are the Python version's list — source, markup, configuration and data files
    /// — and a file that ends in one is read as text. This is the default, because it needs no
    /// multimodal model: a text file is part of the message rather than an attachment.
    /// </remarks>
    TextEncoded = 1,

    /// <summary>Images, sent to the endpoint as a data URL rather than as a path.</summary>
    Image = 2,
}

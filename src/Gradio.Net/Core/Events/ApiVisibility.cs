namespace Gradio.Net;
/// <summary>
/// How far an event binding's endpoint is exposed to clients
/// </summary>
/// <remarks>
/// The Python version's three-way switch, spelled as an enum so that "undocumented" cannot be
/// reached by a typo in a string. It travels into the config verbatim — the frontend reads the
/// string — so the wire spelling lives in <see cref="Dependency"/> rather than here.
/// </remarks>
public enum ApiVisibility
{
    /// <summary>Shown in the API documentation and callable by clients.</summary>
    Public,

    /// <summary>Hidden from the API documentation and not callable by clients.</summary>
    Private,

    /// <summary>Hidden from the API documentation but still callable by clients.</summary>
    Undocumented,
}

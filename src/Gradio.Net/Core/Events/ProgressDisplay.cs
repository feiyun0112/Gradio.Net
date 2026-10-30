namespace Gradio.Net;
/// <summary>
/// How much of the progress animation an event binding shows while it runs.
/// </summary>
/// <remarks>
/// The Python version's <c>show_progress</c>, spelled as an enum. The three names are the wire
/// spellings, so the enum member names are not used on the wire; see <see cref="Dependency"/>.
/// </remarks>
public enum ProgressDisplay
{
    /// <summary>A spinner over the output plus the runtime display.</summary>
    Full,

    /// <summary>Only the runtime display.</summary>
    Minimal,

    /// <summary>No progress animation at all.</summary>
    Hidden,
}

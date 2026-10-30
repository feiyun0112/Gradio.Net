namespace Gradio.Net;
/// <summary>
/// Marks an endpoint the mapping loop put behind the sign-in gate.
/// </summary>
/// <remarks>
/// <para>
/// The route table says which routes the Python version guards; this says which ones the host
/// actually wrapped. They are two statements rather than one because they can disagree — a
/// registration loop that reads the flag but forgets to wrap would leave every guarded route open
/// while the table still read correctly — and because the endpoint export has to report what the
/// host did rather than repeat what the table asked for.
/// </para>
/// <para>
/// The marker is stamped by the same branch that wraps the handler, so the two cannot drift apart
/// without the code that keeps them together changing.
/// </para>
/// </remarks>
internal sealed class GradioLoginRequired
{
    private GradioLoginRequired()
    {
    }

    /// <summary>The metadata instance every guarded endpoint carries.</summary>
    internal static GradioLoginRequired Instance { get; } = new();
}

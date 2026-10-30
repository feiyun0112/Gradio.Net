namespace Gradio.Net;
/// <summary>
/// How much a cache keeps, and whether it keeps it per session.
/// </summary>
/// <remarks>
/// The Python version's three keyword arguments to <c>gr.cache</c> and <c>gr.Cache</c>, as one
/// object: a decorator cannot carry optional arguments in C#, so the choices travel together.
/// </remarks>
public sealed class CacheOptions
{
    /// <summary>The most entries the cache keeps; least recently used ones go first.</summary>
    /// <value>128, which is the Python version's default. Zero keeps everything.</value>
    public int MaxSize { get; init; } = 128;

    /// <summary>
    /// The most memory the entries may add up to, in bytes, or <see langword="null"/> for no limit.
    /// </summary>
    /// <remarks>
    /// An estimate rather than a measurement: a value's own size is known for the shapes that carry
    /// one — text, bytes, a JSON document, a sequence, a mapping — and a fixed allowance stands in
    /// for everything else. The Python version estimates the same way.
    /// </remarks>
    public long? MaxMemory { get; init; }

    /// <summary>Whether each session gets its own namespace.</summary>
    /// <remarks>
    /// A shared cache can hand one user's answer to another, which is right for a model and wrong
    /// for a conversation. The limits apply to the sum of every session's entries, so a busy
    /// application still has one bound rather than one per visitor.
    /// </remarks>
    public bool PerSession { get; init; }
}

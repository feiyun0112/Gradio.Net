namespace Gradio.Net;
/// <summary>
/// A stream that is cached as it is produced: the next caller is handed the chunks again rather than
/// the work.
/// </summary>
/// <remarks>
/// <para>
/// The Python version caches a generator by keeping every value it yielded and replaying them on a
/// hit, which is what makes a cached streaming endpoint answer immediately the second time. The
/// same thing happens here, at the level of the sequence rather than of the handler: the first
/// caller's enumeration records, and the recording is what a later caller replays.
/// </para>
/// <para>
/// A stream that is abandoned part-way records nothing, which is the Python version's behaviour
/// too: what is cached is a complete answer, and half a stream replayed as if it were whole would
/// be a different answer.
/// </para>
/// </remarks>
internal sealed class CachedStream<T> : IAsyncEnumerable<T>
{
    private readonly IAsyncEnumerable<T>? _source;
    private readonly IReadOnlyList<T>? _cached;
    private readonly Action<IReadOnlyList<T>>? _store;

    /// <summary>Creates a stream that records what it produces.</summary>
    /// <param name="source">The stream to read from.</param>
    /// <param name="cached">Nothing, for a stream that is being produced for the first time.</param>
    /// <param name="store">Where to keep the chunks once the stream has run out.</param>
    internal CachedStream(IAsyncEnumerable<T>? source, IReadOnlyList<T>? cached, Action<IReadOnlyList<T>>? store)
    {
        _source = source;
        _cached = cached;
        _store = store;
    }

    /// <summary>Creates a stream that replays what was recorded.</summary>
    /// <param name="cached">The chunks to replay.</param>
    internal CachedStream(IReadOnlyList<T> cached)
        : this(source: null, cached, store: null)
    {
    }

    /// <inheritdoc />
    public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        if (_cached is not null)
        {
            foreach (var chunk in _cached)
            {
                cancellationToken.ThrowIfCancellationRequested();

                yield return chunk;
            }

            yield break;
        }

        var recorded = new List<T>();

        await foreach (var chunk in _source!.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            recorded.Add(chunk);

            yield return chunk;
        }

        _store?.Invoke(recorded);
    }
}

namespace Gradio.Net.Utils;

public class SyncToAsyncIterator<T> : IAsyncEnumerable<T>
{
    private readonly IEnumerator<T> _iterator;

    public SyncToAsyncIterator(IEnumerator<T> iterator)
    {
        _iterator = iterator;
    }

    public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        while (true)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                yield break;
            }

            var hasNext = await Task.Run(() => _iterator.MoveNext(), cancellationToken);

            if (!hasNext)
            {
                yield break;
            }

            yield return _iterator.Current;
        }
    }
}

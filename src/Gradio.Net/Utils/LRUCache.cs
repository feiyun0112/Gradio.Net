
namespace Gradio.Net.Utils;

public class LRUCache<TKey, TValue>
{
    private readonly int _maxSize;
    private readonly Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>> _cache;
    private readonly LinkedList<KeyValuePair<TKey, TValue>> _order;

    public LRUCache(int maxSize = 100)
    {
        _maxSize = maxSize;
        _cache = new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>();
        _order = new LinkedList<KeyValuePair<TKey, TValue>>();
    }

    public TValue this[TKey key]
    {
        get
        {
            if (_cache.TryGetValue(key, out var node))
            {
                _order.Remove(node);
                _order.AddLast(node);
                return node.Value.Value;
            }
            throw new KeyNotFoundException();
        }
        set
        {
            if (_cache.TryGetValue(key, out var node))
            {
                _order.Remove(node);
                _cache.Remove(key);
            }
            else if (_cache.Count >= _maxSize)
            {
                var firstNode = _order.First;
                _order.RemoveFirst();
                _cache.Remove(firstNode.Value.Key);
            }

            var newNode = new LinkedListNode<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(key, value));
            _order.AddLast(newNode);
            _cache[key] = newNode;
        }
    }

    public int Count => _cache.Count;

    public void Clear()
    {
        _cache.Clear();
        _order.Clear();
    }

    public bool ContainsKey(TKey key)
    {
        return _cache.ContainsKey(key);
    }
}

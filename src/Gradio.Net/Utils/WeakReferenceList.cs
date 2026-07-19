namespace Gradio.Net.Utils;

public class WeakReferenceList<T> where T : class
{
    private readonly List<WeakReference<T>> _list = new();

    public void Add(T item)
    {
        _list.Add(new WeakReference<T>(item));
    }

    public bool Remove(T item)
    {
        for (int i = 0; i < _list.Count; i++)
        {
            if (_list[i].TryGetTarget(out var target) && target == item)
            {
                _list.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    public List<T> ToList()
    {
        var result = new List<T>();
        for (int i = _list.Count - 1; i >= 0; i--)
        {
            if (_list[i].TryGetTarget(out var target))
            {
                result.Add(target);
            }
            else
            {
                // Clean up dead references
                _list.RemoveAt(i);
            }
        }
        return result;
    }

    public int Count
    {
        get
        {
            // Clean up dead references while counting
            for (int i = _list.Count - 1; i >= 0; i--)
            {
                if (!_list[i].TryGetTarget(out _))
                {
                    _list.RemoveAt(i);
                }
            }
            return _list.Count;
        }
    }
}

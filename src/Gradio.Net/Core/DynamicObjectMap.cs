using System.Reflection;

namespace Gradio.Net.Core;

public class DynamicObjectMap
{
    private readonly Dictionary<string, object> _dict;

    public DynamicObjectMap(Dictionary<string, object> dict)
    {
        _dict = dict;
        foreach (var (key, value) in dict)
        {
            if (value is Dictionary<string, object> nestedDict)
            {
                SetProperty(key, new DynamicObjectMap(nestedDict));
            }
            else if (value is List<object> nestedList)
            {
                var processedList = new List<object>();
                foreach (var item in nestedList)
                {
                    if (item is Dictionary<string, object> itemDict)
                    {
                        processedList.Add(new DynamicObjectMap(itemDict));
                    }
                    else
                    {
                        processedList.Add(item);
                    }
                }
                SetProperty(key, processedList);
            }
            else
            {
                SetProperty(key, value);
            }
        }
    }

    private void SetProperty(string name, object value)
    {
        var propertyInfo = GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);
        if (propertyInfo != null)
        {
            propertyInfo.SetValue(this, value);
        }
        else
        {
            _dict[name] = value;
        }
    }

    public object this[string key]
    {
        get => _dict[key];
        set => _dict[key] = value;
    }

    public bool ContainsKey(string key) => _dict.ContainsKey(key);

    public object Get(string key, object defaultValue = null)
    {
        return _dict.TryGetValue(key, out var value) ? value : defaultValue;
    }

    public ICollection<string> Keys => _dict.Keys;

    public ICollection<object> Values => _dict.Values;

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _dict.GetEnumerator();

    public override string ToString() => _dict.ToString();

    public override int GetHashCode() => _dict.GetHashCode();

    public override bool Equals(object obj) => _dict.Equals(obj);

    public object Pop(string key, object defaultValue = null)
    {
        if (_dict.TryGetValue(key, out var value))
        {
            _dict.Remove(key);
            return value;
        }
        return defaultValue;
    }
}

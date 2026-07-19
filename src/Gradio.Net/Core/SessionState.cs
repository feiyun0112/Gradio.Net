using System.Collections;

namespace Gradio.Net.Core;

public class SessionState
{
    public object BlocksConfig { get; private set; }
    private readonly Dictionary<int, object> _stateData = new();
    private readonly Dictionary<int, Block> _blockOverrides = new();
    private readonly Dictionary<int, (int TimeToLive, DateTime CreatedAt)> _stateTtl = new();
    private readonly Dictionary<int, object> _configValues = new();
    private readonly Blocks _blocks;
    public bool IsClosed { get; set; } = false;
    private readonly int _stateTtlWhenClosed;

    public SessionState(Blocks blocks)
    {
        _blocks = blocks;
        BlocksConfig = blocks.DefaultConfig;

        if (blocks.BlocksDict != null)
        {
            foreach (var kvp in blocks.BlocksDict)
                _configValues[kvp.Key] = CreateConfigForBlock(kvp.Key, kvp.Value);
        }

        _stateTtlWhenClosed = Environment.GetEnvironmentVariable("GRADIO_IS_E2E_TEST") != null ? 1 : 3600;
    }

    public Dictionary<int, object> ConfigValues => _configValues;

    private object CreateConfigForBlock(int key, Block block) => new Dictionary<string, object>
    {
        { "id", key },
        { "type", block.GetBlockClass() },
        { "props", block.GetConfig() }
    };

    public object this[int key]
    {
        get => GetValue(key);
        set => SetValue(key, value);
    }

    private object GetValue(int key)
    {
        if (_blocks.BlocksDict == null || !_blocks.BlocksDict.TryGetValue(key, out Block? block))
            return null!;

        if (_blockOverrides.TryGetValue(key, out var overriddenBlock))
            block = overriddenBlock;

        if (IsStatefulBlock(block))
        {
            if (!_stateData.ContainsKey(key))
            {
                var defaultValue = GetBlockValue(block);
                if (defaultValue != null)
                    _stateData[key] = DeepCopyValue(defaultValue);
            }
            return _stateData.TryGetValue(key, out var value) ? value : null!;
        }

        return block;
    }

    private void SetValue(int key, object value)
    {
        if (_blocks.BlocksDict == null || !_blocks.BlocksDict.TryGetValue(key, out Block? block))
            return;

        if (IsStatefulBlock(block))
        {
            _stateTtl[key] = (GetTimeToLive(block), DateTime.Now);
            _stateData[key] = value;
        }
        else if (value is Block updatedBlock)
        {
            _blockOverrides[key] = updatedBlock;
        }

        UpdateConfig(key);
    }

    public bool Contains(int key)
    {
        if (_blocks.BlocksDict == null || !_blocks.BlocksDict.TryGetValue(key, out Block? block))
            return false;

        return IsStatefulBlock(block)
            ? _stateData.ContainsKey(key)
            : _blockOverrides.ContainsKey(key) || _blocks.BlocksDict.ContainsKey(key);
    }

    public void UpdateConfig(int key)
    {
        var blockValue = this[key];
        if (blockValue == null) return;

        if (blockValue is Block overridden)
            _configValues[key] = CreateConfigForBlock(key, overridden);
        else if (_blocks.BlocksDict != null && _blocks.BlocksDict.TryGetValue(key, out Block? block))
            _configValues[key] = CreateConfigForBlock(key, block);
    }

    public void UpdateValueInConfig(int key, object value)
    {
        if (!_configValues.ContainsKey(key) && _blocks.BlocksDict != null && _blocks.BlocksDict.TryGetValue(key, out Block? block))
            _configValues[key] = CreateConfigForBlock(key, block);

        if (_configValues.TryGetValue(key, out var config) && config is Dictionary<string, object> configDict)
        {
            if (configDict.TryGetValue("props", out var propsObj) && propsObj is Dictionary<string, object> props)
                props["value"] = value;
        }
    }

    private bool IsStatefulBlock(Block block)
    {
        var statefulProp = block.GetType().GetProperty("Stateful");
        if (statefulProp?.GetValue(block) is bool stateful)
            return stateful;
        return IsStateComponent(block);
    }

    private static bool IsStateComponent(Block block)
    {
        var typeName = block.GetType().Name;
        return typeName == "State" || typeName.EndsWith("State");
    }

    private static object? GetBlockValue(Block block)
        => block.GetType().GetProperty("Value")?.GetValue(block);

    private static int GetTimeToLive(Block block)
        => block.GetType().GetProperty("TimeToLive")?.GetValue(block) is int ttl ? ttl : 3600;

    private static object DeepCopyValue(object value)
    {
        if (value == null) return null!;

        var type = value.GetType();
        if (type.IsPrimitive || type == typeof(string))
            return value;

        if (value is IList list)
        {
            var newList = (IList)Activator.CreateInstance(type)!;
            foreach (var item in list)
                newList.Add(DeepCopyValue(item));
            return newList;
        }

        if (value is IDictionary dict)
        {
            var newDict = (IDictionary)Activator.CreateInstance(type)!;
            foreach (DictionaryEntry entry in dict)
                newDict[entry.Key] = DeepCopyValue(entry.Value!);
            return newDict;
        }

        try
        {
            var method = type.GetMethod("MemberwiseClone", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (method != null)
                return method.Invoke(value, null)!;
        }
        catch { }

        return value;
    }

    public void DeleteState(bool expiredOnly = false)
    {
        var toDelete = new List<int>();

        foreach (var kvp in _stateTtl.ToList())
        {
            int key = kvp.Key;
            var (timeToLive, createdAt) = kvp.Value;

            if (IsClosed) timeToLive = _stateTtlWhenClosed;

            bool isExpired = (DateTime.Now - createdAt).TotalSeconds > timeToLive;

            if (!expiredOnly || isExpired)
            {
                if (_blocks.BlocksDict != null && _blocks.BlocksDict.TryGetValue(key, out Block? block))
                    CallDeleteCallback(block, _stateData.TryGetValue(key, out var value) ? value : null);
                toDelete.Add(key);
            }
        }

        foreach (int key in toDelete)
        {
            _stateData.Remove(key);
            _stateTtl.Remove(key);
            _blockOverrides.Remove(key);
            _configValues.Remove(key);
        }
    }

    private static void CallDeleteCallback(Block block, object? value)
    {
        if (value == null) return;
        var deleteCallbackMethod = block.GetType().GetMethod("DeleteCallback");
        if (deleteCallbackMethod != null)
        {
            try { deleteCallbackMethod.Invoke(block, new[] { value }); }
            catch { }
        }
    }

    public IEnumerable<object> Components
    {
        get
        {
            foreach (var config in ConfigValues.Values)
                if (config != null)
                    yield return config;
        }
    }

    public IEnumerable<(Block Block, object Value, bool Expired)> StateComponents
    {
        get
        {
            var toDelete = new List<int>();

            foreach (var kvp in _stateData.ToList())
            {
                int key = kvp.Key;

                if (_blocks.BlocksDict == null || !_blocks.BlocksDict.TryGetValue(key, out Block? block))
                {
                    toDelete.Add(key);
                    continue;
                }

                if (IsStateComponent(block) && _stateTtl.TryGetValue(key, out var ttlInfo))
                {
                    var (timeToLive, createdAt) = ttlInfo;
                    if (IsClosed) timeToLive = _stateTtlWhenClosed;

                    bool expired = (DateTime.Now - createdAt).TotalSeconds > timeToLive;
                    yield return (block, kvp.Value, expired);
                }
            }

            foreach (int key in toDelete)
            {
                _stateData.Remove(key);
                _stateTtl.Remove(key);
                _blockOverrides.Remove(key);
                _configValues.Remove(key);
            }
        }
    }
}

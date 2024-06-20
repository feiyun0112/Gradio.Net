
using Gradio.Net.Enums;
using System.Collections;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json;
using FnTarget = (int? Id, string EventName);

namespace Gradio.Net;

public class Blocks : Block, IList<Block>, IDisposable
{
    private readonly List<Block> _children = [];
    internal Blocks()
    {
    }
    [IgnoreDataMember]
    internal Blocks ParentBlocks { get; set; }

    public int Count => _children.Count;

    public bool IsReadOnly => false;

    internal string Title { get; set; }
    internal string Theme { get; set; }
    internal bool? AnalyticsEnabled { get; set; }
    internal string Mode { get; set; }
    internal string Css { get; set; }
    internal string Js { get; set; }
    internal string Head { get; set; }
    internal bool? FillHeight { get; set; }
    internal Tuple<int, int> DeleteCache { get; set; }

    public virtual void Add(Block item)
    {
        if (item is Blocks blocks)
        {
            blocks.ParentBlocks = this;
        }
        _children.Add(item);
    }

    public void Clear()
    {
        _children.Clear();
    }

    public bool Contains(Block item)
    {
        return _children.Contains(item);
    }

    public void CopyTo(Block[] array, int arrayIndex)
    {
        _children.CopyTo(array, arrayIndex);
    }

    public virtual void Dispose()
    {
        Context.SetCurrentBlocks(null);
    }

    internal Dictionary<string, object> GetConfig()
    {
        Dictionary<string, object> config = new()
        {
            ["mode"] = this.GetPropertyValue<object>(nameof(this.Mode)),
            ["dev_mode"] = false,
            ["analytics_enabled"] = this.GetPropertyValue<object>(nameof(this.AnalyticsEnabled)),
            ["components"] = GetComponents(),
            ["css"] = this.GetPropertyValue<object>(nameof(this.Css)),
            ["connect_heartbeat"] = false,
            ["js"] = this.GetPropertyValue<object>(nameof(this.Js)),
            ["head"] = this.GetPropertyValue<object>(nameof(this.Head)),
            ["title"] = this.GetPropertyValue<object>(nameof(this.Title)),
            ["space_id"] = null,
            ["enable_queue"] = true,
            ["show_error"] = false,
            ["show_api"] = true,
            ["is_colab"] = false,
            ["max_file_size"] = null,

            ["theme"] = this.GetPropertyValue<object>(nameof(this.Theme)),
            ["protocol"] = "sse_v3",

            ["fill_height"] = this.GetPropertyValue<object>(nameof(this.FillHeight)),

            ["layout"] = GetLayout(),

            ["dependencies"] = GetDependencies()
        };

        return config;
    }

    internal List<Dictionary<string, object>> GetComponents()
    {
        List<Dictionary<string, object>> result = [];
        foreach (Block item in _children)
        {
            result.Add(item.GetConfig());
            if (item is Blocks blocks)
            {

                result.AddRange(blocks.GetComponents());
            }
        }

        return result;
    }

    internal override Dictionary<string, object> GetLayout()
    {
        Dictionary<string, object> result = new()
        {
            ["id"] = Id
        };
        List<Dictionary<string, object>> childrenLayout = [];
        foreach (Block item in _children)
        {
            childrenLayout.Add(item.GetLayout());
        }
        result["children"] = childrenLayout;

        return result;
    }

    public IEnumerator<Block> GetEnumerator()
    {
        return _children.GetEnumerator();
    }

    public bool Remove(Block item)
    {
        return _children.Remove(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _children.GetEnumerator();
    }

    internal (BlockFunction, int) SetEventTrigger(
        IEnumerable<EventListenerMethod> targets,
        Func<Input, Task<Output>> fn = null,
        Func<Input, IAsyncEnumerable<Output>> streamingFn = null,
        IEnumerable<Component> inputs = null,
        IEnumerable<Component> outputs = null,
        bool preprocess = true,
        bool postprocess = true,
        bool scrollToOutput = false,
        ShowProgress showProgress = ShowProgress.Full,
        string apiName = null,
        string js = null,
        bool no_target = false,
        bool? queue = null,
        bool batch = false,
        int maxBatchSize = 4,
        List<int> cancels = null,
        decimal? every = null,
        bool? collects_event_data = null,
        int? trigger_after = null,
        bool trigger_only_on_success = false,
        TriggerMode? triggerMode = TriggerMode.Once,
        ConcurrencyLimit? concurrencyLimit = null,
        string concurrencyId = null,
        bool showApi = true)
    {
        // Support for singular parameter
        List<FnTarget> tmpTargets = targets.Select(target => (!no_target && target.Block != null) ? new FnTarget(target.Block.Id, target.EventName) : new FnTarget(null, target.EventName)).ToList();

        inputs = inputs == null ? [] : inputs.ToList();
        outputs = outputs == null ? [] : outputs.ToList();

        if (fn != null && (cancels == null || cancels.Count == 0))
        {
            //todo:
            //CheckFunctionInputsMatch(fn, inputs);
        }

        if (every != null && every <= 0)
        {
            throw new ArgumentException("Parameter 'every' must be positive or null");
        }

        if (every != null && batch)
        {
            throw new ArgumentException($"Cannot run event in a batch and every {every} seconds. Either batch is true or every is non-zero but not both.");
        }

        if (every != null && fn != null)
        {
            throw new NotImplementedException($"every {every} for fn");
            //todo:
            //fn = GetContinuousFn(fn, every.Value);
        }
        else if (every != null)
        {
            throw new ArgumentException("Cannot set a value for 'every' without a 'fn'.");
        }

        if (every != null && concurrencyLimit != null)
        {
            if (concurrencyLimit == ConcurrencyLimit.Default)
            {
                concurrencyLimit = null;
            }
            else
            {
                throw new ArgumentException("Cannot set a value for 'concurrencyLimit' with 'every'.");
            }
        }

        if (tmpTargets[0].EventName == "change" || tmpTargets[0].EventName == "key_up")
        {
            triggerMode ??= TriggerMode.AlwaysLast;
        }
        else
        {
            triggerMode ??= TriggerMode.Once;
        }

        (IList _, int? progressIndex, int? eventDataIndex) = fn != null ? SpecialArgs(fn) : (null, null, null);

        if (apiName != null && apiName.Trim() == "")
        {
            if (fn != null)
            {
                apiName = "unnamed";
            }
            else if (js != null)
            {
                apiName = "js_fn";
                showApi = false;
            }
            else
            {
                apiName = "unnamed";
                showApi = false;
            }
        }

        if (collects_event_data == null)
        {
            collects_event_data = eventDataIndex != null;
        }

        if (fn == null && streamingFn == null)
        {
            throw new InvalidOperationException("Please set fn or streamingFn");
        }
        if (fn != null && streamingFn != null)
        {
            throw new InvalidOperationException("Please set fn or streamingFn");
        }

        BlockFunction blockFn = new()
        {
            Fn = fn,
            StreamingFn = streamingFn,
            Inputs = inputs,
            Outputs = outputs,
            Preprocess = preprocess,
            Postprocess = postprocess,
            InputsAsDict = false,
            Targets = tmpTargets,
            Batch = batch,
            MaxBatchSize = maxBatchSize,
            ConcurrencyLimit = concurrencyLimit,
            ConcurrencyId = concurrencyId,
            TracksProgress = progressIndex != null,
            ApiName = apiName,
            Js = js,
            ShowProgress = showProgress,
            Every = every,
            Cancels = cancels,
            CollectsEventData = collects_event_data,
            TriggerAfter = trigger_after,
            TriggerOnlyOnSuccess = trigger_only_on_success,
            TriggerMode = triggerMode,
            Queue = queue,
            ScrollToOutput = scrollToOutput,
            ShowApi = showApi
        };

        this.Fns.Add(blockFn);
        return (blockFn, this.Fns.Count - 1);
    }

    internal IEnumerable<Dictionary<string, object>> GetDependencies()
    {
        List<Dictionary<string, object>> result = [];
        int id = 0;
        foreach (BlockFunction func in Fns)
        {
            Dictionary<string, object> dependency = func.GetConfig();
            dependency["id"] = id++;
            result.Add(dependency);

        }

        return result;
    }

    [IgnoreDataMember]
    internal List<BlockFunction> Fns { get; set; } = [];

    public Block this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    private (IList, int? progressIndex, int? eventDataIndex) SpecialArgs(Func<Input, Task<Output>> fn)
    {
        //todo:Checks if function has special arguments Request or EventData (via annotation) or Progress (via default value).
        return (null, null, null);
    }

    static Dictionary<string, object> _defaultProps = new Dictionary<string, object>()
    {
        { nameof(Theme), "default" },
        { nameof(AnalyticsEnabled), true },
        { nameof(Mode), "blocks" },
        { nameof(Title), "Gradio.Net" },
        { nameof(FillHeight), false },
    };
    protected override object? GetDefaultProp(string name) => _defaultProps.ContainsKey(name) ? _defaultProps[name] : null;

    protected override Dictionary<string, object> GetProps(bool useDefaultValue)
    {
        Dictionary<string, object> result = base.GetProps(useDefaultValue);
        if (result.ContainsKey("id"))
        {
            result.Remove("id");
        }

        if (result.ContainsKey("elem_classes"))
        {
            result.Remove("elem_classes");
        }

        return result;
    }

    public int IndexOf(Block item)
    {
        return _children.IndexOf(item);
    }

    public void Insert(int index, Block item)
    {
        _children.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        _children.RemoveAt(index);
    }
}

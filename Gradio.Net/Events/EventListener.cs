using Gradio.Net.Enums;

namespace Gradio.Net;

internal class EventListener
{
    internal EventListener(string event_name,
    bool has_trigger=true,
    Func<Dictionary<string,object>> configData = null,
    ShowProgress showProgress= ShowProgress.Full,
    Func<Block, Task> callback=null,
    int ?trigger_after=null,
    bool trigger_only_on_success=false,
    string doc= "") {
        this.HasTrigger = has_trigger;
        this.ConfigData = configData;
        this.EventName = event_name;
        this.ShowProgress = showProgress;
        this.TriggerAfter = trigger_after;
        this.TriggerOnlyOnSuccess = trigger_only_on_success;
        this.Callback = callback;
        this.Doc = doc;
    }

    public bool HasTrigger { get; }
    public Func<Dictionary<string, object>> ConfigData { get; }
    public string EventName { get; }
    public ShowProgress ShowProgress { get; }
    public int? TriggerAfter { get; }
    public bool TriggerOnlyOnSuccess { get; }
    public Func<Block,Task> Callback { get; }
    public string Doc { get; }


    internal async Task<Dependency> EventTrigger(
        Block block,
        Func<Input,Task<Output>> fn = null,
         Func<Input,IAsyncEnumerable<Output>> streamingFn = null,
        IEnumerable<Component> inputs=null,
        IEnumerable<Component> outputs=null,
        string apiName=null,
        bool scrollToOutput=false,
        ShowProgress? showProgress=null,
        bool? queue=null,
        bool batch=false,
        int maxBatchSize= 4,
        bool preprocess = true,
        bool postprocess=true,
        Dictionary<string,object> cancels=null,
        decimal? every=null,
        TriggerMode? triggerMode=null,
        string js=null,
        ConcurrencyLimit concurrencyLimit= ConcurrencyLimit.Default,
        string concurrencyId=null,
        bool showApi=true
    )
    {
        (BlockFunction dep, int dep_index) = Context.RootBlock.SetEventTrigger(
            [new EventListenerMethod(this.HasTrigger?block:null,this.EventName)],
            fn,
            streamingFn,
            inputs,
            outputs,
            preprocess : preprocess,
            postprocess : postprocess,
            scrollToOutput : scrollToOutput,
            showProgress : showProgress?? this.ShowProgress,
            apiName : apiName,
            js : js,
            concurrencyLimit : concurrencyLimit,
            concurrencyId : concurrencyId,
            queue : queue,
            batch : batch,
            maxBatchSize : maxBatchSize,
            every : every,
            trigger_after : TriggerAfter,
            trigger_only_on_success : TriggerOnlyOnSuccess,
            triggerMode : triggerMode,
            showApi : showApi
        );
        
        if (Callback!=null)
        {
            await Callback(block);
        }

        return new Dependency(block, dep.GetConfig(), dep_index, fn);
    }
}



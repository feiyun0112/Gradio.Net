using Gradio.Net.Enums;

using FnTarget = (int? Id, string EventName);

namespace Gradio.Net;

internal class BlockFunction
{
    internal bool Preprocess { get;  set; }
    internal bool ShowApi { get;  set; }
    internal bool ScrollToOutput { get;  set; }
    internal bool? Queue { get;  set; }
    internal bool TriggerOnlyOnSuccess { get;  set; }
    internal int? TriggerAfter { get;  set; }
    internal bool? CollectsEventData { get;  set; }
    internal IEnumerable<int> Cancels { get;  set; }
    internal decimal? Every { get;  set; }
    internal bool TracksProgress { get;  set; }
    internal string ConcurrencyId { get;  set; }
    internal ConcurrencyLimit? ConcurrencyLimit { get;  set; }
    internal bool Batch { get;  set; }
    internal bool InputsAsDict { get;  set; }
    internal bool Postprocess { get;  set; }
    internal Func<Input,Task<Output>>? Fn { get;  set; }
    internal Func<Input,IAsyncEnumerable<Output>>? StreamingFn { get; set; }
    internal string? Js { get;  set; }
    internal bool TypesContinuous { get;  set; }
    internal bool TypesGenerator { get;  set; }
    internal bool ZeroGpu { get; set; }
    internal TriggerMode? TriggerMode { get; set; }
    internal int MaxBatchSize { get; set; } = 4;
    internal ShowProgress ShowProgress { get; set; } = ShowProgress.Full;
    internal string ApiName { get; set; }
    internal IEnumerable<Component> Inputs { get; set; }
    internal IEnumerable<Component> Outputs { get; set; }
    internal IEnumerable<FnTarget> Targets { get; set; }

    internal Dictionary<string, object> GetConfig()
    {
        Dictionary<string, object> result = new()
        {
            ["targets"] = GetTargetsConfig(),
            ["inputs"] = Inputs.Select(p => p.Id).ToArray(),
            ["outputs"] = Outputs.Select(p => p.Id).ToArray(),
            ["backend_fn"] = (this.Fn != null || this.StreamingFn != null),
            ["js"] = this.Js,
            ["queue"] = this.Queue,
            ["api_name"] = ApiName,
            ["scroll_to_output"] = this.ScrollToOutput,
            ["show_progress"] = ShowProgress.ToString().ToLowerInvariant(),
            ["every"] = this.Every,
            ["batch"] = this.Batch,
            ["max_batch_size"] = MaxBatchSize,
            ["cancels"] = this.Cancels,
            ["types"] = new Dictionary<string, object> {
            {"continuous",this.TypesContinuous },
            {"generator",this.TypesGenerator }
        },
            ["collects_event_data"] = this.CollectsEventData,
            ["trigger_after"] = this.TriggerAfter,
            ["trigger_only_on_success"] = this.TriggerOnlyOnSuccess,
            ["trigger_mode"] = TriggerMode.ToString().ToSnakeCase(),
            ["show_api"] = this.ShowApi,
            ["zerogpu"] = this.ZeroGpu
        };

        return result;
    }

    private IEnumerable<object[]> GetTargetsConfig()
    {
        List<object[]> result = [];  
        foreach ((int? Id, string EventName) target in this.Targets)
        {
            result.Add([target.Id,target.EventName]);
        }
        return result;
    }
}

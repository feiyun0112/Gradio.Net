using Gradio.Net.Enums;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FnTarget = (int? Id, string EventName);

namespace Gradio.Net
{
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
        internal float? Every { get;  set; }
        internal bool TracksProgress { get;  set; }
        internal string ConcurrencyId { get;  set; }
        internal ConcurrencyLimit? ConcurrencyLimit { get;  set; }
        internal bool Batch { get;  set; }
        internal bool InputsAsDict { get;  set; }
        internal bool Postprocess { get;  set; }
        internal Func<Input,Task<Output>>? Fn { get;  set; }
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
            var result = new Dictionary<string, object>();
            result["targets"] = GetTargetsConfig();
            result["inputs"] = Inputs.Select(p => p.Id).ToArray();
            result["outputs"] = Outputs.Select(p => p.Id).ToArray();
            result["backend_fn"] = this.Fn!=null;
            result["js"] = this.Js;
            result["queue"] = this.Queue;
            result["apiName"] = ApiName;
            result["scrollToOutput"] = this.ScrollToOutput;
            result["showProgress"] = ShowProgress.ToString().ToLowerInvariant();
            result["every"] = this.Every;
            result["batch"] = this.Batch;
            result["maxBatchSize"] = MaxBatchSize;
            result["cancels"] = this.Cancels;
            result["types"] = new Dictionary<string, object> {
                {"continuous",this.TypesContinuous },
                {"generator",this.TypesGenerator }
            };
            result["collects_event_data"] = this.CollectsEventData;
            result["trigger_after"] = this.TriggerAfter;
            result["trigger_only_on_success"] = this.TriggerOnlyOnSuccess;
            result["triggerMode"] = TriggerMode.ToString().ToLowerInvariant();
            result["show_api"] = this.ShowApi;
            result["zerogpu"] = this.ZeroGpu;

            return result;
        }

        private IEnumerable<object[]> GetTargetsConfig()
        {
            var result = new List<object[]>();  
            foreach (var target in this.Targets)
            {
                result.Add(new object[] { target.Id,target.EventName});
            }
            return result;
        }
    }
}

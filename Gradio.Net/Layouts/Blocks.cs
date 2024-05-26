
using Gradio.Net.Enums;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;

using FnTarget = (int? Id, string EventName);

namespace Gradio.Net
{
    public class Blocks : Block, ICollection<Block>, IDisposable
    {
        private readonly List<Block> _children = new List<Block>();
        internal Blocks()
        {
        }
        [IgnoreDataMember]
        internal Blocks ParentBlocks { get; set; }

        public int Count => _children.Count;

        public bool IsReadOnly => false;

        internal string Title { get; set; }
        internal string Theme { get;  set; }
        internal bool AnalyticsEnabled { get;  set; }
        internal string Mode { get;  set; }
        internal string Css { get;  set; }
        internal string Js { get;  set; }
        internal string Head { get;  set; }
        internal bool FillHeight { get;  set; }
        internal Tuple<int, int> DeleteCache { get;  set; }

        public  virtual void Add(Block item)
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
            var config = new Dictionary<string, object>();
            config["mode"] = this.Mode;
            config["dev_mode"] = false;
            config["analyticsEnabled"] = this.AnalyticsEnabled;
            config["components"] = GetComponents();
            config["css"] = this.Css;
            config["connect_heartbeat"] = false;
            config["js"] = this.Js;
            config["head"] = this.Head;
            config["title"] = this.Title;
            config["space_id"] = null;
            config["enable_queue"] = true;
            config["show_error"] = false;
            config["showApi"] = true;
            config["is_colab"] = false;
            config["max_file_size"] = null;
            config["stylesheets"] = new string[] {
                "https://fonts.googleapis.com/css2?family=Source+Sans+Pro:wght@400;600&display=swap",
                "https://fonts.googleapis.com/css2?family=IBM+Plex+Mono:wght@400;600&display=swap"
            };
            config["theme"] = this.Theme;
            config["protocol"] = "sse_v3";
            config["body_css"] = new Dictionary<string, object> {
                { "body_background_fill", "white" },
                { "body_text_color", "#1f2937" },
                { "body_background_fill_dark", "#0b0f19" },
                { "body_text_color_dark", "#f3f4f6" },
            };
            config["fillHeight"] = this.FillHeight;

            config["layout"] = GetLayout();

            

            config["dependencies"] = GetDependencies();

            return config;
        }

        internal List<Dictionary<string, object>> GetComponents()
        {
            var result = new List<Dictionary<string, object>>();
            foreach (var item in _children)
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
            var result = new Dictionary<string, object>();
            result["id"] = Id;
            var childrenLayout = new List<Dictionary<string, object>>();
            foreach (var item in _children)
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
            Func<Input,Task<Output>> fn,
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
            float? every = null,
            bool? collects_event_data = null,
            int? trigger_after = null,
            bool trigger_only_on_success = false,
            TriggerMode? triggerMode = TriggerMode.Once,
            ConcurrencyLimit? concurrencyLimit = null,
            string concurrencyId = null,
            bool showApi = true)
        {
            // Support for singular parameter
            List<FnTarget> tmpTargets = targets.Select(target => (!no_target && target.Block != null) ? new FnTarget(target.Block.Id, target.EventName) : new FnTarget(null,target.EventName)).ToList();

            inputs = inputs == null ? new List<Component>() : inputs.OrderBy(x => x.Id).ToList();
            outputs = outputs == null ? new List<Component>() : outputs.OrderBy(x => x.Id).ToList();

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

            if (apiName != null && apiName.Trim() == "") {
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

            var blockFn = new BlockFunction()
        {
                Fn= fn,
                Inputs=inputs,
                Outputs =outputs,
                Preprocess=preprocess,
                Postprocess=postprocess,
                InputsAsDict = false,
                Targets = tmpTargets,
                Batch = batch,
                MaxBatchSize = maxBatchSize,
                ConcurrencyLimit = concurrencyLimit,
                ConcurrencyId = concurrencyId,
                TracksProgress = progressIndex!=null,
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
            var result = new List<Dictionary<string, object>>();
            foreach (var func in Fns)
            {
                result.Add(func.GetConfig());
            }

            return result;
        }

        [IgnoreDataMember]
        internal List<BlockFunction> Fns { get; set; } = new List<BlockFunction>();

        private (IList, int? progressIndex, int? eventDataIndex) SpecialArgs(Func<Input,Task<Output>> fn)
        {
            //todo:Checks if function has special arguments Request or EventData (via annotation) or Progress (via default value).
            return (null, null, null);
        }
    }
}

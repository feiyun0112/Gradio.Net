using Gradio.Net.Enums;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using static System.Formats.Asn1.AsnWriter;

namespace Gradio.Net
{
    public static partial class gr
    {
        public static Button Button(
            string value = "Run",
            decimal? every = null,
            ButtonVariant variant = ButtonVariant.Secondary,
            ButtonSize? size = null,
           string icon = null,
            string link = null,
            bool visible = true,
            bool interactive = true,
            string elemId = null,
           IEnumerable<string> elemClasses = null,
           bool render = true,
            int? scale = null,
           int? minWidth = null)
        {
            var block = new Button()
            {
                Value = value,
                Every = every,
                Variant = variant,
                Size = size,
                Icon = icon,
                Link = link,
                Visible = visible,
                Interactive = interactive,
                ElemId = elemId,
                ElemClasses = elemClasses,
                Render = render,
                Scale = scale,
                MinWidth = minWidth
            };
            Context.AddToCurrentBlocks(block);
            return block;
        }


        public static async Task Click(this Button button,
           Func<Input,Task<Output>> fn = null,
           Func<Input,IAsyncEnumerable<Output>> streamingFn = null,
           IEnumerable<Component> inputs = null,
           IEnumerable<Component> outputs = null,
           string apiName = null,
           bool scrollToOutput = false,
           ShowProgress showProgress = ShowProgress.Full,
           bool? queue = null,
           bool batch = false,
           int maxBatchSize = 4,
           bool preprocess = true,
           bool postprocess = true,
           Dictionary<string, object> cancels = null,
           decimal? every = null,
           TriggerMode? triggerMode = null,
           string js = null,
           ConcurrencyLimit concurrencyLimit = ConcurrencyLimit.Default,
           string concurrencyId = null,
           bool showApi = true)
        {
            await Events.Click.EventTrigger(button,
                fn: fn,
                streamingFn: streamingFn,
                inputs: inputs,
                outputs: outputs,
                apiName: apiName,
                scrollToOutput: scrollToOutput,
                showProgress: showProgress,
                queue: queue,
                batch: batch,
                maxBatchSize: maxBatchSize,
                preprocess: preprocess,
                postprocess: postprocess,
                cancels: cancels,
                every: every,
                triggerMode: triggerMode,
                js: js,
                concurrencyLimit: concurrencyLimit,
                concurrencyId: concurrencyId,
                showApi: showApi
                );
        }
    }
}

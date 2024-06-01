using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Gradio.Net
{
    internal class EventWorker : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var eventIn = await Context.EventChannel.Reader.ReadAsync(stoppingToken);
                if (eventIn == null)
                {
                    await Task.Delay(500, stoppingToken);
                    continue;
                }

                if (eventIn.FnIndex >= 0 && eventIn.FnIndex < Context.RootBlock.Fns.Count)
                {
                    var blockFunction = Context.RootBlock.Fns[eventIn.FnIndex];
                    var fn = blockFunction.Fn;
                    var streamingFn = blockFunction.StreamingFn;
                    var data = eventIn.Data.Data;

                    _ = Task.Factory.StartNew(()=>
                    {
                        try
                        {
                            var input = gr.Input(blockFunction, data);
                            Context.EventResults.TryAdd(eventIn.SessionHash, new EventResult { Event = eventIn, BlockFunction = blockFunction, Input = input, OutputTask = fn?.Invoke(input), StreamingOutputTask = streamingFn?.Invoke(input) });
                        }
                        catch (Exception ex)
                        {
                            Context.EventResults.TryAdd(eventIn.SessionHash, new EventResult { Event = eventIn, BlockFunction = blockFunction, OutputTask = Task.FromResult<Output>(new ErrorOutput(ex)) }); ;
                        }
                    }, default, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default); 
                }
                else
                {
                    Context.EventResults.TryAdd(eventIn.SessionHash, null);
                }
            }
        }
    }
}

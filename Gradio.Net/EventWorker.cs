using Microsoft.Extensions.Hosting;

namespace Gradio.Net;

internal class EventWorker : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Event eventIn = await Context.EventChannel.Reader.ReadAsync(stoppingToken);
            if (eventIn == null)
            {
                await Task.Delay(500, stoppingToken);
                continue;
            }

            if (eventIn.FnIndex >= 0 && eventIn.FnIndex < Context.RootBlock.Fns.Count)
            {
                BlockFunction blockFunction = Context.RootBlock.Fns[eventIn.FnIndex];
                Func<Input, Task<Output>>? fn = blockFunction.Fn;
                Func<Input, IAsyncEnumerable<Output>>? streamingFn = blockFunction.StreamingFn;
                object[] data = eventIn.Data.Data;

                _ = Task.Factory.StartNew(()=>
                {
                    try
                    {
                        Input input = gr.Input(blockFunction, data);
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

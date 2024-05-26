using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                    var fn = Context.RootBlock.Fns[eventIn.FnIndex].Fn;
                    Context.EventResults.TryAdd(eventIn.SessionHash,new EventResult { Event = eventIn, OutputTask = fn?.Invoke(gr.Input(eventIn.Data.Data)) });
                }
                else
                {
                    Context.EventResults.TryAdd(eventIn.SessionHash, null);
                }
            }
        }
    }
}

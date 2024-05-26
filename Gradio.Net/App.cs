
using Gradio.Net;
using Gradio.Net.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gradio.Net
{
    public class App
    {
        const string GRADIO_VERSION = "4.29.0";
        private readonly long _appId;

        public static void Launch(Blocks blocks, Action<GradioServiceConfig>? additionalConfigurationAction = null, params string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddGradio();

            var app = builder.Build();

            app.UseGradio(blocks, additionalConfigurationAction);

            app.Run();
        }

        private GradioServiceConfig _gradioServiceConfig;

        internal App()
        {
            this._appId = new Random(DateTime.Now.Millisecond).NextInt64();
        }

        internal Dictionary<string, object> GetConfig(Microsoft.AspNetCore.Http.HttpRequest request)
        {
            var result = Context.RootBlock.GetConfig();

            result["root"] = $"{request.Scheme}://{request.Host}";
            result["version"] = GRADIO_VERSION;
            result["app_id"] = _appId;
            return result;
        }

        internal void SetConfig(GradioServiceConfig gradioServiceConfig)
        {
            this._gradioServiceConfig = gradioServiceConfig;
        }


        internal object GetApiInfo(HttpRequest request)
        {
            var result = new Dictionary<string, object>();

            result["named_endpoints"] = new Dictionary<string, object>();
            result["unnamed_endpoints"] = new Dictionary<string, object>();
            return result;
        }

        internal async Task<QueueJoinOut> QueueJoin(PredictBodyIn body)
        {
            var eventIn = new Event()
            {
                SessionHash = body.SessionHash,
                FnIndex = body.FnIndex,
                ConcurrencyId = Context.RootBlock.Fns[body.FnIndex].ConcurrencyId,
            };
            eventIn.Data = body;

            await Context.EventChannel.Writer.WriteAsync(eventIn);

            return new QueueJoinOut { EventId = eventIn.Id };
        }

        internal async IAsyncEnumerable<SSEMessage> QueueData(string sessionHash, CancellationToken stoppingToken)
        {
            if (string.IsNullOrEmpty(sessionHash))
            {
                yield return new UnexpectedErrorMessage("Session not found");

                yield break;
            }

            const int heartbeatRate = 15;
            const int checkRate = 500;
            EventResult eventResult = null;
            int heartbeatCount = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(checkRate);
                if (Context.EventResults.TryGetValue(sessionHash, out eventResult))
                {
                    Context.EventResults.Remove(sessionHash, out _);
                    break;
                }

                if (heartbeatCount++ > heartbeatRate)
                {
                    break;
                }

                yield return new HeartbeatMessage();
            }

            if (eventResult == null || eventResult.OutputTask==null)
            {
                yield return new UnexpectedErrorMessage("no task for Session");

                yield break;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                while (!eventResult.OutputTask.IsCompleted)
                {
                    yield return new HeartbeatMessage();
                    await Task.Delay(checkRate);
                }

                break;
            }

            if (eventResult.OutputTask.Exception != null)
            {
                yield return new UnexpectedErrorMessage(eventResult.OutputTask.Exception.Message);

                yield break;
            }

            if (stoppingToken.IsCancellationRequested || !eventResult.OutputTask.IsCompletedSuccessfully)
            {
                yield return new UnexpectedErrorMessage("Task Cancelled");

                yield break;
            }
            var result = eventResult.OutputTask.Result;
            yield return new ProcessCompletedMessage(eventResult.Event.Id, new Dictionary<string, object> {
                { "data",result.Data}
            });

            yield break;

        }
    }
}

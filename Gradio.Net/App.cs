
using Gradio.Net;
using Gradio.Net.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
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

        internal Dictionary<string, object> GetConfig(HttpRequest httpRequest)
        {
            var result = Context.RootBlock.GetConfig();

            result["root"] = httpRequest.GetRootUrl();
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

        internal async Task<QueueJoinOut> QueueJoin(HttpRequest request, PredictBodyIn body)
        {
            var eventIn = new Event()
            {
                RootUrl = request.GetRootUrl(),
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

            if (eventResult == null || (eventResult.OutputTask==null && eventResult.StreamingOutputTask == null))
            {
                yield return new UnexpectedErrorMessage("no task for Session");

                yield break;
            }

            if (eventResult.OutputTask != null)
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    while (!eventResult.OutputTask.IsCompleted)
                    {
                        if (eventResult.Input.Progress != null)
                        {
                            yield return new ProgressMessage(eventResult.Event.Id, eventResult.Input.Progress);
                        }
                        else
                        {
                            yield return new HeartbeatMessage();
                        }
                        await Task.Delay(checkRate);
                    }

                    break;
                }

                if (eventResult.OutputTask.Exception != null)
                {
                    Exception ex = eventResult.OutputTask.Exception;
                    while (ex.InnerException != null)
                    {
                        ex = ex.InnerException;
                    }

                    yield return new UnexpectedErrorMessage(ex.Message);

                    yield break;
                }

                if (stoppingToken.IsCancellationRequested || !eventResult.OutputTask.IsCompletedSuccessfully)
                {
                    yield return new UnexpectedErrorMessage("Task Cancelled");

                    yield break;
                }
                var result = eventResult.OutputTask.Result;

                if (result is ErrorOutput error)
                {
                    yield return new UnexpectedErrorMessage(error.Exception.Message);

                    yield break;
                }

                var data = result.Data;

                yield return new ProcessCompletedMessage(eventResult.Event.Id, new Dictionary<string, object> {
                    { "data",gr.Output(eventResult,data)}
                });
                yield break;
            }
            else if (eventResult.StreamingOutputTask != null)
            {
                yield return new ProcessStartsMessage(eventResult.Event.Id);

                object[] result=null;
                await foreach (var output in  eventResult.StreamingOutputTask)
                {
                    if (stoppingToken.IsCancellationRequested)
                    {
                        yield return new UnexpectedErrorMessage("Task Cancelled");

                        yield break;
                    }
                    
                    var outputData = output.Data;
                    if (result == null)
                    {
                        result = gr.Output(eventResult, outputData);
                        yield return new ProcessGeneratingMessage(eventResult.Event.Id, new Dictionary<string, object> {
                            { "data",result}
                        });
                    }
                    else
                    {
                        var oldResult = result;
                        result = gr.Output(eventResult, outputData);
                        var diffResult = new object[result.Length];
                        for (var i = 0; i < result.Length; i++)
                        {
                            if (oldResult[i] != null && result[i] != null  && oldResult[i] is string oldStr  && result[i] is string str && oldStr==str)
                            {
                                diffResult[i] = new object[0];
                            }
                            else
                            {
                                diffResult[i] = new object[] { new List<object> { "replace", new object[0], result[i] } };
                            }
                            
                        }
                        yield return new ProcessGeneratingMessage(eventResult.Event.Id, new Dictionary<string, object> {
                            { "data", diffResult}
                        });
                    }
                }
                if (result == null)
                {
                    yield return new UnexpectedErrorMessage("Task Cancelled");

                    yield break;
                }


                yield return new ProcessCompletedMessage(eventResult.Event.Id, new Dictionary<string, object> {
                    { "data",result}
                });
                yield break;
            }
            else
            {
                yield return new UnexpectedErrorMessage("no task for Session");

                yield break;
            }
        }

        internal async Task<List<string>> Upload(string uploadId, IFormFileCollection files)
        {
            if (files == null || files.Count == 0)
            {
                throw new ArgumentException("No files uploaded.");
            }

            var outputFiles = new List<string>();
            var filesToCopy = new List<string>();
            var locations = new List<string>();

            foreach (var file in files)
            {
                if (file.Length == 0)
                {
                    continue;
                }

                var fileName = Path.GetFileName(file.FileName);
                var name = Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), ""));
                var directory = Path.Combine(Path.GetTempPath(), file.GetHashCode().ToString());
                Directory.CreateDirectory(directory);
                var dest = Path.Combine(directory, name);

                using (var stream = new FileStream(dest, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                Context.DownloadableFiles.TryAdd(dest,dest);

                outputFiles.Add(dest);
            }

            return outputFiles;
        }

        internal async Task<(string filePath, string contentType)> GetUploadedFile(string pathOrUrl)
        {
            var filePath = new FileInfo(System.Net.WebUtility.UrlDecode(pathOrUrl)).FullName;
            if (!Context.DownloadableFiles.TryGetValue(filePath, out _))
            {
                throw new ArgumentException($"File not allowed: {pathOrUrl}.");
            }

            Context.DownloadableFiles.Remove(filePath, out _);
                        
            


            return (filePath, ClientUtils.GetMimeType(filePath));
        }
    }
}

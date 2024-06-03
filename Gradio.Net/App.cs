using Gradio.Net.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Gradio.Net;

public class App
{
    const string GRADIO_VERSION = "4.29.0";
    private readonly long _appId;

    public static void Launch(Blocks blocks, Action<GradioServiceConfig>? additionalConfigurationAction = null, params string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.Services.AddGradio();

        WebApplication app = builder.Build();

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
        Dictionary<string, object> result = Context.RootBlock.GetConfig();

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
        Dictionary<string, object> result = new()
        {
            ["named_endpoints"] = new Dictionary<string, object>(),
            ["unnamed_endpoints"] = new Dictionary<string, object>()
        };
        return result;
    }

    internal async Task<QueueJoinOut> QueueJoin(HttpRequest request, PredictBodyIn body)
    {
        Event eventIn = new()
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
            Output result = eventResult.OutputTask.Result;

            if (result is ErrorOutput error)
            {
                yield return new UnexpectedErrorMessage(error.Exception.Message);

                yield break;
            }

            object[] data = result.Data;

            yield return new ProcessCompletedMessage(eventResult.Event.Id, new Dictionary<string, object> {
                { "data",gr.Output(eventResult,data)}
            });
            yield break;
        }
        else if (eventResult.StreamingOutputTask != null)
        {
            yield return new ProcessStartsMessage(eventResult.Event.Id);

            object[] result=null;
            await foreach (Output output in  eventResult.StreamingOutputTask)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    yield return new UnexpectedErrorMessage("Task Cancelled");

                    yield break;
                }

                object[] outputData = output.Data;
                if (result == null)
                {
                    result = gr.Output(eventResult, outputData);
                    yield return new ProcessGeneratingMessage(eventResult.Event.Id, new Dictionary<string, object> {
                        { "data",result}
                    });
                }
                else
                {
                    object[] oldResult = result;
                    result = gr.Output(eventResult, outputData);
                    object[] diffResult = new object[result.Length];
                    for (int i = 0; i < result.Length; i++)
                    {
                        if (oldResult[i] != null && result[i] != null  && oldResult[i] is string oldStr  && result[i] is string str && oldStr==str)
                        {
                            diffResult[i] = Array.Empty<object>();
                        }
                        else
                        {
                            diffResult[i] = new object[] { new List<object> { "replace", Array.Empty<object>(), result[i] } };
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

        List<string> outputFiles = [];
        List<string> filesToCopy = [];
        List<string> locations = [];

        foreach (IFormFile file in files)
        {
            if (file.Length == 0)
            {
                continue;
            }

            string fileName = Path.GetFileName(file.FileName);
            string name = Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), ""));
            string directory = Path.Combine(Path.GetTempPath(), file.GetHashCode().ToString());
            Directory.CreateDirectory(directory);
            string dest = Path.Combine(directory, name);

            using (FileStream stream = new(dest, FileMode.Create))
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
        string filePath = new FileInfo(System.Net.WebUtility.UrlDecode(pathOrUrl)).FullName;
        if (!Context.DownloadableFiles.TryGetValue(filePath, out _))
        {
            throw new ArgumentException($"File not allowed: {pathOrUrl}.");
        }

        Context.DownloadableFiles.Remove(filePath, out _);
                    
        


        return (filePath, ClientUtils.GetMimeType(filePath));
    }
}

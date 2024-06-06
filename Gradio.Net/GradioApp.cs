using Gradio.Net.Models;
using Microsoft.Extensions.FileProviders.Embedded;
using Microsoft.Extensions.FileProviders;
using System.Reflection;
using System.Text;
using System.Collections.Concurrent;

namespace Gradio.Net;

public class GradioApp
{
    const string GRADIO_VERSION = "4.32.2";
    private readonly long _appId;

    private GradioServiceConfig _gradioServiceConfig;

    public GradioApp()
    {
        _appId = new Random(DateTime.Now.Millisecond).NextInt64();
    }

    public Dictionary<string, object> GetConfig(string rootUrl)
    {
        Dictionary<string, object> result = Context.RootBlock.GetConfig();

        result["stylesheets"] = this._gradioServiceConfig.Stylesheets;
        result["body_css"] = this._gradioServiceConfig.BodyCss;
        result["root"] = rootUrl;
        result["version"] = GRADIO_VERSION;
        result["app_id"] = _appId;
        return result;
    }

    public void SetConfig(GradioServiceConfig gradioServiceConfig)
    {
        this._gradioServiceConfig = gradioServiceConfig;
    }

    public object GetApiInfo()
    {
        Dictionary<string, object> result = new()
        {
            ["named_endpoints"] = new Dictionary<string, object>(),
            ["unnamed_endpoints"] = new Dictionary<string, object>()
        };
        return result;
    }

    public async Task<QueueJoinOut> QueueJoin(string rootUrl, PredictBodyIn body)
    {
        Event eventIn = new()
        {
            RootUrl = rootUrl,
            SessionHash = body.SessionHash,
            FnIndex = body.FnIndex,
            ConcurrencyId = Context.RootBlock.Fns[body.FnIndex].ConcurrencyId,
        };
        eventIn.Data = body;

        lock (Context.PendingMessageLock)
        {
            if (!Context.PendingEventIdsSession.TryGetValue(eventIn.SessionHash, out List<string> pendingEventIds))
            {
                pendingEventIds = new List<string>();
            }
            pendingEventIds.Add(eventIn.ToString());
            Context.PendingEventIdsSession[eventIn.SessionHash] = pendingEventIds;
        }

        await Context.EventChannel.Writer.WriteAsync(eventIn);

        return new QueueJoinOut { EventId = eventIn.Id };
    }

    public async IAsyncEnumerable<SSEMessage> QueueData(string sessionHash, CancellationToken stoppingToken)
    {
        const int heartbeatRate = 150;
        const int checkRate = 50;
        int heartbeatCount = 0;

        if (string.IsNullOrEmpty(sessionHash))
        {
            yield return new UnexpectedErrorMessage(null, "Session not found");

            yield break;
        }

        while (!Context.PendingEventIdsSession.TryGetValue(sessionHash, out _))
        {
            if (heartbeatCount++ > heartbeatRate)
            {
                yield return new UnexpectedErrorMessage(null, "Session not found");

                yield break;
            }

            await Task.Delay(heartbeatRate);
        }

        heartbeatCount = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            string[] pendingEventIds = null;
            lock (Context.PendingMessageLock)
            {
                if (!Context.PendingEventIdsSession.TryGetValue(sessionHash, out List<string> tmpIds))
                {
                    yield break;
                }

                pendingEventIds = tmpIds.ToArray();
            }

            if (pendingEventIds == null || pendingEventIds.Length == 0)
            {
                yield break;
            }

            foreach (var pendingEventId in pendingEventIds)
            {
                if (!Context.EventResults.TryGetValue(pendingEventId, out EventResult eventResult))
                {
                    if (heartbeatCount++ > heartbeatRate)
                    {
                        yield return new UnexpectedErrorMessage(pendingEventId, "no task for Session");

                        RemovePendingEvent(sessionHash, pendingEventId);
                        continue;
                    }
                    yield return new HeartbeatMessage();
                    await Task.Delay(heartbeatRate);
                    continue;
                }

                if (eventResult == null || (eventResult.OutputTask == null && eventResult.StreamingOutputTask == null))
                {
                    yield return new UnexpectedErrorMessage(pendingEventId, "no task for Session");

                    RemovePendingEvent(sessionHash, pendingEventId);
                    continue;
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

                        yield return new UnexpectedErrorMessage(pendingEventId, ex.Message);

                        RemovePendingEvent(sessionHash, pendingEventId);
                        continue;
                    }

                    if (stoppingToken.IsCancellationRequested || !eventResult.OutputTask.IsCompletedSuccessfully)
                    {
                        yield return new UnexpectedErrorMessage(pendingEventId, "Task Cancelled");

                        RemovePendingEvent(sessionHash, pendingEventId);
                        continue;
                    }
                    Output result = eventResult.OutputTask.Result;

                    if (result is ErrorOutput error)
                    {
                        yield return new UnexpectedErrorMessage(pendingEventId, error.Exception.Message);

                        RemovePendingEvent(sessionHash, pendingEventId);
                        continue;
                    }

                    object[] data = result.Data;

                    yield return new ProcessCompletedMessage(eventResult.Event.Id, new Dictionary<string, object> {
                        { "data",gr.Output(eventResult,data)}
                    });
                    RemovePendingEvent(sessionHash, pendingEventId);
                    continue;
                }
                else if (eventResult.StreamingOutputTask != null)
                {
                    yield return new ProcessStartsMessage(eventResult.Event.Id);

                    object[] result = null;
                    await foreach (Output output in eventResult.StreamingOutputTask)
                    {
                        if (stoppingToken.IsCancellationRequested)
                        {
                            yield return new UnexpectedErrorMessage(pendingEventId, "Task Cancelled");

                            RemovePendingEvent(sessionHash, pendingEventId);
                            continue;
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
                                if (oldResult[i] != null && result[i] != null && oldResult[i] is string oldStr && result[i] is string str && oldStr == str)
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
                        yield return new UnexpectedErrorMessage(pendingEventId, "Task Cancelled");

                        RemovePendingEvent(sessionHash, pendingEventId);
                        continue;
                    }


                    yield return new ProcessCompletedMessage(eventResult.Event.Id, new Dictionary<string, object> {
                        { "data",result}
                    });
                    RemovePendingEvent(sessionHash, pendingEventId);
                    continue;
                }
                else
                {
                    yield return new UnexpectedErrorMessage(pendingEventId, "no task for Session");

                    RemovePendingEvent(sessionHash, pendingEventId);
                    continue;
                }
            }
        }
    }

    public List<string>? ClonseSession(string sessionHash)
    {
        Context.PendingEventIdsSession.TryRemove(sessionHash, out List<string>? tmpIds);
        return tmpIds;
    }

    private void RemovePendingEvent(string sessionHash, string pendingEventId)
    {
        lock (Context.PendingMessageLock)
        {
            if (!Context.PendingEventIdsSession.TryGetValue(sessionHash, out List<string>? tmpIds))
            {
                return;
            }

            tmpIds.Remove(pendingEventId);
        }
    }

    public async Task<List<string>> Upload(string uploadId, List<(Stream Stream, string Name)> files, bool autoCloseFileStream = true)
    {
        if (files == null || files.Count == 0)
        {
            throw new ArgumentException("No files uploaded.");
        }

        List<string> outputFiles = [];
        List<string> filesToCopy = [];
        List<string> locations = [];

        foreach ((Stream Stream, string Name) file in files)
        {
            if (file.Stream.Length == 0)
            {
                continue;
            }

            string fileName = Path.GetFileName(file.Name);
            string name = Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), ""));
            string directory = Path.Combine(Path.GetTempPath(), file.GetHashCode().ToString());
            Directory.CreateDirectory(directory);
            string dest = Path.Combine(directory, name);

            using (FileStream stream = new(dest, FileMode.Create))
            {
                await file.Stream.CopyToAsync(stream);
            }

            Context.DownloadableFiles.TryAdd(dest, dest);

            outputFiles.Add(dest);

            if (autoCloseFileStream)
                file.Stream.Close();
        }

        return outputFiles;
    }

    public async Task<(string filePath, string contentType)> GetUploadedFile(string pathOrUrl)
    {
        string filePath = new FileInfo(System.Net.WebUtility.UrlDecode(pathOrUrl)).FullName;
        if (!Context.DownloadableFiles.TryGetValue(filePath, out _))
        {
            throw new ArgumentException($"File not allowed: {pathOrUrl}.");
        }

        //Context.DownloadableFiles.Remove(filePath, out _);




        return (filePath, ClientUtils.GetMimeType(filePath));
    }

    public virtual IFileInfo GetFileInfo(string subpath)
    {
        Assembly assembl = Assembly.GetExecutingAssembly();
        string baseNamespace = typeof(GradioApp).Namespace ?? "";
        subpath = $@"templates/frontend/{subpath.TrimStart([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar])}";

        if (Path.IsPathRooted(subpath))
        {
            return new NotFoundFileInfo(subpath);
        }

        if (string.IsNullOrEmpty(subpath))
        {
            return new NotFoundFileInfo(subpath);
        }

        StringBuilder builder = new(baseNamespace.Length + subpath.Length);
        builder.Append(baseNamespace);

        if (subpath.StartsWith("/", StringComparison.Ordinal))
        {
            subpath = subpath.Substring(1, subpath.Length - 1);
        }

        builder.Append("." + subpath.Replace("/", "."));

        string resourcePath = builder.ToString();

        string name = Path.GetFileName(subpath);
        if (assembl.GetManifestResourceInfo(resourcePath) == null)
        {
            return new NotFoundFileInfo(name);
        }

        return new EmbeddedResourceFileInfo(assembl, resourcePath, name, DateTimeOffset.UtcNow); ;
    }

    public virtual async Task StartEventLoopAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Event eventIn = await Context.EventChannel.Reader.ReadAsync(stoppingToken);
            if (eventIn == null)
            {
                await Task.Delay(50, stoppingToken);
                continue;
            }

            if (eventIn.FnIndex >= 0 && eventIn.FnIndex < Context.RootBlock.Fns.Count)
            {
                BlockFunction blockFunction = Context.RootBlock.Fns[eventIn.FnIndex];
                Func<Input, Task<Output>>? fn = blockFunction.Fn;
                Func<Input, IAsyncEnumerable<Output>>? streamingFn = blockFunction.StreamingFn;
                object[] data = eventIn.Data.Data;

                _ = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        Input input = gr.Input(blockFunction, data);
                        Context.EventResults.TryAdd(eventIn.ToString(), new EventResult { Event = eventIn, BlockFunction = blockFunction, Input = input, OutputTask = fn?.Invoke(input), StreamingOutputTask = streamingFn?.Invoke(input) });
                    }
                    catch (Exception ex)
                    {
                        Context.EventResults.TryAdd(eventIn.ToString(), new EventResult { Event = eventIn, BlockFunction = blockFunction, OutputTask = Task.FromResult<Output>(new ErrorOutput(ex)) }); ;
                    }
                }, default, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
            }
            else
            {
                Context.EventResults.TryAdd(eventIn.ToString(), null);
            }
        }
    }
}

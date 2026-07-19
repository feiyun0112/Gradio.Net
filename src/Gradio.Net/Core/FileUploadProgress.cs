using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace Gradio.Net.Core;

public class FileUploadProgress
{
    private readonly Dictionary<string, FileUploadProgressTracker> _statuses;
    private readonly Dictionary<string, TaskCompletionSource<bool>> _signals;

    public FileUploadProgress()
    {
        _statuses = new Dictionary<string, FileUploadProgressTracker>();
        _signals = new Dictionary<string, TaskCompletionSource<bool>>();
    }

    public void Track(string uploadId)
    {
        if (!_statuses.ContainsKey(uploadId))
        {
            _statuses[uploadId] = new FileUploadProgressTracker();
            if (_signals.TryGetValue(uploadId, out var tcs))
            {
                tcs.TrySetResult(true);
            }
        }
    }

    public async Task<bool> IsTracked(string uploadId)
    {
        if (_statuses.ContainsKey(uploadId))
        {
            return true;
        }

        if (!_signals.TryGetValue(uploadId, out var tcs))
        {
            tcs = new TaskCompletionSource<bool>();
            _signals[uploadId] = tcs;
        }

        return await tcs.Task;
    }

    public void Append(string uploadId, string filename, byte[] messageBytes)
    {
        if (!_statuses.ContainsKey(uploadId))
        {
            Track(uploadId);
        }

        var tracker = _statuses[uploadId];
        var queue = tracker.Queue;

        if (queue.Count == 0)
        {
            queue.Enqueue(new FileUploadProgressUnit(filename, messageBytes.Length));
        }
        else
        {
            var lastUnit = queue.Dequeue();
            if (lastUnit.Filename != filename)
            {
                queue.Enqueue(new FileUploadProgressUnit(filename, messageBytes.Length));
            }
            else
            {
                queue.Enqueue(new FileUploadProgressUnit(filename, lastUnit.ChunkSize + messageBytes.Length));
            }
        }
    }

    public void SetDone(string uploadId)
    {
        if (!_statuses.ContainsKey(uploadId))
        {
            Track(uploadId);
        }
        _statuses[uploadId].IsDone = true;
    }

    public bool IsDone(string uploadId)
    {
        if (!_statuses.ContainsKey(uploadId))
        {
            throw new FileUploadProgressNotTrackedError();
        }
        return _statuses[uploadId].IsDone;
    }

    public void StopTracking(string uploadId)
    {
        _statuses.Remove(uploadId);
        _signals.Remove(uploadId);
    }

    public FileUploadProgressUnit Pop(string uploadId)
    {
        if (!_statuses.ContainsKey(uploadId))
        {
            throw new FileUploadProgressNotTrackedError();
        }

        var queue = _statuses[uploadId].Queue;
        if (queue.Count == 0)
        {
            throw new FileUploadProgressNotQueuedError();
        }

        return queue.Dequeue();
    }
}

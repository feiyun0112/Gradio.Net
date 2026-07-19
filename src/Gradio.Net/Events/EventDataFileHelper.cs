namespace Gradio.Net.Events;

internal static class EventDataFileHelper
{
    internal static Data.FileData? ToFileData(Dictionary<string, object> data)
    {
        // Python passes FileDataDict directly as the event payload.
        // Accept either {path,url,...} or nested {file:{...}} shapes (best-effort).
        if (data == null)
        {
            return null;
        }

        if (data.TryGetValue("file", out var nested) && nested is Dictionary<string, object> nestedDict)
        {
            data = nestedDict;
        }

        var fd = new Data.FileData
        {
            Path = data.TryGetValue("path", out var pathObj) ? pathObj?.ToString() : null,
            Url = data.TryGetValue("url", out var urlObj) ? urlObj?.ToString() : null,
            OrigName = data.TryGetValue("orig_name", out var origObj) ? origObj?.ToString() : null,
            MimeType = data.TryGetValue("mime_type", out var mimeObj) ? mimeObj?.ToString() : null
        };

        if (data.TryGetValue("size", out var sizeObj) && sizeObj != null)
        {
            try { fd.Size = Convert.ToInt32(sizeObj); } catch { /* ignore */ }
        }
        if (data.TryGetValue("is_stream", out var isStreamObj) && isStreamObj != null)
        {
            try { fd.IsStream = Convert.ToBoolean(isStreamObj); } catch { /* ignore */ }
        }

        // If the payload doesn't look like a FileData, return null
        return fd.IsNone ? null : fd;
    }
}

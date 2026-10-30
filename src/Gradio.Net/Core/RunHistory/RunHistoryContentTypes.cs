namespace Gradio.Net;
/// <summary>
/// What an asset's name says about its bytes.
/// </summary>
/// <remarks>
/// A short table rather than a lookup over the platform's registry, because the answer is a security
/// decision as much as a convenience: the run history endpoint serves an asset back to a browser, and
/// a media type that is not on a known-safe list would be a file a browser might execute. Anything
/// not in the table is answered as bytes to be downloaded.
/// </remarks>
internal static class RunHistoryContentTypes
{
    /// <summary>The type of a name's file, or the one that can never be wrong.</summary>
    /// <param name="name">The asset's name.</param>
    /// <returns>The media type.</returns>
    internal static string Of(string name) =>
        Path.GetExtension(name).ToLowerInvariant() switch
        {
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".svg" => "image/svg+xml",
            ".mp4" => "video/mp4",
            ".webm" => "video/webm",
            ".mp3" => "audio/mpeg",
            ".wav" => "audio/wav",
            ".ogg" => "audio/ogg",
            ".json" => "application/json",
            ".csv" => "text/csv",
            ".txt" => "text/plain",
            ".pdf" => "application/pdf",
            _ => "application/octet-stream",
        };
}

namespace Gradio.Net;

internal static class ClientUtils
{
    private static readonly Dictionary<string, string> MimeTypes = new(StringComparer.InvariantCultureIgnoreCase)
    {
        {".jpg", "image/jpeg"},
        {".jpeg", "image/jpeg"},
        {".png", "image/png"},
        {".gif", "image/gif"},
        {".bmp", "image/bmp"},
        {".tiff", "image/tiff"},
        {".ico", "image/x-icon"},
        {".svg", "image/svg+xml"},
        {".webp", "image/webp"},
        {".mp3", "audio/mpeg"},
        {".wav", "audio/wav"},
        {".ogg", "audio/ogg"},
        {".mp4", "video/mp4"},
        {".avi", "video/x-msvideo"},
        {".mov", "video/quicktime"},
        {".wmv", "video/x-ms-wmv"},
        {".flv", "video/x-flv"},
        {".mkv", "video/x-matroska"},
        {".pdf", "application/pdf"},
        {".doc", "application/msword"},
        {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
        {".xls", "application/vnd.ms-excel"},
        {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
        {".ppt", "application/vnd.ms-powerpoint"},
        {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
        {".txt", "text/plain"},
        {".csv", "text/csv"},
        {".html", "text/html"},
        {".css", "text/css"},
        {".js", "application/javascript"},
        {".json", "application/json"},
        {".xml", "application/xml"},
        {".zip", "application/zip"},
        {".rar", "application/x-rar-compressed"},
        {".7z", "application/x-7z-compressed"},
        {".tar", "application/x-tar"},
        {".gz", "application/gzip"}
    };

    public static string GetMimeType(string filePath, string defaultReturn = "application/octet-stream")
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return defaultReturn;

        string extension = Path.GetExtension(filePath);

        if (string.IsNullOrWhiteSpace(extension))
        {
            return defaultReturn;
        }
        MimeTypes.TryGetValue(extension, out string? mimeType);

        return string.IsNullOrWhiteSpace(mimeType) ? defaultReturn : mimeType;
    }

    internal static bool IsUrl(string str)
    {
        return str.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) || str.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase);
    }
}

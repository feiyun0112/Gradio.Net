namespace Gradio.Net;

public class FileData
{
    public string Path { get; set; }
    public string Name { get; set; } // filename
    public string Data { get; set; } // base64 encoded data
    public long? Size { get; set; } // size in bytes
    public bool? IsFile { get; set; } // whether the data corresponds to a file or base64 encoded data
    public string OrigName { get; set; } // original filename
    public string MimeType { get; set; }
    public bool? IsStream { get; set; }
    public string Url { get; set; }
}

public class VideoData
{
    public FileData Video {  get; set;}
    public FileData Subtitles { get; set; }
}
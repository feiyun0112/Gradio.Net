using Gradio.Net.Enums;
namespace Gradio.Net;

public class Video : Component, IStreamingInput, IHaveChangeEvent, IHaveClearEvent, IHaveStartRecordingEvent, IHaveStopRecordingEvent, IHaveStopEvent, IHavePlayEvent, IHavePauseEvent, IHaveEndEvent, IHaveUploadEvent
{
    internal Video() { }
    internal string Format { get;  set; }
    internal bool MirrorWebcam { get;  set; }    
    internal int? Height { get;  set; }
    internal int? Width { get;  set; }
    internal IEnumerable<VideoSource> Sources { get;  set; }
    internal bool? ShowDownloadButton { get;  set; }
    internal bool? ShowShareButton { get;  set; }
    internal bool? IncludeAudio { get;  set; }
    internal bool Autoplay { get;  set; }
    internal int? MinLength { get;  set; }
    internal int? MaxLength { get;  set; }

    public void CheckStreamable()
    {
        //todo
    }

    internal override object PreProcess(object data)
    {
        if (data == null)
        {
            return null;
        }

        string? str = data.ToString();

        VideoData fileData = JsonUtils.Deserialize<VideoData>(str);
        return fileData.Video.Path;
    }

    internal override object PostProcess(string rootUrl, object data)
    {
        if (data == null)
        {
            return null;
        }
        string? str = data.ToString();

        Context.DownloadableFiles.TryAdd(str, str);
        if (ClientUtils.IsUrl(str))
        {
            return new VideoData { Video = new FileData { Path = null, Url = str } };
        }
        
        return new VideoData{Video=  new FileData { Path = str, Url = $"{rootUrl}/file={str}" } };
    }

    public static string Payload(object obj)
    {
        if (obj == null)
        {
            return null;
        }

        if (obj is string str)
        {
            return str;
        }

        throw new ArgumentException($"Payload Type expect string actual {obj.GetType()}");
    }
}

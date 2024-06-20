using Gradio.Net.Enums;
namespace Gradio.Net;

public class Image : Component, IStreamingInput, IHaveClearEvent, IHaveChangeEvent, IHaveStreamEvent, IHaveSelectEvent, IHaveUploadEvent
{
    internal Image() { }
    internal ImageFormat? Format { get; set; }
    internal bool? MirrorWebcam { get; set; }
    internal ImageType? Type { get; set; }
    internal int? Height { get; set; }
    internal int? Width { get; set; }
    internal ImageMode? ImageMode { get; set; }
    internal IEnumerable<ImageSource> Sources { get; set; }
    internal bool? Streaming { get; set; }
    internal bool? ShowDownloadButton { get; set; }
    internal bool? ShowShareButton { get; set; }

    static Dictionary<string, object> _defaultProps = new Dictionary<string, object>()
    { { nameof(ImageFormat), ImageFormat.Webp },
        { nameof(ImageMode),   Enums.ImageMode.RGB },
         { nameof(ImageType),    ImageType.Filepath},
           { nameof(ShowDownloadButton), true },
          { nameof(Container),true },
        {nameof(MinWidth),160 },

           { nameof(Visible), true },
               { nameof(Streaming), false },
        { nameof(Render), true },
        { nameof(MirrorWebcam), true },
         { nameof(ShowShareButton), GradioUtils.GetSpace() != null },
    };
    protected override object? GetDefaultProp(string name)
    {
        Dictionary<string, object> result = _defaultProps;
        if (name == nameof(Sources))
        {
            result[nameof(Sources)] = (this.GetPropertyValue<bool>(nameof(Streaming)) ? new[] { ImageSource.Webcam } : [ImageSource.Upload, ImageSource.Webcam, ImageSource.Clipboard]);
        }

        return result.ContainsKey(name) ? result[name] : null;
    }

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

        FileData fileData = JsonUtils.Deserialize<FileData>(str);
        return fileData.Path;
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
            return new FileData { Path = null, Url = str };
        }

        return new FileData { Path = str, Url = $"{rootUrl}/file={str}" };
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

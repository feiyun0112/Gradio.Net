using Gradio.Net.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Gradio.Net
{
    public class Image : Component, IStreamingInput
    {
        internal ImageFormat Format { get;  set; }
        internal bool MirrorWebcam { get;  set; }
        internal ImageType Type { get;  set; }
        internal int? Height { get;  set; }
        internal int? Width { get;  set; }
        internal ImageMode ImageMode { get;  set; }
        internal IEnumerable<ImageSource> Sources { get;  set; }
        internal bool Streaming { get;  set; }
        internal bool ShowDownloadButton { get;  set; }
        internal bool? ShowShareButton { get;  set; }

        public void CheckStreamable()
        {
            //todo
        }

        internal override object PreProcess(object data)
        {
            var str = data.ToString();

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };
            var fileData = JsonSerializer.Deserialize<FileData>(str, serializeOptions);
            return fileData.Path;
        }

        internal override object PostProcess(string rootUrl, object data)
        {
            var str = data.ToString();

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };

            Context.DownloadableFiles.TryAdd(str, str);

            return new FileData { Path = str, Url = $"{rootUrl}/file={str}" };
        }
    }
}

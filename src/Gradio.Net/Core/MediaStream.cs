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

public class MediaStream
{
    public List<Data.MediaStreamChunk> Segments { get; set; }
    public string CombinedFile { get; set; }
    public bool Ended { get; set; }
    public int SegmentIndex { get; set; }
    public string Playlist { get; set; }
    public double MaxDuration { get; set; }
    public string DesiredOutputFormat { get; set; }

    public MediaStream(string desiredOutputFormat = null)
    {
        Segments = new List<Data.MediaStreamChunk>();
        CombinedFile = null;
        Ended = false;
        SegmentIndex = 0;
        Playlist = "#EXTM3U\n#EXT-X-PLAYLIST-TYPE:EVENT\n#EXT-X-TARGETDURATION:10\n#EXT-X-VERSION:4\n#EXT-X-MEDIA-SEQUENCE:0\n";
        MaxDuration = 5;
        DesiredOutputFormat = desiredOutputFormat;
    }

    public async Task AddSegment(Data.MediaStreamChunk chunk)
    {
        if (chunk == null)
        {
            return;
        }

        var segmentId = Guid.NewGuid().ToString();
        Segments.Add(new Data.MediaStreamChunk
        {
            Id = segmentId,
            Path = chunk.Path,
            Url = chunk.Url,
            Duration = chunk.Duration,
            Mime = chunk.Mime,
            OrigName = chunk.OrigName
        });

        MaxDuration = Math.Max(MaxDuration, chunk.Duration) + 1;
    }

    public void EndStream()
    {
        Ended = true;
    }
}

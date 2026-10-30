using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The media family as a page and as a handler see it: the defaults a component carries when the
/// application says nothing, and what a value becomes in each direction.
/// </summary>
/// <remarks>
/// <para>
/// The parameters an application sets are compared with the Python version's field by field. What a
/// document cannot show is the defaults: they are on the manifest page only where the page sets
/// something, and a default that drifts is a component that behaves differently for every
/// application that never mentioned it.
/// </para>
/// <para>
/// The streaming assertions are here rather than in a browser because what they are about is the
/// contract, not the player: a chunk that carries its own duration is a segment the playlist can
/// place, and bytes that are not already in the format the playlist serves are refused with a
/// message that says why.
/// </para>
/// </remarks>
public sealed class MediaFamilyTests
{
    [Fact]
    public async Task The_media_family_carries_upstreams_defaults()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            using (blocks.BeginRender())
            {
                blocks.Add(new ImageEditor());
                blocks.Add(new ImageSlider());
                blocks.Add(new Video());
                blocks.Add(new Audio());
                blocks.Add(new AudioGallery());
                blocks.Add(new Gallery());
                blocks.Add(new FileExplorer());
                blocks.Add(new Model3D());
            }
        });

        var config = await GetConfigAsync(host, cancellationToken);

        var editor = Props(config, id: 1);
        Assert.Equal(["upload", "webcam", "clipboard"], Strings(editor["sources"]));
        Assert.Equal("numpy", editor["type"]!.GetValue<string>());
        Assert.Equal("RGBA", editor["image_mode"]!.GetValue<string>());
        Assert.Equal("webp", editor["format"]!.GetValue<string>());
        Assert.Equal([800, 800], Numbers(editor["canvas_size"]));
        Assert.Equal(["crop", "resize"], Strings(editor["transforms"]));
        Assert.False(editor["fixed_canvas"]!.GetValue<bool>());

        // The three tool options are computed when the application names none, which is why they
        // are asserted rather than assumed: an editor that hid its brush by default would look the
        // same in the config as one that offered it, and behave differently.
        Assert.Equal("auto", editor["eraser"]!["default_size"]!.GetValue<string>());
        Assert.Equal(5, editor["brush"]!["colors"]!.AsArray().Count);
        Assert.Equal("rgb(204, 50, 50)", editor["brush"]!["default_color"]!.GetValue<string>());
        Assert.Equal("defaults", editor["brush"]!["color_mode"]!.GetValue<string>());
        Assert.True(editor["layers"]!["allow_additional_layers"]!.GetValue<bool>());
        Assert.Equal(["Layer 1"], Strings(editor["layers"]!["layers"]));
        Assert.False(editor["layers"]!["disabled"]!.GetValue<bool>());

        // No buttons were named, and the Python version's default for an editor is to draw all of them —
        // which it says by carrying no `buttons` key at all.
        Assert.False(editor.ContainsKey("buttons"));

        // The editor is the first component whose behaviour is a server function of its own.
        Assert.Equal(["accept_blobs"], Strings(editor["server_fns"]));

        var slider = Props(config, id: 2);
        Assert.Equal(["download", "fullscreen"], Strings(slider["buttons"]));
        Assert.Equal(50, slider["slider_position"]!.GetValue<double>());
        Assert.Equal(500, slider["max_height"]!.GetValue<int>());
        Assert.Equal("RGB", slider["image_mode"]!.GetValue<string>());
        Assert.False(slider["streamable"]!.GetValue<bool>());

        var video = Props(config, id: 3);
        Assert.Equal(["upload", "webcam"], Strings(video["sources"]));
        Assert.True(video["include_audio"]!.GetValue<bool>());
        Assert.Equal(["download"], Strings(video["buttons"]));
        Assert.Equal("bottom-right", video["watermark"]!["position"]!.GetValue<string>());
        Assert.False(video["streaming"]!.GetValue<bool>());
        Assert.False(video["loop"]!.GetValue<bool>());
        Assert.Equal(0, video["playback_position"]!.GetValue<double>());

        var audio = Props(config, id: 4);
        Assert.Equal(["upload", "microphone"], Strings(audio["sources"]));
        Assert.Equal(["download", "share"], Strings(audio["buttons"]));
        Assert.Equal(44100, audio["waveform_options"]!["sample_rate"]!.GetValue<int>());
        Assert.Equal(5, audio["waveform_options"]!["skip_length"]!.GetValue<double>());
        Assert.True(audio["waveform_options"]!["show_recording_waveform"]!.GetValue<bool>());
        Assert.True(audio["editable"]!.GetValue<bool>());
        Assert.False(audio["recording"]!.GetValue<bool>());

        // The audio gallery is a custom HTML component: its own parameters travel in the extra-props
        // dictionary, and its value is the first file it was given — none, here.
        var galleryOfAudio = config["components"]!.AsArray()[4]!;
        Assert.Equal("html", galleryOfAudio["type"]!.GetValue<string>());
        Assert.Equal("audiogallery", galleryOfAudio["props"]!["name"]!.GetValue<string>());
        Assert.Equal(3, galleryOfAudio["props"]!["props"]!["columns"]!.GetValue<int>());
        // A null inside the extra-props dictionary is written rather than dropped: the top-level
        // props are the ones the Python version strips its nulls from.
        Assert.Null(galleryOfAudio["props"]!["props"]!["labels"]);
        Assert.False(galleryOfAudio["props"]!.AsObject().ContainsKey("value"));

        var gallery = Props(config, id: 6);
        Assert.Empty(gallery["value"]!.AsArray());
        Assert.Equal(2, gallery["columns"]!.GetValue<int>());
        Assert.Equal(["share", "download", "download_all", "fullscreen"], Strings(gallery["buttons"]));
        Assert.Equal(["upload"], Strings(gallery["sources"]));
        Assert.True(gallery["fit_columns"]!.GetValue<bool>());
        Assert.True(gallery["allow_preview"]!.GetValue<bool>());

        var explorer = Props(config, id: 7);
        Assert.Equal("**/*", explorer["glob"]!.GetValue<string>());
        Assert.Equal("multiple", explorer["file_count"]!.GetValue<string>());
        Assert.Equal(500, explorer["max_height"]!.GetValue<int>());
        Assert.Equal(["ls"], Strings(explorer["server_fns"]));
        Assert.True(Path.IsPathRooted(explorer["root_dir"]!.GetValue<string>()));

        var model = Props(config, id: 8);
        Assert.Equal([0, 0, 0, 0], Numbers(model["clear_color"]));
        Assert.Equal(3, model["camera_position"]!.AsArray().Count);
        Assert.All(model["camera_position"]!.AsArray(), position => Assert.Null(position));
        Assert.Equal(1, model["zoom_speed"]!.GetValue<double>());
        Assert.Equal(1, model["pan_speed"]!.GetValue<double>());
        Assert.Empty(model["buttons"]!.AsArray());
    }

    [Fact]
    public async Task An_image_editor_hands_a_handler_three_paths_and_takes_three_back()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        using var elsewhere = new TempFolder();

        ImageEditor editor = null!;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            blocks.UploadFolder = folder.Folder;

            using (blocks.BeginRender())
            {
                editor = blocks.Add(new ImageEditor { Type = "filepath" });
            }
        });

        // Real pictures: an editor decodes what it is given, so a file named .png that holds text
        // is not one.
        var background = elsewhere.Write("background.png", TestImages.Png(4, 4, red: 255));
        var layer = elsewhere.Write("layer.png", TestImages.Png(4, 4, green: 255));

        var value = Assert.IsType<EditorValue>(editor.Preprocess(new JsonObject
        {
            ["background"] = new JsonObject { ["path"] = background },
            ["layers"] = new JsonArray(new JsonObject { ["path"] = layer }),
            ["composite"] = new JsonObject { ["path"] = background },
            ["id"] = null,
        }));

        // Every image is a cached path, which is what makes a handler able to read what the editor
        // drew — the browser's paths are not ones this server keeps.
        Assert.True(MediaCache.IsInFolder(Assert.IsType<string>(value.Background), folder.Folder));
        Assert.Single(value.Layers);
        Assert.True(MediaCache.IsInFolder(Assert.IsType<string>(value.Composite), folder.Folder));

        var sent = Assert.IsType<JsonObject>(editor.Postprocess(new EditorValue
        {
            Background = background,
            Layers = [layer],
            Composite = background,
        }));

        Assert.Equal("background.png", sent["background"]!["orig_name"]!.GetValue<string>());
        Assert.StartsWith("/gradio_api/file=", sent["background"]!["url"]!.GetValue<string>(), StringComparison.Ordinal);
        Assert.Single(sent["layers"]!.AsArray());
        Assert.Null(sent["id"]);
    }

    [Fact]
    public async Task A_streamed_audio_chunk_is_measured_from_its_frames()
    {
        var audio = new Audio { Streaming = true };

        // Ten ADTS frames of 100 bytes each at 44.1 kHz: every frame carries 1024 samples, so the
        // run lasts 10240/44100 seconds. Nothing else can say how long packed audio is without
        // decoding it, which is why the playlist's timing rests on this.
        var frames = Adts(frames: 10, frameLength: 100, sampleRateIndex: 4);

        var (chunk, file) = await audio
            .StreamOutputAsync(frames, "session/run/1/playlist.m3u8", firstChunk: true);

        var segment = Assert.IsType<MediaStreamChunk>(chunk);
        Assert.Equal(".aac", segment.Extension);
        Assert.Equal(10 * 1024 / 44100.0, segment.Duration, precision: 6);

        // The client is told where the playlist is, and that what it is being given is a stream
        // rather than a file — which is what makes the frontend play it as it arrives.
        Assert.Equal("session/run/1/playlist.m3u8", file.Path);
        Assert.Equal("/gradio_api/stream/session/run/1/playlist.m3u8", file.Url);
        Assert.True(file.IsStream);
        Assert.Equal("audio-stream.mp3", file.OrigName);
    }

    [Fact]
    public async Task A_streamed_video_chunk_must_already_be_transport_stream()
    {
        var video = new Video { Streaming = true };

        var refused = await Assert.ThrowsAsync<NotSupportedException>(() => video
            .StreamOutputAsync("not a transport stream"u8.ToArray(), "session/run/1/playlist.m3u8", firstChunk: true));

        Assert.Contains("MediaStreamChunk", refused.Message, StringComparison.Ordinal);

        // A transport stream is a run of packets, each beginning with the sync byte; bytes that
        // begin with one are served as they are.
        var packets = new byte[188 * 2];
        packets[0] = 0x47;
        packets[188] = 0x47;

        var (chunk, file) = await video
            .StreamOutputAsync(packets, "session/run/1/playlist.m3u8", firstChunk: true);

        Assert.Equal(".ts", Assert.IsType<MediaStreamChunk>(chunk).Extension);
        Assert.Equal("video-stream.mp4", file.OrigName);
    }

    [Fact]
    public void A_segment_is_served_as_audio_or_video_and_nothing_else()
    {
        Assert.Throws<ArgumentException>(() => new MediaStreamChunk([1, 2, 3], 1, ".mp4"));
        Assert.Throws<ArgumentOutOfRangeException>(() => new MediaStreamChunk([1, 2, 3], -1, ".aac"));
    }

    [Fact]
    public async Task A_file_explorer_lists_folders_first_and_hides_what_the_pattern_hides()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();

        System.IO.File.WriteAllText(Path.Combine(folder.Folder, "keep.txt"), "kept");
        System.IO.File.WriteAllText(Path.Combine(folder.Folder, "skip.py"), "skipped");
        Directory.CreateDirectory(Path.Combine(folder.Folder, "nested"));

        FileExplorer explorer = null!;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            using (blocks.BeginRender())
            {
                explorer = blocks.Add(new FileExplorer
                {
                    RootDir = folder.Folder,
                    Glob = "*.txt",
                    IgnoreGlob = "*.py",
                    FileCount = "single",
                });
            }
        });

        var listing = explorer.List();

        // A folder is listed whether or not it matches: a folder the user cannot open is one they
        // cannot reach what they want through.
        Assert.Equal(["nested", "keep.txt"], listing.Select(entry => entry!["name"]!.GetValue<string>()));
        Assert.Equal("folder", listing[0]!["type"]!.GetValue<string>());
        Assert.Equal("file", listing[1]!["type"]!.GetValue<string>());
        Assert.True(listing[1]!["valid"]!.GetValue<bool>());

        // A file the ignore pattern names is not listed at all, and neither is one the glob does
        // not match — the two filters are different questions.
        Assert.DoesNotContain(listing, entry => entry!["name"]!.GetValue<string>() == "skip.py");

        var selected = Assert.IsType<string>(explorer.Preprocess(new JsonArray(
            new JsonArray("nested", "keep.txt"))));

        Assert.Equal(Path.Combine(folder.Folder, "nested", "keep.txt"), selected);

        // A selection that climbs out of the browsed directory is refused rather than resolved.
        Assert.Throws<InvalidOperationException>(() => explorer.Preprocess(new JsonArray(
            new JsonArray("..", "elsewhere"))));
    }

    [Fact]
    public async Task A_gallery_sends_a_video_as_a_video_and_an_image_as_an_image()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        using var elsewhere = new TempFolder();

        Gallery gallery = null!;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            blocks.UploadFolder = folder.Folder;

            using (blocks.BeginRender())
            {
                gallery = blocks.Add(new Gallery());
            }
        });

        var image = elsewhere.Write("still.png", "image");
        var video = elsewhere.Write("clip.mp4", "video");

        var sent = Assert.IsType<JsonArray>(gallery.Postprocess(new GalleryEntry[]
        {
            new(image, "a caption"),
            new(video),
        }));

        Assert.Equal("a caption", sent[0]!["caption"]!.GetValue<string>());
        Assert.NotNull(sent[0]!["image"]);
        Assert.Null(sent[0]!["video"]);

        // Which of the two an entry is decided from its name, which is what the Python version's mime guess
        // comes down to for the formats a gallery accepts.
        Assert.NotNull(sent[1]!["video"]);
        Assert.Null(sent[1]!["image"]);
    }

    [Fact]
    public async Task A_gallery_hands_the_handler_the_entry_the_user_picked()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        using var elsewhere = new TempFolder();

        Textbox picked = null!;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            blocks.UploadFolder = folder.Folder;

            using (blocks.BeginRender())
            {
                var gallery = blocks.Add(new Gallery { Label = "Pictures" });
                picked = blocks.Add(new Textbox { Label = "Picked" });

                gallery.Select(
                    (Func<IReadOnlyList<GalleryEntry>, string>)(entries => entries[0].Media as string ?? ""),
                    [gallery],
                    [picked],
                    apiName: "pick");
            }
        });

        // The entry the user clicked is a file object on its way in, and the handler is handed the
        // cached path of it — which is what a selection is: one entry of the gallery's list.
        var image = elsewhere.Write("chosen.png", "chosen");

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("pick", cancellationToken);
        var eventId = await queue.JoinAsync(
            fnIndex,
            [new JsonArray(new JsonObject
            {
                ["image"] = new JsonObject { ["path"] = image, ["orig_name"] = "chosen.png" },
                ["caption"] = null,
            })],
            "gallery",
            cancellationToken);

        await using var stream = await queue.OpenAsync("gallery", cancellationToken);
        var completed = await stream.UntilAsync(
            message => QueueSession.Kind(message) == "process_completed"
                && message["event_id"]?.GetValue<string>() == eventId,
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.NotNull(completed);

        var path = completed["output"]!["data"]!.AsArray()[0]!.GetValue<string>();
        Assert.True(MediaCache.IsInFolder(path, folder.Folder));
        Assert.Equal("chosen", await System.IO.File.ReadAllTextAsync(path, cancellationToken));
    }

    /// <summary>A run of ADTS frames, as packed AAC arrives.</summary>
    /// <param name="frames">How many frames.</param>
    /// <param name="frameLength">How long each one is, header included.</param>
    /// <param name="sampleRateIndex">Which sample rate the header names.</param>
    private static byte[] Adts(int frames, int frameLength, int sampleRateIndex)
    {
        var bytes = new byte[frames * frameLength];

        for (var frame = 0; frame < frames; frame++)
        {
            var offset = frame * frameLength;

            bytes[offset] = 0xFF;
            bytes[offset + 1] = 0xF1;
            bytes[offset + 2] = (byte)(sampleRateIndex << 2);
            bytes[offset + 3] = (byte)((frameLength >> 11) & 0x03);
            bytes[offset + 4] = (byte)((frameLength >> 3) & 0xFF);
            bytes[offset + 5] = (byte)((frameLength & 0x07) << 5);
        }

        return bytes;
    }

    /// <summary>The served config.</summary>
    private static async Task<JsonObject> GetConfigAsync(GradioTestHost host, CancellationToken cancellationToken)
    {
        using var response = await host.Client.GetAsync("/config", cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        return Assert.IsType<JsonObject>(JsonNode.Parse(body));
    }

    /// <summary>One component's props, by id.</summary>
    private static JsonObject Props(JsonObject config, int id) =>
        config["components"]!
            .AsArray()
            .Single(component => component!["id"]!.GetValue<int>() == id)!["props"]!
            .AsObject();

    private static string[] Strings(JsonNode? node) =>
        [.. node!.AsArray().Select(item => item!.GetValue<string>())];

    private static int[] Numbers(JsonNode? node) =>
        [.. node!.AsArray().Select(item => item!.GetValue<int>())];
}

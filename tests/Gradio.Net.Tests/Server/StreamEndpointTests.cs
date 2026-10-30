using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// Streaming media over HTTP: the playlist a player reads, the segments it plays, the whole stream as
/// one file, and the routes a streamed run is fed through.
/// </summary>
/// <remarks>
/// <para>
/// The claim these assert is the one the ticket makes: the playlist and the segments are served by
/// the Python version's rules. That is a claim about bytes a browser's own player consumes — the
/// header lines, the timing of each segment, the end marker that stops a player polling, the media
/// types — so each is asserted rather than "a playlist came back".
/// </para>
/// <para>
/// The run that produces the stream is a real one: it goes through the queue, its chunks become
/// segments as it yields them, and the file object the client is told about is the address the
/// playlist is actually served at. A test that wrote segments into a registry by hand would prove
/// the routes and nothing about the wiring between them and a handler.
/// </para>
/// </remarks>
public sealed class StreamEndpointTests
{
    /// <summary>The bytes of the first segment, which are not real audio: only their identity matters.</summary>
    private static readonly byte[] FirstSegment = "first-segment"u8.ToArray();

    /// <summary>The bytes of the second segment.</summary>
    private static readonly byte[] SecondSegment = "second-segment"u8.ToArray();

    [Fact]
    public async Task A_streamed_audio_output_is_served_as_a_playlist_of_its_segments()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(
            null,
            cancellationToken,
            blocks => AddStreamingAudio(blocks, Chunks));

        var (playlist, path, url) = await PlaylistAsync(host, "stream_audio", "streams", cancellationToken);

        // What the client is told is a stream rather than a file: the address is the playlist's, and
        // the flag is what makes the frontend play it as it arrives instead of downloading it.
        Assert.StartsWith("/gradio_api/stream/", url, StringComparison.Ordinal);

        Assert.StartsWith("#EXTM3U\n", playlist, StringComparison.Ordinal);
        Assert.Contains("#EXT-X-PLAYLIST-TYPE:EVENT\n", playlist, StringComparison.Ordinal);
        Assert.Contains("#EXT-X-TARGETDURATION:1\n", playlist, StringComparison.Ordinal);
        Assert.Contains("#EXT-X-VERSION:4\n", playlist, StringComparison.Ordinal);
        Assert.Contains("#EXT-X-MEDIA-SEQUENCE:0\n", playlist, StringComparison.Ordinal);
        Assert.Contains("#EXT-X-START:TIME-OFFSET=0\n", playlist, StringComparison.Ordinal);

        // One line per segment, each naming how long it lasts — which is what a player places the
        // next segment by. The run is over, so the playlist says no more are coming.
        Assert.Contains("#EXTINF:0.250000,\n", playlist, StringComparison.Ordinal);
        Assert.Contains("#EXTINF:0.500000,\n", playlist, StringComparison.Ordinal);
        Assert.EndsWith("#EXT-X-ENDLIST\n", playlist, StringComparison.Ordinal);

        var segments = playlist
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Where(line => line.EndsWith(".aac", StringComparison.Ordinal))
            .ToArray();

        Assert.Equal(2, segments.Length);

        using var first = await host.Client.GetAsync(
            $"/gradio_api/stream/{FolderOf(path)}{segments[0]}",
            cancellationToken);

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal("audio/aac", first.Content.Headers.ContentType?.MediaType);
        Assert.Equal(FirstSegment, await first.Content.ReadAsByteArrayAsync(cancellationToken));

        // The second segment is served the same way as the first: a playlist that names two and
        // serves one is a player that stalls halfway.
        using var second = await host.Client.GetAsync(
            $"/gradio_api/stream/{FolderOf(path)}{segments[1]}",
            cancellationToken);

        Assert.Equal(HttpStatusCode.OK, second.StatusCode);
        Assert.Equal("audio/aac", second.Content.Headers.ContentType?.MediaType);
        Assert.Equal(SecondSegment, await second.Content.ReadAsByteArrayAsync(cancellationToken));

        // A name the stream does not hold is a third refusal, and a different one from a stream
        // that is not there at all: the stream exists, the segment does not.
        using var noSegment = await host.Client.GetAsync(
            $"/gradio_api/stream/{FolderOf(path)}not-a-segment.aac",
            cancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, noSegment.StatusCode);
        Assert.Equal(
            "Segment not found",
            await noSegment.Content.ReadAsStringAsync(cancellationToken));

        // A signed request carries its signature into every address the playlist names, so the
        // player's own requests for the segments stay signed, and the answer is no longer public.
        using var signed = await host.Client.GetAsync($"{url}?__sign=abc123", cancellationToken);

        Assert.Equal(HttpStatusCode.OK, signed.StatusCode);

        // The header is asserted by what it says rather than by how it is spelled: the framework
        // parses it into a value and writes it back in its own order.
        Assert.True(signed.Headers.CacheControl?.Private);
        Assert.True(signed.Headers.CacheControl?.NoStore);

        var signedPlaylist = await signed.Content.ReadAsStringAsync(cancellationToken);
        var signedSegments = signedPlaylist
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Where(line => line.EndsWith(".aac?__sign=abc123", StringComparison.Ordinal))
            .ToArray();

        Assert.Equal(2, signedSegments.Length);

        // The whole stream as one file is the segments one after another, and it is written once:
        // asking twice answers the same file rather than combining the stream again.
        using var combined = await host.Client.GetAsync(
            $"/gradio_api/stream/{FolderOf(path)}playlist-file",
            cancellationToken);

        Assert.Equal(HttpStatusCode.OK, combined.StatusCode);

        byte[] expected = [.. FirstSegment, .. SecondSegment];
        Assert.Equal(expected, await combined.Content.ReadAsByteArrayAsync(cancellationToken));
    }

    /// <summary>
    /// A streamed video is served the same way an audio one is, with transport-stream segments.
    /// </summary>
    /// <remarks>
    /// The video case is the one the feature exists for — a long recording played while it is still
    /// being made — and the only thing it adds to the audio case is the segment's own shape:
    /// MPEG-TS packets, named <c>.ts</c> and served as <c>video/MP2T</c>. The playlist's lines are
    /// the Python version's for both, because the playlist is a view of what the stream holds
    /// rather than of what it is.
    /// </remarks>
    [Fact]
    public async Task A_streamed_video_output_is_served_as_a_playlist_of_transport_stream_segments()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(
            null,
            cancellationToken,
            blocks => AddStreamingVideo(blocks, VideoChunks));

        var (playlist, path, _) = await PlaylistAsync(host, "stream_video", "streams-video", cancellationToken);

        // The same header an audio stream gets, line for line: the playlist is a view of what the
        // stream holds, and what it holds does not change what a player is told about it.
        Assert.StartsWith("#EXTM3U\n", playlist, StringComparison.Ordinal);
        Assert.Contains("#EXT-X-PLAYLIST-TYPE:EVENT\n", playlist, StringComparison.Ordinal);
        Assert.Contains("#EXT-X-TARGETDURATION:1\n", playlist, StringComparison.Ordinal);
        Assert.Contains("#EXT-X-VERSION:4\n", playlist, StringComparison.Ordinal);
        Assert.Contains("#EXT-X-MEDIA-SEQUENCE:0\n", playlist, StringComparison.Ordinal);
        Assert.Contains("#EXT-X-START:TIME-OFFSET=0\n", playlist, StringComparison.Ordinal);
        Assert.Contains("#EXTINF:0.500000,\n", playlist, StringComparison.Ordinal);
        Assert.Contains("#EXTINF:1.000000,\n", playlist, StringComparison.Ordinal);
        Assert.EndsWith("#EXT-X-ENDLIST\n", playlist, StringComparison.Ordinal);

        var segments = playlist
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Where(line => line.EndsWith(".ts", StringComparison.Ordinal))
            .ToArray();

        Assert.Equal(2, segments.Length);

        // Upper case, because that is how the Python version spells it (`media_type="video/MP2T"`): a media
        // type is case-insensitive, and the point of this framework is to answer what the Python version
        // answers rather than what reads better.
        byte[][] wanted = [FirstPacket, SecondPacket];

        for (var index = 0; index < segments.Length; index++)
        {
            using var segment = await host.Client.GetAsync(
                $"/gradio_api/stream/{FolderOf(path)}{segments[index]}",
                cancellationToken);

            Assert.Equal(HttpStatusCode.OK, segment.StatusCode);
            Assert.Equal("video/MP2T", segment.Content.Headers.ContentType?.MediaType);
            Assert.Equal(wanted[index], await segment.Content.ReadAsByteArrayAsync(cancellationToken));
        }

        // And the whole stream as one file, which is the segments one after another — the remux
        // the Python version does with ffmpeg is the part this does not do, and concatenation is what is left.
        using var combined = await host.Client.GetAsync(
            $"/gradio_api/stream/{FolderOf(path)}playlist-file",
            cancellationToken);

        Assert.Equal(HttpStatusCode.OK, combined.StatusCode);

        byte[] expected = [.. FirstPacket, .. SecondPacket];
        Assert.Equal(expected, await combined.Content.ReadAsByteArrayAsync(cancellationToken));
    }

    /// <summary>
    /// The playlist grows while the stream is running, which is what makes it playable as it arrives.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The other streaming tests read the playlist after the run is over, so they would pass for an
    /// implementation that only wrote it at the end — which is not a stream, it is a file that took a
    /// long time. This one reads it in the middle: one segment, no end marker, and the second
    /// segment arriving after the handler is let go.
    /// </para>
    /// <para>
    /// The handler is held by a task the test completes, so the moment is the test's rather than a
    /// delay's: a playlist read too late would have both segments and prove nothing.
    /// </para>
    /// </remarks>
    [Fact]
    public async Task A_playlist_grows_while_the_stream_is_still_running()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var firstYielded = new TaskCompletionSource();
        var secondAllowed = new TaskCompletionSource();

        async IAsyncEnumerable<object?> Held()
        {
            yield return new MediaStreamChunk(FirstPacket, 0.5, ".ts");

            firstYielded.TrySetResult();

            await secondAllowed.Task.ConfigureAwait(false);

            yield return new MediaStreamChunk(SecondPacket, 1, ".ts");
        }

        await using var host = await GradioTestHost.StartStandaloneAsync(
            null,
            cancellationToken,
            blocks => AddStreamingVideo(blocks, Held));

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("stream_video", cancellationToken);
        var eventId = await queue.JoinAsync(fnIndex, [], "growing", cancellationToken);

        await using var stream = await queue.OpenAsync("growing", cancellationToken);

        // The first chunk is what tells the client where the playlist is: the run is still going, so
        // this is a generating message rather than the completed one.
        var first = await stream.UntilAsync(
            message => message["event_id"]?.GetValue<string>() == eventId
                && QueueSession.Kind(message) == "process_generating"
                && message["output"]?["data"] is JsonArray data
                && data.Count > 0
                && data[0]?["is_stream"]?.GetValue<bool>() == true,
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.NotNull(first);

        // The message is sent as soon as the chunk is stored, which can be a moment before the
        // handler resumes; waiting for it is what makes the next read happen while the handler is
        // parked rather than while it is still producing.
        await firstYielded.Task.WaitAsync(TimeSpan.FromSeconds(15), cancellationToken);

        var url = first!["output"]!["data"]![0]!["url"]!.GetValue<string>();

        using (var running = await host.Client.GetAsync(url, cancellationToken))
        {
            Assert.Equal(HttpStatusCode.OK, running.StatusCode);

            var playlist = await running.Content.ReadAsStringAsync(cancellationToken);

            Assert.Contains("#EXTINF:0.500000,\n", playlist, StringComparison.Ordinal);

            // One segment so far, and no end marker: a player that read this now would wait for
            // more rather than treat the stream as finished.
            Assert.DoesNotContain("#EXT-X-ENDLIST", playlist, StringComparison.Ordinal);
            Assert.Single(
                playlist.Split('\n', StringSplitOptions.RemoveEmptyEntries),
                line => line.EndsWith(".ts", StringComparison.Ordinal));
        }

        secondAllowed.TrySetResult();

        var completed = await stream.UntilAsync(
            message => QueueSession.Kind(message) == "process_completed"
                && message["event_id"]?.GetValue<string>() == eventId,
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.NotNull(completed);

        using var ended = await host.Client.GetAsync(url, cancellationToken);
        var endedPlaylist = await ended.Content.ReadAsStringAsync(cancellationToken);

        Assert.Contains("#EXTINF:1.000000,\n", endedPlaylist, StringComparison.Ordinal);
        Assert.EndsWith("#EXT-X-ENDLIST\n", endedPlaylist, StringComparison.Ordinal);
    }

    [Fact]
    public async Task A_stream_is_ended_rather_than_forgotten_when_the_connection_closes()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(
            null,
            cancellationToken,
            blocks => AddStreamingAudio(blocks, Chunks));

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("stream_audio", cancellationToken);
        var eventId = await queue.JoinAsync(fnIndex, [], "closing", cancellationToken);

        var stream = await queue.OpenAsync("closing", cancellationToken);
        var completed = await stream.UntilAsync(
            message => QueueSession.Kind(message) == "process_completed"
                && message["event_id"]?.GetValue<string>() == eventId,
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.NotNull(completed);

        var url = completed["output"]!["data"]!.AsArray()[0]!["url"]!.GetValue<string>();

        // The client has been told about the stream, and now it goes away: this is the order the
        // ticket's decision is about. The server notices a closed connection on its own thread, so
        // the playlist is asked for across a window rather than once — a 404 at any point in it is
        // the stream having been dropped rather than ended, which is the failure this guards.
        await stream.DisposeAsync();

        var deadline = System.DateTime.UtcNow + TimeSpan.FromSeconds(1);
        while (System.DateTime.UtcNow < deadline)
        {
            using var playlist = await host.Client.GetAsync(url, cancellationToken);

            Assert.Equal(HttpStatusCode.OK, playlist.StatusCode);

            await Task.Delay(50, cancellationToken);
        }
    }

    [Fact]
    public async Task An_update_a_streaming_output_asked_for_is_not_replaced_by_the_stream()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(
            null,
            cancellationToken,
            blocks => AddStreamingAudio(blocks, UpdateThenChunks));

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("stream_audio", cancellationToken);
        var eventId = await queue.JoinAsync(fnIndex, [], "updating", cancellationToken);

        await using var stream = await queue.OpenAsync("updating", cancellationToken);

        var generating = await stream.UntilAsync(
            message => QueueSession.Kind(message) == "process_generating"
                && message["event_id"]?.GetValue<string>() == eventId,
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.NotNull(generating);

        // The handler asked the output to become this component. A streamed output is the one case
        // where the payload is not what the handler returned, and the update is the exception to
        // that exception: replacing it with a playlist address would lose the update and open a
        // stream for a chunk that never existed.
        var update = Assert.IsType<JsonObject>(generating["output"]!["data"]!.AsArray()[0]);
        Assert.Equal("update", update["__type__"]!.GetValue<string>());
        Assert.Equal("Updated", update["label"]!.GetValue<string>());

        var completed = await stream.UntilAsync(
            message => QueueSession.Kind(message) == "process_completed"
                && message["event_id"]?.GetValue<string>() == eventId,
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.NotNull(completed);

        var file = Assert.IsType<JsonObject>(completed["output"]!["data"]!.AsArray()[0]);
        Assert.True(file["is_stream"]!.GetValue<bool>());

        // One segment, from the one chunk: the update is not a chunk and must not have become one.
        using var playlist = await host.Client.GetAsync(file["url"]!.GetValue<string>(), cancellationToken);
        var lines = (await playlist.Content.ReadAsStringAsync(cancellationToken))
            .Split('\n', StringSplitOptions.RemoveEmptyEntries);

        Assert.Single(lines, line => line.StartsWith("#EXTINF:", StringComparison.Ordinal));
    }

    [Fact]
    public async Task A_stream_nobody_opened_is_missing_and_a_segment_of_another_type_is_refused()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken);

        using var missing = await host.Client.GetAsync(
            "/gradio_api/stream/session/run/1/playlist.m3u8",
            cancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);

        using var refused = await host.Client.GetAsync(
            "/gradio_api/stream/session/run/1/segment.mp4",
            cancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, refused.StatusCode);
        Assert.Equal(
            "Unsupported file extension",
            await refused.Content.ReadAsStringAsync(cancellationToken));

        // A segment of a stream that is not there is a different answer from one of a stream that
        // is: the extension is checked first, which is the Python version's order.
        using var noStream = await host.Client.GetAsync(
            "/gradio_api/stream/session/run/1/segment.aac",
            cancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, noStream.StatusCode);
        Assert.Equal("Stream not found", await noStream.Content.ReadAsStringAsync(cancellationToken));
    }

    [Fact]
    public async Task The_routes_that_feed_a_streamed_run_refuse_an_event_nobody_admitted()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken);

        using var feed = await host.Client.PostAsync(
            "/gradio_api/stream/not-an-event",
            new StringContent("""{"data":[]}""", Encoding.UTF8, "application/json"),
            cancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, feed.StatusCode);

        using var close = await host.Client.PostAsync(
            "/gradio_api/stream/not-an-event/close",
            content: null,
            cancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, close.StatusCode);
    }

    [Fact]
    public async Task A_file_explorer_lists_a_folder_when_the_browser_asks_it_to()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();

        System.IO.File.WriteAllText(Path.Combine(folder.Folder, "kept.txt"), "kept");
        Directory.CreateDirectory(Path.Combine(folder.Folder, "nested"));

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            using (blocks.BeginRender())
            {
                blocks.Add(new FileExplorer { RootDir = folder.Folder });
            }
        });

        using var response = await host.Client.PostAsync(
            "/gradio_api/component_server",
            Json("""{"session_hash":"browse","component_id":1,"fn_name":"ls","data":[]}"""),
            cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var listing = JsonNode.Parse(await response.Content.ReadAsStringAsync(cancellationToken))!.AsArray();
        Assert.Equal(["nested", "kept.txt"], listing.Select(entry => entry!["name"]!.GetValue<string>()));

        // A function the component does not declare is a 404 rather than a 400: the route exists and
        // the component exists, and what was asked for is not something it offers.
        using var unknown = await host.Client.PostAsync(
            "/gradio_api/component_server",
            Json("""{"session_hash":"browse","component_id":1,"fn_name":"delete_everything","data":[]}"""),
            cancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, unknown.StatusCode);
        Assert.Equal(
            """{"detail":"Function not found."}""",
            await unknown.Content.ReadAsStringAsync(cancellationToken));

        using var invalid = await host.Client.PostAsync(
            "/gradio_api/component_server",
            new StringContent("not json", Encoding.UTF8, "application/json"),
            cancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
    }

    [Fact]
    public async Task A_component_server_call_carrying_a_file_is_held_to_the_applications_limit()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        ImageEditor editor = null!;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            blocks.UploadFolder = folder.Folder;

            // The limit the Python version's parser is given on this route too: one file, and no more than
            // the application accepts.
            blocks.MaxFileSize = 8;

            using (blocks.BeginRender())
            {
                editor = blocks.Add(new ImageEditor { Type = "filepath" });
            }
        });

        using var oversized = Blobs(editor, "a canvas well past the limit"u8.ToArray(), extraFile: false);
        using var refused = await host.Client.PostAsync("/gradio_api/component_server", oversized, cancellationToken);

        // 413 rather than 400: the body is well formed, and what is wrong with it is its size —
        // which is the pair the Python version's parser draws.
        Assert.Equal(HttpStatusCode.RequestEntityTooLarge, refused.StatusCode);
        Assert.Equal(
            """{"detail":"File size exceeded."}""",
            await refused.Content.ReadAsStringAsync(cancellationToken));

        using var twoFiles = Blobs(editor, "a"u8.ToArray(), extraFile: true);
        using var tooMany = await host.Client.PostAsync("/gradio_api/component_server", twoFiles, cancellationToken);

        // This route takes one file, as the Python version's parser is told to: a second one is a body the
        // route does not accept rather than a size problem.
        Assert.Equal(HttpStatusCode.BadRequest, tooMany.StatusCode);
        Assert.Equal(
            """{"detail":"Too many files."}""",
            await tooMany.Content.ReadAsStringAsync(cancellationToken));

        static MultipartFormDataContent Blobs(ImageEditor editor, byte[] blob, bool extraFile)
        {
            var form = new MultipartFormDataContent
            {
                { new StringContent("blob-1"), "id" },
                { new StringContent("background"), "type" },
                { new StringContent("null"), "index" },
                { new StringContent("editor"), "session_hash" },
                { new StringContent(editor.Id.ToString(System.Globalization.CultureInfo.InvariantCulture)), "component_id" },
                { new StringContent("accept_blobs"), "fn_name" },
                { new StringContent("""{"id":"blob-1","type":"background","index":null}"""), "data" },
            };

            var file = new ByteArrayContent(blob);
            file.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            form.Add(file, "files", "canvas.png");

            if (extraFile)
            {
                var second = new ByteArrayContent(blob);
                second.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                form.Add(second, "files", "canvas-2.png");
            }

            return form;
        }
    }

    [Fact]
    public async Task An_image_editor_takes_a_canvas_the_browser_uploaded_out_of_band()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();

        ImageEditor editor = null!;
        Textbox output = null!;

        using var logs = new CapturingLoggerProvider();

        await using var host = await GradioTestHost.StartStandaloneAsync(logs, cancellationToken, blocks =>
        {
            blocks.UploadFolder = folder.Folder;

            using (blocks.BeginRender())
            {
                editor = blocks.Add(new ImageEditor { Type = "filepath" });
                output = blocks.Add(new Textbox { Label = "Path" });

                editor.Change(
                    (Func<EditorValue, string>)(value => value.Background as string ?? "none"),
                    inputs: [editor],
                    outputs: [output],
                    apiName: "editor_path");
            }
        });

        // The browser sends the canvas as a file beside the id it will name in the value, which is
        // what lets a large image reach the server without being put in the request's JSON.
        var blob = TestImages.Png(4, 4, blue: 255);
        using var form = new MultipartFormDataContent
        {
            { new StringContent("blob-1"), "id" },
            { new StringContent("background"), "type" },
            { new StringContent("null"), "index" },
            { new StringContent("editor"), "session_hash" },
            { new StringContent(editor.Id.ToString(System.Globalization.CultureInfo.InvariantCulture)), "component_id" },
            { new StringContent("accept_blobs"), "fn_name" },
            { new StringContent("""{"id":"blob-1","type":"background","index":null}"""), "data" },
        };

        var file = new ByteArrayContent(blob);
        file.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        form.Add(file, "files", "canvas.png");

        using var accepted = await host.Client.PostAsync("/gradio_api/component_server", form, cancellationToken);
        Assert.True(
            accepted.StatusCode == HttpStatusCode.OK,
            $"{(int)accepted.StatusCode}: {await accepted.Content.ReadAsStringAsync(cancellationToken)} | "
            + string.Join(" || ", logs.Entries.Select(entry => entry.Message)));

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("editor_path", cancellationToken);
        var eventId = await queue.JoinAsync(
            fnIndex,
            [new JsonObject
            {
                ["background"] = null,
                ["layers"] = new JsonArray(),
                ["composite"] = null,
                ["id"] = "blob-1",
            }],
            "editor",
            cancellationToken);

        await using var stream = await queue.OpenAsync("editor", cancellationToken);
        var completed = await stream.UntilAsync(
            message => QueueSession.Kind(message) == "process_completed"
                && message["event_id"]?.GetValue<string>() == eventId,
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.NotNull(completed);

        // The handler was handed the cached image the blob became, which is the whole point of the
        // detour: the value named an id, and the editor is what knew what the id meant. The bytes
        // are the editor's format rather than the ones that were uploaded — it writes in `format`,
        // and its default is webp — so the assertion is about the picture, not the file.
        var path = completed["output"]!["data"]!.AsArray()[0]!.GetValue<string>();

        Assert.True(MediaCache.IsInFolder(path, folder.Folder));

        var handed = RasterImage.Open(path);

        Assert.Equal(4, handed.Width);
        Assert.Equal(4, handed.Height);
    }

    [Fact]
    public async Task A_recording_is_answered_as_the_file_it_was_sent()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken);

        var recording = "recorded frames"u8.ToArray();
        using var form = new MultipartFormDataContent();
        var video = new ByteArrayContent(recording);
        video.Headers.ContentType = new MediaTypeHeaderValue("video/mp4");
        form.Add(video, "video", "capture.mp4");

        using var response = await host.Client.PostAsync(
            "/gradio_api/process_recording",
            form,
            cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("video/mp4", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal(
            "inline; filename=\"gradio-screen-recording.mp4\"",
            response.Content.Headers.ContentDisposition?.ToString());

        // Without ffmpeg the capture is what is answered, which is the Python version's own fallback: the
        // trimming the browser asked for is not done rather than the recording being lost.
        Assert.Equal(recording, await response.Content.ReadAsByteArrayAsync(cancellationToken));

        using var noVideo = await host.Client.PostAsync(
            "/gradio_api/process_recording",
            new MultipartFormDataContent(),
            cancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, noVideo.StatusCode);
    }

    /// <summary>Runs a streaming binding and reads the playlist the client was told about.</summary>
    /// <param name="host">The host serving the application.</param>
    /// <param name="apiName">The binding to run.</param>
    /// <param name="session">The session to run it in.</param>
    /// <param name="cancellationToken">Cancels the test.</param>
    /// <returns>The playlist, the stream's path, and the address the client was given.</returns>
    /// <remarks>
    /// The five steps a streamed output takes — join, run, wait for the completion, read the file
    /// object, fetch the playlist — are the same for audio and for video; what differs is what the
    /// playlist says. Keeping the steps in one place is what makes the two tests about their own
    /// media rather than about the protocol twice.
    /// </remarks>
    private static async Task<(string Playlist, string Path, string Url)> PlaylistAsync(
        GradioTestHost host,
        string apiName,
        string session,
        CancellationToken cancellationToken)
    {
        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync(apiName, cancellationToken);
        var eventId = await queue.JoinAsync(fnIndex, [], session, cancellationToken);

        await using var stream = await queue.OpenAsync(session, cancellationToken);
        var completed = await stream.UntilAsync(
            message => QueueSession.Kind(message) == "process_completed"
                && message["event_id"]?.GetValue<string>() == eventId,
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.NotNull(completed);
        Assert.True(completed["success"]!.GetValue<bool>());

        var file = Assert.IsType<JsonObject>(completed["output"]!["data"]!.AsArray()[0]);

        Assert.True(file["is_stream"]!.GetValue<bool>(), "the client is told the value is a stream");

        var url = file["url"]!.GetValue<string>();

        Assert.EndsWith("/playlist.m3u8", url, StringComparison.Ordinal);

        using var response = await host.Client.GetAsync(url, cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(
            "application/vnd.apple.mpegurl",
            response.Content.Headers.ContentType?.MediaType);

        // A playlist nobody signed is not to be cached, and not to be held privately either: it
        // changes while a stream runs, and the signed spelling of it — asserted where the signature
        // is — is the one that is private as well as uncached.
        Assert.True(response.Headers.CacheControl?.NoStore);
        Assert.False(response.Headers.CacheControl?.Private ?? true);

        return (await response.Content.ReadAsStringAsync(cancellationToken), file["path"]!.GetValue<string>(), url);
    }

    /// <summary>An application whose one binding streams video, with the handler given here.</summary>
    /// <param name="blocks">The application being built.</param>
    /// <param name="chunks">What the handler yields.</param>
    private static void AddStreamingVideo(Blocks blocks, Func<IAsyncEnumerable<object?>> chunks)
    {
        using (blocks.BeginRender())
        {
            var video = blocks.Add(new Video { Streaming = true, Label = "Out" });

            video.Change(
                (Func<IAsyncEnumerable<object?>>)(() => chunks()),
                inputs: [],
                outputs: [video],
                apiName: "stream_video");
        }
    }

    /// <summary>The two transport-stream chunks the video handler yields.</summary>
    private static async IAsyncEnumerable<object?> VideoChunks()
    {
        yield return new MediaStreamChunk(FirstPacket, 0.5, ".ts");

        await Task.Yield();

        yield return new MediaStreamChunk(SecondPacket, 1, ".ts");
    }

    /// <summary>An application whose one binding streams audio, with the handler given here.</summary>
    /// <param name="blocks">The application being built.</param>
    /// <param name="chunks">What the handler yields.</param>
    private static void AddStreamingAudio(Blocks blocks, Func<IAsyncEnumerable<object?>> chunks)
    {
        using (blocks.BeginRender())
        {
            var audio = blocks.Add(new Audio { Streaming = true, Label = "Out" });

            audio.Change(
                (Func<IAsyncEnumerable<object?>>)(() => chunks()),
                inputs: [],
                outputs: [audio],
                apiName: "stream_audio");
        }
    }

    /// <summary>The two chunks the streaming handler yields.</summary>
    private static async IAsyncEnumerable<object?> Chunks()
    {
        yield return new MediaStreamChunk(FirstSegment, 0.25, ".aac");

        await Task.Yield();

        yield return new MediaStreamChunk(SecondSegment, 0.5, ".aac");
    }

    /// <summary>An update to the output, then one chunk.</summary>
    private static async IAsyncEnumerable<object?> UpdateThenChunks()
    {
        yield return new Audio { Label = "Updated" };

        await Task.Yield();

        yield return new MediaStreamChunk(FirstSegment, 0.25, ".aac");
    }

    /// <summary>One MPEG-TS packet, which is what a segment of a video stream is made of.</summary>
    private static readonly byte[] FirstPacket = Packet(0x11);

    /// <summary>A second packet, distinguishable from the first by its payload.</summary>
    private static readonly byte[] SecondPacket = Packet(0x22);

    /// <summary>The part of a stream's identity that a segment's name is appended to.</summary>
    /// <param name="playlistPath">The playlist's path, as the stream's file object carries it.</param>
    /// <returns>The path up to and including the last separator.</returns>
    private static string FolderOf(string playlistPath) =>
        playlistPath[..(playlistPath.LastIndexOf('/') + 1)];

    /// <summary>A transport-stream packet: the sync byte every packet starts with, then a payload.</summary>
    /// <param name="payload">The byte that tells one packet from another.</param>
    /// <returns>The packet.</returns>
    private static byte[] Packet(byte payload)
    {
        var packet = new byte[188];
        packet[0] = 0x47;

        for (var index = 1; index < packet.Length; index++)
        {
            packet[index] = payload;
        }

        return packet;
    }

    /// <summary>A JSON body.</summary>
    private static StringContent Json(string body) => new(body, Encoding.UTF8, "application/json");
}

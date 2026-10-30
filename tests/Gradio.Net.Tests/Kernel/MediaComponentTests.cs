using System.Text.Json.Nodes;
using Gradio.Net;
using Gradio.Net.Tests.Server;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using MediaFile = Gradio.Net.File;
namespace Gradio.Net.Tests.Kernel;

/// <summary>
/// A media component as a handler and as the frontend see it: what a value becomes on the way in,
/// and what a handler's result becomes on the way out.
/// </summary>
/// <remarks>
/// The two directions are asserted separately because they are two different claims. On the way in
/// the handler has to receive the shape the Python version's type produces — a path, or the bytes;
/// on the way out the client has to receive a file object whose URL the file endpoint answers. A
/// test that only ran a whole event would show neither of them when it failed.
/// </remarks>
public sealed class MediaComponentTests
{
    [Fact]
    public void A_file_component_hands_the_handler_the_path_of_the_cached_copy()
    {
        using var folder = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        using var elsewhere = new TempFolder();
        using var blocks = RenderFile(provider, folder, out var file);
        var uploaded = elsewhere.Write("notes.txt", "some notes");

        var value = file.Preprocess(new JsonObject
        {
            ["path"] = uploaded,
            ["meta"] = new JsonObject { ["_type"] = FileData.FileMetaType },
            ["orig_name"] = "notes.txt",
        });

        // The path is the cached one, not the one the client sent: a handler's file has to stay
        // readable after the upload directory is cleaned up.
        var path = Assert.IsType<string>(value);
        Assert.NotEqual(uploaded, path);
        Assert.True(MediaCache.IsInFolder(path, folder.Folder));
        Assert.Equal("some notes", System.IO.File.ReadAllText(path));
    }

    [Fact]
    public void A_file_component_of_type_binary_hands_the_handler_the_bytes()
    {
        using var folder = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        using var blocks = RenderFile(provider, folder, out var file, component => component.Type = "binary");
        var uploaded = folder.Write("notes.txt", "some notes");

        var value = file.Preprocess(new JsonObject { ["path"] = uploaded });

        Assert.Equal("some notes"u8.ToArray(), Assert.IsType<byte[]>(value));
    }

    [Fact]
    public void A_file_component_that_takes_several_hands_over_every_path_in_order()
    {
        using var folder = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        using var blocks = RenderFile(
            provider,
            folder,
            out var file,
            component => component.FileCount = "multiple");

        var first = folder.Write("a.txt", "a");
        var second = folder.Write("b.txt", "b");

        var value = file.Preprocess(new JsonArray(
            new JsonObject { ["path"] = first },
            new JsonObject { ["path"] = second }));

        var paths = Assert.IsType<string[]>(value);
        Assert.Equal(["a.txt", "b.txt"], paths.Select(Path.GetFileName));
    }

    [Fact]
    public void A_file_component_sends_a_result_as_a_file_object_the_endpoint_serves()
    {
        using var folder = new TempFolder();
        using var elsewhere = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        using var blocks = RenderFile(provider, folder, out var file);
        var produced = elsewhere.Write("report.csv", "a,b\n1,2\n");

        var value = Assert.IsType<JsonObject>(file.Postprocess(produced));

        // Every field the Python version writes is there, in the Python version's order, and the ones it leaves unset
        // are written as null rather than left out: a client that round-trips the object needs the
        // keys.
        Assert.Equal(
            ["path", "url", "size", "orig_name", "mime_type", "is_stream", "meta"],
            value.Select(property => property.Key));

        var path = value["path"]!.GetValue<string>();
        Assert.True(MediaCache.IsInFolder(path, folder.Folder));
        Assert.Equal(8, value["size"]!.GetValue<long>());
        Assert.Equal("report.csv", value["orig_name"]!.GetValue<string>());
        Assert.Null(value["mime_type"]);
        Assert.False(value["is_stream"]!.GetValue<bool>());
        Assert.Equal(FileData.FileMetaType, value["meta"]!["_type"]!.GetValue<string>());

        // The URL is the one the file endpoint answers, so a browser can fetch what was produced.
        Assert.Equal(MediaCache.UrlOf(path), value["url"]!.GetValue<string>());
    }

    [Fact]
    public void A_file_component_that_takes_several_sends_a_list()
    {
        using var folder = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        using var blocks = RenderFile(
            provider,
            folder,
            out var file,
            component => component.FileCount = "multiple");

        var value = Assert.IsType<JsonArray>(
            file.Postprocess(new[] { folder.Write("a.txt", "a"), folder.Write("b.txt", "b") }));

        Assert.Equal(2, value.Count);
        Assert.Equal("a.txt", value[0]!["orig_name"]!.GetValue<string>());
        Assert.Equal("b.txt", value[1]!["orig_name"]!.GetValue<string>());
    }

    [Fact]
    public void A_file_component_that_was_given_no_value_sends_nothing()
    {
        using var folder = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        using var blocks = RenderFile(
            provider,
            folder,
            out var file,
            component => component.FileCount = "multiple");

        // The Python version answers None for a component that was never given a value, which is why its
        // props carry no `value` key. An empty list would be a component that was given one.
        Assert.Null(file.Postprocess(null));
        Assert.Null(file.Value);
    }

    [Fact]
    public void An_image_component_sends_a_result_as_a_file_object_without_a_size()
    {
        using var folder = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        using var blocks = RenderImage(provider, folder, out var image);
        var produced = folder.Write("photo.png", "not really a png");

        var value = Assert.IsType<JsonObject>(image.Postprocess(produced));

        // The Python version's image postprocess names the file and its origin but not its size; the file
        // endpoint measures the bytes when it serves them.
        Assert.Null(value["size"]);
        Assert.Equal("photo.png", value["orig_name"]!.GetValue<string>());
        Assert.Equal(MediaCache.UrlOf(value["path"]!.GetValue<string>()), value["url"]!.GetValue<string>());
    }

    [Fact]
    public void An_image_component_writes_the_image_it_is_handed_under_its_format()
    {
        using var folder = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        using var blocks = RenderImage(provider, folder, out var image, component => component.Format = "webp");

        // An image a handler made, which is the shape a .NET handler produces: the bytes are the
        // picture, and the component writes it in its own format rather than passing them on.
        var value = Assert.IsType<JsonObject>(image.Postprocess(RasterImage.Open(TestImages.Png(3, 2))));

        var path = value["path"]!.GetValue<string>();

        Assert.Equal("image.webp", Path.GetFileName(path));

        var written = RasterImage.Open(path);

        Assert.Equal(3, written.Width);
        Assert.Equal(2, written.Height);
    }

    [Fact]
    public void An_image_component_reads_a_value_as_the_path_of_the_cached_copy()
    {
        using var folder = new TempFolder();
        using var elsewhere = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        using var blocks = RenderImage(
            provider,
            folder,
            out var image,
            component =>
            {
                // The one shape that hands a handler a path, and the one mode that asks for nothing
                // to be done to the image: the Python version answers with the file it was given.
                component.Type = "filepath";
                component.ImageMode = null;
            });
        var uploaded = elsewhere.Write("cat.png", TestImages.Png());

        var value = image.Preprocess(new JsonObject { ["path"] = uploaded });

        Assert.True(MediaCache.IsInFolder(Assert.IsType<string>(value), folder.Folder));
    }


    [Fact]
    public void A_default_value_is_copied_into_the_cache_and_described_in_the_config()
    {
        using var folder = new TempFolder();
        using var elsewhere = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();
        blocks.UploadFolder = folder.Folder;

        var source = elsewhere.Write("logo.png", "logo");
        Image image;

        using (blocks.BeginRender())
        {
            image = blocks.Add(new Image { Value = source });
        }

        // The value is postprocessed when the block is added, because that is the moment the
        // component meets the application whose cache it copies into.
        var value = Assert.IsType<JsonObject>(image.Value);
        Assert.True(MediaCache.IsInFolder(value["path"]!.GetValue<string>(), folder.Folder));
        Assert.Equal(
            value["path"]!.GetValue<string>(),
            Entry(blocks, image)["props"]!["value"]!["path"]!.GetValue<string>());
    }

    [Fact]
    public async Task A_component_default_value_is_not_reclaimed()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        using var elsewhere = new TempFolder();
        using var directory = new TempFolder();

        var source = elsewhere.Write("kept.png", "kept");
        var stale = directory.Write("stale.txt", age: TimeSpan.FromHours(1));

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            blocks.UploadFolder = folder.Folder;

            // The same policy the temporary-file tests use: often enough to see a sweep inside the
            // test, with an age that takes the file the application merely wrote.
            blocks.DeleteCache = new DeleteCacheSettings(TimeSpan.FromMilliseconds(50), TimeSpan.FromSeconds(1));
            blocks.TrackTempFile(stale);

            using (blocks.BeginRender())
            {
                blocks.Add(new Image { Value = source });
            }
        });

        // The sweep ran, which is what makes the assertion about the other file mean anything.
        Assert.True(
            await Eventually.IsTrueAsync(
                () => Task.FromResult(!System.IO.File.Exists(stale)),
                TimeSpan.FromSeconds(15),
                cancellationToken),
            "the sweep should have deleted the file the application wrote");

        // The component's default value is part of the application rather than a product of a run:
        // deleting it would break the component instead of reclaiming anything.
        Assert.Contains(folder.Files(), file => Path.GetFileName(file) == "kept.png");
    }

    [Fact]
    public void A_file_already_in_the_cache_is_described_with_its_size_and_name()
    {
        using var folder = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        using var blocks = RenderFile(provider, folder, out var file);

        // A handler that returns the path it was given is returning a file the cache already
        // holds. The Python version describes that one like any other, so the client is told how big it is.
        var uploaded = folder.Write("uploaded.txt", "twelve bytes");

        var value = Assert.IsType<JsonObject>(file.Postprocess(uploaded));

        Assert.Equal(12, value["size"]!.GetValue<long>());
        Assert.Equal("uploaded.txt", value["orig_name"]!.GetValue<string>());
        Assert.Equal(MediaCache.UrlOf(uploaded), value["url"]!.GetValue<string>());
    }

    [Fact]
    public void A_value_that_names_a_location_is_sent_as_the_address_it_is()
    {
        using var folder = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        using var blocks = RenderFile(provider, folder, out var file);

        // The server does not hold this file, so the browser fetches it from where it is. Running
        // the address through the file endpoint would ask the server to serve a path it never had.
        var value = Assert.IsType<JsonObject>(file.Postprocess("https://example.com/a.pdf"));

        Assert.Equal("https://example.com/a.pdf", value["url"]!.GetValue<string>());
        Assert.Equal("https://example.com/a.pdf", value["path"]!.GetValue<string>());
        Assert.Empty(folder.Files());
    }

    [Fact]
    public void A_value_read_before_the_render_is_described_again_into_the_application_folder()
    {
        using var folder = new TempFolder();
        using var elsewhere = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();
        blocks.UploadFolder = folder.Folder;

        var source = elsewhere.Write("early.png", "read too soon");
        var image = new Image { Value = source };

        // An application is free to read the value before it renders the component, and doing so
        // copies the file into the process default — a folder the application's file endpoint does
        // not serve. The memory of that read must not outlive the render.
        var early = Assert.IsType<JsonObject>(image.Value);
        Assert.False(MediaCache.IsInFolder(early["path"]!.GetValue<string>(), folder.Folder));

        using (blocks.BeginRender())
        {
            blocks.Add(image);
        }

        var described = Assert.IsType<JsonObject>(image.Value);

        Assert.True(MediaCache.IsInFolder(described["path"]!.GetValue<string>(), folder.Folder));
        System.IO.File.Delete(early["path"]!.GetValue<string>());
    }

    /// <summary>Renders a file component into an application that keeps its files in a folder of the test's own.</summary>
    /// <param name="provider">The container the application is built from.</param>
    /// <param name="folder">The application's upload folder.</param>
    /// <param name="component">The component, as added.</param>
    /// <param name="configure">Adjusts the component before it is added.</param>
    /// <returns>The application, which the caller disposes.</returns>
    private static Blocks RenderFile(
        IServiceProvider provider,
        TempFolder folder,
        out MediaFile component,
        Action<MediaFile>? configure = null)
    {
        var blocks = provider.CreateBlocksWithContainers();
        blocks.UploadFolder = folder.Folder;

        using (blocks.BeginRender())
        {
            var file = new MediaFile();
            configure?.Invoke(file);
            component = blocks.Add(file);
        }

        return blocks;
    }

    /// <summary>
    /// An image hands a handler the shape its type names: the picture, or the path.
    /// </summary>
    /// <remarks>
    /// The Python version's <c>numpy</c> and <c>pil</c> differ in the container they hand over;
    /// here both are a <see cref="RasterImage"/>, and <c>filepath</c> is the path. What the test
    /// pins is the difference that survives the port — whether a handler has to read a file to see
    /// the pixels.
    /// </remarks>
    [Fact]
    public void An_image_hands_a_handler_the_picture_or_its_path()
    {
        using var folder = new TempFolder();
        using var elsewhere = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        var uploaded = elsewhere.Write("cat.png", TestImages.Png(3, 2, red: 255));
        var sent = new JsonObject { ["path"] = uploaded };

        using var blocks = RenderImage(provider, folder, out var decoded, component => component.Type = "numpy");

        var image = Assert.IsType<RasterImage>(decoded.Preprocess(sent.DeepClone()));

        Assert.Equal(3, image.Width);
        Assert.Equal(2, image.Height);

        // The same file through a component that was asked for a path: the handler gets the file.
        using var paths = RenderImage(
            provider,
            folder,
            out var asPath,
            component =>
            {
                component.Type = "filepath";
                component.ImageMode = null;
            });

        Assert.IsType<string>(asPath.Preprocess(sent.DeepClone()));
    }

    /// <summary>
    /// An image that needs nothing done to it is not re-encoded.
    /// </summary>
    /// <remarks>
    /// This is the request path the ticket cares about: a <c>filepath</c> image with no mode asked
    /// for is never decoded and never re-encoded. The file is still copied into the application's
    /// cache — an upload directory is not one a handler may rely on later — so the assertion is on
    /// the bytes, which is what a re-encode would have changed.
    /// </remarks>
    [Fact]
    public void An_image_that_needs_nothing_done_to_it_is_not_re_encoded()
    {
        using var folder = new TempFolder();
        using var elsewhere = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        var uploaded = elsewhere.Write("cat.png", TestImages.Png(3, 2, green: 255));

        // An opaque PNG, through a component that asks for no mode at all.
        using var blocks = RenderImage(
            provider,
            folder,
            out var image,
            component =>
            {
                component.Type = "filepath";
                component.ImageMode = null;
            });

        var cached = Assert.IsType<string>(image.Preprocess(new JsonObject { ["path"] = uploaded }));

        Assert.Equal(
            System.IO.File.ReadAllBytes(uploaded),
            System.IO.File.ReadAllBytes(cached));

        // And through one that asks for RGB, which is the default, when the file already is RGB: a
        // JPEG has no alpha channel, so the Python version answers with the file it was given. The mode is
        // read from the header, which costs no decoding — an RGBA upload would be converted, and
        // that is the case the previous half covers.
        var photo = elsewhere.Write("photo.jpg", RasterImage.Open(TestImages.Png(3, 2, red: 255)).ToBytes("jpg"));

        using var defaults = RenderImage(provider, folder, out var rgb, component => component.Type = "filepath");

        var same = Assert.IsType<string>(rgb.Preprocess(new JsonObject { ["path"] = photo }));

        Assert.Equal(
            System.IO.File.ReadAllBytes(photo),
            System.IO.File.ReadAllBytes(same));
    }

    /// <summary>
    /// An image a handler returned is written under the format it was actually written as.
    /// </summary>
    /// <remarks>
    /// A format this cannot write is written as PNG, which is the Python version's rule; the other
    /// half of the rule is that the file is then called <c>.png</c> rather than after the format
    /// that was asked for, because a name that disagrees with the bytes is a name that lies to
    /// whatever opens it.
    /// </remarks>
    [Fact]
    public void An_image_is_named_after_the_format_it_was_written_in()
    {
        using var folder = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        using var blocks = RenderImage(provider, folder, out var image, component => component.Format = "gif");

        var described = Assert.IsType<JsonObject>(image.Postprocess(RasterImage.Open(TestImages.Png(2, 2))));

        Assert.Equal("image.png", Path.GetFileName(described["path"]!.GetValue<string>()));
    }

    /// <summary>
    /// The image an editor handed a handler is accepted back from it.
    /// </summary>
    /// <remarks>
    /// A handler that answers with the value it was given is the commonest shape there is — an
    /// editor that draws nothing — and it only works if the way out accepts what the way in
    /// produced.
    /// </remarks>
    [Fact]
    public void An_image_editor_takes_back_the_pictures_it_handed_over()
    {
        using var folder = new TempFolder();
        using var elsewhere = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        var background = elsewhere.Write("background.png", TestImages.Png(4, 4));

        using var blocks = RenderImage(provider, folder, out var image, component => component.Format = "png");

        Assert.NotNull(image.Postprocess(RasterImage.Open(TestImages.Png(4, 4))));

        using var editors = Render(provider, folder, () => new ImageEditor(), out var editor, component => component.Format = "png");

        var sent = Assert.IsType<JsonObject>(editor.Postprocess(editor.Preprocess(new JsonObject
        {
            ["background"] = new JsonObject { ["path"] = background },
            ["layers"] = new JsonArray(),
            ["composite"] = new JsonObject { ["path"] = background },
            ["id"] = null,
        })));

        Assert.Equal("image.png", Path.GetFileName(sent["background"]!["path"]!.GetValue<string>()));
    }

    [Fact]
    public void A_gallery_hands_a_handler_an_image_per_entry_and_a_path_for_a_video()
    {
        using var folder = new TempFolder();
        using var elsewhere = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        var picture = elsewhere.Write("cat.png", TestImages.Png(2, 3));
        var clip = elsewhere.Write("clip.mp4", "frames");

        // A gallery hands a handler paths unless it is asked for pictures, which is the Python version's
        // default: its `type` is `filepath`.
        using var blocks = Render(provider, folder, () => new Gallery(), out var gallery, component => component.Type = "numpy");

        var entries = Assert.IsAssignableFrom<IReadOnlyList<GalleryEntry>>(gallery.Preprocess(new JsonArray(
            new JsonObject { ["image"] = new JsonObject { ["path"] = picture }, ["caption"] = "a cat" },
            new JsonObject { ["video"] = new JsonObject { ["path"] = clip } })));

        Assert.Equal(2, entries.Count);

        // An image is the gallery's type — decoded by default — and a video is always a path,
        // because there is no decoded shape for one.
        Assert.Equal(2, Assert.IsType<RasterImage>(entries[0].Media).Width);
        Assert.Equal("a cat", entries[0].Caption);
        Assert.IsType<string>(entries[1].Media);
    }

    [Fact]
    public void An_image_slider_hands_a_handler_a_pair_of_pictures()
    {
        using var folder = new TempFolder();
        using var elsewhere = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        var before = elsewhere.Write("before.png", TestImages.Png(2, 2, red: 255));
        var after = elsewhere.Write("after.png", TestImages.Png(2, 2, blue: 255));

        using var blocks = Render(provider, folder, () => new ImageSlider(), out var slider);

        var pair = Assert.IsAssignableFrom<IReadOnlyList<object?>>(slider.Preprocess(new JsonArray(
            new JsonObject { ["path"] = before },
            new JsonObject { ["path"] = after })));

        Assert.Equal(2, pair.Count);
        Assert.Equal([255, 0, 0], Assert.IsType<RasterImage>(pair[0]).Pixels.Span[..3].ToArray());
        Assert.Equal([0, 0, 255], Assert.IsType<RasterImage>(pair[1]).Pixels.Span[..3].ToArray());
    }

    [Fact]
    public void An_image_editor_hands_a_handler_a_picture_for_each_of_the_three()
    {
        using var folder = new TempFolder();
        using var elsewhere = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        var background = elsewhere.Write("background.png", TestImages.Png(4, 4));
        var layer = elsewhere.Write("layer.png", TestImages.Png(4, 4));

        using var blocks = Render(provider, folder, () => new ImageEditor(), out var editor);

        var value = Assert.IsType<EditorValue>(editor.Preprocess(new JsonObject
        {
            ["background"] = new JsonObject { ["path"] = background },
            ["layers"] = new JsonArray(new JsonObject { ["path"] = layer }),
            ["composite"] = new JsonObject { ["path"] = background },
            ["id"] = null,
        }));

        Assert.Equal(4, Assert.IsType<RasterImage>(value.Background).Width);
        Assert.Equal(4, Assert.IsType<RasterImage>(Assert.Single(value.Layers)).Height);
        Assert.IsType<RasterImage>(value.Composite);
    }

    /// <summary>
    /// A watermark is drawn into the file the browser receives, at the position the application named.
    /// </summary>
    /// <remarks>
    /// The Python version's <c>add_watermark</c> runs inside the postprocess, which is the only
    /// place a watermark is applied: the value a handler returned is written out with the mark
    /// already in its pixels, so the frontend has nothing to overlay.
    /// </remarks>
    [Fact]
    public void An_image_writes_a_watermark_into_what_a_handler_returned()
    {
        using var folder = new TempFolder();
        using var elsewhere = new TempFolder();
        using var provider = KernelHost.BuildProvider();
        var baseImage = elsewhere.Write("base.png", TestImages.Png(40, 40, red: 255));
        var mark = elsewhere.Write("mark.png", TestImages.Png(4, 4, blue: 255));

        using var blocks = RenderImage(
            provider,
            folder,
            out var image,
            component =>
            {
                component.Format = "png";
                component.Watermark = new WatermarkOptions { Watermark = mark, Position = "top-left" };
            });

        var described = Assert.IsType<JsonObject>(image.Postprocess(baseImage));
        var written = RasterImage.Open(described["path"]!.GetValue<string>());

        // Ten pixels in from the corner, which is where the Python version puts a watermark.
        Assert.Equal([0, 0, 255, 255], written.Pixels.Span.Slice((((10 * 40) + 10) * 4), 4).ToArray());

        // And the corner itself is still the base image.
        Assert.Equal([255, 0, 0, 255], written.Pixels.Span[..4].ToArray());
    }

    /// <summary>Renders any component into an application that keeps its files in a folder of the test's own.</summary>
    /// <param name="provider">The container the application is built from.</param>
    /// <param name="folder">The application's upload folder.</param>
    /// <param name="component">The component, as added.</param>
    /// <param name="configure">Adjusts the component before it is added.</param>
    /// <returns>The application, which the caller disposes.</returns>
    private static Blocks Render<TBlock>(
        IServiceProvider provider,
        TempFolder folder,
        Func<TBlock> create,
        out TBlock component,
        Action<TBlock>? configure = null)
        where TBlock : Block
    {
        var blocks = provider.CreateBlocksWithContainers();
        blocks.UploadFolder = folder.Folder;

        using (blocks.BeginRender())
        {
            var added = create();
            configure?.Invoke(added);
            component = blocks.Add(added);
        }

        return blocks;
    }

    /// <summary>Renders an image component into an application that keeps its files in a folder of the test's own.</summary>
    /// <param name="provider">The container the application is built from.</param>
    /// <param name="folder">The application's upload folder.</param>
    /// <param name="component">The component, as added.</param>
    /// <param name="configure">Adjusts the component before it is added.</param>
    /// <returns>The application, which the caller disposes.</returns>
    private static Blocks RenderImage(
        IServiceProvider provider,
        TempFolder folder,
        out Image component,
        Action<Image>? configure = null)
    {
        var blocks = provider.CreateBlocksWithContainers();
        blocks.UploadFolder = folder.Folder;

        using (blocks.BeginRender())
        {
            var image = new Image();
            configure?.Invoke(image);
            component = blocks.Add(image);
        }

        return blocks;
    }

    /// <summary>A block's entry in the served config.</summary>
    /// <param name="blocks">The application.</param>
    /// <param name="block">The block.</param>
    /// <returns>The entry.</returns>
    private static JsonObject Entry(Blocks blocks, Block block) =>
        blocks.GetConfig()["components"]!.AsArray()
            .Single(component => component!["id"]!.GetValue<int>() == block.Id)!
            .AsObject();
}

namespace Gradio.Net;
/// <summary>
/// The image work a component does on the way in and on the way out: the Python version's
/// <c>image_utils</c>.
/// </summary>
/// <remarks>
/// <para>
/// Every image component asks the same two questions — what does a handler get for what the browser
/// sent, and what does the browser get for what a handler returned — and answers them with the same
/// three shapes: <c>numpy</c>, <c>pil</c> and <c>filepath</c>. In C# the first two are one
/// (<see cref="RasterImage"/>, which carries the pixels an array would and the accessors an object
/// would), so what is left to decide is whether a handler is handed the image or its path.
/// </para>
/// <para>
/// The decode happens once. A value that is already a path and needs no work — no mode asked for,
/// no watermark — is passed on as a path without being decoded at all, which is what the Python
/// version does and what keeps a request that only moves a file from paying for pixels it never
/// looks at.
/// </para>
/// <para>
/// It lives in this layer rather than in the kernel because it knows what a component's parameters
/// mean — a watermark, a format, a mode — and the kernel does not. <see cref="RasterImage"/>
/// is the other half: it knows about pixels and nothing about components.
/// </para>
/// </remarks>
internal static class ImagePipeline
{
    /// <summary>What a handler reads for what the browser sent.</summary>
    /// <param name="payload">The file the client sent, or <see langword="null"/> for none.</param>
    /// <param name="shape">How the component wants it.</param>
    /// <returns>A <see cref="RasterImage"/>, a path, or <see langword="null"/>.</returns>
    /// <exception cref="ArgumentException">The payload names no image, or one this cannot use.</exception>
    /// <remarks>
    /// The Python version's <c>preprocess_image</c>. Three things are the Python version's and are
    /// worth naming: a <c>data:</c> URL is decoded rather than fetched, an SVG is only usable as a
    /// path (the decoded shapes are refused, because an SVG is not pixels), and a file that needs
    /// nothing done to it is passed on without being written again.
    /// </remarks>
    internal static object? Preprocess(FileData? payload, ImageShape shape)
    {
        ArgumentNullException.ThrowIfNull(shape);

        if (payload is null)
        {
            return null;
        }

        if (payload.Url is { Length: > 0 } url && url.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
        {
            var inline = RasterImage.FromBase64(url);

            return shape.WantsImage
                ? inline
                : Write(inline, shape.Folder, "image", shape.Format, shape.Root);
        }

        if (payload.Path is not { Length: > 0 } path)
        {
            throw new ArgumentException("Image path is None.", nameof(payload));
        }

        var (name, extension) = NameAndExtension(payload);

        if (extension == "svg")
        {
            return shape.Type == "filepath"
                ? MediaFiles.Cached(path, shape.Folder, shape.Root)
                : throw new ArgumentException(
                    "SVG files are not supported as input images for this app.",
                    nameof(payload));
        }

        var cached = MediaFiles.Cached(path, shape.Folder, shape.Root);

        // A path with nothing to do to it is a path. It is still copied into the cache — an upload
        // directory is not one a handler may rely on later — but it is never decoded and never
        // re-encoded, which is the cost this saves. The Python version's condition is `image_mode in [None,
        // im.mode]`, so a file already in the mode the component asks for takes this path too: the
        // mode is read from the header, which costs no decoding.
        if (shape.Type == "filepath"
            && (shape.Mode is null || string.Equals(shape.Mode, RasterImage.ModeOf(cached), StringComparison.Ordinal)))
        {
            return cached;
        }

        var image = RasterImage.Open(cached);

        if (shape.Mode is { Length: > 0 } mode)
        {
            image = image.InMode(mode);
        }

        return shape.WantsImage
            ? image
            : Write(image, shape.Folder, name, extension, shape.Root);
    }

    /// <summary>What the browser receives for what a handler returned.</summary>
    /// <param name="value">A path, a URL, a <see cref="RasterImage"/>, bytes, a stream, or a file object.</param>
    /// <param name="shape">How the component wants it.</param>
    /// <param name="watermark">A watermark to draw, or <see langword="null"/> for none.</param>
    /// <returns>The image as the protocol carries it, or <see langword="null"/> for none.</returns>
    /// <exception cref="NotSupportedException">The value is of a shape no image component accepts.</exception>
    /// <remarks>
    /// The Python version's <c>postprocess_image</c>. A path is described rather than decoded —
    /// unless a watermark has to be drawn, which is the one thing that needs the pixels — and an
    /// SVG is sent as its own text, because it is not something this can decode and re-encode.
    /// </remarks>
    internal static FileData? Postprocess(object? value, ImageShape shape, WatermarkOptions? watermark = null)
    {
        ArgumentNullException.ThrowIfNull(shape);

        if (value is null)
        {
            return null;
        }

        if (PathOf(value) is { } named && named.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
        {
            return Svg(named, shape);
        }

        var mark = watermark?.Watermark is { Length: > 0 } watermarkPath
            ? (RasterImage.Open(watermarkPath), watermark.Position)
            : ((RasterImage Image, string Position)?)null;

        if (mark is null && PathOf(value) is { Length: > 0 } untouched)
        {
            // Nothing to draw: the file the handler named is the file the browser is given. It is
            // copied into the cache like any other file and never decoded. What the component's type
            // says is about the way in, not the way out — the Python version's postprocess does not consult it
            // either.
            return MediaFiles.Describe(new FileData { Path = untouched }, shape.Folder, shape.Root, measure: false);
        }

        var image = ImageOf(value);

        if (image is null)
        {
            return null;
        }

        if (mark is { } drawing)
        {
            image = image.Watermark(drawing.Image, drawing.Position);
        }

        return MediaFiles.Describe(
            new FileData { Path = Write(image, shape.Folder, "image", shape.Format, shape.Root) },
            shape.Folder,
            shape.Root,
            measure: false);
    }

    /// <summary>A value read as an image, whatever shape it arrived in.</summary>
    /// <param name="value">The value.</param>
    /// <returns>The image, or <see langword="null"/> when the value names a location this cannot read.</returns>
    /// <exception cref="NotSupportedException">The value is of a shape no image component accepts.</exception>
    private static RasterImage? ImageOf(object? value) => value switch
    {
        null => null,
        RasterImage image => image,
        FileData file => FromPath(file.Path),
        System.Text.Json.Nodes.JsonNode node => FromPath(FileData.FromJson(node)?.Path),
        byte[] bytes => RasterImage.Open(bytes),
        Stream stream => OpenStream(stream),
        string text => FromPath(text),
        _ => throw new NotSupportedException(
            "An image component takes a path, a URL, an image, its bytes, a stream, or a file "
            + $"object; it was handed a {value.GetType()}."),
    };

    /// <summary>A path read as an image, or nothing when it is a location the server does not hold.</summary>
    /// <param name="path">The path, or <see langword="null"/>.</param>
    /// <returns>The image, or <see langword="null"/> for a URL.</returns>
    /// <remarks>
    /// A URL is passed through rather than fetched: the Python version's postprocess does not
    /// download what a handler returned, and a server that fetched an address a handler named would
    /// be a server that makes requests on an application's behalf.
    /// </remarks>
    private static RasterImage? FromPath(string? path) =>
        path is { Length: > 0 } && !MediaCache.NamesALocation(path) ? RasterImage.Open(path) : null;

    /// <summary>A stream read as an image, and left open.</summary>
    /// <param name="stream">The stream.</param>
    private static RasterImage OpenStream(Stream stream)
    {
        using var buffer = new MemoryStream();

        stream.CopyTo(buffer);

        return RasterImage.Open(buffer.ToArray());
    }

    /// <summary>The path a value names, or <see langword="null"/> when it names none.</summary>
    /// <param name="value">The value.</param>
    private static string? PathOf(object? value) => value switch
    {
        string text => text,
        FileData file => file.Path,
        System.Text.Json.Nodes.JsonNode node => FileData.FromJson(node)?.Path,
        _ => null,
    };

    /// <summary>An image written into the application's cache.</summary>
    /// <param name="image">The image.</param>
    /// <param name="folder">The cache folder.</param>
    /// <param name="name">The name to write under, without an extension.</param>
    /// <param name="format">The format's extension.</param>
    /// <param name="root">The application, so that the file can be reclaimed.</param>
    /// <returns>The path of the file that was written.</returns>
    private static string Write(RasterImage image, string folder, string name, string format, Blocks? root)
    {
        var path = image.Save(folder, name, format);
        root?.TrackTempFile(path);

        return path;
    }

    /// <summary>An SVG sent as its own text, which is the one image the frontend draws undecoded.</summary>
    /// <param name="path">The SVG's path.</param>
    /// <param name="shape">How the component wants its images.</param>
    /// <returns>The file, carrying the picture as text rather than as a location.</returns>
    /// <remarks>
    /// The Python version answers an SVG with its own source in a <c>data:</c> URL and no path at
    /// all, and this does the same: the file is read into the cache first, so what the browser is
    /// given is the picture rather than an address on the server's disk. An SVG the server does not
    /// hold — a URL a handler returned — is not something this can read, and it is described as the
    /// address it is.
    /// </remarks>
    private static FileData Svg(string path, ImageShape shape)
    {
        if (MediaCache.NamesALocation(path))
        {
            return MediaFiles.Describe(new FileData { Path = path }, shape.Folder, shape.Root, measure: false);
        }

        var cached = MediaFiles.Cached(path, shape.Folder, shape.Root);
        var content = System.IO.File.ReadAllText(cached);

        return new FileData
        {
            OrigName = Path.GetFileName(cached),
            Url = "data:image/svg+xml," + Uri.EscapeDataString(content),
        };
    }

    /// <summary>The name and extension a payload should be written under.</summary>
    /// <param name="payload">The file the client sent.</param>
    /// <returns>
    /// The name without its extension, and the extension without its dot — <c>jpeg</c> for a
    /// <c>.jpg</c>, and <c>image</c>/<c>webp</c> for a file that arrived without a name.
    /// </returns>
    /// <remarks>
    /// The Python version's rule, and its reason: the name is what the cache file is called, and a
    /// name that came from the browser is a name the browser chose.
    /// </remarks>
    private static (string Name, string Extension) NameAndExtension(FileData payload)
    {
        if (payload.OrigName is not { Length: > 0 } original)
        {
            return ("image", "webp");
        }

        var extension = Path.GetExtension(original).TrimStart('.').ToLowerInvariant();

        if (extension is "jpg" or "jpeg")
        {
            extension = "jpeg";
        }

        return (Path.GetFileNameWithoutExtension(original), extension.Length > 0 ? extension : "webp");
    }
}

using System.Collections.Frozen;
using SkiaSharp;

namespace Gradio.Net;
/// <summary>
/// A decoded image: how big it is, and what its pixels are.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's image layer is PIL plus numpy, and the two shapes it hands a handler — a
/// <c>PIL.Image</c> and a numpy array — are the same picture in two containers. This is the one
/// container C# needs: the size, and the pixels as bytes, which is the array shape, with the
/// accessors an object gives. A component that declares <c>numpy</c> or <c>pil</c> hands one of
/// these over; one that declares <c>filepath</c> hands a path, as the Python version does.
/// </para>
/// <para>
/// The pixels are RGBA, four bytes a pixel, row by row from the top — whatever the file held. A
/// decoder may hand back three channels or one; normalising here is what makes a handler's code the
/// same for every file it is given, and what lets <see cref="InMode"/> answer for a picture whose
/// file was in another mode.
/// </para>
/// <para>
/// The decoder is SkiaSharp, and it is not visible from here: this type copies the pixels
/// out and creates one again to write, so nothing that holds a <see cref="RasterImage"/> holds native
/// memory or has to dispose anything.
/// </para>
/// </remarks>
public sealed class RasterImage
{
    /// <summary>The bytes a pixel takes: red, green, blue, alpha.</summary>
    private const int Channels = 4;

    /// <summary>The formats this can write, by the extension an application names them with.</summary>
    /// <remarks>
    /// The Python version asks PIL, which writes more of them than this does; the ones missing fall
    /// back to PNG where the Python version would have written them, which is the Python version's
    /// own rule for a format it cannot save (see <see cref="ToBytes"/>).
    /// </remarks>
    private static readonly FrozenDictionary<string, SKEncodedImageFormat> _encodable =
        new Dictionary<string, SKEncodedImageFormat>(StringComparer.OrdinalIgnoreCase)
        {
            ["png"] = SKEncodedImageFormat.Png,
            ["jpg"] = SKEncodedImageFormat.Jpeg,
            ["jpeg"] = SKEncodedImageFormat.Jpeg,
            ["webp"] = SKEncodedImageFormat.Webp,
        }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

    private readonly byte[] _pixels;

    /// <summary>Creates an image from pixels this framework holds.</summary>
    /// <param name="width">How many pixels wide.</param>
    /// <param name="height">How many pixels tall.</param>
    /// <param name="rgba">The pixels, four bytes each, row by row.</param>
    /// <exception cref="ArgumentOutOfRangeException">A dimension is not positive.</exception>
    /// <exception cref="ArgumentException">The pixels are not four bytes for every pixel.</exception>
    public RasterImage(int width, int height, ReadOnlySpan<byte> rgba)
    {
        if (width <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width), width, "An image is at least one pixel wide.");
        }

        if (height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(height), height, "An image is at least one pixel tall.");
        }

        if (rgba.Length != width * height * Channels)
        {
            throw new ArgumentException(
                $"An image of {width}x{height} is {width * height * Channels} bytes of RGBA; "
                + $"got {rgba.Length}.",
                nameof(rgba));
        }

        Width = width;
        Height = height;
        _pixels = rgba.ToArray();
    }

    /// <summary>How many pixels wide the image is.</summary>
    public int Width { get; }

    /// <summary>How many pixels tall the image is.</summary>
    public int Height { get; }

    /// <summary>The pixels, four bytes each — red, green, blue, alpha — row by row from the top.</summary>
    public ReadOnlyMemory<byte> Pixels => _pixels;

    /// <summary>Reads an image from a file.</summary>
    /// <param name="path">The file's path.</param>
    /// <returns>The decoded image.</returns>
    /// <exception cref="IOException">The file cannot be read.</exception>
    /// <exception cref="ArgumentException">The file is not an image this can decode.</exception>
    public static RasterImage Open(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        return Decode(System.IO.File.ReadAllBytes(path), path);
    }

    /// <summary>Reads an image from its bytes.</summary>
    /// <param name="bytes">The encoded image.</param>
    /// <returns>The decoded image.</returns>
    /// <exception cref="ArgumentException">The bytes are not an image this can decode.</exception>
    public static RasterImage Open(ReadOnlySpan<byte> bytes) => Decode(bytes.ToArray(), name: null);

    /// <summary>Reads an image from a <c>data:</c> URL.</summary>
    /// <param name="text">The URL, as a browser sends one.</param>
    /// <returns>The decoded image.</returns>
    /// <exception cref="ArgumentException">The text is not a base64 image URL, or not an image.</exception>
    public static RasterImage FromBase64(string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        var comma = text.IndexOf(',', StringComparison.Ordinal);

        if (!text.StartsWith("data:", StringComparison.OrdinalIgnoreCase)
            || comma < 0
            || !text[..comma].Contains(";base64", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("A base64 image is spelled 'data:<type>;base64,<bytes>'.", nameof(text));
        }

        byte[] bytes;

        try
        {
            bytes = Convert.FromBase64String(text[(comma + 1)..]);
        }
        catch (FormatException error)
        {
            throw new ArgumentException("The base64 payload is not valid base64.", nameof(text), error);
        }

        return Decode(bytes, name: null);
    }

    /// <summary>Writes the image in a format, and answers with the bytes.</summary>
    /// <param name="format">The format's extension: <c>png</c>, <c>jpg</c> or <c>webp</c>.</param>
    /// <returns>The encoded image.</returns>
    /// <remarks>
    /// A format this cannot write is written as PNG rather than refused, which is what the Python
    /// version does when PIL cannot save one: the answer is an image either way, and a file named
    /// for a format it is not is a smaller surprise than a run that fails.
    /// </remarks>
    public byte[] ToBytes(string format)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(format);

        var encoder = _encodable[Writable(format)];

        using var bitmap = ToBitmap();

        // WebP at 90 and JPEG at 90 are what a cache image is written at: high enough that a
        // round trip through the cache does not show, low enough that the file is not the raw pixels.
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(encoder, Quality);

        return data?.ToArray() ?? throw new InvalidOperationException(
            $"The image could not be written as {format}.");
    }

    /// <summary>Writes the image into a folder, and answers with its path.</summary>
    /// <param name="folder">The folder to write into.</param>
    /// <param name="name">The file's name, without an extension.</param>
    /// <param name="format">The format's extension.</param>
    /// <returns>The path of the file that was written.</returns>
    /// <exception cref="IOException">The file cannot be written.</exception>
    /// <remarks>
    /// Written through the media cache, because a folder an application serves files from is one
    /// where every file is addressed by the hash of its content: two images with the same bytes are
    /// one file, and a name cannot be guessed into a collision with another run's.
    /// </remarks>
    public string Save(string folder, string name, string format)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folder);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(format);

        // The name follows the bytes: a format this cannot write is written as PNG, and a file
        // called `.gif` that holds a PNG is a name that lies about what the browser will get.
        var written = Writable(format);

        return MediaCache.SaveBytes(ToBytes(written), $"{name}.{written}", folder);
    }

    /// <summary>The image as a <c>data:</c> URL the frontend can draw.</summary>
    /// <param name="format">The format's extension.</param>
    /// <returns>The URL.</returns>
    public string ToBase64(string format)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(format);

        var extension = format.TrimStart('.').ToLowerInvariant();
        var media = extension switch
        {
            "jpg" or "jpeg" => "image/jpeg",
            "webp" => "image/webp",
            _ => "image/png",
        };

        return $"data:{media};base64,{Convert.ToBase64String(ToBytes(extension))}";
    }

    /// <summary>Draws another image over this one, at a corner or at a pixel position.</summary>
    /// <param name="watermark">The image to draw.</param>
    /// <param name="position">A named corner, or <c>x,y</c> in pixels.</param>
    /// <returns>The image with the watermark drawn on it.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="watermark"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="position"/> is not a corner or a pair of numbers.</exception>
    /// <remarks>
    /// The Python version's <c>add_watermark</c>: ten pixels from the edge it was asked for, drawn
    /// with the watermark's own alpha as its mask, and moved to the bottom right when the position
    /// asked for does not fit. That last rule is the Python version's, and it is why a position is
    /// not refused for being out of bounds.
    /// </remarks>
    public RasterImage Watermark(RasterImage watermark, string position)
    {
        ArgumentNullException.ThrowIfNull(watermark);
        ArgumentException.ThrowIfNullOrWhiteSpace(position);

        const int padding = 10;

        var (x, y) = position switch
        {
            "top-left" => (padding, padding),
            "top-right" => (Width - watermark.Width - padding, padding),
            "bottom-left" => (padding, Height - watermark.Height - padding),
            "bottom-right" => (Width - watermark.Width - padding, Height - watermark.Height - padding),
            _ => PixelPosition(position),
        };

        if (x < 0 || x + watermark.Width > Width || y < 0 || y + watermark.Height > Height)
        {
            x = Width - watermark.Width - padding;
            y = Height - watermark.Height - padding;
        }

        var canvas = new byte[_pixels.Length];
        _pixels.CopyTo(canvas, 0);

        for (var row = 0; row < watermark.Height; row++)
        {
            var target = ((y + row) * Width) + x;

            // A watermark placed so that part of it falls outside is drawn up to the edge, which is
            // what a paste does: the pixels that land outside the canvas are dropped.
            if (y + row < 0 || y + row >= Height)
            {
                continue;
            }

            for (var column = 0; column < watermark.Width; column++)
            {
                if (x + column < 0 || x + column >= Width)
                {
                    continue;
                }

                var source = ((row * watermark.Width) + column) * Channels;
                var destination = (target + column) * Channels;

                Blend(watermark._pixels.AsSpan(source, Channels), canvas.AsSpan(destination, Channels));
            }
        }

        return new RasterImage(Width, Height, canvas);
    }

    /// <summary>The image scaled to a size, keeping its pixels.</summary>
    /// <param name="width">How many pixels wide the answer should be.</param>
    /// <param name="height">How many pixels tall the answer should be.</param>
    /// <returns>The scaled image, or this one when it is already that size.</returns>
    /// <exception cref="ArgumentOutOfRangeException">A dimension is not positive.</exception>
    /// <exception cref="InvalidOperationException">The decoder could not produce the scaled pixels.</exception>
    /// <remarks>
    /// The Python version's <c>PIL.Image.resize</c>, which is what the icon endpoint scales with.
    /// The filter is the one PIL picks when it is not told: a cubic resampler with the <c>a =
    /// -0.5</c> kernel that is Catmull-Rom's, which is the shape SkiaSharp offers under that name.
    /// The pixels are not promised to be byte for byte PIL's — two resamplers agree on the picture
    /// and not on the last bit of every channel — so what is promised is the size and that it is
    /// the same picture.
    /// </remarks>
    public RasterImage Resize(int width, int height)
    {
        // Checked here rather than left to the constructor, which checks the same thing: the bitmap
        // this allocates next is native memory sized by these two numbers, so the refusal has to
        // come before the allocation rather than after it.
        if (width <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width), width, "An image is at least one pixel wide.");
        }

        if (height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(height), height, "An image is at least one pixel tall.");
        }

        if (width == Width && height == Height)
        {
            return this;
        }

        using var source = ToBitmap();
        using var target = new SKBitmap(new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Unpremul));

        if (!source.ScalePixels(target.PeekPixels(), new SKSamplingOptions(SKCubicResampler.CatmullRom)))
        {
            throw new InvalidOperationException($"The image could not be scaled to {width}x{height}.");
        }

        return new RasterImage(width, height, target.GetPixelSpan());
    }

    /// <summary>The image as one of the modes a component's <c>image_mode</c> names.</summary>
    /// <param name="mode">The mode: <c>RGB</c>, <c>RGBA</c> or <c>L</c>.</param>
    /// <returns>The image in that mode, still four bytes a pixel.</returns>
    /// <exception cref="ArgumentException">The mode is not one this can produce.</exception>
    /// <remarks>
    /// A mode changes the pixels rather than their container: <c>L</c> is grey — every channel the
    /// luminance, the alpha kept — and <c>RGB</c> is the same picture with its alpha made opaque.
    /// The other modes PIL has are not here, and saying so is better than answering with pixels that
    /// are not what was asked for.
    /// </remarks>
    public RasterImage InMode(string mode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mode);

        switch (mode)
        {
            case "RGBA":
                return this;

            case "RGB":
            {
                var pixels = _pixels.ToArray();

                for (var index = 0; index < pixels.Length; index += Channels)
                {
                    pixels[index + 3] = 255;
                }

                return new RasterImage(Width, Height, pixels);
            }

            case "L":
            {
                var pixels = new byte[_pixels.Length];

                for (var index = 0; index < _pixels.Length; index += Channels)
                {
                    // PIL's "L" is the ITU-R 601-2 luma in the integer form PIL uses: the three
                    // coefficients scaled to sixteen bits, the sum rounded by adding a half before
                    // the shift. Written out rather than as 0.299/0.587/0.114 so that the grey is
                    // the number PIL would have produced.
                    var grey = (byte)(
                        ((_pixels[index] * 19595)
                            + (_pixels[index + 1] * 38470)
                            + (_pixels[index + 2] * 7471)
                            + 32768) >> 16);

                    pixels[index] = grey;
                    pixels[index + 1] = grey;
                    pixels[index + 2] = grey;
                    pixels[index + 3] = _pixels[index + 3];
                }

                return new RasterImage(Width, Height, pixels);
            }

            default:
                throw new ArgumentException(
                    $"An image mode is one of RGB, RGBA or L here; got '{mode}'. The Python version's other "
                    + "modes are refused rather than answered with pixels that are not in them.",
                    nameof(mode));
        }
    }

    /// <summary>The quality an encoded image is written at.</summary>
    private const int Quality = 90;

    /// <summary>The extension this writes a format as, which is PNG for one it cannot write.</summary>
    /// <param name="format">The format's extension, as the application named it.</param>
    /// <returns>The extension the bytes will have.</returns>
    /// <remarks>
    /// The Python version's rule for a format PIL cannot save: fall back to PNG. What it does with
    /// the name is the other half of the rule — the file it writes is called <c>.png</c> — and this
    /// is where both halves are decided, so that the bytes and the name cannot disagree.
    /// </remarks>
    private static string Writable(string format)
    {
        var extension = format.TrimStart('.').ToLowerInvariant();

        return extension switch
        {
            "png" => "png",
            "jpg" or "jpeg" => "jpg",
            "webp" => "webp",
            _ => "png",
        };
    }

    /// <summary>The mode a file's own header declares, or <see langword="null"/> when unreadable.</summary>
    /// <param name="path">The file's path.</param>
    /// <returns><c>RGB</c>, <c>RGBA</c>, <c>L</c>, or <see langword="null"/>.</returns>
    /// <remarks>
    /// Read from the header rather than from the pixels, which is what makes it cheap: the Python
    /// version asks <c>PIL.Image.open</c> the same question and gets an answer without decoding
    /// anything, and a component asks it to decide whether a file needs converting at all.
    /// </remarks>
    internal static string? ModeOf(string path)
    {
        try
        {
            using var data = SKData.Create(path);
            using var codec = data is null ? null : SKCodec.Create(data);

            if (codec is null)
            {
                return null;
            }

            var info = codec.Info;

            if (info.ColorType is SKColorType.Gray8)
            {
                return "L";
            }

            return info.AlphaType is SKAlphaType.Opaque or SKAlphaType.Unknown ? "RGB" : "RGBA";
        }
        catch (IOException)
        {
            return null;
        }
    }

    /// <summary>A position written as a pair of pixels.</summary>
    /// <param name="position">The text, as <c>x,y</c>.</param>
    /// <returns>The two numbers.</returns>
    /// <exception cref="ArgumentException">The text is not a pair of whole numbers.</exception>
    private static (int X, int Y) PixelPosition(string position)
    {
        var parts = position.Split(',', StringSplitOptions.TrimEntries);

        if (parts.Length == 2
            && int.TryParse(parts[0], System.Globalization.CultureInfo.InvariantCulture, out var x)
            && int.TryParse(parts[1], System.Globalization.CultureInfo.InvariantCulture, out var y))
        {
            return (x, y);
        }

        throw new ArgumentException(
            "A watermark's position is 'top-left', 'top-right', 'bottom-left', 'bottom-right' or "
            + $"'x,y'; got '{position}'.",
            nameof(position));
    }

    /// <summary>Draws one pixel of a watermark over another, the way a paste with a mask does.</summary>
    /// <param name="source">The watermark's pixel.</param>
    /// <param name="destination">The pixel it is drawn over.</param>
    private static void Blend(ReadOnlySpan<byte> source, Span<byte> destination)
    {
        var alpha = source[3];

        if (alpha == 0)
        {
            return;
        }

        if (alpha == 255)
        {
            source.CopyTo(destination);

            return;
        }

        // Source-over, which is what a paste with a mask is: every channel — the alpha too — is the
        // mark's weighted by its alpha plus what was there weighted by what is left. The alpha
        // channel is why it is written out rather than approximated: a half-transparent mark over an
        // opaque image ends up less opaque than the mark alone, and only this arithmetic says so.
        var kept = 255 - alpha;

        destination[0] = (byte)(((source[0] * alpha) + (destination[0] * kept)) / 255);
        destination[1] = (byte)(((source[1] * alpha) + (destination[1] * kept)) / 255);
        destination[2] = (byte)(((source[2] * alpha) + (destination[2] * kept)) / 255);
        destination[3] = (byte)(((alpha * alpha) + (destination[3] * kept)) / 255);
    }

    /// <summary>Decodes bytes into an image, or says what they are not.</summary>
    /// <param name="bytes">The encoded image.</param>
    /// <param name="name">The file's name, for the message, or <see langword="null"/> for bytes.</param>
    /// <exception cref="ArgumentException">The bytes are not an image this can decode.</exception>
    private static RasterImage Decode(byte[] bytes, string? name)
    {
        using var data = SKData.CreateCopy(bytes);
        using var codec = SKCodec.Create(data)
            ?? throw new ArgumentException(
                $"{(name is null ? "The bytes" : $"'{Path.GetFileName(name)}'")} are not an image this "
                + "can decode. It reads what the frontend offers — PNG, JPEG, WebP, GIF and BMP — and "
                + "the other formats the decoder knows.",
                nameof(bytes));

        using var bitmap = new SKBitmap(
            new SKImageInfo(codec.Info.Width, codec.Info.Height, SKColorType.Rgba8888, SKAlphaType.Unpremul));

        if (codec.GetPixels(bitmap.Info, bitmap.GetPixels()) is not (SKCodecResult.Success or SKCodecResult.IncompleteInput))
        {
            throw new ArgumentException(
                $"{(name is null ? "The image" : $"'{Path.GetFileName(name)}'")} could not be decoded.",
                nameof(bytes));
        }

        return new RasterImage(bitmap.Width, bitmap.Height, bitmap.GetPixelSpan());
    }

    /// <summary>The image as a bitmap the encoder can write.</summary>
    private SKBitmap ToBitmap()
    {
        var bitmap = new SKBitmap(
            new SKImageInfo(Width, Height, SKColorType.Rgba8888, SKAlphaType.Unpremul));

        System.Runtime.InteropServices.Marshal.Copy(_pixels, 0, bitmap.GetPixels(), _pixels.Length);

        return bitmap;
    }
}

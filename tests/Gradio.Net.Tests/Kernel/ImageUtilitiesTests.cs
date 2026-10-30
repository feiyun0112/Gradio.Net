using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Kernel;

/// <summary>
/// The image layer as an application and a handler see it: what an image is once it is decoded, and
/// what the utilities do with one.
/// </summary>
/// <remarks>
/// <para>
/// This is the auxiliary seam the spec allows — a pure computation with no HTTP in it, called the way
/// an application calls it. What is asserted is the part a component cannot show: that a picture
/// survives a trip through a format, that a watermark lands where it was asked for, and that a mode
/// is the pixels it names rather than a label.
/// </para>
/// <para>
/// The images are built here rather than checked in as files: a test that reads a picture from disk
/// is a test that depends on the decoder having written it, and one that builds its own pixels knows
/// exactly what it should get back.
/// </para>
/// </remarks>
public sealed class ImageUtilitiesTests
{
    /// <summary>A solid colour, as the four bytes of one pixel.</summary>
    private static readonly byte[] _red = [255, 0, 0, 255];

    [Fact]
    public void An_image_survives_a_trip_through_every_format_it_writes()
    {
        var image = Solid(3, 2, _red);

        foreach (var format in new[] { "png", "jpg", "webp" })
        {
            var read = RasterImage.Open(image.ToBytes(format));

            Assert.Equal(3, read.Width);
            Assert.Equal(2, read.Height);

            // JPEG is lossy and has no alpha, so the red comes back close rather than exact; the
            // point of the assertion is that the picture is the picture, not that the codec is
            // lossless.
            var pixel = read.Pixels.Span[..4].ToArray();

            Assert.InRange(pixel[0], 200, 255);
            Assert.InRange(pixel[1], 0, 60);
            Assert.InRange(pixel[2], 0, 60);
        }
    }

    [Fact]
    public void An_image_is_carried_as_a_data_url()
    {
        var image = Solid(2, 2, [0, 128, 255, 255]);

        var url = image.ToBase64("png");

        Assert.StartsWith("data:image/png;base64,", url, StringComparison.Ordinal);

        var read = RasterImage.FromBase64(url);

        Assert.Equal(2, read.Width);
        Assert.Equal([0, 128, 255, 255], read.Pixels.Span[..4].ToArray());
    }

    [Fact]
    public void A_data_url_that_is_not_one_is_refused()
    {
        Assert.Throws<ArgumentException>(() => RasterImage.FromBase64("https://example.com/cat.png"));
        Assert.Throws<ArgumentException>(() => RasterImage.FromBase64("data:image/png;base64,not base64!"));
        Assert.Throws<ArgumentException>(() => RasterImage.FromBase64("data:image/png;base64,aGVsbG8="));
    }

    [Fact]
    public void A_watermark_is_drawn_ten_pixels_from_the_corner_it_was_asked_for()
    {
        var baseImage = Solid(20, 20, [255, 255, 255, 255]);
        var mark = Solid(4, 4, [0, 0, 255, 255]);

        var marked = baseImage.Watermark(mark, "top-left");

        Assert.Equal([0, 0, 255, 255], PixelAt(marked, 10, 10));

        // Ten pixels in, so the corner itself is still the base image.
        Assert.Equal([255, 255, 255, 255], PixelAt(marked, 9, 9));
        Assert.Equal([255, 255, 255, 255], PixelAt(marked, 14, 14));

        var bottomRight = baseImage.Watermark(mark, "bottom-right");

        Assert.Equal([0, 0, 255, 255], PixelAt(bottomRight, 20 - 4 - 10, 20 - 4 - 10));
    }

    [Fact]
    public void A_watermark_that_does_not_fit_is_moved_to_the_bottom_right()
    {
        var baseImage = Solid(20, 20, [255, 255, 255, 255]);
        var mark = Solid(4, 4, [0, 255, 0, 255]);

        // A position of 18,18 puts four pixels of watermark past the edge, and the Python version moves it
        // rather than refusing it.
        var marked = baseImage.Watermark(mark, "18,18");

        Assert.Equal([0, 255, 0, 255], PixelAt(marked, 6, 6));
    }

    [Fact]
    public void A_position_that_is_neither_a_corner_nor_a_pair_is_refused()
    {
        var image = Solid(4, 4, _red);

        Assert.Throws<ArgumentException>(() => image.Watermark(Solid(1, 1, _red), "middle"));
        Assert.Throws<ArgumentException>(() => image.Watermark(Solid(1, 1, _red), "1,2,3"));
    }

    [Fact]
    public void A_mode_is_the_pixels_it_names()
    {
        var image = Solid(1, 1, [10, 20, 30, 40]);

        Assert.Equal([10, 20, 30, 40], image.InMode("RGBA").Pixels.Span[..4].ToArray());

        // RGB is the same picture with its alpha made opaque.
        Assert.Equal([10, 20, 30, 255], image.InMode("RGB").Pixels.Span[..4].ToArray());

        // L is grey: every channel the luminance, the alpha kept.
        var grey = image.InMode("L").Pixels.Span[..4].ToArray();

        Assert.Equal(grey[0], grey[1]);
        Assert.Equal(grey[0], grey[2]);
        Assert.Equal(40, grey[3]);

        Assert.Throws<ArgumentException>(() => image.InMode("CMYK"));
    }

    [Fact]
    public void A_format_that_cannot_be_written_is_written_as_png()
    {
        var image = Solid(1, 1, _red);

        // The Python version falls back to PNG when PIL cannot save the format it was asked for; this writes
        // fewer formats than PIL, so the fallback is the rule that makes both answer with an image.
        Assert.Equal([0x89, 0x50, 0x4E, 0x47], image.ToBytes("tiff").Take(4));
    }

    [Fact]
    public void Something_that_is_not_an_image_is_refused()
    {
        Assert.Throws<ArgumentException>(() => RasterImage.Open("not an image"u8));
    }

    [Fact]
    public void Pixels_that_do_not_fill_the_size_are_refused()
    {
        Assert.Throws<ArgumentException>(() => new RasterImage(2, 2, new byte[4]));
        Assert.Throws<ArgumentOutOfRangeException>(() => new RasterImage(0, 2, []));
    }

    /// <summary>An image of one colour.</summary>
    /// <param name="width">How wide.</param>
    /// <param name="height">How tall.</param>
    /// <param name="rgba">The colour, as one pixel.</param>
    private static RasterImage Solid(int width, int height, byte[] rgba)
    {
        var pixels = new byte[width * height * 4];

        for (var index = 0; index < pixels.Length; index += 4)
        {
            rgba.CopyTo(pixels, index);
        }

        return new RasterImage(width, height, pixels);
    }

    /// <summary>One pixel of an image.</summary>
    /// <param name="image">The image.</param>
    /// <param name="x">The column.</param>
    /// <param name="y">The row.</param>
    private static byte[] PixelAt(RasterImage image, int x, int y)
    {
        var start = ((y * image.Width) + x) * 4;

        return image.Pixels.Span.Slice(start, 4).ToArray();
    }
}

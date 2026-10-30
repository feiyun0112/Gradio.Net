using Gradio.Net;
namespace Gradio.Net.Tests;

/// <summary>
/// Images a test can hand a component: real files, because an image component decodes what it is
/// given and a file named <c>.png</c> that holds text is not one.
/// </summary>
/// <remarks>
/// Written through <see cref="RasterImage"/> rather than checked in as fixtures: a checked-in picture
/// is a file whose pixels nobody can see in the test that uses it, and one encoded here is a picture
/// the test knows exactly.
/// </remarks>
internal static class TestImages
{
    /// <summary>A solid-colour image, as the bytes of a PNG.</summary>
    /// <param name="width">How wide, in pixels.</param>
    /// <param name="height">How tall, in pixels.</param>
    /// <param name="red">The red channel.</param>
    /// <param name="green">The green channel.</param>
    /// <param name="blue">The blue channel.</param>
    /// <param name="alpha">The alpha channel.</param>
    /// <returns>The encoded image.</returns>
    internal static byte[] Png(
        int width = 2,
        int height = 2,
        byte red = 0,
        byte green = 0,
        byte blue = 0,
        byte alpha = 255)
    {
        var pixels = new byte[width * height * 4];

        for (var index = 0; index < pixels.Length; index += 4)
        {
            pixels[index] = red;
            pixels[index + 1] = green;
            pixels[index + 2] = blue;
            pixels[index + 3] = alpha;
        }

        return new RasterImage(width, height, pixels).ToBytes("png");
    }
}

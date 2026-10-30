using System.Buffers.Binary;
using System.IO.Compression;

namespace Gradio.Net;
/// <summary>
/// The little image handling an annotated image needs: how big a picture is, and a mask drawn on a
/// transparent canvas.
/// </summary>
/// <remarks>
/// <para>
/// The Python version rasterizes a mask with PIL and numpy. Here the drawing is a loop over the
/// pixels this class allocates and the encoding is the image library's, so the mask the frontend
/// receives is the same picture the Python version paints: the interior half opaque, a three-pixel
/// border fully opaque, in the label's colour.
/// </para>
/// <para>
/// A picture's size is read from the header for the two formats that carry it there — PNG and JPEG,
/// which is what a base image usually is — and decoded for anything else. The header is the fast
/// path rather than the rule, because a base image in WebP or GIF is a picture this can measure and
/// refusing it would be a refusal the library made unnecessary.
/// </para>
/// </remarks>
internal static class ImageRaster
{
    /// <summary>The eight bytes every PNG starts with.</summary>
    private static readonly byte[] _pngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    /// <summary>A picture's size, or <see langword="null"/> when this cannot read it.</summary>
    /// <param name="bytes">The picture's bytes.</param>
    /// <returns>The width and height, or <see langword="null"/>.</returns>
    internal static (int Width, int Height)? SizeOf(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length >= 24 && bytes[..8].SequenceEqual(_pngSignature))
        {
            // The IHDR chunk is the first one and its data starts at byte 16: width then height,
            // each four bytes big-endian.
            return (
                BinaryPrimitives.ReadInt32BigEndian(bytes[16..20]),
                BinaryPrimitives.ReadInt32BigEndian(bytes[20..24]));
        }

        if (JpegSize(bytes) is { } jpeg)
        {
            return jpeg;
        }

        // Anything else is measured by decoding it, which is what the Python version does for every format it
        // does not have a header reader for.
        try
        {
            var image = RasterImage.Open(bytes);

            return (image.Width, image.Height);
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    /// <summary>A transparent PNG with a rectangle drawn on it.</summary>
    /// <param name="width">The canvas width.</param>
    /// <param name="height">The canvas height.</param>
    /// <param name="box">The rectangle: left, top, right and bottom.</param>
    /// <param name="color">The colour the rectangle is painted in.</param>
    /// <returns>The PNG's bytes.</returns>
    /// <remarks>
    /// The Python version's mask is a float array where the rectangle's interior is half opaque and
    /// its border — three pixels wide — is fully opaque, painted in the annotation's colour. The
    /// same numbers are reproduced here so that the two implementations draw the same picture: the
    /// interior's alpha is 127 rather than 128, because a half written to a byte is truncated.
    /// </remarks>
    internal static byte[] MaskPng(int width, int height, (int Left, int Top, int Right, int Bottom) box, (byte Red, byte Green, byte Blue) color)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);

        const int border = 3;
        var pixels = new byte[width * height * 4];

        var left = Math.Clamp(box.Left, 0, width);
        var right = Math.Clamp(box.Right, 0, width);
        var top = Math.Clamp(box.Top, 0, height);
        var bottom = Math.Clamp(box.Bottom, 0, height);

        for (var y = top; y < bottom; y++)
        {
            for (var x = left; x < right; x++)
            {
                var edge = x < left + border || x >= right - border || y < top + border || y >= bottom - border;
                var alpha = edge ? (byte)255 : (byte)127;
                var offset = ((y * width) + x) * 4;

                pixels[offset] = color.Red;
                pixels[offset + 1] = color.Green;
                pixels[offset + 2] = color.Blue;
                pixels[offset + 3] = alpha;
            }
        }

        return new RasterImage(width, height, pixels).ToBytes("png");
    }

    /// <summary>A colour as a hex code, or red when it is not one.</summary>
    /// <param name="hex">The colour, such as <c>#00ff00</c>.</param>
    /// <returns>The three channels.</returns>
    /// <remarks>
    /// The Python version's <c>hex_to_rgb</c> accepts a three- or six-digit code and falls back to
    /// red for a label whose colour is not in the map. The fallback is here rather than at the call
    /// site because it is part of how the mask is drawn.
    /// </remarks>
    internal static (byte Red, byte Green, byte Blue) ColorOf(string? hex)
    {
        if (hex is null)
        {
            return (255, 0, 0);
        }

        var digits = hex.TrimStart('#');

        if (digits.Length == 3)
        {
            digits = string.Concat(digits.Select(digit => new string(digit, 2)));
        }

        if (digits.Length != 6 || !int.TryParse(digits, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out var value))
        {
            return (255, 0, 0);
        }

        return ((byte)((value >> 16) & 0xFF), (byte)((value >> 8) & 0xFF), (byte)(value & 0xFF));
    }

    /// <summary>The size a JPEG's frame header declares.</summary>
    /// <param name="bytes">The file.</param>
    /// <returns>The size, or <see langword="null"/> when no frame header was found.</returns>
    /// <remarks>
    /// A JPEG is a sequence of markers; the frame header — <c>FFC0</c> to <c>FFCF</c>, excluding the
    /// four that mean something else — carries the height and the width. Only the first one is read,
    /// which is the size of the picture a browser draws.
    /// </remarks>
    private static (int Width, int Height)? JpegSize(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length < 4 || bytes[0] != 0xFF || bytes[1] != 0xD8)
        {
            return null;
        }

        var index = 2;

        while (index + 9 < bytes.Length)
        {
            if (bytes[index] != 0xFF)
            {
                index++;
                continue;
            }

            var marker = bytes[index + 1];

            // A marker with no payload: the restart markers and the fill bytes.
            if (marker is 0xD8 or 0x01 or >= 0xD0 and <= 0xD7)
            {
                index += 2;
                continue;
            }

            var length = BinaryPrimitives.ReadUInt16BigEndian(bytes[(index + 2)..(index + 4)]);

            if (marker is >= 0xC0 and <= 0xCF and not 0xC4 and not 0xC8 and not 0xCC)
            {
                return (
                    BinaryPrimitives.ReadUInt16BigEndian(bytes[(index + 7)..(index + 9)]),
                    BinaryPrimitives.ReadUInt16BigEndian(bytes[(index + 5)..(index + 7)]));
            }

            index += 2 + length;
        }

        return null;
    }

}

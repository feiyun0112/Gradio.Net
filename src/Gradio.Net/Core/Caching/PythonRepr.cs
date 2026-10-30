using System.Globalization;
using System.Text;

namespace Gradio.Net;
/// <summary>
/// How a value is spelled when it is part of a cache key: Python's <c>repr</c>.
/// </summary>
/// <remarks>
/// <para>
/// The Python version hashes the <c>repr</c> of a value, so the spelling is not a detail of this
/// framework — it is the algorithm. Two sides that spell <c>1.0</c> differently compute different
/// keys for the same value, and a cache key is only useful if it is stable. The rules reproduced
/// here are CPython's: a string prefers single quotes and switches to double quotes when it
/// contains one and no double quote; a float keeps the shortest text that reads back as the same
/// number, in fixed notation between <c>1e-4</c> and <c>1e16</c> and scientific notation outside
/// it, with at least two exponent digits.
/// </para>
/// <para>
/// It is internal because it is an implementation of <see cref="CacheHash"/> rather than a thing an
/// application asks for. What is public is the digest, and the digest is what has to agree.
/// </para>
/// </remarks>
internal static class PythonRepr
{
    /// <summary>A value, spelled the way Python's <c>repr</c> spells it.</summary>
    /// <param name="value">The value.</param> <returns>The text.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is
    /// <see langword="null"/>.</exception>
    internal static string Of(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        // CPython picks the quote that needs fewer escapes, and only considers double quotes when
        // the text contains a single one.
        var quote = value.Contains('\'') && !value.Contains('"') ? '"' : '\'';
        var text = new StringBuilder(value.Length + 2);

        text.Append(quote);

        foreach (var character in value)
        {
            switch (character)
            {
                case '\\':
                    text.Append("\\\\");
                    break;

                case '\n':
                    text.Append("\\n");
                    break;

                case '\r':
                    text.Append("\\r");
                    break;

                case '\t':
                    text.Append("\\t");
                    break;

                default:
                    if (character == quote)
                    {
                        text.Append('\\').Append(character);
                    }
                    else if (IsUnprintable(character))
                    {
                        text.Append("\\x").Append(((int)character).ToString("x2", CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        text.Append(character);
                    }

                    break;
            }
        }

        return text.Append(quote).ToString();
    }

    /// <summary>A float, spelled the way Python's <c>repr</c> spells it.</summary>
    /// <param name="value">The number.</param>
    /// <returns>The text.</returns>
    /// <remarks>
    /// The digits are the shortest text that reads back as the same number, which is what both
    /// runtimes produce; what differs is the notation. Python stays in fixed notation while the
    /// decimal exponent is at least <c>-4</c> and below <c>16</c>, marks an integral value with
    /// <c>.0</c> so that it does not read as an integer, and writes the exponent in lower case with
    /// a sign and two digits.
    /// </remarks>
    internal static string Of(double value)
    {
        if (double.IsNaN(value))
        {
            return "nan";
        }

        if (double.IsPositiveInfinity(value))
        {
            return "inf";
        }

        if (double.IsNegativeInfinity(value))
        {
            return "-inf";
        }

        var text = value.ToString("R", CultureInfo.InvariantCulture);
        var exponentAt = text.IndexOfAny(['E', 'e']);

        if (exponentAt < 0)
        {
            return text.Contains('.') ? text : text + ".0";
        }

        var mantissa = text[..exponentAt];
        var exponent = int.Parse(text[(exponentAt + 1)..], NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture);

        // Python keeps short exponents in fixed notation, which is why 1e-4 prints as 0.0001 while
        // 1e-5 prints as 1e-05.
        if (exponent is >= -4 and < 16)
        {
            var fixedText = value.ToString("0.################", CultureInfo.InvariantCulture);

            return fixedText.Contains('.') ? fixedText : fixedText + ".0";
        }

        var sign = exponent < 0 ? '-' : '+';
        var digits = Math.Abs(exponent).ToString("00", CultureInfo.InvariantCulture);

        return $"{mantissa}e{sign}{digits}";
    }

    /// <summary>Whether a character has to be escaped rather than written as itself.</summary>
    /// <param name="character">The character.</param>
    /// <returns><see langword="true"/> when Python's <c>repr</c> escapes it.</returns>
    /// <remarks>
    /// Python escapes anything that is not printable, and "printable" excludes the control,
    /// format, surrogate, private-use and separator categories — a space being the one separator it
    /// keeps. Reproducing the category test rather than a list of characters is what makes a
    /// non-breaking space come out as <c>\xa0</c>, which is what Python writes.
    /// </remarks>
    private static bool IsUnprintable(char character)
    {
        if (character == ' ')
        {
            return false;
        }

        return char.IsControl(character)
            || char.IsSurrogate(character)
            || CharUnicodeInfo.GetUnicodeCategory(character) is
                UnicodeCategory.Format
                or UnicodeCategory.LineSeparator
                or UnicodeCategory.ParagraphSeparator
                or UnicodeCategory.SpaceSeparator
                or UnicodeCategory.PrivateUse
                or UnicodeCategory.OtherNotAssigned;
    }
}

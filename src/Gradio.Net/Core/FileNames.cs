using System.Text;

namespace Gradio.Net;
/// <summary>
/// A name made safe to use as a file name, the way the Python version's
/// <c>strip_invalid_filename_characters</c> does it.
/// </summary>
/// <remarks>
/// <para>
/// Two things use this and both of them are about a name that came from somewhere else: a flagging
/// log names a directory after a component's label, and an upload keeps the name the browser sent.
/// The rule is deliberately narrow — only what is genuinely dangerous is removed, and everything
/// else is kept, unicode included. A label like "Café (best)" is a perfectly good directory name.
/// </para>
/// <para>
/// What is removed rather than replaced: the characters Windows forbids (<c>&lt; &gt; : " / \ | ? *</c>),
/// the control characters, and the ones a shell would act on (<c>` $ ! { }</c>). What is added: a
/// leading underscore for the names Windows reserves for devices, because <c>CON.csv</c> is not a
/// file on NTFS; and a <c>file</c> stem for a name that was left with nothing but its extension.
/// </para>
/// </remarks>
public static class FileNames
{
    /// <summary>The characters that are removed wherever they appear.</summary>
    /// <remarks>
    /// The Python version's <c>_FORBIDDEN_RE</c>, character for character. A removal rather than a
    /// replacement because the name is being repaired, not escaped: two characters removed from
    /// "a:b" leave "ab", which is a name someone can still recognise.
    /// </remarks>
    private static readonly char[] _forbidden =
        ['<', '>', ':', '"', '/', '\\', '|', '?', '*', '`', '$', '!', '{', '}'];

    /// <summary>The names Windows keeps for devices rather than for files.</summary>
    /// <remarks>
    /// Case-insensitive, and matched against the part of the name before its first dot, because
    /// that is how Windows resolves them: <c>NUL.tar.gz</c> is the NUL device.
    /// </remarks>
    private static readonly HashSet<string> _reserved = new(StringComparer.OrdinalIgnoreCase)
    {
        "CON", "PRN", "AUX", "NUL", "CONIN$", "CONOUT$",
        "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "COM¹", "COM²", "COM³",
        "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9", "LPT¹", "LPT²", "LPT³",
    };

    /// <summary>A file name with the dangerous characters removed and its length bounded.</summary>
    /// <param name="filename">The name, which may carry an extension.</param>
    /// <param name="maxBytes">The most bytes the name may take in UTF-8.</param>
    /// <returns>The name to use.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="filename"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxBytes"/> is negative.</exception>
    /// <remarks>
    /// The extension is kept when it fits, because a file without one is a file nothing knows how to
    /// open. Truncation removes characters rather than bytes, so a name that was cut is still valid
    /// UTF-8 — which is why the length is counted after each removal rather than computed once.
    /// </remarks>
    public static string StripInvalidCharacters(string filename, int maxBytes = 200)
    {
        ArgumentNullException.ThrowIfNull(filename);
        ArgumentOutOfRangeException.ThrowIfNegative(maxBytes);

        var (name, extension) = SplitExtension(filename);

        name = Remove(name);

        if (extension.Length > 0)
        {
            extension = "." + Remove(extension[1..]);
        }

        // A name that was nothing but an extension would become a dotfile, which is a different
        // file: ".txt" has no extension as far as a file system is concerned.
        if (name.Length == 0 && extension.Length > 0)
        {
            name = "file";
        }

        // The parent directory, spelled as a name. Kept as it is so that whatever validates the
        // path can refuse it, rather than being handed "file" and accepting it.
        if (name + extension == "..")
        {
            return "..";
        }

        // Windows drops a trailing space or dot from a path segment, silently. Doing it here means
        // the name the caller gets is the name that will exist, on every platform.
        filename = (name + extension).TrimEnd(' ', '.');

        if (filename.Length == 0)
        {
            filename = "file";
        }

        (name, extension) = SplitExtension(filename);

        if (_reserved.Contains(name.Split('.')[0].TrimEnd(' ')))
        {
            name = "_" + name;
        }

        filename = name + extension;

        while (Encoding.UTF8.GetByteCount(filename) > maxBytes && name.Length > 0)
        {
            name = name[..^1];
            filename = name + extension;
        }

        if (Encoding.UTF8.GetByteCount(filename) <= maxBytes)
        {
            return filename;
        }

        // An extension can be longer than the whole budget. The stem is what has to survive, so it
        // becomes the fallback name and the extension is cut back to what fits.
        name = "file";

        while (Encoding.UTF8.GetByteCount(name) > maxBytes && name.Length > 0)
        {
            name = name[..^1];
        }

        while (Encoding.UTF8.GetByteCount(name + extension) > maxBytes && extension.Length > 0)
        {
            extension = extension[..^1];
        }

        return name + extension;
    }

    /// <summary>A name split where a file system splits it: the last dot that is not the first
    /// character.</summary>
    /// <param name="filename">The name.</param>
    /// <returns>The stem and the extension, the extension including its dot or empty.</returns>
    /// <remarks>
    /// Python's <c>os.path.splitext</c>, which is what the Python version calls: leading dots
    /// belong to the stem, so <c>.bashrc</c> has no extension and <c>a.tar.gz</c> has <c>.gz</c>.
    /// </remarks>
    private static (string Name, string Extension) SplitExtension(string filename)
    {
        for (var index = filename.Length - 1; index > 0; index--)
        {
            if (filename[index] == '.')
            {
                return (filename[..index], filename[index..]);
            }
        }

        return (filename, string.Empty);
    }

    /// <summary>A string with every forbidden character removed.</summary>
    /// <param name="text">The text.</param>
    /// <returns>The text without them.</returns>
    private static string Remove(string text)
    {
        var cleaned = new StringBuilder(text.Length);

        foreach (var character in text)
        {
            if (Array.IndexOf(_forbidden, character) >= 0
                || character < ' '
                || character == '\x7f')
            {
                continue;
            }

            cleaned.Append(character);
        }

        return cleaned.ToString();
    }
}

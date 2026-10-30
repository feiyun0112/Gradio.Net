using System.Collections.Frozen;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Gradio.Net;
/// <summary>
/// The directory an application keeps its files in, and the rules for putting one there
/// </summary>
/// <remarks>
/// <para>
/// Everything a component writes — an uploaded file, a handler's output, a component's default
/// value — lands under one directory, addressed by a hash of its content. That is the Python
/// version's scheme, and it is worth copying exactly: two runs that produce the same bytes share
/// one file, a URL cannot be guessed from a name, and "is this file mine to serve?" is answered by
/// asking whether it is under this directory.
/// </para>
/// <para>
/// The hash is salted with a per-process seed, as the Python version salts it with a seed it keeps
/// beside its own package. The consequence of a per-process seed rather than a persisted one is
/// that a path handed out by one process names nothing to the next; the files themselves are still
/// served, because the rule is about the directory rather than about the path, and reclamation
/// removes them.
/// </para>
/// </remarks>
public static partial class MediaCache
{
    /// <summary>The characters a file name may not contain, which are the ones that make a name dangerous.</summary>
    /// <remarks>
    /// The Python version's set: path separators, the characters a shell or a URL reads specially,
    /// control characters, and the Windows device-name punctuation. Everything else — accents,
    /// spaces, brackets — is a legitimate part of a name and is kept.
    /// </remarks>
    private static readonly Regex _forbidden = new(
        "[<>:\"/\\\\|?*\\x00-\\x1f\\x7f`$!{}]",
        RegexOptions.CultureInvariant);

    /// <summary>The names Windows resolves to a device rather than to a file.</summary>
    private static readonly FrozenSet<string> _reservedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "AUX", "CON", "CONIN$", "CONOUT$", "NUL", "PRN",
        "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
        "COM¹", "COM²", "COM³",
        "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9",
        "LPT¹", "LPT²", "LPT³",
    }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    /// <summary>The longest name that is kept whole, in bytes.</summary>
    /// <remarks>
    /// A name is truncated to this many bytes of its stem, extension preserved, because a name that
    /// is too long for the file system is a file that cannot be written at all. The Python version
    /// uses the same limit.
    /// </remarks>
    private const int MaxNameBytes = 200;

    /// <summary>The directory a file goes in when no application says otherwise.</summary>
    /// <value>
    /// <c>GRADIO_TEMP_DIR</c> when it is set, and the system's temporary directory joined with
    /// <c>gradio</c> otherwise — the Python version's rule, and the same variable, so a deployment
    /// that sets it for the Python framework gets the same answer here.
    /// </value>
    public static string DefaultFolder { get; } = ResolveDefaultFolder();

    /// <summary>The per-process salt every content hash starts with.</summary>
    /// <remarks>
    /// It makes a cache path unguessable: without it, anyone who can compute the hash of a file
    /// could name a URL for it before it exists. It is generated once per process and never
    /// written anywhere.
    /// </remarks>
    public static string Seed { get; } = Hex(RandomNumberGenerator.GetBytes(16));

    /// <summary>The address a browser fetches a file from, relative to the application.</summary>
    /// <param name="path">The file's path.</param>
    /// <returns>The path the Python version's file endpoint serves it at.</returns>
    /// <remarks>
    /// The path is percent-encoded because it is carried inside a URL and may contain a space, a
    /// backslash on Windows, or a character a browser would otherwise read as structure. The
    /// separator is left as it is, which is what the Python version's encoder does: a Windows path
    /// reaches the endpoint with its backslashes escaped and its drive colon escaped, and a path on
    /// a Unix system arrives with its slashes intact.
    /// </remarks>
    public static string UrlOf(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        return FileRoute + Uri.EscapeDataString(path).Replace("%2F", "/", StringComparison.Ordinal);
    }

    /// <summary>The prefix every served file's address begins with.</summary>
    /// <remarks>
    /// Shared with the code that reads such an address back into a path: a workflow's graph stores
    /// references, and a reference may name the served address rather than the file. One constant,
    /// so that the writer and the reader cannot drift apart.
    /// </remarks>
    internal const string FileRoute = "/gradio_api/file=";

    /// <summary>Whether a path is inside a directory, or is that directory.</summary>
    /// <param name="path">The path to test.</param>
    /// <param name="folder">The directory to test it against.</param>
    /// <returns>Whether <paramref name="path"/> is <paramref name="folder"/> or below it.</returns>
    /// <remarks>
    /// Both sides are made absolute and compared segment by segment, so <c>/a/bc</c> is not inside
    /// <c>/a/b</c>. The comparison ignores case on Windows, where the file system does, and does
    /// not elsewhere — which is the Python version's rule as pathlib implements it.
    /// </remarks>
    public static bool IsInFolder(string path, string folder) =>
        IsInFolder(
            path,
            folder,
            OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

    /// <summary>Whether a path is a directory or below it, under a comparison the caller chooses.</summary>
    /// <param name="path">The path to test.</param>
    /// <param name="folder">The directory to test it against.</param>
    /// <param name="comparison">How the two are compared.</param>
    /// <returns>Whether <paramref name="path"/> is <paramref name="folder"/> or below it.</returns>
    /// <remarks>
    /// The comparison is the caller's for the lists an application declares: the Python version
    /// lowercases both sides of an allowed or blocked path on every system, so a block holds even
    /// where the file system would treat two spellings as different files.
    /// </remarks>
    internal static bool IsInFolder(string path, string folder, StringComparison comparison)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentException.ThrowIfNullOrWhiteSpace(folder);

        var full = Path.GetFullPath(path);
        var root = Path.GetFullPath(folder);

        if (string.Equals(full, root, comparison))
        {
            return true;
        }

        var prefix = root.EndsWith(Path.DirectorySeparatorChar) ? root : root + Path.DirectorySeparatorChar;

        return full.StartsWith(prefix, comparison);
    }

    /// <summary>Whether a path names a location rather than a file on this server.</summary>
    /// <param name="path">The path a client sent.</param>
    /// <returns>
    /// Whether it carries a scheme, is protocol-relative, or is a Windows UNC spelling.
    /// </returns>
    /// <remarks>
    /// <para>
    /// The Python version's <c>starts_with_protocol</c>, character for character. A rooted path is
    /// deliberately not a location: on a Unix system every path is rooted, so treating one as a
    /// location would make the file endpoint refuse every file it has.
    /// </para>
    /// <para>
    /// The question this answers is "is this somewhere else?", which is what decides whether a
    /// value is copied into the application's cache and whether the browser is told to fetch it
    /// itself.
    /// </para>
    /// </remarks>
    public static bool NamesALocation(string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        return ProtocolPrefix().IsMatch(path);
    }

    /// <summary>A file name with everything dangerous about it removed.</summary>
    /// <param name="name">The name, as the client sent it.</param>
    /// <returns>The name that can be written to disk.</returns>
    /// <remarks>
    /// <para>
    /// The rule is the Python version's, character for character, because the name ends up in a URL
    /// and in a directory listing: forbidden characters are removed, trailing dots and spaces go
    /// (Windows drops them silently, which would make the path the client is told differ from the
    /// file), a reserved device name is prefixed so it is not a device, and an over-long name is
    /// truncated at a character boundary so its extension survives.
    /// </para>
    /// <para>
    /// <c>..</c> is returned as it is: it is not a name that can be written, and leaving it alone is
    /// what makes the join that follows refuse it rather than write it as a file called
    /// <c>..</c> — which would be the parent directory.
    /// </para>
    /// </remarks>
    public static string SanitizeFileName(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        var (rawStem, rawExtension) = SplitExtension(name);
        var stem = _forbidden.Replace(rawStem, string.Empty);
        var extension = rawExtension.Length > 0
            ? "." + _forbidden.Replace(rawExtension[1..], string.Empty)
            : string.Empty;

        if (stem.Length == 0 && extension.Length > 0)
        {
            stem = "file";
        }

        if (stem + extension == "..")
        {
            return "..";
        }

        var trimmed = (stem + extension).TrimEnd(' ', '.');
        if (trimmed.Length == 0)
        {
            trimmed = "file";
        }

        (stem, extension) = SplitExtension(trimmed);

        // Windows resolves a device name from the segment before the first dot, so "NUL.tar.gz" is
        // the device too. The check is on that segment rather than on the whole name.
        var device = stem.Split('.')[0].TrimEnd(' ').ToUpperInvariant();
        if (_reservedNames.Contains(device))
        {
            stem = "_" + stem;
        }

        while (Encoding.UTF8.GetByteCount(stem + extension) > MaxNameBytes && stem.Length > 0)
        {
            stem = stem[..^1];
        }

        if (Encoding.UTF8.GetByteCount(stem + extension) <= MaxNameBytes)
        {
            return stem + extension;
        }

        stem = "file";
        while (extension.Length > 0 && Encoding.UTF8.GetByteCount(stem + extension) > MaxNameBytes)
        {
            extension = extension[..^1];
        }

        return stem + extension;
    }

    /// <summary>A name split into its stem and its extension, the way the Python version's
    /// <c>splitext</c> splits one.</summary> <param name="name">The name.</param> <returns>The
    /// stem, and the extension with its dot or an empty string.</returns>
    /// <remarks>
    /// Not <c>Path.GetFileNameWithoutExtension</c>: that reads a colon or a separator in the name
    /// as structure and drops what comes before it, so <c>a:b?.txt</c> would lose its first
    /// character before the forbidden characters were even removed. The Python version splits on
    /// the last dot and nothing else, and a name that is all dots has no extension.
    /// </remarks>
    private static (string Stem, string Extension) SplitExtension(string name)
    {
        var dot = name.LastIndexOf('.');

        if (dot <= 0 || name.AsSpan(0, dot).TrimStart('.').IsEmpty)
        {
            return (name, string.Empty);
        }

        return (name[..dot], name[dot..]);
    }

    /// <summary>The hash a file's content is addressed by.</summary>
    /// <param name="path">The file.</param>
    /// <returns>The hash, in lower-case hexadecimal.</returns>
    /// <exception cref="FileNotFoundException">The file does not exist.</exception>
    public static string HashFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        using var hash = CreateContentHash();

        using var stream = System.IO.File.OpenRead(path);
        var buffer = new byte[128 * 32];
        int read;

        while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
        {
            hash.AppendData(buffer, 0, read);
        }

        return Complete(hash);
    }

    /// <summary>The hash a value's bytes are addressed by.</summary>
    /// <param name="bytes">The bytes.</param>
    /// <returns>The hash, in lower-case hexadecimal.</returns>
    public static string HashBytes(ReadOnlySpan<byte> bytes)
    {
        using var hash = CreateContentHash();
        hash.AppendData(bytes);

        return Complete(hash);
    }

    /// <summary>Copies a file into the cache, unless its content is already there.</summary>
    /// <param name="path">The file to cache.</param>
    /// <param name="folder">The cache directory.</param>
    /// <returns>The cached path, absolute.</returns>
    /// <exception cref="FileNotFoundException">The file does not exist.</exception>
    /// <remarks>
    /// A file that is already under the cache directory is returned as it is: it was cached by an
    /// earlier call, or it is an upload, and copying it would make a second file with the same
    /// content.
    /// </remarks>
    public static string SaveFile(string path, string folder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentException.ThrowIfNullOrWhiteSpace(folder);

        var source = Path.GetFullPath(path);

        // The existence check comes before the shortcut, because the shortcut answers "the file is
        // already cached" and a file that is not there is not cached: a path inside the folder that
        // names nothing has to be reported rather than handed back as if it were a file.
        if (!System.IO.File.Exists(source))
        {
            throw new FileNotFoundException($"The file '{path}' does not exist.", path);
        }

        if (IsInFolder(source, folder))
        {
            return source;
        }

        var destination = Path.Combine(DirectoryFor(folder, HashFile(source)), SanitizeFileName(Path.GetFileName(source)));

        if (!System.IO.File.Exists(destination))
        {
            System.IO.File.Copy(source, destination, overwrite: false);
        }

        return destination;
    }

    /// <summary>Writes bytes into the cache under a name.</summary>
    /// <param name="bytes">The content.</param>
    /// <param name="name">The name to keep, which is sanitized.</param>
    /// <param name="folder">The cache directory.</param>
    /// <returns>The cached path, absolute.</returns>
    public static string SaveBytes(ReadOnlySpan<byte> bytes, string name, string folder)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(folder);

        var destination = Path.Combine(DirectoryFor(folder, HashBytes(bytes)), SanitizeFileName(name));

        if (!System.IO.File.Exists(destination))
        {
            System.IO.File.WriteAllBytes(destination, bytes.ToArray());
        }

        return destination;
    }

    /// <summary>Copies a stream into the cache under a name.</summary>
    /// <param name="content">The content.</param>
    /// <param name="name">The name to keep, which is sanitized.</param>
    /// <param name="folder">The cache directory.</param>
    /// <returns>The cached path, absolute.</returns>
    /// <remarks>
    /// The content is buffered to a temporary file rather than to memory, because a stream's length
    /// is not known and a file the size of memory is exactly what the hash-and-copy scheme exists to
    /// avoid holding.
    /// </remarks>
    public static string SaveStream(Stream content, string name, string folder)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(folder);

        Directory.CreateDirectory(folder);

        var temporary = Path.Combine(folder, "tmp" + Hex(RandomNumberGenerator.GetBytes(5)));
        try
        {
            using (var file = System.IO.File.Create(temporary))
            {
                content.CopyTo(file);
            }

            var destination = Path.Combine(
                DirectoryFor(folder, HashFile(temporary)),
                SanitizeFileName(name));

            if (!System.IO.File.Exists(destination))
            {
                System.IO.File.Move(temporary, destination);
            }

            return destination;
        }
        finally
        {
            if (System.IO.File.Exists(temporary))
            {
                System.IO.File.Delete(temporary);
            }
        }
    }

    /// <summary>The directory a hash addresses, created if it is new.</summary>
    /// <param name="folder">The cache directory.</param>
    /// <param name="hash">The content hash.</param>
    /// <returns>The directory, absolute.</returns>
    public static string DirectoryFor(string folder, string hash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folder);
        ArgumentException.ThrowIfNullOrWhiteSpace(hash);

        var directory = Path.Combine(Path.GetFullPath(folder), hash);
        Directory.CreateDirectory(directory);

        return directory;
    }

    /// <summary>The default folder, as the environment names it.</summary>
    private static string ResolveDefaultFolder()
    {
        var configured = Environment.GetEnvironmentVariable("GRADIO_TEMP_DIR");

        return !string.IsNullOrWhiteSpace(configured)
            ? Path.GetFullPath(configured)
            : Path.Combine(Path.GetTempPath(), "gradio");
    }

    /// <summary>Starts a hash the way the Python version starts one: with the seed in front of the
    /// content.</summary> <returns>The hash to append content to.</returns>
    /// <remarks>
    /// Exposed so that a stream can be hashed as it is written rather than after: an upload is
    /// addressed by the hash of content that is being received, and reading the file back to hash it
    /// would double the work and the time the file is on disk before it is addressable.
    /// </remarks>
    internal static IncrementalHash CreateContentHash()
    {
        var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        AppendSeed(hash);

        return hash;
    }

    /// <summary>The hash a finished content hash names.</summary>
    /// <param name="hash">The hash the content was appended to.</param>
    /// <returns>The hash, in lower-case hexadecimal.</returns>
    internal static string Complete(IncrementalHash hash)
    {
        ArgumentNullException.ThrowIfNull(hash);

        return Hex(hash.GetHashAndReset());
    }

    /// <summary>Starts a hash the way the Python version starts one: with the seed in front of the
    /// content.</summary>
    private static void AppendSeed(IncrementalHash hash) =>
        hash.AppendData(Encoding.UTF8.GetBytes(Seed));

    /// <summary>Bytes as lower-case hexadecimal, on every target framework.</summary>
    private static string Hex(ReadOnlySpan<byte> bytes) =>
        Convert.ToHexString(bytes).ToLowerInvariant();

    /// <summary>A scheme, a protocol-relative path, or a Windows UNC spelling.</summary>
    [GeneratedRegex(@"^(?:[a-zA-Z][a-zA-Z0-9+\-.]*://|//|\\\\|\\/|/\\)", RegexOptions.CultureInvariant)]
    private static partial Regex ProtocolPrefix();
}

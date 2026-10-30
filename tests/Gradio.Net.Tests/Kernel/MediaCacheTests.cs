using Gradio.Net;
using Gradio.Net.Tests.Server;
using Xunit;

namespace Gradio.Net.Tests.Kernel;

/// <summary>
/// The rules an application keeps its files by: what a name may contain, where a file goes, and
/// which paths are inside the directory at all.
/// </summary>
/// <remarks>
/// These are the rules the file endpoint's security rests on, so they are asserted directly rather
/// than only through a request: a traversal attempt is answered by <c>IsInFolder</c> saying no, and
/// a test that only ever sees a 403 cannot tell which of the checks refused it.
/// </remarks>
public sealed class MediaCacheTests
{
    [Fact]
    public void A_file_url_keeps_its_separators_and_escapes_everything_else()
    {
        var url = MediaCache.UrlOf(Path.Combine(Path.GetTempPath(), "a folder", "b.txt"));

        Assert.StartsWith("/gradio_api/file=", url, StringComparison.Ordinal);

        // The separator is left as it is, which is what the Python version's encoder does: the frontend's
        // route is a catch-all, so a path with slashes in it is one value rather than many
        // segments. A space is not, and has to be escaped.
        Assert.Contains('/', url);
        Assert.Contains("%20", url, StringComparison.Ordinal);
        Assert.DoesNotContain(" ", url, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("/a/b/c.txt", "/a/b", true)]
    [InlineData("/a/b", "/a/b", true)]
    [InlineData("/a/bc/d.txt", "/a/b", false)]
    [InlineData("/a", "/a/b", false)]
    public void A_path_is_inside_a_folder_only_below_its_boundary(string path, string folder, bool inside) =>
        Assert.Equal(inside, MediaCache.IsInFolder(path, folder));

    [Fact]
    public void A_path_that_climbs_out_of_a_folder_is_not_inside_it()
    {
        var folder = Path.Combine(Path.GetTempPath(), "gradio-net-tests", "inside");

        Assert.False(MediaCache.IsInFolder(Path.Combine(folder, "..", "outside.txt"), folder));
    }

    [Theory]
    [InlineData("http://example.com/a.txt")]
    [InlineData("https://example.com/a.txt")]
    [InlineData("//example.com/a.txt")]
    [InlineData("\\\\server\\share\\a.txt")]
    public void A_path_that_names_a_location_is_recognized_as_one(string path) =>
        Assert.True(MediaCache.NamesALocation(path));

    [Theory]
    [InlineData("a.txt")]
    [InlineData("a/b.txt")]
    [InlineData("C:\\Users\\a.txt")]
    [InlineData("/tmp/gradio/a.txt")]
    public void A_path_that_names_a_file_is_not_a_location(string path) =>
        Assert.False(MediaCache.NamesALocation(path));

    [Fact]
    public void A_rooted_path_is_a_file_rather_than_a_location()
    {
        // On a Unix system every path is rooted, so treating a rooted path as a location would make
        // the file endpoint refuse every file it keeps. The Python version's rule is about a protocol, not
        // about where the path starts.
        Assert.False(MediaCache.NamesALocation(Path.Combine(Path.GetTempPath(), "gradio", "a.txt")));
    }

    [Theory]
    [InlineData("plain.txt", "plain.txt")]
    [InlineData("a<b>c.txt", "abc.txt")]
    [InlineData("a:b?.txt", "ab.txt")]
    [InlineData("a|b*.txt", "ab.txt")]
    [InlineData("quote\".txt", "quote.txt")]
    [InlineData("back`tick.txt", "backtick.txt")]
    [InlineData("brace{}.txt", "brace.txt")]
    [InlineData("dollar$.txt", "dollar.txt")]
    [InlineData("bang!.txt", "bang.txt")]
    [InlineData("with space.txt", "with space.txt")]
    [InlineData("naïve.txt", "naïve.txt")]
    public void A_file_name_keeps_what_is_legitimate_and_loses_what_is_dangerous(string name, string expected) =>
        Assert.Equal(expected, MediaCache.SanitizeFileName(name));

    [Theory]
    [InlineData("trailing.txt.")]
    [InlineData("trailing.txt ")]
    [InlineData("trailing.txt. . ")]
    public void Trailing_dots_and_spaces_are_dropped(string name) =>
        Assert.Equal("trailing.txt", MediaCache.SanitizeFileName(name));

    [Theory]
    [InlineData("NUL")]
    [InlineData("nul.txt")]
    [InlineData("CON.tar.gz")]
    [InlineData("com1.log")]
    [InlineData("LPT9")]
    public void A_name_windows_resolves_to_a_device_is_prefixed(string name)
    {
        var sanitized = MediaCache.SanitizeFileName(name);

        Assert.StartsWith("_", sanitized, StringComparison.Ordinal);
        Assert.Equal("_" + name, sanitized);
    }

    [Fact]
    public void A_name_with_nothing_left_of_it_becomes_a_name_that_can_be_written()
    {
        // Every character removed leaves no name at all; writing a file called nothing is not
        // something the file system can do.
        Assert.Equal("file.txt", MediaCache.SanitizeFileName("<>?.txt"));
    }

    [Fact]
    public void Two_dots_are_left_alone_so_that_the_join_refuses_them()
    {
        // A name that survived as ".." would be the parent directory once joined. Returning it as
        // it is makes the caller's join the place that refuses it, which is the Python version's rule.
        Assert.Equal("..", MediaCache.SanitizeFileName(".."));
    }

    [Fact]
    public void An_over_long_name_is_truncated_and_keeps_its_extension()
    {
        var name = new string('a', 400) + ".txt";

        var sanitized = MediaCache.SanitizeFileName(name);

        Assert.True(System.Text.Encoding.UTF8.GetByteCount(sanitized) <= 200);
        Assert.EndsWith(".txt", sanitized, StringComparison.Ordinal);
    }

    [Fact]
    public void Caching_a_file_addresses_it_by_its_content_and_copies_it_once()
    {
        using var source = new TempFolder();
        using var cache = new TempFolder();
        var file = source.Write("hello.txt", "hello cache");

        var first = MediaCache.SaveFile(file, cache.Folder);
        var second = MediaCache.SaveFile(file, cache.Folder);

        // The same bytes name the same file, so caching twice is not two files.
        Assert.Equal(first, second);
        Assert.Equal("hello cache", System.IO.File.ReadAllText(first));
        Assert.True(MediaCache.IsInFolder(first, cache.Folder));
        Assert.Single(cache.Files());
    }

    [Fact]
    public void A_file_already_in_the_cache_is_returned_as_it_is()
    {
        using var cache = new TempFolder();
        var file = cache.Write("already.txt", "already here");

        Assert.Equal(Path.GetFullPath(file), MediaCache.SaveFile(file, cache.Folder));
        Assert.Single(cache.Files());
    }

    [Fact]
    public void Caching_bytes_and_streams_uses_the_same_address_as_caching_a_file()
    {
        using var source = new TempFolder();
        using var fromFile = new TempFolder();
        using var fromBytes = new TempFolder();
        using var fromStream = new TempFolder();

        var content = "the same bytes"u8.ToArray();
        var file = source.Write("same.txt", content);

        var a = MediaCache.SaveFile(file, fromFile.Folder);
        var b = MediaCache.SaveBytes(content, "same.txt", fromBytes.Folder);

        using var stream = new MemoryStream(content);
        var c = MediaCache.SaveStream(stream, "same.txt", fromStream.Folder);

        // The address is the content's hash, so all three name the same directory and the same
        // name — whichever way the bytes arrived.
        Assert.Equal(Hash(a), Hash(b));
        Assert.Equal(Hash(a), Hash(c));
        Assert.Equal(Path.GetFileName(a), Path.GetFileName(b));
        Assert.Equal(Path.GetFileName(a), Path.GetFileName(c));

        static string Hash(string path) => Path.GetFileName(Path.GetDirectoryName(path)!);
    }

    [Fact]
    public void Caching_a_file_that_is_not_there_says_so()
    {
        using var cache = new TempFolder();
        var missing = Path.Combine(cache.Folder, "missing.txt");

        // The path is inside the cache folder, so "it is already cached" is the answer a shortcut
        // would give. It is not cached: it is not there at all.
        Assert.Throws<FileNotFoundException>(() => MediaCache.SaveFile(missing, cache.Folder));
    }

    [Fact]
    public void A_name_carrying_separators_cannot_escape_the_cache_folder()
    {
        using var cache = new TempFolder();

        var cached = MediaCache.SaveBytes("content"u8, "../../evil.txt", cache.Folder);

        // The separators are gone rather than followed, so the file is a file with an odd name
        // inside the cache rather than a file beside it.
        Assert.True(MediaCache.IsInFolder(cached, cache.Folder));
        Assert.Equal("....evil.txt", Path.GetFileName(cached));
    }
}

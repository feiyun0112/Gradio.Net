using System.Text;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Kernel;

/// <summary>
/// A name made safe to use as a file name, held to the Python version's
/// <c>strip_invalid_filename_characters</c>.
/// </summary>
/// <remarks>
/// The cases are the ones that decide whether a name survives: what is removed, what is kept, and
/// what happens when removing leaves nothing. A label is written by a person, so the interesting
/// input is not a hostile one — it is "Why: this?" and "Café (best)".
/// </remarks>
public sealed class FileNamesTests
{
    [Theory]
    [InlineData("plain", "plain")]
    [InlineData("Café (best)", "Café (best)")]
    [InlineData("Why: this?", "Why this")]
    [InlineData("a/b\\c", "abc")]
    [InlineData("a*b|c", "abc")]
    [InlineData("<tag>", "tag")]
    [InlineData("quote\"d", "quoted")]
    [InlineData("a`b$c!d{e}f", "abcdef")]
    [InlineData("tab\there", "tabhere")]
    [InlineData("del\u007feted", "deleted")]
    [InlineData("trailing.  ", "trailing")]
    [InlineData("", "file")]
    [InlineData("\u0001\u0002", "file")]
    [InlineData("###", "###")]
    public void A_name_keeps_what_is_safe_and_loses_what_is_not(string name, string expected) =>
        Assert.Equal(expected, FileNames.StripInvalidCharacters(name));

    [Fact]
    public void An_extension_is_kept_because_a_file_without_one_is_a_file_nothing_can_open()
    {
        Assert.Equal("photo.png", FileNames.StripInvalidCharacters("photo.png"));
        Assert.Equal("a.tar.gz", FileNames.StripInvalidCharacters("a.tar.gz"));

        // A leading dot belongs to the name, so ".bashrc" has no extension to lose.
        Assert.Equal(".bashrc", FileNames.StripInvalidCharacters(".bashrc"));
    }

    [Fact]
    public void A_name_that_is_only_an_extension_becomes_a_file_rather_than_a_dotfile()
    {
        // ":.txt" would otherwise be ".txt", which a file system reads as a name with no extension
        // at all — a different file from the one the upload meant.
        Assert.Equal("file.txt", FileNames.StripInvalidCharacters(":.txt"));
    }

    [Fact]
    public void The_parent_directory_is_left_alone_so_that_it_can_be_refused()
    {
        // Kept as it is rather than repaired: whatever validates the path has to be able to see that
        // the name means "the directory above", and "file" would pass.
        Assert.Equal("..", FileNames.StripInvalidCharacters(".."));
    }

    [Theory]
    [InlineData("CON", "_CON")]
    [InlineData("con.txt", "_con.txt")]
    [InlineData("NUL.tar.gz", "_NUL.tar.gz")]
    [InlineData("COM1", "_COM1")]
    [InlineData("lpt9.log", "_lpt9.log")]
    public void A_name_windows_reserves_for_a_device_is_prefixed(string name, string expected)
    {
        // These are not files on NTFS: an upload that kept the name would fail on the one platform
        // where the name means something else.
        Assert.Equal(expected, FileNames.StripInvalidCharacters(name));
    }

    [Fact]
    public void A_name_that_is_too_long_is_cut_by_characters_rather_than_bytes()
    {
        var name = new string('é', 200) + ".txt";

        var shortened = FileNames.StripInvalidCharacters(name, maxBytes: 20);

        // "é" is two bytes in UTF-8: cutting by bytes would leave half a character behind, and the
        // name would no longer be valid text.
        Assert.True(Encoding.UTF8.GetByteCount(shortened) <= 20);
        Assert.EndsWith(".txt", shortened, StringComparison.Ordinal);
        Assert.All(shortened[..^4], character => Assert.Equal('é', character));
    }

    [Fact]
    public void An_extension_longer_than_the_whole_budget_gives_way_to_the_name()
    {
        var shortened = FileNames.StripInvalidCharacters("photo." + new string('x', 100), maxBytes: 10);

        Assert.True(Encoding.UTF8.GetByteCount(shortened) <= 10);
        Assert.StartsWith("file", shortened, StringComparison.Ordinal);
    }

    [Fact]
    public void A_negative_budget_is_refused()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => FileNames.StripInvalidCharacters("a", maxBytes: -1));
        Assert.Throws<ArgumentNullException>(() => FileNames.StripInvalidCharacters(null!));
    }
}

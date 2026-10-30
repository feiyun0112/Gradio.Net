using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Kernel;

/// <summary>
/// The policy an application declares about its own temporary files: how often to look, and how
/// old a file has to be before it goes.
/// </summary>
public sealed class DeleteCacheSettingsTests
{
    [Fact]
    public void A_frequency_and_an_age_are_kept_as_given()
    {
        var settings = new DeleteCacheSettings(TimeSpan.FromMinutes(1), TimeSpan.FromHours(1));

        Assert.Equal(TimeSpan.FromMinutes(1), settings.Frequency);
        Assert.Equal(TimeSpan.FromHours(1), settings.Age);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void A_frequency_that_is_not_positive_is_rejected(int seconds)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new DeleteCacheSettings(TimeSpan.FromSeconds(seconds), TimeSpan.FromHours(1)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void An_age_that_is_not_positive_is_rejected(int seconds)
    {
        // An age of zero would delete a file the moment it was written, which would make an upload
        // vanish before the run that reads it. The Python version's schedule silently does not start in that
        // case, which is the same mistake spelled quietly.
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new DeleteCacheSettings(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(seconds)));
    }

    [Fact]
    public void An_application_records_the_files_it_wrote_and_can_forget_them()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocks();

        var tracked = blocks.TrackTempFile("relative/path.txt");

        // Recorded as an absolute path, because that is what a sweep compares against and what a
        // process with a changing working directory cannot confuse.
        Assert.True(Path.IsPathFullyQualified(tracked));
        Assert.Equal([tracked], blocks.TempFiles);

        Assert.True(blocks.UntrackTempFile(tracked));
        Assert.False(blocks.UntrackTempFile(tracked));
        Assert.Empty(blocks.TempFiles);
    }

    [Fact]
    public void A_file_recorded_twice_is_recorded_once()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocks();

        var first = blocks.TrackTempFile("file.txt");
        var second = blocks.TrackTempFile(first);

        Assert.Equal(first, second);
        Assert.Single(blocks.TempFiles);
    }

    [Fact]
    public void An_application_that_says_nothing_keeps_its_files()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocks();

        Assert.Null(blocks.DeleteCache);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void An_empty_path_is_rejected(string path)
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocks();

        Assert.Throws<ArgumentException>(() => blocks.TrackTempFile(path));
        Assert.Throws<ArgumentException>(() => blocks.UntrackTempFile(path));
    }
}

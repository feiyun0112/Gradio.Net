using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// Temporary files as an operator sees them: the ones the application wrote are deleted on the
/// schedule it asked for, and when it stops it deletes the rest.
/// </summary>
/// <remarks>
/// The files are real files in a directory of the test's own, because the behaviour being asserted
/// is what happens to a file on disk — an abstraction over the file system would be a test of the
/// abstraction.
/// </remarks>
public sealed class TempFileCleanupTests
{
    [Fact]
    public async Task An_application_that_asked_for_cleanup_deletes_a_file_that_has_aged_out()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var directory = new TempFolder();
        var stale = directory.Write("stale.txt", age: TimeSpan.FromHours(1));

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            blocks.DeleteCache = new DeleteCacheSettings(
                TimeSpan.FromMilliseconds(50),
                TimeSpan.FromSeconds(1));

            blocks.TrackTempFile(stale);
        });

        Assert.True(
            await Eventually.IsTrueAsync(
                () => Task.FromResult(!System.IO.File.Exists(stale)),
                TimeSpan.FromSeconds(15),
                cancellationToken),
            "A file older than the configured age should have been deleted.");
    }

    [Fact]
    public async Task An_application_that_asked_for_cleanup_keeps_a_file_that_is_still_young()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var directory = new TempFolder();
        var fresh = directory.Write("fresh.txt", age: TimeSpan.Zero);

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            blocks.DeleteCache = new DeleteCacheSettings(
                TimeSpan.FromMilliseconds(50),
                TimeSpan.FromHours(1));

            blocks.TrackTempFile(fresh);
        });

        // Several sweeps happen in this window; none of them may take a file that has not aged out.
        await Task.Delay(TimeSpan.FromMilliseconds(300), cancellationToken);

        Assert.True(System.IO.File.Exists(fresh));
    }

    [Fact]
    public async Task An_application_that_did_not_ask_for_cleanup_keeps_its_files_while_it_runs()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var directory = new TempFolder();
        var file = directory.Write("kept.txt", age: TimeSpan.FromHours(1));

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            blocks.TrackTempFile(file);
        });

        await Task.Delay(TimeSpan.FromMilliseconds(300), cancellationToken);

        // No policy, no schedule: an application that says nothing keeps what it wrote, and the
        // operator cleans up.
        Assert.True(System.IO.File.Exists(file));
    }

    [Fact]
    public async Task A_file_the_application_forgot_is_not_deleted()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var directory = new TempFolder();
        var moved = directory.Write("moved.txt", age: TimeSpan.FromHours(1));

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            blocks.DeleteCache = new DeleteCacheSettings(
                TimeSpan.FromMilliseconds(50),
                TimeSpan.FromSeconds(1));

            blocks.TrackTempFile(moved);
            blocks.UntrackTempFile(moved);
        });

        await Task.Delay(TimeSpan.FromMilliseconds(300), cancellationToken);

        Assert.True(System.IO.File.Exists(moved));
    }

    [Fact]
    public async Task A_file_the_application_asked_to_keep_is_not_deleted()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var directory = new TempFolder();
        var kept = directory.Write("default-value.txt", age: TimeSpan.FromHours(1));

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            blocks.DeleteCache = new DeleteCacheSettings(
                TimeSpan.FromMilliseconds(50),
                TimeSpan.FromSeconds(1));

            // A file that is part of the application rather than a product of a run — a component's
            // default value — is recorded as temporary and as one to keep.
            blocks.TrackTempFile(kept);
            blocks.KeepTempFile(kept);
        });

        await Task.Delay(TimeSpan.FromMilliseconds(300), cancellationToken);

        Assert.True(System.IO.File.Exists(kept));
    }

    [Fact]
    public async Task Stopping_deletes_what_the_application_asked_to_have_cleaned_up()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var directory = new TempFolder();
        var file = directory.Write("on-stop.txt", age: TimeSpan.Zero);

        var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            blocks.DeleteCache = new DeleteCacheSettings(TimeSpan.FromHours(1), TimeSpan.FromHours(1));

            blocks.TrackTempFile(file);
        });

        Assert.True(System.IO.File.Exists(file));

        await host.DisposeAsync();

        // A sweep an hour away will never come, so stopping is the last chance to reclaim it.
        Assert.False(System.IO.File.Exists(file));
    }

    [Fact]
    public async Task Stopping_keeps_the_files_of_an_application_that_asked_for_nothing()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var directory = new TempFolder();
        var file = directory.Write("kept-on-stop.txt", age: TimeSpan.FromHours(1));

        var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            blocks.TrackTempFile(file);
        });

        await host.DisposeAsync();

        // No delete_cache, no cleanup — not even at shutdown. The Python version does not start the schedule
        // at all in that case, so a deployment that cleans up by other means is not surprised by a
        // deletion it never asked for.
        Assert.True(System.IO.File.Exists(file));
    }
}

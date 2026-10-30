using System.Globalization;
using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.RunHistory;

/// <summary>
/// What any run history store has to do, asserted against both of the ones this framework ships.
/// </summary>
/// <remarks>
/// <para>
/// A store is a contract, and the two implementations are answers to it: one keeps records in a
/// folder, the other in object storage. The tests are written once and run twice, because the point
/// of the abstraction is that an application can swap one for the other — and a swap is only safe if
/// the second one behaves the way the first one did.
/// </para>
/// <para>
/// The object store is exercised against a fake <see cref="IObjectStorage"/>: the layout it writes,
/// the order it writes in, and what it cleans up when a write fails are all assertions about the
/// calls it makes, which is exactly what a fake can observe and a network cannot.
/// </para>
/// </remarks>
public abstract class RunHistoryStoreContractTests
{
    /// <summary>What a test cancels with, so that a hung test can be stopped.</summary>
    protected static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    /// <summary>The store under test, with an empty container to write into.</summary>
    /// <returns>The store.</returns>
    protected abstract IRunHistoryStore CreateStore();

    /// <summary>A bucket that exists, for the store under test.</summary>
    /// <returns>The bucket's name.</returns>
    protected abstract string CreateBucket();

    [Fact]
    public async Task A_record_is_written_and_read_back()
    {
        var store = CreateStore();
        var bucket = await BucketAsync(store);
        var record = Record("echo", """{"data":["hello"]}""", """{"data":["HELLO"]}""");

        await store.SaveAsync(bucket, "app", record, NoAssets(), Cancellation);

        var records = await store.ListAsync(bucket, "app", limit: 10, Cancellation);

        var read = Assert.Single(records);

        Assert.Equal(record.RecordId, read.RecordId);
        Assert.Equal("echo", read.Endpoint);
        Assert.Equal(record.StartedAt, read.StartedAt);
        Assert.Equal(RunHistoryRecord.CurrentSchemaVersion, read.SchemaVersion);
        Assert.Equal("""{"data":["hello"]}""", read.Inputs!.ToJsonString());
        Assert.Equal("""{"data":["HELLO"]}""", read.Outputs!.ToJsonString());
    }

    [Fact]
    public async Task The_newest_records_come_first_and_the_limit_is_honoured()
    {
        var store = CreateStore();
        var bucket = await BucketAsync(store);

        await store.SaveAsync(bucket, "app", Record("echo", stamp: 1), NoAssets(), Cancellation);
        await store.SaveAsync(bucket, "app", Record("echo", stamp: 2), NoAssets(), Cancellation);
        await store.SaveAsync(bucket, "app", Record("echo", stamp: 3), NoAssets(), Cancellation);

        var newest = await store.ListAsync(bucket, "app", limit: 2, Cancellation);

        // The id starts with the time the run happened, so newest-first is the id's own order.
        Assert.Equal(2, newest.Count);
        Assert.True(string.CompareOrdinal(newest[0].RecordId, newest[1].RecordId) > 0);
    }

    [Fact]
    public async Task A_record_of_another_application_is_not_listed()
    {
        var store = CreateStore();
        var bucket = await BucketAsync(store);

        await store.SaveAsync(bucket, "app", Record("echo"), NoAssets(), Cancellation);

        Assert.Empty(await store.ListAsync(bucket, "another-app", limit: 10, Cancellation));
    }

    [Fact]
    public async Task An_asset_is_written_with_the_record_and_read_back()
    {
        var store = CreateStore();
        var bucket = await BucketAsync(store);
        var record = Record("echo");

        await store.SaveAsync(
            bucket,
            "app",
            record,
            new Dictionary<string, RunHistoryAsset>(StringComparer.Ordinal)
            {
                ["a001.txt"] = new() { Name = "a001.txt", Data = "the bytes"u8.ToArray() },
            },
            Cancellation);

        var asset = await store.ReadAssetAsync(
            bucket,
            "app",
            record.Endpoint,
            record.RecordId,
            "a001.txt",
            Cancellation);

        Assert.NotNull(asset);
        Assert.Equal("the bytes", System.Text.Encoding.UTF8.GetString(asset.Data!));
        Assert.Equal("text/plain", asset.ContentType);
    }

    [Fact]
    public async Task An_asset_that_was_never_written_is_a_miss()
    {
        var store = CreateStore();
        var bucket = await BucketAsync(store);

        Assert.Null(await store.ReadAssetAsync(bucket, "app", "echo", "record", "a001.txt", Cancellation));
    }

    [Theory]
    [InlineData("..", "0000000000000000001abcdef0")]
    [InlineData("echo", "../escape")]
    [InlineData("echo", "a/b")]
    public async Task A_name_that_is_not_a_name_is_refused(string endpoint, string recordId)
    {
        var store = CreateStore();
        var bucket = await BucketAsync(store);

        var failure = await Assert.ThrowsAsync<RunHistoryException>(() => store.SaveAsync(
            bucket,
            "app",
            new RunHistoryRecord { RecordId = recordId, Endpoint = endpoint },
            NoAssets(),
            Cancellation));

        Assert.Equal(422, failure.Status);
    }

    [Fact]
    public async Task A_bucket_that_is_not_a_name_is_refused()
    {
        var store = CreateStore();

        var failure = await Assert.ThrowsAsync<RunHistoryException>(
            () => store.ConnectAsync("../elsewhere", Cancellation));

        Assert.Equal(422, failure.Status);
    }

    [Fact]
    public async Task A_record_written_by_a_newer_release_is_skipped_rather_than_half_read()
    {
        var store = CreateStore();
        var bucket = await BucketAsync(store);

        await store.SaveAsync(bucket, "app", Record("echo"), NoAssets(), Cancellation);
        await SaveUnreadableAsync(store, bucket, "app");

        // The record this build cannot read is not listed, and the one beside it still is: one
        // unreadable file must not hide every run in the bucket.
        var records = await store.ListAsync(bucket, "app", limit: 10, Cancellation);

        Assert.Single(records);
    }

    /// <summary>Prepares a bucket and answers with its name.</summary>
    /// <param name="store">The store.</param>
    /// <returns>The bucket.</returns>
    private async Task<string> BucketAsync(IRunHistoryStore store)
    {
        var bucket = CreateBucket();

        await store.ConnectAsync(bucket, Cancellation);

        return bucket;
    }

    /// <summary>A record with a stamp that makes its id sort the way the test needs.</summary>
    /// <param name="endpoint">The endpoint.</param>
    /// <param name="inputs">The inputs, as JSON.</param>
    /// <param name="outputs">The outputs, as JSON.</param>
    /// <param name="stamp">What the id sorts by, or zero for a fresh one.</param>
    /// <returns>The record.</returns>
    private static RunHistoryRecord Record(
        string endpoint,
        string? inputs = null,
        string? outputs = null,
        int stamp = 0) =>
        new()
        {
            RecordId = stamp == 0
                ? System.DateTime.UtcNow.Ticks.ToString("D19", CultureInfo.InvariantCulture) + "abcdef01"
                : stamp.ToString("D19", CultureInfo.InvariantCulture) + "abcdef01",
            Endpoint = endpoint,
            Inputs = inputs is null ? null : JsonNode.Parse(inputs),
            Outputs = outputs is null ? null : JsonNode.Parse(outputs),
            StartedAt = "2026-01-01T00:00:00.000Z",
        };

    private static Dictionary<string, RunHistoryAsset> NoAssets() => new(StringComparer.Ordinal);

    /// <summary>Writes a record this build cannot read, through the store's own storage.</summary>
    /// <param name="store">The store.</param>
    /// <param name="bucket">The bucket.</param>
    /// <param name="appId">The application.</param>
    /// <returns>A task that completes once it is written.</returns>
    protected abstract Task SaveUnreadableAsync(IRunHistoryStore store, string bucket, string appId);
}

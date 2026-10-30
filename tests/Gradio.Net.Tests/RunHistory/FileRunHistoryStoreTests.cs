using Gradio.Net;
using Gradio.Net.Tests.Server;
using Xunit;

namespace Gradio.Net.Tests.RunHistory;

/// <summary>
/// The store an application gets without asking: records in a folder, one file each.
/// </summary>
public sealed class FileRunHistoryStoreTests : RunHistoryStoreContractTests, IDisposable
{
    private readonly TempFolder _folder = new();

    protected override IRunHistoryStore CreateStore() => new FileRunHistoryStore(_folder.Folder);

    protected override string CreateBucket() => "bucket";

    [Fact]
    public async Task A_bucket_is_a_directory_the_store_makes()
    {
        var store = new FileRunHistoryStore(_folder.Folder);

        Assert.Empty(await store.BucketsAsync(Cancellation));

        await store.ConnectAsync("first", Cancellation);
        await store.ConnectAsync("second", Cancellation);

        Assert.Equal(["first", "second"], await store.BucketsAsync(Cancellation));
        Assert.True(Directory.Exists(Path.Combine(_folder.Folder, "first")));
    }

    [Fact]
    public async Task The_layout_is_the_one_a_bucket_can_be_copied_with()
    {
        var store = new FileRunHistoryStore(_folder.Folder);
        var record = new RunHistoryRecord
        {
            RecordId = "0000000000000000001abcdef0",
            Endpoint = "echo",
            Inputs = System.Text.Json.Nodes.JsonNode.Parse("""["a"]"""),
        };

        await store.SaveAsync(
            "bucket",
            "app",
            record,
            new Dictionary<string, RunHistoryAsset>(StringComparer.Ordinal)
            {
                ["a001.txt"] = new() { Name = "a001.txt", Data = "x"u8.ToArray() },
            },
            Cancellation);

        // The Python version's layout, which is what makes a bucket readable by anything that reads in the Python version's:
        // the record under runs/, its files under assets/, both scoped by application and endpoint.
        Assert.True(System.IO.File.Exists(Path.Combine(
            _folder.Folder, "bucket", "runs", "app", "echo", "0000000000000000001abcdef0.json")));
        Assert.True(System.IO.File.Exists(Path.Combine(
            _folder.Folder, "bucket", "assets", "app", "echo", "0000000000000000001abcdef0", "a001.txt")));
    }

    [Fact]
    public async Task The_record_is_the_json_a_reader_expects()
    {
        var store = new FileRunHistoryStore(_folder.Folder);

        await store.SaveAsync(
            "bucket",
            "app",
            new RunHistoryRecord
            {
                RecordId = "0000000000000000001abcdef0",
                Endpoint = "echo",
                Inputs = System.Text.Json.Nodes.JsonNode.Parse("""["a"]"""),
                Outputs = System.Text.Json.Nodes.JsonNode.Parse("""["A"]"""),
                StartedAt = "2026-01-01T00:00:00.000Z",
            },
            new Dictionary<string, RunHistoryAsset>(StringComparer.Ordinal),
            Cancellation);

        var written = await System.IO.File.ReadAllTextAsync(Path.Combine(
            _folder.Folder, "bucket", "runs", "app", "echo", "0000000000000000001abcdef0.json"), Cancellation);

        // Field for field, in the Python version's order: the document is what a page reads.
        Assert.Equal(
            """{"record_id":"0000000000000000001abcdef0","endpoint":"echo","inputs":["a"],"outputs":["A"],"started_at":"2026-01-01T00:00:00.000Z","schema_version":2}""",
            written);
    }

    [Fact]
    public async Task An_asset_written_from_a_path_is_copied_rather_than_referenced()
    {
        var store = new FileRunHistoryStore(_folder.Folder);
        var source = Path.Combine(_folder.Folder, "source.txt");
        await System.IO.File.WriteAllTextAsync(source, "the bytes", Cancellation);

        var record = new RunHistoryRecord { RecordId = "0000000000000000001abcdef0", Endpoint = "echo" };

        await store.SaveAsync(
            "bucket",
            "app",
            record,
            new Dictionary<string, RunHistoryAsset>(StringComparer.Ordinal)
            {
                ["a001.txt"] = new() { Name = "a001.txt", Path = source },
            },
            Cancellation);

        // A record that kept the path would be a record that stops working when the file does.
        System.IO.File.Delete(source);

        var asset = await store.ReadAssetAsync("bucket", "app", "echo", record.RecordId, "a001.txt", Cancellation);

        Assert.Equal("the bytes", System.Text.Encoding.UTF8.GetString(asset!.Data!));
    }

    protected override async Task SaveUnreadableAsync(IRunHistoryStore store, string bucket, string appId)
    {
        var folder = Path.Combine(_folder.Folder, bucket, "runs", appId, "echo");
        Directory.CreateDirectory(folder);

        await System.IO.File.WriteAllTextAsync(
            Path.Combine(folder, "9999999999999999999abcdef0.json"),
            """{"record_id":"9999999999999999999abcdef0","endpoint":"echo","schema_version":99}""",
            Cancellation);
    }

    public void Dispose() => _folder.Dispose();
}

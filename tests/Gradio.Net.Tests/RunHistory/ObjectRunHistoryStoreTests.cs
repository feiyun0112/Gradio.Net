using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.RunHistory;

/// <summary>
/// The external store, held to the same contract as the local one — and to the things only it does:
/// writing a record's files before the record, and cleaning up when a write fails.
/// </summary>
/// <remarks>
/// The storage is a fake, which is the point: what is being verified is the store's behaviour — the
/// keys it uses, the order it writes in, what it removes when it cannot finish — and none of that
/// needs a service to be reached.
/// </remarks>
public sealed class ObjectRunHistoryStoreTests : RunHistoryStoreContractTests
{
    private readonly FakeObjectStorage _storage = new();

    protected override IRunHistoryStore CreateStore() => new ObjectRunHistoryStore(_storage);

    protected override string CreateBucket() => "bucket";

    [Fact]
    public async Task The_record_is_written_last_so_that_what_is_there_can_be_read()
    {
        var store = new ObjectRunHistoryStore(_storage);
        var record = new RunHistoryRecord { RecordId = "0000000000000000001abcdef0", Endpoint = "echo" };

        await store.SaveAsync(
            "bucket",
            "app",
            record,
            new Dictionary<string, RunHistoryAsset>(StringComparer.Ordinal)
            {
                ["a001.txt"] = new() { Name = "a001.txt", Data = "x"u8.ToArray() },
            },
            Cancellation);

        Assert.Equal(
            [
                "assets/app/echo/0000000000000000001abcdef0/a001.txt",
                "runs/app/echo/0000000000000000001abcdef0.json",
            ],
            _storage.Written);
    }

    [Fact]
    public async Task A_record_that_could_not_be_written_takes_its_files_with_it()
    {
        var store = new ObjectRunHistoryStore(_storage);
        _storage.RefuseWrites("runs/");

        var failure = await Assert.ThrowsAsync<RunHistoryException>(() => store.SaveAsync(
            "bucket",
            "app",
            new RunHistoryRecord { RecordId = "0000000000000000001abcdef0", Endpoint = "echo" },
            new Dictionary<string, RunHistoryAsset>(StringComparer.Ordinal)
            {
                ["a001.txt"] = new() { Name = "a001.txt", Data = "x"u8.ToArray() },
            },
            Cancellation));

        Assert.Equal(502, failure.Status);

        // An object store is billed by what it holds, and a half-written run is a run nobody can
        // read: the files that were written for it are removed.
        Assert.Empty(_storage.Keys);
    }

    [Fact]
    public async Task A_bucket_is_checked_rather_than_made()
    {
        var store = new ObjectRunHistoryStore(_storage);

        // There is no container to create through this port, so connecting is a read: it either
        // works or it does not, which is what a client about to write wants to know.
        await store.ConnectAsync("bucket", Cancellation);

        Assert.Equal(["bucket"], _storage.Listed);
    }

    protected override Task SaveUnreadableAsync(IRunHistoryStore store, string bucket, string appId) =>
        _storage.PutAsync(
            bucket,
            $"runs/{appId}/echo/9999999999999999999abcdef0.json",
            """{"record_id":"9999999999999999999abcdef0","endpoint":"echo","schema_version":99}"""u8.ToArray(),
            "application/json",
            Cancellation);

    /// <summary>Object storage held in a dictionary.</summary>
    private sealed class FakeObjectStorage : IObjectStorage
    {
        private readonly Dictionary<(string Bucket, string Key), byte[]> _objects = [];
        private readonly List<string> _written = [];
        private readonly List<string> _listed = [];
        private string? _refusedPrefix;

        internal IReadOnlyList<string> Written => _written;

        internal IReadOnlyList<string> Listed => _listed;

        internal IReadOnlyCollection<string> Keys => [.. _objects.Keys.Select(key => key.Key)];

        /// <summary>Refuses every write whose key starts with a prefix, as a failing service would.</summary>
        /// <param name="prefix">The prefix.</param>
        internal void RefuseWrites(string prefix) => _refusedPrefix = prefix;

        public Task PutAsync(
            string bucket,
            string key,
            ReadOnlyMemory<byte> content,
            string contentType,
            CancellationToken cancellationToken = default)
        {
            if (_refusedPrefix is not null && key.StartsWith(_refusedPrefix, StringComparison.Ordinal))
            {
                throw new HttpRequestException("the service refused the write");
            }

            _objects[(bucket, key)] = content.ToArray();
            _written.Add(key);

            return Task.CompletedTask;
        }

        public Task<byte[]?> GetAsync(string bucket, string key, CancellationToken cancellationToken = default) =>
            Task.FromResult(_objects.TryGetValue((bucket, key), out var content) ? content : null);

        public Task<IReadOnlyList<string>> ListAsync(
            string bucket,
            string prefix,
            CancellationToken cancellationToken = default)
        {
            _listed.Add(bucket);

            IReadOnlyList<string> keys = [.. _objects.Keys
                .Where(entry => entry.Bucket == bucket && entry.Key.StartsWith(prefix, StringComparison.Ordinal))
                .Select(entry => entry.Key)];

            return Task.FromResult(keys);
        }

        public Task DeleteAsync(string bucket, string key, CancellationToken cancellationToken = default)
        {
            _objects.Remove((bucket, key));

            return Task.CompletedTask;
        }
    }
}

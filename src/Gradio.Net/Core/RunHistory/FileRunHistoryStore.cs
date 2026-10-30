using System.Text;
namespace Gradio.Net;
/// <summary>
/// Run history kept in a folder: one directory per bucket, one file per record, one file per asset.
/// </summary>
/// <remarks>
/// <para>
/// The local implementation, and the one an application gets without asking. It lays records out the
/// way the object store lays them out — <c>runs/{app}/{endpoint}/{record}.json</c> and
/// <c>assets/{app}/{endpoint}/{record}/{name}</c> under the bucket — so that the two can be swapped
/// without the reader changing, and so that a bucket can be copied to a server and read there.
/// </para>
/// <para>
/// Every write goes through the same two steps as the object store's: assets first, record last. A
/// record is what says a run happened, so a record whose files are missing would be a run that cannot
/// be replayed; files without a record are simply unreferenced.
/// </para>
/// </remarks>
public sealed class FileRunHistoryStore : IRunHistoryStore
{
    private static readonly UTF8Encoding _utf8 = new(encoderShouldEmitUTF8Identifier: false);

    /// <summary>Creates a store over a folder.</summary>
    /// <param name="rootFolder">The folder every bucket lives in.</param>
    /// <exception cref="ArgumentException"><paramref name="rootFolder"/> is empty.</exception>
    public FileRunHistoryStore(string rootFolder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rootFolder);

        RootFolder = Path.GetFullPath(rootFolder);
    }

    /// <summary>Creates the store an application gets when it configures nothing.</summary>
    public FileRunHistoryStore()
        : this(new RunHistoryOptions().RootFolder)
    {
    }

    /// <summary>The folder every bucket lives in.</summary>
    public string RootFolder { get; }

    /// <inheritdoc />
    public Task ConnectAsync(string bucket, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Directory.CreateDirectory(BucketFolder(bucket));

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<string>> BucketsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!Directory.Exists(RootFolder))
        {
            return Task.FromResult<IReadOnlyList<string>>([]);
        }

        // A bucket is a directory this store made. One that holds nothing is still a bucket: it was
        // connected, and a client listing them is asking what it can write to.
        IReadOnlyList<string> buckets = [.. Directory.GetDirectories(RootFolder)
            .Select(Path.GetFileName)
            .Where(name => !string.IsNullOrEmpty(name))
            .Select(name => name!)
            .Order(StringComparer.Ordinal)];

        return Task.FromResult(buckets);
    }

    /// <inheritdoc />
    public async Task SaveAsync(
        string bucket,
        string appId,
        RunHistoryRecord record,
        IReadOnlyDictionary<string, RunHistoryAsset> assets,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(record);
        ArgumentNullException.ThrowIfNull(assets);

        RunHistoryKeys.Segment(appId, "application id");
        RunHistoryKeys.Segment(record.Endpoint, "endpoint");
        RunHistoryKeys.RecordId(record.RecordId);

        var folder = BucketFolder(bucket);

        try
        {
            foreach (var (name, asset) in assets)
            {
                RunHistoryKeys.Segment(name, "asset name");

                var path = PathOf(folder, RunHistoryKeys.Asset(appId, record.Endpoint, record.RecordId, name));

                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                await System.IO.File.WriteAllBytesAsync(path, asset.Read(), cancellationToken).ConfigureAwait(false);
            }

            var recordPath = PathOf(folder, RunHistoryKeys.Record(appId, record.Endpoint, record.RecordId));

            Directory.CreateDirectory(Path.GetDirectoryName(recordPath)!);

            await System.IO.File.WriteAllTextAsync(
                recordPath,
                JsonText.Serialize(record.ToJson()),
                _utf8,
                cancellationToken).ConfigureAwait(false);
        }
        catch (IOException failure)
        {
            throw new RunHistoryException("The run could not be written to the history folder.", failure, 502);
        }
        catch (UnauthorizedAccessException failure)
        {
            throw new RunHistoryException("The run could not be written to the history folder.", failure, 502);
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RunHistoryRecord>> ListAsync(
        string bucket,
        string appId,
        int limit,
        CancellationToken cancellationToken = default)
    {
        RunHistoryKeys.Segment(appId, "application id");

        if (limit <= 0)
        {
            return [];
        }

        var prefix = PathOf(BucketFolder(bucket), RunHistoryKeys.RecordsOf(appId));

        if (!Directory.Exists(prefix))
        {
            return [];
        }

        // Newest first is the file name's order, because the id starts with the time the run
        // happened: no record has to be opened to sort them.
        var files = Directory
            .GetFiles(prefix, "*.json", SearchOption.AllDirectories)
            .OrderByDescending(path => Path.GetFileName(path), StringComparer.Ordinal)
            .Take(limit);

        var records = new List<RunHistoryRecord>();

        foreach (var file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var document = System.Text.Json.Nodes.JsonNode.Parse(
                    await System.IO.File.ReadAllTextAsync(file, _utf8, cancellationToken).ConfigureAwait(false));

                records.Add(RunHistoryRecord.FromJson(document));
            }
            catch (Exception failure) when (failure is System.Text.Json.JsonException or FormatException or IOException)
            {
                // A record this build cannot read is skipped rather than failing the listing: one
                // unreadable file must not hide every run beside it.
            }
        }

        return records;
    }

    /// <inheritdoc />
    public async Task<RunHistoryAsset?> ReadAssetAsync(
        string bucket,
        string appId,
        string endpoint,
        string recordId,
        string assetId,
        CancellationToken cancellationToken = default)
    {
        RunHistoryKeys.Segment(appId, "application id");
        RunHistoryKeys.Segment(endpoint, "endpoint");
        RunHistoryKeys.RecordId(recordId);
        RunHistoryKeys.Segment(assetId, "asset name");

        var path = PathOf(BucketFolder(bucket), RunHistoryKeys.Asset(appId, endpoint, recordId, assetId));

        if (!System.IO.File.Exists(path))
        {
            return null;
        }

        try
        {
            return new RunHistoryAsset
            {
                Name = assetId,
                Data = await System.IO.File.ReadAllBytesAsync(path, cancellationToken).ConfigureAwait(false),
                ContentType = RunHistoryContentTypes.Of(assetId),
            };
        }
        catch (IOException failure)
        {
            throw new RunHistoryException("The asset could not be read.", failure, 502);
        }
    }

    /// <summary>The folder a bucket lives in.</summary>
    /// <param name="bucket">The bucket.</param>
    /// <returns>The folder's absolute path.</returns>
    /// <exception cref="RunHistoryException">The bucket is not a name.</exception>
    /// <remarks>
    /// A bucket is a directory here, so it is one segment: the Python version's <c>owner/name</c>
    /// ids describe a container in a service, and a folder has no owner to put in front of it.
    /// </remarks>
    private string BucketFolder(string bucket)
    {
        RunHistoryKeys.Segment(bucket, "bucket id");

        return Path.Combine(RootFolder, bucket);
    }

    /// <summary>A key turned into a path inside a bucket, refusing to leave it.</summary>
    /// <param name="folder">The bucket's folder.</param>
    /// <param name="key">The key.</param>
    /// <returns>The path.</returns>
    /// <exception cref="RunHistoryException">The key would leave the folder.</exception>
    /// <remarks>
    /// Every part of a key is checked before it gets here, so this is the second line rather than the
    /// first: a bug in the composition of a key must not be able to read or write outside the
    /// bucket.
    /// </remarks>
    private static string PathOf(string folder, string key)
    {
        var path = Path.GetFullPath(Path.Combine(folder, key.Replace('/', Path.DirectorySeparatorChar)));

        if (!MediaCache.IsInFolder(path, folder))
        {
            throw new RunHistoryException("The run would be filed outside the bucket.", 400);
        }

        return path;
    }
}

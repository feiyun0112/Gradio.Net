namespace Gradio.Net;
/// <summary>
/// Run history kept in object storage: the same layout as the file store, addressed by key.
/// </summary>
/// <remarks>
/// <para>
/// The external implementation. It is written against <see cref="IObjectStorage"/> rather than
/// against any one provider, because the interesting part — which key a record goes under, what order
/// the assets and the record are written in, and what happens when a write fails — is the same
/// whichever service holds the bytes. A host supplies the port; the contract tests supply a fake and
/// prove the behaviour without a network.
/// </para>
/// <para>
/// The Python version writes to a Hugging Face bucket and this store does not: what it keeps is the
/// Python version's layout in whatever container the port addresses. That is the deliberate
/// difference, and it is what lets the framework have an external option without depending on one.
/// </para>
/// </remarks>
public sealed class ObjectRunHistoryStore : IRunHistoryStore
{
    private readonly IObjectStorage _storage;

    /// <summary>Creates the store.</summary>
    /// <param name="storage">The storage it writes through.</param>
    /// <exception cref="ArgumentNullException"><paramref name="storage"/> is <see langword="null"/>.</exception>
    public ObjectRunHistoryStore(IObjectStorage storage)
    {
        ArgumentNullException.ThrowIfNull(storage);

        _storage = storage;
    }

    /// <inheritdoc />
    /// <remarks>
    /// Nothing is created: a container is either there or it is not, and this store has no way to make
    /// one. Connecting is therefore a check, which is what a client that is about to write wants to
    /// know.
    /// </remarks>
    public async Task ConnectAsync(string bucket, CancellationToken cancellationToken = default)
    {
        RunHistoryKeys.Bucket(bucket);

        await _storage.ListAsync(bucket, string.Empty, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    /// <remarks>
    /// The containers are the ones the storage already has: an object store has no way to tell a
    /// container this application made from one somebody else did.
    /// </remarks>
    public Task<IReadOnlyList<string>> BucketsAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<string>>([]);

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

        var written = new List<string>();

        try
        {
            foreach (var (name, asset) in assets)
            {
                RunHistoryKeys.Segment(name, "asset name");

                var key = RunHistoryKeys.Asset(appId, record.Endpoint, record.RecordId, name);

                await _storage.PutAsync(
                    bucket,
                    key,
                    asset.Read(),
                    asset.ContentType ?? "application/octet-stream",
                    cancellationToken).ConfigureAwait(false);

                written.Add(key);
            }

            // The record is written last, and is the commit: a reader that finds it finds files
            // behind it.
            await _storage.PutAsync(
                bucket,
                RunHistoryKeys.Record(appId, record.Endpoint, record.RecordId),
                System.Text.Encoding.UTF8.GetBytes(JsonText.Serialize(record.ToJson())),
                "application/json",
                cancellationToken).ConfigureAwait(false);
        }
        catch (Exception failure) when (failure is not RunHistoryException and not OperationCanceledException)
        {
            // What was written for a record that did not make it is cleaned up: an object store is
            // billed by what it holds, and a half-written run is a run nobody can read.
            foreach (var key in written)
            {
                try
                {
                    await _storage.DeleteAsync(bucket, key, CancellationToken.None).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    // There is nothing left to do about it, and the failure that matters is the one
                    // being reported.
                }
            }

            throw new RunHistoryException("The run could not be written to the bucket.", failure, 502);
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

        var keys = await _storage
            .ListAsync(bucket, RunHistoryKeys.RecordsOf(appId), cancellationToken)
            .ConfigureAwait(false);

        var newest = keys
            .Where(key => key.EndsWith(".json", StringComparison.Ordinal))
            .OrderByDescending(key => key, StringComparer.Ordinal)
            .Take(limit);

        var records = new List<RunHistoryRecord>();

        foreach (var key in newest)
        {
            var document = await _storage.GetAsync(bucket, key, cancellationToken).ConfigureAwait(false);

            if (document is null)
            {
                continue;
            }

            try
            {
                records.Add(RunHistoryRecord.FromJson(
                    System.Text.Json.Nodes.JsonNode.Parse(System.Text.Encoding.UTF8.GetString(document))));
            }
            catch (Exception failure) when (failure is System.Text.Json.JsonException or FormatException)
            {
                // One unreadable record must not hide every run beside it.
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

        var data = await _storage
            .GetAsync(bucket, RunHistoryKeys.Asset(appId, endpoint, recordId, assetId), cancellationToken)
            .ConfigureAwait(false);

        return data is null
            ? null
            : new RunHistoryAsset
            {
                Name = assetId,
                Data = data,
                ContentType = ContentTypeOf(assetId),
            };
    }

    /// <summary>The media type an asset's name implies.</summary>
    /// <param name="name">The asset's name.</param>
    /// <returns>The media type, or the one a client should be told for an unknown file.</returns>
    private static string ContentTypeOf(string name) => RunHistoryContentTypes.Of(name);
}

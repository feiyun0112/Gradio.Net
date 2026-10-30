namespace Gradio.Net;
/// <summary>
/// Where runs are kept.
/// </summary>
/// <remarks>
/// <para>
/// The Python version keeps run history in a Hugging Face bucket, which is one answer to a question
/// that has several: an application may want its runs on disk beside it, in object storage it
/// already pays for, or nowhere at all. The framework's part is the recording — what a record is,
/// which runs are worth recording, and how a file value becomes an asset — and this interface is
/// where that stops.
/// </para>
/// <para>
/// A store is scoped by a bucket and an application id, because that is what the Python version's
/// is scoped by: the bucket is whose storage it is, and the id is which application's runs are
/// being filed. A local implementation reads the bucket as a directory; a remote one reads it as a
/// container.
/// </para>
/// </remarks>
public interface IRunHistoryStore
{
    /// <summary>Prepares a bucket, creating it when it does not exist.</summary>
    /// <param name="bucket">The bucket.</param>
    /// <param name="cancellationToken">Cancels the work.</param>
    /// <returns>A task that completes once the bucket can be written to.</returns>
    /// <exception cref="RunHistoryException">The bucket cannot be prepared.</exception>
    Task ConnectAsync(string bucket, CancellationToken cancellationToken = default);

    /// <summary>The buckets this store can write to.</summary>
    /// <param name="cancellationToken">Cancels the work.</param>
    /// <returns>The bucket names, or none for a store whose containers are not its to enumerate.</returns>
    /// <exception cref="RunHistoryException">The buckets cannot be listed.</exception>
    /// <remarks>
    /// A folder can say which directories it holds; an object store cannot say which containers
    /// somebody else made. A store that cannot answer answers nothing, which is a different thing
    /// from failing — and a client that is choosing a bucket is choosing among what it was given.
    /// </remarks>
    Task<IReadOnlyList<string>> BucketsAsync(CancellationToken cancellationToken = default);

    /// <summary>Keeps one run, with the files it carries.</summary>
    /// <param name="bucket">The bucket.</param>
    /// <param name="appId">The application whose runs these are.</param>
    /// <param name="record">The record.</param>
    /// <param name="assets">The files the record refers to, by the names it uses.</param>
    /// <param name="cancellationToken">Cancels the work.</param>
    /// <returns>A task that completes once the record is kept.</returns>
    /// <exception cref="RunHistoryException">The record could not be kept.</exception>
    /// <remarks>
    /// The assets are written before the record, because the record is what says a run happened: a
    /// record whose files are missing describes a run that cannot be replayed, while files without a
    /// record are simply unreferenced.
    /// </remarks>
    Task SaveAsync(
        string bucket,
        string appId,
        RunHistoryRecord record,
        IReadOnlyDictionary<string, RunHistoryAsset> assets,
        CancellationToken cancellationToken = default);

    /// <summary>The newest runs of one application.</summary>
    /// <param name="bucket">The bucket.</param>
    /// <param name="appId">The application.</param>
    /// <param name="limit">The most records to answer with.</param>
    /// <param name="cancellationToken">Cancels the work.</param>
    /// <returns>The records, newest first.</returns>
    /// <exception cref="RunHistoryException">The records could not be read.</exception>
    Task<IReadOnlyList<RunHistoryRecord>> ListAsync(
        string bucket,
        string appId,
        int limit,
        CancellationToken cancellationToken = default);

    /// <summary>One file a record carries.</summary>
    /// <param name="bucket">The bucket.</param>
    /// <param name="appId">The application.</param>
    /// <param name="endpoint">The endpoint the record is filed under.</param>
    /// <param name="recordId">The record.</param>
    /// <param name="assetId">The asset's name.</param>
    /// <param name="cancellationToken">Cancels the work.</param>
    /// <returns>The asset with its bytes, or <see langword="null"/> when it is not there.</returns>
    /// <exception cref="RunHistoryException">The asset could not be read.</exception>
    Task<RunHistoryAsset?> ReadAssetAsync(
        string bucket,
        string appId,
        string endpoint,
        string recordId,
        string assetId,
        CancellationToken cancellationToken = default);
}

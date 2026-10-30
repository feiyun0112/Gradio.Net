namespace Gradio.Net;
/// <summary>
/// Object storage, as much of it as run history needs.
/// </summary>
/// <remarks>
/// <para>
/// The port the external run history store is written against. It exists so that the store can be
/// verified without a network and without an account: the layout it produces, the order it writes in,
/// and what it does when a write fails are all assertions about this interface's calls, and a fake
/// implementation is enough to make them.
/// </para>
/// <para>
/// It is deliberately smaller than any object store's API — put, get, list, delete, scoped by a
/// container — because a bigger port would be a second API to keep in step with the first, and
/// nothing in run history needs the rest.
/// </para>
/// </remarks>
public interface IObjectStorage
{
    /// <summary>Writes an object.</summary>
    /// <param name="bucket">The container.</param>
    /// <param name="key">The object's key, which may contain slashes.</param>
    /// <param name="content">The bytes.</param>
    /// <param name="contentType">The media type.</param>
    /// <param name="cancellationToken">Cancels the work.</param>
    /// <returns>A task that completes once the object is written.</returns>
    Task PutAsync(
        string bucket,
        string key,
        ReadOnlyMemory<byte> content,
        string contentType,
        CancellationToken cancellationToken = default);

    /// <summary>Reads an object.</summary>
    /// <param name="bucket">The container.</param>
    /// <param name="key">The object's key.</param>
    /// <param name="cancellationToken">Cancels the work.</param>
    /// <returns>The bytes, or <see langword="null"/> when there is no such object.</returns>
    Task<byte[]?> GetAsync(string bucket, string key, CancellationToken cancellationToken = default);

    /// <summary>Lists the objects under a prefix.</summary>
    /// <param name="bucket">The container.</param>
    /// <param name="prefix">The prefix.</param>
    /// <param name="cancellationToken">Cancels the work.</param>
    /// <returns>The keys, in whatever order the store keeps them.</returns>
    Task<IReadOnlyList<string>> ListAsync(
        string bucket,
        string prefix,
        CancellationToken cancellationToken = default);

    /// <summary>Removes an object.</summary>
    /// <param name="bucket">The container.</param>
    /// <param name="key">The object's key.</param>
    /// <param name="cancellationToken">Cancels the work.</param>
    /// <returns>A task that completes once the object is gone, or was never there.</returns>
    Task DeleteAsync(string bucket, string key, CancellationToken cancellationToken = default);
}

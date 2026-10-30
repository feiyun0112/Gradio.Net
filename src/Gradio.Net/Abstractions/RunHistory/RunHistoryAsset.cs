namespace Gradio.Net;
/// <summary>
/// One file a record carries: the bytes, or where to read them.
/// </summary>
/// <remarks>
/// <para>
/// A run's inputs and outputs can name files, and a record that kept the path would be a record that
/// stops working when the file does. So the file travels with the record: the store is handed the
/// bytes, or a path it is allowed to read them from, under the name the record refers to it by.
/// </para>
/// <para>
/// The same type is what a store answers with when a client asks for an asset back, which is why
/// <see cref="Data"/> is filled in on the way out.
/// </para>
/// </remarks>
public sealed record RunHistoryAsset
{
    /// <summary>The name the record refers to this asset by.</summary>
    /// <remarks>
    /// A name rather than a path: it is what the record holds in place of the file, and what a client
    /// asks for. It never carries a directory separator, which is what makes it safe to join onto a
    /// store's own layout.
    /// </remarks>
    public required string Name { get; init; }

    /// <summary>The bytes, when they are in hand.</summary>
    public byte[]? Data { get; init; }

    /// <summary>The file to read the bytes from, when they are not.</summary>
    public string? Path { get; init; }

    /// <summary>The media type, when it is known.</summary>
    public string? ContentType { get; init; }

    /// <summary>The bytes, from wherever this asset says they are.</summary>
    /// <returns>The bytes, or none when the asset names neither.</returns>
    /// <remarks>
    /// Shared by the stores because "the bytes are in hand, or in a file" is the asset's own rule
    /// rather than a store's: a second copy of it would be a second answer to what an empty asset is.
    /// </remarks>
    internal byte[] Read() => Data ?? (Path is { Length: > 0 } path ? System.IO.File.ReadAllBytes(path) : []);
}

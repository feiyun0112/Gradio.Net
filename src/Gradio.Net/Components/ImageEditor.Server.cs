using System.Collections.Concurrent;
using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// The image editor's own server function, which is how the images it edits reach the server.
/// </summary>
/// <remarks>
/// <para>
/// An editor works on canvases the browser draws, and a canvas is not a file: what the frontend has
/// is a blob, and the only way to hand a blob over is to send it. The Python version uploads it out
/// of band — <c>accept_blobs</c> — and keeps it under an id the value then names, so that the
/// request which runs the handler carries the id rather than the image.
/// </para>
/// <para>
/// That is why the blobs are held here rather than written and forgotten: the value arrives later,
/// in a different request, and the editor is the only thing that knows what the id means. The entry
/// is removed as it is read, which is the Python version's rule — a blob is used once.
/// </para>
/// </remarks>
public partial class ImageEditor
{
    internal ImageEditor()
    {
    }

    /// <summary>The blobs the browser has sent and the value has not claimed yet.</summary>
    private readonly ConcurrentDictionary<string, EditorBlobs> _blobs = new(StringComparer.Ordinal);

    /// <inheritdoc />
    protected internal override IReadOnlyList<string> ServerFunctionNames => ["accept_blobs"];

    /// <summary>Calls one of this component's server functions.</summary>
    /// <param name="request">The function's name, and what the browser sent with it.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns><see langword="null"/>: accepting a blob answers nothing.</returns>
    /// <exception cref="ArgumentException">The call names no blob id, or carries no file.</exception>
    protected internal override ValueTask<JsonNode?> InvokeServerFunctionAsync(
        ComponentServerCall request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.FunctionName != "accept_blobs")
        {
            return base.InvokeServerFunctionAsync(request, cancellationToken);
        }

        var fieldsNode = request.Data;
        var id = fieldsNode is JsonObject fields ? JsonValues.Text(fields["id"]) : null;

        if (id is not { Length: > 0 })
        {
            throw new ArgumentException("A blob needs an id to be stored under.", nameof(request));
        }

        if (request.Files.Count == 0)
        {
            throw new ArgumentException("A blob needs a file.", nameof(request));
        }

        var type = fieldsNode is JsonObject kind ? JsonValues.Text(kind["type"]) : null;
        var index = fieldsNode is JsonObject slot ? JsonValues.Text(slot["index"]) : null;
        var bytes = request.Files[0].Bytes;

        var blobs = _blobs.GetOrAdd(id, _ => new EditorBlobs());

        switch (type)
        {
            case "background":
                blobs.Background = bytes;
                break;

            case "composite":
                blobs.Composite = bytes;
                break;

            case "layer" when int.TryParse(index, out var position):
                blobs.Layer(position, bytes);
                break;

            default:
                throw new ArgumentException(
                    $"A blob is a background, a composite or an indexed layer; got '{type}'.",
                    nameof(request));
        }

        return ValueTask.FromResult<JsonNode?>(null);
    }

    /// <summary>The blobs a value names, as the cached paths a handler reads, and forgets them.</summary>
    /// <param name="id">The id the value carries.</param>
    /// <returns>The blobs, or <see langword="null"/> when the id names none.</returns>
    /// <remarks>
    /// The bytes are written into the application's cache here rather than when they arrived,
    /// because a component that was never rendered into an application has no folder to write to.
    /// </remarks>
    private EditorBlobs? Claim(string id) => _blobs.TryRemove(id, out var blobs) ? blobs : null;

    /// <summary>The blobs of one editor value: three images, each possibly absent.</summary>
    private sealed class EditorBlobs
    {
        private readonly List<byte[]?> _layers = [];

        internal byte[]? Background { get; set; }

        internal byte[]? Composite { get; set; }

        internal IReadOnlyList<byte[]?> Layers
        {
            get
            {
                lock (_layers)
                {
                    return [.. _layers];
                }
            }
        }

        internal void Layer(int index, byte[] bytes)
        {
            lock (_layers)
            {
                while (_layers.Count <= index)
                {
                    _layers.Add(null);
                }

                _layers[index] = bytes;
            }
        }
    }
}

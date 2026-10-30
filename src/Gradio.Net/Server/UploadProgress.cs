using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// How far an upload has got, as the client that asked to watch it is told.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>FileUploadProgress</c>: a client that sends an <c>upload_id</c> with its
/// files opens a second connection, and this is what that connection reports — how many bytes of
/// which file have arrived so far. The count is of bytes the server has written, not of bytes the
/// client has sent, which is why the client draws a second bar for it: the browser's own progress
/// stops when the last byte leaves, and this one continues until the file is on disk.
/// </para>
/// <para>
/// One file is reported at a time, and a new file replaces the one before it: the Python version's
/// queue is a deque it pops before it pushes, so it never holds more than one unit. That matters
/// when a request carries several files — a client that was told "the second file is at 3 KB" must
/// not then be told "the first file is at 64 KB", which is what keeping both would do.
/// </para>
/// <para>
/// One instance per application, held by the container: the upload endpoint writes to it and the
/// progress endpoint reads from it, and they are different requests.
/// </para>
/// </remarks>
internal sealed class UploadProgress
{
    /// <summary>How long a progress request waits for its upload to be announced.</summary>
    /// <remarks>
    /// The client opens the stream before it starts sending, so a request that arrives first is
    /// normal rather than an error. The Python version waits three seconds before deciding the id
    /// is not one anybody is using.
    /// </remarks>
    internal static readonly TimeSpan TrackingTimeout = TimeSpan.FromSeconds(3);

    private readonly Dictionary<string, Tracker> _uploads = new(StringComparer.Ordinal);
    private readonly Dictionary<string, TaskCompletionSource> _announced = new(StringComparer.Ordinal);
    private readonly object _gate = new();

    /// <summary>Begins tracking an upload, which is what a progress request waits for.</summary>
    /// <param name="uploadId">The id the client chose.</param>
    /// <remarks>Tracking an id twice is not an error: a client may retry the same upload.</remarks>
    internal void Track(string uploadId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uploadId);

        TaskCompletionSource? announced;

        lock (_gate)
        {
            if (!_uploads.TryGetValue(uploadId, out var tracker))
            {
                tracker = new Tracker();
                _uploads[uploadId] = tracker;
            }

            if (!_announced.TryGetValue(uploadId, out announced))
            {
                announced = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                _announced[uploadId] = announced;
            }
        }

        announced.TrySetResult();
    }

    /// <summary>Waits for an upload to be announced, or gives up.</summary>
    /// <param name="uploadId">The id the client asked about.</param>
    /// <param name="cancellationToken">Cancels the wait.</param>
    /// <returns>Whether an upload with that id was announced in time.</returns>
    internal async Task<bool> WaitTrackedAsync(string uploadId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uploadId);

        TaskCompletionSource announced;
        lock (_gate)
        {
            if (_uploads.ContainsKey(uploadId))
            {
                return true;
            }

            if (!_announced.TryGetValue(uploadId, out var existing))
            {
                existing = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                _announced[uploadId] = existing;
            }

            announced = existing;
        }

        try
        {
            await announced.Task.WaitAsync(TrackingTimeout, cancellationToken).ConfigureAwait(false);

            return true;
        }
        catch (TimeoutException)
        {
            return false;
        }
    }

    /// <summary>Records that some of a file has arrived.</summary>
    /// <param name="uploadId">The upload the file belongs to.</param>
    /// <param name="fileName">The name the client gave the file.</param>
    /// <param name="bytes">How many bytes arrived in this read.</param>
    /// <remarks>
    /// Consecutive reads of the same file add up; a read of a different file starts its own count
    /// and drops the one before it, which is the Python version's rule for a request that carries
    /// several.
    /// </remarks>
    internal void Append(string uploadId, string fileName, int bytes)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uploadId);
        ArgumentNullException.ThrowIfNull(fileName);

        lock (_gate)
        {
            if (!_uploads.TryGetValue(uploadId, out var tracker))
            {
                tracker = new Tracker();
                _uploads[uploadId] = tracker;
            }

            tracker.Pending = tracker.Pending is { } pending && pending.FileName == fileName
                ? pending with { Bytes = pending.Bytes + bytes }
                : new Unit(fileName, bytes);
        }
    }

    /// <summary>Records that an upload is over, whether it finished or was refused.</summary>
    /// <param name="uploadId">The upload.</param>
    internal void Complete(string uploadId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uploadId);

        lock (_gate)
        {
            if (!_uploads.TryGetValue(uploadId, out var tracker))
            {
                tracker = new Tracker();
                _uploads[uploadId] = tracker;
            }

            tracker.Done = true;
        }
    }

    /// <summary>Whether an upload is over.</summary>
    /// <param name="uploadId">The upload.</param>
    /// <returns>Whether the upload finished, or <see langword="false"/> for an id nobody is using.</returns>
    internal bool IsDone(string uploadId)
    {
        lock (_gate)
        {
            return _uploads.TryGetValue(uploadId, out var tracker) && tracker.Done;
        }
    }

    /// <summary>Takes the unit an upload has pending, if any.</summary>
    /// <param name="uploadId">The upload.</param>
    /// <param name="unit">The unit, when there was one.</param>
    /// <returns>Whether there was a unit to take.</returns>
    /// <remarks>
    /// Taking rather than reading is what makes the report a report of what is new: a client that was
    /// told about a megabyte should not be told about it again on the next tick.
    /// </remarks>
    internal bool TryTake(string uploadId, out Unit unit)
    {
        lock (_gate)
        {
            if (_uploads.TryGetValue(uploadId, out var tracker) && tracker.Pending is { } pending)
            {
                unit = pending;
                tracker.Pending = null;

                return true;
            }
        }

        unit = default;

        return false;
    }

    /// <summary>Forgets an upload, which is what a stream that ended does.</summary>
    /// <param name="uploadId">The upload.</param>
    internal void StopTracking(string uploadId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uploadId);

        lock (_gate)
        {
            _uploads.Remove(uploadId);
            _announced.Remove(uploadId);
        }
    }

    /// <summary>One file's running total.</summary>
    /// <param name="FileName">The name the client gave the file.</param>
    /// <param name="Bytes">How many bytes of it have arrived.</param>
    internal readonly record struct Unit(string FileName, long Bytes);

    /// <summary>What one upload has to report, and whether it is over.</summary>
    private sealed class Tracker
    {
        /// <summary>The one file being reported, or <see langword="null"/> when nothing is new.</summary>
        internal Unit? Pending { get; set; }

        internal bool Done { get; set; }
    }

    /// <summary>The message a client reads about an upload.</summary>
    /// <param name="unit">What arrived, when something did.</param>
    /// <returns>The message.</returns>
    /// <remarks>
    /// The Python version's three messages: an update naming the file and its running total, a
    /// heartbeat while nothing is arriving, and one that says the upload is over. The client stops
    /// reading at the last of them, which is why it is the only one that ends the stream.
    /// </remarks>
    internal static JsonObject Update(Unit unit) => new()
    {
        ["msg"] = "update",
        ["orig_name"] = unit.FileName,
        ["chunk_size"] = unit.Bytes,
    };

    /// <summary>The message that says an upload is over.</summary>
    internal static JsonObject DoneMessage() => new() { ["msg"] = "done" };

    /// <summary>The message that says the stream is alive while nothing is arriving.</summary>
    internal static JsonObject Heartbeat() => new() { ["msg"] = "heartbeat" };
}

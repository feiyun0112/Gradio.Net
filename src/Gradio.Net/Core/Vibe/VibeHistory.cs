namespace Gradio.Net;
/// <summary>
/// What the Vibe editor remembers between requests: the conversation it has had, and the code each
/// edit replaced.
/// </summary>
/// <remarks>
/// <para>
/// The Python version keeps both in the closure of the function that registers the routes — a
/// string for the conversation and a dictionary of snapshot names to conversations — and the
/// snapshots themselves as files under the temporary directory. This is the same arrangement as an
/// object, because a closure is not available here: the routes are answered by a capability
/// resolved from the container, so the state it shares between requests has to be a singleton the
/// container holds.
/// </para>
/// <para>
/// There is one history per application, as the Python version has one per router. Two browsers
/// editing at once therefore share it — which is what the Python version does too, and is the
/// reason the editor is a development tool rather than a feature of a deployed app.
/// </para>
/// <para>
/// The conversation is what makes a follow-up instruction ("now make it blue") understandable: it is
/// folded into the next prompt as text, limited to the last few exchanges. A snapshot is written
/// <em>before</em> an edit is applied, so that undoing one puts the file back exactly as the edit
/// found it, conversation included.
/// </para>
/// </remarks>
internal sealed class VibeHistory
{
    /// <summary>The folder under the application's temporary directory the snapshots are written
    /// to.</summary> <remarks>The Python version's <c>DEFAULT_TEMP_DIR /
    /// "vibe_edit_history"</c>.</remarks>
    private const string FolderName = "vibe_edit_history";

    private readonly Blocks _blocks;
    private readonly object _gate = new();
    private readonly Dictionary<string, string> _transcripts = new(StringComparer.Ordinal);
    private string _conversation = string.Empty;

    /// <summary>Creates the history of an application.</summary>
    /// <param name="blocks">The application whose temporary directory holds the snapshots.</param>
    /// <exception cref="ArgumentNullException"><paramref name="blocks"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// Public because the container builds it, and a container only sees public constructors — the
    /// type itself stays internal, because nothing outside this assembly is meant to hold one.
    /// </remarks>
    public VibeHistory(Blocks blocks)
    {
        ArgumentNullException.ThrowIfNull(blocks);

        _blocks = blocks;
    }

    /// <summary>The conversation so far, as the prompt carries it.</summary>
    internal string Conversation
    {
        get
        {
            lock (_gate)
            {
                return _conversation;
            }
        }
    }

    /// <summary>Adds one exchange to the conversation.</summary>
    /// <param name="prompt">What was asked for.</param>
    /// <param name="answer">What the model answered, as it was recorded.</param>
    /// <remarks>
    /// The shape — a leading newline, then <c>User:</c> and <c>Assistant:</c> lines — is the Python
    /// version's own, and it matters: the prompt that is built from this string is read by the
    /// model as a transcript, and the limiter counts exchanges by splitting on the same marker.
    /// </remarks>
    internal void Append(string prompt, string answer)
    {
        lock (_gate)
        {
            _conversation += $"\nUser: {prompt}\nAssistant: {answer}\n";
        }
    }

    /// <summary>Files the conversation as it stands under a snapshot's name.</summary>
    /// <param name="snapshot">The name of the snapshot being taken.</param>
    /// <remarks>
    /// Taken with the snapshot file rather than after the edit, because undoing an edit has to undo
    /// the exchange too: a conversation that still remembers an instruction whose code was rolled back
    /// would answer the next prompt against code that no longer exists.
    /// </remarks>
    internal void Remember(string snapshot)
    {
        lock (_gate)
        {
            _transcripts[snapshot] = _conversation;
        }
    }

    /// <summary>Puts the conversation back to what it was when a snapshot was taken.</summary>
    /// <param name="snapshot">The snapshot's name.</param>
    /// <remarks>
    /// A name the history does not hold restores an empty conversation, which is the Python
    /// version's <c>get(hash, "")</c>: the snapshot's file is what was asked for, and its
    /// conversation is what came with it.
    /// </remarks>
    internal void Restore(string snapshot)
    {
        lock (_gate)
        {
            _conversation = _transcripts.TryGetValue(snapshot, out var remembered)
                ? remembered
                : string.Empty;
        }
    }

    /// <summary>Writes the code an edit is about to replace.</summary>
    /// <param name="snapshot">The snapshot's name.</param>
    /// <param name="code">The code as it stands.</param>
    /// <param name="cancellationToken">Cancels the write.</param>
    /// <returns>A task that completes once the file is written.</returns>
    internal Task SaveAsync(string snapshot, string code, CancellationToken cancellationToken)
    {
        var folder = Folder;

        Directory.CreateDirectory(folder);

        return System.IO.File.WriteAllTextAsync(PathOf(snapshot), code, cancellationToken);
    }

    /// <summary>The code a snapshot holds.</summary>
    /// <param name="snapshot">The snapshot's name.</param>
    /// <param name="cancellationToken">Cancels the read.</param>
    /// <returns>The code, or <see langword="null"/> when no such snapshot was written.</returns>
    /// <remarks>
    /// The name is checked against the shape this class issues before it is used as a file name. A
    /// name is a request field, and a request field that is joined onto a path is a way to read a file
    /// outside the folder — so a name that is not thirty-two hexadecimal characters is answered as a
    /// snapshot that does not exist, which is what a client that lost its snapshot gets anyway.
    /// </remarks>
    internal async Task<string?> ReadAsync(string snapshot, CancellationToken cancellationToken)
    {
        if (!IsSnapshotName(snapshot))
        {
            return null;
        }

        var path = PathOf(snapshot);

        return System.IO.File.Exists(path)
            ? await System.IO.File.ReadAllTextAsync(path, cancellationToken).ConfigureAwait(false)
            : null;
    }

    /// <summary>Whether a name is one this class issues.</summary>
    /// <param name="snapshot">The name from the request.</param>
    /// <returns><see langword="true"/> when it is thirty-two lower-case hexadecimal characters.</returns>
    /// <remarks>
    /// The Python version draws a name with <c>secrets.token_hex(16)</c>, which is exactly that,
    /// and then joins it onto a path unchecked. The check is the one thing this class does that the
    /// Python version does not.
    /// </remarks>
    internal static bool IsSnapshotName(string snapshot) =>
        snapshot.Length == 32 && snapshot.All(char.IsAsciiHexDigitLower);

    /// <summary>The folder the snapshots are written to.</summary>
    private string Folder => Path.Combine(_blocks.UploadFolder, FolderName);

    /// <summary>The file a snapshot is written to.</summary>
    /// <param name="snapshot">The snapshot's name, already known to be one this class issues.</param>
    private string PathOf(string snapshot) => Path.Combine(Folder, snapshot + ".py");
}

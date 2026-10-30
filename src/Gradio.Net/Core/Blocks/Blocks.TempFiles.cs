namespace Gradio.Net;
/// <summary>
/// The temporary files an application created, and the policy that reclaims them.
/// </summary>
/// <remarks>
/// <para>
/// The Python version keeps a set of the files each <c>Blocks</c> wrote on a <c>Blocks</c> instance
/// and deletes from that set rather than from a directory: what is a temporary file is the
/// application's claim, not a property of where it happens to live. The same rule holds here, which
/// is why the set is on the render root and not on the file system.
/// </para>
/// <para>
/// What records a file here is a component or an endpoint that wrote one: an upload is recorded by
/// the endpoint that stored it, and a handler's output by the component that cached it. The set is
/// what the reclamation policy reads, so a file nothing recorded is a file the application does not
/// delete — which is why recording is the writer's job rather than something done on its behalf.
/// </para>
/// </remarks>
public sealed partial class Blocks
{
    private readonly object _tempFilesGate = new();
    private readonly HashSet<string> _tempFiles = new(StringComparer.Ordinal);
    private readonly HashSet<string> _keptFiles = new(StringComparer.Ordinal);

    /// <summary>
    /// How this application's temporary files are reclaimed, or <see langword="null"/> to keep
    /// them.
    /// </summary>
    /// <value>
    /// <see langword="null"/>, which is the Python version's default: an application that says
    /// nothing keeps what it wrote, and the operator cleans up. Setting it is what makes the
    /// application clean up after itself — every <see cref="DeleteCacheSettings.Frequency"/> it
    /// deletes the files it wrote that are older than <see cref="DeleteCacheSettings.Age"/>, and
    /// when it stops it deletes the rest.
    /// </value>
    public DeleteCacheSettings? DeleteCache { get; set; }

    /// <summary>The files this application recorded as its own temporary files.</summary>
    /// <value>
    /// Absolute paths. A file that has been deleted is not in it, and the order is not meaningful:
    /// it is a set, which is what the Python version keeps.
    /// </value>
    public IReadOnlyCollection<string> TempFiles
    {
        get
        {
            lock (_tempFilesGate)
            {
                return [.. _tempFiles];
            }
        }
    }

    /// <summary>
    /// Records a file as this application's temporary file, so that it is reclaimed.
    /// </summary>
    /// <param name="path">The file the application wrote.</param>
    /// <returns>The path as it was recorded: absolute, which is what reclamation compares.</returns>
    /// <exception cref="ArgumentException"><paramref name="path"/> is empty or whitespace.</exception>
    /// <remarks>
    /// Recording a file the application did not write would have the server delete someone else's,
    /// which is why nothing does this on the caller's behalf: a component that saves a file says so.
    /// </remarks>
    public string TrackTempFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        var full = Path.GetFullPath(path);

        lock (_tempFilesGate)
        {
            _tempFiles.Add(full);
        }

        return full;
    }

    /// <summary>
    /// Records a file as one to keep, so that reclamation leaves it alone.
    /// </summary>
    /// <param name="path">The file to keep.</param>
    /// <returns>The path as it was recorded: absolute.</returns>
    /// <exception cref="ArgumentException"><paramref name="path"/> is empty or whitespace.</exception>
    /// <remarks>
    /// For a file that is part of the application rather than a product of a run — a component's
    /// default value, a file the examples use. The Python version keeps the same distinction as
    /// <c>keep_in_cache</c>, and for the same reason: deleting those would break the application
    /// rather than reclaim anything.
    /// </remarks>
    public string KeepTempFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        var full = Path.GetFullPath(path);

        lock (_tempFilesGate)
        {
            _keptFiles.Add(full);
        }

        return full;
    }

    /// <summary>Forgets a file without deleting it.</summary>
    /// <param name="path">The file to forget.</param>
    /// <returns>Whether it was being tracked.</returns>
    /// <remarks>
    /// For a file the application has moved somewhere it means to keep, or deleted itself.
    /// </remarks>
    public bool UntrackTempFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        lock (_tempFilesGate)
        {
            return _tempFiles.Remove(Path.GetFullPath(path));
        }
    }

    /// <summary>
    /// Deletes the recorded files that have aged out, or all of them.
    /// </summary>
    /// <param name="age">How old a file has to be, or <see langword="null"/> to delete every one.</param>
    /// <param name="now">The moment the ages are measured against.</param>
    /// <returns>How many files were deleted.</returns>
    /// <remarks>
    /// <para>
    /// A file's age is measured from its last write, which is the Python version's rule as its
    /// documentation states it ("more than age seconds have passed since the file was created") and
    /// the closest portable reading of the change time it uses. A file that is already gone is
    /// simply forgotten.
    /// </para>
    /// <para>
    /// A file that cannot be deleted is left recorded rather than dropped from the set: a permission
    /// problem or a file still held open is worth retrying on the next sweep, and forgetting it
    /// would leak it for good.
    /// </para>
    /// </remarks>
    internal int DeleteTempFiles(TimeSpan? age, DateTimeOffset now)
    {
        List<string> candidates;
        lock (_tempFilesGate)
        {
            candidates = [.. _tempFiles];
        }

        var deleted = 0;

        foreach (var path in candidates)
        {
            if (IsKept(path))
            {
                continue;
            }

            if (!System.IO.File.Exists(path))
            {
                UntrackTempFile(path);

                continue;
            }

            if (age is { } limit && now - System.IO.File.GetLastWriteTimeUtc(path) <= limit)
            {
                continue;
            }

            try
            {
                System.IO.File.Delete(path);
            }
            catch (IOException)
            {
                continue;
            }
            catch (UnauthorizedAccessException)
            {
                continue;
            }

            UntrackTempFile(path);
            deleted++;
        }

        return deleted;
    }

    /// <summary>Whether a file was recorded as one to keep.</summary>
    /// <param name="path">The file, absolute.</param>
    /// <returns>Whether reclamation must leave it alone.</returns>
    private bool IsKept(string path)
    {
        lock (_tempFilesGate)
        {
            return _keptFiles.Contains(path);
        }
    }
}

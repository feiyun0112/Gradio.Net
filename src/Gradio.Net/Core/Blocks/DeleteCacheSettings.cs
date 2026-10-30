namespace Gradio.Net;
/// <summary>
/// How an application's temporary files are reclaimed: how often to look, and how old a file has to
/// be before it goes.
/// </summary>
/// <remarks>
/// <para>
/// This is the Python version's <c>delete_cache</c> tuple — <c>(frequency, age)</c>, both in
/// seconds — as a type rather than as a pair of numbers, so that the two cannot be swapped by a
/// caller who remembered the order wrong.
/// </para>
/// <para>
/// The files it applies to are the ones the application recorded as its own (see
/// <see cref="Blocks.TrackTempFile"/>): a server that deleted every file in a directory would take
/// another application's work with it, and one that deleted files it never wrote would be deleting
/// the operator's.
/// </para>
/// </remarks>
/// <param name="Frequency">How often to look for files that have aged out.</param>
/// <param name="Age">How old a file has to be, measured from its last write, before it is deleted.</param>
public sealed record DeleteCacheSettings(TimeSpan Frequency, TimeSpan Age)
{
    /// <summary>How often to look for files that have aged out.</summary>
    /// <exception cref="ArgumentOutOfRangeException">The value is not positive.</exception>
    public TimeSpan Frequency { get; } = Positive(Frequency, nameof(Frequency));

    /// <summary>How old a file has to be, measured from its last write, before it is deleted.</summary>
    /// <exception cref="ArgumentOutOfRangeException">The value is not positive.</exception>
    /// <remarks>
    /// Not zero, which would mean "delete a file the moment it is written" and would make an upload
    /// disappear before the run that reads it. The Python version's schedule does not start at all
    /// when either figure is zero, which is the same mistake spelled silently.
    /// </remarks>
    public TimeSpan Age { get; } = Positive(Age, nameof(Age));

    private static TimeSpan Positive(TimeSpan value, string name) =>
        value > TimeSpan.Zero
            ? value
            : throw new ArgumentOutOfRangeException(
                name,
                value,
                $"{nameof(DeleteCacheSettings)}.{name} must be positive; leave delete_cache unset to keep the files.");
}

namespace Gradio.Net;
/// <summary>
/// Where run history goes when nothing else was chosen.
/// </summary>
/// <remarks>
/// <para>
/// The default store keeps records on disk, in a folder of its own under the application's temporary
/// directory. That is the local implementation the framework ships: it needs no account, no network
/// and no configuration, and an application that wants its runs somewhere else replaces one service
/// registration.
/// </para>
/// <para>
/// The folder follows <c>GRADIO_TEMP_DIR</c> the way the media cache does, so an application that
/// already points its uploads somewhere durable gets its runs there too.
/// </para>
/// </remarks>
public sealed class RunHistoryOptions
{
    /// <summary>The bucket records are written to when a request names none.</summary>
    /// <value><c>local</c>, which the file store reads as the name of its own directory.</value>
    public string Bucket { get; init; } = "local";

    /// <summary>The folder the file store keeps everything in.</summary>
    /// <value>
    /// <c>run-history</c> under <c>GRADIO_TEMP_DIR</c>, or under the process's temporary directory
    /// when that is not set.
    /// </value>
    public string RootFolder { get; init; } = Path.Combine(DefaultFolder(), "run-history");

    /// <summary>The folder the media cache uses, as the environment names it.</summary>
    /// <returns>The folder.</returns>
    private static string DefaultFolder()
    {
        var configured = Environment.GetEnvironmentVariable("GRADIO_TEMP_DIR");

        return !string.IsNullOrWhiteSpace(configured)
            ? Path.GetFullPath(configured)
            : Path.Combine(Path.GetTempPath(), "gradio");
    }
}

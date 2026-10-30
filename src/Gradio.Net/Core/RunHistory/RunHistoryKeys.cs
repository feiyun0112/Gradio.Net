using System.Text.RegularExpressions;
namespace Gradio.Net;
/// <summary>
/// Where a record and its assets live, and what a name has to look like to be allowed there.
/// </summary>
/// <remarks>
/// <para>
/// The layout is the Python version's, because a record written by this framework should be
/// readable by anything that reads in the Python version's: <c>runs/{app}/{endpoint}/{record}.json</c>
/// and <c>assets/{app}/{endpoint}/{record}/{name}</c>. Both stores use it, which is what makes "the
/// local store and the object store keep the same history" a property of the code rather than a
/// promise.
/// </para>
/// <para>
/// Every part of a path comes from somewhere untrusted — an endpoint's name from a binding, a record
/// id from a browser, an asset's name from a request — so each is checked against the pattern the
/// part is allowed to match before it is joined onto anything. A segment that is not a name is
/// refused rather than escaped: a path that was repaired into something else is a path nobody can
/// find again.
/// </para>
/// </remarks>
internal static partial class RunHistoryKeys
{
    /// <summary>The directory records are filed under.</summary>
    internal const string RunsPrefix = "runs";

    /// <summary>The directory assets are filed under.</summary>
    internal const string AssetsPrefix = "assets";

    /// <summary>The record id's shape: what a browser is handed and may hand back.</summary>
    [GeneratedRegex("^[A-Za-z0-9_-]{1,64}$")]
    private static partial Regex RecordIdPattern();

    /// <summary>A name's shape: an application id, an endpoint, an asset.</summary>
    [GeneratedRegex("^[A-Za-z0-9_.-]{1,80}$")]
    private static partial Regex SegmentPattern();

    /// <summary>Checks that a value can be used as one part of a path.</summary>
    /// <param name="value">The value.</param>
    /// <param name="what">What the value is, for the message.</param>
    /// <exception cref="RunHistoryException">The value is not a name.</exception>
    internal static void Segment(string value, string what)
    {
        if (!SegmentPattern().IsMatch(value ?? string.Empty) || value is "." or "..")
        {
            throw new RunHistoryException($"The {what} is not a name a run can be filed under.", 422);
        }
    }

    /// <summary>Checks that a value can be used as a record id.</summary>
    /// <param name="value">The value.</param>
    /// <exception cref="RunHistoryException">The value is not a record id.</exception>
    internal static void RecordId(string value)
    {
        if (!RecordIdPattern().IsMatch(value ?? string.Empty))
        {
            throw new RunHistoryException("The record id is not a name a run can be filed under.", 422);
        }
    }

    /// <summary>Checks a bucket name, which is a container rather than a part of a path.</summary>
    /// <param name="bucket">The bucket.</param>
    /// <exception cref="RunHistoryException">The bucket is not a name.</exception>
    /// <remarks>
    /// The Python version's bucket ids look like <c>owner/name</c>, so a bucket is one or more
    /// segments. What a store does with the slashes is the store's business: the object store keeps
    /// them in the container name, and the local one reads them as directories.
    /// </remarks>
    internal static void Bucket(string bucket)
    {
        var segments = (bucket ?? string.Empty).Split('/');

        if (segments.Length == 0
            || segments.Any(segment => !SegmentPattern().IsMatch(segment) || segment is "." or ".."))
        {
            throw new RunHistoryException("The bucket id is not a bucket this store can write to.", 422);
        }
    }

    /// <summary>The key a record is kept under.</summary>
    /// <param name="appId">The application.</param>
    /// <param name="endpoint">The endpoint.</param>
    /// <param name="recordId">The record.</param>
    /// <returns>The key, with slashes.</returns>
    internal static string Record(string appId, string endpoint, string recordId) =>
        $"{RunsPrefix}/{appId}/{endpoint}/{recordId}.json";

    /// <summary>The prefix every record of one application is kept under.</summary>
    /// <param name="appId">The application.</param>
    /// <returns>The prefix, with its trailing slash.</returns>
    internal static string RecordsOf(string appId) => $"{RunsPrefix}/{appId}/";

    /// <summary>The key one asset is kept under.</summary>
    /// <param name="appId">The application.</param>
    /// <param name="endpoint">The endpoint.</param>
    /// <param name="recordId">The record.</param>
    /// <param name="assetId">The asset.</param>
    /// <returns>The key, with slashes.</returns>
    internal static string Asset(string appId, string endpoint, string recordId, string assetId) =>
        $"{AssetsPrefix}/{appId}/{endpoint}/{recordId}/{assetId}";

    /// <summary>The prefix every asset of one record is kept under.</summary>
    /// <param name="appId">The application.</param>
    /// <param name="endpoint">The endpoint.</param>
    /// <param name="recordId">The record.</param>
    /// <returns>The prefix, with its trailing slash.</returns>
    internal static string AssetsOf(string appId, string endpoint, string recordId) =>
        $"{AssetsPrefix}/{appId}/{endpoint}/{recordId}/";

    /// <summary>A record id that sorts chronologically and does not collide.</summary>
    /// <returns>The id.</returns>
    /// <remarks>
    /// The timestamp in front is what makes "newest first" a sort rather than a scan; the random
    /// tail is what keeps two runs that started in the same nanosecond apart. The Python version's
    /// is <c>f"{time.time_ns():020d}{secrets.token_hex(4)}"</c>, so the timestamp is twenty digits
    /// of nanoseconds since the epoch — counted from the same epoch here, because an id is a name
    /// something else may parse.
    /// </remarks>
    internal static string NewRecordId() =>
        ((System.DateTime.UtcNow - System.DateTime.UnixEpoch).Ticks * 100).ToString("D20", System.Globalization.CultureInfo.InvariantCulture)
        + Guid.NewGuid().ToString("N")[..8];

    /// <summary>The folder an application's runs are filed under.</summary>
    /// <param name="blocks">The application.</param>
    /// <returns>The id.</returns>
    /// <remarks>
    /// The Python version re-mints its application id on every start, and so does this: the id is
    /// the application instance's, which is what keeps two applications in one process — or two
    /// starts of one application — from filing runs into each other's folder.
    /// </remarks>
    internal static string AppIdOf(Blocks blocks) =>
        blocks.AppId.ToString(System.Globalization.CultureInfo.InvariantCulture);

    /// <summary>Now, as a record's start time is spelled.</summary> <returns>The time, to the
    /// millisecond, in UTC.</returns> <remarks>The Python version's <c>now_utc_iso</c>, which is
    /// what the browser's <c>toISOString</c> writes.</remarks>
    internal static string Now() =>
        System.DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", System.Globalization.CultureInfo.InvariantCulture);
}

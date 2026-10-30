using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
namespace Gradio.Net;
/// <summary>
/// The other half of a share link: the state a link names, read back so that the page it opens
/// describes the application as it was when the link was made.
/// </summary>
/// <remarks>
/// <para>
/// A link is the page's own address with <c>?deep_link=&lt;name&gt;</c> on it, and the name stands for
/// a file <c>/gradio_api/deep_link</c> wrote: the components of a session, with the values the user
/// had filled in. Reading it back means serving a config whose components are that file's rather than
/// the application's, which is what makes the page open where the link left off.
/// </para>
/// <para>
/// The name is checked against the Python version's pattern before it is used, and the file is
/// looked for under the application's own <c>deep_links</c> folder. Both matter: a name is a URL
/// parameter, so a name that could walk out of that folder would be a way to read any file the
/// server can read.
/// </para>
/// <para>
/// A link that names nothing — a name that was never written, a file that is not JSON, a name that
/// does not look like one — is not an error: the page is served as it would have been, and the
/// config says the link was invalid so that the frontend can say so.
/// </para>
/// </remarks>
internal static partial class DeepLinkState
{
    /// <summary>The state of a request that named no link.</summary>
    internal const string None = "none";

    /// <summary>The state of a request that named a link the server could read.</summary>
    internal const string Valid = "valid";

    /// <summary>The state of a request that named a link it could not use.</summary>
    internal const string Invalid = "invalid";

    private const string FolderName = "deep_links";
    private const string FileName = "state.json";

    /// <summary>Reads the components a link names.</summary>
    /// <param name="blocks">The application the link belongs to.</param>
    /// <param name="name">The name the request carried.</param>
    /// <returns>
    /// The components, and <see cref="Valid"/>; or <see langword="null"/> and <see cref="Invalid"/>
    /// when the name cannot be used.
    /// </returns>
    /// <remarks>
    /// The name is not a path: it is a token, and the file it names is the one the endpoint wrote
    /// under it. Anything that is not a token is refused before the file system is asked anything.
    /// </remarks>
    internal static (JsonArray? Components, string State) Load(Blocks blocks, string name)
    {
        ArgumentNullException.ThrowIfNull(blocks);
        ArgumentNullException.ThrowIfNull(name);

        if (!NamePattern().IsMatch(name))
        {
            return (null, Invalid);
        }

        var folder = Path.Combine(blocks.UploadFolder, FolderName);
        var path = Path.Combine(folder, name, FileName);

        // The pattern already rules this out; asking anyway is what keeps the rule and the code
        // from drifting apart, and a path that escapes is the one mistake here that matters.
        if (!Path.GetFullPath(path).StartsWith(Path.GetFullPath(folder) + Path.DirectorySeparatorChar, StringComparison.Ordinal))
        {
            return (null, Invalid);
        }

        try
        {
            return (JsonNode.Parse(System.IO.File.ReadAllText(path)) as JsonArray, Valid);
        }
        catch (Exception failure) when (failure is IOException or UnauthorizedAccessException or System.Text.Json.JsonException)
        {
            return (null, Invalid);
        }
    }

    /// <summary>The names a link may carry: letters, digits, underscores and hyphens.</summary>
    [GeneratedRegex("^[A-Za-z0-9_-]{1,128}$", RegexOptions.CultureInvariant)]
    private static partial Regex NamePattern();
}

namespace Gradio.Net;
/// <summary>Why an asset path was served, or why it was refused.</summary>
/// <remarks>
/// The refusals are the Python version's, and they are separate values because the Python version
/// answers them with separate statuses: an empty path is a malformed request (400), a path that
/// would leave the tree is forbidden (403), a directory is not a file (403), and a path that names
/// nothing is missing (404).
/// </remarks>
internal enum AssetStatus
{
    /// <summary>The asset was found and opened.</summary>
    Found,

    /// <summary>The path was empty: a request for the route itself rather than for an asset.</summary>
    EmptyPath,

    /// <summary>The path would leave the asset tree, or names a URL rather than an asset.</summary>
    OutsideRoot,

    /// <summary>The path names a directory; the assets under it are separate requests.</summary>
    Directory,

    /// <summary>No asset has that path.</summary>
    NotFound,
}

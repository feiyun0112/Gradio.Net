using System.Collections.Frozen;
using System.Reflection;

namespace Gradio.Net;
/// <summary>
/// The vendored frontend, addressed by the paths the Python version serves it under.
/// </summary>
/// <remarks>
/// <para>
/// The assets are embedded in this assembly (see <c>Gradio.Net.csproj</c>), so serving them needs
/// no loose files: a package reference, a publish, and a single-file deployment all carry the
/// frontend, and nothing at run time has to know where a package was unpacked. This
/// type is the only place that knows how an asset path maps onto a resource name, which is why
/// <see cref="Resolve"/> answers with the reasons a path is refused rather than with a bare
/// <see langword="null"/>: the Python version refuses an empty path, a path outside the tree, and a
/// directory with three different statuses, and the page is served by a client that can tell them
/// apart.
/// </para>
/// <para>
/// The index is built once per container rather than kept in a static field: the package declares
/// no static mutable state, and a container that owns its own asset index is what lets
/// two hosts in one process serve two different assemblies.
/// </para>
/// </remarks>
internal sealed class FrontendAssets
{
    /// <summary>The prefix every frontend resource name carries.</summary>
    /// <remarks>
    /// Written the same way in <c>Gradio.Net.csproj</c>. A test walks the checked-in hash record
    /// and asserts every recorded file resolves here, so the two spellings cannot drift apart
    /// without a failure.
    /// </remarks>
    internal const string ResourcePrefix = "Gradio.Net.frontend/";

    /// <summary>The page the Python version serves for the application itself.</summary>
    internal const string PageAsset = "index.html";

    /// <summary>The icon the Python version serves when the application names no favicon.</summary>
    internal const string DefaultFaviconAsset = "static/img/logo.svg";

    private readonly Assembly _assembly;
    private readonly FrozenDictionary<string, string> _resources;

    /// <summary>Indexes the frontend this assembly carries.</summary>
    public FrontendAssets()
    {
        _assembly = typeof(FrontendAssets).Assembly;

        var resources = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var name in _assembly.GetManifestResourceNames())
        {
            if (name.StartsWith(ResourcePrefix, StringComparison.Ordinal))
            {
                resources[name[ResourcePrefix.Length..]] = name;
            }
        }

        _resources = resources.ToFrozenDictionary(StringComparer.Ordinal);
    }

    /// <summary>
    /// Opens the asset at <paramref name="path"/>.
    /// </summary>
    /// <param name="path">
    /// The path as it arrives under an asset route: <c>assets/index-D88pgWqG.js</c>, with no
    /// leading slash and no query.
    /// </param>
    /// <param name="content">The asset's bytes, or <see langword="null"/> when it was refused.</param>
    /// <param name="contentType">The type to send it as; set even when the asset is refused.</param>
    /// <returns>Why the path was accepted or refused.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
    internal AssetStatus Open(string path, out Stream? content, out string contentType)
    {
        ArgumentNullException.ThrowIfNull(path);

        contentType = ContentTypes.Of(path);

        var status = Resolve(path, out var resourceName);
        if (status != AssetStatus.Found)
        {
            content = null;

            return status;
        }

        // A resource stream is a view onto the assembly, so it is cheap to open and needs no
        // buffering; the response copies it straight out.
        content = _assembly.GetManifestResourceStream(resourceName!);

        return content is null ? AssetStatus.NotFound : status;
    }

    /// <summary>
    /// Reads an asset as text.
    /// </summary>
    /// <param name="path">The asset path, as <see cref="Open"/> takes it.</param>
    /// <returns>The asset's text.</returns>
    /// <exception cref="InvalidOperationException">The asset is missing from this assembly.</exception>
    /// <remarks>
    /// The page template and the assets it references are checked in by the build, so a missing one
    /// is a broken build rather than a request that should be answered with a status.
    /// </remarks>
    internal string ReadText(string path)
    {
        var status = Open(path, out var content, out _);
        if (status != AssetStatus.Found || content is null)
        {
            throw new InvalidOperationException(
                $"The vendored frontend asset '{path}' is not embedded in this assembly ({status}). "
                + "The tree under src/upstream/frontend is the Python version's, and it is embedded as it is.");
        }

        using (content)
        using (var reader = new StreamReader(content))
        {
            return reader.ReadToEnd();
        }
    }

    private AssetStatus Resolve(string path, out string? resourceName)
    {
        resourceName = null;

        // The Python version refuses an empty path with 400: it is a request for the asset route itself, not
        // for a file.
        if (path.Length == 0)
        {
            return AssetStatus.EmptyPath;
        }

        // A path with a traversal segment would leave the tree; the Python version answers 403 rather than
        // pretending the file is missing. A path that names a URL or an absolute path never reaches
        // here: that is a property of what the client asked for rather than of the joined path, so
        // the endpoint decides it before joining (see GradioFrontendEndpoints).
        if (LeavesRoot(path))
        {
            return AssetStatus.OutsideRoot;
        }

        if (_resources.TryGetValue(path, out var name))
        {
            resourceName = name;

            return AssetStatus.Found;
        }

        // Nothing to serve, but something under it: a directory. The Python version refuses to list one.
        var directory = path.TrimEnd('/') + "/";
        foreach (var candidate in _resources.Keys)
        {
            if (candidate.StartsWith(directory, StringComparison.Ordinal))
            {
                return AssetStatus.Directory;
            }
        }

        return AssetStatus.NotFound;
    }

    private static bool LeavesRoot(string path)
    {
        if (path.Contains('\\'))
        {
            return true;
        }

        foreach (var segment in path.Split('/'))
        {
            if (segment is ".." or ".")
            {
                return true;
            }
        }

        return false;
    }
}

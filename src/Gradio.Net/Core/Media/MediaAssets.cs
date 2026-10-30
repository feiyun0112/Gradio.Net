namespace Gradio.Net;
/// <summary>
/// The sample media this package carries, and where on disk a handler can find it.
/// </summary>
/// <remarks>
/// <para>
/// The Python version ships a directory of sample files beside its media module and returns paths
/// into it. The tree here is the same tree, vendored verbatim and verified by the build, but it is
/// embedded in the assembly rather than left loose — a package reference, a publish and a
/// single-file deployment all carry it that way, and nothing has to know where the package was
/// unpacked.
/// </para>
/// <para>
/// A handler opens a path, not an embedded resource, so a file is written to the cache folder the
/// first time it is asked for and its path is returned. The write is a copy of bytes that never
/// change, so it is guarded by a plain "is it already there" check rather than by bookkeeping: two
/// callers racing produce two identical files at the same path, and the second one's move replaces
/// the first.
/// </para>
/// </remarks>
internal static class MediaAssets
{
    /// <summary>The prefix every embedded media resource's name starts with.</summary>
    internal const string ResourcePrefix = "Gradio.Net.media/";

    /// <summary>The folder the tree is written to, under the cache.</summary>
    /// <remarks>
    /// The Python version's own directory name, so that a path a handler logs reads the way a
    /// migrating application expects it to.
    /// </remarks>
    internal const string FolderName = "media_assets";

    /// <summary>The files in one of the tree's directories.</summary>
    /// <param name="directory">Which one: <c>images</c>, <c>videos</c>, <c>audio</c>, <c>models3d</c>, <c>data</c>.</param>
    /// <returns>The file names, without the directory.</returns>
    /// <remarks>
    /// Read from the assembly's resource names rather than from a list written down here: a file the
    /// vendoring tool added is a file this finds, which is what keeps the two from drifting.
    /// </remarks>
    internal static IReadOnlyList<string> FilesIn(string directory)
    {
        var prefix = $"{ResourcePrefix}{directory}/";
        var names = new List<string>();

        foreach (var resource in typeof(MediaAssets).Assembly.GetManifestResourceNames())
        {
            if (resource.StartsWith(prefix, StringComparison.Ordinal))
            {
                names.Add(resource[prefix.Length..]);
            }
        }

        return names;
    }

    /// <summary>Writes one file of the tree to the cache, unless it is already there.</summary>
    /// <param name="directory">Which directory of the tree the file is in.</param>
    /// <param name="name">The file's name.</param>
    /// <returns>The absolute path a handler can open.</returns>
    /// <exception cref="FileNotFoundException">The tree carries no such file.</exception>
    /// <remarks>
    /// The path is the same on every call and for every process on the machine, which is what makes
    /// the existence check enough: the bytes are the Python version's and cannot change under a
    /// running application.
    /// </remarks>
    internal static string Materialize(string directory, string name)
    {
        var relative = $"{directory}/{name}";
        var destination = Path.Combine(MediaCache.DefaultFolder, FolderName, directory, name);

        if (System.IO.File.Exists(destination))
        {
            return destination;
        }

        using var content = typeof(MediaAssets).Assembly.GetManifestResourceStream(ResourcePrefix + relative)
            ?? throw new FileNotFoundException(
                $"Media file not found: {destination}. The vendored tree carries no '{relative}'.",
                destination);

        Directory.CreateDirectory(Path.Combine(MediaCache.DefaultFolder, FolderName, directory));

        // Written beside the destination and moved onto it, so that a second caller racing this one
        // either finds the finished file or replaces it with the same bytes — never a half one.
        var temporary = $"{destination}.{Guid.NewGuid():N}.tmp";

        try
        {
            using (var file = System.IO.File.Create(temporary))
            {
                content.CopyTo(file);
            }

            System.IO.File.Move(temporary, destination, overwrite: true);
        }
        finally
        {
            if (System.IO.File.Exists(temporary))
            {
                System.IO.File.Delete(temporary);
            }
        }

        return destination;
    }
}

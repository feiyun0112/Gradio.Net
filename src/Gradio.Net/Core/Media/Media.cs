namespace Gradio.Net;
/// <summary>
/// The sample media an application can point a component at: a cheetah, a cantina, a bunny, a
/// table — or a file of its own.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>gradio.media</c>, which exists so that an example is a line rather than
/// a file to download: <c>gr.Image(gr.get_image("cheetah1.jpg"))</c>. Each helper returns a path,
/// and asking for none returns one of the files in that directory at random, which is what makes a
/// demo that shows "some image" possible without shipping one.
/// </para>
/// <para>
/// A remote address is fetched once and handed over as a local file. That is the one place this
/// differs from the Python version, which returns the address it was given: its components
/// understand a URL, but the helper's contract is a path a handler can open, and a handler that
/// receives a URL has to write the download itself. The fetch is one plain HTTP request — no
/// retries, and a redirect is followed only within HTTP, because the alternative is a file that
/// arrives from somewhere the caller did not name: the address is the user's, not the framework's.
/// </para>
/// <para>
/// The files are the Python version's own, vendored verbatim and verified by the build. They are
/// written into the application's cache folder the first time one is asked for, because a handler
/// opens a path and the tree is embedded in the assembly.
/// </para>
/// </remarks>
public static class Media
{
    /// <summary>Where the images live, as the tree spells it.</summary>
    private const string Images = "images";

    /// <summary>Where the videos live.</summary>
    private const string Videos = "videos";

    /// <summary>Where the audio lives.</summary>
    private const string Audio = "audio";

    /// <summary>Where the 3D models live.</summary>
    private const string Models3d = "models3d";

    /// <summary>Where the data files live.</summary>
    private const string Data = "data";

    /// <summary>An image: a path to one of the samples, or a file fetched from an address.</summary>
    /// <param name="filename">
    /// The sample's name, such as <c>cheetah1.jpg</c>; an http(s) address to fetch; or
    /// <see langword="null"/> for one of the samples at random.
    /// </param>
    /// <param name="client">
    /// The client a remote address is fetched with, or <see langword="null"/> for one made here —
    /// which is how a test hands in a client that talks to a local endpoint.
    /// </param>
    /// <returns>The absolute path of the file.</returns>
    /// <exception cref="FileNotFoundException">No sample has that name.</exception>
    /// <exception cref="HttpRequestException">The address could not be fetched.</exception>
    public static string GetImage(string? filename = null, HttpClient? client = null) =>
        Resolve(Images, filename, client);

    /// <summary>A video: a path to one of the samples, or a file fetched from an address.</summary>
    /// <param name="filename">The sample's name, an address, or <see langword="null"/> for a random one.</param>
    /// <param name="client">The client a remote address is fetched with, or <see langword="null"/>.</param>
    /// <returns>The absolute path of the file.</returns>
    /// <exception cref="FileNotFoundException">No sample has that name.</exception>
    /// <exception cref="HttpRequestException">The address could not be fetched.</exception>
    public static string GetVideo(string? filename = null, HttpClient? client = null) =>
        Resolve(Videos, filename, client);

    /// <summary>Audio: a path to one of the samples, or a file fetched from an address.</summary>
    /// <param name="filename">The sample's name, an address, or <see langword="null"/> for a random one.</param>
    /// <param name="client">The client a remote address is fetched with, or <see langword="null"/>.</param>
    /// <returns>The absolute path of the file.</returns>
    /// <exception cref="FileNotFoundException">No sample has that name.</exception>
    /// <exception cref="HttpRequestException">The address could not be fetched.</exception>
    public static string GetAudio(string? filename = null, HttpClient? client = null) =>
        Resolve(Audio, filename, client);

    /// <summary>A 3D model: a path to one of the samples, or a file fetched from an address.</summary>
    /// <param name="filename">The sample's name, an address, or <see langword="null"/> for a random one.</param>
    /// <param name="client">The client a remote address is fetched with, or <see langword="null"/>.</param>
    /// <returns>The absolute path of the file.</returns>
    /// <exception cref="FileNotFoundException">No sample has that name.</exception>
    /// <exception cref="HttpRequestException">The address could not be fetched.</exception>
    public static string GetModel3d(string? filename = null, HttpClient? client = null) =>
        Resolve(Models3d, filename, client);

    /// <summary>A data file: a path to one of the samples, or a file fetched from an address.</summary>
    /// <param name="filename">The sample's name, an address, or <see langword="null"/> for a random one.</param>
    /// <param name="client">The client a remote address is fetched with, or <see langword="null"/>.</param>
    /// <returns>The absolute path of the file.</returns>
    /// <exception cref="FileNotFoundException">No sample has that name.</exception>
    /// <exception cref="HttpRequestException">The address could not be fetched.</exception>
    public static string GetFile(string? filename = null, HttpClient? client = null) =>
        Resolve(Data, filename, client);

    /// <summary>Resolves a name in one of the tree's directories to a path on disk.</summary>
    /// <param name="directory">Which directory.</param>
    /// <param name="filename">The name, an address, or <see langword="null"/>.</param>
    /// <param name="client">The client for an address, or <see langword="null"/>.</param>
    /// <returns>The absolute path.</returns>
    /// <exception cref="FileNotFoundException">No sample has that name.</exception>
    private static string Resolve(string directory, string? filename, HttpClient? client)
    {
        if (filename is null)
        {
            var names = MediaAssets.FilesIn(directory);

            // A tree with nothing in it cannot happen — the build verifies every file — but an
            // index into an empty list would be a worse way to say so than the Python version's sentence.
            if (names.Count == 0)
            {
                throw new InvalidOperationException(
                    $"No media files found in {Path.Combine(MediaCache.DefaultFolder, MediaAssets.FolderName, directory)}");
            }

            return MediaAssets.Materialize(directory, names[Random.Shared.Next(names.Count)]);
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(filename);

        return MediaCache.NamesALocation(filename)
            ? Fetch(filename, directory, client)
            : MediaAssets.Materialize(directory, filename);
    }

    /// <summary>Fetches a remote address once, and answers with the file it was copied to.</summary>
    /// <param name="address">The address the caller named.</param>
    /// <param name="directory">Which directory of the cache the copy belongs in.</param>
    /// <param name="client">The client to use, or <see langword="null"/> for one made here.</param>
    /// <returns>The cached path, absolute.</returns>
    /// <exception cref="HttpRequestException">
    /// The address is not http(s), or it answered with a failure.
    /// </exception>
    /// <remarks>
    /// <para>
    /// One request, and no loop: a retry would be the framework deciding how long a handler may
    /// take, which is the application's decision rather than its own. A client that wants a deadline
    /// passes one configured with it.
    /// </para>
    /// <para>
    /// The response is refused when it is not http(s) — which .NET's own handler already refuses,
    /// since it will not follow a redirect to another scheme — so that the rule is stated where it
    /// is relied on rather than inferred from a library's behaviour.
    /// </para>
    /// </remarks>
    private static string Fetch(string address, string directory, HttpClient? client)
    {
        if (!Uri.TryCreate(address, UriKind.Absolute, out var uri)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new HttpRequestException($"A media address has to be http or https: '{address}'.");
        }

        if (client is not null)
        {
            return Download(uri, directory, client);
        }

        // A client made here and thrown away with the request: nothing in the framework holds one,
        // and a shared one would be state an application cannot replace.
        using var owned = new HttpClient();

        return Download(uri, directory, owned);
    }

    /// <summary>Fetches one address, once, and copies the answer into the cache.</summary>
    /// <param name="uri">The address, already known to be http(s).</param>
    /// <param name="directory">Which directory of the cache the copy belongs in.</param>
    /// <param name="client">The client to fetch with.</param>
    /// <returns>The cached path, absolute.</returns>
    /// <exception cref="HttpRequestException">The address answered with a failure, or resolved to
    /// something that is not http(s).</exception>
    private static string Download(Uri uri, string directory, HttpClient client)
    {
        // The asynchronous path rather than the synchronous one: a handler an application supplies
        // overrides `SendAsync`, and the synchronous `Send` is a convenience that only the default
        // handler implements. Nothing is captured, so waiting here cannot deadlock a caller.
        using var request = new HttpRequestMessage(HttpMethod.Get, uri);
        using var response = client.SendAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();

        response.EnsureSuccessStatusCode();

        var final = response.RequestMessage?.RequestUri ?? uri;
        if (final.Scheme != Uri.UriSchemeHttp && final.Scheme != Uri.UriSchemeHttps)
        {
            throw new HttpRequestException($"A media address resolved to '{final.Scheme}', which is not http or https.");
        }

        var name = Path.GetFileName(final.AbsolutePath);
        if (string.IsNullOrEmpty(name))
        {
            name = "download";
        }

        using var content = response.Content.ReadAsStream();
        var folder = Path.Combine(MediaCache.DefaultFolder, MediaAssets.FolderName, directory);

        return MediaCache.SaveStream(content, name, folder);
    }
}

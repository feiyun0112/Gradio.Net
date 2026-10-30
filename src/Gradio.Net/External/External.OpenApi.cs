using System.Text.Json;
using System.Text.Json.Nodes;
namespace Gradio.Net;
public static partial class External
{
    /// <summary>
    /// Turns an OpenAPI 3 document into a page of forms, one per endpoint.
    /// </summary>
    /// <param name="blocks">The application to render into.</param>
    /// <param name="baseUrl">
    /// The address the endpoints are called at, e.g. <c>https://api.example.com/v1</c>. Each
    /// endpoint's path is appended to it, and its own leading slash is kept.
    /// </param>
    /// <param name="spec">The document itself, as the JSON it is published as.</param>
    /// <param name="paths">
    /// The paths to draw, as patterns matched at the start of a path — <c>/books</c> keeps
    /// <c>/books</c> and <c>/books/{id}</c> — or <see langword="null"/> for every path in the
    /// document.
    /// </param>
    /// <param name="excludePaths">
    /// The paths to leave out, as patterns, or <see langword="null"/> for none. An exclusion wins
    /// over an inclusion, so a path that matches both is left out.
    /// </param>
    /// <param name="methods">
    /// The methods to draw — <c>get</c>, <c>post</c>, and so on — or <see langword="null"/> for every
    /// method the document declares.
    /// </param>
    /// <param name="authToken">
    /// The token to send as a bearer token with every call, or <see langword="null"/> for none.
    /// </param>
    /// <param name="buildInterface">
    /// How each endpoint's interface is built, or <see langword="null"/> for a plain one. The
    /// Python version forwards a dictionary of <c>gr.Interface</c> arguments here; a delegate is
    /// the same thing with the arguments checked when the page is written rather than when it is
    /// served, and it can set anything an <see cref="Interface"/> can be told.
    /// </param>
    /// <param name="httpClient">The client to call the service with, or <see langword="null"/> for one of the page's own.</param>
    /// <returns><paramref name="blocks"/>, once the page has been drawn into it.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="blocks"/> or <paramref name="spec"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="spec"/> is not a JSON object, or no endpoint is left after the filters.
    /// </exception>
    /// <remarks>
    /// <para>
    /// The page is a panel listing what it holds, and then one interface per endpoint: a field per
    /// parameter, a box of JSON per request body, and a JSON component for the answer. A service
    /// that answers with an error status is shown as it answered — the status and the message — and
    /// a call that cannot be made at all fails the run, the way any handler's failure does.
    /// </para>
    /// <para>
    /// The document is the one this call is given. Its references are resolved within itself, which
    /// is where the references of a document this page can use point; a body schema that refers to
    /// something outside it is left unresolved and its field starts empty.
    /// </para>
    /// </remarks>
    public static Blocks LoadOpenApi(
        Blocks blocks,
        string baseUrl,
        JsonNode spec,
        IReadOnlyList<string>? paths = null,
        IReadOnlyList<string>? excludePaths = null,
        IReadOnlyList<string>? methods = null,
        string? authToken = null,
        Func<OpenApiEndpoint, Interface>? buildInterface = null,
        HttpClient? httpClient = null)
    {
        ArgumentNullException.ThrowIfNull(blocks);
        ArgumentNullException.ThrowIfNull(spec);

        if (spec is not JsonObject document)
        {
            throw new ArgumentException(
                "An OpenAPI specification is a JSON object; got " + spec.GetValueKind() + ".",
                nameof(spec));
        }

        return Render(
            blocks,
            baseUrl,
            document,
            paths,
            excludePaths,
            methods,
            authToken,
            buildInterface,
            httpClient);
    }

    /// <summary>
    /// Turns an OpenAPI 3 document into a page of forms, reading the document from an address or a
    /// file first.
    /// </summary>
    /// <param name="blocks">The application to render into.</param>
    /// <param name="baseUrl">The address the endpoints are called at.</param>
    /// <param name="specLocation">
    /// Where the document is: an <c>http</c> or <c>https</c> address, or a path on this machine.
    /// </param>
    /// <param name="paths">The paths to draw, as patterns, or <see langword="null"/> for all of them.</param>
    /// <param name="excludePaths">The paths to leave out, as patterns, or <see langword="null"/> for none.</param>
    /// <param name="methods">The methods to draw, or <see langword="null"/> for all of them.</param>
    /// <param name="authToken">The token to send as a bearer token with every call, or <see langword="null"/> for none.</param>
    /// <param name="buildInterface">How each endpoint's interface is built, or <see langword="null"/> for a plain one.</param>
    /// <param name="httpClient">The client to read and call with, or <see langword="null"/> for one of the page's own.</param>
    /// <param name="cancellationToken">Cancels the read.</param>
    /// <returns><paramref name="blocks"/>, once the page has been drawn into it.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="blocks"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="specLocation"/> is empty, the document is not JSON, or no endpoint is left
    /// after the filters.
    /// </exception>
    /// <exception cref="HttpRequestException">The address could not be read.</exception>
    /// <remarks>
    /// The read is asynchronous because a document served at an address is fetched, and a loader
    /// that blocked a thread to wait for it would block the one that started the application. What
    /// is drawn is the same page <see cref="LoadOpenApi"/> draws from a document already in hand.
    /// </remarks>
    public static async Task<Blocks> LoadOpenApiAsync(
        Blocks blocks,
        string baseUrl,
        string specLocation,
        IReadOnlyList<string>? paths = null,
        IReadOnlyList<string>? excludePaths = null,
        IReadOnlyList<string>? methods = null,
        string? authToken = null,
        Func<OpenApiEndpoint, Interface>? buildInterface = null,
        HttpClient? httpClient = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(blocks);

        if (string.IsNullOrWhiteSpace(specLocation))
        {
            throw new ArgumentException(
                "An OpenAPI specification is read from an address or a path; neither was given.",
                nameof(specLocation));
        }

        var client = httpClient ?? new HttpClient();
        var text = await ReadAsync(specLocation, client, cancellationToken).ConfigureAwait(false);

        JsonNode? spec;
        try
        {
            spec = JsonNode.Parse(text);
        }
        catch (JsonException error)
        {
            throw new ArgumentException(
                $"'{specLocation}' does not hold an OpenAPI document in JSON: {error.Message}",
                nameof(specLocation),
                error);
        }

        return LoadOpenApi(
            blocks,
            baseUrl,
            spec ?? throw new ArgumentException(
                $"'{specLocation}' does not hold an OpenAPI document in JSON.",
                nameof(specLocation)),
            paths,
            excludePaths,
            methods,
            authToken,
            buildInterface,
            client);
    }

    /// <summary>Draws the page, whichever way the document arrived.</summary>
    /// <param name="blocks">The application to render into.</param>
    /// <param name="baseUrl">The address the endpoints are called at.</param>
    /// <param name="spec">The document.</param>
    /// <param name="paths">The paths to draw, as patterns, or <see langword="null"/> for all of them.</param>
    /// <param name="excludePaths">The paths to leave out, as patterns, or <see langword="null"/> for none.</param>
    /// <param name="methods">The methods to draw, or <see langword="null"/> for all of them.</param>
    /// <param name="authToken">The token to send, or <see langword="null"/> for none.</param>
    /// <param name="buildInterface">How each endpoint's interface is built, or <see langword="null"/> for a plain one.</param>
    /// <param name="httpClient">The client to call the service with, or <see langword="null"/> for one of the page's own.</param>
    /// <returns><paramref name="blocks"/>.</returns>
    private static Blocks Render(
        Blocks blocks,
        string baseUrl,
        JsonObject spec,
        IReadOnlyList<string>? paths,
        IReadOnlyList<string>? excludePaths,
        IReadOnlyList<string>? methods,
        string? authToken,
        Func<OpenApiEndpoint, Interface>? buildInterface,
        HttpClient? httpClient)
    {
        var page = new OpenApiPage(
            blocks,
            spec,
            baseUrl,
            authToken,
            httpClient ?? new HttpClient(),
            buildInterface ?? (endpoint => new Interface(
                blocks,
                endpoint.Fn,
                [.. endpoint.Inputs],
                [.. endpoint.Outputs],
                apiName: endpoint.ApiName,

                // The Python version's generated handler carries the document's description as its docstring,
                // so the endpoint's own `api_description` is left unset and the API document reads
                // the text from there. The two are different fields in the config, and they are
                // kept apart deliberately.
                apiDocstring: endpoint.ApiDescription,
                parameterNames: endpoint.ParameterNames)));

        page.Render(paths, excludePaths, methods);

        return blocks;
    }

    /// <summary>The document's text, read from an address or from this machine.</summary>
    /// <param name="location">Where it is.</param>
    /// <param name="client">The client to read an address with.</param>
    /// <param name="cancellationToken">Cancels the read.</param>
    /// <returns>The text.</returns>
    /// <exception cref="HttpRequestException">The address could not be read.</exception>
    private static async Task<string> ReadAsync(
        string location,
        HttpClient client,
        CancellationToken cancellationToken)
    {
        if (!Attachments.IsAddress(location))
        {
            return await System.IO.File
                .ReadAllTextAsync(location, cancellationToken)
                .ConfigureAwait(false);
        }

        using var response = await client
            .GetAsync(location, HttpCompletionOption.ResponseContentRead, cancellationToken)
            .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
    }
}

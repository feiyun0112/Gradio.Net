using System.Text.Json.Nodes;
namespace Gradio.Net;
public static partial class gr
{
    /// <summary>
    /// Builds a chat around an endpoint that answers like OpenAI's: the Python version's
    /// <c>gr.load_chat</c>.
    /// </summary>
    /// <param name="baseUrl">The endpoint's base URL, e.g.
    /// <c>http://localhost:11434/v1/</c>.</param> <param name="model">The model to ask for, as the
    /// endpoint names it.</param> <param name="token">The bearer token to send, or
    /// <see langword="null"/> for the placeholder a local endpoint ignores.</param>
    /// <param name="fileTypes">What the field accepts besides text: one kind, a list of them, or
    /// <see langword="null"/> for the Python version's default.</param>
    /// <param name="systemMessage">The instruction the conversation opens with, or
    /// <see langword="null"/> for none.</param> <param name="streaming">Whether the answer is shown
    /// as it arrives.</param> <returns>The application the chat was drawn into.</returns>
    /// <exception cref="ArgumentException"><paramref name="baseUrl"/> or <paramref name="model"/>
    /// is empty, or a file type is not one of the two the Python version names.</exception>
    /// <remarks>
    /// <para>
    /// The loader returns an application here as it does in the Python version — a Python
    /// <c>ChatInterface</c> <em>is</em> a <c>Blocks</c> — so <c>gr.LoadChat(...).Launch()</c> is
    /// the same two lines as the Python.
    /// </para>
    /// <para>
    /// <c>fileTypes</c> keeps the Python version's union: a string naming one kind, or a list of
    /// them. <see cref="ChatFileType"/> is the set they add up to, which is what
    /// <see cref="External.LoadChat"/> takes.
    /// </para>
    /// </remarks>
    public static Blocks LoadChat(
        string baseUrl,
        string model,
        string? token = null,
        object? fileTypes = null,
        string? systemMessage = null,
        bool streaming = true)
    {
        var demo = NewApplication(null);

        _ = External.LoadChat(
            demo,
            baseUrl,
            model,
            token,
            FileTypesOf(fileTypes),
            systemMessage,
            streaming);

        return demo;
    }

    /// <summary>
    /// Builds a page of forms from an OpenAPI document: the Python version's
    /// <c>gr.load_openapi</c>.
    /// </summary>
    /// <param name="openapiSpec">The document, or where it is: an <c>http</c> address or a path on this machine.</param>
    /// <param name="baseUrl">The address the endpoints are called at.</param>
    /// <param name="paths">The paths to draw, as patterns, or <see langword="null"/> for all of them.</param>
    /// <param name="excludePaths">The paths to leave out, or <see langword="null"/> for none.</param>
    /// <param name="methods">The methods to draw, or <see langword="null"/> for every one the document declares.</param>
    /// <param name="authToken">The token to send as a bearer token, or <see langword="null"/> for none.</param>
    /// <returns>The application the page was drawn into.</returns>
    /// <exception cref="ArgumentException"><paramref name="openapiSpec"/> is not a document, a path or an address.</exception>
    /// <remarks>
    /// <para>
    /// A document given as text is read here and handed over; a document given as an address is read
    /// by this call, which blocks while it is fetched. An application that cannot afford to block
    /// during start-up awaits <see cref="LoadOpenApiAsync"/> instead.
    /// </para>
    /// <para>
    /// The parameter names are the Python version's, <c>openapiSpec</c> first — the document, then
    /// where the endpoints are.
    /// </para>
    /// </remarks>
    public static Blocks LoadOpenapi(
        object openapiSpec,
        string baseUrl,
        IReadOnlyList<string>? paths = null,
        IReadOnlyList<string>? excludePaths = null,
        IReadOnlyList<string>? methods = null,
        string? authToken = null)
    {
        var demo = NewApplication(null);

        _ = External.LoadOpenApi(
            demo,
            baseUrl,
            DocumentOf(openapiSpec),
            paths,
            excludePaths,
            methods,
            authToken);

        return demo;
    }

    /// <summary>
    /// The same page as <see cref="LoadOpenapi"/>, with the document read without blocking.
    /// </summary>
    /// <param name="openapiSpec">The document, or where it is: an <c>http</c> address or a path on this machine.</param>
    /// <param name="baseUrl">The address the endpoints are called at.</param>
    /// <param name="paths">The paths to draw, as patterns, or <see langword="null"/> for all of them.</param>
    /// <param name="excludePaths">The paths to leave out, or <see langword="null"/> for none.</param>
    /// <param name="methods">The methods to draw, or <see langword="null"/> for every one the document declares.</param>
    /// <param name="authToken">The token to send as a bearer token, or <see langword="null"/> for none.</param>
    /// <param name="cancellationToken">Cancels the read.</param>
    /// <returns>The application the page was drawn into.</returns>
    /// <exception cref="ArgumentException"><paramref name="openapiSpec"/> is not a document, a path or an address.</exception>
    /// <remarks>
    /// <para>
    /// This name is this layer's own rather than the Python version's: <c>load_openapi</c> is
    /// synchronous there, and reading a document from an address while a host is starting is the
    /// one place a .NET application would rather await. The spelling follows the rest of this
    /// package (<see cref="External.LoadOpenApiAsync"/>); the sync spelling above follows the
    /// Python version, so a ported line still compiles.
    /// </para>
    /// <para>
    /// A document already in hand draws the same page without waiting for anything.
    /// </para>
    /// </remarks>
    public static async Task<Blocks> LoadOpenApiAsync(
        object openapiSpec,
        string baseUrl,
        IReadOnlyList<string>? paths = null,
        IReadOnlyList<string>? excludePaths = null,
        IReadOnlyList<string>? methods = null,
        string? authToken = null,
        CancellationToken cancellationToken = default)
    {
        var demo = NewApplication(null);

        if (openapiSpec is string location)
        {
            _ = await External
                .LoadOpenApiAsync(demo, baseUrl, location, paths, excludePaths, methods, authToken, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        else
        {
            _ = External.LoadOpenApi(
                demo,
                baseUrl,
                DocumentOf(openapiSpec),
                paths,
                excludePaths,
                methods,
                authToken);
        }

        return demo;
    }

    /// <summary>The document an <c>openapiSpec</c> names: itself, or what the address or path holds.</summary>
    /// <param name="spec">A document, a path, or an address.</param>
    /// <returns>The document.</returns>
    /// <exception cref="ArgumentException">The value is none of those.</exception>
    private static JsonNode DocumentOf(object spec) => spec switch
    {
        JsonNode document => document,
        string location when location.StartsWith("http", StringComparison.OrdinalIgnoreCase) =>
            JsonNode.Parse(new HttpClient().GetStringAsync(location).GetAwaiter().GetResult())!,
        string path => JsonNode.Parse(System.IO.File.ReadAllText(path))!,
        _ => throw new ArgumentException(
            $"An OpenAPI document is a document, a path or an address; got {spec.GetType()}.",
            nameof(spec)),
    };

    /// <summary>Reads the Python version's <c>fileTypes</c>: one kind's name, or a list of
    /// them.</summary> <param name="value">A name, a list of names, or <see langword="null"/> for
    /// the Python version's default.</param> <returns>The kinds, as the set
    /// <see cref="External.LoadChat"/> takes.</returns> <exception cref="ArgumentException">A name
    /// is not one of the two kinds.</exception>
    private static ChatFileType FileTypesOf(object? value)
    {
        if (value is null)
        {
            return ChatFileType.TextEncoded;
        }

        var names = value is string name ? [name] : ((IEnumerable<object?>)value).Select(item => $"{item}");

        var kinds = ChatFileType.None;

        foreach (var spelling in names)
        {
            kinds |= spelling switch
            {
                "text_encoded" => ChatFileType.TextEncoded,
                "image" => ChatFileType.Image,
                _ => throw new ArgumentException(
                    $"A chat's file_types are 'text_encoded' and 'image'; got '{spelling}'.",
                    nameof(value)),
            };
        }

        return kinds;
    }
}

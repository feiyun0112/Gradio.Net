using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;

namespace Gradio.Net;
/// <summary>
/// The endpoint the browser calls a component's own server functions through.
/// </summary>
/// <remarks>
/// <para>
/// A few components are more than a value and a handler: a file explorer lists a directory when the
/// user opens it, and an image editor uploads the canvases it is working on. Those are functions of
/// the component rather than of the application, so they are not bindings — they have no inputs, no
/// outputs and no place in the API — and the Python version reaches them through one route that
/// names the component and the function.
/// </para>
/// <para>
/// The request arrives in one of two shapes. A JSON body carries the function's argument; a
/// multipart body carries it as form fields beside the files it refers to, which is how an editor
/// sends images without putting their bytes in JSON. Both are read here and handed to the block as
/// one call, so that a component does not have to know which shape the browser chose.
/// </para>
/// </remarks>
internal sealed class GradioComponentServerEndpoint : IGradioEndpoint
{
    private const string RoutePath = "/gradio_api/component_server";

    /// <summary>The refusal a request naming no function gets, spelled as FastAPI spells it.</summary>
    private const string MissingFunction = """{"detail":"Function not found."}""";

    /// <summary>The refusal a request whose body is not JSON gets.</summary>
    private const string InvalidBody = """{"detail":"Invalid JSON body."}""";

    /// <summary>The refusal a request carrying more than one file gets, as the Python version's
    /// parser words it.</summary>
    private const string TooManyFiles = """{"detail":"Too many files."}""";

    /// <summary>The refusal a request carrying a file over the application's limit gets.</summary>
    private const string TooLarge = """{"detail":"File size exceeded."}""";

    private readonly Blocks _blocks;

    /// <summary>Creates the capability.</summary>
    /// <param name="blocks">The application whose components are called.</param>
    /// <exception cref="ArgumentNullException"><paramref name="blocks"/> is <see langword="null"/>.</exception>
    public GradioComponentServerEndpoint(Blocks blocks)
    {
        ArgumentNullException.ThrowIfNull(blocks);

        _blocks = blocks;
    }

    /// <inheritdoc />
    public IReadOnlyCollection<string> Routes { get; } = [new GradioRoute("POST", RoutePath).Key];

    /// <inheritdoc />
    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var read = await ReadAsync(context).ConfigureAwait(false);

        if (read.Call is not { } request)
        {
            await RefuseAsync(context, read.Status, read.Detail).ConfigureAwait(false);

            return;
        }

        var (componentId, functionName, data, files) = request;

        var block = _blocks.Registered.FirstOrDefault(candidate => candidate.Id == componentId);

        if (block is null || !block.ServerFunctionNames.Contains(functionName, StringComparer.Ordinal))
        {
            await RefuseAsync(context, StatusCodes.Status404NotFound, MissingFunction).ConfigureAwait(false);

            return;
        }

        JsonNode? result;
        try
        {
            result = await block
                .InvokeServerFunctionAsync(
                    new ComponentServerCall
                    {
                        FunctionName = functionName,
                        Data = data,
                        Files = files,
                    },
                    context.RequestAborted)
                .ConfigureAwait(false);
        }
        catch (Exception error) when (error is ArgumentException or InvalidOperationException)
        {
            await RefuseAsync(
                context,
                StatusCodes.Status400BadRequest,
                JsonText.Serialize(new JsonObject { ["detail"] = error.Message })).ConfigureAwait(false);

            return;
        }

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "application/json";

        // A server function that answers nothing is answered with JSON's null, which is what
        // FastAPI does with a Python function that returns None.
        await context.Response
            .WriteAsync(
                result is null ? "null" : JsonText.Serialize(result),
                context.RequestAborted)
            .ConfigureAwait(false);
    }

    /// <summary>Reads a call, whichever of the two shapes it arrived in.</summary>
    /// <param name="context">The request to read.</param>
    /// <returns>The call, or the refusal that says why there is none.</returns>
    /// <remarks>
    /// The refusal is part of the answer rather than an exception because the two ways a body can
    /// be wrong are answered differently: a body that is not a call is malformed (400), and a file
    /// larger than the application accepts is too large (413) — the same pair the Python version's
    /// parser draws, and the reason this returns a status rather than <see langword="null"/>.
    /// </remarks>
    private async Task<ReadOutcome> ReadAsync(HttpContext context)
    {
        var contentType = context.Request.ContentType;

        if (contentType is not null
            && contentType.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase))
        {
            IFormCollection form;
            try
            {
                form = await context.Request.ReadFormAsync(context.RequestAborted).ConfigureAwait(false);
            }
            catch (InvalidDataException error)
            {
                // The framework refuses a body over its own multipart limit, and a part whose
                // headers are too long, both of which are "too large" rather than "malformed" —
                // the same pair the upload reader draws.
                return ReadOutcome.Refused(
                    error.Message.Contains("exceeded", StringComparison.OrdinalIgnoreCase)
                        ? StatusCodes.Status413PayloadTooLarge
                        : StatusCodes.Status400BadRequest,
                    JsonText.Serialize(new JsonObject { ["detail"] = error.Message }));
            }

            if (!int.TryParse(form["component_id"], out var formId)
                || form["fn_name"].ToString() is not { Length: > 0 } formFunction)
            {
                return ReadOutcome.Refused(StatusCodes.Status400BadRequest, InvalidBody);
            }

            // The Python version's parser takes one file on this route and refuses the rest. The body has
            // already been parsed by the framework here, so what this can promise is the refusal
            // before the file is copied into memory rather than before it is read.
            if (form.Files.Count > 1)
            {
                return ReadOutcome.Refused(StatusCodes.Status400BadRequest, TooManyFiles);
            }

            var files = new List<ComponentServerFile>();
            foreach (var file in form.Files)
            {
                if (_blocks.MaxFileSize is { } limit && file.Length > limit)
                {
                    return ReadOutcome.Refused(StatusCodes.Status413PayloadTooLarge, TooLarge);
                }

                using var buffer = new MemoryStream();
                await file.CopyToAsync(buffer, context.RequestAborted).ConfigureAwait(false);
                files.Add(new ComponentServerFile(file.FileName, buffer.ToArray()));
            }

            return new ReadOutcome(
                StatusCodes.Status200OK,
                string.Empty,
                (formId, formFunction, Parse(form["data"].ToString()), files));
        }

        JsonObject? body;
        try
        {
            body = await JsonNode.ParseAsync(context.Request.Body, cancellationToken: context.RequestAborted)
                .ConfigureAwait(false) as JsonObject;
        }
        catch (System.Text.Json.JsonException)
        {
            return ReadOutcome.Refused(StatusCodes.Status400BadRequest, InvalidBody);
        }

        if (body is null
            || JsonValues.Int32(body["component_id"]) is not { } componentId
            || JsonValues.Text(body["fn_name"]) is not { Length: > 0 } functionName)
        {
            return ReadOutcome.Refused(StatusCodes.Status400BadRequest, InvalidBody);
        }

        return new ReadOutcome(
            StatusCodes.Status200OK,
            string.Empty,
            (componentId, functionName, body["data"]?.DeepClone(), []));
    }

    /// <summary>What reading a call produced: the call, or why there is none.</summary>
    /// <param name="Status">The status the refusal answers with, or 200 when there is a call.</param>
    /// <param name="Detail">The refusal's body, or empty when there is a call.</param>
    /// <param name="Call">The call, or <see langword="null"/> when the body was refused.</param>
    private sealed record ReadOutcome(
        int Status,
        string Detail,
        (int ComponentId, string FunctionName, JsonNode? Data, IReadOnlyList<ComponentServerFile> Files)? Call)
    {
        /// <summary>A body that is not a call.</summary>
        /// <param name="status">The status to answer with.</param>
        /// <param name="detail">The body to answer with.</param>
        /// <returns>The refusal.</returns>
        internal static ReadOutcome Refused(int status, string detail) => new(status, detail, null);
    }

    /// <summary>The argument a multipart call carried, which arrives as JSON text.</summary>
    private static JsonNode? Parse(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        try
        {
            return JsonNode.Parse(text);
        }
        catch (System.Text.Json.JsonException)
        {
            return null;
        }
    }

    /// <summary>Answers a refusal with the body FastAPI would have written.</summary>
    private static async Task RefuseAsync(HttpContext context, int status, string body)
    {
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(body, context.RequestAborted).ConfigureAwait(false);
    }
}

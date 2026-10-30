using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Gradio.Net;
/// <summary>
/// The Vibe editor: the routes that rewrite the application's own source from a sentence, undo one of
/// those rewrites, and hand the browser the code it is editing.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's five routes, answered from the same file and the same shape of document.
/// What is the Python version's and kept: the paths, the request bodies, the answers, the 403 for
/// an application that has not turned the editor on, the snapshot that is written before an edit so
/// that it can be undone, and the two numbers the panel shows beside a message.
/// </para>
/// <para>
/// What is deliberately different is where the answer comes from. The Python version calls a
/// Hugging Face <c>InferenceClient</c> with a fixed model; here the model is
/// <see cref="IVibeLanguageModel"/>, registered by the application, and an application that
/// registers none is told so rather than being handed a vendor it did not choose. The prompt is the
/// Python version's, minus the documentation it fetches from gradio.app at request time — the same
/// reason.
/// </para>
/// <para>
/// This is a development tool, not a feature of a deployed app: it reads and writes a file on the
/// server by path, and its state is one conversation shared by every browser that opens the panel.
/// That is the Python version's arrangement, and turning it on is a decision an operator makes with
/// <c>GRADIO_VIBE_MODE</c> and <c>GRADIO_WATCH_DEMO_PATH</c>.
/// </para>
/// <para>
/// The two refusals a request can meet — the editor being off, and its body being malformed — are
/// answered in the order FastAPI answers them rather than the order the handler code reads in:
/// FastAPI validates a route's body before it calls the function, so a malformed body to an
/// application that has not turned the editor on is refused as malformed. Only the routes that take
/// a body can answer that way, which is why the switch is asked inside each handler.
/// </para>
/// </remarks>
internal sealed class GradioVibeEndpoints : IGradioEndpoint
{
    private const string EditPath = "/gradio_api/vibe-edit";
    private const string UndoPath = "/gradio_api/undo-vibe-edit";
    private const string CodePath = "/gradio_api/vibe-code";
    private const string StarterQueriesPath = "/gradio_api/vibe-starter-queries";

    /// <summary>The refusal an application that has not turned the editor on answers with.</summary>
    /// <remarks>
    /// The Python version's sentence, including the flag it names, because it is the one piece of
    /// documentation a caller who reached this route needs.
    /// </remarks>
    private const string NotEnabled = "Vibe editor is not enabled. Use --vibe flag to enable.";

    /// <summary>What a client is told when the answer carries no code.</summary>
    private const string NoAnswer = "Error generating code";

    private readonly Blocks _blocks;
    private readonly VibeOptions _options;
    private readonly VibeHistory _history;
    private readonly IVibeLanguageModel? _backend;
    private readonly Action<ILogger, string, Exception?> _logGenerationFailed;

    /// <summary>Creates the capability.</summary>
    /// <param name="blocks">The application whose source is edited, and whose switch decides whether it may be.</param>
    /// <param name="options">The file to edit, and the model to ask.</param>
    /// <param name="history">The conversation, and the snapshots an undo restores.</param>
    /// <param name="backend">
    /// The language model to ask, or <see langword="null"/> when the application registered none —
    /// which is the default, and which the routes report rather than work around.
    /// </param>
    /// <exception cref="ArgumentNullException">An argument other than <paramref name="backend"/> is <see langword="null"/>.</exception>
    public GradioVibeEndpoints(
        Blocks blocks,
        VibeOptions options,
        VibeHistory history,
        IVibeLanguageModel? backend = null)
    {
        ArgumentNullException.ThrowIfNull(blocks);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(history);

        _blocks = blocks;
        _options = options;
        _history = history;
        _backend = backend;

        _logGenerationFailed = LoggerMessage.Define<string>(
            LogLevel.Warning,
            new EventId(1, "GradioVibeGenerationFailed"),
            "The Vibe editor's language model failed: {Reason}");
    }

    /// <inheritdoc />
    public IReadOnlyCollection<string> Routes { get; } =
    [
        new GradioRoute("POST", EditPath).Key,
        new GradioRoute("POST", UndoPath).Key,
        new GradioRoute("GET", CodePath).Key,
        new GradioRoute("POST", CodePath).Key,
        new GradioRoute("POST", StarterQueriesPath).Key,
    ];

    /// <inheritdoc />
    public Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var route = GradioRouteMetadata.PathOf(context);

        return route switch
        {
            EditPath => EditAsync(context),
            UndoPath => UndoAsync(context),
            CodePath => HttpMethods.IsPost(context.Request.Method)
                ? UpdateCodeAsync(context)
                : ReadCodeAsync(context),
            StarterQueriesPath => StarterQueriesAsync(context),
            _ => Answers.NotFoundAsync(context),
        };
    }

    /// <summary>Rewrites the file from an instruction, and answers with what changed.</summary>
    /// <param name="context">The request.</param>
    /// <returns>A task that completes once the answer is written.</returns>
    /// <remarks>
    /// The order is the Python version's: the snapshot is written before the model is asked, so
    /// that an answer that turns out to be unusable can still be undone, and the conversation is
    /// extended before the explanation is taken out of the answer, so that what is remembered is
    /// what the model said. The two checks that come first — a backend and a file to edit — are
    /// this framework's, because both are configuration this framework has to be told about rather
    /// than things the Python version can discover.
    /// </remarks>
    private async Task EditAsync(HttpContext context)
    {
        var prompt = await TextFieldAsync(context, "prompt").ConfigureAwait(false);

        if (prompt is null || !await EnabledAsync(context).ConfigureAwait(false))
        {
            return;
        }

        if (await BackendAsync(context).ConfigureAwait(false) is not { } backend)
        {
            return;
        }

        if (await ReadDemoAsync(context).ConfigureAwait(false) is not { } original)
        {
            return;
        }

        var snapshot = NewSnapshotName();

        await _history
            .SaveAsync(snapshot, original, context.RequestAborted)
            .ConfigureAwait(false);

        _history.Remember(snapshot);

        var answer = await AskAsync(
            context,
            backend,
            VibePrompt.RewriteRules,
            VibePrompt.RewritePrompt(original, prompt, VibePrompt.LimitHistory(_history.Conversation)))
            .ConfigureAwait(false);

        if (answer is null)
        {
            return;
        }

        var cleaned = VibeAnswer.WithoutThinkBlocks(VibeAnswer.WithoutMarkers(answer));

        _history.Append(prompt, cleaned);

        var (reasoning, text) = VibeAnswer.SplitReasoning(cleaned);
        var updated = VibeAnswer.ExtractCode(text);
        var (added, removed) = VibeDiff.Counts(original, updated);

        if (!await WriteDemoAsync(context, updated).ConfigureAwait(false))
        {
            return;
        }

        await Answers
            .JsonAsync(
                context,
                StatusCodes.Status200OK,
                new JsonObject
                {
                    ["hash"] = snapshot,
                    ["diff_stats"] = new JsonObject
                    {
                        ["lines_added"] = added,
                        ["lines_removed"] = removed,
                    },
                    ["reasoning"] = reasoning,
                })
            .ConfigureAwait(false);
    }

    /// <summary>Puts the file and the conversation back to what a snapshot holds.</summary>
    /// <param name="context">The request.</param>
    /// <returns>A task that completes once the answer is written.</returns>
    /// <remarks>
    /// A name that is not one this capability issued is answered as a snapshot that does not exist,
    /// which is what a browser that lost one gets anyway — and is also the answer to a name built to
    /// reach a file outside the folder the snapshots live in.
    /// </remarks>
    private async Task UndoAsync(HttpContext context)
    {
        var snapshot = await TextFieldAsync(context, "hash").ConfigureAwait(false);

        if (snapshot is null || !await EnabledAsync(context).ConfigureAwait(false))
        {
            return;
        }

        if (await _history.ReadAsync(snapshot, context.RequestAborted).ConfigureAwait(false) is not { } restored)
        {
            await RefuseAsync(context, StatusCodes.Status404NotFound, "Snapshot not found").ConfigureAwait(false);

            return;
        }

        if (!await WriteDemoAsync(context, restored).ConfigureAwait(false))
        {
            return;
        }

        _history.Restore(snapshot);

        await Answers
            .JsonAsync(context, StatusCodes.Status200OK, new JsonObject { ["success"] = true })
            .ConfigureAwait(false);
    }

    /// <summary>Answers with the code the editor is editing.</summary>
    /// <param name="context">The request.</param>
    /// <returns>A task that completes once the answer is written.</returns>
    private async Task ReadCodeAsync(HttpContext context)
    {
        if (!await EnabledAsync(context).ConfigureAwait(false))
        {
            return;
        }

        if (await ReadDemoAsync(context).ConfigureAwait(false) is not { } code)
        {
            return;
        }

        await Answers
            .JsonAsync(context, StatusCodes.Status200OK, new JsonObject { ["code"] = code })
            .ConfigureAwait(false);
    }

    /// <summary>Writes the code the editor's own text box now holds.</summary>
    /// <param name="context">The request.</param>
    /// <returns>A task that completes once the answer is written.</returns>
    /// <remarks>
    /// The Python version writes the file directly rather than editing the application in memory,
    /// because the reload watcher is what turns the write into a running app; an application here
    /// is written by the same watcher, if the deployment has one.
    /// </remarks>
    private async Task UpdateCodeAsync(HttpContext context)
    {
        var code = await TextFieldAsync(context, "code").ConfigureAwait(false);

        if (code is null || !await EnabledAsync(context).ConfigureAwait(false))
        {
            return;
        }

        if (!await WriteDemoAsync(context, code).ConfigureAwait(false))
        {
            return;
        }

        await Answers
            .JsonAsync(context, StatusCodes.Status200OK, new JsonObject { ["success"] = true })
            .ConfigureAwait(false);
    }

    /// <summary>Asks the model what one might want to change about this app.</summary>
    /// <param name="context">The request.</param>
    /// <returns>A task that completes once the answer is written.</returns>
    /// <remarks>
    /// The answer is split on its line breaks as it is, empty lines included: the Python version
    /// does not filter them, and a panel that drew a blank chip would be showing what the model
    /// actually said. This route takes no body — the Python version's handler declares none — so
    /// the switch is asked first.
    /// </remarks>
    private async Task StarterQueriesAsync(HttpContext context)
    {
        if (!await EnabledAsync(context).ConfigureAwait(false))
        {
            return;
        }

        if (await BackendAsync(context).ConfigureAwait(false) is not { } backend)
        {
            return;
        }

        if (await ReadDemoAsync(context).ConfigureAwait(false) is not { } code)
        {
            return;
        }

        var answer = await AskAsync(
            context,
            backend,
            VibePrompt.StarterQueryRules,
            VibePrompt.StarterQueriesPrompt(code))
            .ConfigureAwait(false);

        if (answer is null)
        {
            return;
        }

        var cleaned = VibeAnswer.WithoutMarkers(VibeAnswer.WithoutThinkBlocks(answer));
        var queries = new JsonArray();

        foreach (var query in cleaned.Split('\n'))
        {
            queries.Add(query);
        }

        await Answers
            .JsonAsync(context, StatusCodes.Status200OK, new JsonObject { ["starter_queries"] = queries })
            .ConfigureAwait(false);
    }

    /// <summary>Asks the model a question, and answers the request if it cannot.</summary>
    /// <param name="context">The request.</param>
    /// <param name="backend">The language model to ask.</param>
    /// <param name="rules">The rules the answer is written under.</param>
    /// <param name="prompt">What to ask.</param>
    /// <returns>The answer, or <see langword="null"/> when the request has already been answered.</returns>
    /// <remarks>
    /// A backend that throws is reported as a failed generation and its reason is logged: the panel
    /// has one thing to say about a failure, and an operator reading the log is the one who can act on
    /// which of the many ways a model call fails actually happened.
    /// </remarks>
    private async Task<string?> AskAsync(
        HttpContext context,
        IVibeLanguageModel backend,
        string rules,
        string prompt)
    {
        try
        {
            var request = new VibeCompletionRequest(_options.Model, rules, prompt, _options.MaxTokens);
            var answer = await backend.CompleteAsync(request, context.RequestAborted).ConfigureAwait(false);

            if (answer is not null)
            {
                return answer;
            }

            await RefuseAsync(context, StatusCodes.Status500InternalServerError, NoAnswer).ConfigureAwait(false);
        }
        catch (Exception failure) when (!context.RequestAborted.IsCancellationRequested)
        {
            _logGenerationFailed(Logger(context), failure.Message, failure);

            await RefuseAsync(context, StatusCodes.Status500InternalServerError, NoAnswer).ConfigureAwait(false);
        }

        return null;
    }

    /// <summary>Whether the editor is on, and the refusal an application that has not turned it on gets.</summary>
    /// <param name="context">The request.</param>
    /// <returns><see langword="true"/> when the editor may answer; otherwise the request has been answered.</returns>
    private async Task<bool> EnabledAsync(HttpContext context)
    {
        if (_blocks.VibeMode)
        {
            return true;
        }

        await RefuseAsync(context, StatusCodes.Status403Forbidden, NotEnabled).ConfigureAwait(false);

        return false;
    }

    /// <summary>The backend to ask, or the refusal an application that registered none is given.</summary>
    /// <param name="context">The request.</param>
    /// <returns>The backend, or <see langword="null"/> when the request has already been answered.</returns>
    /// <remarks>
    /// The answer says what to do about it, because this is the one failure an application's author
    /// causes rather than one a deployment hits: the editor is on, and the thing that writes the code
    /// was never registered.
    /// </remarks>
    private async Task<IVibeLanguageModel?> BackendAsync(HttpContext context)
    {
        if (_backend is { } backend)
        {
            return backend;
        }

        await RefuseAsync(
            context,
            StatusCodes.Status500InternalServerError,
            $"The Vibe editor has no language-model backend. Register an implementation of "
            + $"{nameof(IVibeLanguageModel)} with the container; no vendor is bound by default.")
            .ConfigureAwait(false);

        return null;
    }

    /// <summary>The file the editor rewrites, or the refusal a deployment without one is given.</summary>
    /// <param name="context">The request.</param>
    /// <returns>The code, or <see langword="null"/> when the request has already been answered.</returns>
    /// <remarks>
    /// An empty path is a deployment that named no demo file, which the Python version discovers
    /// the same way: its <c>open("")</c> fails, and the browser is told the file is not there. A
    /// file that is gone is answered with the same 404 here, whichever route asked — the Python
    /// version only maps that failure for the route that reads the code and lets the other two
    /// surface as a server error, which is the accident of an uncaught exception rather than an
    /// answer.
    /// </remarks>
    private async Task<string?> ReadDemoAsync(HttpContext context)
    {
        if (_options.DemoPath.Length == 0)
        {
            await RefuseAsync(context, StatusCodes.Status404NotFound, "Demo file not found").ConfigureAwait(false);

            return null;
        }

        try
        {
            return await System.IO.File
                .ReadAllTextAsync(_options.DemoPath, context.RequestAborted)
                .ConfigureAwait(false);
        }
        catch (Exception failure) when (
            (failure is FileNotFoundException or DirectoryNotFoundException)
            && !context.RequestAborted.IsCancellationRequested)
        {
            await RefuseAsync(context, StatusCodes.Status404NotFound, "Demo file not found").ConfigureAwait(false);

            return null;
        }
        catch (Exception failure) when (!context.RequestAborted.IsCancellationRequested)
        {
            await RefuseAsync(
                context,
                StatusCodes.Status500InternalServerError,
                $"Error reading file: {failure.Message}").ConfigureAwait(false);

            return null;
        }
    }

    /// <summary>Writes the file the editor rewrites.</summary>
    /// <param name="context">The request.</param>
    /// <param name="code">The code to write.</param>
    /// <returns><see langword="true"/> when it was written, and <see langword="false"/> when the request has been answered.</returns>
    private async Task<bool> WriteDemoAsync(HttpContext context, string code)
    {
        if (_options.DemoPath.Length == 0)
        {
            await RefuseAsync(context, StatusCodes.Status404NotFound, "Demo file not found").ConfigureAwait(false);

            return false;
        }

        try
        {
            await System.IO.File
                .WriteAllTextAsync(_options.DemoPath, code, context.RequestAborted)
                .ConfigureAwait(false);

            return true;
        }
        catch (Exception failure) when (!context.RequestAborted.IsCancellationRequested)
        {
            await RefuseAsync(
                context,
                StatusCodes.Status500InternalServerError,
                $"Error writing file: {failure.Message}").ConfigureAwait(false);

            return false;
        }
    }

    /// <summary>A required string field of the request body, or the refusal FastAPI would give.</summary>
    /// <param name="context">The request.</param>
    /// <param name="field">The field's name.</param>
    /// <returns>The text, or <see langword="null"/> when the request has already been answered.</returns>
    /// <remarks>
    /// A field that is not there and a field that is there as something other than a string are two
    /// different refusals to FastAPI, and a client that reads the document tells them apart. A body
    /// that is not JSON at all is answered as the field it therefore lacks, rather than as
    /// pydantic's <c>json_invalid</c> — that entry carries the text of Python's decoder, which this
    /// framework would have to invent.
    /// </remarks>
    private static async Task<string?> TextFieldAsync(HttpContext context, string field)
    {
        JsonNode? body = null;

        try
        {
            body = await JsonNode
                .ParseAsync(context.Request.Body, cancellationToken: context.RequestAborted)
                .ConfigureAwait(false);
        }
        catch (JsonException)
        {
            // A body that is not JSON is a body without the field, which is the answer below.
        }

        if (body is not JsonObject fields || !fields.TryGetPropertyValue(field, out var node))
        {
            await Answers
                .JsonAsync(
                    context,
                    StatusCodes.Status422UnprocessableEntity,
                    new JsonObject { ["detail"] = new JsonArray(Answers.MissingBodyField(field)) })
                .ConfigureAwait(false);

            return null;
        }

        if (JsonValues.Text(node) is { } text)
        {
            return text;
        }

        await Answers
            .JsonAsync(
                context,
                StatusCodes.Status422UnprocessableEntity,
                new JsonObject { ["detail"] = new JsonArray(Answers.WrongBodyType(field, node)) })
            .ConfigureAwait(false);

        return null;
    }

    /// <summary>A name for the snapshot an edit is about to take.</summary> <returns>Thirty-two
    /// lower-case hexadecimal characters, as the Python version draws them.</returns>
    private static string NewSnapshotName() =>
        Convert.ToHexString(RandomNumberGenerator.GetBytes(16)).ToLowerInvariant();

    /// <summary>Answers with a failure a client can read.</summary>
    /// <param name="context">The request.</param>
    /// <param name="status">The status to answer with.</param>
    /// <param name="detail">What went wrong.</param>
    /// <returns>A task that completes once the answer is written.</returns>
    private static Task RefuseAsync(HttpContext context, int status, string detail) =>
        Answers.JsonAsync(context, status, new JsonObject { ["detail"] = detail });

    /// <summary>The logger to report a failed generation to.</summary>
    /// <param name="context">The request being answered.</param>
    /// <returns>The logger, when the host registered one.</returns>
    private static ILogger Logger(HttpContext context) =>
        context.RequestServices.GetService(typeof(ILogger<GradioVibeEndpoints>)) as ILogger<GradioVibeEndpoints>
        ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<GradioVibeEndpoints>.Instance;
}

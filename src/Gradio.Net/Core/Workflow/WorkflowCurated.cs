using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// The templates the workflow canvas offers an author: the spaces and models the Python version
/// curates as good starting points, read from the snapshot it ships (the Python version's
/// <c>_workflow_curated_snapshot.json</c> and the functions that answer from it).
/// </summary>
/// <remarks>
/// <para>
/// The Python version answers these from a cached list it first tries to fetch from a Hugging Face
/// dataset and falls back to a bundled snapshot for. The fetch is the part this build does not
/// have: it is a request to a third party made while a request is being served. The snapshot is the
/// part it keeps, vendored byte for byte and verified by the build, so what the canvas is offered
/// here is exactly what the Python version offers when its fetch fails — which is also what it
/// offers an offline deployment.
/// </para>
/// <para>
/// The five answers are the ones that only read that list: two searches over it, the two lists the
/// canvas filters by, and "is this repository one of the templates". Everything else the Python
/// version registers on the canvas — the token, the Space, the model and dataset callers, the save
/// — is an integration with a service, and is absent for the same reason.
/// </para>
/// <para>
/// The shapes are the Python version's down to the key order and to the JSON spelling, which is why
/// <see cref="PythonJson"/> writes them rather than a serializer: the answer is a string a browser
/// parses, and it is the Python version's document as the Python version writes it.
/// </para>
/// </remarks>
internal static class WorkflowCurated
{
    /// <summary>The name the snapshot is embedded under.</summary>
    private const string ResourceName = "Gradio.Net.workflow/_workflow_curated_snapshot.json";

    /// <summary>The most entries one search answers with.</summary>
    /// <remarks>The Python version's limit, which keeps a dropdown a dropdown.</remarks>
    private const int ResultLimit = 48;

    /// <summary>The latency a template without one is ranked by.</summary> <remarks>The Python
    /// version's stand-in: an unmeasured template sorts after every measured one.</remarks>
    private const int UnmeasuredLatency = 999_999;

    private static readonly Lazy<IReadOnlyList<JsonObject>> Entries = new(Load, isThreadSafe: true);

    /// <summary>The spaces, filtered and ranked the way the canvas asks for them.</summary>
    /// <param name="data">
    /// The call's arguments, as the browser sends them: kind, query, task, modality, and whether to
    /// keep only the templates that run without a GPU.
    /// </param>
    /// <returns>The entries as a JSON document.</returns>
    internal static string Spaces(JsonNode? data) =>
        PythonJson.Dump(Search(Arguments(data), "space"));

    /// <summary>The models, filtered and ranked the way the canvas asks for them.</summary>
    /// <param name="data">The call's arguments: kind, query, task, and modality.</param>
    /// <returns>The entries as a JSON document.</returns>
    internal static string Models(JsonNode? data) =>
        PythonJson.Dump(Search(Arguments(data), "model"));

    /// <summary>Whether a repository is one of the templates.</summary>
    /// <param name="data">The call's arguments: the repository's id, and its kind when known.</param>
    /// <returns>
    /// <c>{"curated": false}</c>, or <c>{"curated": true, "status": "ok", "featured": …}</c>.
    /// </returns>
    /// <remarks>
    /// A template whose last check failed is answered as not curated: the canvas is deciding whether
    /// to offer it, and a template that did not answer is not one to offer.
    /// </remarks>
    internal static string IsCurated(JsonNode? data)
    {
        var arguments = Arguments(data);
        var id = Text(arguments, 0);
        var kind = Text(arguments, 1);

        foreach (var entry in Entries.Value)
        {
            if (TextOf(entry, "id") != id || (kind.Length > 0 && TextOf(entry, "kind") != kind))
            {
                continue;
            }

            if (Failed(entry))
            {
                return PythonJson.Dump(new JsonObject { ["curated"] = false });
            }

            return PythonJson.Dump(new JsonObject
            {
                ["curated"] = true,
                ["status"] = "ok",
                ["featured"] = Truthy(Field(entry, "featured")),
            });
        }

        return PythonJson.Dump(new JsonObject { ["curated"] = false });
    }

    /// <summary>The modalities the templates cover.</summary>
    /// <param name="data">The call's arguments, which this answer ignores.</param>
    /// <returns>The modalities, sorted, as a JSON document.</returns>
    internal static string Modalities(JsonNode? data)
    {
        _ = data;

        var modalities = new SortedSet<string>(StringComparer.Ordinal);

        foreach (var entry in Entries.Value)
        {
            if (Failed(entry))
            {
                continue;
            }

            if (TextOf(entry, "modality") is { Length: > 0 } modality)
            {
                modalities.Add(modality);
            }
        }

        return PythonJson.Dump(ArrayOf(modalities));
    }

    /// <summary>The tasks the templates cover, optionally within one modality.</summary>
    /// <param name="data">The call's arguments: the modality to narrow to, or nothing.</param>
    /// <returns>The tasks and space categories, sorted, as a JSON document.</returns>
    /// <remarks>
    /// A template names its task one of two ways — a model has a task, a space has a category — and
    /// the canvas offers both in one list, so both are reported.
    /// </remarks>
    internal static string ModalityTasks(JsonNode? data)
    {
        var modality = Text(Arguments(data), 0);
        var tasks = new SortedSet<string>(StringComparer.Ordinal);

        foreach (var entry in Entries.Value)
        {
            if (modality.Length > 0 && TextOf(entry, "modality") != modality)
            {
                continue;
            }

            if (Failed(entry))
            {
                continue;
            }

            foreach (var name in new[] { "task", "space_category" })
            {
                if (TextOf(entry, name) is { Length: > 0 } task)
                {
                    tasks.Add(task);
                }
            }
        }

        return PythonJson.Dump(ArrayOf(tasks));
    }

    /// <summary>The entries one search answers with, in the order it answers them.</summary>
    /// <param name="arguments">The call's arguments.</param>
    /// <param name="kind">Which role's entries to search: <c>space</c> or <c>model</c>.</param>
    /// <returns>The entries, as the documents the canvas reads.</returns>
    /// <remarks>
    /// The two roles differ in three places, and each is the Python version's: a space may be
    /// narrowed to the ones that run without a GPU, the ranking of a space prefers those, and a
    /// space is described with the fields its card draws while a model is described with its
    /// download count.
    /// </remarks>
    private static JsonArray Search(JsonArray arguments, string kind)
    {
        var newestFirst = Text(arguments, 0) == "new";
        var query = Text(arguments, 1);
        var task = Text(arguments, 2);
        var modality = Text(arguments, 3);
        var withoutGpu = kind == "space" && Truthy(arguments.Count > 4 ? arguments[4] : null);

        var entries = Filter(kind, task, query, modality);

        if (withoutGpu)
        {
            entries = entries.Where(WithoutGpu).ToList();
        }

        entries = newestFirst
            ? entries.OrderByDescending(entry => TextOf(entry, "added_at"), StringComparer.Ordinal).ToList()
            : kind == "space"
                ? entries
                    .OrderBy(entry => WithoutGpu(entry) ? 0 : 1)
                    .ThenBy(entry => Featured(entry) ? 0 : 1)
                    .ThenBy(RankingLatency)
                    .ThenBy(entry => TextOf(entry, "added_at"), StringComparer.Ordinal)
                    .ToList()
                : entries
                    .OrderBy(entry => Featured(entry) ? 0 : 1)
                    .ThenBy(entry => TextOf(entry, "added_at"), StringComparer.Ordinal)
                    .ToList();

        var results = new JsonArray();

        foreach (var entry in entries.Take(ResultLimit))
        {
            results.Add(kind == "space" ? SpaceResult(entry) : ModelResult(entry));
        }

        return results;
    }

    /// <summary>The entries of one role that match what was asked for.</summary>
    /// <param name="kind">The role.</param>
    /// <param name="task">A task or space category to keep, or the empty string for any.</param>
    /// <param name="query">What to look for in a title, a description or an id, or the empty string.</param>
    /// <param name="modality">A modality to keep, or the empty string for any.</param>
    /// <returns>The matching entries, in the order the snapshot lists them.</returns>
    /// <remarks>
    /// A task is the narrower filter and wins when both are given, which is the Python version's
    /// rule: the canvas sends a modality only while no task is chosen. A template whose last check
    /// failed is not a template to offer.
    /// </remarks>
    private static List<JsonObject> Filter(string kind, string task, string query, string modality)
    {
        var wanted = query.ToLowerInvariant().Trim();
        var matches = new List<JsonObject>();

        foreach (var entry in Entries.Value)
        {
            if (TextOf(entry, "kind") != kind || Failed(entry))
            {
                continue;
            }

            if (task.Length > 0)
            {
                if (TextOf(entry, "task") != task && TextOf(entry, "space_category") != task)
                {
                    continue;
                }
            }
            else if (modality.Length > 0 && TextOf(entry, "modality") != modality)
            {
                continue;
            }

            if (wanted.Length > 0 && !Haystack(entry).Contains(wanted, StringComparison.Ordinal))
            {
                continue;
            }

            matches.Add(entry);
        }

        return matches;
    }

    /// <summary>What a search looks through: the title, the description and the id.</summary>
    /// <param name="entry">The template.</param>
    /// <returns>The three fields, joined by a space, lower-cased.</returns>
    /// <remarks>
    /// Joined with a space rather than searched one by one, because the Python version's haystack
    /// lets a query span the boundary — a query that ends one field and starts the next matches
    /// there.
    /// </remarks>
    private static string Haystack(JsonObject entry) =>
        string.Join(' ', TextOf(entry, "title"), TextOf(entry, "description"), TextOf(entry, "id"))
            .ToLowerInvariant();

    /// <summary>How long a template takes, for the ranking.</summary>
    /// <param name="entry">The template.</param>
    /// <returns>The milliseconds, or the stand-in when nothing was measured.</returns>
    /// <remarks>
    /// A latency of zero ranks as unmeasured rather than as instant, which is the Python version's
    /// <c>latency_ms or 999_999</c>: the field is written by a check that either measured something
    /// or did not, and a zero means the second. A negative latency is kept — it is a measurement,
    /// however odd — because Python's <c>or</c> only replaces the two falsy values. The card still
    /// shows what the list says, because the score is a different question.
    /// </remarks>
    private static int RankingLatency(JsonObject entry) =>
        Latency(entry) is { } milliseconds && milliseconds != 0 ? milliseconds : UnmeasuredLatency;

    /// <summary>Whether a template runs without a GPU.</summary>
    /// <param name="entry">The template.</param>
    /// <returns>The list's answer.</returns>
    private static bool WithoutGpu(JsonObject entry) => Truthy(Field(entry, "zero_gpu"));

    /// <summary>Whether the list features a template.</summary>
    /// <param name="entry">The template.</param>
    /// <returns>The list's answer.</returns>
    private static bool Featured(JsonObject entry) => Truthy(Field(entry, "featured"));

    /// <summary>A space as the canvas's card draws it.</summary>
    /// <param name="entry">The template.</param>
    /// <returns>The document.</returns>
    /// <remarks>
    /// The likes, downloads and stage are constants because a curated entry carries none of them:
    /// the Python version reports what the list says rather than asking the hub, and every entry
    /// says the same three things.
    /// </remarks>
    private static JsonObject SpaceResult(JsonObject entry)
    {
        var tags = WithoutGpu(entry) ? new JsonArray("zero-gpu") : new JsonArray();

        return new JsonObject
        {
            ["id"] = TextOrNull(entry, "id"),
            ["likes"] = 0,
            ["trendingScore"] = Latency(entry) ?? 0,
            ["runtime"] = new JsonObject { ["stage"] = "RUNNING" },
            ["pipeline_tag"] = TextOrNull(entry, "task"),
            ["tags"] = tags.DeepClone(),
            ["cardData"] = new JsonObject
            {
                ["title"] = TextOrNull(entry, "title"),
                ["short_description"] = TextOrNull(entry, "description"),
                ["tags"] = tags,
                ["sdk"] = "gradio",
            },
            ["_curated"] = true,
            ["_featured"] = Featured(entry),
            ["_thumbnail"] = Field(entry, "thumbnail")?.DeepClone(),
        };
    }

    /// <summary>A model as the canvas's card draws it.</summary>
    /// <param name="entry">The template.</param>
    /// <returns>The document.</returns>
    private static JsonObject ModelResult(JsonObject entry) => new()
    {
        ["id"] = TextOrNull(entry, "id"),
        ["likes"] = 0,
        ["downloads"] = 0,
        ["pipeline_tag"] = TextOrNull(entry, "task"),
        ["_curated"] = true,
        ["_featured"] = Featured(entry),
        ["_thumbnail"] = Field(entry, "thumbnail")?.DeepClone(),
    };

    /// <summary>The templates the snapshot lists.</summary>
    /// <returns>The entries, or none when the snapshot cannot be read.</returns>
    /// <remarks>
    /// A snapshot that cannot be read is an application that offers no templates rather than one that
    /// fails to start: the canvas asks for them when an author opens the list, and the rest of the
    /// workflow works without them.
    /// </remarks>
    private static List<JsonObject> Load()
    {
        using var stream = typeof(WorkflowCurated).Assembly.GetManifestResourceStream(ResourceName);

        if (stream is null)
        {
            return [];
        }

        using var reader = new StreamReader(stream);

        if (JsonNode.Parse(reader.ReadToEnd()) is not JsonObject payload)
        {
            return [];
        }

        return payload["items"] is JsonArray items
            ? items.OfType<JsonObject>().ToList()
            : [];
    }

    /// <summary>The call's arguments as an array, whether or not the browser sent one.</summary>
    /// <param name="data">What the browser sent.</param>
    /// <returns>The arguments, or an empty array.</returns>
    private static JsonArray Arguments(JsonNode? data) => data as JsonArray ?? [];

    /// <summary>One argument, when it is a string.</summary>
    /// <param name="arguments">The arguments.</param>
    /// <param name="index">Which one.</param>
    /// <returns>The text, or the empty string.</returns>
    /// <remarks>
    /// A non-string argument is read as absent. That is this build's choice rather than the Python
    /// version's: the Python version guards only the modality (<c>isinstance(data[3], str)</c>) and
    /// would either filter everything out (a number where a task belongs) or fail outright (a
    /// number where the query belongs, because it calls <c>.lower()</c> on it). The canvas sends
    /// strings, so the difference only reaches a hand-written request — and there the answer here
    /// is a list rather than a crash.
    /// </remarks>
    private static string Text(JsonArray arguments, int index) =>
        index < arguments.Count && arguments[index] is JsonValue value
        && value.TryGetValue<string>(out var text)
            ? text
            : string.Empty;

    /// <summary>One of a template's fields, as text or as nothing.</summary>
    /// <param name="entry">The template.</param>
    /// <param name="name">The field.</param>
    /// <returns>The text, or <see langword="null"/> when the field is absent.</returns>
    /// <remarks>
    /// A card reports what the list says, so a field the list does not carry is written as null
    /// rather than as an empty string — which is what the Python version's <c>entry.get(name)</c>
    /// produces. The filters use <see cref="TextOf"/> instead, where an absent field and an empty
    /// one are the same answer: neither matches a task.
    /// </remarks>
    private static string? TextOrNull(JsonObject entry, string name) =>
        Field(entry, name) is JsonValue value && value.TryGetValue<string>(out var text) ? text : null;

    /// <summary>One of a template's fields, as text.</summary>
    /// <param name="entry">The template.</param>
    /// <param name="name">The field.</param>
    /// <returns>The text, or the empty string when the field is absent or not text.</returns>
    private static string TextOf(JsonObject entry, string name) =>
        Field(entry, name) is JsonValue value && value.TryGetValue<string>(out var text) ? text : string.Empty;

    /// <summary>One of a template's fields.</summary>
    /// <param name="entry">The template.</param>
    /// <param name="name">The field.</param>
    /// <returns>The field, or <see langword="null"/> when the template does not carry it.</returns>
    private static JsonNode? Field(JsonObject entry, string name) =>
        entry.TryGetPropertyValue(name, out var value) ? value : null;

    /// <summary>The state of a template's last check, as the list records it.</summary>
    /// <param name="entry">The template.</param>
    /// <returns>The status, defaulting to <c>ok</c> when the list records none.</returns>
    /// <remarks>
    /// The default is <c>ok</c>, and an empty string is a status the list wrote rather than a
    /// failure: the Python version's test is <c>if status and status != "ok"</c>, which keeps a
    /// template whose status is blank. Reading a blank as a failure would hide a template the
    /// Python version offers.
    /// </remarks>
    private static string Status(JsonObject entry) =>
        Field(entry, "validation") is JsonObject validation
        && Field(validation, "status") is JsonValue value
        && value.TryGetValue<string>(out var status)
            ? status
            : "ok";

    /// <summary>Whether a template's last check failed, which is what keeps it out of the answers.</summary>
    /// <param name="entry">The template.</param>
    /// <returns><see langword="true"/> when the list records a status other than <c>ok</c>.</returns>
    private static bool Failed(JsonObject entry) => Status(entry) is { Length: > 0 } status && status != "ok";

    /// <summary>How long a template last took to answer, when it was measured.</summary>
    /// <param name="entry">The template.</param>
    /// <returns>The milliseconds, or <see langword="null"/> when the list records none.</returns>
    private static int? Latency(JsonObject entry) =>
        Field(entry, "validation") is JsonObject validation
        && Field(validation, "latency_ms") is JsonValue value
        && value.TryGetValue<int>(out var milliseconds)
            ? milliseconds
            : null;

    /// <summary>Whether a value counts as true.</summary>
    /// <param name="value">The value.</param>
    /// <returns><see langword="true"/> when Python would read it as true.</returns>
    /// <remarks>
    /// The canvas sends <c>false</c>, <c>null</c> or nothing at all for a switch it has not turned
    /// on, and an empty string for a filter it has not filled in. Python's truth test treats all of
    /// them as false, and so does this: a JSON value is true when it is a non-empty string, a
    /// non-zero number, a non-empty container or <c>true</c>.
    /// </remarks>
    private static bool Truthy(JsonNode? value) => value switch
    {
        null => false,
        JsonArray array => array.Count > 0,
        JsonObject entry => entry.Count > 0,
        JsonValue scalar => scalar.GetValueKind() switch
        {
            System.Text.Json.JsonValueKind.True => true,
            System.Text.Json.JsonValueKind.False => false,
            System.Text.Json.JsonValueKind.String => scalar.GetValue<string>().Length > 0,
            System.Text.Json.JsonValueKind.Number => scalar.ToJsonString() != "0",
            _ => false,
        },
        _ => false,
    };

    /// <summary>A set of names as the JSON array the canvas reads.</summary>
    /// <param name="names">The names, already in the order to report them.</param>
    /// <returns>The array.</returns>
    private static JsonArray ArrayOf(IEnumerable<string> names)
    {
        var array = new JsonArray();

        foreach (var name in names)
        {
            array.Add(name);
        }

        return array;
    }
}

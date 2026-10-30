using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// The edit that turns one streamed chunk into the next, in the form the client applies
/// (<c>gradio.utils.diff</c>).
/// </summary>
/// <remarks>
/// <para>
/// A generator's later chunks are not sent whole. The Python version sends the difference between
/// the chunk before and the chunk now, and the frontend applies it to what it already has — which
/// is what keeps a streamed value cheap to deliver when only part of it changed. The encoding is
/// therefore part of the protocol rather than an optimisation: a chunk sent whole where the client
/// expects an edit list is a chunk it cannot apply.
/// </para>
/// <para>
/// An edit is a three-element array: the operation, the path to the value it applies to (a list of
/// keys and indices, empty for the value itself), and the value it carries. The operations are
/// <c>replace</c>, <c>append</c>, <c>delete</c> and <c>add</c>.
/// </para>
/// <para>
/// One place where this cannot be a literal port: the Python version tells two values apart by
/// Python type, so an integer and a float are different types and one replaces the other. JSON has
/// a single number type, so a number is compared to a number here. Nothing this framework streams
/// depends on the distinction, and inventing one would mean deciding which of two .NET number types
/// a JSON number came from.
/// </para>
/// </remarks>
internal static class JsonDiff
{
    /// <summary>The edits that turn <paramref name="previous"/> into <paramref name="current"/>.</summary>
    /// <param name="previous">The chunk the client already has.</param>
    /// <param name="current">The chunk to send.</param>
    /// <returns>The edit list, empty when the two are the same.</returns>
    internal static JsonArray Between(JsonNode? previous, JsonNode? current)
    {
        var edits = new JsonArray();

        foreach (var edit in Compare(previous, current, []))
        {
            edits.Add(edit);
        }

        return edits;
    }

    /// <summary>Collects the edits for one pair of values at one path.</summary>
    /// <remarks>
    /// The edits are returned as a list rather than written into a shared one because that is what
    /// the Python version does, and the difference is observable: the renumbering below walks the
    /// edits of one level, nested ones included, so which edits are in scope when it runs decides
    /// what the client is told to delete.
    /// </remarks>
    private static List<JsonArray> Compare(JsonNode? previous, JsonNode? current, JsonArray path)
    {
        var edits = new List<JsonArray>();

        if (JsonNode.DeepEquals(previous, current))
        {
            return edits;
        }

        if (Kind(previous) != Kind(current))
        {
            edits.Add(Edit("replace", path, current));

            return edits;
        }

        // A string that grew is the commonest thing a streamed value does — tokens arriving one
        // after another — so it is carried as the text that was added rather than as a
        // replacement of the whole string.
        if (previous is JsonValue previousText
            && previousText.TryGetValue<string>(out var before)
            && current is JsonValue currentText
            && currentText.TryGetValue<string>(out var after)
            && after.StartsWith(before, StringComparison.Ordinal))
        {
            edits.Add(Edit("append", path, JsonValue.Create(after[before.Length..])));

            return edits;
        }

        if (previous is JsonArray previousArray && current is JsonArray currentArray)
        {
            return CompareArrays(previousArray, currentArray, path);
        }

        if (previous is JsonObject previousObject && current is JsonObject currentObject)
        {
            CompareObjects(previousObject, currentObject, path, edits);

            return edits;
        }

        edits.Add(Edit("replace", path, current));

        return edits;
    }

    private static List<JsonArray> CompareArrays(JsonArray previous, JsonArray current, JsonArray path)
    {
        var edits = new List<JsonArray>();
        var shared = Math.Min(previous.Count, current.Count);

        for (var index = 0; index < shared; index++)
        {
            edits.AddRange(Compare(previous[index], current[index], Step(path, index)));
        }

        for (var index = shared; index < previous.Count; index++)
        {
            edits.Add(Edit("delete", Step(path, index), null));
        }

        for (var index = shared; index < current.Count; index++)
        {
            edits.Add(Edit("add", Step(path, index), current[index]));
        }

        RenumberDeletes(edits);

        return edits;
    }

    private static void CompareObjects(JsonObject previous, JsonObject current, JsonArray path, List<JsonArray> edits)
    {
        foreach (var (name, value) in previous)
        {
            if (current.TryGetPropertyValue(name, out var now))
            {
                edits.AddRange(Compare(value, now, Step(path, name)));
            }
            else
            {
                edits.Add(Edit("delete", Step(path, name), null));
            }
        }

        foreach (var (name, value) in current)
        {
            if (!previous.ContainsKey(name))
            {
                edits.Add(Edit("add", Step(path, name), value));
            }
        }
    }

    /// <summary>
    /// Moves each deletion's index back by the deletions before it.
    /// </summary>
    /// <param name="edits">The edits of one level, renumbered in place.</param>
    /// <remarks>
    /// A client applies the edits in order, so by the time it reaches the second deletion of a list
    /// the first one has already shortened it. The Python version numbers them for the list as it
    /// is being walked rather than as it was, and a client that deleted the recorded index would
    /// take the wrong element.
    /// </remarks>
    private static void RenumberDeletes(List<JsonArray> edits)
    {
        var seen = 0;

        foreach (var edit in edits)
        {
            if (edit.Count != 3
                || edit[0]?.GetValue<string>() != "delete"
                || edit[1] is not JsonArray { Count: > 0 } target
                || target[^1] is not JsonValue last
                || !last.TryGetValue<int>(out var index))
            {
                continue;
            }

            target[^1] = index - seen;
            seen++;
        }
    }

    /// <summary>One edit: the operation, the path, and the value.</summary>
    private static JsonArray Edit(string operation, JsonArray path, JsonNode? value) =>
        [operation, path.DeepClone(), value?.DeepClone()];

    /// <summary>The path with one more step on it.</summary>
    private static JsonArray Step(JsonArray path, JsonNode? step)
    {
        var extended = path.DeepClone().AsArray();
        extended.Add(step?.DeepClone());

        return extended;
    }

    /// <summary>
    /// Which of JSON's shapes a value has.
    /// </summary>
    /// <remarks>
    /// This is what stands in for the Python version's <c>type(a) is not type(b)</c>: two values of
    /// different shapes cannot be compared step by step, so one replaces the other outright.
    /// </remarks>
    private static string Kind(JsonNode? value) => value switch
    {
        null => "null",
        JsonArray => "array",
        JsonObject => "object",
        JsonValue json when json.TryGetValue<bool>(out _) => "boolean",
        JsonValue json when json.TryGetValue<string>(out _) => "string",
        JsonValue => "number",
        _ => "unknown",
    };
}

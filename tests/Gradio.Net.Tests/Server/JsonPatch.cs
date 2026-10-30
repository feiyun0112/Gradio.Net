using System.Text.Json.Nodes;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The client's half of a streamed answer: applying the edits a chunk is described by.
/// </summary>
/// <remarks>
/// <para>
/// A generator's first chunk is sent whole and every chunk after it is sent as the edits that turn
/// the one before it into it (<c>gradio.utils.diff</c>). The frontend applies them, so a test that
/// wants to know what a browser ends up with has to apply them too — comparing the raw edit lists
/// would assert the encoding rather than the value the user sees.
/// </para>
/// <para>
/// Written here rather than reused from the queue on purpose: a check that shares its reader with
/// the thing it checks cannot find a mistake in the reader.
/// </para>
/// </remarks>
internal static class JsonPatch
{
    /// <summary>The value a chunk's edits produce, applied to the value the client already has.</summary>
    /// <param name="value">The value the client has, or <see langword="null"/> for the first chunk.</param>
    /// <param name="edits">The edits, as the chunk carried them.</param>
    /// <returns>The value after the edits.</returns>
    /// <exception cref="InvalidOperationException">An edit names an operation this does not know.</exception>
    internal static JsonNode? Apply(JsonNode? value, JsonArray edits)
    {
        ArgumentNullException.ThrowIfNull(edits);

        var current = value?.DeepClone();

        foreach (var edit in edits)
        {
            var operation = edit![0]!.GetValue<string>();
            var path = edit[1]!.AsArray();

            current = operation switch
            {
                "replace" => Replace(current, path, edit[2]),
                "append" => Append(current, path, edit[2]),
                "add" => Add(current, path, edit[2]),
                "delete" => Delete(current, path),
                _ => throw new InvalidOperationException($"Unknown edit '{operation}'."),
            };
        }

        return current;
    }

    private static JsonNode? Replace(JsonNode? current, JsonArray path, JsonNode? value)
    {
        if (path.Count == 0)
        {
            return value?.DeepClone();
        }

        Set(Owner(current, path), Last(path), value);

        return current;
    }

    private static JsonNode? Append(JsonNode? current, JsonArray path, JsonNode? value)
    {
        var owner = Owner(current, path);
        var last = Last(path);

        if (owner is JsonObject parent && parent[last] is JsonValue text)
        {
            parent[last] = text.GetValue<string>() + (value?.GetValue<string>() ?? string.Empty);
        }

        return current;
    }

    private static JsonNode? Add(JsonNode? current, JsonArray path, JsonNode? value)
    {
        if (path.Count == 0)
        {
            return value?.DeepClone();
        }

        var owner = Owner(current, path);
        var last = Last(path);

        if (owner is JsonArray array)
        {
            array.Insert(int.Parse(last, System.Globalization.CultureInfo.InvariantCulture), value?.DeepClone());
        }
        else if (owner is JsonObject parent)
        {
            parent[last] = value?.DeepClone();
        }

        return current;
    }

    private static JsonNode? Delete(JsonNode? current, JsonArray path)
    {
        if (path.Count == 0)
        {
            return null;
        }

        var owner = Owner(current, path);
        var last = Last(path);

        if (owner is JsonArray array)
        {
            array.RemoveAt(int.Parse(last, System.Globalization.CultureInfo.InvariantCulture));
        }
        else if (owner is JsonObject parent)
        {
            parent.Remove(last);
        }

        return current;
    }

    /// <summary>The node an edit's path leads to, one step short of its end.</summary>
    private static JsonNode? Owner(JsonNode? current, JsonArray path)
    {
        var node = current;

        for (var step = 0; step < path.Count - 1; step++)
        {
            node = Step(node, path[step]);
        }

        return node;
    }

    private static JsonNode? Step(JsonNode? node, JsonNode? step)
    {
        var name = step!.ToJsonString().Trim('"');

        return node switch
        {
            JsonArray array => array[int.Parse(name, System.Globalization.CultureInfo.InvariantCulture)],
            JsonObject document => document[name],
            _ => null,
        };
    }

    /// <summary>The last step of a path, as the key or index it names.</summary>
    private static string Last(JsonArray path) => path[^1]!.ToJsonString().Trim('"');

    /// <summary>Writes a value at the last step of a path.</summary>
    private static void Set(JsonNode? owner, string last, JsonNode? value)
    {
        switch (owner)
        {
            case JsonArray array:
                array[int.Parse(last, System.Globalization.CultureInfo.InvariantCulture)] = value?.DeepClone();
                break;

            case JsonObject document:
                document[last] = value?.DeepClone();
                break;
        }
    }
}

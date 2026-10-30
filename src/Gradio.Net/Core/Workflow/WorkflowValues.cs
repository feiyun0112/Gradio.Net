using System.Collections.Frozen;
using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// The values a workflow's ports carry, and how they are shaped on the way between a component, a
/// node and a caller.
/// </summary>
/// <remarks>
/// <para>
/// A port's type is the canvas's, so it decides only two things here: whether the value is a
/// reference to a file rather than a scalar, and which element of a multi-value answer feeds a port
/// that did not say. Everything else travels as it is — a node that wants an object is given the
/// object the previous node produced, because the graph has no schema for a value's inside.
/// </para>
/// <para>
/// The media types are the ones the canvas treats as files, which is why a bare path is wrapped
/// into a reference on the way in: an operator is written to receive what the Python client
/// would have sent it, and that is a <c>{path}</c> or <c>{url}</c> object rather than a string.
/// </para>
/// <para>
/// Values arrive in two shapes — CLR values from a component or a handler, and JSON nodes from the
/// graph the canvas saved — so each entry point unwraps the second into the first before deciding
/// anything. A port type that names a shape this build does not know carries whatever it is given:
/// the graph is a document a later canvas may extend, and refusing its values would refuse the
/// whole pipeline over one port.
/// </para>
/// </remarks>
public static class WorkflowValues
{
    /// <summary>The port types that carry a file rather than a value.</summary>
    /// <remarks>
    /// Frozen rather than merely read-only: a <see cref="HashSet{T}"/> behind an
    /// <see cref="IReadOnlySet{T}"/> is still one cast away from being changed for the whole
    /// process, and the analyzer cannot see through the interface either.
    /// </remarks>
    private static readonly FrozenSet<string> _mediaTypes =
        new HashSet<string>(StringComparer.Ordinal)
        {
            "image",
            "audio",
            "video",
            "file",
            "gallery",
            "model3d",
        }.ToFrozenSet(StringComparer.Ordinal);

    /// <summary>Whether a port type carries a file.</summary>
    /// <param name="portType">The port's type.</param>
    /// <returns><see langword="true"/> for a media type.</returns>
    public static bool IsMedia(string? portType) =>
        portType is not null && _mediaTypes.Contains(portType);

    /// <summary>Shapes a component's value into what a node's port expects.</summary>
    /// <param name="value">The value the component produced.</param>
    /// <param name="portType">The type of the port it is going to.</param>
    /// <returns>The value, or a reference to it when the port carries a file.</returns>
    public static object? ToArgument(object? value, string? portType)
    {
        var unwrapped = Unwrap(value);

        if (unwrapped is null || !IsMedia(portType))
        {
            return unwrapped;
        }

        return unwrapped switch
        {
            // Already a reference — the shape a canvas value and a previous node both produce.
            IDictionary<string, object?> => unwrapped,
            string path => new Dictionary<string, object?>(StringComparer.Ordinal) { ["path"] = path },
            _ => unwrapped,
        };
    }

    /// <summary>Whether a value is the shape a port carries.</summary>
    /// <param name="value">The value.</param>
    /// <param name="portType">The port's type.</param>
    /// <returns><see langword="true"/> when the value can travel through the port.</returns>
    /// <remarks>
    /// This is how a port picks its value out of an answer that holds several: the type is a hint
    /// about which element was meant, and an answer that matches nothing falls back to position
    /// rather than failing, because the port is what the caller is waiting on.
    /// </remarks>
    public static bool MatchesPort(object? value, string? portType)
    {
        var unwrapped = Unwrap(value);

        if (unwrapped is null)
        {
            return false;
        }

        if (IsMedia(portType))
        {
            return unwrapped switch
            {
                string reference => IsReference(reference),
                IDictionary<string, object?> reference =>
                    reference.ContainsKey("path") || reference.ContainsKey("url"),
                _ => false,
            };
        }

        return portType switch
        {
            "text" or "markdown" => unwrapped is string,
            "number" => IsNumber(unwrapped),
            "boolean" => unwrapped is bool,
            "json" => unwrapped is IDictionary<string, object?> or System.Collections.IList,
            _ => true,
        };
    }

    /// <summary>Picks the element of an answer that feeds one output port.</summary>
    /// <param name="port">The port.</param>
    /// <param name="portIndex">The port's position among the node's outputs.</param>
    /// <param name="output">The answer's elements, in order.</param>
    /// <param name="portCount">How many output ports the node has.</param>
    /// <returns>The value the port carries.</returns>
    /// <remarks>
    /// A port that names its element gets that one. Otherwise a single port takes the element at its
    /// own position — unless the answer has more elements than the node has ports, in which case the
    /// port has to be matched by shape instead, because position no longer says which is which.
    /// </remarks>
    public static object? PickResponse(
        WorkflowPort port,
        int portIndex,
        IReadOnlyList<object?> output,
        int portCount)
    {
        ArgumentNullException.ThrowIfNull(port);
        ArgumentNullException.ThrowIfNull(output);

        object? primary;
        if (port.OutputIndex is { } explicitIndex)
        {
            // A hand-edited graph can name an element that is not there, including one before the
            // first: the port then falls back to matching by shape rather than throwing at an
            // endpoint over a number the canvas would never have written.
            primary = explicitIndex >= 0 && explicitIndex < output.Count ? output[explicitIndex] : null;
        }
        else if (portCount == 1 && output.Count > 1)
        {
            primary = null;
        }
        else
        {
            primary = portIndex < output.Count ? output[portIndex] : null;
        }

        if (primary is not null && MatchesPort(primary, port.Type))
        {
            return primary;
        }

        foreach (var item in output)
        {
            if (MatchesPort(item, port.Type))
            {
                return item;
            }
        }

        return primary ?? (output.Count > 0 ? output[0] : null);
    }

    /// <summary>Shapes a node's value into what the component showing it expects.</summary>
    /// <param name="value">The value the node produced.</param>
    /// <returns>The value, with a reference narrowed to the path or address it names.</returns>
    /// <remarks>
    /// Only the final value of a subject is narrowed: an intermediate reference travels as the
    /// object it is, so that a downstream operator can still read the path and the address it
    /// carries.
    /// </remarks>
    public static object? FromOutput(object? value) => Unwrap(value) switch
    {
        IDictionary<string, object?> reference when reference.TryGetValue("path", out var path)
            && path is not null => path,
        IDictionary<string, object?> reference when reference.TryGetValue("url", out var url)
            && url is not null => url,
        var other => other,
    };

    /// <summary>The value a free input starts with, as its component can take it.</summary>
    /// <param name="input">The input, whose node carries the value the canvas saved.</param>
    /// <returns>The value, or <see langword="null"/> for one the graph does not pre-fill.</returns>
    /// <remarks>
    /// A reference the browser alone can resolve — a <c>blob:</c> address, which names an object in
    /// that browser's memory — is left out rather than sent on: the page that receives it could not
    /// read it, and a component that fails to load its value is worse than one that starts empty.
    /// </remarks>
    public static object? ReferenceDefault(WorkflowInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var value = FirstValue(
            input.Node.Data,
            input.Node.Outputs.Select(port => port.Id).Concat(input.Node.Inputs.Select(port => port.Id)));

        if (!IsMedia(input.Type))
        {
            return Unwrap(value);
        }

        if (input.Type == "gallery" && Unwrap(value) is System.Collections.IList items)
        {
            var references = new List<object?>();
            foreach (var item in items)
            {
                if (ReferenceOf(item) is { } reference)
                {
                    references.Add(reference);
                }
            }

            return references.Count > 0 ? references : null;
        }

        return ReferenceOf(value);
    }

    /// <summary>Whether a string names something a component can load.</summary>
    private static bool IsReference(string value) =>
        value.StartsWith("http://", StringComparison.Ordinal)
        || value.StartsWith("https://", StringComparison.Ordinal)
        || value.StartsWith("blob:", StringComparison.Ordinal)
        || value.StartsWith("data:", StringComparison.Ordinal)
        || value.StartsWith('/');

    /// <summary>Whether a value is a number, which a boolean is not.</summary>
    private static bool IsNumber(object value) =>
        value is sbyte or byte or short or ushort or int or uint or long or ulong
            or float or double or decimal;

    /// <summary>A JSON value as the CLR value it carries, and anything else as it is.</summary>
    private static object? Unwrap(object? value) =>
        value is JsonNode node ? JsonValues.ToObject(node) : value;

    /// <summary>The first port value the graph gave, in the order the ports are declared.</summary>
    private static JsonNode? FirstValue(JsonObject data, IEnumerable<string> portIds)
    {
        foreach (var portId in portIds)
        {
            if (data[portId] is { } value)
            {
                return value;
            }
        }

        return null;
    }

    /// <summary>A reference the component can load, or <see langword="null"/> for one it cannot.</summary>
    private static string? ReferenceOf(object? value)
    {
        var source = Unwrap(value) switch
        {
            IDictionary<string, object?> reference =>
                TextOf(reference, "path") ?? TextOf(reference, "url"),
            string text => text,
            _ => null,
        };

        if (string.IsNullOrEmpty(source) || source.StartsWith("blob:", StringComparison.Ordinal))
        {
            return null;
        }

        if (source.StartsWith("http://", StringComparison.Ordinal)
            || source.StartsWith("https://", StringComparison.Ordinal)
            || source.StartsWith("data:", StringComparison.Ordinal))
        {
            return source;
        }

        var path = source.StartsWith(MediaCache.FileRoute, StringComparison.Ordinal)
            ? Uri.UnescapeDataString(source[MediaCache.FileRoute.Length..])
            : source;

        return System.IO.File.Exists(path) ? path : null;
    }

    private static string? TextOf(IDictionary<string, object?> reference, string key) =>
        reference.TryGetValue(key, out var value) && value is string text ? text : null;
}

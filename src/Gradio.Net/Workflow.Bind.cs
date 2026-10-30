using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// The graph a workflow is given when it has no file: one node per bound function, wired by the
/// edges the application declared.
/// </summary>
/// <remarks>
/// <para>
/// The text is the canvas's first schema, not the second, and that is deliberate: what an
/// application writes by hand is a <em>draft</em> — functions and wires, with no positions and no
/// settings — and the canvas turns a draft into a saved graph the first time somebody arranges it.
/// Only a saved graph can be run as an endpoint, which is why an application that binds functions
/// and saves no file serves a canvas it can draw and no app view to run.
/// </para>
/// <para>
/// The text is written the way Python's <c>json.dumps</c> writes it, because it is served to the
/// browser as a string and compared with the Python version's: the spacing is part of the value,
/// not of the document that holds it.
/// </para>
/// </remarks>
public sealed partial class Workflow
{
    /// <summary>The kind a bound function is drawn as.</summary>
    /// <remarks>
    /// Not the kind that runs it — a <c>fn</c> operator — but the shape the canvas draws it with:
    /// one box in, one box out, no model settings. The Python version calls it a transform.
    /// </remarks>
    private const string BoundNodeKind = "transform";

    /// <summary>The width a bound function is drawn at, in pixels.</summary>
    private const int BoundNodeWidth = 220;

    /// <summary>Writes the graph a set of bound functions and wires describes.</summary>
    /// <param name="bound">The functions, keyed by the name the graph names them by.</param>
    /// <param name="edges">The wires, or <see langword="null"/> for none.</param>
    /// <param name="name">What the graph calls itself.</param> <returns>The graph as the canvas's
    /// first schema, spelled the way Python writes it.</returns>
    /// <exception cref="ArgumentException">An edge names a function or a port that is not
    /// bound.</exception>
    /// <remarks>
    /// No positions are written. Where a node sits is per-viewer state the canvas keeps in each
    /// visitor's browser storage, so a generated graph declares the graph and lets the canvas
    /// arrange it on first open.
    /// </remarks>
    private static string GraphFromBind(
        IReadOnlyDictionary<string, Delegate> bound,
        IReadOnlyList<WorkflowEdgeSpec>? edges,
        string name)
    {
        var nodes = new JsonArray();

        foreach (var (functionName, function) in bound)
        {
            nodes.Add(NodeFromBind(functionName, function));
        }

        var graph = new JsonObject
        {
            ["version"] = "1",
            ["name"] = name,
            ["nodes"] = nodes,
            ["edges"] = EdgesFromBind(edges, nodes),
        };

        return PythonJson.Dump(graph);
    }

    /// <summary>Describes one bound function as a node.</summary>
    private static JsonObject NodeFromBind(string functionName, Delegate function)
    {
        var parameters = BoundParameters(function);
        var inputs = new JsonArray();

        foreach (var parameter in parameters)
        {
            if (DelegateInvocation.IsSuppliedByRun(parameter.ParameterType))
            {
                continue;
            }

            inputs.Add(new JsonObject
            {
                ["id"] = $"in_{parameter.Name}",
                ["label"] = parameter.Name,
                ["type"] = PortTypeOf(parameter.ParameterType),
            });
        }

        // A function that reads nothing still needs a box to connect: the canvas wires ports, and a
        // node with no inputs could not be wired to anything.
        if (inputs.Count == 0)
        {
            inputs.Add(new JsonObject
            {
                ["id"] = "in_0",
                ["label"] = "input",
                ["type"] = "text",
            });
        }

        var outputs = new JsonArray
        {
            new JsonObject
            {
                ["id"] = "out_0",
                ["label"] = "output",
                ["type"] = PortTypeOf(ReturnTypeOf(function)),
            },
        };

        return new JsonObject
        {
            ["id"] = $"fn_{functionName}",
            ["source"] = "fn",
            ["fn"] = functionName,
            ["kind"] = BoundNodeKind,
            ["label"] = functionName,
            ["width"] = BoundNodeWidth,
            ["inputs"] = inputs,
            ["outputs"] = outputs,
            ["data"] = new JsonObject(),
        };
    }

    /// <summary>Turns the declared wires into edges between the nodes that were built.</summary>
    /// <exception cref="ArgumentException">A wire names a function or a port that is not there.</exception>
    private static JsonArray EdgesFromBind(IReadOnlyList<WorkflowEdgeSpec>? edges, JsonArray nodes)
    {
        var result = new JsonArray();

        for (var index = 0; index < (edges?.Count ?? 0); index++)
        {
            var (fromNode, fromPort, type) = Resolve(edges![index].From, "outputs", nodes);
            var (toNode, toPort, _) = Resolve(edges[index].To, "inputs", nodes);

            result.Add(new JsonObject
            {
                ["id"] = $"edge_{index}",
                ["from_node_id"] = fromNode,
                ["from_port_id"] = fromPort,
                ["to_node_id"] = toNode,
                ["to_port_id"] = toPort,
                ["type"] = type,
            });
        }

        return result;
    }

    /// <summary>Finds the node and port a wire's end names.</summary>
    /// <param name="spec">Either <c>function</c> or <c>function.port</c>.</param>
    /// <param name="ports">Which side of the node to look at.</param>
    /// <param name="nodes">The nodes built so far.</param>
    /// <returns>The node's id, the port's id, and what the port carries.</returns>
    /// <exception cref="ArgumentException">Nothing matches.</exception>
    private static (string Node, string Port, string Type) Resolve(
        string spec,
        string ports,
        JsonArray nodes)
    {
        var separator = spec.IndexOf('.', StringComparison.Ordinal);
        var functionName = (separator < 0 ? spec : spec[..separator]).Trim();
        var hint = separator < 0 ? null : spec[(separator + 1)..].Trim();

        var node = nodes
            .Select(entry => entry as JsonObject)
            .FirstOrDefault(candidate => TextOf(candidate?["fn"]) == functionName)
            ?? throw new ArgumentException(
                $"edges: no function '{functionName}' in the bound functions. "
                + $"Bound: {string.Join(", ", nodes.Select(entry => TextOf(entry?["fn"])))}.");

        var declared = node[ports] as JsonArray ?? [];
        if (declared.Count == 0)
        {
            throw new ArgumentException($"edges: node '{functionName}' has no {ports}.");
        }

        if (string.IsNullOrEmpty(hint))
        {
            var first = declared[0]!;

            return (TextOf(node["id"])!, TextOf(first["id"])!, TextOf(first["type"]) ?? "text");
        }

        var port = declared
            .Select(entry => entry as JsonObject)
            .FirstOrDefault(candidate =>
                TextOf(candidate?["label"]) == hint || TextOf(candidate?["id"]) == hint)
            ?? throw new ArgumentException(
                $"edges: no {ports.TrimEnd('s')} port '{hint}' on '{functionName}'. "
                + $"Available: {string.Join(", ", declared.Select(entry => TextOf(entry?["label"])))}.");

        return (TextOf(node["id"])!, TextOf(port["id"])!, TextOf(port["type"]) ?? "text");
    }

    /// <summary>The parameters a bound function declares, in the order it declares them.</summary>
    /// <remarks>
    /// The method's own signature rather than the delegate type's: a <see cref="Func{T,TResult}"/>
    /// names its arguments <c>arg1</c>, <c>arg2</c>, while the method the application wrote names
    /// them after what they are — and those names become the node's ports, which is what the canvas
    /// shows an author.
    /// </remarks>
    private static ParameterInfo[] BoundParameters(Delegate function) =>
        function.Method.GetParameters();

    /// <summary>The port type a CLR type is drawn as.</summary>
    /// <remarks>
    /// Everything that is not a number or a flag carries text, which is what the canvas falls back
    /// to for a value it does not model: a string, a list, a dictionary and an object all round-trip
    /// through the text port as JSON.
    /// </remarks>
    private static string PortTypeOf(Type type)
    {
        var actual = Nullable.GetUnderlyingType(type) ?? type;

        if (actual == typeof(bool))
        {
            return "boolean";
        }

        return actual == typeof(int) || actual == typeof(long) || actual == typeof(short)
            || actual == typeof(byte) || actual == typeof(uint) || actual == typeof(ulong)
            || actual == typeof(ushort) || actual == typeof(sbyte)
            || actual == typeof(float) || actual == typeof(double) || actual == typeof(decimal)
            ? "number"
            : "text";
    }

    /// <summary>What a bound function answers with, as a type rather than as a task of one.</summary>
    private static Type ReturnTypeOf(Delegate function)
    {
        var type = function.Method.ReturnType;

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
        {
            return type.GetGenericArguments()[0];
        }

        return type == typeof(Task) ? typeof(object) : type;
    }

    /// <summary>A bound function's name as an endpoint name may spell it.</summary>
    /// <param name="name">The name the application bound the function under.</param>
    /// <returns>The name with everything but letters, digits, underscores and dashes replaced.</returns>
    private static string Sanitized(string name)
    {
        var builder = new StringBuilder(name.Length);

        foreach (var character in name)
        {
            builder.Append(char.IsAsciiLetterOrDigit(character) || character is '_' or '-' ? character : '_');
        }

        return builder.ToString();
    }

    /// <summary>The name a graph file gives the workflow.</summary>
    /// <param name="fileName">The file's name without its directory.</param>
    /// <returns>The name in title case, with underscores and dashes read as spaces.</returns>
    /// <remarks>
    /// This is Python's <c>str.title</c>: a letter is upper case when the character before it is
    /// not a letter, and lower case otherwise. So <c>my_workflow2</c> becomes <c>My Workflow2</c>
    /// and <c>a1b</c> becomes <c>A1B</c> — the digit starts a word as far as that rule is
    /// concerned.
    /// </remarks>
    private static string TitleOf(string fileName)
    {
        var text = Path.GetFileNameWithoutExtension(fileName).Replace('_', ' ').Replace('-', ' ');
        var builder = new StringBuilder(text.Length);
        var afterLetter = false;

        foreach (var character in text)
        {
            builder.Append(char.IsLetter(character)
                ? afterLetter
                    ? char.ToLowerInvariant(character)
                    : char.ToUpperInvariant(character)
                : character);

            afterLetter = char.IsLetter(character);
        }

        return builder.ToString();
    }

    private static string? TextOf(JsonNode? value) =>
        value is JsonValue scalar && scalar.TryGetValue<string>(out var text)
            ? text
            : value is null ? null : Convert.ToString(value, CultureInfo.InvariantCulture);
}

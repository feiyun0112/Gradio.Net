using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// The graph a workflow draws — its nodes, the edges between them, and what can be derived from
/// those.
/// </summary>
/// <remarks>
/// <para>
/// This is the document the canvas saves, read into the three roles it names: <c>references</c> are
/// the values a caller supplies, <c>operators</c> are the steps that compute, and <c>subjects</c>
/// are the values the application shows. Everything the rest of the framework needs is derived
/// here rather than in the two places that would otherwise each derive it: which nodes feed an
/// output, which values the caller must supply, and which outputs belong to one pipeline.
/// </para>
/// <para>
/// Only schema version 2 is executable, which is the Python version's rule as well: the canvas
/// migrates a version 1 graph as it loads it, so a file that is still version 1 is a graph the
/// application generates from bound functions rather than one it saved. <see cref="FromJson"/>
/// answers <see langword="null"/> for it, and the application then has no outputs to expose.
/// </para>
/// <para>
/// The node fields the framework does not act on are kept in <see cref="WorkflowNode.Raw"/> instead
/// of being modelled: a graph is the canvas's document, and a field this build has never heard of
/// is one a later canvas may still understand.
/// </para>
/// </remarks>
public sealed class WorkflowGraph
{
    /// <summary>The schema version this build executes.</summary>
    public const string CurrentSchema = "2";

    private const string DefaultLabel = "output";

    private readonly Dictionary<string, WorkflowNode> _nodes = new(StringComparer.Ordinal);

    /// <summary>Reads a graph, refusing one that is not a graph.</summary>
    /// <param name="data">The graph as a JSON object.</param>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// A node has no id or shares one with another, a node or edge is not an object, a list field is
    /// not a list, or an edge does not name all four of its ends.
    /// </exception>
    /// <remarks>
    /// The checks are the ones that make the rest of the class total: every edge names nodes that
    /// exist, every node has an id to be named by, and no id means two things. A graph that fails
    /// them is refused here rather than producing a canvas that draws half of it.
    /// </remarks>
    public WorkflowGraph(JsonObject data)
    {
        ArgumentNullException.ThrowIfNull(data);

        Name = TextOf(data["name"]) ?? "Workflow";
        References = ReadNodes(data, "references", "reference");
        Operators = ReadNodes(data, "operators", "operator");
        Subjects = ReadNodes(data, "subjects", "subject");
        Edges = ReadEdges(data);
    }

    /// <summary>What the graph calls itself, which becomes the application's title.</summary>
    public string Name { get; }

    /// <summary>The values a caller supplies.</summary>
    public IReadOnlyList<WorkflowNode> References { get; }

    /// <summary>The steps that compute.</summary>
    public IReadOnlyList<WorkflowNode> Operators { get; }

    /// <summary>The values the application shows.</summary>
    public IReadOnlyList<WorkflowNode> Subjects { get; }

    /// <summary>The connections between them.</summary>
    public IReadOnlyList<WorkflowEdge> Edges { get; }

    /// <summary>Reads a graph from the text a workflow file holds.</summary>
    /// <param name="text">The file's text, or <see langword="null"/> for a file that is not there.</param>
    /// <returns>
    /// The graph, or <see langword="null"/> when the text is absent, is not a JSON object, declares
    /// a schema version other than <see cref="CurrentSchema"/>, or is not a graph.
    /// </returns>
    /// <remarks>
    /// A graph that cannot be read is not an error here: an application starts on whatever the file
    /// happens to hold, and a half-written or older file leaves it with no outputs rather than
    /// refusing to start. The canvas is where such a graph is reported, because that is where
    /// somebody can fix it.
    /// </remarks>
    public static WorkflowGraph? FromJson(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        JsonNode? parsed;
        try
        {
            parsed = JsonNode.Parse(text);
        }
        catch (JsonException)
        {
            return null;
        }

        if (parsed is not JsonObject data || TextOf(data["schema_version"]) != CurrentSchema)
        {
            return null;
        }

        try
        {
            return new WorkflowGraph(data);
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    /// <summary>The node an id names.</summary>
    /// <param name="id">The node's id.</param>
    /// <returns>The node, or <see langword="null"/> for an id the graph does not have.</returns>
    public WorkflowNode? NodeOf(string id) =>
        id is not null && _nodes.TryGetValue(id, out var node) ? node : null;

    /// <summary>Whether an edge ends at a node.</summary>
    /// <param name="nodeId">The node's id.</param>
    /// <returns><see langword="true"/> when something feeds it.</returns>
    /// <remarks>
    /// A reference nothing feeds is a value the caller supplies; one that is fed is a value the
    /// graph computes, which is how a reference can stand for a constant the canvas has rewired.
    /// </remarks>
    public bool HasIncoming(string nodeId) =>
        Edges.Any(edge => string.Equals(edge.ToNodeId, nodeId, StringComparison.Ordinal));

    /// <summary>The edge that feeds one port of a node.</summary>
    /// <param name="nodeId">The node's id.</param>
    /// <param name="portId">The input port's id.</param>
    /// <returns>The edge, or <see langword="null"/> when the port takes its own value.</returns>
    public WorkflowEdge? IncomingEdge(string nodeId, string portId) =>
        Edges.FirstOrDefault(edge =>
            string.Equals(edge.ToNodeId, nodeId, StringComparison.Ordinal)
            && string.Equals(edge.ToPortId, portId, StringComparison.Ordinal));

    /// <summary>Orders a set of nodes so that every node follows the ones that feed it.</summary>
    /// <param name="nodeIds">The nodes to order.</param>
    /// <returns>The nodes, dependencies first.</returns>
    /// <exception cref="WorkflowExecutionException">The nodes contain a cycle.</exception>
    /// <remarks>
    /// Edges that leave the set are ignored, which is what makes this usable on a subgraph: a
    /// subject's the Python version nodes are ordered among themselves, and an edge from a node
    /// outside the set is one the subgraph does not need.
    /// </remarks>
    public IReadOnlyList<string> TopologicalOrder(IEnumerable<string> nodeIds)
    {
        ArgumentNullException.ThrowIfNull(nodeIds);

        var ids = nodeIds.Distinct(StringComparer.Ordinal).ToList();
        var included = new HashSet<string>(ids, StringComparer.Ordinal);
        var indegree = ids.ToDictionary(id => id, _ => 0, StringComparer.Ordinal);
        var successors = ids.ToDictionary(id => id, _ => new List<string>(), StringComparer.Ordinal);

        foreach (var edge in Edges)
        {
            if (!included.Contains(edge.FromNodeId) || !included.Contains(edge.ToNodeId))
            {
                continue;
            }

            indegree[edge.ToNodeId]++;
            successors[edge.FromNodeId].Add(edge.ToNodeId);
        }

        var ready = new Queue<string>(ids.Where(id => indegree[id] == 0));
        var order = new List<string>(ids.Count);

        while (ready.Count > 0)
        {
            var id = ready.Dequeue();
            order.Add(id);

            foreach (var successor in successors[id])
            {
                if (--indegree[successor] == 0)
                {
                    ready.Enqueue(successor);
                }
            }
        }

        // A cycle leaves nodes whose indegree never reached zero. The canvas cannot draw one, but a
        // hand-edited file can, and dropping them silently would run a graph that looks complete.
        if (order.Count != ids.Count)
        {
            throw new WorkflowExecutionException(
                "Workflow contains a cycle and cannot be executed.");
        }

        return order;
    }

    /// <summary>Every node that feeds one node, including the node itself.</summary>
    /// <param name="targetId">The node to trace back from.</param>
    /// <returns>The nodes, as a set.</returns>
    /// <remarks>
    /// An edge whose source node does not exist is skipped rather than followed: a hand-edited file
    /// can name a node it never declares, and including it would fail later at the point where the
    /// node is looked up rather than here, where the edge can simply be ignored.
    /// </remarks>
    public IReadOnlySet<string> UpstreamNodeIds(string targetId)
    {
        var included = new HashSet<string>(StringComparer.Ordinal) { targetId };
        var pending = new Queue<string>([targetId]);

        while (pending.Count > 0)
        {
            var current = pending.Dequeue();

            foreach (var edge in Edges)
            {
                if (string.Equals(edge.ToNodeId, current, StringComparison.Ordinal)
                    && _nodes.ContainsKey(edge.FromNodeId)
                    && included.Add(edge.FromNodeId))
                {
                    pending.Enqueue(edge.FromNodeId);
                }
            }
        }

        return included;
    }

    /// <summary>The values a caller supplies for a set of nodes.</summary>
    /// <param name="subgraphIds">The nodes being run.</param>
    /// <returns>The reference nodes nothing feeds, in the order the graph declares them.</returns>
    /// <remarks>
    /// Declaration order rather than topological order: these become an endpoint's parameters, and
    /// reordering them would change how a client that already calls it has to call it.
    /// </remarks>
    public IReadOnlyList<WorkflowInput> FreeInputs(IReadOnlySet<string> subgraphIds)
    {
        ArgumentNullException.ThrowIfNull(subgraphIds);

        var inputs = new List<WorkflowInput>();

        foreach (var reference in References)
        {
            if (!subgraphIds.Contains(reference.Id) || HasIncoming(reference.Id))
            {
                continue;
            }

            var port = reference.Outputs.Count > 0 ? reference.Outputs[0] : null;
            inputs.Add(new WorkflowInput(
                reference,
                port,
                port?.Type ?? reference.AssetType ?? "text",
                reference.Label));
        }

        return inputs;
    }

    /// <summary>The values a caller supplies to run one group of outputs.</summary>
    /// <param name="group">The group's output nodes.</param>
    /// <returns>The reference nodes nothing feeds, in the order the graph declares them.</returns>
    /// <remarks>
    /// The union of each output's the Python version nodes, so that a value feeding two outputs of
    /// the same group is one parameter rather than two the caller has to keep in step.
    /// </remarks>
    public IReadOnlyList<WorkflowInput> GroupFreeInputs(IReadOnlyList<WorkflowNode> group)
    {
        ArgumentNullException.ThrowIfNull(group);

        var upstream = new HashSet<string>(StringComparer.Ordinal);
        foreach (var subject in group)
        {
            upstream.UnionWith(UpstreamNodeIds(subject.Id));
        }

        return FreeInputs(upstream);
    }

    /// <summary>The pipelines a graph's outputs divide into, each with the name of its endpoint.</summary>
    /// <returns>
    /// One group per set of outputs connected through edges, ordered by the declaration of each
    /// group's first output.
    /// </returns>
    /// <remarks>
    /// Connectedness is taken over the edges as if they had no direction: two outputs are one
    /// pipeline when some node feeds both, which is the only reason running them together saves
    /// anything. Names are made unique the way the Python version makes them — by appending an
    /// underscore — so that two groups whose first outputs share a label still get two endpoints.
    /// </remarks>
    public IReadOnlyList<WorkflowGroup> Groups()
    {
        var parent = _nodes.Keys.ToDictionary(id => id, id => id, StringComparer.Ordinal);

        foreach (var edge in Edges)
        {
            if (parent.ContainsKey(edge.FromNodeId) && parent.ContainsKey(edge.ToNodeId))
            {
                parent[Find(parent, edge.FromNodeId)] = Find(parent, edge.ToNodeId);
            }
        }

        var groups = new Dictionary<string, List<WorkflowNode>>(StringComparer.Ordinal);
        var order = new List<string>();

        foreach (var subject in Subjects)
        {
            var root = Find(parent, subject.Id);
            if (!groups.TryGetValue(root, out var members))
            {
                members = [];
                groups[root] = members;
                order.Add(root);
            }

            members.Add(subject);
        }

        var used = new HashSet<string>(StringComparer.Ordinal);
        var result = new List<WorkflowGroup>(order.Count);

        foreach (var root in order)
        {
            var members = groups[root];
            var apiName = Slugify(members[0].Label);

            while (!used.Add(apiName))
            {
                apiName += "_";
            }

            result.Add(new WorkflowGroup(members, apiName));
        }

        return result;
    }

    /// <summary>What an output node carries.</summary>
    /// <param name="subject">The output node.</param>
    /// <returns>Its first input port's type, or the node's own asset type, or text.</returns>
    public static string SubjectOutputType(WorkflowNode subject)
    {
        ArgumentNullException.ThrowIfNull(subject);

        return subject.Inputs.Count > 0
            ? subject.Inputs[0].Type
            : subject.AssetType ?? "text";
    }

    /// <summary>The endpoints a graph exposes, as the canvas's API panel describes them.</summary>
    /// <returns>One endpoint per group, in the order the groups are declared.</returns>
    public IReadOnlyList<WorkflowEndpoint> DescribeEndpoints()
    {
        var endpoints = new List<WorkflowEndpoint>();

        foreach (var group in Groups())
        {
            var inputs = GroupFreeInputs(group.Subjects);

            endpoints.Add(new WorkflowEndpoint(
                group.ApiName,
                LabelOf(group.Subjects[0]),
                [.. inputs.Select((input, index) => new WorkflowEndpointParameter(
                    input.Label,
                    $"in_{index}",
                    input.Type,
                    PythonInputType(input.Type)))],
                [.. group.Subjects.Select(subject =>
                {
                    var type = SubjectOutputType(subject);
                    return new WorkflowEndpointReturn(LabelOf(subject), type, PythonType(type));
                })]));
        }

        return endpoints;
    }

    /// <summary>Turns a label into a name an endpoint can carry.</summary>
    /// <param name="label">The label, or <see langword="null"/> for none.</param>
    /// <returns>The label in lower case, with every run of other characters replaced by one underscore.</returns>
    /// <remarks>
    /// <c>endpoint</c> for a label with nothing usable in it, so that an unnamed output still gets an
    /// address rather than an empty one.
    /// </remarks>
    public static string Slugify(string? label)
    {
        var text = (label ?? string.Empty).Trim().ToLowerInvariant();
        var slug = new StringBuilder(text.Length);
        var separated = false;

        foreach (var character in text)
        {
            if (char.IsAsciiLetterOrDigit(character))
            {
                if (separated)
                {
                    slug.Append('_');
                    separated = false;
                }

                slug.Append(character);
            }
            else if (slug.Length > 0)
            {
                // Runs collapse into the one underscore written before the next usable character, and
                // a run at the end is never written at all — which is what stripping "_" does.
                separated = true;
            }
        }

        return slug.Length == 0 ? "endpoint" : slug.ToString();
    }

    /// <summary>A node's label, or the default the Python version gives an unlabelled
    /// one.</summary>
    private static string LabelOf(WorkflowNode node) =>
        node.Label.Length > 0 ? node.Label : DefaultLabel;

    /// <summary>The Python type a port type is called in the API panel's snippets.</summary>
    private static string PythonType(string portType) => portType switch
    {
        "markdown" => "str",
        "number" => "float",
        "boolean" => "bool",
        "image" or "audio" or "video" or "model3d" or "3d" or "file" => "filepath",
        "gallery" => "list[filepath]",
        "dataframe" => "list[list]",
        "json" => "dict",
        _ => "str",
    };

    /// <summary>The Python type a caller passes for a port type.</summary> <remarks>JSON is entered
    /// as text: the canvas edits it as text and passes it on unchanged.</remarks>
    private static string PythonInputType(string portType) =>
        portType == "json" ? "str" : PythonType(portType);

    /// <summary>The representative of a node's set, with the path to it compressed.</summary>
    private static string Find(Dictionary<string, string> parent, string id)
    {
        var root = id;
        while (!string.Equals(parent[root], root, StringComparison.Ordinal))
        {
            root = parent[root];
        }

        while (!string.Equals(parent[id], root, StringComparison.Ordinal))
        {
            var next = parent[id];
            parent[id] = root;
            id = next;
        }

        return root;
    }

    private List<WorkflowNode> ReadNodes(JsonObject data, string field, string role)
    {
        var list = data[field];
        if (list is null)
        {
            return [];
        }

        if (list is not JsonArray array)
        {
            throw new ArgumentException(
                $"A workflow's '{field}' must be a list.", nameof(data));
        }

        var nodes = new List<WorkflowNode>(array.Count);

        for (var index = 0; index < array.Count; index++)
        {
            if (array[index] is not JsonObject node)
            {
                throw new ArgumentException(
                    $"Workflow {role} at index {index} must be an object.", nameof(data));
            }

            var id = RequiredText(node, "id", role, index, data);
            if (!_nodes.TryAdd(id, ReadNode(node, id, role)))
            {
                throw new ArgumentException(
                    $"Workflow contains duplicate node id: {id}", nameof(data));
            }

            nodes.Add(_nodes[id]);
        }

        return nodes;
    }

    private static WorkflowNode ReadNode(JsonObject node, string id, string role) =>
        new(
            id,
            role,
            TextOf(node["kind"]),
            TextOf(node["fn"]),
            TextOf(node["label"]) ?? string.Empty,
            ReadPorts(node, "inputs", role, id),
            ReadPorts(node, "outputs", role, id),
            node["data"] is JsonObject data ? (JsonObject)data.DeepClone() : [],
            TextOf(node["asset_type"]),
            (JsonObject)node.DeepClone());

    private static List<WorkflowPort> ReadPorts(JsonObject node, string field, string role, string id)
    {
        if (node[field] is not JsonArray array)
        {
            return [];
        }

        var ports = new List<WorkflowPort>(array.Count);

        foreach (var entry in array)
        {
            if (entry is not JsonObject port)
            {
                throw new ArgumentException(
                    $"A workflow {role}'s '{field}' must hold objects; node '{id}' holds something else.");
            }

            ports.Add(new WorkflowPort(
                TextOf(port["id"]) ?? string.Empty,
                TextOf(port["label"]) ?? string.Empty,
                TextOf(port["type"]) ?? "text",
                port["required"]?.GetValueKind() == JsonValueKind.True,
                port["output_index"] is JsonValue index && index.TryGetValue<int>(out var position)
                    ? position
                    : null));
        }

        return ports;
    }

    private static List<WorkflowEdge> ReadEdges(JsonObject data)
    {
        if (data["edges"] is null)
        {
            return [];
        }

        if (data["edges"] is not JsonArray array)
        {
            throw new ArgumentException("A workflow's 'edges' must be a list.", nameof(data));
        }

        var edges = new List<WorkflowEdge>(array.Count);

        for (var index = 0; index < array.Count; index++)
        {
            if (array[index] is not JsonObject edge)
            {
                throw new ArgumentException(
                    $"Workflow edge at index {index} must be an object.", nameof(data));
            }

            edges.Add(new WorkflowEdge(
                TextOf(edge["id"]) ?? string.Empty,
                RequiredText(edge, "from_node_id", "edge", index, data),
                RequiredText(edge, "from_port_id", "edge", index, data),
                RequiredText(edge, "to_node_id", "edge", index, data),
                RequiredText(edge, "to_port_id", "edge", index, data),
                TextOf(edge["type"]) ?? "text"));
        }

        return edges;
    }

    private static string RequiredText(
        JsonObject owner,
        string field,
        string role,
        int index,
        JsonObject data)
    {
        var text = TextOf(owner[field]);

        return !string.IsNullOrEmpty(text)
            ? text
            : throw new ArgumentException(
                $"Workflow {role} at index {index} is missing {field}.", nameof(data));
    }

    /// <summary>A JSON value's text, or <see langword="null"/> when it is not a string.</summary>
    private static string? TextOf(JsonNode? value) =>
        value is JsonValue scalar && scalar.TryGetValue<string>(out var text) ? text : null;
}

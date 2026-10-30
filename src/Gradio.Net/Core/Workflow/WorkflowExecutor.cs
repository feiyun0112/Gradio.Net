namespace Gradio.Net;
/// <summary>
/// Runs a workflow's outputs on the server: the same graph the canvas runs in the browser, executed
/// once per group of outputs so that a workflow can also be called as an endpoint
/// </summary>
/// <remarks>
/// <para>
/// The canvas runs a workflow in the browser, node by node, because that is what an author watches
/// happen. An API caller has no browser, so the same order has to be derived again here: the nodes
/// that feed an output, in dependency order, each one fed by the nodes before it. Both paths exist
/// the Python version for the same reason, and both must agree — which is why the ordering and the
/// subgraph are derived from the graph rather than written out twice.
/// </para>
/// <para>
/// A node is run by the <see cref="WorkflowOperator"/> registered for its kind. The kinds this
/// build can run are the ones the application registered: bound functions are registered by the
/// application that built the workflow, and the three kinds that call a service have no caller
/// unless the application supplies one. A node of a kind with no caller is a failure
/// that names the kind, which is a better answer than an empty value.
/// </para>
/// </remarks>
public sealed class WorkflowExecutor
{
    private readonly WorkflowGraph _graph;
    private readonly IReadOnlyDictionary<string, WorkflowOperator> _operators;

    /// <summary>Creates an executor for one graph.</summary>
    /// <param name="graph">The graph to run.</param>
    /// <param name="operators">What runs each kind of node, keyed by kind.</param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    public WorkflowExecutor(
        WorkflowGraph graph,
        IReadOnlyDictionary<string, WorkflowOperator> operators)
    {
        ArgumentNullException.ThrowIfNull(graph);
        ArgumentNullException.ThrowIfNull(operators);

        _graph = graph;
        _operators = operators;
    }

    /// <summary>Runs the nodes that feed one output, and answers what it carries.</summary>
    /// <param name="subjectId">The output node's id.</param>
    /// <param name="inputs">The values the caller supplied, keyed by reference node id.</param>
    /// <param name="cancellationToken">Cancels the run.</param>
    /// <returns>The output's value.</returns>
    /// <exception cref="WorkflowExecutionException">The run failed.</exception>
    public async Task<object?> RunAsync(
        string subjectId,
        IReadOnlyDictionary<string, object?> inputs,
        CancellationToken cancellationToken)
    {
        var results = await RunManyAsync([subjectId], inputs, cancellationToken).ConfigureAwait(false);

        return results[0];
    }

    /// <summary>Runs the nodes that feed several outputs, in one pass.</summary>
    /// <param name="subjectIds">The output nodes' ids, in the order the answer is returned.</param>
    /// <param name="inputs">The values the caller supplied, keyed by reference node id.</param>
    /// <param name="cancellationToken">Cancels the run.</param>
    /// <returns>One value per output, in the order asked for.</returns>
    /// <exception cref="WorkflowExecutionException">The run failed.</exception>
    /// <remarks>
    /// One pass rather than one per output, so that a node feeding two outputs runs once: a model
    /// call shared by two results would otherwise be paid for twice.
    /// </remarks>
    public async Task<IReadOnlyList<object?>> RunManyAsync(
        IReadOnlyList<string> subjectIds,
        IReadOnlyDictionary<string, object?> inputs,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(subjectIds);
        ArgumentNullException.ThrowIfNull(inputs);

        foreach (var subjectId in subjectIds)
        {
            if (_graph.NodeOf(subjectId) is not { Role: "subject" })
            {
                throw new WorkflowExecutionException($"No such workflow output: {subjectId}");
            }
        }

        var upstream = new HashSet<string>(StringComparer.Ordinal);
        foreach (var subjectId in subjectIds)
        {
            upstream.UnionWith(_graph.UpstreamNodeIds(subjectId));
        }

        // Declaration order, so that two runs of the same graph visit independent nodes in the same
        // order: the topological sort is only free to choose among nodes that do not feed each other.
        var order = _graph.TopologicalOrder(
            _graph.References.Concat(_graph.Operators).Concat(_graph.Subjects)
                .Select(node => node.Id)
                .Where(upstream.Contains));

        var values = new Dictionary<string, Dictionary<string, object?>>(StringComparer.Ordinal);

        foreach (var nodeId in order)
        {
            await ExecuteNodeAsync(nodeId, values, inputs, cancellationToken).ConfigureAwait(false);
        }

        var outputs = new List<object?>(subjectIds.Count);
        foreach (var subjectId in subjectIds)
        {
            var subject = _graph.NodeOf(subjectId)!;

            outputs.Add(subject.Inputs.Count == 0
                ? null
                : WorkflowValues.FromOutput(
                    values.TryGetValue(subjectId, out var bucket)
                        && bucket.TryGetValue(subject.Inputs[0].Id, out var value)
                        ? value
                        : null));
        }

        return outputs;
    }

    private async ValueTask ExecuteNodeAsync(
        string nodeId,
        Dictionary<string, Dictionary<string, object?>> values,
        IReadOnlyDictionary<string, object?> inputs,
        CancellationToken cancellationToken)
    {
        var node = _graph.NodeOf(nodeId)!;

        switch (node.Role)
        {
            case "reference" when _graph.HasIncoming(nodeId):
            case "subject":
                Relay(node, values);
                return;

            case "reference":
                Seed(node, values, inputs);
                return;

            default:
                await RunOperatorAsync(node, values, cancellationToken).ConfigureAwait(false);
                return;
        }
    }

    /// <summary>Gives a reference node the value the caller supplied.</summary>
    private static void Seed(
        WorkflowNode node,
        Dictionary<string, Dictionary<string, object?>> values,
        IReadOnlyDictionary<string, object?> inputs)
    {
        var value = inputs.TryGetValue(node.Id, out var supplied) ? supplied : null;
        var bucket = new Dictionary<string, object?>(StringComparer.Ordinal);

        foreach (var port in node.Outputs)
        {
            bucket[port.Id] = WorkflowValues.ToArgument(value, port.Type);
        }

        values[node.Id] = bucket;
    }

    /// <summary>Passes a node's single input value through to everything it produces.</summary>
    /// <remarks>
    /// A reference the graph feeds is a value it computed rather than one the caller supplies, and a
    /// subject is where a pipeline stops: both take the one value arriving and hand it on unchanged,
    /// which is what makes an output port of a reference name the same value as its input.
    /// </remarks>
    private void Relay(WorkflowNode node, Dictionary<string, Dictionary<string, object?>> values)
    {
        var bucket = new Dictionary<string, object?>(StringComparer.Ordinal);

        if (node.Inputs.Count > 0)
        {
            var resolved = Resolve(node, values);
            var value = resolved[node.Inputs[0].Id];

            bucket[node.Inputs[0].Id] = value;
            foreach (var port in node.Outputs)
            {
                bucket[port.Id] = value;
            }
        }

        values[node.Id] = bucket;
    }

    /// <summary>Runs an operator and gives its ports the elements of what it answered.</summary>
    private async ValueTask RunOperatorAsync(
        WorkflowNode node,
        Dictionary<string, Dictionary<string, object?>> values,
        CancellationToken cancellationToken)
    {
        var resolved = Resolve(node, values);
        Require(node, resolved);

        var kind = node.Kind ?? "space";
        if (!_operators.TryGetValue(kind, out var run))
        {
            throw new WorkflowExecutionException(
                $"No executor available for '{kind}' nodes.");
        }

        var arguments = node.Inputs.Select(port => resolved[port.Id]).ToList();

        IReadOnlyList<object?> answer;
        try
        {
            answer = await run(node, arguments, cancellationToken).ConfigureAwait(false);
        }
        catch (WorkflowExecutionException)
        {
            throw;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception error)
        {
            // The node's own words reach the caller: an operator that failed knows more about why
            // than the framework does, and "the workflow failed" tells nobody what to fix.
            throw new WorkflowExecutionException(error.Message, error);
        }

        var bucket = new Dictionary<string, object?>(StringComparer.Ordinal);
        for (var index = 0; index < node.Outputs.Count; index++)
        {
            bucket[node.Outputs[index].Id] = WorkflowValues.PickResponse(
                node.Outputs[index],
                index,
                answer,
                node.Outputs.Count);
        }

        values[node.Id] = bucket;
    }

    /// <summary>What each of a node's input ports holds, from an edge or from the node itself.</summary>
    private Dictionary<string, object?> Resolve(
        WorkflowNode node,
        Dictionary<string, Dictionary<string, object?>> values)
    {
        var resolved = new Dictionary<string, object?>(StringComparer.Ordinal);

        foreach (var port in node.Inputs)
        {
            if (_graph.IncomingEdge(node.Id, port.Id) is { } edge)
            {
                resolved[port.Id] = values.TryGetValue(edge.FromNodeId, out var bucket)
                    && bucket.TryGetValue(edge.FromPortId, out var value)
                    ? value
                    : null;
            }
            else
            {
                resolved[port.Id] = node.Data[port.Id] is { } declared
                    ? WorkflowValues.ToArgument(JsonValues.ToObject(declared), port.Type)
                    : null;
            }
        }

        return resolved;
    }

    /// <summary>Refuses to run a node whose required input has no value.</summary>
    private static void Require(WorkflowNode node, Dictionary<string, object?> resolved)
    {
        foreach (var port in node.Inputs)
        {
            if (port.Required && resolved[port.Id] is null)
            {
                throw new WorkflowExecutionException(
                    $"\"{(port.Label.Length > 0 ? port.Label : port.Id)}\" is required by "
                    + $"\"{(node.Label.Length > 0 ? node.Label : node.Id)}\" but was not provided");
            }
        }
    }
}

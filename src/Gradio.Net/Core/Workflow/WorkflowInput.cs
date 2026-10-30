namespace Gradio.Net;
/// <summary>
/// A value a workflow's caller supplies: a reference node nothing feeds, and the port it leaves by
/// </summary>
/// <param name="Node">The reference node.</param>
/// <param name="Port">Its output port, or <see langword="null"/> for a reference that declares none.</param>
/// <param name="Type">What the value is: the port's type, or the node's asset type when it has no port.</param>
/// <param name="Label">What the input is called, which is the node's label.</param>
/// <remarks>
/// These are the parameters of the endpoint an application gets for a workflow: everything the
/// graph cannot compute for itself, in the order the graph declares it. The order is the declaration
/// order rather than the topological one, so that adding a node does not renumber the parameters of
/// an endpoint a client already calls.
/// </remarks>
public sealed record WorkflowInput(
    WorkflowNode Node,
    WorkflowPort? Port,
    string Type,
    string Label);

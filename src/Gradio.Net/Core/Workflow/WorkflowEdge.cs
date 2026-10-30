namespace Gradio.Net;
/// <summary>
/// One connection in a workflow: an output port feeding an input port.
/// </summary>
/// <param name="Id">The edge's name in the graph.</param>
/// <param name="FromNodeId">The node the value comes from.</param>
/// <param name="FromPortId">The output port it leaves by.</param>
/// <param name="ToNodeId">The node the value goes to.</param>
/// <param name="ToPortId">The input port it arrives at.</param>
/// <param name="Type">What travels through it, which is the source port's type.</param>
/// <remarks>
/// An edge names both ends rather than pointing at ports by reference, because the graph is a
/// document a browser edits and writes back: an id survives the round trip, and a reference into a
/// list would not.
/// </remarks>
public sealed record WorkflowEdge(
    string Id,
    string FromNodeId,
    string FromPortId,
    string ToNodeId,
    string ToPortId,
    string Type);

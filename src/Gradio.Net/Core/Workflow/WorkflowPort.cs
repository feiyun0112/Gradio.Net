namespace Gradio.Net;
/// <summary>
/// One connection point of a workflow node: a value the node reads, or one it produces
/// </summary>
/// <param name="Id">The port's name inside its node, which is what an edge names.</param>
/// <param name="Label">What the canvas shows the port as, and what an edge may name instead of the id.</param>
/// <param name="Type">What travels through it — <c>text</c>, <c>image</c>, <c>number</c> and the rest.</param>
/// <param name="Required">Whether the node refuses to run without a value.</param>
/// <param name="OutputIndex">
/// Which element of a node's response this output port takes, or <see langword="null"/> to take the
/// port's own position.
/// </param>
/// <remarks>
/// The type is a string rather than an enum because it is the canvas's vocabulary, not the
/// framework's: a saved workflow may carry a type this build has never heard of, and refusing to
/// read it would refuse a graph the frontend can still draw. A port of an unknown type is treated
/// as text, which is what the Python version does with everything that is not one of the types it
/// maps.
/// </remarks>
public sealed record WorkflowPort(
    string Id,
    string Label,
    string Type,
    bool Required = false,
    int? OutputIndex = null);

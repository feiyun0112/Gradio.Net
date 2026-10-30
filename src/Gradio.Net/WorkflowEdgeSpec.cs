namespace Gradio.Net;
/// <summary>
/// One wire an application declares between two bound functions, used when the workflow has no
/// graph file yet.
/// </summary>
/// <param name="From">Where the wire starts: <c>function</c> for its first output, or <c>function.port</c>.</param>
/// <param name="To">Where it ends: <c>function</c> for its first input, or <c>function.port</c>.</param>
/// <remarks>
/// A port is named by its label or its id, whichever the application finds readable — a bound
/// function's ports are named after its parameters, so <c>clean.output</c> and <c>clean.out_0</c>
/// are the same wire.
/// </remarks>
public sealed record WorkflowEdgeSpec(string From, string To);

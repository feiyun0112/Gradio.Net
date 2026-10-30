using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// One step of a workflow: a value the caller supplies, a computation, or a value the application
/// shows.
/// </summary>
/// <param name="Id">The node's name in the graph, which is what an edge names.</param>
/// <param name="Role">Which of the three collections it came from: <c>reference</c>, <c>operator</c> or <c>subject</c>.</param>
/// <param name="Kind">
/// What runs an operator: <c>fn</c> for a bound function, and <c>space</c>, <c>model</c> or
/// <c>dataset</c> for the three kinds that call a service. <see langword="null"/> for a node that
/// runs nothing.
/// </param>
/// <param name="Fn">The bound function an operator names, or <see langword="null"/> for another kind.</param>
/// <param name="Label">What the canvas shows the node as.</param>
/// <param name="Inputs">The ports it reads.</param>
/// <param name="Outputs">The ports it produces.</param>
/// <param name="Data">The values its ports start with, keyed by port id.</param>
/// <param name="AssetType">What a reference or subject carries, when it declares it on the node rather than on a port.</param>
/// <param name="Raw">The node as the graph wrote it, for the fields this build does not model.</param>
/// <remarks>
/// <para>
/// The graph is the canvas's document, so this is a reading of it rather than a schema of the
/// framework's own: the fields the framework acts on are lifted out, and everything else — a
/// model's provider, a Space's id, the width the node is drawn at — stays in <paramref name="Raw"/>
/// where a caller that understands it can reach it.
/// </para>
/// <para>
/// <paramref name="Kind"/> is a string for the same reason a port's type is: a saved graph may name
/// a kind this build has no caller for, and that is a failure to report when the node runs rather
/// than a graph to refuse.
/// </para>
/// </remarks>
public sealed record WorkflowNode(
    string Id,
    string Role,
    string? Kind,
    string? Fn,
    string Label,
    IReadOnlyList<WorkflowPort> Inputs,
    IReadOnlyList<WorkflowPort> Outputs,
    JsonObject Data,
    string? AssetType,
    JsonObject Raw);

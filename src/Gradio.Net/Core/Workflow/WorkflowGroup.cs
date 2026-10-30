namespace Gradio.Net;
/// <summary>
/// One pipeline of a workflow: the outputs it produces, and the endpoint that runs it
/// </summary>
/// <param name="Subjects">The output nodes it produces, in the order the graph declares them.</param>
/// <param name="ApiName">The endpoint's name, made unique among the groups of the same graph.</param>
/// <remarks>
/// Outputs that share the Python version work are one group: running them apart would compute the
/// same nodes twice, and a caller that asked for both wanted one answer with two parts. The group's
/// name comes from its first output's label, because that is what a caller reads in the API
/// documentation.
/// </remarks>
public sealed record WorkflowGroup(
    IReadOnlyList<WorkflowNode> Subjects,
    string ApiName);

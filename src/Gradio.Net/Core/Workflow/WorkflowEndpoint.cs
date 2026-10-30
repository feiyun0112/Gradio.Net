namespace Gradio.Net;
/// <summary>
/// One endpoint a workflow exposes: a group of outputs that one pipeline feeds
/// </summary>
/// <param name="ApiName">The endpoint's name, without the leading slash the API info adds.</param>
/// <param name="Label">What the canvas's API panel calls it — the group's first output's label.</param>
/// <param name="Parameters">The values a caller supplies, in the order the endpoint takes them.</param>
/// <param name="Returns">The values it answers with, in the order it answers them.</param>
/// <remarks>
/// A workflow with several independent pipelines gets one endpoint per pipeline rather than one per
/// output: the outputs a single run produces belong together, and a caller that wanted them
/// separately would run the same Python work twice.
/// </remarks>
public sealed record WorkflowEndpoint(
    string ApiName,
    string Label,
    IReadOnlyList<WorkflowEndpointParameter> Parameters,
    IReadOnlyList<WorkflowEndpointReturn> Returns);

/// <summary>One value an endpoint takes.</summary> <param name="Label">What the workflow calls
/// it.</param> <param name="ParameterName">The name a client passes it under.</param>
/// <param name="Type">The port type it travels as.</param> <param name="PythonType">The Python type
/// a caller of the Python implementation would pass.</param>
/// <remarks>
/// The Python type is what the canvas's "View API" panel shows in the snippets it offers. It is a
/// description of the Python client's calling convention rather than of this framework, which is
/// why it is a string here and not a type.
/// </remarks>
public sealed record WorkflowEndpointParameter(
    string Label,
    string ParameterName,
    string Type,
    string PythonType);

/// <summary>One value an endpoint answers with.</summary> <param name="Label">What the workflow
/// calls it — the output node's label.</param> <param name="Type">The port type it travels
/// as.</param> <param name="PythonType">The Python type a caller of the Python implementation would
/// receive.</param>
public sealed record WorkflowEndpointReturn(
    string Label,
    string Type,
    string PythonType);

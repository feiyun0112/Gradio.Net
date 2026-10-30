namespace Gradio.Net;
/// <summary>
/// What runs a workflow node of one kind: the work a <c>fn</c>, <c>model</c>, <c>space</c> or
/// <c>dataset</c> operator stands for.
/// </summary>
/// <param name="node">The node being run, with everything the graph said about it.</param>
/// <param name="arguments">Its input values, in the order its ports are declared.</param>
/// <param name="cancellationToken">Cancels the work.</param>
/// <returns>Its output values, which the ports then pick from by index or by shape.</returns>
/// <remarks>
/// <para>
/// The Python version keeps these as the four server functions the canvas calls as well —
/// <c>call_fn</c>, <c>call_model</c>, <c>call_space</c>, <c>fetch_dataset</c> — because in Python
/// one callable can serve both a browser and a request handler. Here the two are different call
/// shapes, so a caller that runs a node is a type of its own and the canvas reaches the same work
/// through the endpoint it was given.
/// </para>
/// <para>
/// The type exists so that the kinds this build cannot run are a matter of registration rather than
/// of missing code: bound functions are always available, and a node that calls a model or a Space
/// needs a caller the application supplies — which is where an inference client of its choosing
/// goes.
/// </para>
/// </remarks>
public delegate ValueTask<IReadOnlyList<object?>> WorkflowOperator(
    WorkflowNode node,
    IReadOnlyList<object?> arguments,
    CancellationToken cancellationToken);

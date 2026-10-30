namespace Gradio.Net;
/// <summary>
/// One endpoint of an OpenAPI document, ready to be drawn as an <see cref="Interface"/>.
/// </summary>
/// <remarks>
/// <para>
/// This is what <see cref="External.LoadOpenApi"/> hands the caller for each path and method it
/// keeps: the handler that calls the service, the components its arguments are read from, and what
/// the API documents should say about it. The Python version forwards a dictionary of keyword
/// arguments to each generated <c>gr.Interface</c> instead; a delegate that receives this record is
/// the same thing with the parameters checked at compile time, and it can say anything an
/// <see cref="Interface"/> can be told.
/// </para>
/// <para>
/// The defaults are the Python version's: a page of endpoints that was given no options draws each
/// one as a plain interface, titled by its summary and described by the same text the API document
/// carries.
/// </para>
/// </remarks>
/// <param name="Path">The path the endpoint is served at, as the document spells it.</param>
/// <param name="Method">The method it answers to, lower case, as the document spells it.</param>
/// <param name="Fn">The handler: one argument per input, in the order the components are drawn.</param>
/// <param name="Inputs">The components the handler's arguments are read from.</param>
/// <param name="Outputs">The components the answer is written to — the JSON one the page draws.</param>
/// <param name="ApiName">
/// The name the endpoint is served under: the method and the path, as the Python version spells it
/// — a path of <c>/books/{id}</c> answered by GET is <c>get__books_id</c>.
/// </param>
/// <param name="ApiDescription">
/// The description the API document carries for the endpoint: its own description, or its summary,
/// or the empty string when it has neither.
/// </param>
/// <param name="ParameterNames">
/// The names the API document gives the arguments: the document's own names for the parameters,
/// then <c>request_body</c> when there is a body.
/// </param>
public sealed record OpenApiEndpoint(
    string Path,
    string Method,
    Delegate Fn,
    IReadOnlyList<Component> Inputs,
    IReadOnlyList<Component> Outputs,
    string ApiName,
    string ApiDescription,
    IReadOnlyList<string> ParameterNames);

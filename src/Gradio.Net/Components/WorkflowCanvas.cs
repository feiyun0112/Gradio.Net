using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A canvas that draws a workflow of connected steps, and the functions those steps call
/// </summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. The value is the workflow as the canvas serializes it —
/// a JSON string — and the frontend draws the nodes, their connections and their settings from it.
/// </para>
/// <para>
/// The functions an application hands the canvas are what a step runs when the workflow is
/// executed: the canvas calls them over <c>/gradio_api/component_server</c>, naming one by the key
/// it was registered under, which is the same path the HTML component's own functions take. The
/// names are written to the config's <c>server_fns</c>, so the frontend knows what it may call.
/// </para>
/// <para>
/// <see cref="AppView"/> is the other half of the view: the HTML id of a layout block holding the
/// same workflow as an ordinary application, which a visitor without write access is shown instead
/// of the canvas. The canvas draws nothing there; the frontend switches between the two.
/// </para>
/// <para>
/// The Python version's class is both a container and a component, and it is skipped by the API — a
/// canvas has no value an endpoint can be called with. The two are declared here as a component
/// that draws no children: the canvas holds the workflow rather than other blocks, so its layout
/// node carries the empty list a container's does without the container behaviour.
/// </para>
/// </remarks>
[GradioBlock("workflowcanvas")]
[HandlerValue("string?", "change")]
public sealed partial class WorkflowCanvas : Component
{
    /// <summary>Declares a canvas with the Python version's defaults.</summary>
    /// <remarks>
    /// A constructor rather than an initializer because one default is not the component base's:
    /// a canvas is drawn without a container around it, which is also why it shows no label unless
    /// the application asks for one. What a re-render keeps is the value, as for every component,
    /// so that default is the base's and is left alone.
    /// </remarks>
    internal WorkflowCanvas() => Container = false;

    /// <summary>The workflow the canvas draws.</summary>
    /// <value>The workflow as a JSON string, or <see langword="null"/> for an empty canvas.</value>
    /// <remarks>
    /// The Python version also accepts a function here, called on every browser session to compute
    /// the starting workflow. That is <see cref="Component.LoadFn"/>, which is how the canvas an
    /// application builds mounts its own initial load: the value is what this build and the config
    /// read, and the function is what each page opened later runs.
    /// </remarks>
    public string? Value { get; set; }

    /// <summary>The label drawn above the canvas.</summary>
    public override string? Label { get; set; }

    /// <summary>The functions the canvas may call on the server, keyed by name.</summary>
    /// <value>
    /// A dictionary of function names to the delegates that handle them, or <see langword="null"/>
    /// for none. The Python version takes a list of callables and derives the names from them; a
    /// .NET delegate carries no dependable name — a lambda's is generated — so the name is part of
    /// what the application registers.
    /// </value>
    /// <remarks>
    /// Nothing about the functions is written to the config's props — the Python version's config
    /// carries no prop key for them either. The names travel in <c>server_fns</c>, which the base
    /// class writes from <see cref="ServerFunctionNames"/>.
    /// </remarks>
    [OmitFromConfig]
    public IReadOnlyDictionary<string, Delegate>? ServerFunctions { get; set; }

    /// <summary>The HTML id of the layout block showing the same workflow as an ordinary app.</summary>
    /// <value><see langword="null"/> for a canvas with no read-only view.</value>
    /// <remarks>
    /// An id rather than the block itself, which is what the Python version carries: the frontend
    /// looks the block up in the page it has already been sent, so the canvas does not have to name
    /// a block that may be written after it.
    /// </remarks>
    public string? AppView { get; set; }

    /// <inheritdoc />
    /// <value>
    /// <see langword="true"/>: a canvas has no value an API endpoint can be called with, which is
    /// the Python version's rule for the block as well.
    /// </value>
    public override bool SkipApi => true;

    /// <inheritdoc />
    /// <value>A string, which is what the workflow serializes to.</value>
    /// <remarks>Declared because every component describes its type, even one the API skips.</remarks>
    public override JsonObject ApiInfo => new() { ["type"] = "string" };

    /// <summary>
    /// The blocks this component draws inside itself.
    /// </summary>
    /// <value>
    /// Empty: the Python version's canvas is a container, and its layout node carries the empty
    /// children list a container's does. The canvas holds the workflow rather than other blocks, so
    /// nothing is ever written into it here.
    /// </value>
    protected internal override IReadOnlyList<Block>? LayoutChildren => [];

    /// <summary>Whether the label is drawn when the application says nothing.</summary>
    /// <value><see langword="false"/>, which is the Python version's default for a canvas.</value>
    protected override bool ShowLabelWhenUnset => false;

    /// <inheritdoc />
    protected internal override IReadOnlyList<string> ServerFunctionNames =>
        ServerFunctionTable.NamesOf(ServerFunctions);

    /// <summary>Calls one of the functions the application registered on this canvas.</summary>
    /// <param name="request">The function's name, and what the browser sent with it.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>What the function produced, as the JSON the frontend reads.</returns>
    /// <exception cref="NotSupportedException">The canvas declares no such function.</exception>
    protected internal override async ValueTask<JsonNode?> InvokeServerFunctionAsync(
        ComponentServerCall request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (ServerFunctions is not null
            && ServerFunctions.TryGetValue(request.FunctionName, out var function))
        {
            return await ServerFunctionTable
                .CallAsync(function, request.Data, cancellationToken)
                .ConfigureAwait(false);
        }

        return await base.InvokeServerFunctionAsync(request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>The workflow a handler reads.</summary>
    /// <param name="value">What the client sent.</param>
    /// <returns>The workflow string, or <see langword="null"/> for an empty canvas.</returns>
    public override object? Preprocess(JsonNode? value) => JsonValues.Text(value);

    /// <summary>The workflow the client receives from what a handler returned.</summary>
    /// <param name="value">The workflow string, or none.</param>
    /// <returns>The workflow string, or <see langword="null"/> for none.</returns>
    public override JsonNode? Postprocess(object? value) => JsonValues.FromObject(value);
}

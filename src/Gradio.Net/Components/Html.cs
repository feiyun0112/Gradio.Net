using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A component drawn from a template of the application's own: HTML, CSS and a little JavaScript,
/// with the component's props filled in.
/// </summary>
/// <remarks>
/// <para>
/// This is the Python version's <c>gr.HTML</c>, and its parameters are not generated from the
/// manifest because the Python version's own signature ends in <c>**props</c>: a custom component
/// declares its own parameters as extra props, which the config carries in a dictionary of their
/// own rather than as keys beside the rest. The mapping is therefore written here, in the
/// signature's order, and the extra props go last — which is where a subclass puts them too.
/// </para>
/// <para>
/// The frontend renders whatever the template produces, so the value is arbitrary: a string of
/// HTML, a list the template iterates, or a value the component's own JavaScript reads. Nothing here
/// interprets it.
/// </para>
/// <para>
/// A subclass is a component of its own: the config's <c>type</c> stays <c>html</c> — that is what
/// the frontend knows how to draw — while <c>props.name</c> is the subclass's name, and
/// <c>component_class_name</c> tells the frontend which class to report an error against. That is the
/// split <see cref="BlockType"/> exists for.
/// </para>
/// </remarks>
[GradioBlock("html")]
[HandlerValue("object?", "change", "input", "submit", "focus", "blur", "stop", "clear")]
public partial class Html : Component
{
    /// <summary>The JavaScript the Python version runs for a component that says nothing about its
    /// own.</summary>
    private const string DefaultJsOnLoad =
        "element.addEventListener('click', function() { trigger('click') });";

    /// <summary>The blocks written inside this one, which its template may place.</summary>
    private readonly List<Block> _children = [];

    /// <summary>Declares a component with no content of its own.</summary>
    /// <remarks>
    /// A constructor rather than an initializer because two of the defaults are not the component
    /// base's: an HTML component is drawn without a container around it, which is also why it shows
    /// no label unless the application asks for one.
    /// </remarks>
    /// <summary>Builds an empty HTML block.</summary>
    /// <remarks>
    /// Protected rather than internal: this is the type a custom component derives from, and a
    /// derived class has to be able to name its base's constructor. A page is still written with
    /// <c>gr.Html(...)</c> — the factory is the only way to put one on a page.
    /// </remarks>
    protected internal Html() => Container = false;

    /// <summary>The value the template is filled with.</summary>
    public virtual object? Value { get; set; }

    /// <summary>The label the component is described by.</summary>
    public override string? Label { get; set; }

    /// <summary>The template the component is drawn from.</summary>
    /// <value><c>${value}</c> unless the application supplies one.</value>
    public string HtmlTemplate { get; set; } = "${value}";

    /// <summary>The CSS the component is drawn with.</summary>
    /// <value>Empty unless the application supplies some; it is scoped to the component.</value>
    public string CssTemplate { get; set; } = string.Empty;

    /// <summary>The JavaScript run when the component is loaded.</summary>
    /// <value>
    /// A click handler that raises the component's <c>click</c> event, which is the Python
    /// version's default — a component that says nothing about its behaviour is still clickable.
    /// </value>
    public string? JsOnLoad { get; set; } = DefaultJsOnLoad;

    /// <summary>Whether the application's own styles are added to the framework's.</summary>
    public bool ApplyDefaultCss { get; set; } = true;

    /// <summary>The relative width of the component next to its neighbours.</summary>
    public int? Scale { get; set; }

    /// <summary>The width below which the component wraps.</summary>
    public int? MinWidth { get; set; }

    /// <summary>The height the component shrinks to at least.</summary>
    public object? MinHeight { get; set; }

    /// <summary>The height the component grows to at most.</summary>
    public object? MaxHeight { get; set; }

    /// <summary>Whether the component is drawn with padding of its own.</summary>
    public bool Padding { get; set; }

    /// <summary>Whether the component scrolls to its newest content.</summary>
    public bool Autoscroll { get; set; }

    /// <summary>The buttons drawn beside the component.</summary>
    public IReadOnlyList<string> Buttons { get; set; } = [];

    /// <summary>HTML injected into the page's head before the component's JavaScript runs.</summary>
    public string? Head { get; set; }

    /// <summary>
    /// The functions the component's JavaScript may call on the server, keyed by name.
    /// </summary>
    /// <value>
    /// A dictionary of function names to the delegates that handle them, or <see langword="null"/>
    /// for none. The names are written to the config's <c>server_fns</c> so that the frontend knows
    /// what it may call, and the calls arrive at <c>/gradio_api/component_server</c> naming one of
    /// them. A delegate takes the call's one argument, converted to its parameter's type — a
    /// <c>JsonNode</c> parameter receives the argument as it arrived, and any other type receives it
    /// deserialized — and its result is written as JSON of whatever shape it returned.
    /// </value>
    /// <remarks>
    /// <para>
    /// Nothing about the functions is written to the config's <em>props</em> — which is also true
    /// the Python version, whose config carries no prop key for them. The function names travel in
    /// <c>server_fns</c> instead, which the base class writes from
    /// <see cref="ServerFunctionNames"/>, in the dictionary's enumeration order — the order a
    /// <see cref="Dictionary{TKey, TValue}"/> enumerates the entries that were added to it.
    /// </para>
    /// <para>
    /// A subclass that declares its own server functions overrides
    /// <see cref="ServerFunctionNames"/> and <see cref="InvokeServerFunctionAsync(ComponentServerCall, CancellationToken)"/>
    /// directly, which is the pattern <see cref="ImageEditor"/> and <see cref="FileExplorer"/>
    /// follow; the dictionary here is for a component whose functions the application supplies
    /// without subclassing.
    /// </para>
    /// <para>
    /// The files a multipart call carries are not among the delegate's arguments: a function
    /// registered this way is handed the call's one value. An application whose JavaScript uploads
    /// something uses the upload route, whose answer names the path to send back as that value —
    /// which is the flow the Python version's <c>upload</c> helper has. A component that needs the
    /// bytes themselves overrides this method, as the image editor does.
    /// </para>
    /// </remarks>
    public IReadOnlyDictionary<string, Delegate>? ServerFunctions { get; set; }

    /// <inheritdoc />
    /// <remarks>
    /// The names the application declared, when it declared any; empty otherwise, so a component
    /// with no functions carries no <c>server_fns</c> key rather than an empty one.
    /// </remarks>
    protected internal override IReadOnlyList<string> ServerFunctionNames =>
        ServerFunctionTable.NamesOf(ServerFunctions);

    /// <summary>Calls one of this component's server functions.</summary>
    /// <param name="request">The function's name, and what the browser sent with it.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>What the function produced, as the JSON the frontend reads.</returns>
    /// <exception cref="NotSupportedException">This component declares no such function.</exception>
    /// <remarks>
    /// Dispatches to the delegate the application registered under the function's name, and falls
    /// through to the base for a function this component does not have — which is how a subclass
    /// that overrides this reaches its own functions first and the dictionary second. The delegate
    /// is awaited when it returns a task, so that an application's <c>async</c> function works the
    /// way the Python version's does.
    /// </remarks>
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

    /// <summary>The parameters a subclass declares beyond the ones every HTML component has.</summary>
    /// <value>
    /// An empty dictionary unless the application put something in it: the Python version's
    /// constructor collects its <c>**props</c> into one, and a component given none carries an
    /// empty one rather than none at all.
    /// </value>
    public virtual JsonObject? ExtraProperties { get; set; } = new();

    /// <inheritdoc />
    /// <remarks>
    /// A custom HTML component is drawn by the <c>html</c> component of the frontend, whatever it is
    /// called; what differs is its name and the class the frontend reports.
    /// </remarks>
    public override string BlockType => "html";

    /// <summary>
    /// The blocks this component draws inside itself.
    /// </summary>
    /// <value>
    /// Whatever was written into it, which the template places where it says <c>@children</c>.
    /// </value>
    /// <remarks>
    /// The Python version's HTML is a <c>BlockContext</c> as well as a component, which is how
    /// <c>with gr.HTML(...):</c> can hold blocks. C# has one base class to give, so the same thing
    /// is said the other way round: a component that holds children answers this list and accepts
    /// them, and <see cref="Blocks.Enter"/> opens a scope over any block that does. What the
    /// children are for is the template's business — a component that says nothing about
    /// <c>@children</c> is drawn without them, exactly as the Python version's is.
    /// </remarks>
    protected internal override IReadOnlyList<Block>? LayoutChildren => _children;

    /// <summary>The blocks written inside this component, in the order they were written.</summary>
    public IReadOnlyList<Block> Children => _children;

    /// <inheritdoc />
    internal override void AppendChild(Block child)
    {
        ArgumentNullException.ThrowIfNull(child);

        _children.Add(child);
    }

    /// <inheritdoc />
    internal override bool HoldsChildren => true;

    /// <summary>Whether the components written straight into this one are grouped as their kind asks.</summary>
    /// <value>
    /// <see langword="true"/>, which is what the Python version's HTML does: it is a
    /// <c>BlockContext</c> like any other, so a textbox written inside it is put into the form its
    /// kind expects before the template draws it.
    /// </value>
    protected internal override bool AllowsExpectedParents => true;

    /// <inheritdoc />
    /// <remarks>
    /// The same rule a layout runs, over the children this component holds: a run of form components
    /// becomes a form, and a button between two of them ends the run.
    /// </remarks>
    internal override void FillExpectedParents(Func<string, BlockContext> createContainer) =>
        ExpectedParents.Fill(_children, BlockClass, this, createContainer);

    /// <summary>Whether the label is drawn when the application says nothing.</summary>
    /// <value><see langword="false"/>: an HTML component is its own content, and a label above it would be a second one.</value>
    protected override bool ShowLabelWhenUnset => false;

    /// <inheritdoc />
    public override JsonObject ApiInfo => new() { ["type"] = "string" };

    /// <inheritdoc />
    public override JsonNode? ExampleInputs => JsonValue.Create("<p>Hello</p>");

    /// <summary>The props this component writes, in the signature's order.</summary>
    /// <param name="props">The props being built.</param>
    /// <remarks>
    /// Written by hand because the parameter list is open: the Python version's constructor takes
    /// <c>**props</c>, so there is no manifest entry to generate a mapping from and no fixed list a
    /// missing member could be checked against.
    /// </remarks>
    protected internal override void WriteProps(PropWriter props)
    {
        ArgumentNullException.ThrowIfNull(props);

        props.Add("value", Value);
        props.Add("label", Label);
        props.Add("html_template", HtmlTemplate);
        props.Add("css_template", CssTemplate);
        props.Add("js_on_load", JsOnLoad);
        props.Add("apply_default_css", ApplyDefaultCss);
        props.Add("every", Every);
        props.Add("inputs", Inputs);
        props.Add("show_label", ShowLabel);
        props.Add("scale", Scale);
        props.Add("min_width", MinWidth);
        props.Add("visible", Visible);
        props.Add("elem_id", ElemId);
        props.Add("elem_classes", ElemClasses);
        props.Add("key", Key);
        props.Add("preserved_by_key", PreservedByKey);
        props.Add("min_height", MinHeight);
        props.Add("max_height", MaxHeight);
        props.Add("container", Container);
        props.Add("padding", Padding);
        props.Add("autoscroll", Autoscroll);
        props.Add("buttons", Buttons);
        props.Add("head", Head);
        props.Add("props", ExtraProperties);

        props.Add("component_class_name", ComponentClassName);
    }

    /// <summary>The class name the frontend reports a failure against.</summary>
    /// <value>
    /// The component's own class name, except for this one: the Python version spells its base
    /// class <c>HTML</c>, and the name is the string the frontend shows rather than the type's name
    /// here.
    /// </value>
    protected virtual string ComponentClassName => GetType() == typeof(Html) ? "HTML" : GetType().Name;

    /// <inheritdoc />
    /// <remarks>
    /// An HTML component declares every event the Python version has, so the four whose binding
    /// says something about the component are written whether or not anything listens. All four are
    /// constants in the Python version — an HTML component is never likeable, undoable or
    /// retryable, and what it streams is its own business rather than the config's — so they are
    /// written as constants here too, and only <c>_selectable</c> follows a binding.
    /// </remarks>
    protected internal override void WriteEventConfig(PropWriter props)
    {
        ArgumentNullException.ThrowIfNull(props);

        props.Add("_undoable", false);
        props.Add("_retryable", false);
        props.Add("likeable", false);
        props.Add("streamable", false);
    }

    /// <summary>The value a handler reads.</summary>
    /// <param name="value">The value the client sent.</param>
    /// <returns>The value as the JSON it is, since an HTML component makes no claim about its shape.</returns>
    public override object? Preprocess(JsonNode? value) => JsonValues.ToObject(value);
}

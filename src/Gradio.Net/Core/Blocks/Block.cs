using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// The smallest renderable UI unit: it receives a stable id when it is added to a render root and
/// remembers which block it was added to.
/// </summary>
/// <remarks>
/// <para>
/// A block describes itself to the vendored frontend as a <em>config entry</em>: an id, a type, a
/// bag of props, and — for the blocks that carry a value — the typing information the API needs.
/// The shape is the Python version's (<c>Blocks.config_for_block</c>), because the frontend
/// consumes it unchanged.
/// </para>
/// <para>
/// The props a block contributes are named after the Python version's constructor parameters, in
/// the Python version's order, and a parameter left at <see langword="null"/> is left out — the
/// Python version deletes every <c>None</c> from the props before it serves them, and "the
/// parameter the Python version declares but this build does not send" is exactly the drift the
/// config comparison exists to catch.
/// </para>
/// </remarks>
public abstract class Block
{
    /// <summary>The value of <see cref="Id"/> while the block has not been added to a render root.</summary>
    public const int UnrenderedId = -1;

    /// <summary>
    /// The id of this block within its render root, unique there and stable for the lifetime of
    /// the block.
    /// </summary>
    /// <value><see cref="UnrenderedId"/> until the block is added to a render root.</value>
    public int Id { get; private set; } = UnrenderedId;

    /// <summary>The block this one was added to, or <see langword="null"/> for a render root.</summary>
    public Block? Parent { get; private set; }

    /// <summary>
    /// The render root this block belongs to, or <see langword="null"/> when it was never added to
    /// one.
    /// </summary>
    /// <remarks>
    /// Walking up rather than holding a second reference: the parent chain already says which
    /// application a block was rendered into, and a second field would be a second thing to keep in
    /// step. Event binding is what needs this — a component has to name its application without
    /// being handed one.
    /// </remarks>
    internal Blocks? Root
    {
        get
        {
            var block = this;
            while (block.Parent is not null)
            {
                block = block.Parent;
            }

            return block as Blocks;
        }
    }

    /// <summary>
    /// The path of the page this block belongs to; the empty string is the application's home page.
    /// </summary>
    /// <remarks>
    /// The path rather than the name the navbar shows: it is what the config's <c>page</c> map is
    /// keyed by, and what the page is served at.
    /// </remarks>
    public string PagePath { get; internal set; } = string.Empty;

    /// <summary>
    /// The blocks this one draws inside itself, or <see langword="null"/> when it draws none.
    /// </summary>
    /// <value>
    /// <see langword="null"/> for a block that holds nothing, which is every block but a container.
    /// </value>
    /// <remarks>
    /// A container is not the only block that draws children: the Python version's custom-HTML
    /// component is a container too, so its layout node carries an empty list even though nothing
    /// was written into it. That is why this is a property of the block rather than a test for the
    /// container type — the layout tree has to say the same thing the Python version says, and a
    /// component that is a container but holds nothing still answers a list.
    /// </remarks>
    protected internal virtual IReadOnlyList<Block>? LayoutChildren => null;

    /// <summary>Puts a block among this one's children.</summary>
    /// <param name="child">The block to add.</param>
    /// <exception cref="InvalidOperationException">This block does not hold children.</exception>
    /// <remarks>
    /// A layout answers this by appending to its children; a component that is also a context — the
    /// HTML component, whose template decides where its children are drawn — answers it the same
    /// way. Everything else refuses, which is what makes <see cref="Blocks.Enter"/> a scope only for
    /// the blocks that have somewhere to put what is written inside them.
    /// </remarks>
    internal virtual void AppendChild(Block child) =>
        throw new InvalidOperationException($"'{GetType().Name}' does not hold other blocks.");

    /// <summary>Whether blocks can be written inside this one.</summary>
    /// <value>
    /// <see langword="false"/> for a component, and <see langword="true"/> for a layout or for a
    /// component whose template has a place for children.
    /// </value>
    /// <remarks>
    /// This is the question <see cref="Blocks.Enter"/> asks, and it is asked separately from
    /// <see cref="LayoutChildren"/> on purpose: a block may say what the layout tree should hold for
    /// it — an empty list, which is a node with no children — without being able to take one.
    /// </remarks>
    internal virtual bool HoldsChildren => false;

    /// <summary>Whether the components written straight into this one are grouped as their kind asks.</summary>
    /// <value><see langword="false"/> for a block that holds nothing.</value>
    protected internal virtual bool AllowsExpectedParents => false;

    /// <summary>Groups this block's children, the way its kind asks, when its scope closes.</summary>
    /// <param name="createContainer">Makes a container of the expected kind and gives it its id.</param>
    /// <remarks>
    /// Nothing, for a block that holds nothing. A layout groups what was written into it, and so
    /// does a component that holds children: the Python version's HTML is a <c>BlockContext</c> as
    /// well as a component, and its children are grouped exactly as a layout's are.
    /// </remarks>
    internal virtual void FillExpectedParents(Func<string, BlockContext> createContainer)
    {
        // A block that holds nothing has nothing to regroup.
    }

    /// <summary>
    /// Whether this block is kept out of the API description.
    /// </summary>
    /// <value>
    /// <see langword="true"/> for containers, which carry no value and so cannot be an input or an
    /// output; components override it to <see langword="false"/>.
    /// </value>
    public virtual bool SkipApi => true;

    /// <summary>The block's type as the config spells it: the class name, lower case.</summary>
    /// <remarks>
    /// A template block — the Python version's <c>is_template</c> — reports the name of the class
    /// it was derived from instead, because the frontend draws it with that class's component. The
    /// four buttons that are a plain button with a behaviour attached are templates for exactly
    /// that reason: a clear button is drawn by the button component, and the config says so.
    /// </remarks>
    public virtual string BlockClass => (IsTemplate ? GetType().BaseType! : GetType()).Name.ToLowerInvariant();

    /// <summary>Whether the block is drawn by its parent's component rather than by one of its own.</summary>
    /// <value><see langword="false"/> for a block whose own class is the component.</value>
    protected virtual bool IsTemplate => false;

    /// <summary>
    /// The block's type as the config's <c>type</c> spells it, when that is not the class name.
    /// </summary>
    /// <value><see cref="BlockClass"/> for every block whose type is its class name.</value>
    /// <remarks>
    /// The Python version separates the two: <c>props.name</c> is the class name — the name a
    /// <c>gr.load</c> rebuilds the block from — while <c>type</c> is what the frontend dispatches
    /// on, and two blocks spell that one differently from their class (a <c>Tab</c> is a
    /// <c>tabitem</c>, a <c>Step</c> a <c>walkthroughstep</c>). A second field rather than an
    /// override of <see cref="BlockClass"/> because both are written, and they disagree.
    /// </remarks>
    public virtual string BlockType => BlockClass;

    /// <summary>
    /// Whether the application may hold only one of these blocks on a page.
    /// </summary>
    /// <value><see langword="false"/> for every block but the one that configures its page.</value>
    /// <remarks>
    /// The render refuses a second one rather than serving a page whose navbar is described twice,
    /// which is the Python version's <c>validate_navbar_settings</c> said as a rule about blocks
    /// rather than about one component.
    /// </remarks>
    protected internal virtual bool SinglePerPage => false;

    /// <summary>Whether the block is drawn; a hidden block is still in the config.</summary>
    /// <remarks>
    /// The Python version's <c>visible</c>, which is a boolean or the word <c>hidden</c> — see
    /// <see cref="Visibility"/> for why the third value is a value of its own.
    /// </remarks>
    public Visibility Visible { get; set; } = Visibility.Shown;

    /// <summary>The HTML id of the block, when the application gives it one.</summary>
    public string? ElemId { get; set; }

    /// <summary>The HTML classes of the block.</summary>
    [FactoryArgument(FactoryArgumentKind.Names)]
    public IReadOnlyList<string> ElemClasses { get; set; } = [];

    /// <summary>
    /// Whether the block is part of the rendered tree.
    /// </summary>
    /// <remarks>
    /// Declared because the Python version declares it, and because the parameter mapping is
    /// checked against the Python version's parameter list in full. The Python version removes it
    /// from the props before serving them — it decides whether a block is in the tree at all rather
    /// than being a prop of one — so it is never written to the config.
    /// </remarks>
    public bool Render { get; set; } = true;

    /// <summary>
    /// The key a block is identified by across re-renders.
    /// </summary>
    /// <remarks>
    /// Declared for the same reason as <see cref="Render"/>. Nothing sets it yet: dynamic
    /// re-rendering is its own batch, and until then the config reports a null key, which is what
    /// the Python version reports for a block that was never keyed.
    /// </remarks>
    public object? Key { get; set; }

    /// <summary>
    /// The props this block contributes, in the Python version's constructor-parameter order.
    /// </summary>
    /// <remarks>
    /// Built by generated code for components and layouts alike, so that a parameter the Python
    /// version declares and this build does not map is a compile error rather than a silently
    /// missing key .
    /// </remarks>
    protected internal virtual void WriteProps(PropWriter props)
    {
        ArgumentNullException.ThrowIfNull(props);
    }

    /// <summary>
    /// The props an event of this block contributes, written before its own.
    /// </summary>
    /// <param name="props">The props being built.</param>
    /// <remarks>
    /// <para>
    /// The Python version's <c>config_data</c>: an event may declare a key about the block rather
    /// than about a constructor parameter, and the key is written first. The only one this build
    /// has is <c>streamable</c>, which the <c>stream</c> event contributes to every component that
    /// declares it — an image, an audio player, a video player — and which the frontend reads to
    /// decide whether it may open a stream at all.
    /// </para>
    /// <para>
    /// A separate method rather than a first line of <see cref="WriteProps"/> because
    /// <see cref="WriteProps"/> is generated from the parameter manifest, and a hand-written line in
    /// a generated body is a line nobody can keep.
    /// </para>
    /// </remarks>
    protected internal virtual void WriteEventConfig(PropWriter props)
    {
        ArgumentNullException.ThrowIfNull(props);
    }

    /// <summary>
    /// The props this block appends after its parameter mapping, as the Python version's
    /// <c>get_config</c> override does.
    /// </summary>
    /// <param name="props">The props being built, already carrying the parameters, the name and the
    /// flags the event bindings set.</param>
    /// <remarks>
    /// <para>
    /// A component whose config says something the constructor parameters do not — a dataset naming
    /// the components it draws and the ids it draws them from — writes it here rather than in
    /// <see cref="WriteProps"/>, because the generated mapping owns that method and the Python
    /// version's key order puts these keys last.
    /// </para>
    /// <para>
    /// A block that appends nothing ignores this, which is why it is not abstract.
    /// </para>
    /// </remarks>
    protected internal virtual void WriteExtraProps(PropWriter props)
    {
        ArgumentNullException.ThrowIfNull(props);
    }

    /// <summary>
    /// Every prop this block contributes, with the nulls in place.
    /// </summary>
    /// <returns>The props, in the Python version's parameter order.</returns>
    /// <remarks>
    /// This is the in-memory shape rather than the served one: a dataset copies a component's props
    /// to describe how it draws a sample, and the Python version copies them before the nulls are
    /// removed. The name and the flags an event binding sets are not constructor parameters and are
    /// not here — the Python version's copy keeps only the parameters the component's signature
    /// declares.
    /// </remarks>
    internal JsonObject AllProps()
    {
        var props = new PropWriter(keepNulls: true);
        WriteProps(props);

        return props.Build();
    }

    /// <summary>
    /// Called once, when this block has been added to an application and has its id.
    /// </summary>
    /// <param name="root">The application the block was added to.</param>
    /// <remarks>
    /// <para>
    /// The Python version runs this in the constructor: a block registers itself with the current
    /// application as it is written, so a button that binds its own events can do it there. Here
    /// the id — and therefore the identity a binding names — is given by <see cref="Blocks.Add"/>,
    /// so the same moment is the end of that call instead. The few blocks that care are the ones
    /// whose whole behaviour is a binding they set up themselves: a button that clears other
    /// components, one that copies a link, one that signs a user in.
    /// </para>
    /// <para>
    /// A block that binds nothing ignores this, which is why it is not abstract.
    /// </para>
    /// </remarks>
    protected internal virtual void OnAddedTo(Blocks root)
    {
        ArgumentNullException.ThrowIfNull(root);
    }

    /// <summary>
    /// Records that one of this block's signals is bound to a handler.
    /// </summary>
    /// <param name="eventName">The signal's name, as the Python version spells it.</param>
    /// <remarks>
    /// <para>
    /// The Python version's event listeners call back into the block when they are bound, which is
    /// how a component learns that something is listening for it. The few that care are the ones
    /// whose config says what the frontend may offer — a chat that can be liked, a block whose
    /// items can be selected — and they report it there rather than at the point the handler runs.
    /// </para>
    /// <para>
    /// A block that does not care ignores this, which is why it is not abstract: most signals change
    /// nothing about the block that raises them.
    /// </para>
    /// </remarks>
    protected internal virtual void OnEventBound(string eventName)
    {
    }

    /// <summary>
    /// The files this block wants kept out of reclamation, because it is part of what the block is.
    /// </summary>
    /// <value>
    /// Absolute paths; empty for a block that holds no file of its own.
    /// </value>
    /// <remarks>
    /// A component's default value is copied into the cache like any other file, but deleting it
    /// would break the component rather than reclaim anything, which is the Python version's
    /// <c>keep_in_cache</c>. The render root records these when the block is added, because that is
    /// the moment the block and the application meet.
    /// </remarks>
    protected internal virtual IEnumerable<string> KeptFiles => [];

    /// <summary>The directory this block caches files in.</summary>
    /// <value>
    /// The application's upload folder, or the default one for a block that was never rendered.
    /// </value>
    /// <remarks>
    /// A block that belongs to an application uses that application's folder, so that a file it
    /// writes is a file the application's file endpoint will serve. A block that belongs to none
    /// has no application to ask, and falls back to the process default — which is what the Python
    /// version's module-level cache folder is.
    /// </remarks>
    protected internal string MediaFolder => Root?.UploadFolder ?? MediaCache.DefaultFolder;

    /// <summary>
    /// The server functions this block declares, by name.
    /// </summary>
    /// <value>
    /// Empty for every block but the few whose behaviour is a function the frontend calls: a file
    /// explorer's listing, an image editor's blob upload.
    /// </value>
    /// <remarks>
    /// The Python version's <c>@server</c> methods, which it collects by reflection over the class.
    /// The names are written to the config so that the frontend knows what it may call, and the
    /// calls arrive at <c>/gradio_api/component_server</c> naming one of them.
    /// </remarks>
    protected internal virtual IReadOnlyList<string> ServerFunctionNames => [];

    /// <summary>
    /// Calls one of this block's server functions.
    /// </summary>
    /// <param name="request">The function's name, and what the frontend sent with it.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>What the function produced, as the JSON the frontend reads.</returns>
    /// <exception cref="NotSupportedException">This block declares no such function.</exception>
    /// <remarks>
    /// Asynchronous because a server function may be: the Python version declares its own with
    /// <c>async def</c>, and an application's functions are written the same way. A function that
    /// answers synchronously returns a completed task.
    /// </remarks>
    protected internal virtual ValueTask<JsonNode?> InvokeServerFunctionAsync(
        ComponentServerCall request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        throw new NotSupportedException(
            $"'{request.FunctionName}' is not a server function of a {GetType().Name}.");
    }

    /// <summary>
    /// The kind of container this block has to live inside, or <see langword="null"/> when it may
    /// sit wherever it was written.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A component that expects a container is grouped into one at the end of the render, which is
    /// how a textbox written straight into a <c>Blocks</c> ends up in a synthesised form.
    /// </para>
    /// <para>
    /// The kind is a name rather than a type because a component does not reference a layout: a
    /// component says "a form", and the factory that owns forms turns that name into one.
    /// </para>
    /// </remarks>
    protected internal virtual string? GetExpectedParentKind() => null;

    /// <summary>
    /// Whether this block ends a run of siblings that would otherwise be grouped into one container.
    /// </summary>
    /// <remarks>
    /// A block that is grouped into a container is never asked: the run it belongs to is the run
    /// that expects the container. This decides what happens to a block that expects none — a
    /// button between two textboxes ends the form rather than being pulled into it.
    /// </remarks>
    protected internal virtual bool BreaksGrouping => true;

    /// <summary>Records the identity this block received from a render root.</summary>
    internal void MarkRendered(int id, Block? parent)
    {
        Id = id;
        Parent = parent;
    }

    /// <summary>Re-parents this block without giving it a new id.</summary>
    /// <remarks>
    /// Grouping moves a block into the container that was created for it; the id it was given when
    /// it was rendered is its identity and does not change.
    /// </remarks>
    internal void MarkParent(Block? parent) => Parent = parent;

    /// <summary>
    /// The block's entry in the served config.
    /// </summary>
    /// <remarks>
    /// Every registered block gets one, including a container the render synthesised: the Python
    /// version only filters by "was this rendered" when a dynamic render is in progress, which is
    /// its own batch.
    /// </remarks>
    internal JsonObject ToConfigEntry()
    {
        var props = new PropWriter();
        WriteEventConfig(props);
        WriteProps(props);
        props.Add("name", BlockClass);

        if (this is Component selectable)
        {
            props.Add("_selectable", selectable.Selectable);
        }

        // The functions the frontend may call on this block. The Python version writes them last, after the
        // name and the flags its event bindings set, and only when there are any: a block with none
        // carries no key rather than an empty list.
        if (ServerFunctionNames.Count > 0)
        {
            props.Add("server_fns", ServerFunctionNames);
        }

        WriteExtraProps(props);

        var entry = new JsonObject
        {
            ["id"] = Id,
            ["type"] = BlockType,
            ["props"] = props.Build(),
            ["skip_api"] = SkipApi,

            // The Python version derives this from the path the module its class is installed at, so the
            // value identifies an installation rather than a component and a second
            // implementation cannot reproduce it. A built-in component therefore serves null,
            // which the frontend reads as "draw this from the registry you already have" - the
            // same thing it does with an id it does not know. A component that brings its own
            // frontend is the one case where the identity matters, because it is the path segment
            // its bundle is served under, so it declares one (ICustomComponentBundle).
            ["component_class_id"] = (this as ICustomComponentBundle)?.ComponentClassId,

            // Two different things carry the name "key", and both are null for a block that was
            // never keyed. The props carry the key the application gave, and are written by the
            // generated mapping (so an unset key is simply absent there). This entry carries the
            // key the frontend identifies the block by across re-renders, which the Python version derives
            // from the key and the renderable it is in. Keys are not wired up yet; the entry says
            // null, which is what the Python version says for a block that was never keyed.
            ["key"] = null,
        };

        if (SkipApi || this is not Component component)
        {
            return entry;
        }

        entry["api_info"] = component.ApiInfo;
        entry["api_info_as_input"] = component.ApiInfoAsInput;
        entry["api_info_as_output"] = component.ApiInfoAsOutput;
        entry["example_inputs"] = component.ExampleInputs?.DeepClone();

        return entry;
    }
}

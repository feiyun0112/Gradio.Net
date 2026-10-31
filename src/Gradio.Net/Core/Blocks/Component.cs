using System.Security.Cryptography;
using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// A block that carries a value, and can therefore be an event input or output
/// </summary>
/// <remarks>
/// <para>
/// What separates a component from a layout is that it has a value and a type: the config entry of
/// a component additionally carries the typing information the API description and the OpenAPI
/// document are built from. A layout has neither, and is skipped by the API entirely.
/// </para>
/// <para>
/// The typing information is a property of the component class rather than of a value, so it is
/// declared per component — a textbox is always a string, whatever its current text.
/// </para>
/// </remarks>
public abstract class Component : Block
{
    /// <summary>The signals of this component that a handler is bound to.</summary>
    private readonly HashSet<string> _boundEvents = new(StringComparer.Ordinal);

    /// <summary>The name a trigger set declares a signal under, which is the event's name capitalized.</summary>
    private const string ChangeTrigger = "Change";

    private bool? _showLabel;

    private bool? _interactive;

    private bool _selectable;

    /// <inheritdoc />
    public override bool SkipApi => false;

    /// <summary>Whether the component is drawn inside a container.</summary>
    public bool Container { get; set; } = true;

    /// <summary>
    /// The label drawn above the component, or <see langword="null"/> when the application named
    /// none.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Python version declares <c>label</c> on each component that has one, and reads it back
    /// through the instance — <c>component.label</c> — wherever something has to speak about "the
    /// label" without knowing which component it is: an interface naming its inputs after the
    /// handler's parameters, a flagging log naming its columns, a dataset heading its table.
    /// Declared once here for the same reason, so that those three do not each need a way of
    /// reaching into a component they cannot name.
    /// </para>
    /// <para>
    /// The parameter mapping is still per component and still generated from the Python version's
    /// list: a component declares the property as an override so that its own parameter is the one
    /// written to the config, and a component whose parameters in the Python version include no
    /// <c>label</c> never has this key written at all.
    /// </para>
    /// </remarks>
    public virtual string? Label { get; set; }

    /// <summary>
    /// Whether this component sends its value to the server a chunk at a time.
    /// </summary>
    /// <value>
    /// <see langword="false"/> for every component but the three that declare the parameter, which
    /// override it.
    /// </value>
    /// <remarks>
    /// The Python version reads <c>getattr(component, "streaming", None)</c>, which is why a
    /// component that declares no such parameter answers falsy rather than raising. Declared here
    /// for the same reason as <see cref="Label"/>: a caller that has to decide whether a live
    /// interface binds <c>stream</c> or <c>change</c> asks the component rather than knowing which
    /// components those are.
    /// </remarks>
    public virtual bool Streaming { get; set; }

    /// <summary>
    /// The function that computes this component's value when a page opens, or
    /// <see langword="null"/> for a value the application wrote.
    /// </summary>
    /// <value>
    /// <see langword="null"/> unless the application asked for one; setting it binds the load event
    /// <see cref="Blocks"/> attaches once the application has been written.
    /// </value>
    /// <remarks>
    /// <para>
    /// The Python version's callable <c>value</c>, which every component accepts: a component whose
    /// value is a function has it called on each browser session and starts with what it answered.
    /// It is a member of its own here because <c>Value</c> is typed per component and a delegate is
    /// not a <see cref="string"/> — the same reason a canvas takes its server functions as a
    /// dictionary rather than deriving their names. <see cref="LoadFunction"/> carries the
    /// endpoint's name beside the function, because a delegate has no dependable name to derive one
    /// from.
    /// </para>
    /// <para>
    /// The binding runs on the page's own <c>load</c> signal and on the <c>change</c> of every
    /// component in <see cref="Inputs"/>, which is the Python version's rule. It shows no progress
    /// animation when there is such a trigger and the full one when the page's opening is all there
    /// is, also the Python version's rule: a value recomputed on every keystroke of a field would
    /// otherwise flash a spinner on every keystroke.
    /// </para>
    /// <para>
    /// What the Python version also does with a callable value is compute it once at build time, so
    /// that the config carries the first value as well. That is not done here: <c>Value</c> is the
    /// application's and is what the config and the server-side build read, and the load event is
    /// what a page opened later runs. An application that wants both writes both — a canvas that
    /// draws its graph at build time does exactly that.
    /// </para>
    /// <para>
    /// One quirk of the Python version is worth naming here, because the member's name comes from
    /// it: the <c>load_fn</c> parameter its base constructor takes is overwritten before it is
    /// used, and what actually drives the load event is the callable <c>value</c>. So
    /// <c>gr.Fallback(load_fn=f)</c> does nothing the Python version. Here the member is the live
    /// one, which is the same thing said the other way round: a .NET component has one member for
    /// the function, and it works.
    /// </para>
    /// </remarks>
    [OmitFromConfig]
    public LoadFunction? LoadFn { get; set; }

    /// <summary>The components <see cref="LoadFn"/> reads, or <see langword="null"/> for none.</summary>
    /// <remarks>
    /// <para>
    /// Each one is a trigger: the value is recomputed when any of them changes, as well as when the
    /// page opens. They are also the binding's inputs, so the function's parameters are filled from
    /// them in the order they are written.
    /// </para>
    /// <para>
    /// The Python version takes one component as readily as a list. A list is what this takes, for
    /// the reason the raw bindings take one: a parameter that is sometimes a component and
    /// sometimes a sequence is a shape C# would have to be told twice, and a one-element array is a
    /// list.
    /// </para>
    /// <para>
    /// An input that raises no <c>change</c> is read but does not re-run the function, which is the
    /// Python version's rule: a value that names a component the user cannot edit would otherwise
    /// claim to be recomputed on its every change.
    /// </para>
    /// </remarks>
    [OmitFromConfig]
    [FactoryArgument(FactoryArgumentKind.OneOrMany)]
    public object[]? Inputs { get; set; }

    /// <summary>
    /// How often the component's value is recalculated, in seconds.
    /// </summary>
    /// <value>
    /// <see langword="null"/> unless the application asks for a refresh; the Python version
    /// recomputes the value on that interval when it was given a function to compute it, and does
    /// nothing otherwise.
    /// </value>
    /// <remarks>
    /// <para>
    /// The Python version never serves this: its constructor takes the parameter and does not store
    /// it, so no config carries the key. <c>OmitFromConfig</c> says the same here — the member
    /// exists so that the parameter is still checked against the Python version, and it is not
    /// written.
    /// </para>
    /// <para>
    /// It is also the one part of the load event that is not wired, and the reason is the base
    /// class rather than a missing batch: the Python version answers an interval by creating a
    /// timer component when the component is constructed, and a component does not name another
    /// component type here. What an application writes instead is that timer itself — a <c>Timer</c>
    /// bound to the same handler, whose <c>tick</c> is the interval. Every other part of the
    /// parameter — that the value is recomputed when its inputs change, and when the page opens —
    /// is what <see cref="LoadFn"/> does.
    /// </para>
    /// </remarks>
    [OmitFromConfig]
    public double? Every { get; set; }

    /// <summary>
    /// Whether the user can change the component's value.
    /// </summary>
    /// <value>
    /// <see langword="null"/> unless the application says, which is what the Python version's form
    /// components default to: it infers it from whether anything reads the component, so a
    /// component nobody listens to is drawn disabled. A component whose own default is a value
    /// overrides <see cref="InteractiveWhenUnset"/> instead.
    /// </value>
    public bool? Interactive
    {
        get => _interactive ?? InteractiveWhenUnset;
        set => _interactive = value;
    }

    /// <summary>Whether the component is interactive when the application says nothing about it.</summary>
    /// <value>
    /// <see langword="null"/> for every component but the buttons, which the Python version
    /// declares as interactive and writes into their props.
    /// </value>
    protected virtual bool? InteractiveWhenUnset => null;

    /// <summary>
    /// Whether the component's label is drawn.
    /// </summary>
    /// <value>
    /// The Python version's effective value: an unset label is shown, and a component that is not
    /// drawn in a container has no label to show. The config carries this rather than the raw
    /// argument, because it is what the Python version carries.
    /// </value>
    /// <remarks>
    /// A component with a rule of its own overrides this and calls the base — the checkbox turns the
    /// label on whenever one was given, and still has no label to show outside a container. A
    /// component whose only difference is the default overrides <see cref="ShowLabelWhenUnset"/>
    /// instead, which leaves the container rule in one place.
    /// </remarks>
    public virtual bool ShowLabel
    {
        get => Container && (_showLabel ?? ShowLabelWhenUnset);
        set => _showLabel = value;
    }

    /// <summary>Whether a label is drawn when the application says nothing about it.</summary>
    /// <value><see langword="true"/> for every component but the one whose own default is not to.</value>
    protected virtual bool ShowLabelWhenUnset => true;

    /// <summary>
    /// The parameters a re-render preserves.
    /// </summary>
    /// <value>
    /// The value, as the Python version defaults for a component: re-rendering a component with the
    /// same key keeps what the user typed rather than resetting it to the constructor's value.
    /// </value>
    [FactoryArgument(FactoryArgumentKind.Names)]
    public IReadOnlyList<string> PreservedByKey { get; set; } = ["value"];

    /// <summary>
    /// Whether the frontend may let the user select this component's content.
    /// </summary>
    /// <value>
    /// <see langword="false"/> until something listens for the component's <c>select</c> event, or
    /// until the application says otherwise, which is the Python version's rule: the affordance is
    /// offered because a handler asked for it.
    /// </value>
    /// <remarks>
    /// Settable because four of the Python version's components — the image editor and the three
    /// templates built from it — take the flag as a constructor parameter, and a <c>gr</c> factory
    /// writes every parameter it is given. Setting it to <see langword="false"/> is the same as
    /// saying nothing: a bound <c>select</c> still turns the flag on, which is what the Python
    /// version does when its listener is attached.
    /// </remarks>
    public virtual bool Selectable
    {
        get => _selectable || IsBound(EventNames.Select);
        set => _selectable = value;
    }

    /// <summary>Whether a handler is bound to one of this component's signals.</summary>
    /// <param name="eventName">The signal's name, as the Python version spells it.</param>
    /// <returns>Whether something listens for it.</returns>
    /// <remarks>
    /// A component's config sometimes says what the frontend may offer — a chat that can be liked,
    /// a block whose items can be selected — and that is a fact about the bindings rather than
    /// about the constructor arguments. The Python version records it the same way, when the
    /// listener is attached.
    /// </remarks>
    protected bool IsBound(string eventName) => _boundEvents.Contains(eventName);

    /// <inheritdoc />
    protected internal override void OnEventBound(string eventName) => _boundEvents.Add(eventName);

    /// <summary>The component's typing information, as the config's <c>api_info</c> spells it.</summary>
    /// <remarks>
    /// Abstract rather than defaulted to a throw: a component that cannot describe its type cannot
    /// be an event input or output, and the compiler saying so is better than a component that
    /// fails the first time a page is served.
    /// </remarks>
    public abstract JsonObject ApiInfo { get; }

    /// <summary>The typing information of this component used as an input.</summary>
    public virtual JsonObject ApiInfoAsInput => ApiInfo;

    /// <summary>The typing information of this component used as an output.</summary>
    public virtual JsonObject ApiInfoAsOutput => ApiInfo;

    /// <summary>
    /// A representative value of this component, used to render the API documentation's examples.
    /// </summary>
    /// <value><see langword="null"/> for a component that offers no example.</value>
    public virtual JsonNode? ExampleInputs => null;

    /// <summary>
    /// The value a handler reads from this component.
    /// </summary>
    /// <param name="value">The value the client sent, as JSON.</param>
    /// <returns>The value to hand the handler.</returns>
    /// <remarks>
    /// <para>
    /// The default turns the JSON into the CLR value it carries, which is right for every component
    /// whose value is what it looks like on the wire. A component that means more by it overrides
    /// this — an image component turns a path into a file, say.
    /// </para>
    /// <para>
    /// A <see langword="null"/> stays <see langword="null"/> rather than becoming an empty value:
    /// the Python version's components pass an absent value through as absent, and a handler that
    /// can tell "nothing was sent" from "an empty string was sent" depends on it.
    /// </para>
    /// </remarks>
    public virtual object? Preprocess(JsonNode? value) => JsonValues.ToObject(value);

    /// <summary>
    /// The value this component receives from what a handler returned.
    /// </summary>
    /// <param name="value">One of the values the handler's result carried.</param>
    /// <returns>The value to send the client.</returns>
    /// <remarks>
    /// The inverse of <see cref="Preprocess"/>, and overridden for the same reason: a component
    /// whose wire form is not the value itself has to say what it sends.
    /// </remarks>
    public virtual JsonNode? Postprocess(object? value) => JsonValues.FromObject(value);

    /// <summary>
    /// Writes this component's value to a form a flagging log can hold.
    /// </summary>
    /// <param name="value">The value the client sent, as the protocol carried it.</param>
    /// <param name="directory">The directory the flagged sample is being written into.</param>
    /// <returns>The cell to write into the log's row for this component.</returns>
    /// <remarks>
    /// <para>
    /// The Python version's <c>Component.flag</c>. The value is the one the client sent rather than
    /// a preprocessed one, because the flagging binding runs with preprocessing off: what is being
    /// recorded is what the user submitted, not what the handler would have been handed.
    /// </para>
    /// <para>
    /// A file is copied into the log's own directory rather than recorded where it happens to sit.
    /// The cache it was uploaded into is reclaimed, so a path into it would be a path that stops
    /// working; a flagged sample is meant to outlive that.
    /// </para>
    /// <para>
    /// A component whose flagging form is not its wire form overrides this — the Python version
    /// overrides it on the components that carry files, and those overrides are ticket 26's.
    /// </para>
    /// </remarks>
    public virtual string Flag(JsonNode? value, string directory)
    {
        ArgumentException.ThrowIfNullOrEmpty(directory);

        // A missing value is written the way JSON writes one, which is what the Python version's base
        // implementation produces for a component whose value is JSON: it hands everything that is
        // not text to the JSON writer. A component whose value is a data model is the case the Python version
        // does not answer — it fails on the missing value — and writing "null" is this build's
        // answer rather than its absence. The loggers that want an empty cell for it — the two that
        // write a header and guard the sample themselves — say so before they get here.
        if (value is null)
        {
            return "null";
        }

        if (value is JsonValue scalar && scalar.TryGetValue<string>(out var text))
        {
            return text;
        }

        return value is JsonObject file && IsFileObject(file)
            ? FlagFile(file, directory)
            : JsonText.Serialize(value);
    }

    /// <summary>
    /// Turns a cell of a flagging log back into this component's value.
    /// </summary>
    /// <param name="cell">The cell, as the log holds it.</param>
    /// <returns>The value the component would carry.</returns>
    /// <remarks>
    /// <para>
    /// The Python version's <c>Component.read_from_flag</c>, which is the other half of
    /// <see cref="Flag"/>: an answer that was worked out once and written down is read back into
    /// the components that show it, and a cell is text while a value need not be.
    /// </para>
    /// <para>
    /// Text is what most components answer, because text is what a log holds. The exception is a
    /// file: <see cref="Flag"/> records one as the JSON that describes it, so a cell that is a file
    /// object is read back as the object rather than as text — which is what makes a cached answer
    /// that named a file still name one. A component whose value is JSON in its own right overrides
    /// this.
    /// </para>
    /// </remarks>
    public virtual JsonNode? ReadFromFlag(string cell)
    {
        ArgumentNullException.ThrowIfNull(cell);

        return ParseFileObject(cell) ?? JsonValue.Create(cell);
    }

    /// <summary>A cell read back as the file object it describes, or <see langword="null"/>.</summary>
    /// <param name="cell">The cell.</param>
    /// <returns>The object, when the cell is one.</returns>
    /// <remarks>
    /// Asked of the content rather than of the component, because that is how <see cref="Flag"/>
    /// decided to write it: only a value that is a file object is written as one.
    /// </remarks>
    internal static JsonNode? ParseFileObject(string cell)
    {
        if (!cell.StartsWith('{'))
        {
            return null;
        }

        try
        {
            return JsonNode.Parse(cell) is JsonObject file && IsFileObject(file) ? file : null;
        }
        catch (System.Text.Json.JsonException)
        {
            return null;
        }
    }

    /// <summary>A cell read back as the JSON value it holds.</summary>
    /// <param name="cell">The cell.</param>
    /// <returns>The value.</returns>
    /// <exception cref="System.Text.Json.JsonException">The cell is not JSON.</exception>
    /// <remarks>
    /// The Python version's <c>parse_escaped_json</c>. A cell can carry one extra apostrophe in
    /// front, because writing it put one there: a value that starts with <c>-</c> is prefixed so
    /// that a spreadsheet reads it as text rather than as a formula. Reading it back has to undo
    /// that, or a negative number comes back as a broken cell.
    /// </remarks>
    protected static JsonNode? ParseEscapedJson(string cell)
    {
        ArgumentNullException.ThrowIfNull(cell);

        try
        {
            return JsonNode.Parse(cell);
        }
        catch (System.Text.Json.JsonException) when (cell.StartsWith('\''))
        {
            return JsonNode.Parse(cell[1..]);
        }
    }

    /// <summary>Whether a value is a file the protocol carries rather than an ordinary object.</summary>
    /// <param name="value">The value.</param>
    /// <returns><see langword="true"/> when it is marked as one.</returns>
    /// <remarks>
    /// The marker is <c>meta._type</c>, which every file object carries and nothing else does — the
    /// Python version's <c>is_file_obj_with_meta</c>. A value that is a file without the marker is
    /// an object, which is what it will be recorded as.
    /// </remarks>
    internal static bool IsFileObject(JsonObject value) =>
        value["meta"] is JsonObject meta
        && JsonValues.Text(meta["_type"]) == FileData.FileMetaType;

    /// <summary>Copies a file into the log's directory and records where it now is.</summary>
    /// <param name="file">The file object the client sent.</param>
    /// <param name="directory">The directory the sample is being written into.</param>
    /// <returns>The file object as JSON, naming the copy.</returns>
    /// <remarks>
    /// <para>
    /// A file that is not on this machine — a value that is a URL — is recorded as it stands: there
    /// is nothing to copy, and the Python version leaves it alone for the same reason.
    /// </para>
    /// <para>
    /// The copy keeps the name it had, which is what the Python version's <c>shutil.copy</c> does,
    /// and the rest of the object is left as the client sent it — including <c>url</c>, which still
    /// names the cached original. That is the Python version's answer too; the cell is a record of
    /// what was submitted, and the path is the part of it that outlives the cache.
    /// </para>
    /// </remarks>
    private static string FlagFile(JsonObject file, string directory)
    {
        var path = JsonValues.Text(file["path"]);
        if (path is null || !System.IO.File.Exists(path))
        {
            return JsonText.Serialize(file);
        }

        // The copy gets a directory of its own, which is the Python version's rule: two samples whose files
        // are both called "photo.png" are then two files, where one directory would have the second
        // overwrite the first and leave the earlier row pointing at the later sample's bytes.
        var destination = Path.Combine(directory, NewCopyDirectoryName(), Path.GetFileName(path));
        Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
        System.IO.File.Copy(path, destination);
        file["path"] = destination;

        return JsonText.Serialize(file);
    }

    /// <summary>A name for the directory one copied file goes into.</summary>
    /// <returns>Twenty hexadecimal characters, which is what the Python version draws.</returns>
    /// <remarks>
    /// Random rather than derived from the file, because two copies of the same file are two
    /// samples: a name that was worked out from the path would give the second one the first one's
    /// directory. The Python version's <c>secrets.token_hex(10)</c>.
    /// </remarks>
    private static string NewCopyDirectoryName() =>
        Convert.ToHexString(RandomNumberGenerator.GetBytes(10)).ToLowerInvariant();
    /// <summary>Binds the load event this component asked for.</summary>
    /// <param name="page">The signal the application raises once a page has mounted.</param>
    /// <returns>
    /// Whether the event was bound: <see langword="false"/> when one of the components it reads has
    /// not been rendered yet, which is what a render that ends before the rest of the application is
    /// written looks like.
    /// </returns>
    /// <remarks>
    /// <para>
    /// The Python version's <c>attach_load_event</c> and the binding its root makes from it. The
    /// triggers are the <c>change</c> of each component the function reads that raises one, then
    /// the page's own signal — the order the Python version builds them in. The progress animation
    /// is hidden when there is such a trigger and full when the page's opening is all there is,
    /// which is the Python version's rule and the reason it exists: a value recomputed on every
    /// keystroke would otherwise repaint a spinner on every keystroke.
    /// </para>
    /// <para>
    /// It answers whether it bound rather than binding unconditionally, because a component written
    /// before the components it reads is a shape an application can have: the render that ends in
    /// between has nothing to bind yet, and the one that writes the rest does.
    /// </para>
    /// </remarks>
    internal bool AttachLoadEvent(EventTrigger page)
    {
        ArgumentNullException.ThrowIfNull(page);

        if (LoadFn is not { } load)
        {
            return false;
        }

        var inputs = Inputs is { Length: > 0 } declared
            ? EventBinding.BlocksOf(declared, nameof(Inputs))
            : [];

        if (inputs.Any(input => input.Id == UnrenderedId))
        {
            return false;
        }

        // The Python version keeps `[(i, "change") for i in inputs if hasattr(i, "change")]`: a component
        // that raises no change is read but never re-runs the function. It matters for the progress
        // animation as well as for the triggers — one input that raises nothing leaves the page's
        // opening as the only trigger, which is the full animation rather than none.
        var changing = inputs.Where(input => input is Component component && component.OffersChange).ToArray();

        var triggers = new List<EventTrigger>(changing.Length + 1);

        foreach (var input in changing)
        {
            triggers.Add(new EventTrigger(input, EventNames.Change));
        }

        triggers.Add(page);

        EventBinding.Bind(
            triggers,
            load.Fn,
            inputs,
            [this],
            apiName: load.ApiName, showProgress: changing.Length > 0 ? ProgressDisplay.Hidden : ProgressDisplay.Full);

        return true;
    }

    /// <summary>Whether this component offers a <c>change</c> signal.</summary>
    /// <value>
    /// Whether the trigger set this component answers with declares one, which is the same question
    /// as the Python version's <c>hasattr(component, "change")</c> asked of this framework's shape:
    /// a trigger is a property of the set rather than an attribute of the component.
    /// </value>
    /// <remarks>
    /// Read from the type rather than declared, because the alternative is sixty overrides that all
    /// say the same thing. A component that raises no change declares no such property, and one that
    /// does is the component that has to be asked.
    /// </remarks>
    private bool OffersChange => Events.GetType().GetProperty(ChangeTrigger) is not null;
}

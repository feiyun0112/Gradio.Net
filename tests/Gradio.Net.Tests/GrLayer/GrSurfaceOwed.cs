namespace Gradio.Net.Tests.GrLayer;

/// <summary>
/// The names the Python version exports and this layer does not have yet, one row per name, grouped
/// by the ticket that closes the gap.
/// </summary>
/// <remarks>
/// <para>
/// This is the readable half of the surface comparison: the comparison says how many names are
/// missing, and this says which ones and who owes them. It is a checked-in list rather than a count
/// because a count cannot be reviewed — and because the way the list changes is the point: adding a
/// factory takes a name out of it, and a name the Python version stops exporting has to come out
/// too, or <see cref="GrSurfaceTests"/> fails and says which row is stale.
/// </para>
/// <para>
/// Grouped by ticket rather than by kind, so that a reader can see what is left of the layer's work
/// rather than only what is missing from it. <see cref="Never"/> is the other half: the names this
/// layer will not mirror, each with the reason it will not.
/// </para>
/// </remarks>
internal static class GrSurfaceOwed
{
    /// <summary>The components the Python version exports and this layer does not build — nothing,
    /// now.</summary>
    /// <remarks>
    /// <para>
    /// Ticket 41 closed this list, and it is kept as an empty table rather than deleted because the
    /// way it changes is the point: the comparison in <see cref="GrSurfaceTests"/> fails when the
    /// Python version exports a component no factory builds and no row owes, so an empty list here
    /// means "the two sets are the same" rather than "nobody looked".
    /// </para>
    /// <para>
    /// What filled it was a generator rather than sixty hand-written factories: each
    /// component's parameters, their order and their defaults are read from the recorded Python
    /// signature, so this list cannot quietly refill with names somebody forgot to port.
    /// </para>
    /// </remarks>
    internal static IReadOnlyList<string> Components { get; } = [];

    /// <summary>The layouts the Python version exports and this layer does not build — nothing,
    /// now.</summary>
    /// <remarks>
    /// <para>
    /// Ticket 42 closed this list with the ten names that stood for nine layouts: <c>TabItem</c> is
    /// <c>Tab</c> spelled twice, and <c>Step</c> is the step of a <c>Walkthrough</c>. Kept as an
    /// empty table for the reason <see cref="Components"/> gives: the comparison fails when the
    /// Python version exports a layout no factory builds and no row owes, so an empty list means
    /// "the two sets are the same" rather than "nobody looked".
    /// </para>
    /// <para>
    /// The layouts' factories are written by hand rather than generated, because a
    /// layout's factory and the scope that enters it are one feature. What keeps them honest is the
    /// same comparison the generated ones face: their parameter names, their order and their
    /// defaults are held against the recorded Python signature, and
    /// <c>GrDefaultsTests</c> holds <c>gr.Row()</c> to <c>new Row()</c>.
    /// </para>
    /// </remarks>
    internal static IReadOnlyList<string> Layouts { get; } = [];

    /// <summary>
    /// Everything else the Python version exports at its top level — the page builders, the
    /// functions, the media helpers — which is ticket 44's.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Ticket 43 took <c>On</c>, <c>Skip</c> and <c>Update</c> out of this table: merging triggers
    /// and answering a handler with an update are the two halves of binding, and the layer has them.
    /// </para>
    /// <para>
    /// Ticket 44 has taken out the functions and the theme entry — <c>Examples</c>, <c>Progress</c>,
    /// <c>I18n</c>, <c>Api</c>, <c>GetPackageVersion</c>, the five media helpers, <c>NoReload</c> and
    /// <c>Theme</c>. The two loaders (<c>LoadChat</c>, <c>LoadOpenapi</c>) are the wrappers around
    /// <see cref="External"/>, and <c>Load</c> has moved to <see cref="Never"/>. What is left needs
    /// something the kernel does not have: the callouts (<c>Info</c>, <c>Warning</c>, <c>Error</c>,
    /// <c>Success</c>) need a channel from a running handler to the client, <c>Render</c> needs
    /// dynamic rendering, <c>Mcp</c> needs a way to register a tool without an attribute,
    /// <c>SetStaticPaths</c> needs the server to serve directories an application names, and
    /// <c>Workflow</c> needs the graph-file loader's caller-file resolution.
    /// </para>
    /// </remarks>
    internal static IReadOnlyList<string> TopLevel { get; } =
    [
        "Error", "Info", "Mcp", "Render",
        "SetStaticPaths", "Success", "Warning", "Workflow",
    ];

    /// <summary>
    /// The names this layer will never mirror, and why.
    /// </summary>
    /// <remarks>
    /// Each of these is a decision rather than a gap, and the reason is the whole of it: a name here
    /// that a later ticket does want is a name that ticket moves out of this table, which is a
    /// visible edit in a review. Leaving them in the gap list instead would make the layer look
    /// permanently unfinished for reasons no amount of work would change.
    /// </remarks>
    internal static IReadOnlyDictionary<string, string> Never { get; } = new Dictionary<string, string>(StringComparer.Ordinal)
    {
        ["Brush"] = "a value type the core owns",
        ["CSVLogger"] = "an extension point the core already defines as a type of its own",
        ["Cache"] = "a decorator, and caching is configured on the binding instead (ticket 26)",
        ["ChatMessage"] = "a value type the core owns",
        ["CloseAll"] = "a process-wide registry of launched applications; here a server is disposed by the frame that started it",
        ["Component"] = "the abstract base of every block, and the Python version's `component` factory: here a custom component is a class deriving from it rather than one a call builds (ticket 35)",
        ["CopyData"] = "an event payload the core owns",
        ["DeletedFileData"] = "a value type the core owns",
        ["Deploy"] = "deploys to Hugging Face Spaces, which the exclusion criteria rule out",
        ["DownloadData"] = "an event payload the core owns",
        ["EditData"] = "an event payload the core owns",
        ["Eraser"] = "a value type the core owns",
        ["EventData"] = "an event payload the core owns",
        ["FileData"] = "a value type the core owns",
        ["FileSize"] = "a value type the core owns",
        ["FlaggingCallback"] = "an extension point the core already defines as a type of its own",
        ["Header"] = "a value type the core owns",
        ["InputHTMLAttributes"] = "a TypedDict, carried as an object by the one parameter that takes it",
        ["KeyUpData"] = "an event payload the core owns",
        ["LayerOptions"] = "a TypedDict: a shape for typing, not a value",
        ["LikeData"] = "an event payload the core owns",
        ["Load"] = "fetches a Space or a model from the Hugging Face hub, which the exclusion criteria rule out; load_openapi covers a service that publishes a document",
        ["LoadIpythonExtension"] = "an IPython hook, and there is no IPython here",
        ["MessageDict"] = "a TypedDict: a shape for typing, not a value",
        ["MountGradioApp"] = "mounting into a host is UseGradio(), a host extension rather than a top-level function",
        ["OAuthProfile"] = "a value type the core owns",
        ["OAuthToken"] = "a value type the core owns",
        ["Request"] = "the core's own type; a port names it with a using",
        ["RetryData"] = "an event payload the core owns",
        ["SelectData"] = "an event payload the core owns",
        ["Server"] = "the host is GradioServer, which belongs to the Server layer",
        ["SimpleCSVLogger"] = "the same extension point, spelled simply",
        ["UndoData"] = "an event payload the core owns",
        ["Validate"] = "the Python version's CLI validator, and this framework ships no CLI",
        ["Validators"] = "the CLI validators' module, for the same reason",
        ["Version"] = "the module's own dunder; an assembly carries its version instead",
        ["WatermarkOptions"] = "a TypedDict: a shape for typing, not a value",
        ["WaveformOptions"] = "a TypedDict: a shape for typing, not a value",
        ["WebcamOptions"] = "a TypedDict: a shape for typing, not a value",
    };
}

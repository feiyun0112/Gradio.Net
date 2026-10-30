using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A workflow application: a graph the canvas draws, the functions its steps may call, and the app
/// view a visitor without the canvas sees instead.
/// </summary>
/// <remarks>
/// <para>
/// This is the Python version's <c>gr.Workflow</c>. It is an application rather than a component —
/// the canvas and the app view are what it draws — and it is handed the application to render into
/// for the reason <see cref="Interface"/> is: the render root is resolved from the container, one
/// container hosting one application.
/// </para>
/// <para>
/// Two things it draws share one graph. The canvas is where the workflow is arranged and run by its
/// author; the app view is the same workflow as an ordinary form — an input per value the graph
/// cannot compute, a button, and an output per result — which is what a visitor sees, and what the
/// endpoints are registered from. <c>app_view</c> is the id that tells the frontend which of the
/// two to show.
/// </para>
/// <para>
/// The graph comes from a file or from the functions the application binds, never both: a file is
/// what the canvas saved, and it wins when it is there, because it is the author's arrangement
/// rather than a draft of one. An application that binds functions and saves nothing gets a canvas
/// with a draft in it and no app view — a draft has no saved graph to derive outputs from.
/// </para>
/// </remarks>
public sealed partial class Workflow
{
    /// <summary>The file a workflow reads when the application names none.</summary>
    public const string DefaultGraphFile = "workflow.json";

    /// <summary>The prefix a bound function's endpoint is named with.</summary>
    /// <remarks>
    /// The Python version's name for the same endpoint. It is what the canvas calls when a step
    /// bound to a function runs, so it is part of the protocol between the two rather than a naming
    /// choice.
    /// </remarks>
    private const string BoundEndpointPrefix = "predict_fn_";

    /// <summary>The name the canvas's initial value is loaded under.</summary>
    private const string LoadEventName = "_load_initial";

    /// <summary>The id the app view's column carries, which the canvas is pointed at.</summary>
    private const string AppViewId = "workflow-app";

    /// <summary>The class the app view's body carries.</summary>
    private const string AppBodyClass = "workflow-app-body";

    /// <summary>What a workflow without a graph says instead of drawing an app view.</summary>
    private const string NoOutputsNote =
        "This workflow doesn't have any outputs yet. Open the workflow and add an output node to "
        + "turn it into an app.";

    /// <summary>The stylesheet the app view's header needs.</summary>
    /// <remarks>
    /// The header is the row that holds the sign-in button on a Space. It is laid out to the right,
    /// and the button keeps its own width instead of being stretched by the row — a row gives each
    /// child a zero flex basis, which overrides the width a button with <c>scale=0</c> asks for.
    /// The Python version prepends this at launch, which is where it applies its own styles; there
    /// is no launch here to hang it on, so it is prepended when the workflow is built.
    /// </remarks>
    private const string AppCss =
        "\n#workflow-app-header { justify-content: flex-end; }\n"
        + "#workflow-app-header > button { flex: none; }\n";

    private readonly Blocks _blocks;

    /// <summary>The graph file, absolute: it is also what the canvas keys a visitor's arrangement by.</summary>
    /// <remarks>
    /// Two applications served from the same origin in turn must not share a key, and a relative
    /// path would depend on where the process was started.
    /// </remarks>
    private readonly string _graphFile;
    private readonly string _name;
    private readonly IReadOnlyDictionary<string, Delegate> _bound;
    private readonly IReadOnlyList<WorkflowEdgeSpec> _edges;
    private readonly IReadOnlyDictionary<string, WorkflowOperator> _operators;

    private WorkflowCanvas? _canvas;

    /// <summary>Builds a workflow and renders it into <paramref name="blocks"/>.</summary>
    /// <param name="blocks">The application to render into.</param>
    /// <param name="graph">
    /// The path to the graph file, relative to the directory of the source file that calls this
    /// constructor. <see langword="null"/> for <see cref="DefaultGraphFile"/> in that directory.
    /// </param>
    /// <param name="bind">
    /// The functions a step may call, keyed by the name the graph names them by, or
    /// <see langword="null"/> for none. A .NET delegate carries no dependable name — a lambda's is
    /// generated — so the name is part of what the application registers.
    /// </param>
    /// <param name="edges">
    /// The wires between bound functions, or <see langword="null"/> for none. Ignored when the
    /// graph file is there: a saved graph is the author's arrangement, and wiring it from the code
    /// would overwrite what the canvas saved.
    /// </param>
    /// <param name="operators">
    /// What runs a node of a kind this build does not run itself, keyed by kind. Bound functions
    /// are always available; a node that calls a model or a Space needs a caller here, which is
    /// where an inference client of the application's choosing goes.
    /// </param>
    /// <param name="callerFilePath">
    /// The source file that called this constructor, filled in by the compiler. It is how a
    /// relative <paramref name="graph"/> finds the directory the application's own code is in,
    /// which is where the Python version looks too — a workflow's file sits beside the code that
    /// declares it.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="blocks"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// Two bound names become the same endpoint name, or an edge names a function or a port that is
    /// not bound.
    /// </exception>
    /// <remarks>
    /// The directory is the calling file's, falling back to the current directory when the compiled
    /// assembly no longer has the source tree beside it — a published build has no source file, and
    /// a workflow there is read from where the process was started.
    /// </remarks>
    public Workflow(
        Blocks blocks,
        string? graph = null,
        IReadOnlyDictionary<string, Delegate>? bind = null,
        IReadOnlyList<WorkflowEdgeSpec>? edges = null,
        IReadOnlyDictionary<string, WorkflowOperator>? operators = null,
        [CallerFilePath] string? callerFilePath = null)
    {
        ArgumentNullException.ThrowIfNull(blocks);

        _blocks = blocks;
        _bound = bind ?? new Dictionary<string, Delegate>(StringComparer.Ordinal);
        _edges = edges ?? [];
        _operators = operators ?? new Dictionary<string, WorkflowOperator>(StringComparer.Ordinal);
        _graphFile = ResolveGraphFile(graph, callerFilePath);
        _name = TitleOf(_graphFile);

        CheckBoundNames(nameof(bind));

        blocks.Mode = "workflow";

        // Ahead of whatever the application styles itself with, which is the Python version's order too: the
        // workflow's own rules are the ones it cannot work without, so they go first.
        blocks.Css = AppCss + (blocks.Css ?? string.Empty);

        Build();
    }

    /// <summary>Whether a visitor may change the workflow.</summary>
    /// <value>
    /// <see langword="false"/>: this build has no write path, so every visitor is a read-only
    /// visitor — which is the Python version's answer for a visitor who does not hold the
    /// launch-time write token.
    /// </value>
    /// <remarks>
    /// The canvas asks before it offers to edit anything, so the answer is served rather than
    /// assumed: a frontend that is not told otherwise assumes it may write, and would then let an
    /// author drag a graph the server cannot save.
    /// </remarks>
    private const string WriteAccess = "false";

    /// <summary>Draws the workflow: the canvas, the app view, and the endpoints.</summary>
    /// <remarks>
    /// The order is the config's: the canvas, the column the app view goes in, the endpoints bound
    /// functions are reached by, and only then the blocks the app view is built from. Every block
    /// is numbered as it is created, so building them in another order would describe a different
    /// application — one whose app view is numbered before the endpoints that fill it. The load
    /// event is not in that list because it is not written here: the canvas carries it, and the
    /// application attaches it once the render has closed, which is where the Python version puts
    /// it too.
    /// </remarks>
    private void Build()
    {
        Column appRoot;

        // The canvas and the column its app view goes in are written first, and the load event is
        // attached right after them — which is why it is the first dependency of the application.
        // The Python version does the same when its own `with` block closes, before the endpoints are
        // registered, and everything written after that is numbered after them. The attach is asked
        // for here rather than left to the render closing, because a caller can hand this workflow a
        // root whose render it opened itself, and then no render closes at this point.
        using (var first = _blocks.IsRendering ? null : _blocks.BeginRender())
        {
            _canvas = _blocks.Add(new WorkflowCanvas
            {
                // Both halves of what the Python version passes as one callable: the value is what this build
                // and the config read, and the function is the load event a page opened later runs
                // — so an edit to the file is picked up without restarting the application. The
                // canvas mounts that event itself, which is the Python version's own mechanism; binding it by
                // hand here would be the same dependency written in a place the canvas does not
                // know about, and in the wrong position.
                Value = LoadInitial(),
                LoadFn = new LoadFunction(LoadInitial, LoadEventName),
                AppView = AppViewId,
                ServerFunctions = ServerFunctions(),
            });

            appRoot = _blocks.Add(new Column { ElemId = AppViewId });
        }

        // Already bound if the render above closed; this is what binds it when one was open already.
        _blocks.AttachLoadEvents();

        using var rest = _blocks.IsRendering ? null : _blocks.BeginRender();

        RegisterBoundFunctions();

        var graph = WorkflowGraph.FromJson(_canvas.Value);
        if (graph is not null)
        {
            _blocks.Title = graph.Name;
        }

        BuildAppView(graph, appRoot);
    }

    /// <summary>The graph as it stands, read from the file each time.</summary>
    /// <returns>The file's text, the draft the bound functions describe, or none.</returns>
    /// <remarks>
    /// A missing file is not a failure: an application that binds functions is meant to start with
    /// no file at all, and one whose file has not been saved yet has a draft to draw instead.
    /// </remarks>
    private string? LoadInitial()
    {
        try
        {
            return System.IO.File.ReadAllText(_graphFile);
        }
        catch (FileNotFoundException)
        {
            return Draft();
        }
        catch (DirectoryNotFoundException)
        {
            return Draft();
        }
    }

    /// <summary>The graph the bound functions and wires describe, or none when nothing is bound.</summary>
    private string? Draft() => _bound.Count > 0 ? GraphFromBind(_bound, _edges, _name) : null;

    /// <summary>Registers an endpoint per bound function.</summary>
    /// <exception cref="ArgumentException">Two names become the same endpoint name.</exception>
    /// <remarks>
    /// The canvas runs a step bound to a function by calling this endpoint rather than by calling a
    /// server function: a step is a run of the application, with the queue and the API around it,
    /// and the endpoint is what gives it both. The endpoint is undocumented because it is a step of
    /// a larger pipeline rather than something a client calls on its own.
    /// </remarks>
    private void RegisterBoundFunctions()
    {
        foreach (var (name, function) in _bound)
        {
            _blocks.Api(
                BoundEndpoint(function),
                apiName: BoundEndpointPrefix + Sanitized(name), apiVisibility: ApiVisibility.Undocumented, concurrencyLimit: ConcurrencyLimit.ApplicationDefault);
        }
    }

    /// <summary>Refuses two bound names that would become one endpoint name.</summary>
    /// <exception cref="ArgumentException">Two names differ only in the characters an endpoint cannot carry.</exception>
    /// <remarks>
    /// The check is before anything is drawn, because the alternative is a second endpoint quietly
    /// replacing the first: both would be registered, and only one of them would be reachable.
    /// </remarks>
    private void CheckBoundNames(string parameterName)
    {
        var taken = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var name in _bound.Keys)
        {
            var sanitized = Sanitized(name);

            if (taken.TryGetValue(sanitized, out var existing))
            {
                throw new ArgumentException(
                    $"The bound function names '{existing}' and '{name}' both become the endpoint "
                    + $"name '{sanitized}'. Rename one of them.",
                    parameterName);
            }

            taken[sanitized] = name;
        }
    }

    /// <summary>What runs a node, by kind: bound functions, and whatever the application added.</summary>
    private Dictionary<string, WorkflowOperator> Operators()
    {
        var operators = new Dictionary<string, WorkflowOperator>(StringComparer.Ordinal)
        {
            ["fn"] = RunBoundAsync,
        };

        foreach (var (kind, run) in _operators)
        {
            operators[kind] = run;
        }

        return operators;
    }

    /// <summary>Runs a node bound to a function.</summary>
    /// <exception cref="WorkflowExecutionException">Nothing is bound under the node's name.</exception>
    private async ValueTask<IReadOnlyList<object?>> RunBoundAsync(
        WorkflowNode node,
        IReadOnlyList<object?> arguments,
        CancellationToken cancellationToken)
    {
        if (node.Fn is null || !_bound.TryGetValue(node.Fn, out var function))
        {
            throw new WorkflowExecutionException(
                $"No function '{node.Fn}' bound to this workflow");
        }

        var result = await DelegateInvocation
            .InvokeAsync(function, ArgumentsFor(function, arguments), cancellationToken)
            .ConfigureAwait(false);

        return OutputsOf(result);
    }

    /// <summary>The endpoint a bound function is reached by.</summary>
    /// <param name="function">The function.</param>
    /// <returns>A handler that reads the arguments as JSON and answers with the result as JSON.</returns>
    /// <remarks>
    /// Both ends are JSON text rather than a typed signature because the endpoint's shape is fixed:
    /// a workflow step's arguments are the values arriving on its ports, and how many there are is a
    /// property of the graph rather than of the function. The answer is a list for the same reason —
    /// the canvas reads it as the node's outputs — and a single value is a list of one.
    /// </remarks>
    private static Func<string, Task<object?>> BoundEndpoint(Delegate function) =>
        async argsJson =>
        {
            var arguments = ArgumentsOf(argsJson);
            var result = await DelegateInvocation
                .InvokeAsync(function, ArgumentsFor(function, arguments), CancellationToken.None)
                .ConfigureAwait(false);

            return PythonJson.Dump(new JsonArray([.. OutputsOf(result).Select(JsonValues.FromAnyObject)]));
        };

    /// <summary>The arguments an endpoint call carries.</summary>
    /// <param name="argsJson">The JSON text, or none.</param>
    /// <returns>The values, in the order the caller sent them.</returns>
    private static object?[] ArgumentsOf(string? argsJson)
    {
        if (string.IsNullOrEmpty(argsJson))
        {
            return [];
        }

        var value = JsonValues.ToObject(JsonNode.Parse(argsJson));

        return value is object?[] list ? list : [value];
    }

    /// <summary>A function's result as the values a node's ports take.</summary>
    /// <remarks>
    /// A sequence is one value per output, which is how a function that answers with several values
    /// feeds several ports; anything else is one value.
    /// </remarks>
    private static IReadOnlyList<object?> OutputsOf(object? result) => result switch
    {
        IReadOnlyList<object?> list => list,
        ITuple tuple => [.. Enumerable.Range(0, tuple.Length).Select(index => tuple[index])],
        _ => [result],
    };

    /// <summary>A function's arguments, as the types its parameters declare.</summary>
    /// <remarks>
    /// A value that already is the declared type is passed as it is — a string stays the string it
    /// was, rather than being written as JSON and read back — and anything else is converted through
    /// JSON, which is the only description of a value the graph has.
    /// </remarks>
    private static object?[] ArgumentsFor(Delegate function, IReadOnlyList<object?> arguments)
    {
        var parameters = BoundParameters(function);
        var values = new object?[parameters.Length];

        for (var index = 0; index < parameters.Length; index++)
        {
            var argument = index < arguments.Count ? arguments[index] : null;
            var type = parameters[index].ParameterType;

            values[index] = argument is null || type.IsInstanceOfType(argument)
                ? argument
                : TypedValues.From(JsonValues.FromAnyObject(argument), type);
        }

        return values;
    }

    /// <summary>The functions the canvas may call on the server.</summary>
    /// <remarks>
    /// <para>
    /// The ten of the Python version's that this build can answer. Five are the editor itself —
    /// whether it may write, the key it stores an arrangement under, the version of the app view a
    /// page holds, the functions it may add as nodes, and the endpoints it may call. Five more are
    /// the curated templates an author starts from: the two searches over them, the two lists the
    /// canvas filters by, and "is this repository one of them". Those five read a list the Python
    /// version ships, so they need no third party; the rest of the Python version's list — the
    /// token, the Space, the model and dataset callers, the save — belongs to integrations this
    /// build does not have. The canvas asks for each by name before using it, so an absent one is a
    /// feature it does without rather than a call that fails.
    /// </para>
    /// <para>
    /// The order is the Python version's, with the absent entries left out: the list is served in
    /// the config and compared with the Python version's, and reordering it would report a
    /// difference that is not one.
    /// </para>
    /// </remarks>
    private Dictionary<string, Delegate> ServerFunctions() =>
        new Dictionary<string, Delegate>(StringComparer.Ordinal)
        {
            ["get_write_access"] = new Func<JsonNode?, string>(_ => WriteAccess),
            ["get_workflow_key"] = new Func<JsonNode?, string>(_ => WorkflowKey()),
            ["get_app_version"] = new Func<JsonNode?, string>(_ => AppVersion),
            ["search_spaces"] = new Func<JsonNode?, string>(WorkflowCurated.Spaces),
            ["search_models"] = new Func<JsonNode?, string>(WorkflowCurated.Models),
            ["is_curated"] = new Func<JsonNode?, string>(WorkflowCurated.IsCurated),
            ["curated_modalities"] = new Func<JsonNode?, string>(WorkflowCurated.Modalities),
            ["curated_modality_tasks"] = new Func<JsonNode?, string>(WorkflowCurated.ModalityTasks),
            ["list_bound_fns"] = new Func<JsonNode?, string>(_ => BoundFunctionsText()),
            ["get_workflow_api"] = new Func<JsonNode?, string>(_ => WorkflowApiText()),
        };

    /// <summary>The version of the app view a page holds, as <c>structure.defaults</c>.</summary>
    /// <value>
    /// <c>1.0</c>: the app view has been built once, and nothing has changed the values it was built
    /// with. A page compares this against what it already has to know whether its copy is stale.
    /// </value>
    private const string AppVersion = "1.0";

    /// <summary>The key a visitor's arrangement of this workflow is stored under.</summary>
    /// <returns><c>file:</c> and the first sixteen hex digits of the file's absolute path hashed.</returns>
    /// <remarks>
    /// The path is hashed rather than sent as it is, so that a visitor's browser storage does not
    /// carry the author's directory layout. The workflow's name cannot do this job: it is editable
    /// from the canvas, and two unrelated workflows served from one origin in turn can share it —
    /// which would apply one arrangement to the other's nodes.
    /// </remarks>
    private string WorkflowKey()
    {
        var digest = SHA256.HashData(Encoding.UTF8.GetBytes(Path.GetFullPath(_graphFile)));

        return "file:" + Convert.ToHexString(digest)[..16].ToLowerInvariant();
    }

    /// <summary>The bound functions as the canvas adds them: one template per function.</summary>
    /// <remarks>
    /// A template is what a node is made of when an author adds one from the canvas's own list, so
    /// it carries the same ports the draft would have given it — re-adding a deleted node then
    /// produces the node it was.
    /// </remarks>
    private string BoundFunctionsText()
    {
        var templates = new JsonArray();

        foreach (var (name, function) in _bound)
        {
            var parameters = BoundParameters(function);
            var inputs = new JsonArray();

            foreach (var parameter in parameters)
            {
                if (DelegateInvocation.IsSuppliedByRun(parameter.ParameterType))
                {
                    continue;
                }

                inputs.Add(new JsonObject
                {
                    ["id"] = $"in_{parameter.Name}",
                    ["label"] = parameter.Name,
                    ["type"] = PortTypeOf(parameter.ParameterType),
                });
            }

            if (inputs.Count == 0)
            {
                inputs.Add(new JsonObject
                {
                    ["id"] = "in_0",
                    ["label"] = "input",
                    ["type"] = "text",
                });
            }

            templates.Add(new JsonObject
            {
                ["fn"] = name,
                ["label"] = name,
                ["inputs"] = inputs,
                ["outputs"] = new JsonArray
                {
                    new JsonObject
                    {
                        ["id"] = "out_0",
                        ["label"] = "output",
                        ["type"] = PortTypeOf(ReturnTypeOf(function)),
                    },
                },
            });
        }

        return PythonJson.Dump(templates);
    }

    /// <summary>The endpoints the workflow exposes, as the canvas's API panel shows them.</summary>
    private string WorkflowApiText()
    {
        var endpoints = new JsonArray();

        foreach (var endpoint in WorkflowGraph.FromJson(LoadInitial())?.DescribeEndpoints() ?? [])
        {
            endpoints.Add(new JsonObject
            {
                ["api_name"] = "/" + endpoint.ApiName,
                ["label"] = endpoint.Label,
                ["parameters"] = new JsonArray([.. endpoint.Parameters.Select(parameter => new JsonObject
                {
                    ["label"] = parameter.Label,
                    ["parameter_name"] = parameter.ParameterName,
                    ["type"] = parameter.Type,
                    ["python_type"] = parameter.PythonType,
                })]),
                ["returns"] = new JsonArray([.. endpoint.Returns.Select(value => new JsonObject
                {
                    ["label"] = value.Label,
                    ["type"] = value.Type,
                    ["python_type"] = value.PythonType,
                })]),
            });
        }

        return PythonJson.Dump(new JsonObject { ["endpoints"] = endpoints });
    }

    /// <summary>Where the graph file is, given what the application named.</summary>
    /// <param name="graph">The path the application gave, or <see langword="null"/>.</param>
    /// <param name="callerFilePath">The source file that called the constructor, or <see langword="null"/>.</param>
    /// <returns>The absolute path.</returns>
    private static string ResolveGraphFile(string? graph, string? callerFilePath)
    {
        var file = string.IsNullOrEmpty(graph) ? DefaultGraphFile : graph;

        if (Path.IsPathRooted(file))
        {
            return Path.GetFullPath(file);
        }

        return Path.GetFullPath(Path.Combine(DirectoryOf(callerFilePath), file));
    }

    /// <summary>The directory the application's code is in, or the process's own.</summary>
    /// <remarks>
    /// A published build carries no source tree, and the path the compiler recorded points at a
    /// directory that is not there: a workflow then reads its file from where the process was
    /// started, which is the only place left that means anything.
    /// </remarks>
    private static string DirectoryOf(string? callerFilePath)
    {
        if (!string.IsNullOrEmpty(callerFilePath) && Path.IsPathRooted(callerFilePath))
        {
            var directory = Path.GetDirectoryName(callerFilePath);

            if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
            {
                return directory;
            }
        }

        return Environment.CurrentDirectory;
    }
}

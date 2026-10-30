using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Gradio.Net.Analyzers.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Gradio.Net.Analyzers;

/// <summary>
/// Generates each component's trigger set and raw bindings from the recorded Python events.
/// </summary>
/// <remarks>
/// <para>
/// Which signals a component raises is a fact about the Python release, not about this project:
/// a textbox raises eight, an HTML block raises all thirty-four, a button raises two. Written by
/// hand that fact appears three times per component — once as a trigger property, once as a
/// binding, once in a doc comment — and the way the copy fails is by being short:
/// <c>Html</c> declared nine of its thirty-four for two releases, and nothing noticed, because the
/// only comparison was a person reading the Python version's class body.
/// </para>
/// <para>
/// So the list is recorded (<c>component-events.json</c>, read from the Python version's
/// <c>EVENTS</c>) and the two mechanical halves are generated from it. What stays hand-written is
/// the half that is not mechanical: the typed binding of each event, whose handler parameter is
/// this build's choice of C# type for the component's value, and the prose that explains an event's
/// meaning.
/// </para>
/// <para>
/// A component whose base already declares its triggers generates nothing: a template's events are
/// its base's (the Python version inherits <c>EVENTS</c> the same way), so generating them twice
/// would be two declarations of one set.
/// </para>
/// </remarks>
[Generator(LanguageNames.CSharp)]
public sealed class ComponentEventsGenerator : IIncrementalGenerator
{
    /// <summary>The diagnostic reported when the Python version names an event this build has no
    /// constant for.</summary>
    public const string UnknownEventDiagnosticId = "GRADIO0009";

    /// <summary>The diagnostic reported when the event manifest is not readable.</summary>
    public const string UnreadableEventsDiagnosticId = "GRADIO0010";

    private const string AttributeName = "Gradio.Net.GradioBlockAttribute";
    private const string HandlerValueAttributeName = "Gradio.Net.HandlerValueAttribute";
    private const string ManifestSuffix = "component-events.json";
    private const string EventsBaseType = "Gradio.Net.ComponentEvents";
    private const string ComponentType = "Gradio.Net.Component";
    private const string EventNamesType = "Gradio.Net.EventNames";
    private const string TriggerType = "Gradio.Net.EventTrigger";
    private const string Category = "Gradio.Architecture";

    private static readonly DiagnosticDescriptor _unknownEvent = new(
        UnknownEventDiagnosticId,
        "The Python version names an event this build has no constant for",
        "'{0}' declares the event '{1}' for the block '{2}', and there is no constant for it in "
        + "EventNames; the trigger would be a name nothing can bind",
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Every event the Python version declares has to be spellable here, or the generated trigger set is missing one.");

    private static readonly DiagnosticDescriptor _unreadableEvents = new(
        UnreadableEventsDiagnosticId,
        "The event manifest is not readable",
        "The event manifest could not be read ({0}); the trigger sets cannot be written from it",
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The manifest is recorded from the Python release and checked in; the build reads it rather than a copy of it.");

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var manifests = context.AdditionalTextsProvider
            .Where(static text => text.Path.EndsWith(ManifestSuffix, StringComparison.OrdinalIgnoreCase))
            .Select(static (text, token) => text.GetText(token)?.ToString())
            .Collect();

        var blocks = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                AttributeName,
                predicate: static (node, _) => node is ClassDeclarationSyntax,
                transform: static (attribute, _) => Block.From(attribute))
            .Where(static block => block is not null)
            .Select(static (block, _) => block!);

        context.RegisterSourceOutput(
            blocks.Combine(manifests),
            static (production, pair) => Emit(production, pair.Left, pair.Right));
    }

    private static void Emit(SourceProductionContext production, Block block, ImmutableArray<string?> manifests)
    {
        // A layout is a block but not a component: it raises no value, so the framework never asks
        // it for a trigger set. Its events belong to the batch that builds the layout's scopes.
        if (!block.IsComponent || block.DeclaresItsOwnEvents is false)
        {
            return;
        }

        if (!Manifest.TryRead(manifests, out var manifest, out var problem))
        {
            production.ReportDiagnostic(Diagnostic.Create(_unreadableEvents, block.Location, problem));

            return;
        }

        if (!manifest.TryGet(block.BlockName, out var events))
        {
            return;
        }

        // The Python version lists `pause` twice for the microphone; one trigger is one trigger.
        var names = events.Distinct(StringComparer.Ordinal).ToArray();

        foreach (var name in names)
        {
            if (block.ConstantFor(name) is null)
            {
                production.ReportDiagnostic(Diagnostic.Create(
                    _unknownEvent,
                    block.Location,
                    block.Name,
                    name,
                    block.BlockName));

                return;
            }
        }

        production.AddSource(
            $"{block.Name}.Events.g.cs",
            SourceText.From(BuildSource(block, names), Encoding.UTF8));
    }

    /// <summary>The binding parameters every event method takes, in the Python version's order.</summary>
    /// <remarks>
    /// The Python version passes these as a keyword list to every event method, and so does this
    /// side: one list, written once here and emitted into every generated method, so a parameter
    /// the framework adds later reaches every component at once.
    /// </remarks>
    private const string BINDING_PARAMETERS =
        """
                IReadOnlyList<string?>? parameterNames = null,
                bool preprocess = true,
                bool postprocess = true,
                bool scrollToOutput = false,
                ProgressDisplay? showProgress = null,
                IReadOnlyList<Block>? showProgressOn = null,
                string? apiName = null,
                string? apiDescription = null,
                string? endpointDescription = null,
                string? js = null,
                bool jsRunsInBrowser = false,
                string? jsImplementation = null,
                bool queue = true,
                bool batch = false,
                int maxBatchSize = 4,
                TriggerMode? triggerMode = null,
                ConcurrencyLimit concurrencyLimit = default,
                string? concurrencyId = null,
                ApiVisibility apiVisibility = ApiVisibility.Public,
                double? timeLimit = null,
                double streamEvery = 0.5,
                IReadOnlyList<Dependency>? cancels = null,
                Delegate? validator = null
                ) =>
        """;
    /// <summary>The record those parameters are carried in, built inside the method.</summary>
    /// <returns>The source of the record, ready to be passed to the binder.</returns>
    private static string BindingOptions() =>
        """
                new EventBindingOptions
                {
                    ParameterNames = parameterNames,
                    Preprocess = preprocess,
                    Postprocess = postprocess,
                    ScrollToOutput = scrollToOutput,
                    ShowProgress = showProgress,
                    ShowProgressOn = showProgressOn,
                    ApiName = apiName,
                    ApiDescription = apiDescription,
                    EndpointDescription = endpointDescription,
                    Js = js,
                    JsRunsInBrowser = jsRunsInBrowser,
                    JsImplementation = jsImplementation,
                    Queue = queue,
                    Batch = batch,
                    MaxBatchSize = maxBatchSize,
                    TriggerMode = triggerMode,
                    ConcurrencyLimit = concurrencyLimit,
                    ConcurrencyId = concurrencyId,
                    ApiVisibility = apiVisibility,
                    TimeLimit = timeLimit,
                    StreamEvery = streamEvery,
                    Cancels = cancels,
                    Validator = validator,
                }
        """;
    /// <summary>Writes one doc line per binding parameter, in the order they are declared.</summary>
    /// <param name="source">The source being written.</param>
    private static void AppendBindingDocs(StringBuilder source)
    {
        foreach (var line in BINDING_PARAMETERS.Split('\n'))
        {
            var core = line.Trim().TrimEnd(',');

            // The list ends with the closing `) =>`, which is not a parameter.
            if (core.Length == 0 || core.StartsWith(")", StringComparison.Ordinal))
            {
                continue;
            }

            var name = core.Split('=')[0].Trim().Split(' ').Last();
            source.Append("    /// <param name=\"").Append(name).AppendLine("\">A binding parameter.</param>");
        }
    }

    private static string BuildSource(Block block, IReadOnlyList<string> events)
    {
        var source = new StringBuilder();
        var eventsType = block.Name + "Events";

        source.AppendLine("// <auto-generated />");
        source.AppendLine("// Written from the recorded Python events; do not edit.");
        source.AppendLine("#nullable enable");
        source.AppendLine();
        source.AppendLine("using System;");
        source.AppendLine("using System.Collections.Generic;");
        source.AppendLine();
        source.Append("namespace ").Append(block.Namespace).AppendLine(";");
        source.AppendLine();
        source.Append("public partial class ").AppendLine(block.Name);
        source.AppendLine("{");
        source.Append("    /// <summary>The signals this component raises.</summary>");
        source.AppendLine();
        source.Append("    public override ").Append(eventsType).AppendLine(" Events => new(this);");

        foreach (var (name, valueType) in block.TypedEvents(events))
        {
            source.AppendLine();
            source.Append("    /// <summary>Binds the <c>").Append(name)
                .AppendLine("</c> event to a handler that receives the component's value.</summary>");
            source.AppendLine("    /// <param name=\"fn\">The handler, whose first parameter is the component's value.</param>");
            source.AppendLine("    /// <param name=\"inputs\">The components the handler reads.</param>");
            source.AppendLine("    /// <param name=\"outputs\">The components the handler's result updates.</param>");
            AppendBindingDocs(source);
            source.AppendLine("    /// <returns>The binding.</returns>");
            source.Append("    public Dependency ").Append(Pascal(name)).AppendLine("(");
            source.Append("        Func<").Append(valueType).AppendLine(", object?> fn,");
            source.AppendLine("        Block[] inputs,");
            source.AppendLine("        Block[] outputs,");
            source.AppendLine(BINDING_PARAMETERS);
            source.Append("        BindEvent(Events.").Append(Pascal(name)).Append(", fn, inputs, outputs, ")
                .Append(BindingOptions()).AppendLine(");");
        }

        foreach (var name in events)
        {
            source.AppendLine();
            source.Append("    /// <summary>Binds the <c>").Append(name)
                .AppendLine("</c> event to a handler of any shape.</summary>");
            source.AppendLine("    /// <param name=\"fn\">The handler, or <see langword=\"null\"/> for a frontend-only binding.</param>");
            source.AppendLine("    /// <param name=\"inputs\">The components the handler reads.</param>");
            source.AppendLine("    /// <param name=\"outputs\">The components the handler's result updates.</param>");
            AppendBindingDocs(source);
            source.AppendLine("    /// <returns>The binding.</returns>");
            source.Append("    public Dependency ").Append(Pascal(name)).AppendLine("(");
            source.AppendLine("        Delegate? fn,");
            source.AppendLine("        object[]? inputs = null,");
            source.AppendLine("        object[]? outputs = null,");
            source.AppendLine(BINDING_PARAMETERS);
            source.Append("        BindEventRaw(Events.").Append(Pascal(name)).Append(", fn, inputs, outputs, ")
                .Append(BindingOptions()).AppendLine(");");
        }

        source.AppendLine("}");
        source.AppendLine();
        source.Append("/// <summary>The signals ").Append(block.Name).AppendLine(" raises.</summary>");
        source.Append("public sealed class ").Append(eventsType).AppendLine(" : ComponentEvents");
        source.AppendLine("{");
        source.Append("    /// <summary>Creates the trigger set of a block.</summary>");
        source.AppendLine();
        source.AppendLine("    /// <param name=\"owner\">The block the triggers belong to.</param>");
        source.Append("    internal ").Append(eventsType).AppendLine("(Block owner)");
        source.AppendLine("        : base(owner)");
        source.AppendLine("    {");
        source.AppendLine("    }");

        foreach (var name in events)
        {
            var constant = block.ConstantFor(name)!;

            source.AppendLine();
            source.Append("    /// <summary>The <c>").Append(name).AppendLine("</c> signal.</summary>");
            source.Append("    public ").Append(block.IsBaseTrigger(Pascal(name)) ? "override " : string.Empty)
                .Append("EventTrigger ").Append(Pascal(name)).Append(" => new(Owner, EventNames.")
                .Append(constant).AppendLine(");");
        }

        source.AppendLine("}");
        return source.ToString();
    }

    /// <summary>The C# spelling of an event name: <c>key_up</c> is <c>KeyUp</c>.</summary>
    private static string Pascal(string name) => CSharpName.PascalCase(name);

    /// <summary>One attributed class, reduced to what the generator needs from it.</summary>
    private sealed class Block
    {
        private readonly INamedTypeSymbol _type;
        private readonly IReadOnlyList<(string Event, string ValueType)> _typedEvents;
        private readonly INamedTypeSymbol? _eventsBase;
        private readonly INamedTypeSymbol? _eventNames;
        private readonly INamedTypeSymbol? _component;

        private Block(
            INamedTypeSymbol type,
            string block,
            string @namespace,
            INamedTypeSymbol? eventsBase,
            INamedTypeSymbol? eventNames,
            INamedTypeSymbol? component,
            Location location)
        {
            _type = type;
            _typedEvents = ReadTypedEvents(type);
            _eventsBase = eventsBase;
            _eventNames = eventNames;
            _component = component;
            BlockName = block;
            Name = type.Name;
            Namespace = @namespace;
            Location = location;
        }

        /// <summary>Whether the block is a component, which is the layer that raises events.</summary>
        internal bool IsComponent => _component is not null && InheritsFrom(_type, _component);

        internal string BlockName { get; }

        internal string Name { get; }

        internal string Namespace { get; }

        internal Location Location { get; }

        /// <summary>Whether this class is where the component's triggers are declared.</summary>
        /// <value>
        /// <see langword="false"/> for a class that inherits them. Two kinds of class do: a
        /// template — <c>TextArea</c> is <c>Textbox</c> with other defaults, and the Python version
        /// inherits <c>EVENTS</c> the same way — and a block that re-uses another block's name,
        /// like the audio gallery, which is an <c>Html</c> component and says so. Both are answered
        /// by the class's own name and its base: the class that *is* the block, at the root of its
        /// own hierarchy, is the one that declares the set.
        /// </value>
        internal bool DeclaresItsOwnEvents =>
            string.Equals(Name.ToLowerInvariant(), BlockName, StringComparison.Ordinal)
            && !HasAttributedBase
            && !HasInheritedEvents;

        /// <summary>Whether a base type declares triggers of its own without being a block.</summary>
        /// <value>
        /// The three plot components are one: <c>BarPlot</c>, <c>LinePlot</c> and
        /// <c>ScatterPlot</c> are a shared <c>NativePlot</c> with a different value, and that base
        /// is not a block of its own, so it carries the trigger set its subclasses inherit.
        /// </value>
        private bool HasInheritedEvents
        {
            get
            {
                for (var current = _type.BaseType; current is not null && current.Name != "Component"; current = current.BaseType)
                {
                    if (current.GetMembers("Events").OfType<IPropertySymbol>().Any())
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>Whether a base type is itself a block, and so already declares these triggers.</summary>
        private bool HasAttributedBase
        {
            get
            {
                for (var current = _type.BaseType; current is not null; current = current.BaseType)
                {
                    foreach (var attribute in current.GetAttributes())
                    {
                        if (attribute.AttributeClass?.ToDisplayString() == AttributeName)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        /// <summary>The events whose handler is given the component's value, and the type it
        /// takes.</summary> <param name="events">The events the Python version declares for the
        /// block, in the Python version's order.</param> <returns>The typed events, in the Python
        /// version's order, with the type spelled for C#.</returns>
        /// <remarks>
        /// Declared on the component (<c>HandlerValue</c>) rather than read from the manifest,
        /// because the type is this build's spelling of the value and not the Python version's
        /// declaration of the event. An event declared here that the Python version does not list
        /// is not generated: the recorded list is what the trigger set is written from, and a typed
        /// binding for a signal the component does not raise would be a method nothing can call.
        /// </remarks>
        internal IReadOnlyList<(string Event, string ValueType)> TypedEvents(IReadOnlyList<string> events) =>
            [.. events
                .Select(name => (Event: name, Value: _typedEvents.FirstOrDefault(typed => string.Equals(typed.Event, name, StringComparison.Ordinal)).ValueType))
                .Where(typed => typed.Value is not null)
                .Select(typed => (typed.Event, typed.Value!))];

        private static List<(string Event, string ValueType)> ReadTypedEvents(INamedTypeSymbol type)
        {
            var declarations = new List<(string, string)>();

            // The whole chain, because the three plot components are one chart with three values
            // and the declaration sits on the base they share — which is not a block of its own and
            // so never gets a trigger set of its own either.
            for (var current = type; current is not null; current = current.BaseType)
            {
                declarations.AddRange(DeclaredOn(current));
            }

            return declarations;
        }

        private static List<(string Event, string ValueType)> DeclaredOn(INamedTypeSymbol type)
        {
            var declarations = new List<(string, string)>();

            foreach (var attribute in type.GetAttributes())
            {
                if (attribute.AttributeClass?.ToDisplayString() != HandlerValueAttributeName
                    || attribute.ConstructorArguments.Length != 2
                    || attribute.ConstructorArguments[0].Value is not string spelling
                    || attribute.ConstructorArguments[1].Kind != TypedConstantKind.Array)
                {
                    continue;
                }

                foreach (var @event in attribute.ConstructorArguments[1].Values)
                {
                    if (@event.Value is string name)
                    {
                        declarations.Add((name, spelling));
                    }
                }
            }

            return declarations;
        }

        /// <summary>The constant in <c>EventNames</c> that spells a Python event, if there is
        /// one.</summary> <param name="name">The event's name, as the Python version spells
        /// it.</param> <returns>The constant's name, or <see langword="null"/>.</returns>
        internal string? ConstantFor(string name)
        {
            if (_eventNames is null)
            {
                return null;
            }

            var candidate = Pascal(name);

            return _eventNames.GetMembers(candidate).OfType<IFieldSymbol>().Any()
                ? candidate
                : null;
        }

        /// <summary>Whether the shared base already declares a trigger with this name.</summary>
        /// <param name="name">The trigger's name, spelled the C# way.</param>
        /// <returns>Whether the generated property has to override it.</returns>
        /// <remarks>
        /// Two triggers are asked about by the framework itself rather than by a component —
        /// <c>Submit</c> and <c>Stream</c> — because an <c>Interface</c> has to know whether a
        /// component can be submitted or streams before it can bind anything. A component that
        /// raises one overrides it; a component that does not inherits the null.
        /// </remarks>
        internal bool IsBaseTrigger(string name) =>
            _eventsBase?.GetMembers(name).OfType<IPropertySymbol>().Any() == true;

        private static bool InheritsFrom(INamedTypeSymbol type, INamedTypeSymbol ancestor)
        {
            for (var current = type.BaseType; current is not null; current = current.BaseType)
            {
                if (SymbolEqualityComparer.Default.Equals(current, ancestor))
                {
                    return true;
                }
            }

            return false;
        }

        internal static Block? From(GeneratorAttributeSyntaxContext attribute)
        {
            if (attribute.TargetSymbol is not INamedTypeSymbol type
                || type.ContainingType is not null
                || type.TypeParameters.Length > 0
                || attribute.Attributes.Length == 0
                || attribute.Attributes[0].ConstructorArguments.Length == 0
                || attribute.Attributes[0].ConstructorArguments[0].Value is not string block)
            {
                return null;
            }

            var @namespace = type.ContainingNamespace is { IsGlobalNamespace: false } containing
                ? containing.ToDisplayString()
                : string.Empty;

            return new Block(
                type,
                block,
                @namespace,
                attribute.SemanticModel.Compilation.GetTypeByMetadataName(EventsBaseType),
                attribute.SemanticModel.Compilation.GetTypeByMetadataName(EventNamesType),
                attribute.SemanticModel.Compilation.GetTypeByMetadataName(ComponentType),
                type.Locations.Length > 0 ? type.Locations[0] : Location.None);
        }
    }

    /// <summary>The Python event manifest: each block's events, in the Python version's
    /// order.</summary>
    private sealed class Manifest
    {
        private readonly Dictionary<string, IReadOnlyList<string>> _blocks;

        private Manifest(Dictionary<string, IReadOnlyList<string>> blocks) => _blocks = blocks;

        internal bool TryGet(string block, out IReadOnlyList<string> events)
        {
            if (_blocks.TryGetValue(block, out var declared))
            {
                events = declared;
                return true;
            }

            events = Array.Empty<string>();
            return false;
        }

        internal static bool TryRead(ImmutableArray<string?> manifests, out Manifest manifest, out string problem)
        {
            var blocks = new Dictionary<string, IReadOnlyList<string>>(StringComparer.Ordinal);
            manifest = new Manifest(blocks);
            problem = string.Empty;

            var supplied = manifests.Where(text => text is not null).ToArray();
            if (supplied.Length == 0)
            {
                problem = $"no '{ManifestSuffix}' was supplied to this build";
                return false;
            }

            try
            {
                var document = JsonValue.Parse(supplied[0]!);
                if (document.Member("blocks")?.Object is not { } declared)
                {
                    problem = "the manifest declares no 'blocks'";
                    return false;
                }

                foreach (var block in declared)
                {
                    if (block.Value.Member("events")?.Array is not { } events)
                    {
                        continue;
                    }

                    blocks[block.Key] = [.. events.Select(item => item.AsString() ?? string.Empty)
                        .Where(name => name.Length > 0)];
                }

                return true;
            }
            catch (FormatException error)
            {
                problem = error.Message;
                return false;
            }
        }
    }
}

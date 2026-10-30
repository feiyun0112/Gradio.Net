using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
using Gradio.Net.Analyzers.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Gradio.Net.Analyzers;

/// <summary>
/// Generates the <c>gr</c> compatibility layer's factories from the recorded Python signature.
/// </summary>
/// <remarks>
/// <para>
/// The compatibility layer exists so that a ported application reads the same in C# as it did in
/// Python, which means every component the Python version exports needs a factory whose parameters
/// are the Python version's — its names, their order and their defaults. Written by hand that is
/// more than a thousand parameters to copy, and the way such a copy fails is by being almost right:
/// a default of 160 where the Python version has 200 compiles, runs, and draws a subtly different
/// page.
/// </para>
/// <para>
/// So it is generated, from the same manifest the props mapping is generated from, and the two
/// halves of "the same block" cannot disagree. What the generator takes from the compilation is
/// everything a manifest cannot know: the member each parameter writes, the member's type, and
/// whether it is nullable. What it takes from the manifest is what only the Python version knows:
/// the parameter list, its order, and each default.
/// </para>
/// <para>
/// A parameter whose default C# cannot spell — a list, a visibility, a component or a document — is
/// declared <see langword="null"/> and written only when it was given. That is not a shortcut: the
/// block's own default is the Python version's, so writing nothing is the same as writing the
/// Python version's default. The one place that is not true is a mistake the defaults comparison
/// reports.
/// </para>
/// <para>
/// The generator covers the component layer. A layout's factory is only half of what makes a layout
/// usable — the scope that enters it is the other half, and that is the layout batch's work — so a
/// layout keeps its place in the ledger rather than arriving here as a block nothing can nest into.
/// </para>
/// <para>
/// Every factory ends by placing the block it built (<c>Ambient.Place</c>), because the Python
/// version's factory is a line on the page rather than a value: <c>gr.Textbox(...)</c> in Python
/// puts the textbox on the page it is written on, and a C# factory that only returned the block
/// would make every ported line say <c>demo.Add(...)</c> as well. The placement is the one thing a
/// factory does that is not an assignment, and it is the same one line for all of them.
/// </para>
/// </remarks>
[Generator(LanguageNames.CSharp)]
public sealed class GrFactoryGenerator : IIncrementalGenerator
{
    /// <summary>The diagnostic reported when a parameter cannot be written into a factory.</summary>
    public const string UnwritableParameterDiagnosticId = "GRADIO0007";

    /// <summary>The diagnostic reported when a declaration the factories are written from is not readable.</summary>
    public const string UnreadableDeclarationDiagnosticId = "GRADIO0008";

    private const string AttributeName = "Gradio.Net.GradioBlockAttribute";
    private const string ParameterNameAttributeName = "Gradio.Net.UpstreamParameterAttribute";
    private const string FactoryArgumentAttributeName = "Gradio.Net.FactoryArgumentAttribute";
    private const string ParamsSuffix = "component-params.json";
    private const string ExportsSuffix = "exports.json";
    private const string ComponentKind = "component";
    private const string Category = "Gradio.Architecture";

    /// <summary>
    /// The spellings the Python version exports that name the same class as another spelling.
    /// </summary>
    /// <remarks>
    /// Three of the Python version's exports are assignments rather than classes — <c>Text =
    /// Textbox</c>, <c>Highlight = HighlightedText</c>, <c>Mic = Microphone</c> — so there is no
    /// block of that name to write a signature from and the signature is the class's own. The rest
    /// of the aliases (<c>DataFrame</c>, <c>Json</c>, <c>Checkboxgroup</c>…) are spelled the way
    /// their block is once the case is folded, so they need no entry here.
    /// </remarks>
    private static readonly string[] _aliases = ["Highlight", "Mic", "Text"];

    /// <summary>The block an export stands for, when the Python version spells it as an
    /// assignment.</summary> <param name="name">The exported name.</param> <returns>The block's
    /// name, or <see langword="null"/> when the name is a block of its own.</returns>
    private static string? AliasTarget(string name) => name switch
    {
        "Highlight" => "highlightedtext",
        "Mic" => "microphone",
        "Text" => "textbox",
        _ => null,
    };

    private static readonly DiagnosticDescriptor _unwritable = new(
        UnwritableParameterDiagnosticId,
        "A block's parameter cannot be written into a factory",
        "'{0}' declares '{1}' for the block '{2}', and it cannot be written from a parameter ({3}); "
        + "the factory would be a signature that silently drops it",
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Every parameter the Python version declares has to be writable into the member it names, or the factory is missing a parameter the Python version has.");

    private static readonly DiagnosticDescriptor _unreadable = new(
        UnreadableDeclarationDiagnosticId,
        "A declaration the factories are written from is not readable",
        "{0} could not be read ({1}); the factories cannot be written from it",
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The declarations are recorded from the Python release and checked in; the build reads them rather than a copy of them.");

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var paramsManifests = context.AdditionalTextsProvider
            .Where(static text => text.Path.EndsWith(ParamsSuffix, StringComparison.OrdinalIgnoreCase))
            .Select(static (text, token) => text.GetText(token)?.ToString())
            .Collect();

        var exportManifests = context.AdditionalTextsProvider
            .Where(static text => text.Path.EndsWith(ExportsSuffix, StringComparison.OrdinalIgnoreCase))
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
            blocks.Combine(paramsManifests).Combine(exportManifests),
            static (production, pair) => Emit(production, pair.Left.Left, pair.Left.Right, pair.Right));
    }

    private static void Emit(
        SourceProductionContext production,
        Block block,
        ImmutableArray<string?> paramsManifests,
        ImmutableArray<string?> exportManifests)
    {
        if (!Manifest.TryRead(paramsManifests, out var manifest, out var manifestProblem))
        {
            production.ReportDiagnostic(Diagnostic.Create(_unreadable, block.Location, ParamsSuffix, manifestProblem));

            return;
        }

        if (!Exports.TryRead(exportManifests, out var exported, out var exportsProblem))
        {
            production.ReportDiagnostic(Diagnostic.Create(_unreadable, block.Location, ExportsSuffix, exportsProblem));

            return;
        }

        if (!manifest.TryGet(block.BlockName, out var parameters, out var defaults))
        {
            return;
        }

        // A block whose class is not the block it names is a template the framework composes with —
        // a coloured checkbox group is an HTML component, an audio gallery is a gallery — and the
        // factory is the one the class that *is* the block writes. Two classes claiming one block
        // would otherwise generate one method twice.
        if (!string.Equals(block.Name.ToLowerInvariant(), block.BlockName, StringComparison.Ordinal))
        {
            return;
        }

        var names = exported
            .Where(export => string.Equals(export.Key.ToLowerInvariant(), block.BlockName, StringComparison.Ordinal))
            .Where(export => string.Equals(export.Value, ComponentKind, StringComparison.Ordinal))
            .Select(export => export.Key)
            .Concat(_aliases.Where(alias => string.Equals(AliasTarget(alias), block.BlockName, StringComparison.Ordinal)))
            .OrderBy(name => string.Equals(name, block.Name, StringComparison.Ordinal) ? 0 : 1)
            .ThenBy(name => name, StringComparer.Ordinal)
            .ToArray();

        if (names.Length == 0)
        {
            // A block the Python version does not export as a component: a template the framework composes
            // with (a clear button is one), one an application never names (the API component), or
            // a layout, whose factory is only half of what makes it usable — the scope that enters
            // it is the other half, and that is the layout batch's work.
            return;
        }

        var arguments = new List<Argument>(parameters.Count);
        foreach (var parameter in parameters)
        {
            var argument = Argument.For(block, parameter, defaults);
            if (argument.Problem is { } reason)
            {
                production.ReportDiagnostic(Diagnostic.Create(
                    _unwritable,
                    block.Location,
                    block.Name,
                    parameter,
                    block.BlockName,
                    reason));

                return;
            }

            arguments.Add(argument);
        }

        production.AddSource(
            $"gr.{block.Name}.g.cs",
            SourceText.From(BuildSource(block, names, arguments), Encoding.UTF8));
    }

    private static string BuildSource(Block block, string[] names, IReadOnlyList<Argument> arguments)
    {
        var source = new StringBuilder();
        source.AppendLine("// <auto-generated />");
        source.AppendLine("// Written from the recorded Python signature; do not edit.");
        source.AppendLine("#nullable enable");
        source.AppendLine();
        source.AppendLine("using System.Collections.Generic;");
        source.AppendLine("using System.Text.Json.Nodes;");
        source.AppendLine();
        source.AppendLine("namespace Gradio.Net;");
        source.AppendLine();

        // The one warning the generated file would raise, switched off in the file rather than in
        // `.editorconfig`: a rule's severity in an editor config is not applied to a source file
        // the compiler did not read from disk, so the narrow exemption that covers `Gr/Gr.cs` does
        // not reach this one. It is the same exemption for the same reason: the layer's
        // entry point is the Python version's module name, spelled in lower case on purpose.
        source.AppendLine("#pragma warning disable CS8981");
        source.AppendLine("public static partial class gr");
        source.AppendLine("{");

        var primary = names[0];

        source.Append("    /// <summary>Builds the Python version's <c>").Append(block.BlockName).Append("</c>: a <c>")
            .Append(block.Name).AppendLine("</c>, with the Python version's parameters in the Python version's order.</summary>");
        source.AppendLine("    /// <returns>The block, which is the core type rather than a wrapper around it.</returns>");
        source.AppendLine("    /// <remarks>");
        source.AppendLine("    /// <para>");
        source.AppendLine("    /// The block is placed on the page being written — the Python version's factory is a line on the page,");
        source.AppendLine("    /// not a value — and returned so that it can be named, bound and passed on.");
        source.AppendLine("    /// </para>");
        source.AppendLine("    /// <para>");
        source.AppendLine("    /// Built outside a render, it is built and not placed: a factory is usable on its own, and a");
        source.AppendLine("    /// block that is not on a page yet is not an error.");
        source.AppendLine("    /// </para>");
        source.AppendLine("    /// </remarks>");
        source.Append("    public static ").Append(block.QualifiedName).Append(' ').Append(primary).Append('(');
        AppendParameters(source, arguments);
        source.AppendLine(")");
        source.AppendLine("    {");
        source.Append("        var block = new ").Append(block.QualifiedName).AppendLine("();");

        foreach (var argument in arguments)
        {
            source.AppendLine();
            argument.Write(source);
        }

        source.AppendLine();
        source.AppendLine("        return Ambient.Place(block);");
        source.AppendLine("    }");

        for (var index = 1; index < names.Length; index++)
        {
            source.AppendLine();
            source.Append("    /// <summary>Builds the same block as <c>").Append(primary)
                .AppendLine("</c>: the Python version exports both spellings.</summary>");
            source.Append("    public static ").Append(block.QualifiedName).Append(' ').Append(names[index]).Append('(');
            AppendParameters(source, arguments);
            source.Append(") => ").Append(primary).Append('(');
            source.Append(string.Join(", ", arguments.Select(argument => argument.Name)));
            source.AppendLine(");");
        }

        source.AppendLine("}");
        return source.ToString();
    }

    private static void AppendParameters(StringBuilder source, IReadOnlyList<Argument> arguments)
    {
        if (arguments.Count == 0)
        {
            return;
        }

        source.AppendLine();

        for (var index = 0; index < arguments.Count; index++)
        {
            source.Append("        ").Append(arguments[index].Declaration);
            source.AppendLine(index == arguments.Count - 1 ? string.Empty : ",");
        }

        source.Append("    ");
    }

    /// <summary>
    /// The shapes the Python version accepts for a parameter that is sometimes one thing and
    /// sometimes several, as <c>FactoryArgumentKind</c> declares them on the member.
    /// </summary>
    /// <remarks>
    /// A copy of the framework's enum rather than the enum itself: the framework's is internal to
    /// the package, and an analyzer is a separate assembly that reads the annotation's argument as
    /// the integer it is. The two are read together — the numbers here are the numbers there.
    /// </remarks>
    private enum Shape
    {
        /// <summary>One item, or a sequence of them.</summary>
        OneOrMany,

        /// <summary>One name, or a sequence of names.</summary>
        Names,
    }

    /// <summary>One attributed class, reduced to what the generator needs from it.</summary>
    private sealed class Block
    {
        private readonly INamedTypeSymbol _type;

        private Block(INamedTypeSymbol type, string block, string name, Location location)
        {
            _type = type;
            BlockName = block;
            Name = name;
            Location = location;
            QualifiedName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }

        internal string BlockName { get; }

        internal string Name { get; }

        /// <summary>The type's name with its namespace, which the generated file cannot leave to a using.</summary>
        /// <remarks>
        /// Three of the Python version's components are named after a type the framework already
        /// has in scope — <c>DateTime</c>, <c>File</c>, <c>List</c> — and a generated file that
        /// wrote the short name would be ambiguous with <see cref="System.DateTime"/> and
        /// <see cref="System.IO.File"/>. Qualifying the one name the factory returns is cheaper
        /// than qualifying every parameter type as well.
        /// </remarks>
        internal string QualifiedName { get; }

        internal Location Location { get; }

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

            return new Block(
                type,
                block,
                type.Name,
                type.Locations.Length > 0 ? type.Locations[0] : Location.None);
        }

        /// <summary>The member a parameter is written into, or <see langword="null"/>.</summary>
        /// <param name="parameter">The parameter's name, as the Python version spells it.</param>
        /// <returns>The property, when the block declares one a parameter can be written
        /// into.</returns>
        /// <remarks>
        /// The whole base chain is searched, because the parameters a component shares with every
        /// other component are declared once on the base they share it with. A member may be named
        /// differently from its parameter when the conventional name is taken — see
        /// <c>UpstreamParameterAttribute</c> — and the attribute is what keeps that from being a
        /// parameter the build silently forgets.
        /// </remarks>
        internal IPropertySymbol? PropertyFor(string parameter)
        {
            for (var current = _type; current is not null; current = current.BaseType)
            {
                foreach (var member in current.GetMembers().OfType<IPropertySymbol>())
                {
                    if (IsWritable(member) && NamesParameter(member, parameter))
                    {
                        return member;
                    }
                }
            }

            var conventional = CSharpName.PascalCase(parameter);

            for (var current = _type; current is not null; current = current.BaseType)
            {
                foreach (var member in current.GetMembers(conventional).OfType<IPropertySymbol>())
                {
                    if (IsWritable(member))
                    {
                        return member;
                    }
                }
            }

            return null;
        }

        /// <summary>The shape a factory's parameter for a member takes, or <see langword="null"/>.</summary>
        /// <param name="property">The member the parameter writes.</param>
        /// <returns>The declared shape, when the member declares one.</returns>
        internal static Shape? ShapeOf(IPropertySymbol property)
        {
            IPropertySymbol? current = property;

            for (; current is not null; current = current.OverriddenProperty)
            {
                foreach (var attribute in current.GetAttributes())
                {
                    if (attribute.AttributeClass?.ToDisplayString() != FactoryArgumentAttributeName
                        || attribute.ConstructorArguments.Length != 1
                        || attribute.ConstructorArguments[0].Value is not int kind)
                    {
                        continue;
                    }

                    return kind switch
                    {
                        0 => Shape.OneOrMany,
                        1 => Shape.Names,
                        _ => null,
                    };
                }
            }

            return null;
        }

        private static bool IsWritable(IPropertySymbol member) =>
            !member.IsStatic && !member.IsIndexer && member.SetMethod is not null;

        private static bool NamesParameter(IPropertySymbol property, string parameter)
        {
            foreach (var attribute in property.GetAttributes())
            {
                if (attribute.AttributeClass?.ToDisplayString() == ParameterNameAttributeName
                    && attribute.ConstructorArguments.Length == 1
                    && attribute.ConstructorArguments[0].Value is string declared
                    && string.Equals(declared, parameter, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }


    }

    /// <summary>One factory parameter: how it is declared, and what it writes.</summary>
    private sealed class Argument
    {
        private Argument(string name, string declaration, string member, string expression, bool conditional, string? problem)
        {
            Name = name;
            Declaration = declaration;
            Member = member;
            Expression = expression;
            Conditional = conditional;
            Problem = problem;
        }

        internal string Name { get; }

        internal string Declaration { get; }

        internal string Member { get; }

        internal string Expression { get; }

        internal bool Conditional { get; }

        internal string? Problem { get; }

        internal static Argument For(Block block, string parameter, IReadOnlyDictionary<string, JsonValue> defaults)
        {
            var property = block.PropertyFor(parameter);
            if (property is null)
            {
                return new Argument(
                    parameter,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    false,
                    "the block declares no settable member for it");
            }

            var display = Display(property.Type);

            string parameterType;
            string template;

            switch (Block.ShapeOf(property))
            {
                case Shape.Names:
                    parameterType = "object?";
                    template = "Values.Strings({0})";
                    break;
                case Shape.OneOrMany:
                    parameterType = "object?";
                    template = "Values.Items({0})";
                    break;
                default:
                    if (display == "object")
                    {
                        parameterType = "object?";
                        template = "{0}";
                    }
                    else if (display == "Visibility")
                    {
                        parameterType = "object?";
                        template = "Values.VisibilityOf({0})";
                    }
                    else if (display == "JsonNode")
                    {
                        parameterType = "object?";
                        template = "Values.Node({0})";
                    }
                    else
                    {
                        parameterType = display;
                        template = "{0}";
                    }

                    break;
            }

            var declared = defaults.TryGetValue(parameter, out var node) ? node : null;
            var literal = declared is null ? null : Literal(declared, parameterType);

            // C# can only spell `null` as the default of a reference type or of a value type that
            // admits one, so a default it cannot spell widens the parameter and leaves the member
            // unwritten when nothing was given — which is the Python version's default, because the block's
            // own default is the Python version's.
            var conditional = literal is null;

            if (conditional && parameterType != "object?" && !parameterType.EndsWith("?", StringComparison.Ordinal))
            {
                parameterType += "?";
            }

            // The parameter is spelled the C# way even though the manifest spells it the Python way:
            // `elem_id` arrives as `elemId`, and the manifest key stays the key the config uses.
            var identifier = CSharpName.CamelCase(parameter);

            return new Argument(
                identifier,
                $"{parameterType} {identifier} = {literal ?? "null"}",
                property.Name,
                string.Format(CultureInfo.InvariantCulture, template, conditional ? identifier + "Given" : identifier),
                conditional,
                null);
        }

        /// <summary>Writes the assignment this parameter makes.</summary>
        /// <param name="source">The source being written.</param>
        internal void Write(StringBuilder source)
        {
            if (Conditional)
            {
                // The pattern variable is named after the parameter because a pattern variable
                // declared in an `if` condition is in scope for the rest of the block, and thirty
                // of them called `given` would be thirty declarations of one name.
                var given = Name + "Given";

                source.Append("        if (").Append(Name).Append(" is { } ").Append(given).AppendLine(")");
                source.AppendLine("        {");
                source.Append("            block.").Append(Member).Append(" = ").Append(Expression).AppendLine(";");
                source.AppendLine("        }");
                return;
            }

            source.Append("        block.").Append(Member).Append(" = ").Append(Expression).AppendLine(";");
        }

        /// <summary>The C# spelling of a type, with the namespaces left to the file's usings.</summary>
        private static string Display(ITypeSymbol type) =>
            type.ToDisplayString(new SymbolDisplayFormat(
                globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
                genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
                miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes
                    | SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier));

        /// <summary>Writes a recorded default as a C# literal, or <see langword="null"/> when it cannot be one.</summary>
        private static string? Literal(JsonValue value, string parameterType)
        {
            if (value.IsNull)
            {
                return null;
            }

            switch (parameterType)
            {
                case "string":
                case "string?":
                    return value.AsString() is { } text ? Quote(text) : null;
                case "int":
                case "int?":
                    return value.AsNumber() is { } whole
                        ? ((long)whole).ToString(CultureInfo.InvariantCulture)
                        : null;
                case "double":
                case "double?":
                    return value.AsNumber() is { } number ? number.ToString("R", CultureInfo.InvariantCulture) : null;
                case "bool":
                case "bool?":
                    return value.AsBool() is { } flag ? (flag ? "true" : "false") : null;
                default:
                    // An object-shaped parameter takes only `null` as a default, and a list or a
                    // document is not a constant: those are written when they are given.
                    return null;
            }
        }

        private static string Quote(string text)
        {
            var builder = new StringBuilder(text.Length + 2);
            builder.Append('"');

            foreach (var character in text)
            {
                switch (character)
                {
                    case '\\':
                        builder.Append("\\\\");
                        break;
                    case '"':
                        builder.Append("\\\"");
                        break;
                    case '\n':
                        builder.Append("\\n");
                        break;
                    case '\r':
                        builder.Append("\\r");
                        break;
                    case '\t':
                        builder.Append("\\t");
                        break;
                    default:
                        builder.Append(character);
                        break;
                }
            }

            builder.Append('"');
            return builder.ToString();
        }
    }

    /// <summary>The Python parameter manifest: each block's parameters and their
    /// defaults.</summary>
    private sealed class Manifest
    {
        private readonly Dictionary<string, Entry> _blocks;

        private Manifest(Dictionary<string, Entry> blocks) => _blocks = blocks;

        internal bool TryGet(
            string block,
            out IReadOnlyList<string> parameters,
            out IReadOnlyDictionary<string, JsonValue> defaults)
        {
            if (_blocks.TryGetValue(block, out var entry))
            {
                parameters = entry.Parameters;
                defaults = entry.Defaults;
                return true;
            }

            parameters = Array.Empty<string>();
            defaults = new Dictionary<string, JsonValue>(StringComparer.Ordinal);
            return false;
        }

        internal static bool TryRead(ImmutableArray<string?> manifests, out Manifest manifest, out string problem)
        {
            var blocks = new Dictionary<string, Entry>(StringComparer.Ordinal);
            manifest = new Manifest(blocks);
            problem = string.Empty;

            var supplied = manifests.Where(text => text is not null).ToArray();
            if (supplied.Length == 0)
            {
                problem = "no manifest was supplied to this build";
                return false;
            }

            foreach (var text in supplied)
            {
                try
                {
                    var document = JsonValue.Parse(text!);
                    if (document.Member("blocks")?.Object is not { } declared)
                    {
                        problem = "the manifest declares no 'blocks'";
                        return false;
                    }

                    foreach (var block in declared)
                    {
                        if (block.Value.Member("params")?.Array is not { } items)
                        {
                            continue;
                        }

                        var defaults = new Dictionary<string, JsonValue>(StringComparer.Ordinal);
                        if (block.Value.Member("defaults")?.Object is { } recorded)
                        {
                            foreach (var pair in recorded)
                            {
                                defaults[pair.Key] = pair.Value;
                            }
                        }

                        blocks[block.Key] = new Entry(
                            items.Select(item => item.AsString() ?? string.Empty).ToArray(),
                            defaults);
                    }
                }
                catch (FormatException error)
                {
                    problem = error.Message;
                    return false;
                }
            }

            return true;
        }

        private sealed class Entry(IReadOnlyList<string> parameters, IReadOnlyDictionary<string, JsonValue> defaults)
        {
            internal IReadOnlyList<string> Parameters { get; } = parameters;

            internal IReadOnlyDictionary<string, JsonValue> Defaults { get; } = defaults;
        }
    }

    /// <summary>The names the Python version exports at its top level, and what each of them
    /// is.</summary>
    private sealed class Exports
    {
        internal static bool TryRead(
            ImmutableArray<string?> manifests,
            out IReadOnlyDictionary<string, string> names,
            out string problem)
        {
            names = new Dictionary<string, string>(StringComparer.Ordinal);
            problem = string.Empty;

            var supplied = manifests.Where(text => text is not null).ToArray();
            if (supplied.Length == 0)
            {
                problem = "no export declaration was supplied to this build";
                return false;
            }

            try
            {
                if (JsonValue.Parse(supplied[0]!).Member("exports")?.Object is not { } exports)
                {
                    problem = "the declaration lists no 'exports'";
                    return false;
                }

                var read = new Dictionary<string, string>(StringComparer.Ordinal);
                foreach (var export in exports)
                {
                    read[export.Key] = export.Value.Member("kind")?.AsString() ?? string.Empty;
                }

                names = read;
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

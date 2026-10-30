using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Gradio.Net.Analyzers.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Gradio.Net.Analyzers;

/// <summary>
/// Generates each block's parameter mapping from the recorded Python manifest, and fails the
/// build when the Python version declares a parameter the block does not.
/// </summary>
/// <remarks>
/// <para>
/// The mapping from a component's members to the keys of its config props is generated rather than
/// written by hand, because the list of keys is the Python version's, not this project's. The
/// manifest is read from the pinned release, so a parameter the Python version adds arrives in the
/// manifest the next time it is recorded, and a build that has not declared a member for it stops
/// rather than serving a config that is quietly missing a key.
/// </para>
/// <para>
/// The manifest is supplied as an additional file. A project that declares no block — the test
/// project, for instance — gets nothing from this generator and is not asked for the file.
/// </para>
/// </remarks>
[Generator(LanguageNames.CSharp)]
public sealed class ComponentPropsGenerator : IIncrementalGenerator
{
    /// <summary>The diagnostic reported when a block does not declare a parameter the Python
    /// version has.</summary>
    public const string MissingParameterDiagnosticId = "GRADIO0004";

    /// <summary>The diagnostic reported when a block is not a block the manifest knows.</summary>
    public const string UnknownBlockDiagnosticId = "GRADIO0005";

    /// <summary>The diagnostic reported when the manifest is not readable.</summary>
    public const string UnreadableManifestDiagnosticId = "GRADIO0006";

    private const string AttributeName = "Gradio.Net.GradioBlockAttribute";
    private const string OmitAttributeName = "Gradio.Net.OmitFromConfigAttribute";
    private const string ParameterNameAttributeName = "Gradio.Net.UpstreamParameterAttribute";
    private const string ManifestSuffix = "component-params.json";
    private const string RenderParameter = "render";

    private const string Category = "Gradio.Architecture";

    private static readonly DiagnosticDescriptor _missingParameter = new(
        MissingParameterDiagnosticId,
        "A block does not declare a Python parameter",
        "'{0}' does not declare '{1}', which the Python version declares for the block '{2}'; add the member, "
        + "or the parameter disappears from the config",
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Every parameter the Python version declares for a block has to be declared here, so that a parameter the Python version adds fails the build instead of silently missing from the config.");

    private static readonly DiagnosticDescriptor _unknownBlock = new(
        UnknownBlockDiagnosticId,
        "A block is not one the manifest knows",
        "'{0}' declares itself the block '{1}', which the manifest does not have; the manifest is "
        + "recorded from the Python release, so either the name is wrong or the block is not the Python version's",
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "A block name that the manifest does not have cannot be checked against the Python version, which makes the check meaningless for that block.");

    private static readonly DiagnosticDescriptor _unreadableManifest = new(
        UnreadableManifestDiagnosticId,
        "The component parameter manifest is not readable",
        "The component parameter manifest could not be read ({0}); the parameter mapping cannot be "
        + "generated from it",
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
                predicate: static (node, _) => node is Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax,
                transform: static (attribute, _) => BlockDeclaration.From(attribute))
            .Where(static declaration => declaration is not null)
            .Select(static (declaration, _) => declaration!);

        context.RegisterSourceOutput(
            blocks.Combine(manifests),
            static (production, pair) => Emit(production, pair.Left, pair.Right));
    }

    private static void Emit(
        SourceProductionContext production,
        BlockDeclaration declaration,
        ImmutableArray<string?> manifests)
    {
        var manifest = Manifest.Read(manifests);
        if (manifest.Error is not null)
        {
            production.ReportDiagnostic(Diagnostic.Create(
                _unreadableManifest,
                declaration.Location,
                manifest.Error));

            return;
        }

        if (!manifest.HasDocument)
        {
            production.ReportDiagnostic(Diagnostic.Create(
                _unreadableManifest,
                declaration.Location,
                $"no '{ManifestSuffix}' was supplied to this build"));

            return;
        }

        if (!manifest.Parameters.TryGetValue(declaration.Block, out var parameters))
        {
            production.ReportDiagnostic(Diagnostic.Create(
                _unknownBlock,
                declaration.Location,
                declaration.Name,
                declaration.Block));

            return;
        }

        // A block whose constructor takes **kwargs has no fixed parameter list to compare against.
        if (parameters is null)
        {
            return;
        }

        var missing = parameters
            .Where(parameter => declaration.MemberFor(parameter) is null)
            .ToArray();

        if (missing.Length > 0)
        {
            foreach (var parameter in missing)
            {
                production.ReportDiagnostic(Diagnostic.Create(
                    _missingParameter,
                    declaration.Location,
                    declaration.Name,
                    parameter,
                    declaration.Block));
            }

            return;
        }

        production.AddSource(
            $"{declaration.Name}.GradioProps.g.cs",
            SourceText.From(BuildSource(declaration, parameters), Encoding.UTF8));
    }

    private static string BuildSource(BlockDeclaration declaration, IReadOnlyList<string> parameters)
    {
        var source = new StringBuilder();
        source.AppendLine("// <auto-generated />");
        source.AppendLine("// Generated from the Python parameter manifest; do not edit.");
        source.AppendLine("#nullable enable");
        source.AppendLine();
        source.Append("namespace ").Append(declaration.Namespace).AppendLine(";");
        source.AppendLine();
        source.Append(declaration.Accessibility).Append(" partial class ").AppendLine(declaration.Name);
        source.AppendLine("{");
        source.AppendLine("    /// <inheritdoc />");
        source.AppendLine("    protected internal override void WriteProps(global::Gradio.Net.PropWriter props)");
        source.AppendLine("    {");

        foreach (var parameter in parameters)
        {
            // The Python version removes `render` from the props before it serves them: it decides whether a
            // block is part of the tree at all rather than being a prop of one.
            if (string.Equals(parameter, RenderParameter, StringComparison.Ordinal))
            {
                continue;
            }

            // A block that removes a parameter in its own `get_config` says so on the member, which
            // is where a reviewer reading the parameter list finds it.
            var member = declaration.MemberFor(parameter)!;

            if (declaration.Omits(member))
            {
                continue;
            }

            source.Append("        props.Add(\"").Append(parameter).Append("\", this.")
                .Append(member).AppendLine(");");
        }

        source.AppendLine("    }");
        source.AppendLine("}");

        return source.ToString();
    }

    /// <summary>One attributed class, reduced to what the generator needs from it.</summary>
    private sealed class BlockDeclaration
    {
        private readonly INamedTypeSymbol _type;

        private BlockDeclaration(INamedTypeSymbol type, string block, string name, string @namespace, string accessibility)
        {
            _type = type;
            Block = block;
            Name = name;
            Namespace = @namespace;
            Accessibility = accessibility;
            Location = type.Locations.Length > 0 ? type.Locations[0] : Location.None;
        }

        internal string Block { get; }

        internal string Name { get; }

        internal string Namespace { get; }

        internal string Accessibility { get; }

        internal Location Location { get; }

        internal static BlockDeclaration? From(GeneratorAttributeSyntaxContext attribute)
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

            return new BlockDeclaration(type, block, type.Name, @namespace, AccessibilityOf(type));
        }

        /// <summary>
        /// The member a parameter is written from: the one named after it, or one that says it
        /// stands for it.
        /// </summary>
        /// <param name="parameter">The parameter's name, as the Python version spells it.</param>
        /// <returns>The member's name, or <see langword="null"/> when the block declares
        /// neither.</returns>
        /// <remarks>
        /// The whole base chain is searched, because the parameters a component shares with every
        /// other component are declared once on the base they share it with. A member may be named
        /// differently from its parameter when the conventional name is taken by the kernel — see
        /// <c>UpstreamParameterAttribute</c> — and the attribute is what keeps that from being a
        /// parameter the build silently forgets.
        /// </remarks>
        internal string? MemberFor(string parameter)
        {
            // A member that says which parameter it stands for is asked first: the conventional
            // name may be taken by a member of the kernel, and the parameter it stands for is not
            // that member.
            for (var current = _type; current is not null; current = current.BaseType)
            {
                foreach (var member in current.GetMembers())
                {
                    if (!IsValueMember(member))
                    {
                        continue;
                    }

                    foreach (var attribute in member.GetAttributes())
                    {
                        if (attribute.AttributeClass?.ToDisplayString() == ParameterNameAttributeName
                            && attribute.ConstructorArguments.Length == 1
                            && attribute.ConstructorArguments[0].Value is string declared
                            && string.Equals(declared, parameter, StringComparison.Ordinal))
                        {
                            return member.Name;
                        }
                    }
                }
            }

            var conventional = CSharpName.PascalCase(parameter);

            for (var current = _type; current is not null; current = current.BaseType)
            {
                foreach (var member in current.GetMembers(conventional))
                {
                    if (IsValueMember(member))
                    {
                        return conventional;
                    }
                }
            }

            return null;
        }

        /// <summary>Whether a member holds a value a parameter can be written from.</summary>
        private static bool IsValueMember(ISymbol member) =>
            !member.IsStatic && member is IPropertySymbol { IsIndexer: false } or IFieldSymbol;

        /// <summary>Whether a member says it is not part of the config.</summary>
        /// <param name="name">The member's name.</param>
        /// <returns>Whether the member carries <c>OmitFromConfig</c>.</returns>
        /// <remarks>
        /// This is how a block reproduces a parameter the Python version deletes from its own
        /// props: the member exists — the parameter list is checked against the Python version in
        /// full — and says it is not written.
        /// </remarks>
        internal bool Omits(string name)
        {
            for (var current = _type; current is not null; current = current.BaseType)
            {
                foreach (var member in current.GetMembers(name))
                {
                    foreach (var attribute in member.GetAttributes())
                    {
                        if (attribute.AttributeClass?.ToDisplayString() == OmitAttributeName)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static string AccessibilityOf(INamedTypeSymbol type) => type.DeclaredAccessibility switch
        {
            Microsoft.CodeAnalysis.Accessibility.Public => "public",
            Microsoft.CodeAnalysis.Accessibility.Internal => "internal",
            Microsoft.CodeAnalysis.Accessibility.Protected => "protected",
            Microsoft.CodeAnalysis.Accessibility.Private => "private",
            _ => "internal",
        };
    }

    /// <summary>The Python parameter manifest, or why it could not be read.</summary>
    private sealed class Manifest
    {
        private Manifest(bool hasDocument, Dictionary<string, IReadOnlyList<string>?> parameters, string? error)
        {
            HasDocument = hasDocument;
            Parameters = parameters;
            Error = error;
        }

        internal bool HasDocument { get; }

        /// <summary>Each block's parameters; the value is null for a block with an open parameter list.</summary>
        internal Dictionary<string, IReadOnlyList<string>?> Parameters { get; }

        internal string? Error { get; }

        internal static Manifest Read(ImmutableArray<string?> manifests)
        {
            var supplied = manifests.Where(text => text is not null).ToArray();
            if (supplied.Length == 0)
            {
                return new Manifest(hasDocument: false, new Dictionary<string, IReadOnlyList<string>?>(StringComparer.Ordinal), error: null);
            }

            var parameters = new Dictionary<string, IReadOnlyList<string>?>(StringComparer.Ordinal);

            foreach (var text in supplied)
            {
                try
                {
                    var document = JsonValue.Parse(text!);
                    if (document.Member("blocks")?.Object is not { } blocks)
                    {
                        return new Manifest(hasDocument: false, parameters, "the manifest declares no 'blocks'");
                    }

                    foreach (var block in blocks)
                    {
                        var declared = block.Value.Member("params");
                        if (declared is null || declared.IsNull)
                        {
                            // An open-ended constructor: there is no fixed list to compare against.
                            parameters[block.Key] = null;
                            continue;
                        }

                        if (declared.Array is not { } items)
                        {
                            return new Manifest(
                                hasDocument: true,
                                parameters,
                                $"'{block.Key}' declares a 'params' that is not an array");
                        }

                        // A block whose constructor ends in `**kwargs` records the parameters before
                        // it and says the list is not the whole of it. The list is still what a
                        // factory's signature is written from, but a props mapping written from it
                        // would be a mapping that silently drops whatever the kwargs carry.
                        if (block.Value.Member("openEnded") is { } openEnded && !openEnded.IsNull
                            && openEnded.AsBool() == true)
                        {
                            parameters[block.Key] = null;
                            continue;
                        }

                        parameters[block.Key] = items.Select(item => item.AsString() ?? string.Empty).ToArray();
                    }
                }
                catch (FormatException error)
                {
                    return new Manifest(hasDocument: true, parameters, error.Message);
                }
            }

            return new Manifest(hasDocument: true, parameters, error: null);
        }
    }
}

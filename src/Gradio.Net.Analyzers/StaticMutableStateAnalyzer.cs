using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Gradio.Net.Analyzers;

/// <summary>
/// Reports static mutable state: static fields that are neither <c>const</c> nor
/// <c>readonly</c>, static properties with a <c>set</c> accessor, and static events.
/// </summary>
/// <remarks>
/// <para>
/// The framework must not keep static mutable state: it stops two hosts from coexisting
/// in one process, breaks concurrent rendering, and cannot be substituted in a test
///.
/// </para>
/// <para>
/// This analyzer inspects declarations, so it does not catch a <c>static readonly</c>
/// reference to a mutable object (for example a list that is appended to). It is a
/// guardrail against the common shape, not a proof of immutability.
/// </para>
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class StaticMutableStateAnalyzer : DiagnosticAnalyzer
{
    /// <summary>The identifier of the diagnostic this analyzer reports.</summary>
    public const string DiagnosticId = "GRADIO0001";

    private const string Category = "Gradio.Architecture";

    private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(
        DiagnosticId,
        "Static mutable state is prohibited",
        "Static mutable state '{0}' is prohibited; take state from dependency injection instead",
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Static mutable state prevents two hosts from coexisting, breaks concurrent rendering and cannot be substituted in tests.");

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeField, SyntaxKind.FieldDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeProperty, SyntaxKind.PropertyDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeEventField, SyntaxKind.EventFieldDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeEvent, SyntaxKind.EventDeclaration);
    }

    private static void AnalyzeField(SyntaxNodeAnalysisContext context)
    {
        var declaration = (FieldDeclarationSyntax)context.Node;
        if (!IsStaticMutable(declaration.Modifiers))
        {
            return;
        }

        foreach (var variable in declaration.Declaration.Variables)
        {
            Report(context, variable.Identifier);
        }
    }

    private static void AnalyzeProperty(SyntaxNodeAnalysisContext context)
    {
        var declaration = (PropertyDeclarationSyntax)context.Node;
        if (!IsStatic(declaration.Modifiers))
        {
            return;
        }

        // An `init` accessor is not mutable state: it can only run during initialization.
        var hasMutableSetter = declaration.AccessorList is { } accessors
            && accessors.Accessors.Any(accessor => accessor.IsKind(SyntaxKind.SetAccessorDeclaration));

        if (hasMutableSetter)
        {
            Report(context, declaration.Identifier);
        }
    }

    private static void AnalyzeEventField(SyntaxNodeAnalysisContext context)
    {
        var declaration = (EventFieldDeclarationSyntax)context.Node;
        if (!IsStatic(declaration.Modifiers))
        {
            return;
        }

        foreach (var variable in declaration.Declaration.Variables)
        {
            Report(context, variable.Identifier);
        }
    }

    private static void AnalyzeEvent(SyntaxNodeAnalysisContext context)
    {
        var declaration = (EventDeclarationSyntax)context.Node;
        if (IsStatic(declaration.Modifiers))
        {
            Report(context, declaration.Identifier);
        }
    }

    private static bool IsStatic(SyntaxTokenList modifiers) =>
        modifiers.Any(modifier => modifier.IsKind(SyntaxKind.StaticKeyword));

    private static bool IsStaticMutable(SyntaxTokenList modifiers) =>
        IsStatic(modifiers)
        && !modifiers.Any(modifier => modifier.IsKind(SyntaxKind.ConstKeyword))
        && !modifiers.Any(modifier => modifier.IsKind(SyntaxKind.ReadOnlyKeyword));

    private static void Report(SyntaxNodeAnalysisContext context, SyntaxToken identifier) =>
        context.ReportDiagnostic(Diagnostic.Create(_rule, identifier.GetLocation(), identifier.ValueText));
}

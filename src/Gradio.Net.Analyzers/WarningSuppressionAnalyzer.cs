using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Gradio.Net.Analyzers;

/// <summary>
/// Reports the two directive forms that switch warnings off instead of fixing them:
/// <c>#pragma warning disable</c> and <c>#nullable disable</c>.
/// </summary>
/// <remarks>
/// The baseline is "warnings are errors". A pragma would let a change pass
/// the build while hiding the problem, so the only accepted ways to silence a warning
/// are fixing it or changing the rule's severity in <c>.editorconfig</c>, where the
/// change is visible to everyone.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class WarningSuppressionAnalyzer : DiagnosticAnalyzer
{
    /// <summary>The identifier of the diagnostic reported for <c>#pragma warning disable</c>.</summary>
    public const string PragmaDiagnosticId = "GRADIO0002";

    /// <summary>The identifier of the diagnostic reported for <c>#nullable disable</c>.</summary>
    public const string NullableDiagnosticId = "GRADIO0003";

    private const string Category = "Gradio.Architecture";

    private static readonly DiagnosticDescriptor _pragmaRule = new DiagnosticDescriptor(
        PragmaDiagnosticId,
        "#pragma warning disable is prohibited",
        "'#pragma warning disable' is prohibited; fix the warning or change the rule severity in .editorconfig",
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Warnings are errors in this repository; suppressing them at the call site hides the problem from everyone else.");

    private static readonly DiagnosticDescriptor _nullableRule = new DiagnosticDescriptor(
        NullableDiagnosticId,
        "#nullable disable is prohibited",
        "'#nullable disable' is prohibited; nullability is analysed for the whole assembly",
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Nullable analysis is enabled repo-wide; switching it off per file re-introduces the warnings the baseline removed.");

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(_pragmaRule, _nullableRule);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxTreeAction(AnalyzeTree);
    }

    private static void AnalyzeTree(SyntaxTreeAnalysisContext context)
    {
        var root = context.Tree.GetRoot(context.CancellationToken);

        foreach (var trivia in root.DescendantTrivia(descendIntoTrivia: true))
        {
            switch (trivia.GetStructure())
            {
                case PragmaWarningDirectiveTriviaSyntax pragma
                    when pragma.DisableOrRestoreKeyword.IsKind(SyntaxKind.DisableKeyword):
                    context.ReportDiagnostic(Diagnostic.Create(_pragmaRule, pragma.GetLocation()));
                    break;

                case NullableDirectiveTriviaSyntax nullable
                    when nullable.SettingToken.IsKind(SyntaxKind.DisableKeyword):
                    context.ReportDiagnostic(Diagnostic.Create(_nullableRule, nullable.GetLocation()));
                    break;

                default:
                    break;
            }
        }
    }
}

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Gradio.Net.Tests.Analyzers;

/// <summary>
/// Compiles a snippet in memory and returns what the given analyzers report for it.
/// </summary>
/// <remarks>
/// The analyzer is the seam here: driving it with a compilation is exactly what the
/// compiler does during a real build, so an Error-severity diagnostic reported here is
/// what turns into a failed build.
/// </remarks>
internal static class AnalyzerTestHost
{
    private static readonly ImmutableArray<MetadataReference> _references = CreateReferences();

    internal static async Task<ImmutableArray<Diagnostic>> AnalyzeAsync(
        string source,
        params DiagnosticAnalyzer[] analyzers)
    {
        var compilation = CSharpCompilation.Create(
            "GradioNetAnalyzerProbe",
            [CSharpSyntaxTree.ParseText(source)],
            _references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));

        return await compilation
            .WithAnalyzers(analyzers.ToImmutableArray())
            .GetAnalyzerDiagnosticsAsync();
    }

    private static ImmutableArray<MetadataReference> CreateReferences()
    {
        var trustedPlatformAssemblies = (string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")
            ?? throw new InvalidOperationException("The test host does not expose TRUSTED_PLATFORM_ASSEMBLIES.");

        return trustedPlatformAssemblies
            .Split(System.IO.Path.PathSeparator)
            .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))
            .ToImmutableArray();
    }
}

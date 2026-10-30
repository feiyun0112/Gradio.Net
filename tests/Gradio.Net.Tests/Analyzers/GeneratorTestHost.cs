using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Gradio.Net.Tests.Analyzers;

/// <summary>
/// Compiles a snippet in memory and runs a source generator over it, the way the compiler does.
/// </summary>
/// <remarks>
/// Driving the generator with a compilation is what a real build does, so an Error-severity
/// diagnostic reported here is what turns into a failed build, and the generated text here is what
/// the component's parameter mapping is at build time.
/// </remarks>
internal static class GeneratorTestHost
{
    private static readonly ImmutableArray<MetadataReference> _references = CreateReferences();

    internal static GeneratorRun Run(
        string source,
        string? manifest,
        ISourceGenerator generator,
        string manifestName = "component-params.json")
    {
        var compilation = CSharpCompilation.Create(
            "GradioNetGeneratorProbe",
            [CSharpSyntaxTree.ParseText(source)],
            _references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));

        var additionalTexts = manifest is null
            ? []
            : new[] { (AdditionalText)new InMemoryAdditionalText(manifestName, manifest) };

        var driver = CSharpGeneratorDriver.Create(
            [generator],
            additionalTexts,
            parseOptions: (CSharpParseOptions)compilation.SyntaxTrees.First().Options);

        var result = driver.RunGeneratorsAndUpdateCompilation(compilation, out _, out _);
        var runResult = result.GetRunResult();

        // The run result aggregates what each generator reported; the driver's own output carries
        // the same diagnostics, so reading one of the two keeps a report from naming each twice.
        return new GeneratorRun(
            runResult.Diagnostics,
            runResult.Results
                .SelectMany(static result => result.GeneratedSources)
                .Select(static source => (source.HintName, source.SourceText.ToString()))
                .ToArray());
    }

    private static ImmutableArray<MetadataReference> CreateReferences()
    {
        var trustedPlatformAssemblies = (string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")
            ?? throw new InvalidOperationException("The test host does not expose TRUSTED_PLATFORM_ASSEMBLIES.");

        return trustedPlatformAssemblies
            .Split(Path.PathSeparator)
            .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))
            .ToImmutableArray();
    }

    private sealed class InMemoryAdditionalText : AdditionalText
    {
        private readonly SourceText _text;

        internal InMemoryAdditionalText(string path, string text)
        {
            Path = path;
            _text = SourceText.From(text, Encoding.UTF8);
        }

        public override string Path { get; }

        public override SourceText GetText(CancellationToken cancellationToken = default) => _text;
    }
}

/// <summary>What a generator run reported and produced.</summary>
/// <param name="Diagnostics">The diagnostics, including the driver's own.</param>
/// <param name="Sources">The generated sources, by hint name.</param>
internal sealed record GeneratorRun(
    ImmutableArray<Diagnostic> Diagnostics,
    IReadOnlyList<(string HintName, string Text)> Sources)
{
    internal IReadOnlyList<Diagnostic> WithId(string id) =>
        Diagnostics.Where(diagnostic => diagnostic.Id == id).ToArray();

    internal string SourceNamed(string hintName) =>
        Sources.Single(source => source.HintName == hintName).Text;
}

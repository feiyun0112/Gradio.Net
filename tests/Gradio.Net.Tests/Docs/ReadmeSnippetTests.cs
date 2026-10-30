using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Gradio.Net.Tests.Docs;

/// <summary>
/// The README's examples compile, as a reader's project would.
/// </summary>
/// <remarks>
/// <para>
/// A README snippet is the first code anybody copies, and the way it fails is silent: it is not part
/// of the solution, so nothing builds it, and a renamed method leaves a block that looks right and
/// does not compile. This file was written after exactly that — three of the five blocks named an
/// extension method in a namespace the example did not import, and a `Blocks` that needed one it did
/// not have.
/// </para>
/// <para>
/// The compilation is given the implicit usings a <c>dotnet new web</c> project gets, because that is
/// the project the examples are written for: a snippet may use <c>WebApplication</c> without an
/// import and still be correct. Everything else is the real package, so a member that does not exist
/// is the error a reader would see.
/// </para>
/// </remarks>
public sealed partial class ReadmeSnippetTests
{
    /// <summary>
    /// The imports the Web SDK adds to every file, which a snippet is entitled to rely on.
    /// </summary>
    private const string ImplicitUsings = """
        global using global::System;
        global using global::System.Collections.Generic;
        global using global::System.IO;
        global using global::System.Linq;
        global using global::System.Net.Http;
        global using global::System.Net.Http.Json;
        global using global::System.Threading;
        global using global::System.Threading.Tasks;
        global using global::Microsoft.AspNetCore.Builder;
        global using global::Microsoft.AspNetCore.Hosting;
        global using global::Microsoft.AspNetCore.Http;
        global using global::Microsoft.AspNetCore.Routing;
        global using global::Microsoft.Extensions.Configuration;
        global using global::Microsoft.Extensions.DependencyInjection;
        global using global::Microsoft.Extensions.Hosting;
        global using global::Microsoft.Extensions.Logging;
        """;

    [Fact]
    public void Every_csharp_example_in_the_readme_compiles()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var readme = System.IO.File.ReadAllText(Path.Combine(RepositoryRoot.FullPath, "README.md"));

        var examples = CSharpBlock()
            .Matches(readme)
            .Select(match => match.Groups["code"].Value)
            .ToList();

        // The control: a README that lost its examples would pass this test without checking
        // anything, and the quick start is four blocks long on its own.
        Assert.True(examples.Count >= 4, $"The README holds {examples.Count} C# example(s); the quick start alone has more.");

        var broken = new List<string>();

        for (var index = 0; index < examples.Count; index++)
        {
            var errors = Compile(examples[index], cancellationToken)
                .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
                .Select(diagnostic => diagnostic.ToString())
                .ToList();

            if (errors.Count > 0)
            {
                var firstLine = examples[index].Split('\n')[0].Trim();

                broken.Add($"example {index + 1} ('{firstLine}'): {string.Join(" | ", errors)}");
            }
        }

        Assert.True(
            broken.Count == 0,
            $"These README examples do not compile against this package: {string.Join("; ", broken)}.");
    }

    /// <summary>Compiles one snippet the way a <c>dotnet new web</c> project would.</summary>
    /// <param name="source">The snippet.</param>
    /// <param name="cancellationToken">Cancels the compile.</param>
    /// <returns>What the compiler said.</returns>
    private static ImmutableArray<Diagnostic> Compile(string source, CancellationToken cancellationToken) =>
        CSharpCompilation
            .Create(
                "ReadmeExample",
                [
                    CSharpSyntaxTree.ParseText(ImplicitUsings, cancellationToken: cancellationToken),
                    CSharpSyntaxTree.ParseText(source, cancellationToken: cancellationToken),
                ],
                References(),
                new CSharpCompilationOptions(
                    OutputKind.ConsoleApplication,
                    nullableContextOptions: NullableContextOptions.Enable))
            .GetDiagnostics(cancellationToken);

    /// <summary>The references a project that uses this package has.</summary>
    /// <returns>The platform's assemblies, and this package's.</returns>
    private static ImmutableArray<MetadataReference> References()
    {
        var trusted = (string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")
            ?? throw new InvalidOperationException("The test host does not expose TRUSTED_PLATFORM_ASSEMBLIES.");

        return
        [
            .. trusted
                .Split(Path.PathSeparator)
                .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path)),
            MetadataReference.CreateFromFile(typeof(Gradio.Net.gr).Assembly.Location),
        ];
    }

    /// <summary>A fenced C# block in the README.</summary>
    /// <remarks>
    /// The line ending is either, because the repository stores LF and a Windows checkout hands the
    /// file over with CRLF — and a pattern that assumed one of them would find no example at all.
    /// </remarks>
    [GeneratedRegex(@"```csharp\r?\n(?<code>.*?)```", RegexOptions.Singleline | RegexOptions.CultureInvariant)]
    private static partial Regex CSharpBlock();
}

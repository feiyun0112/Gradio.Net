using Gradio.Net;
using Gradio.Net.Analyzers;
using Microsoft.CodeAnalysis;
using Xunit;

namespace Gradio.Net.Tests.Analyzers;

/// <summary>
/// Covers GRADIO0002 and GRADIO0003: warnings must be fixed, not switched off.
/// </summary>
public sealed class WarningSuppressionAnalyzerTests
{
    private static readonly WarningSuppressionAnalyzer _analyzer = new();

    [Theory]
    [InlineData("class C { }\n#pragma warning disable CS1591\nclass D { }")]
    [InlineData("class C { }\n#pragma warning disable\nclass D { }")]
    [InlineData("class C { }\n#pragma warning disable CS1591, CS8618\nclass D { }")]
    public async Task Reports_pragma_warning_disable_as_an_error(string source)
    {
        var diagnostics = await AnalyzerTestHost.AnalyzeAsync(source, _analyzer);

        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal(WarningSuppressionAnalyzer.PragmaDiagnosticId, diagnostic.Id);
        Assert.Equal(DiagnosticSeverity.Error, diagnostic.Severity);
    }

    [Theory]
    [InlineData("#nullable disable\nclass C { }")]
    [InlineData("class C { }\n#nullable disable warnings\nclass D { }")]
    public async Task Reports_nullable_disable_as_an_error(string source)
    {
        var diagnostics = await AnalyzerTestHost.AnalyzeAsync(source, _analyzer);

        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal(WarningSuppressionAnalyzer.NullableDiagnosticId, diagnostic.Id);
        Assert.Equal(DiagnosticSeverity.Error, diagnostic.Severity);
    }

    [Theory]
    [InlineData("class C { }")]
    [InlineData("class C { }\n#pragma warning restore CS1591\nclass D { }")]
    [InlineData("#nullable enable\nclass C { }")]
    [InlineData("#nullable restore\nclass C { }")]
    public async Task Accepts_sources_that_do_not_suppress_warnings(string source)
    {
        Assert.Empty(await AnalyzerTestHost.AnalyzeAsync(source, _analyzer));
    }
}

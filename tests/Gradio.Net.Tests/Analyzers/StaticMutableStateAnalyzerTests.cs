using System.Globalization;
using Gradio.Net;
using Gradio.Net.Analyzers;
using Microsoft.CodeAnalysis;
using Xunit;

namespace Gradio.Net.Tests.Analyzers;

/// <summary>
/// Covers GRADIO0001: static mutable state must fail the build.
/// </summary>
public sealed class StaticMutableStateAnalyzerTests
{
    private static readonly StaticMutableStateAnalyzer _analyzer = new();

    [Theory]
    [InlineData("class C { static int _counter; }")]
    [InlineData("class C { public static string Name = \"x\"; }")]
    [InlineData("class C { static int Counter { get; set; } }")]
    [InlineData("class C { static event System.EventHandler? Changed; }")]
    [InlineData("class C { static event System.EventHandler? Changed { add { } remove { } } }")]
    public async Task Reports_static_mutable_state_as_an_error(string source)
    {
        var diagnostics = await AnalyzerTestHost.AnalyzeAsync(source, _analyzer);

        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal(StaticMutableStateAnalyzer.DiagnosticId, diagnostic.Id);
        Assert.Equal(DiagnosticSeverity.Error, diagnostic.Severity);
    }

    [Fact]
    public async Task Names_the_offending_member()
    {
        var diagnostics = await AnalyzerTestHost.AnalyzeAsync("class C { static int _counter; }", _analyzer);

        var diagnostic = Assert.Single(diagnostics);
        Assert.Contains("_counter", diagnostic.GetMessage(CultureInfo.InvariantCulture), StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("class C { static readonly int[] Values = new int[0]; }")]
    [InlineData("class C { private const string Name = \"x\"; }")]
    [InlineData("class C { const int Limit = 3; }")]
    [InlineData("class C { int Value; }")]
    [InlineData("class C { static int Value => 1; }")]
    [InlineData("class C { static int Value { get; } = 1; }")]
    [InlineData("class C { static int Value { get; init; } }")]
    [InlineData("class C { event System.EventHandler? Changed; }")]
    public async Task Accepts_immutable_and_instance_state(string source)
    {
        Assert.Empty(await AnalyzerTestHost.AnalyzeAsync(source, _analyzer));
    }
}

using System.Globalization;
using Gradio.Net;
using Gradio.Net.Analyzers;
using Microsoft.CodeAnalysis;
using Xunit;

namespace Gradio.Net.Tests.Analyzers;

/// <summary>
/// The component parameter mapping is generated from the Python manifest, and a Python
/// parameter this build does not declare stops the build.
/// </summary>
/// <remarks>
/// The seam is the generator itself, driven with a compilation: that is what the compiler does
/// during a real build, so an Error-severity diagnostic reported here is a build failure and the
/// generated text is the mapping the config is built from.
/// </remarks>
public sealed class ComponentPropsGeneratorTests
{
    [Fact]
    public void The_mapping_is_generated_in_upstream_parameter_order()
    {
        var run = Run(Declaration("textbox", "Value", "Lines", "Render"), Manifest("textbox", "value", "lines", "render"));

        Assert.Empty(run.Diagnostics);
        var generated = run.SourceNamed("Textbox.GradioProps.g.cs");

        Assert.Contains("props.Add(\"value\", this.Value);", generated, StringComparison.Ordinal);
        Assert.Contains("props.Add(\"lines\", this.Lines);", generated, StringComparison.Ordinal);

        // render is the Python version's own bookkeeping and is removed from the props before they are
        // served, so the mapping writes it nowhere.
        Assert.DoesNotContain("\"render\"", generated, StringComparison.Ordinal);

        Assert.True(
            generated.IndexOf("props.Add(\"value\"", StringComparison.Ordinal)
            < generated.IndexOf("props.Add(\"lines\"", StringComparison.Ordinal),
            "the mapping has to follow the Python version's parameter order.");
    }

    [Fact]
    public void An_upstream_parameter_the_block_does_not_declare_is_an_error()
    {
        var run = Run(
            Declaration("textbox", "Value"),
            Manifest("textbox", "value", "placeholder"));

        var error = Assert.Single(run.WithId(ComponentPropsGenerator.MissingParameterDiagnosticId));

        Assert.Equal(DiagnosticSeverity.Error, error.Severity);
        Assert.Contains("placeholder", error.GetMessage(CultureInfo.InvariantCulture), StringComparison.Ordinal);
        Assert.Contains("textbox", error.GetMessage(CultureInfo.InvariantCulture), StringComparison.Ordinal);
        Assert.Empty(run.Sources);
    }

    [Fact]
    public void A_parameter_declared_on_a_base_type_counts_as_declared()
    {
        var run = Run(
            """
            namespace Gradio.Net
            {
                internal sealed class GradioBlockAttribute(string block) : System.Attribute { public string Block { get; } = block; }
                public abstract class Block { protected internal virtual void WriteProps(PropWriter props) { } public bool Visible { get; set; } }
                public sealed class PropWriter { public void Add(string name, object? value) { } }
            }

            namespace Gradio.Net
            {
                [GradioBlock("textbox")]
                internal sealed partial class Textbox : Block
                {
                }
            }
            """,
            Manifest("textbox", "visible"));

        Assert.Empty(run.Diagnostics);
        Assert.Contains("props.Add(\"visible\", this.Visible);", run.SourceNamed("Textbox.GradioProps.g.cs"), StringComparison.Ordinal);
    }

    [Fact]
    public void A_block_the_manifest_does_not_have_is_an_error()
    {
        var run = Run(Declaration("textbox", "Value"), Manifest("number", "value"));

        var error = Assert.Single(run.WithId(ComponentPropsGenerator.UnknownBlockDiagnosticId));

        Assert.Equal(DiagnosticSeverity.Error, error.Severity);
        Assert.Contains("textbox", error.GetMessage(CultureInfo.InvariantCulture), StringComparison.Ordinal);
    }

    [Fact]
    public void A_build_with_no_manifest_is_an_error()
    {
        var run = Run(Declaration("textbox", "Value"), manifest: null);

        var error = Assert.Single(run.WithId(ComponentPropsGenerator.UnreadableManifestDiagnosticId));

        Assert.Equal(DiagnosticSeverity.Error, error.Severity);
        Assert.Contains("component-params.json", error.GetMessage(CultureInfo.InvariantCulture), StringComparison.Ordinal);
    }

    [Fact]
    public void A_manifest_that_is_not_json_is_an_error_rather_than_a_silent_empty_mapping()
    {
        var run = Run(Declaration("textbox", "Value"), "{ not json");

        var error = Assert.Single(run.WithId(ComponentPropsGenerator.UnreadableManifestDiagnosticId));

        Assert.Equal(DiagnosticSeverity.Error, error.Severity);
    }

    [Fact]
    public void A_block_whose_upstream_constructor_is_open_ended_is_left_alone()
    {
        // A **kwargs constructor has no fixed parameter list to compare a declaration against, so
        // the manifest records none and the build does not invent one.
        var run = Run(Declaration("html", "Value"), OpenEndedManifest("html"));

        Assert.Empty(run.Diagnostics);
        Assert.Empty(run.Sources);
    }

    /// <summary>
    /// The manifest the build actually reads declares every block this build declares.
    /// </summary>
    /// <remarks>
    /// The generator proves the mapping is complete for the blocks it finds; this proves it finds
    /// the ones that are there, which a manifest naming a different release would not.
    /// </remarks>
    [Fact]
    public void The_shipped_manifest_covers_the_blocks_this_build_declares()
    {
        var manifest = System.IO.File.ReadAllText(Path.Combine(
            RepositoryRoot.FullPath,
            "src",
            "upstream",
            "component-params.json"));

        foreach (var block in new[] { "textbox", "number", "form" })
        {
            Assert.Contains($"\"{block}\"", manifest, StringComparison.Ordinal);
        }
    }

    private static GeneratorRun Run(string source, string? manifest) =>
        GeneratorTestHost.Run(source, manifest, new ComponentPropsGenerator().AsSourceGenerator());

    private static string Declaration(string block, params string[] members)
    {
        var body = string.Join(
            Environment.NewLine,
            members.Select(member => $"        public object? {member} {{ get; set; }}"));

        return $$"""
            namespace Gradio.Net
            {
                internal sealed class GradioBlockAttribute(string block) : System.Attribute { public string Block { get; } = block; }
                public abstract class Block { protected internal virtual void WriteProps(PropWriter props) { } }
                public sealed class PropWriter { public void Add(string name, object? value) { } }
            }

            namespace Gradio.Net
            {
                [GradioBlock("{{block}}")]
                internal sealed partial class {{char.ToUpperInvariant(block[0]) + block[1..]}} : Block
                {
            {{body}}
                }
            }
            """;
    }

    private static string Manifest(string block, params string[] parameters) => Build(block, parameters);

    private static string OpenEndedManifest(string block) => Build(block, parameters: null);

    private static string Build(string block, string[]? parameters)
    {
        var declared = parameters is null
            ? "null"
            : "[" + string.Join(", ", parameters.Select(parameter => $"\"{parameter}\"")) + "]";

        return $$"""
            {
              "schemaVersion": 1,
              "kind": "component-params",
              "upstreamVersion": "{{GradioVersion.Upstream}}",
              "blocks": {
                "{{block}}": { "class": "X", "module": "y", "params": {{declared}} }
              }
            }
            """;
    }
}

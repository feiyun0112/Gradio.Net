using System.Collections.Immutable;
using System.Globalization;
using Gradio.Net;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Gradio.Net.Tests.GrLayer;

/// <summary>
/// The static-type half of the acceptance: a component that has no such event cannot be bound to it,
/// and the compiler is what says so.
/// </summary>
/// <remarks>
/// <para>
/// "No runtime check" is the claim, so a runtime check is not the evidence. The evidence is a
/// compilation: a snippet that calls <c>Click</c> on a textbox is compiled against this assembly and
/// has to fail with the error a person would get, and the same call on a button has to succeed — the
/// second half is what proves the first half is not passing because the harness cannot see anything.
/// </para>
/// <para>
/// The snippet names the core type rather than the layer's factory, because the members live on the
/// component: whether a binding exists is a property of the block, which is what makes it a compile
/// error rather than a lookup that returns null.
/// </para>
/// </remarks>
public sealed class GrCompileTimeTests
{
    [Fact]
    public void A_textbox_cannot_be_bound_to_a_click()
    {
        var diagnostics = Compile(
            """
            using Gradio.Net;

            internal static class Probe
            {
                internal static void Bind(Textbox textbox) => textbox.Click(() => "x", []);
            }
            """);

        var error = Assert.Single(diagnostics.Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error));

        Assert.Equal("CS1061", error.Id);
        Assert.Contains("Click", error.GetMessage(CultureInfo.InvariantCulture), StringComparison.Ordinal);
    }

    [Fact]
    public void A_button_can_be_bound_to_a_click()
    {
        // The control: the same shape of call, on the block that has the event. Without it the test
        // above would pass just as well if the compilation could not see this assembly at all.
        var diagnostics = Compile(
            """
            using Gradio.Net;

            internal static class Probe
            {
                internal static void Bind(Button button) => button.Click(() => "x", []);
            }
            """);

        Assert.Empty(diagnostics.Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error));
    }

    [Fact]
    public void A_textbox_has_change_and_submit_and_a_button_has_click()
    {
        // The three bindings the ticket names, spelled the way a port writes them: the word is
        // the Python version's and only its first letter changed.
        Assert.Empty(Compile(
            """
            using Gradio.Net;

            internal static class Probe
            {
                internal static void Bind(Textbox textbox, Button button)
                {
                    textbox.Change(value => value, [textbox], [textbox]);
                    textbox.Submit(value => value, [textbox], [textbox]);
                    button.Click(() => "x", [textbox]);
                }
            }
            """).Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error));
    }

    [Fact]
    public void The_trigger_points_are_reachable_under_Events()
    {
        // `btn.click` used as a value — `gr.on([btn.click, tb.change], ...)` — is the one place the
        // name had to move rather than change case, and the move is to `Events`.
        Assert.Empty(Compile(
            """
            using Gradio.Net;

            internal static class Probe
            {
                internal static EventTrigger[] Triggers(Textbox textbox, Button button) =>
                    [button.Events.Click, textbox.Events.Change, textbox.Events.Submit];
            }
            """).Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error));
    }

    [Fact]
    public void A_textbox_cannot_be_bound_to_a_signal_it_does_not_raise()
    {
        // The ticket's own example: a textbox has `Change`, `Input` and `Submit`, and no `Like` or
        // `Retry`. Those belong to a chatbot, and because they are members of that type rather than
        // entries in a table, binding one on a textbox is the compiler's answer rather than a
        // handler nothing ever runs.
        foreach (var signal in new[] { "Like", "Retry" })
        {
            var diagnostics = Compile(
                $$"""
                using Gradio.Net;

                internal static class Probe
                {
                    internal static void Bind(Textbox textbox) => textbox.{{signal}}(value => value, [textbox], [textbox]);
                }
                """);

            var error = Assert.Single(diagnostics.Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error));

            Assert.Equal("CS1061", error.Id);
            Assert.Contains(signal, error.GetMessage(CultureInfo.InvariantCulture), StringComparison.Ordinal);
        }
    }

    [Fact]
    public void A_chatbot_can_be_bound_to_the_signals_a_textbox_cannot()
    {
        // The control for the test above: the same two calls on the component that does raise them.
        // Without it, a compilation that could not see the assembly at all would pass it.
        Assert.Empty(Compile(
            """
            using Gradio.Net;

            internal static class Probe
            {
                internal static void Bind(Chatbot chatbot)
                {
                    chatbot.Like(value => value, [chatbot], [chatbot]);
                    chatbot.Retry(value => value, [chatbot], [chatbot]);
                }
            }
            """).Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error));
    }

    [Fact]
    public void Merging_two_trigger_points_is_the_line_the_decision_record_writes()
    {
        // `gr.on([btn.click, tb.change], ...)` with the two names that moved under `Events`, and
        // nothing else about the line changed: the triggers are values, the handler is
        // the second argument, and the components are the ones the handler reads and writes.
        Assert.Empty(Compile(
            """
            using Gradio.Net;

            internal static class Probe
            {
                internal static void Bind(Button button, Textbox textbox) =>
                    gr.On(
                        [button.Events.Click, textbox.Events.Change],
                        (System.Func<object?, object?>)(value => value),
                        [textbox],
                        [button]);
            }
            """).Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error));
    }

    [Fact]
    public void A_layout_built_by_the_layer_has_no_binding_to_offer()
    {
        // A layout factory returns the core layout, and a layout carries no value — so it has no
        // binding members, and `gr.Row().Click(...)` is a compile error rather than a lookup that
        // returns null at run time. That is what "the factory returns the core type" buys.
        var diagnostics = Compile(
            """
            using Gradio.Net;

            internal static class Probe
            {
                internal static void Bind() => gr.Row().Click(() => "x", []);
            }
            """);

        var error = Assert.Single(diagnostics.Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error));

        Assert.Equal("CS1061", error.Id);
        Assert.Contains("Click", error.GetMessage(CultureInfo.InvariantCulture), StringComparison.Ordinal);
    }

    [Fact]
    public void A_layouts_signal_is_a_trigger_point_rather_than_a_binding()
    {
        // A layout's events are trigger points, not bindings: `tabs.Change` is a value a caller
        // hands to `gr.On`, and calling it is the mistake the compiler catches.
        Assert.Empty(Compile(
            """
            using Gradio.Net;

            internal static class Probe
            {
                internal static EventTrigger Change(Tabs tabs) => tabs.Change;
            }
            """).Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error));

        var diagnostics = Compile(
            """
            using Gradio.Net;

            internal static class Probe
            {
                internal static void Bind(Tabs tabs) => tabs.Change(() => "x");
            }
            """);

        Assert.Equal(
            "CS1955",
            Assert.Single(diagnostics.Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)).Id);
    }

    private static ImmutableArray<Diagnostic> Compile(string source) =>
        CSharpCompilation.Create(
            "GradioNetGrProbe",
            [CSharpSyntaxTree.ParseText(source)],
            References(),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable))
        .GetDiagnostics();

    private static ImmutableArray<MetadataReference> References()
    {
        var trusted = (string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")
            ?? throw new InvalidOperationException("The test host does not expose TRUSTED_PLATFORM_ASSEMBLIES.");

        return
        [
            .. trusted
                .Split(Path.PathSeparator)
                .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path)),
            MetadataReference.CreateFromFile(typeof(gr).Assembly.Location),
        ];
    }
}

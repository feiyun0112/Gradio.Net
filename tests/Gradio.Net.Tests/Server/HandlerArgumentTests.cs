using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;
using static Gradio.Net.Tests.Server.QueueRuns;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The value a handler is handed is the C# type it declared, whatever the component produced.
/// </summary>
/// <remarks>
/// <para>
/// A component hands over the number its own rule produced — a whole number from a field rounded to
/// no decimals, a <c>double</c> from one with a precision — and a handler declares the type it
/// works in. The run converts between numeric types on the way to the call, which is what makes a
/// ported Python handler work: the Python version's number field gives an <c>int</c> at zero
/// precision, and a handler annotated <c>int</c> receives one.
/// </para>
/// <para>
/// Both shapes are asserted here because both are what a C# author writes: one declares the type the
/// component produces and one declares the type it means.
/// </para>
/// </remarks>
[Collection(QueueCollection.Name)]
public sealed class HandlerArgumentTests
{
    [Fact]
    public async Task A_handler_declared_with_an_int_is_handed_a_whole_number()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            var (field, output) = Fields(blocks);

            // The raw overload, because a number field's typed one takes the `double` it produces:
            // a handler declared with an `int` is the ported-Python shape, and it goes through here.
            field.Change(
                (Func<int, object?>)(value => $"int {value}"),
                [field],
                [output],
                apiName: "whole");
        });

        using var queue = new QueueClient(host.BaseAddress);

        Assert.Equal(
            "int 4",
            await RunAsync(
                queue,
                await queue.EndpointAsync("whole", cancellationToken),
                [3.6],
                "whole",
                cancellationToken));
    }

    [Fact]
    public async Task A_handler_declared_with_a_double_is_handed_the_same_number()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            var (field, output) = Fields(blocks);

            field.Change(
                (Func<double, object?>)(value => $"double {value}"),
                [field],
                [output],
                apiName: "fractional");
        });

        using var queue = new QueueClient(host.BaseAddress);

        // The component produced a whole number, and the handler asked for the other numeric type:
        // the conversion is what makes both declarations usable on one component.
        Assert.Equal(
            "double 4",
            await RunAsync(
                queue,
                await queue.EndpointAsync("fractional", cancellationToken),
                [3.6],
                "fractional",
                cancellationToken));
    }

    /// <summary>A number field with no decimals, and the field its handler answers into.</summary>
    /// <param name="blocks">The application to render into.</param>
    private static (Number Field, Textbox Output) Fields(Blocks blocks)
    {
        using (blocks.BeginRender())
        {
            var field = blocks.Add(new Number { Label = "Count", Precision = 0 });
            var output = blocks.Add(new Textbox { Label = "Out" });

            return (field, output);
        }
    }
}

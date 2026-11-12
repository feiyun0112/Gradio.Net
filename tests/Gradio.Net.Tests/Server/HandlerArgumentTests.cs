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
/// component produces and one declares the type it means. The same holds for a component whose value
/// is a list: a multi-select dropdown and a checkbox group both answer with a list of values, and a
/// handler may spell that list as an array or as a <see cref="List{T}"/>.
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

    [Fact]
    public async Task A_multiselect_dropdown_hands_an_array_declared_handler_its_values()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        Assert.Equal(
            "array 2 alpha,beta",
            await SelectionAsync(
                () => new Dropdown
                {
                    Label = "Tags",
                    Choices = [new Choice("alpha"), new Choice("beta")],
                    Multiselect = true,
                },
                (Func<object[], object?>)(values =>
                    $"array {values.Length} {string.Join(",", values.Select(value => value?.ToString() ?? ""))}"),
                "dropdown_array",
                cancellationToken));
    }

    [Fact]
    public async Task A_multiselect_dropdown_hands_a_list_declared_handler_its_values()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        Assert.Equal(
            "list 2 alpha,beta",
            await SelectionAsync(
                () => new Dropdown
                {
                    Label = "Tags",
                    Choices = [new Choice("alpha"), new Choice("beta")],
                    Multiselect = true,
                },
                (Func<List<object>, object?>)(values =>
                    $"list {values.Count} {string.Join(",", values.Select(value => value?.ToString() ?? ""))}"),
                "dropdown_list",
                cancellationToken));
    }

    [Fact]
    public async Task A_checkbox_group_hands_an_array_declared_handler_its_values()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        Assert.Equal(
            "array 2 alpha,beta",
            await SelectionAsync(
                () => new CheckboxGroup
                {
                    Label = "Letters",
                    Choices = [new Choice("alpha"), new Choice("beta")],
                },
                (Func<object[], object?>)(values =>
                    $"array {values.Length} {string.Join(",", values.Select(value => value?.ToString() ?? ""))}"),
                "group_array",
                cancellationToken));
    }

    [Fact]
    public async Task A_checkbox_group_hands_a_list_declared_handler_its_values()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        Assert.Equal(
            "list 2 alpha,beta",
            await SelectionAsync(
                () => new CheckboxGroup
                {
                    Label = "Letters",
                    Choices = [new Choice("alpha"), new Choice("beta")],
                },
                (Func<List<object>, object?>)(values =>
                    $"list {values.Count} {string.Join(",", values.Select(value => value?.ToString() ?? ""))}"),
                "group_list",
                cancellationToken));
    }

    [Fact]
    public async Task A_typed_checkbox_group_change_reads_the_values_through_a_list_interface()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            using (blocks.BeginRender())
            {
                var field = blocks.Add(new CheckboxGroup
                {
                    Label = "Letters",
                    Choices = [new Choice("alpha"), new Choice("beta")],
                });
                var output = blocks.Add(new Textbox { Label = "Out" });

                field.Change(
                    (Func<IReadOnlyList<object?>, object?>)(values =>
                        $"typed {values.Count} {string.Join(",", values)}"),
                    [field],
                    [output],
                    apiName: "typed_group");
            }
        });

        using var queue = new QueueClient(host.BaseAddress);

        Assert.Equal(
            "typed 2 alpha,beta",
            await RunAsync(
                queue,
                await queue.EndpointAsync("typed_group", cancellationToken),
                new JsonArray(new JsonArray("alpha", "beta")),
                "typed_group",
                cancellationToken));
    }

    /// <summary>Runs one change on a component that holds a list of values.</summary>
    /// <param name="input">The component the run reads.</param>
    /// <param name="handler">The handler, whose parameter names the list's container type.</param>
    /// <param name="apiName">The name the endpoint is served under.</param>
    /// <param name="cancellationToken">Cancels the run.</param>
    /// <returns>The text the handler answered with.</returns>
    private static async Task<string> SelectionAsync(
        Func<Component> input,
        Delegate handler,
        string apiName,
        CancellationToken cancellationToken)
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            using (blocks.BeginRender())
            {
                var field = blocks.Add(input());
                var output = blocks.Add(new Textbox { Label = "Out" });

                blocks.On(
                    [new EventTrigger(field, EventNames.Change)],
                    new EventBindingOptions
                    {
                        Fn = handler,
                        Inputs = [field],
                        Outputs = [output],
                        ApiName = apiName,
                    });
            }
        });

        using var queue = new QueueClient(host.BaseAddress);

        return await RunAsync(
            queue,
            await queue.EndpointAsync(apiName, cancellationToken),
            new JsonArray(new JsonArray("alpha", "beta")),
            apiName,
            cancellationToken);
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

using Gradio.Net;
namespace Gradio.Net.Tests.Server;

/// <summary>
/// The page the event-binding tests are about.
/// </summary>
/// <remarks>
/// <para>
/// It binds the same twelve events as the case's the Python version demo, in the same order,
/// because the served documents are compared field by field: the order is what makes a binding's id
/// — and so the <c>trigger_after</c> chain and the <c>cancels</c> list — line up between the two.
/// </para>
/// <para>
/// Every field of a binding gets a value the default would not produce, so that a mapping which
/// writes a default where the application asked for something else is caught. The bindings that set
/// nothing are the other half of that: they are what the defaults are compared against.
/// </para>
/// </remarks>
internal static class EventPage
{
    /// <summary>The title the page and the OpenAPI document carry.</summary>
    internal const string Title = "Event page";

    /// <summary>Builds the page into an application.</summary>
    /// <param name="blocks">The application to render into.</param>
    internal static void Build(Blocks blocks)
    {
        blocks.Title = Title;

        // Fixed on both sides of the case rather than left to the defaults, which differ on
        // purpose.
        blocks.AnalyticsEnabled = true;

        Textbox a;
        Textbox b;
        Textbox output;
        Textbox output2;
        Number number;
        Number numberOutput;

        using (blocks.BeginRender())
        {
            a = blocks.Add(new Textbox { Label = "A" });
            b = blocks.Add(new Textbox { Label = "B" });
            output = blocks.Add(new Textbox { Label = "Out" });
            output2 = blocks.Add(new Textbox { Label = "Out 2" });
            number = blocks.Add(new Number { Label = "N" });
            numberOutput = blocks.Add(new Number { Label = "Out N" });
        }

        // Several inputs, several outputs, and a parameter from each group the options divide into.
        a.Change(
            Concat,
            b,
            [output, output2],
            apiName: "concat", apiDescription: "Join two values", parameterNames: ["A", "B"], scrollToOutput: true, showProgress: ProgressDisplay.Minimal, showProgressOn: [output], batch: true, maxBatchSize: 8, concurrencyLimit: ConcurrencyLimit.Of(3), concurrencyId: "group", timeLimit: 30, streamEvery: 0.25, validator: Accept);

        // One input, one output — the shape most bindings take.
        a.Input(
            Shout,
            [a],
            [output],
            apiName: "shout",
            endpointDescription: "One input, one output.",
            parameterNames: ["A"]);

        // A number binding, so that the component the handler reads is not always a textbox.
        number.Change(
            CopyNumber,
            [number],
            [numberOutput],
            apiName: "number_changed", endpointDescription: "A handler reused for more than one binding.", parameterNames: ["A"]);

        // No inputs and no outputs at all.
        a.Blur(Copy, [], [], apiName: "blurred", endpointDescription: "A handler reused for more than one binding.");

        // A step that follows another binding, and one that only runs when it failed. A chained
        // step listens to no component, so it is bound through the application rather than through
        // one — which is what merging triggers is for.
        var chained = a.Submit(
            Concat,
            b,
            [output],
            apiName: "first",
            endpointDescription: "Two inputs, one output. The parameters are named after the components they read, because that is what an endpoint's inputs are called in the API documentation: the Python version reads the handler's own parameter names, and this side of the case gives a binding the same names explicitly.",
            parameterNames: ["A", "B"]);
        blocks.On(
            [chained.Events.Then],
            fn: (Delegate)Step, inputs: [a, output], outputs: [output2], apiName: "second", endpointDescription: "The second half of a chain.", parameterNames: ["A", "Out"]);
        blocks.On(
            [chained.Events.Failure],
            fn: (Delegate)Copy, inputs: [a], outputs: [output2], apiName: "on_failure", endpointDescription: "A handler reused for more than one binding.", parameterNames: ["A"]);

        // One endpoint for two signals, which is what merging triggers is for.
        blocks.On(
            [a.Events.Change, b.Events.Change],
            fn: (Delegate)Copy, inputs: [a], outputs: [output2], apiName: "merged", endpointDescription: "A handler reused for more than one binding.", parameterNames: ["A"]);

        // A binding that cancels another when it starts.
        var longRunning = a.Input(Copy, [a], [output], apiName: "long_running", endpointDescription: "A handler reused for more than one binding.", parameterNames: ["A"]);
        a.Submit(
            Copy,
            [a],
            [output],
            apiName: "cancelling", endpointDescription: "A handler reused for more than one binding.", parameterNames: ["A"], cancels: [longRunning]);

        // A binding that never reaches the server.
        a.Change(
            fn: (Delegate?)null,
            inputs: [],
            outputs: [],
             apiName: "client_only", js: "() => 1");
    }

    private static object? Concat(string left, string right) => left + right;

    private static object? Shout(string value) => value.ToUpperInvariant();

    private static string Step(string left, string right) => $"{left}/{right}";

    private static object? Copy(string value) => value;

    private static object? CopyNumber(double value) => value;

    private static string? Accept(string value) => value.Length >= 3 ? null : "too short";
}

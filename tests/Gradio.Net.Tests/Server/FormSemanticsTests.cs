using System.Text.Json.Nodes;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// What a form means once a submission reaches the server: the fields it groups are read in one
/// run, and grouping changes nothing else about them.
/// </summary>
/// <remarks>
/// <para>
/// The layout rule — which fields end up in a form, and how wide that form is — is asserted where
/// the layout is served. What only a run can show is the other
/// half: that a binding naming fields of two forms reads all of them at once, and that a field
/// inside a form still fires the events the application bound to it. A form is a layout node, not a
/// submission unit, and a submission is a binding.
/// </para>
/// <para>
/// The page is <see cref="FormPage"/>, so the two are talking about one application rather than two
/// that happen to look alike.
/// </para>
/// </remarks>
public sealed class FormSemanticsTests
{
    [Fact]
    public async Task One_submission_reads_every_field_on_the_page()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            cancellationToken,
            FormPage.Build);

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("join", cancellationToken);

        await queue.JoinAsync(fnIndex, ["a", "b", "c", "d", 5.5, "f", "g"], "form", cancellationToken);
        await using var stream = await queue.OpenAsync("form", cancellationToken);

        Assert.True(await stream.UntilClosedAsync(TimeSpan.FromSeconds(15), cancellationToken));

        var completed = stream.Messages.Last(message => QueueSession.Kind(message) == "process_completed");

        // Seven values, one run: two fields from the row's first form, one from the form the button
        // starts, two from the row's second form, and two from the compact column that belongs to
        // no form at all. The handler sees them together, which is what "submitted together" means
        // once the page is served.
        Assert.Equal(
            "a | b | c | d | 5.5 | f | g",
            completed["output"]!["data"]!.AsArray()[0]!.GetValue<string>());
    }

    [Fact]
    public async Task A_field_inside_a_form_keeps_the_event_the_application_bound_to_it()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            cancellationToken,
            FormPage.Build);

        using var queue = new QueueClient(host.BaseAddress);
        var fnIndex = await queue.EndpointAsync("shout", cancellationToken);

        await queue.JoinAsync(fnIndex, ["abc"], "shout", cancellationToken);
        await using var stream = await queue.OpenAsync("shout", cancellationToken);

        Assert.True(await stream.UntilClosedAsync(TimeSpan.FromSeconds(15), cancellationToken));

        var completed = stream.Messages.Last(message => QueueSession.Kind(message) == "process_completed");

        // The first field is inside the row's first form. Grouping is a layout rule, so it neither
        // took the binding away nor turned the form into the thing that fires.
        Assert.Equal("ABC", completed["output"]!["data"]!.AsArray()[0]!.GetValue<string>());
    }

    [Fact]
    public async Task The_grouping_neither_adds_a_binding_nor_names_a_form_in_one()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            cancellationToken,
            FormPage.Build);

        var config = JsonNode.Parse(
            await host.Client.GetStringAsync("/config", cancellationToken))!.AsObject();

        var formIds = config["components"]!.AsArray()
            .Where(component => component!["type"]!.GetValue<string>() == "form")
            .Select(component => component!["id"]!.GetValue<int>())
            .ToArray();

        // Four runs of fields, so four forms: the button between the first two ends one run, the
        // state between the next two does not, the compact column groups nothing at all, and the
        // two fields written after it are a run of their own.
        Assert.Equal(4, formIds.Length);

        var dependencies = config["dependencies"]!.AsArray();

        // The three bindings the application wrote, and no fourth for a form to fire: a form has no
        // events of its own, so grouping cannot have created one.
        Assert.Equal(3, dependencies.Count);

        foreach (var dependency in dependencies)
        {
            // A target is a two-element list — the block and the signal — and a form is never one
            // of them; the same goes for what a binding reads and writes.
            var named = dependency!["targets"]!.AsArray()
                .SelectMany(target => target!.AsArray().Take(1))
                .Concat(dependency["inputs"]!.AsArray())
                .Concat(dependency["outputs"]!.AsArray())
                .Select(id => id!.GetValue<int>())
                .ToArray();

            Assert.DoesNotContain(named, formIds.Contains);
        }
    }
}

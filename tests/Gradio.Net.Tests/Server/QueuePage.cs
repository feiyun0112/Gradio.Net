using Gradio.Net;
namespace Gradio.Net.Tests.Server;

/// <summary>
/// The page the queue tests are about: four bindings, one per shape a run takes.
/// </summary>
/// <remarks>
/// <para>
/// It is written as the case's Python demo writes it, because the served message sequence is
/// compared field by field and the order of the messages is part of what is compared. Each handler
/// is a different method because two bindings that share one share its execution history, and the
/// second one's estimate would no longer be absent.
/// </para>
/// <para>
/// The progress handler sleeps after reporting, because progress is coalesced and sent on a timer:
/// a handler that reported and returned immediately would race its own completion, and the
/// recording would be of whichever arrived first.
/// </para>
/// </remarks>
internal static class QueuePage
{
    /// <summary>The title the page carries.</summary>
    internal const string Title = "Queue page";

    /// <summary>Builds the page into an application.</summary>
    /// <param name="blocks">The application to render into.</param>
    internal static void Build(Blocks blocks)
    {
        blocks.Title = Title;

        Textbox a;
        Textbox output;

        using (blocks.BeginRender())
        {
            a = blocks.Add(new Textbox { Label = "A" });
            output = blocks.Add(new Textbox { Label = "Out" });
        }

        a.Change(Echo, [a], [output], apiName: "echo");

        a.Change(
            (Func<string, IAsyncEnumerable<string>>)Stream,
            [a],
            [output],
            apiName: "streamer");

        a.Change(
            (Func<string, Progress, Task<string>>)Progressing,
            [a],
            [output],
            apiName: "progressor");

        a.Change(Boom, [a], [output], apiName: "boom");
    }

    /// <summary>A plain handler: one value in, one value out.</summary>
    private static object? Echo(string value) => value;

    /// <summary>A generator: three chunks, so that the first, the middle and the last are all sent.</summary>
    private static async IAsyncEnumerable<string> Stream(string value)
    {
        for (var index = 0; index < 3; index++)
        {
            yield return $"{value}-{index}";

            await Task.Yield();
        }
    }

    /// <summary>A handler that reports progress once and then takes long enough for it to be sent.</summary>
    private static async Task<string> Progressing(string value, Progress progress)
    {
        progress.Report(0.5, "halfway");

        await Task.Delay(500);

        return value.ToUpperInvariant();
    }

    /// <summary>A handler that raises, so that the failure shape is compared too.</summary>
    private static object? Boom(string value) =>
        throw new InvalidOperationException($"the handler refused {value}");
}

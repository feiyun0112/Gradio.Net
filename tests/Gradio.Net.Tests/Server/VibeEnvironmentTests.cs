using System.Net;
using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The two variables that decide whether the Vibe editor is on, and what it edits.
/// </summary>
/// <remarks>
/// <para>
/// Everything else about the editor is tested through a <see cref="VibeOptions"/> and a
/// <see cref="Blocks.VibeMode"/> a test set by hand, which is the state an application ends up in
/// once it has read the environment — but it is not the reading. These two are the reading: nothing
/// here registers an option or sets the property, so the defaults are what answer.
/// </para>
/// <para>
/// A variable is process-wide and is read when a host is built, so this class runs on its own: the
/// collection below is declared not to run beside any other, because a host built by another test
/// inside the window would read this test's value. That is a real cost — the suite waits for these
/// two tests — and the alternative is a variable that every other test could trip over.
/// </para>
/// </remarks>
[Collection(VibeEnvironmentDefinition.Name)]
public sealed class VibeEnvironmentTests : IDisposable
{
    private readonly TempFolder _folder = new();

    /// <summary>
    /// The switch is a flag that was set, not a word to be parsed.
    /// </summary>
    /// <remarks>
    /// The Python version's test is <c>bool(os.getenv("GRADIO_VIBE_MODE", ""))</c>, so
    /// <c>"false"</c> and <c>"0"</c> both turn the editor <em>on</em> — and <c>gradio</c> 6.30.0
    /// answers <c>vibe_mode: true</c> for both. A later reader would be tempted to "fix" that into
    /// a boolean parse; the fix would silently disagree with the Python deployment beside it.
    /// </remarks>
    [Fact]
    public async Task Any_value_of_the_switch_variable_turns_the_editor_on()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        using var mode = EnvironmentVariable.Set("GRADIO_VIBE_MODE", "false");

        await using var host = await StartAsync(cancellationToken);

        var config = JsonNode.Parse(await host.Client.GetStringAsync("/config", cancellationToken))!;

        Assert.True(config["vibe_mode"]!.GetValue<bool>());

        // The switch is on and this deployment named no demo file, so the route answers about the
        // file rather than about the switch — which is the difference between 404 and 403.
        using var read = await host.Client.GetAsync("/gradio_api/vibe-code", cancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, read.StatusCode);
        Assert.Equal(
            """{"detail":"Demo file not found"}""",
            await read.Content.ReadAsStringAsync(cancellationToken));
    }

    /// <summary>
    /// The file the editor edits is the one the environment names.
    /// </summary>
    /// <remarks>
    /// <c>GRADIO_WATCH_DEMO_PATH</c> is the Python version's name for it — the file its reload
    /// watcher follows — and it is the only way a deployment tells this capability what to rewrite.
    /// The switch has to be on for the route to answer at all, so both variables are set: what is
    /// asserted is that the path came from the variable rather than from a test.
    /// </remarks>
    [Fact]
    public async Task The_demo_file_is_the_one_the_environment_names()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var demo = _folder.Write("from_env.py", "import gradio as gr\n");

        using var mode = EnvironmentVariable.Set("GRADIO_VIBE_MODE", "1");
        using var path = EnvironmentVariable.Set("GRADIO_WATCH_DEMO_PATH", demo);

        await using var host = await StartAsync(cancellationToken);

        using var read = await host.Client.GetAsync("/gradio_api/vibe-code", cancellationToken);

        Assert.Equal(HttpStatusCode.OK, read.StatusCode);
        Assert.Equal(
            new JsonObject { ["code"] = "import gradio as gr\n" }.ToJsonString(),
            await read.Content.ReadAsStringAsync(cancellationToken));
    }

    public void Dispose() => _folder.Dispose();

    /// <summary>Starts a host that registers nothing of the editor's, so the environment decides.</summary>
    /// <param name="cancellationToken">Cancels the start.</param>
    /// <returns>The host.</returns>
    private static Task<GradioTestHost> StartAsync(CancellationToken cancellationToken) =>
        GradioTestHost.StartIntegratedAsync(
            logs: null,
            cancellationToken,
            configure: null,
            blocks =>
            {
                using (blocks.BeginRender())
                {
                    blocks.Add(new Textbox { Label = "Anything" });
                }
            });
}

/// <summary>
/// The collection the environment tests run in: alone, because they set a process-wide variable.
/// </summary>
/// <remarks>
/// <see cref="CollectionDefinitionAttribute.DisableParallelization"/> is what makes the collection
/// run by itself rather than beside the others, which is the whole point: a host built by any other
/// test during the window would read the variable this collection set.
/// </remarks>
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class VibeEnvironmentDefinition
{
    /// <summary>The collection's name, as the tests declare it.</summary>
    public const string Name = "vibe-environment";
}

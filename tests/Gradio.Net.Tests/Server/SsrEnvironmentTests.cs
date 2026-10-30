using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The variable that decides whether server-side rendering is on.
/// </summary>
/// <remarks>
/// <para>
/// Everything else about SSR is tested through a <see cref="GradioServerOptions"/> a test filled
/// in, which is the state an application ends up in once it has read the environment — but it is
/// not the reading. This is the reading, and the order it resolves in: the option wins, then
/// <c>GRADIO_SSR_MODE</c>, then off (the Python version's <c>ssr_mode</c>).
/// </para>
/// <para>
/// The variable is process-wide and is read when a server is built, so this class runs on its own:
/// the collection below is declared not to run beside any other, because a server built by another
/// test inside the window would read this test's value. The alternative — a variable every other
/// test could trip over — is worse than the suite waiting for this one.
/// </para>
/// </remarks>
[Collection(SsrEnvironmentDefinition.Name)]
public sealed class SsrEnvironmentTests
{
    [Fact]
    public async Task The_mode_can_come_from_the_environment_and_the_option_wins_over_it()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var missing = SsrNode.NoSuchRuntimePath();

        using var mode = EnvironmentVariable.Set("GRADIO_SSR_MODE", "true");

        // The variable turned it on: with no option set, the server goes looking for the runtime it
        // was pointed at and fails on the path rather than serving a client-rendered page.
        await using (var server = new GradioServer(new GradioServerOptions
        {
            ServerPort = 0,
            SsrNodePath = missing,
        }))
        {
            ClickExample.Build(server.Blocks);

            var error = await Assert.ThrowsAsync<InvalidOperationException>(
                () => server.StartAsync(cancellationToken));

            Assert.Contains(missing, error.Message, StringComparison.Ordinal);
        }

        // And the option wins: the same variable is still set, the application says off, and the
        // page is the .NET template again. The Python version resolves it in this order so that an image can
        // be turned over to SSR without recompiling - which only works if the explicit answer wins.
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            cancellationToken,
            ClickExample.Build,
            options: options =>
            {
                options.SsrMode = false;
                options.SsrNodePath = missing;
            });

        var page = await host.Client.GetStringAsync("/", cancellationToken);

        Assert.Contains("<gradio-app", page, StringComparison.Ordinal);
        Assert.DoesNotContain("__sveltekit_", page, StringComparison.Ordinal);
    }
}

/// <summary>
/// The collection the SSR environment test runs in: alone, because it sets a process-wide variable.
/// </summary>
/// <remarks>
/// <see cref="CollectionDefinitionAttribute.DisableParallelization"/> is what makes the collection
/// run by itself rather than beside the others, which is the whole point: a server built by any
/// other test during the window would read the variable this collection set.
/// </remarks>
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class SsrEnvironmentDefinition
{
    /// <summary>The collection's name, as the test declares it.</summary>
    public const string Name = "ssr-environment";
}

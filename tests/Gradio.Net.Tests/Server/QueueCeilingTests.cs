using Gradio.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The queue's whole-application ceiling, which the Python version sets with
/// <c>launch(max_threads=...)</c> rather than fixing in the framework.
/// </summary>
/// <remarks>
/// Both hosting entry points are asserted: a standalone host names the number in its launch
/// options, and an integrated host names the same number in the kernel options it configures.
/// Reading the queue the host actually started is what proves the option arrived at
/// <c>GradioQueue.Start</c> — a test of the options object alone would pass with the ceiling
/// still hard-coded.
/// </remarks>
public sealed class QueueCeilingTests
{
    [Fact]
    public async Task A_host_that_names_no_ceiling_gets_the_python_default()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(null, cancellationToken);

        Assert.Equal(40, host.Services.GetRequiredService<GradioQueue>().WorkerCount);
    }

    [Fact]
    public async Task A_standalone_host_can_lower_the_ceiling()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(
            null,
            cancellationToken,
            options: options => options.MaxThreads = 3);

        Assert.Equal(3, host.Services.GetRequiredService<GradioQueue>().WorkerCount);
    }

    [Fact]
    public async Task An_integrated_host_can_lower_the_ceiling_in_its_kernel_options()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartIntegratedAsync(
            null,
            cancellationToken,
            configure: services => services.Configure<GradioCoreOptions>(options => options.MaxThreads = 6));

        Assert.Equal(6, host.Services.GetRequiredService<GradioQueue>().WorkerCount);
    }

    [Fact]
    public async Task A_ceiling_below_one_is_one_running_slot()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var host = await GradioTestHost.StartStandaloneAsync(
            null,
            cancellationToken,
            options: options => options.MaxThreads = 0);

        Assert.Equal(1, host.Services.GetRequiredService<GradioQueue>().WorkerCount);
    }
}

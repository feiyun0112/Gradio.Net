using Gradio.Net;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gradio.Net.Tests.Kernel;

/// <summary>
/// What <c>timeLimit</c> means in the Python version, and what it deliberately does not mean.
/// </summary>
/// <remarks>
/// <para>
/// The parameter is not a deadline for an ordinary handler and not a deadline for waiting in the
/// queue. Both of those still run to completion: Python uses it for a run the browser feeds chunk
/// by chunk, where the first call spends part of the budget, the generating message reports what
/// is left, the wait for the next chunk spends the rest, and a run that has spent it all stops.
/// </para>
/// <para>
/// These tests hold the arithmetic to that rule. The message shapes and the run's stopping are
/// asserted in <see cref="QueueMessageTests"/> and in the server tests, so a change to either half
/// cannot quietly turn the parameter into a hard timeout for every handler.
/// </para>
/// </remarks>
public sealed class TimeLimitTests
{
    [Fact]
    public void A_run_with_no_limit_waits_the_python_default_less_the_first_call()
    {
        using var page = new PageScope();
        var binding = Bind(page, timeLimit: null);

        Assert.Equal(TimeSpan.FromSeconds(29.25), StreamingOutputs.WaitOf(binding, 0.75));
    }

    [Fact]
    public void A_run_with_a_limit_waits_what_the_first_call_left_of_it()
    {
        using var page = new PageScope();
        var binding = Bind(page, timeLimit: 3);

        Assert.Equal(TimeSpan.FromSeconds(2.25), StreamingOutputs.WaitOf(binding, 0.75));
    }

    [Fact]
    public void A_first_call_that_spent_the_whole_limit_leaves_no_wait()
    {
        using var page = new PageScope();
        var binding = Bind(page, timeLimit: 3);

        Assert.Equal(TimeSpan.Zero, StreamingOutputs.WaitOf(binding, 5));
    }

    [Fact]
    public void A_stated_limit_of_zero_falls_back_to_the_python_default()
    {
        using var page = new PageScope();
        var binding = Bind(page, timeLimit: 0);

        // `fn.time_limit or 30`: zero is falsy in Python, so it is not a limit of zero seconds.
        Assert.Equal(TimeSpan.FromSeconds(29.5), StreamingOutputs.WaitOf(binding, 0.5));
    }

    [Fact]
    public void A_binding_on_the_stream_event_is_the_one_the_limit_applies_to()
    {
        using var page = new PageScope();

        var fed = page.Blocks.On(
            [new EventTrigger(page.Input, EventNames.Stream)],
            fn: (Func<string, string>)(value => value),
            inputs: [page.Input],
            outputs: [page.Output],
            timeLimit: 3);

        var ordinary = Bind(page, timeLimit: 3);

        Assert.Equal(Dependency.StreamConnection, fed.Connection);
        Assert.Equal(Dependency.SseConnection, ordinary.Connection);
        Assert.Equal(3, fed.TimeLimit);
        Assert.Equal(3, ordinary.TimeLimit);
    }

    /// <summary>A binding with a time limit, on the ordinary event a page handler uses.</summary>
    private static Dependency Bind(PageScope page, double? timeLimit) =>
        page.Input.Change(
            (Func<string, string>)(value => value),
            [page.Input],
            [page.Output],
            timeLimit: timeLimit);

    /// <summary>An application with the two components a binding needs, and a render scope around them.</summary>
    private sealed class PageScope : IDisposable
    {
        private readonly ServiceProvider _provider;
        private readonly IDisposable _render;

        internal PageScope()
        {
            _provider = KernelHost.BuildProvider();
            Blocks = _provider.CreateBlocksWithContainers();
            _render = Blocks.BeginRender();

            Input = Blocks.Add(new Textbox());
            Output = Blocks.Add(new Textbox());
        }

        internal Blocks Blocks { get; }

        internal Textbox Input { get; }

        internal Textbox Output { get; }

        public void Dispose()
        {
            _render.Dispose();
            Blocks.Dispose();
            _provider.Dispose();
        }
    }
}

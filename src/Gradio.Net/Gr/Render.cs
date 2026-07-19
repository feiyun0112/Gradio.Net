using Gradio.Net.Core;
using Gradio.Net.Events;

namespace Gradio.Net;

public static partial class gr
{
    // -----------------------------------------------------------------------
    // Internal helper — shared by all public overloads
    // -----------------------------------------------------------------------
    private static Renderable RenderCore(
        Delegate fn,
        object? inputs,
        object? triggers,
        bool queue,
        string triggerMode,
        object? concurrencyLimit,
        string? concurrencyId,
        string showProgress)
    {
        var decorator = RenderDecorator.Render(
            inputs: inputs,
            triggers: triggers,
            queue: queue,
            triggerMode: triggerMode,
            concurrencyLimit: concurrencyLimit,
            concurrencyId: concurrencyId,
            showProgress: showProgress);

        decorator(fn);

        return (Renderable)Context.RootBlock.Renderables[^1];
    }

    // -----------------------------------------------------------------------
    // Untyped overload (fn passed as Delegate explicitly)
    // -----------------------------------------------------------------------

    public static Renderable Render(
        Delegate? fn = null,
        object? inputs = null,
        object? triggers = null,
        bool queue = true,
        string triggerMode = "always_last",
        object? concurrencyLimit = null,
        string? concurrencyId = null,
        string showProgress = "full")
    {
        if (fn != null)
            return RenderCore(fn, inputs, triggers, queue, triggerMode, concurrencyLimit, concurrencyId, showProgress);

        // No fn — create empty renderable (rare, but keep parity)
        RenderDecorator.Render(
            inputs: inputs,
            triggers: triggers,
            queue: queue,
            triggerMode: triggerMode,
            concurrencyLimit: concurrencyLimit,
            concurrencyId: concurrencyId,
            showProgress: showProgress);

        return (Renderable)Context.RootBlock.Renderables[^1];
    }

}

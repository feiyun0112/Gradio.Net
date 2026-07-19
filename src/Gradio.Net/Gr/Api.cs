namespace Gradio.Net;

public static partial class gr
{
    public static Gradio.Net.Events.Dependency Api(
        Delegate fn,
        string? apiName = null,
        string? apiDescription = null,
        bool queue = true,
        bool batch = false,
        int maxBatchSize = 4,
        object? concurrencyLimit = null,
        string? concurrencyId = null,
        string apiVisibility = "public",
        int? timeLimit = null,
        float streamEvery = 0.5f)
        => Gradio.Net.Events.Events.Api(
            fn: fn,
            apiName: apiName,
            apiDescription: apiDescription,
            queue: queue,
            batch: batch,
            maxBatchSize: maxBatchSize,
            concurrencyLimit: concurrencyLimit,
            concurrencyId: concurrencyId,
            apiVisibility: apiVisibility,
            timeLimit: timeLimit,
            streamEvery: streamEvery);
}

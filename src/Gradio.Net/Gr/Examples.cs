using Gradio.Net.Core;

namespace Gradio.Net;

public static partial class gr
{
    public static Gradio.Net.Core.Examples Examples(
        object examples,
        object inputs,
        object? outputs = null,
        Delegate? fn = null,
        bool? cacheExamples = null,
        string? cacheMode = null,
        int examplesPerPage = 10,
        bool apiMode = false,
        object? label = null,
        string? elemId = null,
        bool runOnClick = false,
        bool preprocess = true,
        bool postprocess = true,
        string apiVisibility = "undocumented",
        string? apiName = "load_example",
        object? apiDescription = null,
        bool batch = false,
        List<string>? exampleLabels = null,
        object? visible = null,
        object? preload = null)
    {
        return Helpers.CreateExamples(
            examples,
            inputs,
            outputs,
            fn,
            cacheExamples,
            cacheMode,
            examplesPerPage,
            apiMode,
            label,
            elemId,
            runOnClick,
            preprocess,
            postprocess,
            apiVisibility,
            apiName,
            apiDescription,
            batch,
            exampleLabels,
            visible,
            preload);
    }
}

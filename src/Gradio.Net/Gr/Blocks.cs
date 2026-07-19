using Gradio.Net.Core;

namespace Gradio.Net;

public static partial class gr
{
    public static Core.Blocks Blocks(
        bool? analyticsEnabled = null,
        string mode = "blocks",
        string title = "Gradio",
        bool fillHeight = false,
        bool fillWidth = false,
        (int frequency, int age)? deleteCache = null)
    {
        // Convert nullable tuple to System.Tuple<int, int>
        System.Tuple<int, int>? tupleDeleteCache = deleteCache.HasValue ?
            new System.Tuple<int, int>(deleteCache.Value.frequency, deleteCache.Value.age) :
            null;

        var blocks = new Core.Blocks(
            analyticsEnabled: analyticsEnabled,
            mode: mode,
            title: title,
            fillHeight: fillHeight,
            fillWidth: fillWidth,
            deleteCache: tupleDeleteCache);

        // Python-style parity for `with gr.Blocks() as demo:`
        // create + enter context in one step.
        blocks.Enter();
        return blocks;
    }
}

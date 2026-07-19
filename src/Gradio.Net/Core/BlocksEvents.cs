using Gradio.Net.Events;

namespace Gradio.Net.Core;

public class BlocksEvents
{
    private readonly Blocks _blocks;

    public BlocksEvents(Blocks blocks)
    {
        _blocks = blocks;
    }

    public BlockFunction Load(
        Delegate? fn,
        object inputs,
        object outputs,
        bool preprocess = true,
        bool postprocess = true)
    {
        // Create event targets
        // Python parity: demo.load() targets [[demo._id, 'load']], so pass _blocks as the target block.
        var targets = new List<EventListenerMethod>
        {
            new EventListenerMethod(_blocks, EventType.Load)
        };

        // Call SetEventTrigger through DefaultConfig
        var (blockFunction, _) = _blocks.DefaultConfig.SetEventTrigger(
            targets: targets,
            fn: fn,
            inputs: inputs,
            outputs: outputs,
            preprocess: preprocess,
            postprocess: postprocess
        );

        return blockFunction;
    }

    public BlockFunction Unload(Delegate? fn)
    {
        var targets = new List<EventListenerMethod>
        {
            new EventListenerMethod(_blocks, "unload")
        };

        var (blockFunction, _) = _blocks.DefaultConfig.SetEventTrigger(
            targets: targets,
            fn: fn,
            inputs: null,
            outputs: null,
            preprocess: false,
            postprocess: false,
            queue: false,
            apiVisibility: "private"
        );

        return blockFunction;
    }
}

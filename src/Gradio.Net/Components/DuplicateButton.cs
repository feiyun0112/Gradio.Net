using Gradio.Net.Events;

namespace Gradio.Net.Components;

public class DuplicateButton : Button
{
    public bool IsTemplate { get; } = true;

    public DuplicateButton(
        string value = "Duplicate Space",
        object? every = null,
        object? inputs = null,
        string variant = "huggingface",
        string size = "sm",
        object? icon = null,
        string? link = null,
        bool visible = true,
        bool interactive = true,
        string? elemId = null,
        object? elemClasses = null,
        bool render = true,
        object? key = null,
        object? preservedByKey = null,
        int? scale = 0,
        int? minWidth = null,
        bool activate = true
    ) : base(
        value: value,
        variant: variant,
        size: size,
        icon: icon?.ToString(),
        link: link,
        visible: visible,
        interactive: interactive,
        elemId: elemId,
        elemClasses: elemClasses as List<string>,
        key: key,
        preservedByKey: preservedByKey as string,
        scale: scale,
        minWidth: minWidth)
    {
        if (elemClasses is string cls)
        {
            ElemClasses = new List<string> { cls };
        }

        if (preservedByKey is string pbk)
        {
            PreservedByKey = new List<string> { pbk };
        }
        else if (preservedByKey is List<string> pbkList)
        {
            PreservedByKey = pbkList;
        }

        if (activate && Core.Context.GetBlocksContext() != null)
        {
            Activate();
        }
    }

    public void Activate()
    {
        var spaceName = global::Gradio.Net.Utils.Utils.GetSpace();
        if (string.IsNullOrWhiteSpace(spaceName))
        {
            return;
        }

        var blocksConfig = Core.Context.GetBlocksContext();
        if (blocksConfig == null)
        {
            return;
        }

        var js = $"() => {{ window.open(`https://huggingface.co/spaces/{spaceName}?duplicate=true`, '_blank') }}";

        blocksConfig.SetEventTrigger(
            targets: new List<EventListenerMethod> { new EventListenerMethod(this, "click") },
            fn: null,
            inputs: Array.Empty<object>(),
            outputs: null,
            js: js,
            preprocess: false,
            postprocess: false,
            apiVisibility: "private"
        );
    }
}

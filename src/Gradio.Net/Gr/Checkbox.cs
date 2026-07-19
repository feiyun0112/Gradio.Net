using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static Checkbox Checkbox(
        bool? value = false,
        string label = null,
        string info = null,
        object every = null,
        object inputs = null,
        bool? showLabel = null,
        bool? container = null,
        int? scale = null,
        int? minWidth = null,
        bool? interactive = null,
        bool visible = true,
        string elemId = null,
        List<string> elemClasses = null,
        bool render = true,
        object key = null,
        string preservedByKey = null)
    {
        return new Checkbox(
            value: value,
            label: label,
            info: info,
            every: every,
            inputs: inputs,
            showLabel: showLabel,
            container: container,
            scale: scale,
            minWidth: minWidth,
            interactive: interactive,
            visible: visible,
            elemId: elemId,
            elemClasses: elemClasses,
            render: render,
            key: key,
            preservedByKey: preservedByKey);
    }
}

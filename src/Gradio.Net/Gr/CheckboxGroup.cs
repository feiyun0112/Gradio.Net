using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static CheckboxGroup CheckboxGroup(
        object choices = null,
        object value = null,
        string type = "value",
        string label = null,
        string info = null,
        object every = null,
        object inputs = null,
        bool? showLabel = null,
        bool showSelectAll = false,
        bool container = true,
        int? scale = null,
        int minWidth = 160,
        bool? interactive = null,
        object visible = null,
        string elemId = null,
        object elemClasses = null,
        bool render = true,
        object key = null,
        object preservedByKey = null,
        List<string> buttons = null)
    {
        return new CheckboxGroup(
            choices: choices,
            value: value,
            type: type,
            label: label,
            info: info,
            every: every,
            inputs: inputs,
            showLabel: showLabel,
            showSelectAll: showSelectAll,
            container: container,
            scale: scale,
            minWidth: minWidth,
            interactive: interactive,
            visible: visible,
            elemId: elemId,
            elemClasses: elemClasses,
            render: render,
            key: key,
            preservedByKey: preservedByKey,
            buttons: buttons);
    }
}

using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static Dropdown Dropdown(
        object choices = null,
        object value = null,
        string type = "value",
        bool? multiselect = null,
        bool allowCustomValue = false,
        int? maxChoices = null,
        bool filterable = true,
        string label = null,
        string info = null,
        object every = null,
        object inputs = null,
        bool? showLabel = null,
        bool container = true,
        int? scale = null,
        int minWidth = 160,
        bool? interactive = null,
        object visible = null,
        string elemId = null,
        object elemClasses = null,
        bool render = true,
        object key = null,
        object preservedByKey = null)
    {
        return new Dropdown(
            choices: choices,
            value: value,
            type: type,
            multiselect: multiselect,
            allowCustomValue: allowCustomValue,
            maxChoices: maxChoices,
            filterable: filterable,
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

using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static FileComponent File(
        object value = null,
        string fileCount = "single",
        List<string> fileTypes = null,
        string type = "filepath",
        string label = null,
        object every = null,
        object inputs = null,
        bool? showLabel = null,
        bool container = true,
        int? scale = null,
        int minWidth = 160,
        object height = null,
        bool? interactive = null,
        object visible = null,
        string elemId = null,
        object elemClasses = null,
        bool render = true,
        object key = null,
        object preservedByKey = null,
        bool allowReordering = false)
    {
        return new FileComponent(
            value: value,
            fileCount: fileCount,
            fileTypes: fileTypes,
            type: type,
            label: label,
            every: every,
            inputs: inputs,
            showLabel: showLabel,
            container: container,
            scale: scale,
            minWidth: minWidth,
            height: height,
            interactive: interactive,
            visible: visible,
            elemId: elemId,
            elemClasses: elemClasses,
            render: render,
            key: key,
            preservedByKey: preservedByKey,
            allowReordering: allowReordering);
    }
}

using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static Gallery Gallery(
        object value = null,
        string format = "webp",
        List<string> fileTypes = null,
        string label = null,
        object every = null,
        object inputs = null,
        bool? showLabel = null,
        bool container = true,
        int? scale = null,
        int minWidth = 160,
        object visible = null,
        string elemId = null,
        object elemClasses = null,
        bool render = true,
        object key = null,
        object preservedByKey = null,
        int? columns = 2,
        int? rows = null,
        object height = null,
        bool allowPreview = true,
        bool? preview = null,
        int? selectedIndex = null,
        string objectFit = null,
        List<string> buttons = null,
        bool? interactive = null,
        string type = "filepath",
        bool fitColumns = true)
    {
        return new Gallery(
            value: value,
            format: format,
            fileTypes: fileTypes,
            label: label,
            every: every,
            inputs: inputs,
            showLabel: showLabel,
            container: container,
            scale: scale,
            minWidth: minWidth,
            visible: visible,
            elemId: elemId,
            elemClasses: elemClasses,
            render: render,
            key: key,
            preservedByKey: preservedByKey,
            columns: columns,
            rows: rows,
            height: height,
            allowPreview: allowPreview,
            preview: preview,
            selectedIndex: selectedIndex,
            objectFit: objectFit,
            buttons: buttons,
            interactive: interactive,
            type: type,
            fitColumns: fitColumns);
    }
}

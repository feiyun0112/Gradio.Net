using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static FileExplorer FileExplorer(
        string glob = "**/*",
        object value = null,
        string fileCount = "multiple",
        object rootDir = null,
        string ignoreGlob = null,
        string label = null,
        object every = null,
        object inputs = null,
        bool? showLabel = null,
        bool container = true,
        int? scale = null,
        int minWidth = 160,
        object height = null,
        object maxHeight = null,
        object minHeight = null,
        bool? interactive = null,
        object visible = null,
        string elemId = null,
        object elemClasses = null,
        bool render = true,
        object key = null,
        object preservedByKey = null)
    {
        return new FileExplorer(
            glob: glob,
            value: value,
            fileCount: fileCount,
            rootDir: rootDir,
            ignoreGlob: ignoreGlob,
            label: label,
            every: every,
            inputs: inputs,
            showLabel: showLabel,
            container: container,
            scale: scale,
            minWidth: minWidth,
            height: height,
            maxHeight: maxHeight,
            minHeight: minHeight,
            interactive: interactive,
            visible: visible,
            elemId: elemId,
            elemClasses: elemClasses,
            render: render,
            key: key,
            preservedByKey: preservedByKey);
    }
}

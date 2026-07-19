using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static HighlightedText HighlightedText(
        object value = null,
        Dictionary<string, string> colorMap = null,
        bool showLegend = false,
        bool showInlineCategory = true,
        bool combineAdjacent = false,
        string adjacentSeparator = "",
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
        bool? interactive = null,
        bool rtl = false)
    {
        return new HighlightedText(
            value: value,
            colorMap: colorMap,
            showLegend: showLegend,
            showInlineCategory: showInlineCategory,
            combineAdjacent: combineAdjacent,
            adjacentSeparator: adjacentSeparator,
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
            interactive: interactive,
            rtl: rtl);
    }
}

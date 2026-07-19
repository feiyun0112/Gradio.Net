using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static DateTimeComponent DateTime(
        object value = null,
        bool includeTime = true,
        string type = "timestamp",
        string timezone = null,
        string label = null,
        bool? showLabel = null,
        string info = null,
        object every = null,
        int? scale = null,
        int minWidth = 160,
        object visible = null,
        bool? interactive = null,
        string elemId = null,
        object elemClasses = null,
        bool render = true,
        object key = null,
        object preservedByKey = null)
    {
        return new DateTimeComponent(
            value: value,
            includeTime: includeTime,
            type: type,
            timezone: timezone,
            label: label,
            showLabel: showLabel,
            info: info,
            every: every,
            scale: scale,
            minWidth: minWidth,
            visible: visible,
            interactive: interactive,
            elemId: elemId,
            elemClasses: elemClasses,
            render: render,
            key: key,
            preservedByKey: preservedByKey);
    }
}

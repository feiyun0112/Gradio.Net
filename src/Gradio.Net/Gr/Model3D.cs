using Gradio.Net.Components;

namespace Gradio.Net;

public static partial class gr
{
    public static Model3D Model3D(
        object value = null,
        string displayMode = null,
        object clearColor = null,
        object cameraPosition = null,
        double zoomSpeed = 1,
        double panSpeed = 1,
        object height = null,
        string label = null,
        bool? showLabel = null,
        object every = null,
        object inputs = null,
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
        return new Model3D(
            value: value,
            displayMode: displayMode,
            clearColor: clearColor,
            cameraPosition: cameraPosition,
            zoomSpeed: zoomSpeed,
            panSpeed: panSpeed,
            height: height,
            label: label,
            showLabel: showLabel,
            every: every,
            inputs: inputs,
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

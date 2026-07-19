
namespace Gradio.Net.Components;

public class LinePlot : NativePlot
{
    public LinePlot(
        object? value = null,
        string? x = null,
        string? y = null,
        string? color = null,
        string? title = null,
        string? xTitle = null,
        string? yTitle = null,
        string? colorTitle = null,
        object? xBin = null,
        string? yAggregate = null,
        Dictionary<string, string>? colorMap = null,
        List<string>? colorsInLegend = null,
        List<double?>? xLim = null,
        List<double?>? yLim = null,
        double xLabelAngle = 0,
        double yLabelAngle = 0,
        object? xAxisLabelsVisible = null,
        string? caption = null,
        object? sort = null,
        object? tooltip = null,
        int? height = null,
        string? label = null,
        bool? showLabel = null,
        bool container = true,
        int? scale = null,
        int minWidth = 160,
        object? every = null,
        object? inputs = null,
        object? visible = null,
        string? elemId = null,
        object? elemClasses = null,
        bool render = true,
        List<string>? buttons = null,
        object? key = null,
        object? preservedByKey = null)
        : base(value: value, x: x, y: y, color: color, title: title, xTitle: xTitle, yTitle: yTitle,
               colorTitle: colorTitle, xBin: xBin, yAggregate: yAggregate, colorMap: colorMap,
               colorsInLegend: colorsInLegend, xLim: xLim, yLim: yLim, xLabelAngle: xLabelAngle,
               yLabelAngle: yLabelAngle, xAxisLabelsVisible: xAxisLabelsVisible, caption: caption,
               sort: sort, tooltip: tooltip, height: height, label: label, showLabel: showLabel,
               container: container, scale: scale, minWidth: minWidth, every: every, inputs: inputs,
               visible: visible, elemId: elemId, elemClasses: elemClasses, render: render,
               buttons: buttons, key: key, preservedByKey: preservedByKey)
    { }

    protected override string GetMark() => "line";
}

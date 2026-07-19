using Gradio.Net.Components;


namespace Gradio.Net;

public sealed class Numpy : Dataframe
{
    public bool IsTemplate { get; } = true;

    public Numpy(
        object value = null,
        List<string> headers = null,
        object rowCount = null,
        (int? Min, int? Max)? rowLimits = null,
        object colCount = null,
        object columnCount = null,
        (int? Min, int? Max)? columnLimits = null,
        object datatype = null,
        string type = "numpy",
        List<Dictionary<string, object>> latexDelimiters = null,
        string label = null,
        bool? showLabel = null,
        object every = null,
        object inputs = null,
        int maxHeight = 500,
        int? scale = null,
        int minWidth = 160,
        bool? interactive = null,
        object visible = null,
        string elemId = null,
        object elemClasses = null,
        bool render = true,
        object key = null,
        object preservedByKey = null,
        bool wrap = false,
        bool lineBreaks = true,
        List<object> columnWidths = null,
        bool showRowNumbers = false,
        string showSearch = "none",
        List<int> staticColumns = null,
        int? pinnedColumns = null,
        int? maxChars = null,
        List<string> buttons = null)
        : base(
            value: value,
            headers: headers,
            rowCount: rowCount ?? (1, "dynamic"),
            rowLimits: rowLimits,
            columnCount: colCount ?? columnCount,
            columnLimits: columnLimits,
            datatype: datatype ?? "str",
            type: type,
            latexDelimiters: latexDelimiters,
            label: label,
            showLabel: showLabel,
            every: every,
            inputs: inputs,
            maxHeight: maxHeight,
            scale: scale,
            minWidth: minWidth,
            interactive: interactive,
            visible: visible,
            elemId: elemId,
            elemClasses: elemClasses,
            render: render,
            key: key,
            preservedByKey: preservedByKey,
            wrap: wrap,
            lineBreaks: lineBreaks,
            columnWidths: columnWidths,
            buttons: buttons,
            showRowNumbers: showRowNumbers,
            maxChars: maxChars,
            showSearch: showSearch,
            pinnedColumns: pinnedColumns,
            staticColumns: staticColumns)
    {
    }
}

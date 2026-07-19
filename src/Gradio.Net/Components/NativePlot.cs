using System.Collections;
using System.Data;

namespace Gradio.Net.Components;

[Events.Event("select")]
[Events.Event("double_click")]
public class NativePlot : Component
{
    public string? X { get; set; }
    public string? Y { get; set; }
    public string? Color { get; set; }
    public string? Title { get; set; }
    public string? XTitle { get; set; }
    public string? YTitle { get; set; }
    public string? ColorTitle { get; set; }
    public object? XBin { get; set; }
    public string? YAggregate { get; set; }
    public Dictionary<string, string>? ColorMap { get; set; }
    public List<string>? ColorsInLegend { get; set; }
    public List<double?>? XLim { get; set; }
    public List<double?>? YLim { get; set; }
    public double XLabelAngle { get; set; }
    public double YLabelAngle { get; set; }
    public object XAxisLabelsVisible { get; set; }
    public string? Caption { get; set; }
    public object? Sort { get; set; }
    public object Tooltip { get; set; }
    public int? Height { get; set; }
    public List<string>? Buttons { get; set; }

    public NativePlot(
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
    {
        X = x;
        Y = y;
        Color = color;
        Title = title;
        XTitle = xTitle;
        YTitle = yTitle;
        ColorTitle = colorTitle;
        XBin = xBin;
        YAggregate = yAggregate;
        ColorMap = colorMap;
        ColorsInLegend = colorsInLegend;
        XLim = xLim;
        YLim = yLim;
        XLabelAngle = xLabelAngle;
        YLabelAngle = yLabelAngle;
        XAxisLabelsVisible = xAxisLabelsVisible ?? true;
        Caption = caption;
        Sort = sort;
        Tooltip = tooltip ?? "axis";
        Height = height;
        Buttons = buttons;

        if (label == null && showLabel == null)
        {
            showLabel = false;
        }

        Label = label;
        ShowLabel = showLabel ?? ShowLabel;
        Container = container;
        Scale = scale;
        MinWidth = minWidth;
        if (visible is bool vb) Visible = vb;
        ElemId = elemId;
        ElemClasses = elemClasses switch
        {
            string one => new List<string> { one },
            List<string> many => many,
            _ => ElemClasses
        };
        Key = key;
        PreservedByKey = preservedByKey switch
        {
            string one => new List<string> { one },
            List<string> many => many,
            _ => PreservedByKey
        };

        var inputBlocks = ExtractNativePlotInputBlocks(inputs);
        if (value is Delegate loadDelegate)
        {
            object? initialValue = null;
            if (inputBlocks.Length == 0)
            {
                try { initialValue = loadDelegate.DynamicInvoke(); }
                catch { /* ignore invoke errors when inputs are not yet available */ }
            }
            Value = initialValue != null ? Postprocess(initialValue) : null;
            AttachLoadEvent(loadDelegate, every, inputBlocks);
        }
        else
        {
            Value = value != null ? Postprocess(value) : null;
        }
    }

    private static Core.Block[] ExtractNativePlotInputBlocks(object? inputs)
    {
        if (inputs == null) return Array.Empty<Core.Block>();
        if (inputs is Core.Block single) return new[] { single };
        if (inputs is System.Collections.IEnumerable e and not string)
        {
            var list = new List<Core.Block>();
            foreach (var item in e)
                if (item is Core.Block b) list.Add(b);
            return list.ToArray();
        }
        return Array.Empty<Core.Block>();
    }

    public override string GetBlockName() => "nativeplot";

    protected virtual string GetMark() => "native";

    public override object? Preprocess(object? input)
    {
        return input;
    }

    public override object? Postprocess(object? value)
    {
        // Python: None or dict is returned directly (update semantics).
        if (value == null || value is Dictionary<string, object>)
        {
            return value;
        }

        if (value is NativePlotData np)
        {
            np.Mark = GetMark();
            return np;
        }

        if (TryFromDataTable(value, out var fromTable))
        {
            fromTable.Mark = GetMark();
            return fromTable;
        }

        if (TryFromRows(value, out var fromRows))
        {
            fromRows.Mark = GetMark();
            return fromRows;
        }

        // Best-effort fallback for unknown tabular objects.
        return value;
    }

    public override object ExamplePayload() => null!;

    public override object ExampleValue()
    {
        var xCol = X ?? "x";
        var yCol = Y ?? "y";
        return new List<Dictionary<string, object>>
        {
            new() { [xCol] = 1, [yCol] = 4 },
            new() { [xCol] = 2, [yCol] = 5 },
            new() { [xCol] = 3, [yCol] = 6 }
        };
    }

    public override Dictionary<string, object> ApiInfo()
    {
        return new Dictionary<string, object>
        {
            ["type"] = new Dictionary<string, object>(),
            ["description"] = "any valid json"
        };
    }

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["x"] = X;
        config["y"] = Y;
        config["color"] = Color;
        config["title"] = Title;
        config["x_title"] = XTitle;
        config["y_title"] = YTitle;
        config["color_title"] = ColorTitle;
        config["x_bin"] = XBin;
        config["y_aggregate"] = YAggregate;
        config["color_map"] = ColorMap;
        config["colors_in_legend"] = ColorsInLegend;
        config["x_lim"] = XLim;
        config["y_lim"] = YLim;
        config["x_label_angle"] = XLabelAngle;
        config["y_label_angle"] = YLabelAngle;
        config["x_axis_labels_visible"] = XAxisLabelsVisible;
        config["caption"] = Caption;
        config["sort"] = Sort;
        config["tooltip"] = Tooltip;
        config["height"] = Height;
        config["buttons"] = Buttons;
        return config;
    }

    private static bool TryFromDataTable(object value, out NativePlotData data)
    {
        data = new NativePlotData();
        if (value is not DataTable table)
        {
            return false;
        }

        data.Columns = table.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
        data.Datatypes = table.Columns.Cast<DataColumn>()
            .ToDictionary(c => c.ColumnName, c => SimplifyType(c.DataType));

        foreach (DataRow row in table.Rows)
        {
            var vals = new List<object?>();
            foreach (DataColumn c in table.Columns)
            {
                vals.Add(ConvertDataValue(row[c]));
            }
            data.Data.Add(vals);
        }

        return true;
    }

    private static bool TryFromRows(object value, out NativePlotData data)
    {
        data = new NativePlotData();

        if (value is not IEnumerable enumerable || value is string)
        {
            return false;
        }

        var rows = new List<Dictionary<string, object?>>();
        foreach (var item in enumerable)
        {
            if (item is IDictionary<string, object> d)
            {
                rows.Add(d.ToDictionary(k => k.Key, v => (object?)v.Value));
            }
            else if (item is IDictionary ud)
            {
                var one = new Dictionary<string, object?>();
                foreach (DictionaryEntry de in ud)
                {
                    one[de.Key?.ToString() ?? string.Empty] = de.Value;
                }
                rows.Add(one);
            }
        }

        if (rows.Count == 0)
        {
            return false;
        }

        data.Columns = rows.SelectMany(r => r.Keys).Distinct().ToList();
        data.Datatypes = data.Columns.ToDictionary(
            c => c,
            c => InferType(rows.Select(r => r.TryGetValue(c, out var v) ? v : null)));

        foreach (var row in rows)
        {
            data.Data.Add(data.Columns.Select(c => ConvertDataValue(row.TryGetValue(c, out var v) ? v : null)).ToList());
        }

        return true;
    }

    private static string InferType(IEnumerable<object?> values)
    {
        foreach (var v in values)
        {
            if (v == null) continue;
            var t = v.GetType();
            return SimplifyType(t);
        }

        return "nominal";
    }

    private static string SimplifyType(Type t)
    {
        if (t == typeof(DateTime) || t == typeof(DateTimeOffset)) return "temporal";
        if (t == typeof(byte) || t == typeof(short) || t == typeof(int) || t == typeof(long) || t == typeof(float) || t == typeof(double) || t == typeof(decimal))
            return "quantitative";
        return "nominal";
    }

    private static object? ConvertDataValue(object? v)
    {
        if (v is DateTime dt)
        {
            // Convert to UTC before wrapping so Kind=Local is handled correctly.
            var utc = dt.ToUniversalTime();
            return new DateTimeOffset(utc).ToUnixTimeMilliseconds();
        }
        if (v is DateTimeOffset dto)
            return dto.ToUnixTimeMilliseconds();
        return v;
    }
}

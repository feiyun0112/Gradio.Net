using System.Collections;
using System.Text.Json;
using Gradio.Net.Core.Exceptions;

namespace Gradio.Net.Components;

[Events.Event("change")]
[Events.Event("input")]
[Events.Event("select")]
[Events.Event("edit")]
public class Dataframe : Component
{
    public List<string> Headers { get; set; }
    public (int Count, string Mode) RowCount { get; set; }
    public (int Count, string Mode) ColumnCount { get; set; }
    public (int? Min, int? Max)? RowLimits { get; set; }
    public (int? Min, int? Max)? ColumnLimits { get; set; }
    public List<int> StaticColumns { get; set; }
    public object Datatype { get; set; }
    public string Type { get; set; }
    public List<Dictionary<string, object>> LatexDelimiters { get; set; }
    public object MaxHeight { get; set; }
    public bool Wrap { get; set; }
    public bool LineBreaks { get; set; }
    public List<string> ColumnWidths { get; set; }
    public List<string>? Buttons { get; set; }
    public bool ShowRowNumbers { get; set; }
    public int? MaxChars { get; set; }
    public string ShowSearch { get; set; }
    public int? PinnedColumns { get; set; }

    public Dataframe(
        object? value = null,
        List<string>? headers = null,
        object? rowCount = null,
        (int? Min, int? Max)? rowLimits = null,
        object? columnCount = null,
        (int? Min, int? Max)? columnLimits = null,
        object? datatype = null,
        string type = "pandas",
        List<Dictionary<string, object>>? latexDelimiters = null,
        string? label = null,
        bool? showLabel = null,
        object? every = null,
        object? inputs = null,
        object? maxHeight = null,
        int? scale = null,
        int minWidth = 160,
        bool? interactive = null,
        object? visible = null,
        string? elemId = null,
        object? elemClasses = null,
        bool render = true,
        object? key = null,
        object? preservedByKey = null,
        bool wrap = false,
        bool lineBreaks = true,
        List<object>? columnWidths = null,
        List<string>? buttons = null,
        bool showRowNumbers = false,
        int? maxChars = null,
        string showSearch = "none",
        int? pinnedColumns = null,
        List<int>? staticColumns = null)
    {
        RowCount = ProcessCounts(rowCount);
        RowLimits = rowLimits;

        var defaultColumnCount = headers?.Count ?? 3;
        ColumnCount = ProcessCounts(columnCount, defaultColumnCount);
        ColumnLimits = columnLimits;

        ValidateHeaders(headers, ColumnCount.Count);

        Headers = headers ?? Enumerable.Range(1, ColumnCount.Count).Select(i => i.ToString()).ToList();
        StaticColumns = staticColumns ?? new List<int>();

        var validTypes = new[] { "pandas", "numpy", "array", "polars" };
        if (!validTypes.Contains(type))
        {
            throw new Error($"Invalid value for parameter `type`: {type}. Please choose from one of: [{string.Join(", ", validTypes)}]");
        }
        Type = type;

        Datatype = datatype ?? "str";
        if (Datatype is string dt && dt == "auto")
        {
            Datatype = SetAutoDatatype(value);
        }

        LatexDelimiters = latexDelimiters ?? new List<Dictionary<string, object>>
        {
            new()
            {
                ["left"] = "$$",
                ["right"] = "$$",
                ["display"] = true
            }
        };

        MaxHeight = maxHeight ?? 500;
        Wrap = wrap;
        LineBreaks = lineBreaks;
        ColumnWidths = (columnWidths ?? new List<object>())
            .Select(w => NormalizeColumnWidth(w))
            .ToList();
        Buttons = buttons;
        ShowRowNumbers = showRowNumbers;
        MaxChars = maxChars;
        ShowSearch = showSearch;
        PinnedColumns = pinnedColumns;

        Value = value;
        Label = label;
        ShowLabel = showLabel ?? ShowLabel;
        Scale = scale;
        MinWidth = minWidth;
        Interactive = interactive;
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
    }

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        if (Value != null)
        {
            var postprocessed = Postprocess(Value);
            if (postprocessed is DataframeData dd)
            {
                config["value"] = new Dictionary<string, object?>
                {
                    ["headers"] = (object)dd.Headers,
                    ["data"] = dd.Data,
                    ["metadata"] = dd.Metadata
                };
            }
            else
            {
                config["value"] = postprocessed;
            }
        }
        config["headers"] = Headers;
        config["row_count"] = new object[] { RowCount.Count, RowCount.Mode };
        config["row_limits"] = RowLimits == null ? null : new object?[] { RowLimits.Value.Min, RowLimits.Value.Max };
        config["column_count"] = new object[] { ColumnCount.Count, ColumnCount.Mode };
        config["col_count"] = new object[] { ColumnCount.Count, ColumnCount.Mode };
        config["column_limits"] = ColumnLimits == null ? null : new object?[] { ColumnLimits.Value.Min, ColumnLimits.Value.Max };
        config["datatype"] = Datatype;
        config["type"] = Type;
        config["latex_delimiters"] = LatexDelimiters;
        config["max_height"] = MaxHeight;
        config["wrap"] = Wrap;
        config["line_breaks"] = LineBreaks;
        config["column_widths"] = ColumnWidths;
        config["buttons"] = Buttons;
        config["show_row_numbers"] = ShowRowNumbers;
        config["max_chars"] = MaxChars;
        config["show_search"] = ShowSearch;
        config["pinned_columns"] = PinnedColumns;
        config["static_columns"] = StaticColumns;
        return config;
    }

    public override object? Preprocess(object? payload)
    {
        if (payload == null)
        {
            return null;
        }

        var data = ParsePayload(payload);

        // Python returns pandas/numpy/polars for selected modes.
        // Best-effort in C#: return list-of-lists representation for all modes.
        if (Type is "array" or "numpy" or "pandas" or "polars")
        {
            return data.Data;
        }

        throw new Error($"Unknown type: {Type}. Please choose from: 'pandas', 'numpy', 'array', 'polars'.");
    }

    public override object Postprocess(object output)
    {
        var headers = GetHeaders(output);
        if (headers.Count == 0)
        {
            headers = new List<object>(Headers.Cast<object>());
        }

        var data = IsEmpty(output) ? new List<List<object?>>() : GetCellData(output);
        if (data.Count == 0)
        {
            return new DataframeData { Headers = headers, Data = data, Metadata = null };
        }

        var colCount = data[0].Count;
        if (headers.Count > colCount)
        {
            headers = headers.Take(colCount).ToList();
        }
        else if (headers.Count < colCount)
        {
            for (var i = headers.Count + 1; i <= colCount; i++)
            {
                headers.Add(i.ToString());
            }
        }

        return new DataframeData
        {
            Headers = headers,
            Data = data,
            Metadata = GetMetadata(output)
        };
    }

    public override Dictionary<string, object> ApiInfo()
    {
        return new Dictionary<string, object>
        {
            ["type"] = "object",
            ["description"] = "Tabular data payload with headers and 2D data rows"
        };
    }

    public override object ProcessExample(object value)
    {
        if (value == null)
        {
            return string.Empty;
        }

        var pp = Postprocess(value) as DataframeData;
        if (pp == null)
        {
            return new List<List<object?>>();
        }

        return pp.Data.Take(5).ToList();
    }

    public override object ExamplePayload()
    {
        return new Dictionary<string, object>
        {
            ["headers"] = new List<string> { "a", "b" },
            ["data"] = new List<List<string>> { new() { "foo", "bar" } }
        };
    }

    public override object ExampleValue()
    {
        return new Dictionary<string, object>
        {
            ["headers"] = new List<string> { "a", "b" },
            ["data"] = new List<List<string>> { new() { "foo", "bar" } }
        };
    }

    private static (int Count, string Mode) ProcessCounts(object? count, int defaultCount = 3)
    {
        if (count == null)
        {
            return (defaultCount, "dynamic");
        }

        if (count is int i)
        {
            return (i, "dynamic");
        }

        if (count is long l)
        {
            return ((int)l, "dynamic");
        }

        if (count is double d)
        {
            return ((int)d, "dynamic");
        }

        if (count is float f)
        {
            return ((int)f, "dynamic");
        }

        if (count is ValueTuple<int, string> vt)
        {
            return vt;
        }

        if (count is Tuple<int, string> t)
        {
            return (t.Item1, t.Item2);
        }

        if (count is object[] arr && arr.Length >= 2)
        {
            return (Convert.ToInt32(arr[0]), arr[1]?.ToString() ?? "dynamic");
        }

        if (count is IList list && list.Count >= 2)
        {
            return (Convert.ToInt32(list[0]), list[1]?.ToString() ?? "dynamic");
        }

        return (defaultCount, "dynamic");
    }

    private static void ValidateHeaders(List<string>? headers, int columnCount)
    {
        if (headers != null && headers.Count != columnCount)
        {
            throw new Error(
                $"The length of the headers list must be equal to the column_count. The column count is set to {columnCount} but `headers` has {headers.Count} items.");
        }
    }

    private static string NormalizeColumnWidth(object width)
    {
        if (width is string s)
        {
            if (s.EndsWith("px", StringComparison.OrdinalIgnoreCase)
                || s.EndsWith("%", StringComparison.OrdinalIgnoreCase)
                || string.Equals(s, "auto", StringComparison.OrdinalIgnoreCase))
            {
                return s;
            }

            return $"{s}px";
        }

        return $"{width}px";
    }

    private static object SetAutoDatatype(object? value)
    {
        var rows = GetCellData(value);
        if (rows.Count == 0)
        {
            return "str";
        }

        var firstRow = rows[0];
        var dtypes = new List<string>();
        foreach (var cell in firstRow)
        {
            dtypes.Add(cell switch
            {
                null => "str",
                bool => "bool",
                byte or sbyte or short or ushort or int or uint or long or ulong or float or double or decimal => "number",
                DateTime or DateTimeOffset => "date",
                _ => "str"
            });
        }
        return dtypes;
    }

    private static DataframeData ParsePayload(object payload)
    {
        if (payload is DataframeData dd)
        {
            return dd;
        }

        if (payload is Dictionary<string, object> dict)
        {
            return new DataframeData
            {
                Headers = ToObjectList(dict.TryGetValue("headers", out var headers) ? headers : null),
                Data = To2DList(dict.TryGetValue("data", out var data) ? data : null),
                Metadata = dict.TryGetValue("metadata", out var md) && md is Dictionary<string, object> mdObj ? mdObj : null
            };
        }

        // Handle System.Text.Json.JsonElement (deserialized from HTTP request body)
        if (payload is JsonElement je && je.ValueKind == JsonValueKind.Object)
        {
            var headers = new List<object>();
            if (je.TryGetProperty("headers", out var headersEl) && headersEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var h in headersEl.EnumerateArray())
                {
                    headers.Add(JsonElementToClr(h) ?? string.Empty);
                }
            }

            var data = new List<List<object?>>();
            if (je.TryGetProperty("data", out var dataEl) && dataEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var row in dataEl.EnumerateArray())
                {
                    var rowData = new List<object?>();
                    if (row.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var cell in row.EnumerateArray())
                        {
                            rowData.Add(JsonElementToClr(cell));
                        }
                    }
                    data.Add(rowData);
                }
            }

            Dictionary<string, object>? metadata = null;
            if (je.TryGetProperty("metadata", out var metaEl) && metaEl.ValueKind == JsonValueKind.Object)
            {
                metadata = new Dictionary<string, object>();
                foreach (var prop in metaEl.EnumerateObject())
                {
                    var val = JsonElementToClr(prop.Value);
                    if (val != null)
                        metadata[prop.Name] = val;
                }
            }

            return new DataframeData { Headers = headers, Data = data, Metadata = metadata };
        }

        throw new Error("Dataframe payload must be DataframeData or a dictionary with 'headers' and 'data'.");
    }

    private static object? JsonElementToClr(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number when element.TryGetInt64(out var l) => (object)l,
            JsonValueKind.Number => (object)element.GetDouble(),
            JsonValueKind.True => (object)true,
            JsonValueKind.False => (object)false,
            JsonValueKind.Null => null,
            JsonValueKind.Array => element.EnumerateArray()
                .Select(e => JsonElementToClr(e))
                .ToList<object?>(),
            JsonValueKind.Object => element.EnumerateObject()
                .ToDictionary(p => p.Name, p => JsonElementToClr(p.Value)),
            _ => element.ToString()
        };
    }

    private static bool IsEmpty(object? value)
    {
        if (value == null)
        {
            return true;
        }

        if (value is Dictionary<string, object> dict && dict.TryGetValue("data", out var data))
        {
            return To2DList(data).Count == 0;
        }

        if (value is IEnumerable enumerable && value is not string)
        {
            foreach (var _ in enumerable)
            {
                return false;
            }
            return true;
        }

        return false;
    }

    private static List<object> GetHeaders(object? value)
    {
        if (value is DataframeData dd)
        {
            return dd.Headers;
        }

        if (value is Dictionary<string, object> dict)
        {
            return ToObjectList(dict.TryGetValue("headers", out var headers) ? headers : null);
        }

        return new List<object>();
    }

    private static List<List<object?>> GetCellData(object? value)
    {
        if (value == null)
        {
            return new List<List<object?>>();
        }

        if (value is DataframeData dd)
        {
            return dd.Data;
        }

        if (value is Dictionary<string, object> dict)
        {
            return To2DList(dict.TryGetValue("data", out var data) ? data : null);
        }

        if (value is IEnumerable enumerable && value is not string)
        {
            var rows = new List<List<object?>>();
            foreach (var row in enumerable)
            {
                if (row is IEnumerable rowEnum && row is not string)
                {
                    var outRow = new List<object?>();
                    foreach (var cell in rowEnum)
                    {
                        outRow.Add(FormatCell(cell));
                    }
                    rows.Add(outRow);
                }
                else
                {
                    rows.Add(new List<object?> { FormatCell(row) });
                }
            }
            return rows;
        }

        throw new Error($"Cannot process value of type {value.GetType()} in gr.Dataframe");
    }

    private static object? FormatCell(object? cell)
    {
        return cell switch
        {
            DateTime dt => FormatDateTime(dt.Year, dt.Month, dt.Day,
                dt.Hour, dt.Minute, dt.Second,
                (int)((dt.Ticks % TimeSpan.TicksPerSecond) / 10)),
            DateTimeOffset dto => FormatDateTime(dto.Year, dto.Month, dto.Day,
                dto.Hour, dto.Minute, dto.Second,
                (int)((dto.Ticks % TimeSpan.TicksPerSecond) / 10)),
            _ => cell
        };
    }

    private static string FormatDateTime(int y, int mo, int d, int h, int mi, int s, int us)
    {
        var date = $"{y:D4}-{mo:D2}-{d:D2} {h:D2}:{mi:D2}:{s:D2}";
        return us == 0 ? date : $"{date}.{us:D6}";
    }

    private static Dictionary<string, object>? GetMetadata(object? value)
    {
        if (value is DataframeData dd)
        {
            return dd.Metadata;
        }

        if (value is Dictionary<string, object> dict)
        {
            return dict.TryGetValue("metadata", out var meta) && meta is Dictionary<string, object> m ? m : null;
        }

        return null;
    }

    private static List<object> ToObjectList(object? value)
    {
        var result = new List<object>();
        if (value == null)
        {
            return result;
        }

        if (value is IEnumerable enumerable && value is not string)
        {
            foreach (var item in enumerable)
            {
                result.Add(item!);
            }
            return result;
        }

        result.Add(value);
        return result;
    }

    private static List<List<object?>> To2DList(object? value)
    {
        var rows = new List<List<object?>>();
        if (value == null)
        {
            return rows;
        }

        if (value is IEnumerable enumerable && value is not string)
        {
            foreach (var row in enumerable)
            {
                if (row is IEnumerable rowEnum && row is not string)
                {
                    var outRow = new List<object?>();
                    foreach (var cell in rowEnum)
                    {
                        outRow.Add(cell);
                    }
                    rows.Add(outRow);
                }
                else
                {
                    rows.Add(new List<object?> { row });
                }
            }
        }

        return rows;
    }
}

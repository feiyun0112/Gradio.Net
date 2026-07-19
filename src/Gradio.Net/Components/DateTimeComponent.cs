using System.Globalization;
using System.Text.RegularExpressions;
using Gradio.Net.Core.Exceptions;

namespace Gradio.Net.Components;

[Events.Event("change")]
[Events.Event("submit")]
public class DateTimeComponent : FormComponent
{
    public bool IncludeTime { get; set; }
    public string Type { get; set; }
    public string? Timezone { get; set; }
    public string TimeFormat { get; set; }
    public List<object> Buttons { get; set; } = new();

    public DateTimeComponent(
        object? value = null,
        bool includeTime = true,
        string type = "timestamp",
        string? timezone = null,
        string? label = null,
        bool? showLabel = null,
        string? info = null,
        object? every = null,
        int? scale = null,
        int minWidth = 160,
        object? visible = null,
        bool? interactive = null,
        string? elemId = null,
        object? elemClasses = null,
        bool render = true,
        object? key = null,
        object? preservedByKey = null)
    {
        var validTypes = new[] { "timestamp", "datetime", "string" };
        if (Array.IndexOf(validTypes, type) < 0)
        {
            throw new Error($"Invalid value for parameter `type`: {type}. Please choose from one of: [{string.Join(", ", validTypes)}]");
        }

        Type = type;
        IncludeTime = includeTime;
        Timezone = timezone;
        TimeFormat = IncludeTime ? "yyyy-MM-dd HH:mm:ss" : "yyyy-MM-dd";

        Value = value;
        Label = label;
        ShowLabel = showLabel ?? ShowLabel;
        Info = info;
        Scale = scale;
        MinWidth = minWidth;
        Interactive = interactive ?? true;
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

    public override string GetBlockName() => "datetime";
    public override string GetBlockClass() => "datetime";

    public override IReadOnlyList<string> KeepNullProps =>
        new[] { "timezone", "info", "scale", "elem_id" };

    public override Dictionary<string, object> GetConfig(Type? cls = null)
    {
        var config = base.GetConfig(cls);
        config["include_time"] = IncludeTime;
        config["type"] = Type;
        config["timezone"] = Timezone;
        config["buttons"] = Buttons;
        return config;
    }

    public override object? Preprocess(object? payload)
    {
        var text = payload?.ToString();
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        if (Type == "string" && !text.Contains("now", StringComparison.OrdinalIgnoreCase))
        {
            return text;
        }

        var dt = GetDateTimeFromString(text);

        if (Type == "string")
        {
            return dt.ToString(TimeFormat, CultureInfo.InvariantCulture);
        }

        if (Type == "datetime")
        {
            return dt;
        }

        // timestamp
        return new DateTimeOffset(dt).ToUnixTimeSeconds();
    }

    public override object? Postprocess(object? value)
    {
        if (value == null)
        {
            return null;
        }

        if (value is DateTime dt)
        {
            return dt.ToString(TimeFormat, CultureInfo.InvariantCulture);
        }

        if (value is string s)
        {
            return s;
        }

        if (TryToDouble(value, out var ts))
        {
            var offset = DateTimeOffset.FromUnixTimeSeconds((long)Math.Floor(ts));
            var zoned = ApplyTimezone(offset.DateTime);
            return zoned.ToString(TimeFormat, CultureInfo.InvariantCulture);
        }

        return value.ToString();
    }

    public override Dictionary<string, object> ApiInfo()
    {
        return new Dictionary<string, object>
        {
            ["type"] = "string",
            ["description"] = $"Formatted as YYYY-MM-DD{(IncludeTime ? " HH:MM:SS" : string.Empty)}"
        };
    }

    public override object ExamplePayload() => "2020-10-01 05:20:15";

    public override object ExampleValue() => "2020-10-01 05:20:15";

    public DateTime GetDateTimeFromString(string date)
    {
        var nowRegex = new Regex(@"^(?:\s*now\s*(?:-\s*(\d+)\s*([dmhs]))?)?\s*$", RegexOptions.IgnoreCase);

        if (date.Contains("now", StringComparison.OrdinalIgnoreCase))
        {
            var match = nowRegex.Match(date);
            if (!match.Success)
            {
                throw new Error("Invalid 'now' time format");
            }

            var num = match.Groups[1].Success ? int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture) : 0;
            var unit = match.Groups[2].Success ? match.Groups[2].Value.ToLowerInvariant() : "s";

            var delta = unit switch
            {
                "d" => TimeSpan.FromDays(num),
                "h" => TimeSpan.FromHours(num),
                "m" => TimeSpan.FromMinutes(num),
                _ => TimeSpan.FromSeconds(num)
            };

            return DateTime.Now - delta;
        }

        if (!DateTime.TryParseExact(date, TimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
        {
            throw new Error($"Invalid datetime value: {date}");
        }

        return ApplyTimezone(parsed);
    }

    private DateTime ApplyTimezone(DateTime dt)
    {
        if (string.IsNullOrWhiteSpace(Timezone))
        {
            return dt;
        }

        try
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(Timezone);
            return TimeZoneInfo.ConvertTime(dt, tz);
        }
        catch
        {
            // Keep local datetime if timezone id is not available on host OS.
            return dt;
        }
    }

    private static bool TryToDouble(object value, out double result)
    {
        switch (value)
        {
            case double d:
                result = d;
                return true;
            case float f:
                result = f;
                return true;
            case int i:
                result = i;
                return true;
            case long l:
                result = l;
                return true;
            case decimal m:
                result = (double)m;
                return true;
            default:
                return double.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out result);
        }
    }
}

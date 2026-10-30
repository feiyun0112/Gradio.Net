using System.Globalization;
using System.Text.RegularExpressions;
using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A date, and optionally a time of day.
/// </summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. What the component carries is text — the field is a
/// text field with a calendar beside it — while <see cref="Type"/> says what a handler receives:
/// the text, a <see cref="DateTime"/>, or a Unix timestamp in seconds.
/// </para>
/// <para>
/// <see cref="IncludeTime"/> changes both the text's shape and the format a handler is handed, so
/// the two are derived from it rather than configured separately.
/// </para>
/// </remarks>
[GradioBlock("datetime")]
[HandlerValue("object?", "change", "submit")]
public sealed partial class DateTime : FormComponent
{
    internal DateTime()
    {
    }

    private const string DateFormat = "yyyy-MM-dd";
    private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

    private string _type = "timestamp";
    private object? _value;

    /// <summary>The date and time the field starts on, or <see langword="null"/> for none.</summary>
    /// <value>
    /// The value as the text the field shows: a <see cref="System.DateTime"/> is formatted, a
    /// number is read as a Unix timestamp, and text is carried as it was given — which is the
    /// Python version's rule, and the reason a value that is already text is never reformatted.
    /// </value>
    /// <exception cref="ArgumentException">The value is not a date, a timestamp or text.</exception>
    public object? Value
    {
        get => _value switch
        {
            null => null,
            string text => text,
            System.DateTime moment => moment.ToString(Format, CultureInfo.InvariantCulture),
            DateTimeOffset moment => moment.ToString(Format, CultureInfo.InvariantCulture),
            double seconds => FromTimestamp(seconds),
            int seconds => FromTimestamp(seconds),
            long seconds => FromTimestamp(seconds),
            _ => throw new ArgumentException(
                $"A date and time's value is a date, a timestamp or text; got a {_value.GetType()}.",
                nameof(Value)),
        };
        set => _value = value;
    }

    /// <summary>Whether the field offers a time as well as a date.</summary>
    public bool IncludeTime { get; set; } = true;

    /// <summary>What a handler receives: <c>timestamp</c>, <c>datetime</c> or <c>string</c>.</summary>
    /// <value>
    /// <c>timestamp</c> unless the application says otherwise; a value the Python version does not
    /// offer is refused rather than passed on to a handler that cannot read it.
    /// </value>
    public string Type
    {
        get => _type;
        set
        {
            if (value is not ("timestamp" or "datetime" or "string"))
            {
                throw new ArgumentException(
                    $"A date and time's type must be timestamp, datetime or string; got '{value}'.",
                    nameof(value));
            }

            _type = value;
        }
    }

    /// <summary>The time zone a timestamp is read in, or <see langword="null"/> for this machine's.</summary>
    public string? Timezone { get; set; }

    /// <summary>The label drawn above the field.</summary>
    public override string? Label { get; set; }

    /// <summary>The note drawn under the label.</summary>
    public string? Info { get; set; }

    /// <summary>The relative width of the field next to its neighbours.</summary>
    public override int? Scale { get; set; }

    /// <summary>The width below which the field wraps.</summary>
    public override int? MinWidth { get; set; } = 160;

    /// <summary>The buttons drawn beside the field.</summary>
    public IReadOnlyList<string> Buttons { get; set; } = [];

    /// <inheritdoc />
    public override JsonObject ApiInfo => new()
    {
        ["type"] = "string",
        ["description"] = $"Formatted as YYYY-MM-DD{(IncludeTime ? " HH:MM:SS" : string.Empty)}",
    };

    /// <inheritdoc />
    public override JsonNode? ExampleInputs => JsonValue.Create("2020-10-01 05:20:15");

    /// <summary>The value a handler reads.</summary>
    /// <param name="value">The text the client sent.</param>
    /// <returns>The text, a <see cref="System.DateTime"/>, or a timestamp, as <see cref="Type"/> says.</returns>
    /// <exception cref="ArgumentException">The text is not a date in the field's format.</exception>
    /// <remarks>
    /// Text is only reformatted when the component's type says a handler wants a date or a
    /// timestamp: a handler that asked for text is handed exactly what was typed, which is what
    /// the Python version does and what makes "the user's own spelling" reachable.
    /// </remarks>
    public override object? Preprocess(JsonNode? value)
    {
        var text = JsonValues.Text(value);

        if (text is null || string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        // Text a handler asked for is handed over as it was typed — unless it is relative: "now" has
        // to be resolved to a moment before it can be written in the field's own format.
        if (Type == "string" && !text.Contains("now", StringComparison.Ordinal))
        {
            return text;
        }

        var moment = ParseMoment(text);

        return Type switch
        {
            "string" => moment.ToString(Format, CultureInfo.InvariantCulture),
            "datetime" => moment,
            _ => ToTimestamp(moment),
        };
    }

    /// <summary>The text the client receives from what a handler returned.</summary>
    /// <param name="value">A date, a timestamp or text.</param>
    /// <returns>The text the field shows, or <see langword="null"/>.</returns>
    public override JsonNode? Postprocess(object? value) => value switch
    {
        null => null,
        string text => JsonValue.Create(text),
        System.DateTime moment => JsonValue.Create(moment.ToString(Format, CultureInfo.InvariantCulture)),
        DateTimeOffset moment => JsonValue.Create(moment.ToString(Format, CultureInfo.InvariantCulture)),
        int seconds => JsonValue.Create(FromTimestamp(seconds)),
        long seconds => JsonValue.Create(FromTimestamp(seconds)),
        double seconds => JsonValue.Create(FromTimestamp(seconds)),
        _ => throw new ArgumentException(
            $"A date and time's value is a date, a timestamp or text; got a {value.GetType()}.",
            nameof(value)),
    };

    /// <summary>The format the field's text has.</summary>
    private string Format => IncludeTime ? DateTimeFormat : DateFormat;

    /// <summary>The same format as Python spells it, for a message that has to read like the Python
    /// version's.</summary>
    /// <remarks>
    /// The two spellings describe one format: <c>%Y-%m-%d</c> is <c>yyyy-MM-dd</c>. Only the
    /// message needs Python's, because that is what <c>strptime</c>'s own refusal says.
    /// </remarks>
    private string PythonFormat => IncludeTime ? "%Y-%m-%d %H:%M:%S" : "%Y-%m-%d";

    /// <summary>A timestamp read in the component's time zone.</summary>
    /// <param name="seconds">Seconds since the Unix epoch.</param>
    /// <returns>The moment as the field spells it.</returns>
    private string FromTimestamp(double seconds)
    {
        var moment = DateTimeOffset.FromUnixTimeSeconds((long)seconds);
        var zone = Zone();

        return (zone is null ? moment.LocalDateTime : TimeZoneInfo.ConvertTime(moment, zone).DateTime)
            .ToString(Format, CultureInfo.InvariantCulture);
    }

    /// <summary>Text read as a moment, relative or absolute.</summary>
    /// <param name="text">The text, in the field's format or spelled as <c>now</c>.</param>
    /// <returns>The moment.</returns>
    /// <exception cref="ArgumentException">The text is neither.</exception>
    /// <remarks>
    /// The Python version also accepts a relative moment — <c>now</c>, <c>now-5d</c>,
    /// <c>now-30m</c> — which is how a page that shows "the last five days" is written without
    /// computing the date itself. The units are days, hours, minutes and seconds, and a spelling
    /// that is not one of them is refused rather than read as an absolute date.
    /// </remarks>
    private System.DateTime ParseMoment(string text)
    {
        if (!text.Contains("now", StringComparison.Ordinal))
        {
            return Parse(text);
        }

        var match = NowPattern().Match(text);

        if (!match.Success)
        {
            // The Python version's message, which is what a user who typed "now-5w" is shown.
            throw new ArgumentException("Invalid 'now' time format", nameof(text));
        }

        var amount = match.Groups[1].Success
            ? int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture)
            : 0;

        var delta = match.Groups[2].Value switch
        {
            "d" => TimeSpan.FromDays(amount),
            "h" => TimeSpan.FromHours(amount),
            "m" => TimeSpan.FromMinutes(amount),
            _ => TimeSpan.FromSeconds(amount),
        };

        return System.DateTime.SpecifyKind(System.DateTime.Now, DateTimeKind.Unspecified).Subtract(delta);
    }

    /// <summary>The shape of a relative moment.</summary>
    /// <returns>The pattern, generated at build time.</returns>
    [GeneratedRegex(@"^(?:\s*now\s*(?:-\s*(\d+)\s*([dmhs]))?)?\s*$", RegexOptions.CultureInvariant)]
    private static partial Regex NowPattern();

    /// <summary>Text read as a moment in the field's format.</summary>
    /// <param name="text">The text.</param>
    /// <returns>The moment.</returns>
    /// <exception cref="ArgumentException">The text is not in the field's format.</exception>
    /// <remarks>
    /// The refusal is the one <c>datetime.strptime</c> raises, word for word: the Python version
    /// reads the text with it, so a value the field cannot parse is reported with the format it
    /// expected. The format in the message is Python's spelling of it — <c>%Y-%m-%d</c> — rather
    /// than the .NET one, because that is what the Python version's message says.
    /// </remarks>
    private System.DateTime Parse(string text) =>
        System.DateTime.TryParseExact(
            text,
            Format,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var moment)
            ? moment
            : throw new ArgumentException(
                $"time data {PythonText.Repr(text)} does not match format {PythonText.Repr(PythonFormat)}",
                nameof(text));

    /// <summary>The time zone the component reads timestamps in.</summary>
    /// <returns>The zone, or <see langword="null"/> for this machine's.</returns>
    /// <exception cref="ArgumentException">The application named a zone this machine does not know.</exception>
    private TimeZoneInfo? Zone()
    {
        if (Timezone is null)
        {
            return null;
        }

        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(Timezone);
        }
        catch (TimeZoneNotFoundException error)
        {
            throw new ArgumentException(
                $"'{Timezone}' is not a time zone this machine knows.",
                nameof(Timezone),
                error);
        }
        catch (InvalidTimeZoneException error)
        {
            throw new ArgumentException(
                $"'{Timezone}' is not a usable time zone on this machine.",
                nameof(Timezone),
                error);
        }
    }

    /// <summary>A moment read as a Unix timestamp in the component's time zone.</summary>
    /// <param name="moment">The moment, as the field spells it.</param>
    /// <returns>Seconds since the Unix epoch.</returns>
    /// <remarks>
    /// The offset is asked for at the moment itself rather than at the current time, so that a date
    /// on the other side of a daylight-saving change is read with the offset it actually had.
    /// </remarks>
    private long ToTimestamp(System.DateTime moment)
    {
        var offset = Zone()?.GetUtcOffset(moment) ?? TimeZoneInfo.Local.GetUtcOffset(moment);

        return new DateTimeOffset(moment, offset).ToUnixTimeSeconds();
    }
}

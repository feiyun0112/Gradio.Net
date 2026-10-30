using System.Globalization;
using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>A numeric input.</summary>
/// <remarks>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. The Python version types the value as a float, so
/// <see cref="Value"/> is one too: a whole number is written as one, which is what the Python
/// version writes for it.
/// </remarks>
[GradioBlock("number")]
[HandlerValue("double", "change", "input", "submit", "focus", "blur")]
public sealed partial class Number : FormComponent
{
    internal Number()
    {
    }

    private double? _value;

    /// <summary>
    /// The number the component starts with, or <see langword="null"/> for an empty one.
    /// </summary>
    /// <value>
    /// The value rounded to <see cref="Precision"/>, which is what the Python version carries: it
    /// rounds the value when the component is built, so the config holds the rounded number rather
    /// than the one the application passed.
    /// </value>
    public double? Value
    {
        get => Round(_value);
        set => _value = value;
    }

    /// <summary>The label drawn above the field.</summary>
    public override string? Label { get; set; }

    /// <summary>The hint shown while the field is empty.</summary>
    public string? Placeholder { get; set; }

    /// <summary>The note drawn under the field.</summary>
    public string? Info { get; set; }

    /// <summary>The relative width of the field next to its neighbours.</summary>
    public override int? Scale { get; set; }

    /// <summary>The width below which the field wraps.</summary>
    public override int? MinWidth { get; set; } = 160;

    /// <summary>The buttons drawn inside the field.</summary>
    public IReadOnlyList<string> Buttons { get; set; } = [];

    /// <summary>How many decimal places the value is rounded to.</summary>
    public int? Precision { get; set; }

    /// <summary>The smallest value the field accepts.</summary>
    public double? Minimum { get; set; }

    /// <summary>The largest value the field accepts.</summary>
    public double? Maximum { get; set; }

    /// <summary>The step the field's spinner moves by.</summary>
    public double Step { get; set; } = 1;

    /// <summary>The typing information of the field, which narrows to an integer at zero precision.</summary>
    /// <value>
    /// <c>integer</c> when <see cref="Precision"/> is zero — a value rounded to no decimals is
    /// whole — and <c>number</c> otherwise, which is the Python version's rule.
    /// </value>
    public override JsonObject ApiInfo => new()
    {
        ["type"] = Precision == 0 ? "integer" : "number",
    };

    /// <summary>
    /// A representative value of the field.
    /// </summary>
    /// <value>
    /// The smallest value the field accepts, or three when it accepts anything, rounded to the
    /// field's precision — the Python version's example is the bottom of the range rather than an
    /// arbitrary number, so that it is always a value the field would take.
    /// </value>
    /// <remarks>
    /// The example is a whole number when the field rounds to no decimals and the value it settled
    /// on is whole, because that is the type the Python version's is: no precision leaves the value
    /// as it was given, and a precision of zero converts it to an integer outright. The difference
    /// is observable in the command-line snippet, which writes <c>3</c> for an integer and
    /// <c>3.0</c> for a float.
    /// </remarks>
    public override JsonNode? ExampleInputs
    {
        get
        {
            var example = Round(Minimum ?? 3)!.Value;

            return (Precision is null or 0) && example == Math.Floor(example) && Math.Abs(example) <= int.MaxValue
                ? JsonValue.Create((int)example)
                : JsonValue.Create(example);
        }
    }

    /// <summary>The number a handler reads.</summary>
    /// <param name="value">The number the client sent.</param>
    /// <returns>
    /// The number rounded to <see cref="Precision"/>, or <see langword="null"/>; a whole number when
    /// the precision is zero.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The number is outside <see cref="Minimum"/>..<see cref="Maximum"/>.
    /// </exception>
    /// <remarks>
    /// The bounds are checked on the way in rather than clamped: the Python version refuses a value
    /// the field said it would not take, and answering with a different number would hide the
    /// disagreement rather than report it.
    /// </remarks>
    public override object? Preprocess(JsonNode? value)
    {
        if (value is null)
        {
            return null;
        }

        var number = JsonValues.Number(value)
            ?? throw new ArgumentException(
                $"A number field's value is a number; the client sent '{value.ToJsonString()}'.",
                nameof(value));

        RaiseIfOutOfBounds(number, Minimum, Maximum);

        return Rounded(number, Precision);
    }

    /// <summary>The number the client receives from what a handler returned.</summary>
    /// <param name="value">The number, or <see langword="null"/> for none.</param>
    /// <returns>The number rounded to <see cref="Precision"/>.</returns>
    /// <exception cref="ArgumentException"><paramref name="value"/> is not a number.</exception>
    /// <remarks>
    /// A number is rounded on the way out as well as on the way in, which is the Python version's
    /// rule: what a handler returns is what the field shows, and a field that rounds what it is
    /// given would show more decimals than it promised.
    /// </remarks>
    public override JsonNode? Postprocess(object? value)
    {
        if (value is null)
        {
            return null;
        }

        if (value is not (int or long or double or float or decimal))
        {
            throw new ArgumentException(
                $"A number field's value is a number; the handler returned a {value.GetType()}.",
                nameof(value));
        }

        return JsonValues.FromObject(
            Rounded(Convert.ToDouble(value, CultureInfo.InvariantCulture), Precision));
    }

    /// <summary>A cell of a flagging log read back as this component's value.</summary>
    /// <param name="cell">The cell.</param>
    /// <returns>The number the cell holds.</returns>
    /// <remarks>
    /// A number is written as text, and a negative one is written with an apostrophe in front so
    /// that a spreadsheet does not read it as a formula; reading it back has to undo both.
    /// </remarks>
    public override JsonNode? ReadFromFlag(string cell) => ParseEscapedJson(cell);

    /// <summary>Refuses a number outside a range.</summary>
    /// <param name="value">The number.</param>
    /// <param name="minimum">The bottom of the range, or <see langword="null"/> for none.</param>
    /// <param name="maximum">The top of the range, or <see langword="null"/> for none.</param>
    /// <exception cref="ArgumentOutOfRangeException">The number is outside the range.</exception>
    internal static void RaiseIfOutOfBounds(double value, double? minimum, double? maximum)
    {
        if (minimum is not null && value < minimum.Value)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                $"Value {value} is less than minimum value {minimum}.");
        }

        if (maximum is not null && value > maximum.Value)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                $"Value {value} is greater than maximum value {maximum}.");
        }
    }

    /// <summary>Rounds a value the way the Python version rounds one.</summary>
    /// <param name="value">The value to round.</param> <returns>The value rounded to
    /// <see cref="Precision"/>, unchanged when precision is unset.</returns>
    /// <remarks>
    /// Zero precision means a whole number rather than a float with no decimals, which is why the
    /// rounded value is truncated to an integer in that case; both sides round halves to even.
    /// </remarks>
    private double? Round(double? value) => RoundToPrecision(value, Precision);

    /// <summary>
    /// Rounds a value to a precision, the way the Python version rounds one.
    /// </summary>
    /// <param name="value">The value to round.</param>
    /// <param name="precision">The number of decimals, or <see langword="null"/> for none.</param>
    /// <returns>The rounded value, unchanged when it or the precision is absent.</returns>
    /// <remarks>
    /// Shared with the slider, which rounds its value the same way and says so in the Python
    /// version's own terms — it calls the number component's rule rather than having a second one.
    /// </remarks>
    internal static double? RoundToPrecision(double? value, int? precision)
    {
        if (value is null || precision is null)
        {
            return value;
        }

        return precision.Value == 0
            ? Math.Round(value.Value)
            : Math.Round(value.Value, precision.Value);
    }

    /// <summary>The number a handler reads, rounded the way the Python version rounds
    /// one.</summary> <param name="value">The number the client sent.</param>
    /// <param name="precision">The number of decimals, or <see langword="null"/> for none.</param>
    /// <returns>
    /// A whole number when the precision is zero — the Python version's <c>int</c>, which is why a
    /// handler may declare one — and a <see cref="double"/> otherwise.
    /// </returns>
    /// <remarks>
    /// The two types are not interchangeable to a handler: the Python version's
    /// <c>round_to_precision</c> returns <c>int(round(num, 0))</c> at zero precision, so a Python
    /// handler annotated <c>int</c> receives one. A number too large for a <see cref="long"/> keeps
    /// its fractional form rather than overflowing, which is the one place this cannot follow the
    /// Python version's unbounded integer.
    /// </remarks>
    internal static object? Rounded(double value, int? precision)
    {
        if (precision != 0)
        {
            return RoundToPrecision(value, precision);
        }

        var whole = Math.Round(value);

        // Written as a branch rather than as a conditional expression on purpose: a `?:` with a
        // `long` on one side and a `double` on the other has `double` as its type, which would turn
        // the whole number back into one with a fractional part.
        if (whole is >= long.MinValue and <= long.MaxValue)
        {
            return (long)whole;
        }

        return whole;
    }
}

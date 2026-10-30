using System.Globalization;
using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A number chosen by dragging between two ends of a range.
/// </summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. Two of them are computed rather than stored, because
/// the Python version computes them where every argument is known: the <em>step</em>, which
/// defaults to a hundredth of the range so that a slider has a sensible granularity without being
/// told one, and the <em>value</em>, which defaults to the bottom of the range.
/// </para>
/// <para>
/// The precision is the number field's rule — a value is rounded to it, and a precision of zero
/// makes the component report itself as an integer — which is why it is the number component that
/// owns that rule rather than this one.
/// </para>
/// </remarks>
[GradioBlock("slider")]
[HandlerValue("double", "change", "input", "release")]
public sealed partial class Slider : FormComponent
{
    internal Slider()
    {
    }

    private double? _value;
    private double? _step;
    private double? _randomValue;

    /// <summary>The bottom of the range.</summary>
    public double Minimum { get; set; }

    /// <summary>The top of the range.</summary>
    public double Maximum { get; set; } = 100;

    /// <summary>The number the slider starts on.</summary>
    /// <value>
    /// The value rounded to <see cref="Precision"/>, or <see cref="Minimum"/> when the application
    /// gave none — which is what the Python version carries, because it rounds the value when the
    /// component is built and answers the bottom of the range for an absent one.
    /// </value>
    /// <remarks>
    /// A randomized slider starts on a value drawn from the range, as the Python version's does;
    /// the draw is made once and kept, so that the config says the same thing however often it is
    /// read.
    /// </remarks>
    public double? Value
    {
        get => Number.RoundToPrecision(
            _value ?? (Randomize ? RandomValue() : Minimum),
            Precision);
        set => _value = value;
    }

    /// <summary>The increment between the values the slider can take.</summary>
    /// <value>
    /// A hundredth of the range — one when the range is the default hundred — unless the application
    /// names one.
    /// </value>
    public double Step
    {
        get => _step ?? DefaultStep();
        set => _step = value;
    }

    /// <summary>How many decimal places the value is rounded to.</summary>
    public int? Precision { get; set; }

    /// <summary>The label drawn above the slider.</summary>
    public override string? Label { get; set; }

    /// <summary>The note drawn under the slider.</summary>
    public string? Info { get; set; }

    /// <summary>The relative width of the slider next to its neighbours.</summary>
    public override int? Scale { get; set; }

    /// <summary>The width below which the slider wraps.</summary>
    public override int? MinWidth { get; set; } = 160;

    /// <summary>Whether the slider starts on a value drawn at random from its range.</summary>
    /// <remarks>
    /// Declared because the Python version declares it, but never written to the config: the Python
    /// version replaces the value with a function when it is set, and a function is not a prop.
    /// What the config carries is the value the draw produced.
    /// </remarks>
    [OmitFromConfig]
    public bool Randomize { get; set; }

    /// <summary>The buttons drawn beside the slider.</summary>
    /// <value>
    /// No buttons unless the application asks for one; <c>reset</c> is the only one the Python
    /// version has. Absent rather than empty, because the Python version stores whatever it was
    /// given — and a slider nobody gave buttons to was given none.
    /// </value>
    public IReadOnlyList<string>? Buttons { get; set; }

    /// <summary>The typing information of the slider, which narrows to an integer at zero precision.</summary>
    public override JsonObject ApiInfo => new()
    {
        ["type"] = Precision == 0 ? "integer" : "number",
        ["description"] = $"numeric value between {Text(Minimum)} and {Text(Maximum)}",
    };

    /// <summary>A representative value of the slider: the bottom of its range.</summary>
    public override JsonNode? ExampleInputs => JsonValue.Create(Minimum);

    /// <summary>The number a handler reads.</summary>
    /// <param name="value">The number the client sent.</param>
    /// <returns>
    /// The number, rounded to <see cref="Precision"/>; a whole number when the precision is zero.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">The number is outside the range.</exception>
    public override object? Preprocess(JsonNode? value)
    {
        var number = JsonValues.Number(value)
            ?? throw new ArgumentException(
                $"A slider's value is a number; the client sent '{value?.ToJsonString() ?? "null"}'.",
                nameof(value));

        Number.RaiseIfOutOfBounds(number, Minimum, Maximum);

        return Number.Rounded(number, Precision);
    }

    /// <summary>A cell of a flagging log read back as this component's value.</summary>
    /// <param name="cell">The cell.</param>
    /// <returns>The number the cell holds.</returns>
    /// <remarks>Stored the way a number is, and read back the way one is.</remarks>
    public override JsonNode? ReadFromFlag(string cell) => ParseEscapedJson(cell);

    /// <summary>The number the client receives from what a handler returned.</summary>
    /// <param name="value">The number, or <see langword="null"/> for the bottom of the range.</param>
    /// <returns>The rounded number; a whole number when the precision is zero.</returns>
    /// <exception cref="ArgumentException"><paramref name="value"/> is not a number.</exception>
    public override JsonNode? Postprocess(object? value)
    {
        if (value is null)
        {
            return JsonValues.FromObject(Number.Rounded(Minimum, Precision));
        }

        if (value is not (int or long or double or float or decimal))
        {
            throw new ArgumentException(
                $"A slider's value is a number; the handler returned a {value.GetType()}.",
                nameof(value));
        }

        return JsonValues.FromObject(
            Number.Rounded(Convert.ToDouble(value, CultureInfo.InvariantCulture), Precision));
    }

    /// <summary>The step a slider takes when the application names none.</summary>
    /// <returns>A hundredth of the range, as a power of ten.</returns>
    /// <remarks>
    /// <para>
    /// The Python version's rule: the step is <c>10^(floor(log10(range)) - 2)</c>, which for a
    /// range of a hundred is one, for a range of ten is a tenth, and for a range of a thousand is
    /// ten. A non-positive range is refused rather than producing an infinite step.
    /// </para>
    /// <para>
    /// The refusal happens when the slider is described rather than when it is built, because a C#
    /// component is built by assigning its members one at a time: there is no moment at which the
    /// range is known to be complete except the one where the component is read. The Python
    /// version, whose constructor takes both ends together, refuses at construction.
    /// </para>
    /// </remarks>
    private double DefaultStep()
    {
        var range = Maximum - Minimum;

        if (range <= 0)
        {
            throw new ArgumentException(
                $"A slider's minimum ({Text(Minimum)}) must be less than its maximum ({Text(Maximum)}).",
                nameof(Minimum));
        }

        return Math.Pow(10, Math.Floor(Math.Log10(range)) - 2);
    }

    /// <summary>The value a randomized slider starts on.</summary>
    /// <returns>A value on the step grid, drawn once per component.</returns>
    private double RandomValue()
    {
        if (_randomValue is { } drawn)
        {
            return drawn;
        }

        var steps = (int)((Maximum - Minimum) / Step);
        var value = Minimum + Random.Shared.Next(steps + 1) * Step;

        // The Python version rounds the draw to the decimals the step has, so that a slider whose step is a
        // tenth does not show a value with a long tail of digits.
        var decimals = Decimals(Step);

        _randomValue = decimals > 0 ? Math.Round(value, decimals) : value;

        return _randomValue.Value;
    }

    /// <summary>How many decimals a number is written with.</summary>
    /// <param name="value">The number.</param>
    /// <returns>The number of decimals its shortest spelling has.</returns>
    private static int Decimals(double value)
    {
        var text = value.ToString("R", CultureInfo.InvariantCulture);
        var point = text.IndexOf('.', StringComparison.Ordinal);

        return point < 0 ? 0 : text.Length - point - 1;
    }

    /// <summary>A number as the Python version writes one in a description.</summary>
    /// <param name="value">The number.</param>
    /// <returns>The shortest spelling that round-trips.</returns>
    private static string Text(double value) => value.ToString("R", CultureInfo.InvariantCulture);
}

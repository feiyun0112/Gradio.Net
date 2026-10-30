using System.Globalization;
using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// One option of a component that offers a list of them: what the user reads, and what the handler
/// receives.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's choices are a list of pairs — a display label and a value — and a bare
/// string or number in the list is shorthand for a pair whose two halves are the same text. The
/// config carries the pairs, because the frontend draws the label and sends the value back; this
/// type is that pair, and it is what a component's <c>choices</c> prop is written from.
/// </para>
/// <para>
/// The value is an <see cref="object"/> rather than a string because the Python version accepts
/// numbers as choices and hands the number to the handler: a dropdown of years should give the
/// handler <c>2024</c>, not <c>"2024"</c>. <see cref="SameValue"/> is how two of them are compared
/// — a value that came back from a browser is a JSON number, which this framework reads as a
/// <see cref="double"/>, while the choice it names may have been written as an <see cref="int"/>.
/// </para>
/// </remarks>
public sealed class Choice : IConfigValue
{
    /// <summary>Declares a choice whose label is its value.</summary>
    /// <param name="value">The value the handler receives.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public Choice(object value)
        : this(value, null)
    {
    }

    /// <summary>Declares a choice with a label of its own.</summary>
    /// <param name="value">The value the handler receives.</param>
    /// <param name="label">The text the user reads, or <see langword="null"/> for the value's own text.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="value"/> is not a string, a number or a boolean.
    /// </exception>
    /// <remarks>
    /// <para>
    /// The value is limited to the scalars a JSON config can carry, because it is written into one
    /// and sent back by a browser: an option whose value were an object would arrive as something
    /// other than what was put in.
    /// </para>
    /// <para>
    /// The arguments are the value and then the label — the opposite of the config's <c>[label,
    /// value]</c> pair — because the value is the part a choice cannot be without. The two orders
    /// are worth keeping in mind when reading an application against the Python version's:
    /// <c>gr.Radio(choices=[("Two", 2)])</c> is <c>new Choice(2, "Two")</c>.
    /// </para>
    /// </remarks>
    public Choice(object value, string? label)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value is not (string or int or long or double or float or decimal or bool))
        {
            throw new ArgumentException(
                $"A choice's value must be a string, a number or a boolean; got a {value.GetType()}.",
                nameof(value));
        }

        Value = value;
        Label = label ?? Text(value);
    }

    /// <summary>The text the user reads.</summary>
    public string Label { get; }

    /// <summary>The value the handler receives.</summary>
    public object Value { get; }

    /// <summary>The choice as the config carries it: the label and the value, in that order.</summary>
    /// <returns>A new array.</returns>
    public JsonNode ToConfigValue() => new JsonArray(JsonValue.Create(Label), JsonValues.FromObject(Value));

    /// <summary>Reads a string as a choice whose label is its own text.</summary>
    /// <param name="value">The value the handler receives.</param>
    /// <returns>The choice.</returns>
    /// <remarks>
    /// A conversion rather than a constructor call at every site, because the Python version writes
    /// its choices as bare values — <c>gr.Radio(choices=["a", "b"])</c> — and a ported line should
    /// not have to wrap each one.
    /// </remarks>
    public static implicit operator Choice(string value) => new(value);

    /// <summary>Reads the Python version's pair — the label, then the value — as a choice.</summary>
    /// <param name="pair">The label the user reads and the value the handler receives.</param>
    /// <returns>The choice.</returns>
    /// <remarks>
    /// The pair is in the Python version's order, <c>("Two", 2)</c>, which is the opposite of the
    /// constructor's: <c>gr.Radio(choices=[("Two", 2)])</c> is the line a ported file already has,
    /// and it keeps its shape here. The constructor stays value-first because a choice cannot be
    /// without a value.
    /// </remarks>
    public static implicit operator Choice((string Label, object Value) pair) => new(pair.Value, pair.Label);

    /// <summary>Reads a whole number as a choice whose label is its own text.</summary>
    /// <param name="value">The value the handler receives.</param>
    /// <returns>The choice.</returns>
    public static implicit operator Choice(int value) => new(value);

    /// <summary>Whether a value names this choice.</summary>
    /// <param name="value">The value to compare, as a handler or a client spells it.</param>
    /// <returns>Whether it is this choice's value.</returns>
    /// <remarks>
    /// Numbers are compared by magnitude rather than by CLR type: a browser sends every number as
    /// one JSON number, and a choice written as <c>1</c> has to match a value that arrived as
    /// <c>1.0</c>.
    /// </remarks>
    public bool Matches(object? value) => SameValue(Value, value);

    /// <summary>Whether two choice values are the same value.</summary>
    /// <param name="left">One value.</param>
    /// <param name="right">The other.</param>
    /// <returns>Whether they are the same.</returns>
    public static bool SameValue(object? left, object? right) => (left, right) switch
    {
        (null, null) => true,
        (null, _) or (_, null) => false,
        (string text, string other) => string.Equals(text, other, StringComparison.Ordinal),
        (bool flag, bool other) => flag == other,
        _ when IsNumber(left) && IsNumber(right) => ToDouble(left) == ToDouble(right),
        _ => left.Equals(right),
    };

    /// <summary>The index of the choice a value names, or <see langword="null"/> when none does.</summary>
    /// <param name="choices">The choices to search.</param>
    /// <param name="value">The value to find.</param>
    /// <returns>The index, or <see langword="null"/>.</returns>
    public static int? IndexOf(IReadOnlyList<Choice> choices, object? value)
    {
        ArgumentNullException.ThrowIfNull(choices);

        for (var index = 0; index < choices.Count; index++)
        {
            if (choices[index].Matches(value))
            {
                return index;
            }
        }

        return null;
    }

    /// <summary>Refuses a value the list does not offer, with the Python version's
    /// message.</summary> <param name="choices">The choices on offer.</param>
    /// <param name="value">The value the client sent, as it sent it.</param>
    /// <param name="spelledPlainly">
    /// Whether to name the value as <c>str()</c> spells it rather than as <c>repr()</c> does.
    /// </param>
    /// <exception cref="ArgumentException">The value is not one of the choices.</exception>
    /// <remarks>
    /// <para>
    /// The Python version's message is <c>f"Value: {value!r} (type: {type(value)}) is not in the
    /// list of choices: {choice_values}"</c>, and every component that offers a list raises it from
    /// its own preprocessing step. It lives here because it is one message about one question — is
    /// this value on the list — asked by three components.
    /// </para>
    /// <para>
    /// <paramref name="spelledPlainly"/> exists for one call site: the Python version's dropdown
    /// names a value inside a list with <c>repr</c> and a value on its own with <c>str</c>, so the
    /// same value is quoted in one message and bare in the other. That reads like an oversight, and
    /// it is reproduced because a client that compares the two messages against the Python
    /// version's has to find the same text.
    /// </para>
    /// </remarks>
    internal static void Refuse(JsonNode? value, IReadOnlyList<Choice> choices, bool spelledPlainly = false)
    {
        ArgumentNullException.ThrowIfNull(choices);

        if (IndexOf(choices, JsonValues.ToObject(value)) is not null)
        {
            return;
        }

        var named = spelledPlainly
            ? PythonText.Str(value)
            : $"{PythonText.Repr(value)} (type: {PythonText.TypeName(value)})";

        throw new ArgumentException(
            $"Value: {named} is not in the list of choices: "
            + PythonText.ReprOf(choices.Select(choice => choice.Value)),
            nameof(value));
    }

    /// <summary>A value read as the list of values it stands for.</summary>
    /// <param name="value">A list, a single value, or <see langword="null"/>.</param>
    /// <returns>The values, empty for none.</returns>
    /// <remarks>
    /// The Python version accepts a single choice as shorthand for a list of one, and answers an
    /// empty list for none. Every component that carries several choices says so this way, so the
    /// shorthand means the same thing in all of them.
    /// </remarks>
    internal static IReadOnlyList<object?> AsList(object? value) => value switch
    {
        null => [],
        IReadOnlyList<object?> values => values,
        _ => [value],
    };

    /// <summary>The values of a list of choices, as a schema's <c>enum</c> spells them.</summary>
    /// <param name="choices">The choices.</param>
    /// <returns>A new array of their values.</returns>
    internal static JsonArray EnumOf(IReadOnlyList<Choice> choices)
    {
        ArgumentNullException.ThrowIfNull(choices);

        var values = new JsonArray();

        foreach (var choice in choices)
        {
            values.Add(JsonValues.FromObject(choice.Value));
        }

        return values;
    }

    /// <summary>The text a scalar value is shown as when it has no label of its own.</summary>
    /// <param name="value">The value.</param>
    /// <returns>The text, spelled as the Python version spells it.</returns>
    private static string Text(object value) => value switch
    {
        string text => text,
        bool flag => flag ? "True" : "False",
        double number => number.ToString("R", CultureInfo.InvariantCulture),
        float number => ((double)number).ToString("R", CultureInfo.InvariantCulture),
        decimal number => number.ToString(CultureInfo.InvariantCulture),
        IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
        _ => value.ToString() ?? string.Empty,
    };

    private static bool IsNumber(object? value) =>
        value is int or long or double or float or decimal;

    private static double ToDouble(object value) => value switch
    {
        int number => number,
        long number => number,
        float number => number,
        decimal number => (double)number,
        _ => (double)value,
    };
}

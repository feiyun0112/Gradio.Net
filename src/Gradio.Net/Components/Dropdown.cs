using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A list of options the user picks one or more of, with the option to allow values that are not on
/// the list.
/// </summary>
/// <remarks>
/// <para>
/// The members are the Python version's constructor parameters, and the config props are generated
/// from them in the Python version's order. The dropdown is the one component of the family whose
/// value has three states rather than two: <em>unset</em>, which means "the first option" (or no
/// options at all for a multiple one), <em>null</em>, which means "nothing selected", and a value.
/// The first is the default and the second is what an application asks for by setting it, so they
/// cannot share a representation — hence the sentinel the property starts on.
/// </para>
/// <para>
/// <see cref="Multiselect"/> changes what a value is: a list of them, and a single value the
/// application sets is read as a list of one. <see cref="AllowCustomValue"/> changes what is
/// accepted: with it, a value that is not on the list is the user's answer rather than an error.
/// </para>
/// </remarks>
[GradioBlock("dropdown")]
[HandlerValue("object?", "change", "input", "focus", "blur")]
public sealed partial class Dropdown : FormComponent
{
    internal Dropdown()
    {
    }

    /// <summary>The value of a dropdown nobody has set, which means "the first option".</summary>
    private static readonly object Unset = new();

    private object? _value = Unset;
    private string _type = "value";
    private bool _filterable = true;
    private int? _numChoicesShown = 100;

    /// <summary>The options offered.</summary>
    public IReadOnlyList<Choice> Choices { get; set; } = [];

    /// <summary>The value selected when the page loads.</summary>
    /// <value>
    /// The first option unless the application says otherwise; no value at all when there are no
    /// options; a list of values for a multiple dropdown. Setting it to <see langword="null"/>
    /// explicitly means "nothing selected", which is a different thing from not setting it.
    /// </value>
    public object? Value
    {
        get => Selected(_value);
        set => _value = value;
    }

    /// <summary>What the component hands a handler: the selected value, or its position.</summary>
    /// <value><c>value</c> or <c>index</c>; anything else is refused.</value>
    public string Type
    {
        get => _type;
        set
        {
            if (value is not ("value" or "index"))
            {
                throw new ArgumentException(
                    $"A dropdown's type must be value or index; got '{value}'.",
                    nameof(value));
            }

            _type = value;
        }
    }

    /// <summary>Whether several options may be selected.</summary>
    /// <value>
    /// <see langword="null"/> unless the application says, which is the Python version's third
    /// state: the frontend reads an absent flag as "one option", and the flag is only written when
    /// it was given.
    /// </value>
    public bool? Multiselect { get; set; }

    /// <summary>Whether the user may type a value that is not on the list.</summary>
    public bool AllowCustomValue { get; set; }

    /// <summary>How many options may be selected, or <see langword="null"/> for no limit.</summary>
    public int? MaxChoices { get; set; }

    /// <summary>How many matching options are shown before the list has to be scrolled.</summary>
    /// <exception cref="ArgumentOutOfRangeException">The number is not positive.</exception>
    public int? NumChoicesShown
    {
        get => _numChoicesShown;
        set
        {
            if (value is <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    value,
                    "The number of choices shown must be greater than 0.");
            }

            _numChoicesShown = value;
        }
    }

    /// <summary>Whether the user may filter the list by typing.</summary>
    /// <value>
    /// Whether the application asked for it, or <see langword="true"/> when custom values are
    /// allowed — a dropdown that takes a value nobody offered has to be typeable, and the Python
    /// version corrects the combination rather than serving one that cannot work.
    /// </value>
    public bool Filterable
    {
        get => _filterable || AllowCustomValue;
        set => _filterable = value;
    }

    /// <summary>The label drawn above the list.</summary>
    public override string? Label { get; set; }

    /// <summary>The note drawn under the label.</summary>
    public string? Info { get; set; }

    /// <summary>The relative width of the list next to its neighbours.</summary>
    public override int? Scale { get; set; }

    /// <summary>The width below which the list wraps.</summary>
    public override int? MinWidth { get; set; } = 160;

    /// <summary>The buttons drawn beside the list.</summary>
    public IReadOnlyList<string> Buttons { get; set; } = [];

    /// <inheritdoc />
    public override JsonObject ApiInfo => Multiselect == true
        ? new JsonObject
        {
            ["type"] = "array",
            ["items"] = new JsonObject
            {
                ["type"] = "string",
                ["enum"] = Choice.EnumOf(Choices),
            },
        }
        : new JsonObject
        {
            ["type"] = "string",
            ["enum"] = Choice.EnumOf(Choices),
        };

    /// <inheritdoc />
    public override JsonNode? ExampleInputs
    {
        get
        {
            if (Multiselect == true)
            {
                return Choices.Count == 0
                    ? new JsonArray()
                    : new JsonArray(JsonValues.FromObject(Choices[0].Value));
            }

            return Choices.Count == 0 ? null : JsonValues.FromObject(Choices[0].Value);
        }
    }

    /// <summary>The value a handler reads.</summary>
    /// <param name="value">The value the client sent.</param>
    /// <returns>The value, or its position when <see cref="Type"/> is <c>index</c>.</returns>
    /// <exception cref="ArgumentException">
    /// A value is not on the list and custom values are not allowed.
    /// </exception>
    /// <remarks>
    /// The refusal is the Python version's, down to the spelling — see <see cref="Choice.Refuse"/>,
    /// which is where the message lives, and which reproduces the one thing the dropdown does
    /// differently: a value inside a list is named as <c>repr()</c> spells it, a value on its own
    /// as <c>str()</c> does.
    /// </remarks>
    public override object? Preprocess(JsonNode? value)
    {
        if (value is null)
        {
            return null;
        }

        var chosen = JsonValues.ToObject(value);

        if (!AllowCustomValue)
        {
            if (value is JsonArray list)
            {
                foreach (var entry in list)
                {
                    Choice.Refuse(entry, Choices);
                }
            }
            else
            {
                Choice.Refuse(value, Choices, spelledPlainly: true);
            }
        }

        if (Type == "value")
        {
            return chosen;
        }

        if (chosen is IReadOnlyList<object?> values)
        {
            return values.Select(entry => (object?)Choice.IndexOf(Choices, entry)).ToArray();
        }

        return Choice.IndexOf(Choices, chosen);
    }

    /// <summary>The value the client receives from what a handler returned.</summary>
    /// <param name="value">The value, or a single one for a multiple dropdown.</param>
    /// <returns>The value, as a list when the dropdown is a multiple one.</returns>
    public override JsonNode? Postprocess(object? value)
    {
        if (value is null)
        {
            return null;
        }

        return JsonValues.FromObject(Multiselect == true ? Choice.AsList(value) : value);
    }

    /// <summary>The value a dropdown carries, with the default resolved.</summary>
    /// <param name="value">The value the application set, or the sentinel.</param>
    /// <returns>The value as the config carries it.</returns>
    private object? Selected(object? value)
    {
        if (ReferenceEquals(value, Unset))
        {
            if (Multiselect == true)
            {
                return Array.Empty<object?>();
            }

            return Choices.Count == 0 ? null : Choices[0].Value;
        }

        // A single value given to a multiple dropdown is a list of one, whatever it is: the Python version
        // wraps anything that is not already a list, and a dropdown whose value arrived as one
        // number would otherwise be described with a value its own API schema says it cannot hold.
        if (Multiselect == true && value is not IReadOnlyList<object?>)
        {
            return new object?[] { value };
        }

        return value;
    }

}

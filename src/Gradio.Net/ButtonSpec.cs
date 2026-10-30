namespace Gradio.Net;
/// <summary>
/// A button an <see cref="Interface"/> draws, given either as the label it should carry or as a
/// button the application has already configured.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>str | Button</c>, and <see cref="None"/> for the one parameter that also accepts
/// "no button at all". A type rather than an <see cref="object"/> so that a mistake is a compile
/// error: a label and a button are both things an application writes, and neither is anything else.
/// </para>
/// <para>
/// It converts from both, so a parameter of this type reads the way it does in the Python version —
/// <c>submitBtn: "Go"</c> or <c>submitBtn: new Button { Variant = "primary" }</c>.
/// </para>
/// </remarks>
public readonly struct ButtonSpec
{
    private readonly Button? _button;
    private readonly string? _label;
    private readonly bool _none;

    private ButtonSpec(Button? button, string? label, bool none)
    {
        _button = button;
        _label = label;
        _none = none;
    }

    /// <summary>The interface draws no such button.</summary>
    /// <remarks>
    /// The Python version's <c>None</c>, which only the clear button accepts: an interface always
    /// has something to submit with, and one that hides its clear button is drawn without it.
    /// </remarks>
    public static ButtonSpec None => new(null, null, none: true);

    /// <summary>The button the application gave, or <see langword="null"/> when it gave a label.</summary>
    public Button? Button => _button;

    /// <summary>The label the application gave, or <see langword="null"/> when it gave a button.</summary>
    public string? Label => _label;

    /// <summary>Whether the interface draws no such button.</summary>
    public bool IsNone => _none;

    /// <summary>Reads a label as the button that carries it.</summary>
    /// <param name="label">The label.</param>
    public static implicit operator ButtonSpec(string label) => new(null, label, none: false);

    /// <summary>Reads a button as itself.</summary>
    /// <param name="button">The button.</param>
    public static implicit operator ButtonSpec(Button button) => new(button, null, none: false);

    /// <inheritdoc />
    public override string ToString() => _none ? "none" : _button?.Value ?? _label ?? "default";
}

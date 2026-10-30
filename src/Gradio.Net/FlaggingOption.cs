namespace Gradio.Net;
/// <summary>
/// One option a user may flag a sample with: the label on the button, and the value written to the
/// log.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>list[str] | list[tuple[str, str]]</c>. A list of strings becomes "Flag as …"
/// buttons whose value is the string; a list of these keeps the label and the value apart, so that a
/// button can say "Wrong answer" and the log can record <c>wrong</c>.
/// </para>
/// <para>
/// It converts from a string, so <c>flaggingOptions: ["wrong", "bad"]</c> reads the way it does
/// in the Python version.
/// </para>
/// </remarks>
/// <param name="Label">The text drawn on the button.</param>
/// <param name="Value">The value recorded in the log, or <see langword="null"/> for none.</param>
public sealed record FlaggingOption(string Label, string? Value)
{
    /// <summary>Reads a string as an option labelled "Flag as …" that records the string itself.</summary>
    /// <param name="value">The value, which is also what the button is labelled with.</param>
    public static implicit operator FlaggingOption(string value) =>
        new($"Flag as {value}", value);
}

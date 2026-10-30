using System.Text.Json;
using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// Reads the parameter shapes the Python version accepts but C# cannot express, and normalises
/// them.
/// </summary>
/// <remarks>
/// <para>
/// Four of the Python version's constructor parameters take "one thing or several" — <c>inputs</c>,
/// <c>elem_classes</c>, <c>preserved_by_key</c>, <c>buttons</c> — and two take a boolean that also
/// accepts the string <c>"hidden"</c>. C# has no union types, so those parameters are declared
/// <see cref="object"/> and read here, once, rather than six times over seventy factories.
/// </para>
/// <para>
/// The queue's two union parameters — a number of seconds or <c>"auto"</c>, a number or
/// <c>"not_set"</c> — are read here for the same reason, and are the two readings that are not a
/// constructor's.
/// </para>
/// <para>
/// This is the "one value or a list" detail the bindings already accept; it is the same reading of
/// the Python version's signatures, applied to the parameters that are not bindings. A parameter
/// that accepts something the framework cannot represent is declared with the type it can represent
/// instead, so that the difference shows up when the port is compiled rather than when it runs.
/// </para>
/// </remarks>
internal static class Values
{
    /// <summary>Reads a value the Python version accepts as "one or several" as the list the
    /// framework holds.</summary> <param name="value">A single item, a sequence of them, or
    /// <see langword="null"/>.</param> <returns>The items, or <see langword="null"/> when nothing
    /// was given.</returns> <exception cref="ArgumentException">The value is neither one item nor a
    /// sequence of them.</exception>
    internal static object[]? Items(object? value) => value switch
    {
        null => null,
        object[] many => many,
        string => throw NotOneOrMany(value, "a string is one value, so write [text] for a list of one"),
        System.Collections.IEnumerable many => [.. many.Cast<object>()],
        _ => [value],
    };

    /// <summary>Reads a value the Python version accepts as one string or several as a list of
    /// strings.</summary> <param name="value">A string, a sequence of strings, or
    /// <see langword="null"/>.</param> <returns>The strings, or an empty list when nothing was
    /// given.</returns> <exception cref="ArgumentException">The value is neither a string nor a
    /// sequence of them.</exception>
    internal static IReadOnlyList<string> Strings(object? value) => value switch
    {
        null => [],
        string one => [one],
        IEnumerable<string> many => [.. many],
        _ => throw NotOneOrMany(value, "a list of strings is a list of strings, not of something else"),
    };

    /// <summary>Reads the Python version's <c>visible</c>: a boolean, or the string
    /// <c>"hidden"</c>.</summary> <param name="value">A boolean, the string <c>"hidden"</c>, a
    /// visibility, or <see langword="null"/>.</param> <returns>The visibility, shown when nothing
    /// was given.</returns> <exception cref="ArgumentException">The value is not one of the
    /// three.</exception>
    /// <remarks>
    /// The third value is not a nuance: a block hidden this way keeps the space it would have taken
    /// and an invisible one gives it up, and the frontend draws the two differently. The framework
    /// already has the type for it, so the factory hands over the type rather than a flag.
    /// </remarks>
    internal static Visibility VisibilityOf(object? value) => value switch
    {
        null => Visibility.Shown,
        bool shown => shown,
        Visibility visibility => visibility,
        string text when string.Equals(text, "hidden", StringComparison.Ordinal) => Visibility.Hidden,
        _ => throw new ArgumentException(
            $"'{value}' is not a visibility: the Python version accepts true, false or \"hidden\".",
            nameof(value)),
    };

    /// <summary>Reads a value a component keeps as JSON: a path, a list of them, or a document.</summary>
    /// <param name="value">A string, a number, a boolean, a sequence, a document, or <see langword="null"/>.</param>
    /// <returns>The value as the document the config carries, or <see langword="null"/>.</returns>
    /// <exception cref="ArgumentException">The value cannot be written as JSON.</exception>
    /// <remarks>
    /// A file, an image and a state hold their value as the document it will be served as, because
    /// what the value means depends on the component — a path is copied into the application's
    /// cache, a state is the application's own object. The factory hands the document over and lets
    /// the component decide, which is the same division the Python version makes.
    /// </remarks>
    internal static JsonNode? Node(object? value) => value switch
    {
        null => null,
        JsonNode node => node,
        _ => JsonSerializer.SerializeToNode(value, value.GetType()),
    };

    /// <summary>Reads the Python version's <c>status_update_rate</c>: a number of seconds, or
    /// <c>"auto"</c>.</summary> <param name="value">A number, the string <c>"auto"</c>, or
    /// <see langword="null"/>.</param> <returns>The interval in seconds, or <see langword="null"/>
    /// for "re-describe when something changes".</returns> <exception cref="ArgumentException">The
    /// value is not one of the two.</exception>
    /// <remarks>
    /// <c>"auto"</c> and "nothing" are the same answer here because they are the same answer the
    /// Python version: <c>status_update_rate="auto"</c> is the default, and it means the queue
    /// describes itself when a run finishes rather than on a timer.
    /// </remarks>
    internal static double? Seconds(object? value) => value switch
    {
        null => null,
        string text when string.Equals(text, "auto", StringComparison.Ordinal) => null,
        double seconds => seconds,
        int seconds => seconds,
        _ => throw new ArgumentException(
            $"'{value}' is not a status update rate: the Python version accepts a number of seconds or \"auto\".",
            nameof(value)),
    };

    /// <summary>Reads the Python version's <c>default_concurrency_limit</c>: a number, or
    /// <c>"not_set"</c>.</summary> <param name="value">A number, the string <c>"not_set"</c>, a
    /// limit, or <see langword="null"/>.</param> <returns>The limit.</returns>
    /// <exception cref="ArgumentException">The value is not one of the three.</exception>
    /// <remarks>
    /// <para>
    /// Nothing given is the Python version's default — the word <c>"not_set"</c>, which means "read
    /// <c>GRADIO_DEFAULT_CONCURRENCY_LIMIT</c> and fall back to one run at a time". That is the
    /// same resolution <see cref="ConcurrencyLimit.ApplicationDefault"/> names, and the queue
    /// performs it, so the two spellings of "not set" agree by construction.
    /// </para>
    /// <para>
    /// A limit of "any number of runs" is the Python version's <c>None</c>, which C# spells
    /// <see cref="ConcurrencyLimit.Unlimited"/>: it is a value rather than "nothing", so the two
    /// are told apart by the value given rather than by the one left out.
    /// </para>
    /// <para>
    /// A number below one is refused by <see cref="ConcurrencyLimit.Of"/> rather than accepted and
    /// treated as unlimited, which is the one place this build is stricter than the Python version:
    /// a limit of zero is a binding nothing can run.
    /// </para>
    /// </remarks>
    internal static ConcurrencyLimit Concurrency(object? value) => value switch
    {
        null => ConcurrencyLimit.ApplicationDefault,
        ConcurrencyLimit limit => limit,
        string text when string.Equals(text, "not_set", StringComparison.Ordinal) => ConcurrencyLimit.ApplicationDefault,
        int runs => ConcurrencyLimit.Of(runs),
        _ => throw new ArgumentException(
            $"'{value}' is not a concurrency limit: the Python version accepts a number or \"not_set\".",
            nameof(value)),
    };

    private static ArgumentException NotOneOrMany(object value, string because) =>
        new($"'{value}' ({value.GetType().Name}) is not what this parameter accepts: {because}.", nameof(value));

    /// <summary>Reads the Python version's <c>primary_hue</c> and its siblings: a colour ramp, or
    /// the name of one.</summary> <param name="value">A <see cref="Color"/>, a ramp's name, or
    /// <see langword="null"/>.</param> <returns>The ramp, or <see langword="null"/> when nothing
    /// was given.</returns> <exception cref="ArgumentException">The value is neither.</exception>
    /// <remarks>
    /// The Python version types these <c>Color | str</c>, and the string is the name of one of the ramps it
    /// ships — <c>violet</c>, <c>zinc</c> and so on. The names are the recorded set, so a misspelt
    /// one is refused where it was written rather than silently replaced by a default.
    /// </remarks>
    internal static Color? Hue(object? value) => value switch
    {
        null => null,
        Color hue => hue,
        string name => ThemePresets.FindHue(name),
        _ => throw new ArgumentException(
            $"'{value}' is not a hue: the Python version accepts a colour ramp or the name of one.",
            nameof(value)),
    };

    /// <summary>Reads the Python version's <c>text_size</c>: a size, or one of its names.</summary>
    /// <param name="value">A <see cref="Size"/>, a name such as <c>sm</c>, or
    /// <see langword="null"/>.</param> <returns>The size, or <see langword="null"/> when nothing
    /// was given.</returns> <exception cref="ArgumentException">The value is neither.</exception>
    internal static Size? TextSize(object? value) => SizeOf(value, "text");

    /// <summary>Reads the Python version's <c>spacing_size</c>: a size, or one of its
    /// names.</summary> <param name="value">A <see cref="Size"/>, a name such as <c>sm</c>, or
    /// <see langword="null"/>.</param> <returns>The size, or <see langword="null"/> when nothing
    /// was given.</returns> <exception cref="ArgumentException">The value is neither.</exception>
    internal static Size? SpacingSize(object? value) => SizeOf(value, "spacing");

    /// <summary>Reads the Python version's <c>radius_size</c>: a size, or one of its
    /// names.</summary> <param name="value">A <see cref="Size"/>, a name such as <c>sm</c>, or
    /// <see langword="null"/>.</param> <returns>The size, or <see langword="null"/> when nothing
    /// was given.</returns> <exception cref="ArgumentException">The value is neither.</exception>
    internal static Size? RadiusSize(object? value) => SizeOf(value, "radius");

    /// <summary>Reads the Python version's <c>font</c>: one font, several, or their
    /// names.</summary> <param name="value">A <see cref="Font"/>, a font name, a sequence of
    /// either, or <see langword="null"/>.</param> <returns>The fonts, or <see langword="null"/>
    /// when nothing was given.</returns> <exception cref="ArgumentException">The value is none of
    /// those.</exception>
    /// <remarks>
    /// A list is a font stack: the first family a browser finds is the one it draws with, which is
    /// what the Python version's <c>font=["Inter", "sans-serif"]</c> means.
    /// </remarks>
    internal static IReadOnlyList<Font>? Fonts(object? value) => value switch
    {
        null => null,
        Font font => [font],
        string name => [name],
        IEnumerable<Font> fonts => [.. fonts],
        IEnumerable<string> names => [.. names.Select(name => (Font)name)],
        _ => throw new ArgumentException(
            $"'{value}' is not a font: the Python version accepts a font, its name, or a list of either.",
            nameof(value)),
    };

    private static Size? SizeOf(object? value, string kind) => value switch
    {
        null => null,
        Size size => size,
        string name => ThemePresets.FindSize(name, kind),
        _ => throw new ArgumentException(
            $"'{value}' is not a {kind} size: the Python version accepts a size or the name of one.",
            nameof(value)),
    };
}

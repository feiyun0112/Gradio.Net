using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// The translations an application offers, keyed by language: the dictionary the frontend
/// translates its own labels from.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>gr.I18n</c>. It holds a catalogue per locale — a map from a translation
/// key to the text that key reads as in that language — and nothing else: translating happens in
/// the browser, which knows the reader's language and carries the catalogue of its own chrome. What
/// the server does is name the keys and hand the catalogues over, which is why an instance is worth
/// having only once it reaches the application (<see cref="Blocks.I18n"/>).
/// </para>
/// <para>
/// A key reaches a component as the string <see cref="TranslationKey.Of"/> spells, so the props
/// stay the plain strings they are everywhere else and the config carries a value the vendored
/// frontend recognises without a second wire format. <see cref="TranslationKey"/> says why that
/// spelling is on the wire rather than in a type.
/// </para>
/// <para>
/// A locale is not checked against BCP 47. The Python version warns about a code that is not a
/// language tag and includes it anyway, because the frontend ignores a locale it has no messages
/// for and dropping it would be an edit of what the application said; there is no warning channel
/// here, so the rule is the behaviour.
/// </para>
/// </remarks>
public sealed class I18n
{
    private readonly List<(string Locale, IReadOnlyDictionary<string, string> Translations)> _catalogues = [];
    private readonly IReadOnlyList<string> _locales;

    /// <summary>Declares the catalogues, one per locale.</summary>
    /// <param name="translations">
    /// Each locale with the text its keys read as there, in the order the locales should be served.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="translations"/> is <see langword="null"/>, or one of its locales or
    /// catalogues is.
    /// </exception>
    /// <remarks>
    /// <para>
    /// The order is the order the locales are given in, because it is the order the config serves
    /// them in — the Python version's dictionary is built the same way, from the keyword arguments
    /// in the order they were written.
    /// </para>
    /// <para>
    /// A locale declared twice keeps the catalogue it was declared with last, in the place it was
    /// declared first, because that is what assigning to a key of a Python dictionary does and
    /// the Python version's catalogues are built by exactly that assignment.
    /// </para>
    /// </remarks>
    public I18n(params (string Locale, IReadOnlyDictionary<string, string> Translations)[] translations)
    {
        ArgumentNullException.ThrowIfNull(translations);

        // Where a locale sits in the list, for the length of the loop: a second declaration has to
        // find the first one to replace it in place. It is not kept, because the list is the
        // catalogue and an index beside it would be a second thing to keep true.
        var positions = new Dictionary<string, int>(StringComparer.Ordinal);

        foreach (var (locale, catalogue) in translations)
        {
            ArgumentNullException.ThrowIfNull(locale);
            ArgumentNullException.ThrowIfNull(catalogue);

            if (positions.TryGetValue(locale, out var position))
            {
                _catalogues[position] = (locale, catalogue);
                continue;
            }

            positions[locale] = _catalogues.Count;
            _catalogues.Add((locale, catalogue));
        }

        _locales = [.. _catalogues.Select(catalogue => catalogue.Locale)];
    }

    /// <summary>The locales the application declares, in the order it declared them.</summary>
    /// <value>
    /// The language tags as written, without checking them against BCP 47 — see the type's own
    /// remarks for why an unrecognized tag is kept.
    /// </value>
    /// <remarks>
    /// Readable because the Python version's catalogue is: <c>translations_dict</c> is a plain
    /// attribute, and an application that wanted to build a language menu of its own would read the
    /// same keys. Nothing in this framework reads it — the config serves the catalogues whole — so
    /// this is the application's view of what it declared rather than a step in serving it.
    /// </remarks>
    public IReadOnlyList<string> Locales => _locales;

    /// <summary>The catalogue one locale reads as, or <see langword="null"/> for one not declared.</summary>
    /// <param name="locale">The language tag, as it was declared.</param>
    /// <returns>The text each key reads as in that language, or <see langword="null"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="locale"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// A locale the application did not declare answers with <see langword="null"/> rather than with
    /// an empty catalogue, because the two are different statements: one says nothing is declared for
    /// that language, the other that it declares a language with no text in it — which is what
    /// <c>pt-BR</c> is, and what the config serves as <c>{}</c>.
    /// </remarks>
    public IReadOnlyDictionary<string, string>? TranslationsFor(string locale)
    {
        ArgumentNullException.ThrowIfNull(locale);

        var position = _catalogues.FindIndex(
            catalogue => string.Equals(catalogue.Locale, locale, StringComparison.Ordinal));

        return position < 0 ? null : _catalogues[position].Translations;
    }

    /// <summary>The string a component carries in place of the text for one key.</summary>
    /// <param name="key">The translation key, such as <c>greeting</c>.</param>
    /// <returns>The key in the spelling the frontend translates.</returns>
    /// <remarks>
    /// <para>
    /// The C# spelling of the Python version's <c>i18n("greeting")</c>: a string rather than an
    /// object, because the props a translation is written into — a label, a placeholder, a button's
    /// text — are strings, and the frontend reads the key out of the string it finds.
    /// </para>
    /// <para>
    /// A key this application has no text for is still a key: the frontend falls back to the
    /// English catalogue and then to the key itself, which is what a half-translated application
    /// looks like the Python version too.
    /// </para>
    /// </remarks>
    public string this[string key] => TranslationKey.Of(key);

    /// <summary>
    /// The catalogues as the config carries them, one entry per locale in declaration order.
    /// </summary>
    /// <returns>The catalogues, as the Python version's <c>i18n_translations</c>.</returns>
    /// <remarks>
    /// Written here rather than by the config, as a theme writes its own stylesheet: the shape of
    /// the dictionary is this type's to know, and the config's job is only to say where it goes.
    /// </remarks>
    internal JsonObject ToConfig()
    {
        var catalogues = new JsonObject();

        foreach (var (locale, translations) in _catalogues)
        {
            var catalogue = new JsonObject();

            foreach (var (key, text) in translations)
            {
                catalogue[key] = text;
            }

            catalogues[locale] = catalogue;
        }

        return catalogues;
    }
}

using System.Text.RegularExpressions;

namespace Gradio.Net.I18n;

public class I18n
{
    private static readonly Regex LocalePattern = new Regex(@"^[a-z]{2,3}(-[A-Za-z0-9]{2,8})*$");

    public Dictionary<string, Dictionary<string, string>> Translations { get; }

    public I18n(params object[] translations)
    {
        Translations = new Dictionary<string, Dictionary<string, string>>();

        if (translations.Length % 2 != 0)
        {
            throw new ArgumentException("Translations must be provided as locale-dictionary pairs.");
        }

        for (int i = 0; i < translations.Length; i += 2)
        {
            if (translations[i] is string locale && translations[i + 1] is Dictionary<string, string> translationDict)
            {
                if (!IsValidLocale(locale))
                {
                }
                Translations[locale] = translationDict;
            }
            else
            {
                throw new ArgumentException("Translations must be provided as locale-dictionary pairs.");
            }
        }
    }

    public I18n(Dictionary<string, Dictionary<string, string>> translationsDict)
    {
        Translations = new Dictionary<string, Dictionary<string, string>>();

        foreach (var (locale, translationDict) in translationsDict)
        {
            if (!IsValidLocale(locale))
            {
            }
            Translations[locale] = translationDict;
        }
    }

    private bool IsValidLocale(string locale)
    {
        return LocalePattern.IsMatch(locale);
    }

    public I18nData Call(string key)
    {
        return new I18nData(key);
    }

    public Dictionary<string, Dictionary<string, string>> TranslationsDict => Translations;
}

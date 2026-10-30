using Gradio.Net;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The application the i18n tests are about, written as its Python demo writes it.
/// </summary>
/// <remarks>
/// <para>
/// The page says what an application with translations says: a catalogue per locale, and keys
/// where a reader sees text — a heading, a label, a value, a button's text, both halves of a
/// choice pair. Two strings are deliberately left as text, so that a build which marked everything
/// would be caught rather than passing on a page whose labels read the same in English either way.
/// </para>
/// <para>
/// The catalogues are uneven on purpose: three locales translate the whole page, one translates two
/// keys and one translates none, which is a language tag with a region and a half-translated
/// application — neither of them an error, and both of them served as written.
/// </para>
/// </remarks>
internal static class I18nPage
{
    /// <summary>Builds the page into an application.</summary>
    /// <param name="blocks">The application to render into.</param>
    internal static void Build(Blocks blocks)
    {
        var i18n = Catalogue();

        blocks.I18n = i18n;

        // Fixed on both sides of the case rather than left to the defaults, which differ on purpose
        //.
        blocks.AnalyticsEnabled = true;

        Textbox name;
        Textbox output;
        Textbox selected;
        Button greet;
        Button reset;
        Radio format;

        using (blocks.BeginRender())
        {
            blocks.Add(new Markdown { Value = i18n["greeting"] });

            var nameRow = blocks.Add(new Row());
            using (blocks.Enter(nameRow))
            {
                name = blocks.Add(new Textbox { Label = i18n["name_label"], Value = i18n["john_doe"] });
            }

            var outputRow = blocks.Add(new Row());
            using (blocks.Enter(outputRow))
            {
                output = blocks.Add(new Textbox { Label = i18n["result_label"] });
            }

            var greetRow = blocks.Add(new Row());
            using (blocks.Enter(greetRow))
            {
                greet = blocks.Add(new Button { Value = i18n["submit_button"] });
            }

            var resetRow = blocks.Add(new Row());
            using (blocks.Enter(resetRow))
            {
                reset = blocks.Add(new Button { Value = "Reset Name" });
            }

            var formatRow = blocks.Add(new Row());
            using (blocks.Enter(formatRow))
            {
                format = blocks.Add(new Radio
                {
                    Choices =
                    [
                        new Choice("bold", i18n["choice_bold"]),
                        new Choice("italic", i18n["choice_italic"]),
                    ],
                    Value = "bold",
                    Label = i18n["format_label"],
                });
                selected = blocks.Add(new Textbox { Label = "Selected Format" });
            }

            blocks.Add(new Markdown
            {
                Value = "Text an application does not translate is served as it was written.",
            });
        }

        // Two of the three bindings answer with a key: the reset writes a translated value back
        // into the field, and the radio echoes the value it was given. The page's translations are
        // therefore not only the ones it was loaded with.
        greet.Click(
            (Func<string, object?>)(value => "hello " + value),
            [name],
            [output],
            apiName: "greet", parameterNames: ["name"]);

        reset.Click(
            (Func<object?>)(() => i18n["john_doe"]),
            [],
            [name],
            apiName: "reset_name");

        format.Change(
            (Func<object?, object?>)(value => value),
            [format],
            [selected],
            apiName: "echo_format", parameterNames: ["fmt"]);
    }

    /// <summary>The catalogue, written the way the demo writes one.</summary>
    /// <returns>The translations, in the order the demo declares them.</returns>
    internal static I18n Catalogue() => new(
        ("en", new Dictionary<string, string>
        {
            ["greeting"] = "Hello, welcome to my app!",
            ["name_label"] = "Your Name",
            ["submit_button"] = "Greet",
            ["john_doe"] = "John English",
            ["result_label"] = "Result",
            ["format_label"] = "Format",
            ["choice_bold"] = "Bold",
            ["choice_italic"] = "Italic",
        }),
        ("es", new Dictionary<string, string>
        {
            ["greeting"] = "¡Hola, bienvenido a mi aplicación!",
            ["name_label"] = "Tu Nombre",
            ["submit_button"] = "Saludar",
            ["john_doe"] = "John Spanish",
            ["result_label"] = "Resultado",
            ["format_label"] = "Formato",
            ["choice_bold"] = "Negrita",
            ["choice_italic"] = "Cursiva",
        }),
        ("fr", new Dictionary<string, string>
        {
            ["greeting"] = "Bonjour, bienvenue dans mon application!",
            ["name_label"] = "Votre Nom",
            ["submit_button"] = "Saluer",
            ["john_doe"] = "John French",
            ["result_label"] = "Résultat",
            ["format_label"] = "Format",
            ["choice_bold"] = "Gras",
            ["choice_italic"] = "Italique",
        }),
        ("zh-CN", new Dictionary<string, string>
        {
            ["greeting"] = "你好，欢迎使用我的应用！",
            ["name_label"] = "你的名字",
        }),
        ("pt-BR", new Dictionary<string, string>()));
}

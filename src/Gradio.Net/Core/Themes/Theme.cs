using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
namespace Gradio.Net;
/// <summary>
/// A set of CSS variables, and the rules that turn them into the stylesheet the page loads.
/// </summary>
/// <remarks>
/// <para>
/// This is the Python version's <c>ThemeClass</c>: an ordered bag of named values, a set of naming
/// rules, and a serialised form. It is deliberately not a set of typed properties — the variables
/// are CSS values, so a theme may put a colour, a length, a shadow or a reference to another
/// variable in any of them, and the meaning is whatever the stylesheet makes of it.
/// <see cref="ThemeOptions"/> is the typed face of the same surface, for the application that wants
/// the compiler to check the variable it is setting.
/// </para>
/// <para>
/// The order the variables were assigned in is part of the output, not an implementation detail:
/// the stylesheet lists them in that order, and the comparison against the Python version is byte
/// for byte. So the bag keeps insertion order and an assignment to a variable that is already there
/// keeps its place, exactly as assigning to a Python attribute does.
/// </para>
/// <para>
/// Three rules produce the stylesheet, and they are the whole of the theme system's semantics:
/// a variable's underscores become dashes (<c>body_text_color</c> is <c>--body-text-color</c>);
/// a value written <c>*name</c> is a reference, rendered as <c>var(--name)</c>; and a variable
/// whose name ends in <c>_dark</c> is the dark-mode value of the variable without the suffix,
/// collected into the dark block instead of the light one.
/// </para>
/// </remarks>
public class Theme
{
    /// <summary>How deep a chain of <c>*references</c> may go before the theme calls it
    /// circular.</summary> <remarks>The Python version's own limit, so that a theme that renders
    /// here renders there.</remarks>
    private const int MaxReferenceDepth = 100;

    /// <summary>A reference to another variable: <c>*name</c>, with <c>name</c> in the Python
    /// version's spelling.</summary>
    /// <remarks>
    /// The Python version's pattern is <c>(\*)([\w_]+)(\b)</c>, reproduced character for character:
    /// the trailing word boundary is what stops a reference at a character that could not be part
    /// of a variable name, and <c>\w</c> is Unicode-aware on both sides.
    /// </remarks>
    private static readonly Regex _reference = new(
        @"(\*)([\w_]+)(\b)",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private readonly List<string> _order = [];
    private readonly Dictionary<string, string?> _values = new(StringComparer.Ordinal);
    private readonly List<Font> _fonts = [];
    private readonly List<Font> _monoFonts = [];
    private readonly List<string> _fontCss = [];
    private readonly List<string> _stylesheets = [];

    /// <summary>Creates a theme with no variables but the two the Python version starts every theme
    /// with.</summary>
    /// <remarks>
    /// The Python version's <c>ThemeClass.__init__</c> gives a bare theme a <c>null</c> name and an
    /// empty custom stylesheet, and those are variables like any other: a theme that is never named
    /// fails to render here for the same reason it fails there.
    /// </remarks>
    public Theme()
    {
        Assign("name", null);
        Assign("custom_css", string.Empty);
    }

    /// <summary>The theme's name, which the stylesheet carries as <c>--name</c>.</summary>
    /// <value>
    /// <see langword="null"/> for a theme that was never named. <c>config.theme</c> reports this
    /// name, which is how the frontend knows which preset an application is using.
    /// </value>
    public string? Name
    {
        get => _values["name"];
        set => Assign("name", value);
    }

    /// <summary>CSS appended after the generated rules, as the Python version's <c>custom_css</c>
    /// is.</summary> <value>Empty, which appends nothing — not even the blank line a non-empty
    /// value adds.</value>
    public string CustomCss
    {
        get => _values["custom_css"] ?? string.Empty;
        set => Assign("custom_css", value);
    }

    /// <summary>The stylesheet URLs the theme's fonts need, in the order the fonts were declared.</summary>
    /// <remarks>
    /// A Google font that the vendored frontend does not carry is loaded from
    /// <c>fonts.googleapis.com</c> by URL, and the page lists it among its stylesheets; a local font
    /// needs no URL because its <c>@font-face</c> rules travel with the theme's CSS.
    /// </remarks>
    public IReadOnlyList<string> Stylesheets => _stylesheets;

    /// <summary>Sets the variables an application asked for, and defaults the rest.</summary>
    /// <param name="options">The values to set; a <see langword="null"/> property sets nothing.</param>
    /// <returns>This theme, so that calls chain.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// <para>
    /// The Python version's <c>Base.set()</c> assigns every variable it owns, using the argument
    /// when one was given and the inherited or default value otherwise. Two things follow, and both
    /// are reproduced: a variable that is already set keeps its value (unless the Python version
    /// reads its default from a differently-spelled name, which four of them do), and a variable
    /// that is not set yet takes its place at the end of the stylesheet in the Python version's
    /// assignment order.
    /// </para>
    /// <para>
    /// A property set to the empty string counts as not set, because the Python version's
    /// assignment is <c>value or default</c> and an empty string is false in Python. There is no
    /// way to give a variable an empty value through this method, the Python version or here;
    /// <see cref="Set(string,string?)"/> is the escape hatch.
    /// </para>
    /// </remarks>
    public Theme Set(ThemeOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        foreach (var variable in ThemeDefaults.Settable)
        {
            // The Python version reads the inherited value from `Fallback`, which for all but four variables
            // is the variable itself; reading it from itself is the same as leaving it alone.
            if (variable.Fallback == variable.Name && _values.ContainsKey(variable.Name))
            {
                continue;
            }

            var inherited = variable.Fallback != variable.Name && _values.TryGetValue(variable.Fallback, out var other)
                ? other
                : variable.Value;

            Assign(variable.Name, inherited);
        }

        foreach (var (name, value) in options.Assigned())
        {
            Assign(name, value);
        }

        return this;
    }

    /// <summary>Sets one variable by the Python version's name.</summary>
    /// <param name="variable">The variable's name, with underscores as the Python version spells
    /// it.</param>
    /// <param name="value">
    /// The value, or <see langword="null"/> to leave a dark variable to inherit its light value.
    /// </param>
    /// <returns>This theme, so that calls chain.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="variable"/> is
    /// <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="variable"/> is not a variable the Python
    /// version has.</exception>
    /// <remarks>
    /// The typed <see cref="Set(ThemeOptions)"/> is the path an application should take; this is the
    /// one for a value that arrives as data — a theme file, a configuration entry — where the name
    /// is not known until it is read. An unknown name is refused rather than added, because a
    /// misspelt variable would otherwise become a declaration nothing reads.
    /// </remarks>
    public Theme Set(string variable, string? value)
    {
        ArgumentNullException.ThrowIfNull(variable);

        if (!ThemeDefaults.Names.Contains(variable))
        {
            throw new ArgumentException(
                $"'{variable}' is not a theme variable. The theme has {ThemeDefaults.Names.Count} of "
                + "them, all of which ThemeOptions declares.",
                nameof(variable));
        }

        Assign(variable, value);

        return this;
    }

    /// <summary>Reads a variable's value, before any <c>*reference</c> in it is resolved.</summary>
    /// <param name="variable">The variable's name.</param>
    /// <returns>The value as it was set, or <see langword="null"/> when the theme does not carry it.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="variable"/> is <see langword="null"/>.</exception>
    public string? GetValue(string variable)
    {
        ArgumentNullException.ThrowIfNull(variable);

        return _values.TryGetValue(variable, out var value) ? value : null;
    }

    /// <summary>
    /// Resolves a variable to the value it ends up meaning, following <c>*references</c>.
    /// </summary>
    /// <param name="variable">The variable's name, with underscores as the Python version spells
    /// it.</param> <returns>The value with every reference replaced by what it refers to.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="variable"/> is
    /// <see langword="null"/>.</exception> <exception cref="InvalidOperationException">The variable
    /// holds <see langword="null"/>, which no value can be computed from.</exception>
    /// <remarks>
    /// <para>
    /// This is what <c>config.body_css</c> carries: the page paints itself with these four values
    /// before the stylesheet arrives, so they cannot be <c>var(--...)</c> — they have to be the
    /// colours themselves.
    /// </para>
    /// <para>
    /// A dark variable resolves its references against dark variables, and falls back to the light
    /// value of the same name when it has no dark value of its own. That is the Python version's
    /// rule, and it is why a theme that sets only the light half of a pair still renders a dark
    /// mode.
    /// </para>
    /// </remarks>
    public string GetComputedValue(string variable) => Compute(variable, depth: 0);

    /// <summary>Renders the stylesheet the page loads from <c>/theme.css</c>.</summary>
    /// <returns>The font rules, the light variables, the dark variables, and any custom CSS.</returns>
    /// <exception cref="InvalidOperationException">
    /// A variable that is not a dark variable holds <see langword="null"/>, or a reference breaks
    /// one of the rules the dark-mode derivation depends on.
    /// </exception>
    /// <remarks>
    /// <para>
    /// The output is the Python version's, down to the blank lines: the fonts, then <c>:root { …
    /// }</c>, then <c>:root.dark, :root .dark { … }</c>, then the custom CSS if there is any. The
    /// dark block carries every light variable too, so that a dark-mode rule can override one
    /// without the others falling back to a browser default.
    /// </para>
    /// <para>
    /// One quirk is reproduced rather than tidied: a dark variable whose value is <c>null</c> is
    /// written into the dark block as <c>--name_with_underscores: None;</c>. It is invalid CSS and
    /// a browser ignores it, which is exactly what the Python version's output does — and the light
    /// value of the same variable is still emitted, so the declaration that matters is there.
    /// </para>
    /// </remarks>
    public string ToCss()
    {
        var light = new OrderedVariables();
        var dark = new OrderedVariables();

        foreach (var name in _order)
        {
            var value = _values[name];

            if (value is null)
            {
                if (!name.EndsWith("_dark", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        $"Cannot set '{name}' to null - only dark mode variables can be null.");
                }

                // The Python version records the un-suffixed name and stops before the underscores are turned
                // into dashes, so this entry is written as it was spelled and renders as `None`.
                dark.Set(name[..^5], null);

                continue;
            }

            var resolved = ResolveReferences(name, value);
            var cssName = name.Replace('_', '-');

            if (cssName.EndsWith("-dark", StringComparison.Ordinal))
            {
                dark.Set(cssName[..^5], resolved);
            }
            else
            {
                light.Set(cssName, resolved);
            }
        }

        foreach (var (name, value) in light)
        {
            dark.SetIfAbsent(name, value);
        }

        var css = new StringBuilder();

        css.Append(string.Join("\n", _fontCss)).Append('\n');
        css.Append(":root {\n").Append(Rules(light)).Append("\n}");
        css.Append('\n');
        css.Append("\n:root.dark, :root .dark {\n").Append(Rules(dark)).Append("\n}");

        if (CustomCss.Length > 0)
        {
            css.Append("\n\n").Append(CustomCss);
        }

        return css.ToString();
    }

    /// <summary>Serialises the theme the way the Python version's <c>to_dict()</c> does.</summary>
    /// <returns>
    /// A document with the theme's variables and its Python version, ready to be written to a
    /// file and read back by <see cref="FromDictionary"/>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// The shape is the Python version's, and it is JSON rather than a dictionary of strings
    /// because two of the entries are not strings: the font stacks are lists, and a font in them is
    /// a tagged object (<c>__gradio_font__</c>) that says whether it is a local font and which
    /// weights it carries. That is the Python version's own encoding, which is what makes a theme
    /// file portable between the two implementations.
    /// </para>
    /// <para>
    /// The keys are sorted, because the Python version builds them by walking <c>dir(self)</c>; the
    /// order is not meaningful, but it is stable, which is what a file in a repository wants.
    /// </para>
    /// </remarks>
    public JsonObject ToDictionary()
    {
        var fonts = new JsonArray();
        foreach (var font in _fonts)
        {
            fonts.Add(font.ToJson());
        }

        var monoFonts = new JsonArray();
        foreach (var font in _monoFonts)
        {
            monoFonts.Add(font.ToJson());
        }

        var fontCss = new JsonArray();
        foreach (var rule in _fontCss)
        {
            fontCss.Add(rule);
        }

        var stylesheets = new JsonArray();
        foreach (var sheet in _stylesheets)
        {
            stylesheets.Add(sheet);
        }

        var theme = new JsonObject
        {
            ["_font"] = fonts,
            ["_font_css"] = fontCss,
            ["_font_mono"] = monoFonts,
            ["_stylesheets"] = stylesheets,
        };

        foreach (var name in _values.Keys.Order(StringComparer.Ordinal))
        {
            // The Python version's to_dict keeps the attributes that hold a string or a list, so a dark
            // variable left to inherit — which holds None — is absent from the document rather
            // than present and null.
            if (_values[name] is { } value)
            {
                theme[name] = value;
            }
        }

        return new JsonObject
        {
            ["theme"] = theme,
            ["gradio_version"] = GradioVersion.Upstream,
        };
    }

    /// <summary>Reads a theme back from <see cref="ToDictionary"/>'s output.</summary>
    /// <param name="document">The document.</param>
    /// <returns>The theme.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="document"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The document has no <c>theme</c> object, or names a variable that does not exist.</exception>
    /// <remarks>
    /// <para>
    /// A variable the document leaves out is filled in from <see cref="BaseTheme"/>, which is what
    /// makes a theme file written against an older release still render: it is a set of overrides,
    /// not a complete definition.
    /// </para>
    /// <para>
    /// The Python version warns when the file was written for a different major version. There is
    /// no warning channel here — the library logs nothing of its own — so the version is read but
    /// not acted on; the variables that changed shape are the ones that will show up as a
    /// difference.
    /// </para>
    /// </remarks>
    public static Theme FromDictionary(JsonObject document)
    {
        ArgumentNullException.ThrowIfNull(document);

        if (document["theme"] is not JsonObject values)
        {
            throw new ArgumentException("The document carries no 'theme' object.", nameof(document));
        }

        var theme = new Theme();

        foreach (var (name, node) in values)
        {
            switch (name)
            {
                case "_font":
                    theme._fonts.AddRange(ReadFonts(node));
                    break;

                case "_font_mono":
                    theme._monoFonts.AddRange(ReadFonts(node));
                    break;

                case "_font_css":
                    theme._fontCss.AddRange(ReadStrings(node, name));
                    break;

                case "_stylesheets":
                    theme._stylesheets.AddRange(ReadStrings(node, name));
                    break;

                default:
                    theme.Set(name, node?.GetValue<string>());
                    break;
            }
        }

        // Whatever the document leaves out is filled in from the base theme, so that a file
        // written against an older release still renders as the theme it describes. The order is
        // the base theme's own, which is the Python version's: it fills from `Base().__dict__`, and a
        // document written by `to_dict()` is alphabetical, so neither side can keep the order the
        // original theme assigned its variables in.
        var defaults = new BaseTheme();
        foreach (var name in ThemeDefaults.FromConstructor.Concat(ThemeDefaults.Settable.Select(variable => variable.Name)))
        {
            if (!theme._values.ContainsKey(name))
            {
                theme.Assign(name, defaults._values[name]);
            }
        }

        return theme;
    }

    /// <summary>Reads a theme from a file written by <see cref="Dump"/>.</summary>
    /// <param name="path">The file's path.</param>
    /// <returns>The theme.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
    /// <exception cref="FileNotFoundException">There is no such file.</exception>
    /// <exception cref="ArgumentException">The file is not a theme document.</exception>
    public static Theme Load(string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        var document = JsonNode.Parse(System.IO.File.ReadAllText(path, Encoding.UTF8)) as JsonObject
            ?? throw new ArgumentException($"'{path}' does not hold a theme document.", nameof(path));

        return FromDictionary(document);
    }

    /// <summary>Writes the theme to a file that <see cref="Load"/> can read.</summary>
    /// <param name="path">The file's path.</param>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
    public void Dump(string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        System.IO.File.WriteAllText(path, ToDictionary().ToJsonString(JsonText.Options), Encoding.UTF8);
    }

    /// <summary>Gives the theme its font stacks, and the CSS and stylesheet URLs they need.</summary>
    /// <param name="font">The families, in the order they are offered to the browser.</param>
    /// <param name="mono">The monospace families.</param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    /// <remarks>
    /// The two stacks are variables like any other — <c>--font</c> and <c>--font-mono</c> carry
    /// them — but they also produce CSS of their own: a local font's <c>@font-face</c> rules go into
    /// the stylesheet, and a remote one's URL goes into the page's stylesheet list.
    /// </remarks>
    protected void UseFonts(IReadOnlyList<Font> font, IReadOnlyList<Font> mono)
    {
        ArgumentNullException.ThrowIfNull(font);
        ArgumentNullException.ThrowIfNull(mono);

        _fonts.Clear();
        _fonts.AddRange(font);
        _monoFonts.Clear();
        _monoFonts.AddRange(mono);

        Assign("font", string.Join(", ", _fonts.Select(family => family.ToString())));
        Assign("font_mono", string.Join(", ", _monoFonts.Select(family => family.ToString())));

        _fontCss.Clear();
        _stylesheets.Clear();

        foreach (var family in _fonts.Concat(_monoFonts))
        {
            var stylesheet = family.Stylesheet();

            if (stylesheet.Css is not null)
            {
                _fontCss.Add(stylesheet.Css);
            }
            else if (stylesheet.Url is not null)
            {
                _stylesheets.Add(stylesheet.Url);
            }
        }
    }

    /// <summary>Writes a variable, keeping its place when it is already there.</summary>
    /// <param name="name">The variable's name.</param>
    /// <param name="value">The value.</param>
    protected void Assign(string name, string? value)
    {
        if (!_values.ContainsKey(name))
        {
            _order.Add(name);
        }

        _values[name] = value;
    }

    private string Compute(string variable, int depth)
    {
        if (depth > MaxReferenceDepth)
        {
            // The Python version warns and answers with nothing rather than failing, and a caller that gets
            // an empty value at least renders.
            return string.Empty;
        }

        var isDark = variable.EndsWith("_dark", StringComparison.Ordinal);

        // The Python version's `getattr(self, property, getattr(self, property[:-5], ""))`: the dark value
        // when there is one, the light value when there is not, and an empty string when neither.
        // A dark variable that is present and null is *not* missing — it is what the Python version fails on.
        string? value;
        if (_values.TryGetValue(variable, out var direct))
        {
            value = direct;
        }
        else if (isDark && _values.TryGetValue(variable[..^5], out var light))
        {
            value = light;
        }
        else
        {
            value = string.Empty;
        }

        if (value is null)
        {
            throw new InvalidOperationException(
                $"Cannot compute '{variable}': it is null, and only a dark variable may be null.");
        }

        var suffix = isDark ? "_dark" : string.Empty;

        return _reference.Replace(value, match => Compute(match.Groups[2].Value + suffix, depth + 1));
    }

    /// <summary>Turns every <c>*reference</c> in a value into <c>var(--name)</c>, enforcing the dark-mode rules.</summary>
    /// <param name="variable">The variable the value belongs to.</param>
    /// <param name="value">The value as it was set.</param>
    /// <returns>The value as the stylesheet spells it.</returns>
    /// <exception cref="InvalidOperationException">The value breaks one of the dark-mode reference rules.</exception>
    private static string ResolveReferences(string variable, string value) =>
        _reference.Replace(
            value,
            match =>
            {
                var reference = match.Groups[2].Value;

                if (reference.EndsWith("_dark", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        $"Cannot refer '{variable}' to '{value}' - dark variable references are "
                        + "automatically used for dark mode attributes, so do not use the _dark "
                        + "suffix in the value.");
                }

                if (variable.EndsWith("_dark", StringComparison.Ordinal)
                    && variable[..^5] == reference)
                {
                    throw new InvalidOperationException(
                        $"Cannot refer '{variable}' to '{value}' - if dark and light mode values are "
                        + "the same, set dark mode version to None.");
                }

                return $"var(--{reference.Replace('_', '-')})";
            });

    private static string Rules(OrderedVariables variables)
    {
        var rules = new List<string>();

        foreach (var (name, value) in variables)
        {
            rules.Add($"  --{name}: {value ?? "None"};");
        }

        return string.Join("\n", rules);
    }

    private static IEnumerable<Font> ReadFonts(JsonNode? node) =>
        node is JsonArray array
            ? array.Select(Font.FromJson)
            : throw new ArgumentException("A font stack must be an array.");

    private static IEnumerable<string> ReadStrings(JsonNode? node, string name) =>
        node is JsonArray array
            ? array.Select(entry => entry?.GetValue<string>()
                ?? throw new ArgumentException($"'{name}' carries a null entry."))
            : throw new ArgumentException($"'{name}' must be an array.");

    /// <summary>An ordered set of CSS variables, where a later write keeps the earlier position.</summary>
    /// <remarks>
    /// A dictionary cannot express this — <see cref="Dictionary{TKey,TValue}"/> enumerates in
    /// whatever order it likes — and the order is the stylesheet's.
    /// </remarks>
    private sealed class OrderedVariables
    {
        private readonly List<string> _order = [];
        private readonly Dictionary<string, string?> _values = new(StringComparer.Ordinal);

        internal void Set(string name, string? value)
        {
            if (!_values.ContainsKey(name))
            {
                _order.Add(name);
            }

            _values[name] = value;
        }

        internal void SetIfAbsent(string name, string? value)
        {
            if (!_values.ContainsKey(name))
            {
                Set(name, value);
            }
        }

        public IEnumerator<(string Name, string? Value)> GetEnumerator() =>
            _order.Select(name => (name, _values[name])).GetEnumerator();
    }
}

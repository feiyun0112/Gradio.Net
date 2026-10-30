using System.Globalization;
using System.Text;
using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// Renders the application page from the vendored template.
/// </summary>
/// <remarks>
/// <para>
/// The page the Python version serves is a Jinja template (<c>frontend/index.html</c> in the
/// vendored tree) filled with the application's config and its API info. The template is an
/// immutable asset, so it cannot be rewritten into another form; what this type replaces is the two
/// things Jinja did — evaluating the expressions the template contains, and HTML-escaping what they
/// produce the way Jinja's autoescaping does.
/// </para>
/// <para>
/// Only the constructs the template actually uses are implemented: a name, a chain of
/// <c>.get(key, default)</c> and <c>[key]</c> lookups, <c>or</c>, and the <c>default</c> and
/// <c>toorjson</c> filters. Anything else fails loudly, naming the expression: the template is
/// pinned by the build, so an unknown construct means the vendored frontend was replaced
/// without teaching this renderer about it, and answering with half a page would be worse than
/// answering with a failure.
/// </para>
/// <para>
/// <c>toorjson</c> is the Python version's own filter, not a convenience: the config and the API
/// info are embedded in the page as JavaScript, and the Python version's serializer leaves the text
/// as text (no non-ASCII escaping) while escaping the four characters that would let a value break
/// out of the script element. Both halves of that are reproduced here, because a browser that read
/// the bytes rather than parsing them would see a difference otherwise.
/// </para>
/// </remarks>
internal sealed class PageTemplate
{
    private static readonly char[] _scriptEscapes = ['<', '>', '&', '\''];

    private readonly List<Segment> _segments;

    /// <summary>Reads the template the assembly carries.</summary>
    /// <param name="assets">Where the template is read from.</param>
    /// <exception cref="ArgumentNullException"><paramref name="assets"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">The template is missing, or uses a construct this renderer does not implement.</exception>
    public PageTemplate(FrontendAssets assets)
    {
        ArgumentNullException.ThrowIfNull(assets);

        _segments = Parse(TrimTrailingNewline(assets.ReadText(FrontendAssets.PageAsset)));
    }

    /// <summary>
    /// Drops the newline the template file ends with, the way Jinja does.
    /// </summary>
    /// <param name="template">The template as the file has it.</param>
    /// <returns>The template as Jinja reads it.</returns>
    /// <remarks>
    /// Jinja's <c>keep_trailing_newline</c> is off by default, so the newline a file ends with is
    /// not part of what it renders. A page that kept it would be one character longer than
    /// the Python version's.
    /// </remarks>
    private static string TrimTrailingNewline(string template) =>
        template.EndsWith("\r\n", StringComparison.Ordinal)
            ? template[..^2]
            : template.EndsWith('\n') ? template[..^1] : template;

    /// <summary>Renders the page.</summary>
    /// <param name="baseUrl">What the document's relative URLs resolve against.</param>
    /// <param name="config">The config the page carries, as the endpoint assembled it.</param>
    /// <param name="apiInfo">The API info the page carries.</param>
    /// <returns>The page's HTML.</returns>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">An expression uses a construct this renderer does not implement.</exception>
    internal string Render(string baseUrl, JsonObject config, JsonObject apiInfo)
    {
        ArgumentNullException.ThrowIfNull(baseUrl);
        ArgumentNullException.ThrowIfNull(config);
        ArgumentNullException.ThrowIfNull(apiInfo);

        var page = new StringBuilder();
        var values = new PageValues(baseUrl, config, apiInfo);

        foreach (var segment in _segments)
        {
            page.Append(segment.IsExpression ? values.Evaluate(segment.Text) : segment.Text);
        }

        return page.ToString();
    }

    private static List<Segment> Parse(string template)
    {
        var segments = new List<Segment>();
        var index = 0;

        while (index < template.Length)
        {
            var open = template.IndexOf("{{", index, StringComparison.Ordinal);
            var statement = template.IndexOf("{%", index, StringComparison.Ordinal);
            var comment = template.IndexOf("{#", index, StringComparison.Ordinal);

            if (statement >= 0 && (open < 0 || statement < open))
            {
                throw Unsupported($"the statement '{Excerpt(template, statement)}'");
            }

            if (comment >= 0 && (open < 0 || comment < open))
            {
                throw Unsupported($"the comment '{Excerpt(template, comment)}'");
            }

            if (open < 0)
            {
                segments.Add(new Segment(template[index..], IsExpression: false));
                break;
            }

            if (open > index)
            {
                segments.Add(new Segment(template[index..open], IsExpression: false));
            }

            var close = template.IndexOf("}}", open + 2, StringComparison.Ordinal);
            if (close < 0)
            {
                throw Unsupported($"the unterminated expression '{template[open..]}'");
            }

            segments.Add(new Segment(template[(open + 2)..close].Trim(), IsExpression: true));
            index = close + 2;
        }

        return segments;
    }

    private static string Excerpt(string template, int index) =>
        template[index..Math.Min(template.Length, index + 40)];

    private static InvalidOperationException Unsupported(string construct) =>
        new($"The vendored page template uses {construct}, which this renderer does not implement. "
            + "The template is an immutable asset, so this means the frontend was "
            + "replaced without teaching PageTemplate about the construct.");

    private readonly record struct Segment(string Text, bool IsExpression);

    /// <summary>The names an expression in the template can refer to, and the evaluator for it.</summary>
    /// <remarks>
    /// One instance per rendered page: the values belong to the request, and the package keeps no
    /// state of its own.
    /// </remarks>
    private sealed class PageValues
    {
        private const string Config = "config";
        private const string ApiInfo = "gradio_api_info";
        private const string BaseUrl = "base_url";
        private const string Url = "url";

        private readonly string _baseUrl;
        private readonly JsonObject _config;
        private readonly JsonObject _apiInfo;

        internal PageValues(string baseUrl, JsonObject config, JsonObject apiInfo)
        {
            _baseUrl = baseUrl;
            _config = config;
            _apiInfo = apiInfo;
        }

        internal string Evaluate(string expression)
        {
            var (head, filters) = Split(expression, '|');

            var value = EvaluateOr(head.Trim());
            foreach (var filter in filters)
            {
                value = ApplyFilter(filter.Trim(), value);
            }

            return RenderValue(value);
        }

        private object? EvaluateOr(string expression)
        {
            var parts = Split(expression, " or ");

            object? value = EvaluateChain(parts[0].Trim());
            for (var index = 1; index < parts.Count; index++)
            {
                if (IsTruthy(value))
                {
                    return value;
                }

                value = EvaluateChain(parts[index].Trim());
            }

            return value;
        }

        private object? EvaluateChain(string expression)
        {
            var index = 0;
            SkipSpace(expression, ref index);

            if (index >= expression.Length)
            {
                throw Unsupported("the empty expression");
            }

            object? value = expression[index] switch
            {
                '\'' or '"' => ReadString(expression, ref index),
                _ => Lookup(ReadIdentifier(expression, ref index)),
            };

            while (index < expression.Length)
            {
                SkipSpace(expression, ref index);
                if (index >= expression.Length)
                {
                    break;
                }

                if (expression[index] == '.')
                {
                    index++;
                    var name = ReadIdentifier(expression, ref index);
                    SkipSpace(expression, ref index);

                    if (name != "get" || index >= expression.Length || expression[index] != '(')
                    {
                        throw Unsupported($"'.{name}'");
                    }

                    index++;
                    var arguments = ReadArguments(expression, ref index);
                    value = Get(value, arguments);
                }
                else if (expression[index] == '[')
                {
                    index++;
                    var key = ReadString(expression, ref index);
                    Expect(expression, ref index, ']');
                    value = Index(value, key);
                }
                else
                {
                    throw Unsupported($"'{expression}'");
                }
            }

            return value;
        }

        private object? Lookup(string name) => name switch
        {
            Config => _config,
            ApiInfo => _apiInfo,
            BaseUrl => _baseUrl,

            // The template reads `url` in a meta tag, and the Python version never passes it: Jinja's
            // undefined renders as nothing, which is what the served page carries.
            Url => Undefined.Instance,

            _ => throw Unsupported($"the name '{name}'"),
        };

        private static object? Get(object? receiver, List<object?> arguments)
        {
            var fallback = arguments.Count > 1 ? arguments[1] : Undefined.Instance;

            // A null the config carries is not the same thing as a missing key in the Python version, but the
            // template asks `.get(...)` of it either way, and the fallback is the only answer that
            // keeps the page renderable while a subsystem has not filled the value in yet.
            if (receiver is null || receiver is Undefined)
            {
                return fallback;
            }

            if (receiver is not JsonObject document)
            {
                throw Unsupported($"'.get' on {receiver.GetType().Name}");
            }

            var key = arguments[0] as string
                ?? throw Unsupported($"'.get({arguments[0]})'");

            return document.TryGetPropertyValue(key, out var value) ? value : fallback;
        }

        private static object? Index(object? receiver, string key)
        {
            if (receiver is not JsonObject document)
            {
                throw Unsupported($"'[{key}]' on {receiver?.GetType().Name ?? "None"}");
            }

            // Jinja answers a missing key with undefined rather than failing, which is how the
            // template's `config['thumbnail'] or ''` gets an empty string for a config that does
            // not carry the key.
            return document.TryGetPropertyValue(key, out var value) ? value : Undefined.Instance;
        }

        private static object? ApplyFilter(string filter, object? value)
        {
            var (name, arguments) = ParseFilter(filter);

            switch (name)
            {
                case "default":
                    return value is Undefined && arguments.Count > 0 ? arguments[0] : value;

                case "toorjson":
                    return ToOrJson(value);

                default:
                    throw Unsupported($"the filter '{name}'");
            }
        }

        private static (string Name, List<object?> Arguments) ParseFilter(string filter)
        {
            var open = filter.IndexOf('(', StringComparison.Ordinal);
            if (open < 0)
            {
                return (filter, []);
            }

            var index = open + 1;
            var arguments = ReadArguments(filter, ref index);

            return (filter[..open].Trim(), arguments);
        }

        private static Markup ToOrJson(object? value)
        {
            if (value is not JsonNode document)
            {
                throw Unsupported($"'toorjson' on {value?.GetType().Name ?? "None"}");
            }

            var json = document.ToJsonString(JsonText.Options);

            // The Python version's filter escapes the four characters that could end the script element
            // early, and leaves everything else as text.
            var escaped = new StringBuilder(json.Length);
            foreach (var character in json)
            {
                if (Array.IndexOf(_scriptEscapes, character) >= 0)
                {
                    escaped.Append("\\u").Append(((int)character).ToString("x4", CultureInfo.InvariantCulture));
                }
                else
                {
                    escaped.Append(character);
                }
            }

            return new Markup(escaped.ToString());
        }

        private static List<object?> ReadArguments(string expression, ref int index)
        {
            var arguments = new List<object?>();

            SkipSpace(expression, ref index);
            if (index < expression.Length && expression[index] == ')')
            {
                index++;

                return arguments;
            }

            while (true)
            {
                SkipSpace(expression, ref index);

                if (index >= expression.Length)
                {
                    throw Unsupported($"the argument list '{expression}'");
                }

                if (expression[index] is '\'' or '"')
                {
                    arguments.Add(ReadString(expression, ref index));
                }
                else if (expression[index] == '{' && index + 1 < expression.Length && expression[index + 1] == '}')
                {
                    index += 2;
                    arguments.Add(new JsonObject());
                }
                else
                {
                    arguments.Add(ReadNumberOrName(expression, ref index));
                }

                SkipSpace(expression, ref index);
                if (index >= expression.Length)
                {
                    throw Unsupported($"the argument list '{expression}'");
                }

                if (expression[index] == ',')
                {
                    index++;
                    continue;
                }

                Expect(expression, ref index, ')');

                return arguments;
            }
        }

        private static object ReadNumberOrName(string expression, ref int index)
        {
            var start = index;
            while (index < expression.Length && (char.IsLetterOrDigit(expression[index]) || expression[index] is '.' or '_'))
            {
                index++;
            }

            var text = expression[start..index];
            if (text.Length == 0)
            {
                throw Unsupported($"the argument list '{expression}'");
            }

            if (long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var number))
            {
                return number;
            }

            if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var real))
            {
                return real;
            }

            throw Unsupported($"the name '{text}'");
        }

        private static string ReadString(string expression, ref int index)
        {
            var quote = expression[index];
            index++;

            var text = new StringBuilder();
            while (index < expression.Length)
            {
                if (expression[index] == quote)
                {
                    index++;

                    return text.ToString();
                }

                if (expression[index] == '\\' && index + 1 < expression.Length)
                {
                    index++;
                }

                text.Append(expression[index]);
                index++;
            }

            throw Unsupported($"the unterminated string '{expression}'");
        }

        private static string ReadIdentifier(string expression, ref int index)
        {
            var start = index;
            while (index < expression.Length && (char.IsLetterOrDigit(expression[index]) || expression[index] == '_'))
            {
                index++;
            }

            if (start == index)
            {
                throw Unsupported($"'{expression}'");
            }

            return expression[start..index];
        }

        private static void Expect(string expression, ref int index, char expected)
        {
            if (index >= expression.Length || expression[index] != expected)
            {
                throw Unsupported($"'{expression}'");
            }

            index++;
        }

        private static void SkipSpace(string expression, ref int index)
        {
            while (index < expression.Length && char.IsWhiteSpace(expression[index]))
            {
                index++;
            }
        }

        /// <summary>Splits on a separator that is not inside a string or a nested expression.</summary>
        private static (string Head, List<string> Tails) Split(string expression, char separator)
        {
            var tails = new List<string>();
            var depth = 0;
            var quote = '\0';
            var start = 0;

            for (var index = 0; index < expression.Length; index++)
            {
                var character = expression[index];

                if (quote != '\0')
                {
                    if (character == '\\')
                    {
                        index++;
                    }
                    else if (character == quote)
                    {
                        quote = '\0';
                    }

                    continue;
                }

                switch (character)
                {
                    case '\'' or '"':
                        quote = character;
                        break;

                    case '(' or '[' or '{':
                        depth++;
                        break;

                    case ')' or ']' or '}':
                        depth--;
                        break;

                    case var _ when character == separator && depth == 0:
                        tails.Add(expression[start..index]);
                        start = index + 1;
                        break;
                }
            }

            tails.Add(expression[start..]);

            return (tails[0], tails.Skip(1).ToList());
        }

        /// <summary>Splits on a word that is not inside a string or a nested expression.</summary>
        private static List<string> Split(string expression, string separator)
        {
            var parts = new List<string>();
            var depth = 0;
            var quote = '\0';
            var start = 0;

            for (var index = 0; index < expression.Length; index++)
            {
                var character = expression[index];

                if (quote != '\0')
                {
                    if (character == '\\')
                    {
                        index++;
                    }
                    else if (character == quote)
                    {
                        quote = '\0';
                    }

                    continue;
                }

                switch (character)
                {
                    case '\'' or '"':
                        quote = character;
                        break;

                    case '(' or '[' or '{':
                        depth++;
                        break;

                    case ')' or ']' or '}':
                        depth--;
                        break;

                    default:
                        if (depth == 0
                            && index + separator.Length <= expression.Length
                            && string.CompareOrdinal(expression, index, separator, 0, separator.Length) == 0)
                        {
                            parts.Add(expression[start..index]);
                            index += separator.Length - 1;
                            start = index + 1;
                        }

                        break;
                }
            }

            parts.Add(expression[start..]);

            return parts;
        }

        private static bool IsTruthy(object? value) => value switch
        {
            null or Undefined => false,
            Markup markup => markup.Text.Length > 0,
            string text => text.Length > 0,
            JsonArray array => array.Count > 0,
            JsonObject document => document.Count > 0,
            JsonValue json => IsTruthy(json),
            long number => number != 0,
            double number => number != 0,
            _ => true,
        };

        private static bool IsTruthy(JsonValue value)
        {
            if (value.TryGetValue<string>(out var text))
            {
                return text.Length > 0;
            }

            if (value.TryGetValue<bool>(out var flag))
            {
                return flag;
            }

            return true;
        }

        /// <summary>Renders a value the way Jinja's autoescaping would.</summary>
        private static string RenderValue(object? value) => value switch
        {
            null => "None",
            Undefined => string.Empty,
            Markup markup => markup.Text,
            string text => Escape(text),
            JsonValue json => RenderValue(json),
            _ => throw Unsupported($"the value of type {value.GetType().Name}"),
        };

        private static string RenderValue(JsonValue value)
        {
            if (value.TryGetValue<string>(out var text))
            {
                return Escape(text);
            }

            if (value.TryGetValue<bool>(out var flag))
            {
                return flag ? "True" : "False";
            }

            return value.ToJsonString();
        }

        /// <summary>Escapes what markupsafe escapes, which is what Jinja's autoescaping produces.</summary>
        private static string Escape(string text)
        {
            var escaped = new StringBuilder(text.Length);

            foreach (var character in text)
            {
                escaped.Append(character switch
                {
                    '&' => "&amp;",
                    '<' => "&lt;",
                    '>' => "&gt;",
                    '\'' => "&#39;",
                    '"' => "&#34;",
                    _ => character.ToString(),
                });
            }

            return escaped.ToString();
        }
    }

    /// <summary>Text that is already safe to write into the page.</summary>
    /// <remarks>
    /// The Python version's <c>toorjson</c> returns <c>markupsafe.Markup</c>, and Jinja does not
    /// escape markup: the JSON is written into the script element as the serializer produced it.
    /// </remarks>
    private sealed record Markup(string Text);

    /// <summary>Jinja's undefined: a value that is not there, and renders as nothing.</summary>
    private sealed class Undefined
    {
        internal static readonly Undefined Instance = new();

        private Undefined()
        {
        }
    }
}

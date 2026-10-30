using System.Text;
using System.Text.RegularExpressions;

namespace Gradio.Net;
/// <summary>
/// Translates between the Python version's path template syntax and ASP.NET Core's route pattern
/// syntax.
/// </summary>
/// <remarks>
/// <para>
/// The Python version writes parameters as <c>{name}</c> or <c>{name:converter}</c>, where the
/// converter <c>path</c> means "match the rest of the URL, slashes included". ASP.NET Core spells
/// the same idea <c>{name}</c> and <c>{**name}</c>, but requires a catch-all to make up a whole
/// segment: <c>/gradio_api/proxy={**url_path}</c> is rejected by the route parser, and the pattern
/// that can be built around the restriction is never matched.
/// </para>
/// <para>
/// So where the Python version has a literal prefix in front of a catch-all, the prefix becomes a
/// route constraint instead of a literal segment. The endpoint then matches exactly the URLs the
/// Python version matches — <c>/gradio_api/proxy=…</c> and nothing else — which is what the route
/// table is for; the URL is the contract, the way it is expressed is not.
/// </para>
/// </remarks>
internal static class RouteTemplate
{
    private const string PathConverter = "path";

    /// <summary>Converts a Python path template into an ASP.NET Core route pattern.</summary>
    internal static string ToAspNetCore(string upstreamPath)
    {
        var segments = upstreamPath.Split('/');
        var builder = new StringBuilder(upstreamPath.Length);

        for (var index = 0; index < segments.Length; index++)
        {
            if (index > 0)
            {
                builder.Append('/');
            }

            builder.Append(ConvertSegment(segments[index]));
        }

        return builder.ToString();
    }

    /// <summary>
    /// Reduces a template from either side to the comparable form.
    /// </summary>
    /// <remarks>
    /// Two equivalences are declared here rather than left to the caller, because both sides of
    /// the comparison have to agree on them:
    /// <list type="bullet">
    /// <item>
    /// every parameter becomes <c>{name}</c>, whatever its converter, catch-all marker or
    /// optionality;
    /// </item>
    /// <item>
    /// a trailing slash is dropped. The Python version registers <c>/config</c> and <c>/config/</c>
    /// as two routes; ASP.NET Core matches either URL to one endpoint, so the two are one route
    /// here and the trailing-slash URL is served by it.
    /// </item>
    /// </list>
    /// </remarks>
    internal static string Normalize(string template)
    {
        var reduced = RewriteParameters(template, static parameter =>
        {
            var name = parameter.TrimStart('*');
            var separator = name.IndexOf(':');

            return separator >= 0 ? name[..separator] : name;
        });

        return reduced.Length > 1 && reduced.EndsWith('/') ? reduced[..^1] : reduced;
    }

    private static string ConvertSegment(string segment)
    {
        if (segment.Length == 0 || !segment.Contains('{', StringComparison.Ordinal))
        {
            return segment;
        }

        var catchAll = CatchAllName(segment);
        if (catchAll is null)
        {
            return DropConverters(segment);
        }

        var prefix = segment[..segment.IndexOf('{', StringComparison.Ordinal)];
        if (prefix.Length == 0)
        {
            return "{**" + catchAll + "}";
        }

        // The literal prefix cannot stay a literal: a catch-all has to own its segment. As a
        // constraint it still decides what matches, and it is anchored so that the value has to
        // begin with it rather than merely contain it.
        return "{**" + catchAll + ":regex(^" + Regex.Escape(prefix) + ".*)}";
    }

    /// <summary>The parameter name when the segment ends in a whole-segment trailing <c>:path</c> parameter.</summary>
    private static string? CatchAllName(string segment)
    {
        var open = segment.IndexOf('{', StringComparison.Ordinal);
        if (open < 0 || !segment.EndsWith('}'))
        {
            return null;
        }

        var body = segment[(open + 1)..^1];
        var separator = body.IndexOf(':', StringComparison.Ordinal);
        if (separator < 0 || !string.Equals(body[(separator + 1)..], PathConverter, StringComparison.Ordinal))
        {
            return null;
        }

        var name = body[..separator];
        return name.Length == 0 ? null : name;
    }

    private static string DropConverters(string segment) =>
        RewriteParameters(segment, static parameter =>
        {
            var separator = parameter.IndexOf(':');

            return separator >= 0 ? parameter[..separator] : parameter;
        });

    /// <summary>
    /// Rewrites every <c>{...}</c> group of a template through <paramref name="nameOf"/>, leaving
    /// the rest of the text as it was.
    /// </summary>
    /// <remarks>
    /// Both the comparison form and the registered pattern are produced from the same scan, so
    /// there is one place that knows what a parameter looks like.
    /// </remarks>
    private static string RewriteParameters(string text, Func<string, string> nameOf)
    {
        var builder = new StringBuilder(text.Length);
        var index = 0;

        while (index < text.Length)
        {
            var open = text.IndexOf('{', index);
            if (open < 0)
            {
                builder.Append(text, index, text.Length - index);
                break;
            }

            var close = text.IndexOf('}', open);
            if (close < 0)
            {
                builder.Append(text, index, text.Length - index);
                break;
            }

            builder.Append(text, index, open - index);
            builder.Append('{').Append(nameOf(text[(open + 1)..close])).Append('}');

            index = close + 1;
        }

        return builder.ToString();
    }
}

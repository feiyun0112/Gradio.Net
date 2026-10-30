using System.Text;

namespace Gradio.Net.Tests.Shared;

/// <summary>
/// Turns a Python path template into a concrete URL that reaches that route.
/// </summary>
/// <remarks>
/// Parameter values are chosen to be unambiguous — a plain <c>a</c>, or a value that carries the
/// literal prefix the template requires — so that a request built from a template lands on the
/// route it was built from rather than on a neighbour that happens to match first. The tests
/// assert which route answered, so a collision would show up as a failure rather than as a false
/// pass.
/// </remarks>
internal static class RouteSampleUrl
{
    internal static string For(string upstreamPath)
    {
        var segments = upstreamPath.Split('/');

        for (var index = 0; index < segments.Length; index++)
        {
            segments[index] = FillSegment(segments[index]);
        }

        return string.Join('/', segments);
    }

    private static string FillSegment(string segment)
    {
        var open = segment.IndexOf('{', StringComparison.Ordinal);
        if (open < 0)
        {
            return segment;
        }

        var close = segment.IndexOf('}', open);
        var body = segment[(open + 1)..close];
        var separator = body.IndexOf(':', StringComparison.Ordinal);
        var isCatchAll = separator >= 0 && string.Equals(body[(separator + 1)..], "path", StringComparison.Ordinal);
        var prefix = segment[..open];

        var value = isCatchAll
            ? prefix.Length > 0 ? "https://example.com/a.png" : "a.txt"
            : "a";

        return prefix + value + segment[(close + 1)..];
    }
}

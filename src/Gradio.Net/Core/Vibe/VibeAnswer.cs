using System.Text.RegularExpressions;

namespace Gradio.Net;
/// <summary>
/// What the Vibe editor does with the text a model answers with.
/// </summary>
/// <remarks>
/// <para>
/// A model that has been asked for code answers with more than code: an explanation, sometimes a
/// block it thought in, sometimes the control tokens of whatever chat template trained it, and
/// sometimes the code wrapped in a fence. This is where all of that is taken apart, and it is the
/// part of the capability that can be read and checked without a model, a file or a request.
/// </para>
/// <para>
/// The order matters and is the Python version's, which is why the four steps are separate rather
/// than one pipeline: the markers are removed first (a think block can carry them), then the think
/// blocks, then the explanation is separated from the code, and only then is a fence unwrapped. The
/// Python version calls the first two in the opposite order for the starter questions, so the two
/// callers say which order they want rather than this type deciding for them.
/// </para>
/// <para>
/// The regular expressions are the Python version's, character for character. They are deliberately
/// loose about whitespace inside a tag — a model that writes <c>&lt; / reasoning &gt;</c> meant the
/// same thing — and deliberately case-insensitive for the same reason.
/// </para>
/// </remarks>
internal static partial class VibeAnswer
{
    /// <summary>The opening fence of the code block an answer may wrap its app in.</summary>
    private const string OpeningFence = "```python\n";

    /// <summary>Strips the markers a chat-tuned model wraps its answer in.</summary>
    /// <param name="raw">The answer.</param>
    /// <returns>The answer without them.</returns>
    /// <remarks>
    /// The Python version's <c>clean_out_markers</c>, which knows two shapes: a gpt-oss style
    /// answer that names where its final channel starts and ends, and anything else that carries
    /// control tokens, which are removed wherever they are. The first shape is taken as the answer
    /// and the analysis channel before it is dropped whole; the second is swept token by token.
    /// </remarks>
    internal static string WithoutMarkers(string raw)
    {
        if (raw.Length == 0)
        {
            return raw;
        }

        var finalStart = FinalChannel().Match(raw);

        if (finalStart.Success)
        {
            var text = raw[(finalStart.Index + finalStart.Length)..];
            var end = ChannelEnd().Match(text);

            return (end.Success ? text[..end.Index] : text).Trim();
        }

        return ControlToken().Replace(raw, string.Empty).Trim();
    }

    /// <summary>Removes the reasoning a model writes for itself.</summary>
    /// <param name="text">The answer.</param>
    /// <returns>The answer without its think blocks.</returns>
    /// <remarks>
    /// The Python version's <c>strip_think_blocks</c>. A think block is not the answer and not the
    /// explanation the editor shows: it is the model talking to itself, which the panel has nowhere
    /// to put.
    /// </remarks>
    internal static string WithoutThinkBlocks(string text) => ThinkBlock().Replace(text, string.Empty);

    /// <summary>Separates the explanation from the code.</summary>
    /// <param name="text">The answer.</param>
    /// <returns>The explanation, and what is left of the answer once it is taken out.</returns>
    /// <remarks>
    /// The Python version's <c>split_reasoning_code</c>: every reasoning block is collected, exact
    /// repeats are dropped, and the rest of the text is the code. The de-duplication is the Python
    /// version's, and it is there because a model that restates its explanation in two blocks has
    /// said it once.
    /// </remarks>
    internal static (string Reasoning, string Code) SplitReasoning(string text)
    {
        var chunks = new List<string>();
        var seen = new HashSet<string>(StringComparer.Ordinal);

        foreach (Match match in ReasoningBlock().Matches(text))
        {
            var body = match.Groups["body"].Value.Trim();

            if (body.Length > 0 && seen.Add(body))
            {
                chunks.Add(body);
            }
        }

        return (string.Join("\n\n", chunks).Trim(), ReasoningBlock().Replace(text, string.Empty).Trim());
    }

    /// <summary>Takes the code out of the fence a model may have wrapped it in.</summary>
    /// <param name="text">The answer, once its explanation is out.</param>
    /// <returns>The code the answer carries.</returns>
    /// <remarks>
    /// The Python version's extraction, which looks for a <c>```python</c> fence and, when it finds one, keeps
    /// what is between it and the next closing fence — or the rest of the answer when the model never
    /// closed it. An answer with no fence is taken as code already, which is what the rules ask for.
    /// The newline before the closing fence is not part of the code, so a fenced answer leaves a file
    /// that does not end with one.
    /// </remarks>
    internal static string ExtractCode(string text)
    {
        var start = text.IndexOf(OpeningFence, StringComparison.Ordinal);

        if (start < 0)
        {
            return text;
        }

        start += OpeningFence.Length;

        var end = text.IndexOf("\n```", start, StringComparison.Ordinal);

        return end < 0 ? text[start..] : text[start..end];
    }

    /// <summary>A control token, as a chat template writes one.</summary>
    [GeneratedRegex(@"<\|[^>]*\|>", RegexOptions.CultureInvariant)]
    private static partial Regex ControlToken();

    /// <summary>Where the final channel of a gpt-oss style answer starts.</summary>
    [GeneratedRegex(@"<\|start\|>assistant<\|channel\|>final<\|message\|>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex FinalChannel();

    /// <summary>Where an answer's final channel ends.</summary>
    [GeneratedRegex(@"<\|end\|>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex ChannelEnd();

    /// <summary>A block of explanation.</summary>
    [GeneratedRegex(@"<\s*reasoning\s*>\s*(?<body>.*?)\s*<\s*/\s*reasoning\s*>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant)]
    private static partial Regex ReasoningBlock();

    /// <summary>A block a model thinks in.</summary>
    [GeneratedRegex(@"<\s*think\s*>.*?<\s*/\s*think\s*>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant)]
    private static partial Regex ThinkBlock();
}

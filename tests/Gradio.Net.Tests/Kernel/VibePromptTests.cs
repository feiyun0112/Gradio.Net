using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Kernel;

/// <summary>
/// The text a Vibe prompt is made of, checked for the one thing a checkout can change: its line
/// endings.
/// </summary>
/// <remarks>
/// <para>
/// A raw string literal spells the endings of the file it was written in, and this repository stores
/// LF while a Windows working copy may check the same file out as CRLF. Without normalization the
/// text a model is asked with would depend on the machine that built the application — the rules,
/// the separators between the parts, and therefore every assertion about a prompt. It is LF on every
/// operating system here, which is what the Python version sends as well: its tokenizer rewrites a
/// literal's endings.
/// </para>
/// <para>
/// The values put into a prompt are a different matter, and are carried as they came — the file an
/// edit is asked about is a file, and rewriting its endings on the way to the model would make the
/// answer disagree with it line for line, which the diff counts as a rewrite of the whole file.
/// </para>
/// </remarks>
public sealed class VibePromptTests
{
    [Fact]
    public void The_rules_carry_no_carriage_return()
    {
        Assert.DoesNotContain("\r", VibePrompt.RewriteRules, StringComparison.Ordinal);
        Assert.DoesNotContain("\r", VibePrompt.StarterQueryRules, StringComparison.Ordinal);
    }

    [Fact]
    public void A_prompt_carries_no_carriage_return_of_its_own()
    {
        var rewrite = VibePrompt.RewritePrompt("a\nb", "Add a title", string.Empty);
        var starters = VibePrompt.StarterQueriesPrompt("a\nb");

        Assert.DoesNotContain("\r", rewrite, StringComparison.Ordinal);
        Assert.DoesNotContain("\r", starters, StringComparison.Ordinal);

        // The three parts are in place, separated the way the Python version's f-string separates
        // them — which is the text a model reads, so it is asserted rather than assumed.
        Assert.Contains("Existing code:\n```python\na\nb\n```", rewrite, StringComparison.Ordinal);
        Assert.Contains("Prompt:\nAdd a title", rewrite, StringComparison.Ordinal);
        Assert.Contains("History:\nNo chat history.", rewrite, StringComparison.Ordinal);
        Assert.Contains("Existing code:\n```python\na\nb\n```", starters, StringComparison.Ordinal);
    }

    [Fact]
    public void A_part_is_carried_as_it_came()
    {
        var rewrite = VibePrompt.RewritePrompt("a\r\nb", "Add a title", "\nUser: x\nAssistant: y\n");

        // A file that uses CRLF reaches the model as it is: the normalization is the literal's, not
        // the prompt's, which is the line the Python version's tokenizer draws too.
        Assert.Contains("Existing code:\n```python\na\r\nb\n```", rewrite, StringComparison.Ordinal);
        Assert.Contains("History:\n\nUser: x\nAssistant: y\n", rewrite, StringComparison.Ordinal);
    }
}

using System.Globalization;
using System.Text;

namespace Gradio.Net;
/// <summary>
/// The text the Vibe editor puts in front of a model: the rules, the two prompts, and the trimming
/// of the conversation that goes into them.
/// </summary>
/// <remarks>
/// <para>
/// The Python version writes all of this inline in <c>routes.py</c>. It is here as its own type
/// because it is the part of the capability that can be read and checked without a model, a file or
/// a request — and because the routes that use it read better when the shape of a prompt is not
/// buried between two <c>await</c>s. What happens to the answer is <see cref="VibeAnswer"/>.
/// </para>
/// <para>
/// The texts are the Python version's. Two differences are deliberate: the Python version's
/// <c>load_system_prompt</c> fetches <c>https://www.gradio.app/llms.txt</c> at request time and
/// brackets the rules with it, and nothing here reaches the network — so the rules are sent once,
/// without the documentation they would have been wrapped around. (When that fetch fails, which is
/// the case for an offline deployment, the Python version sends the rules twice.)
/// </para>
/// <para>
/// The rules are the Python version's words, reflowed: the Python version's copy of the second set
/// carries the indentation of the Python source it is written in, and both copies end with the
/// whitespace their triple quotes leave behind. Indentation and trailing whitespace are the one
/// thing not reproduced, because a model reads the words.
/// </para>
/// <para>
/// The line endings are LF on every operating system, which is what <see cref="Lf"/> is for. A raw
/// string literal spells the endings of the file it was written in, and the repository stores LF
/// while a Windows working copy checks the file out as CRLF — so without the normalization the text
/// a model is asked with would depend on which machine built it, and a checkout on Windows would
/// answer with a prompt no other machine ever sends. The Python version gets LF for free: its
/// tokenizer rewrites a literal's endings, which is the behaviour being reproduced here.
/// </para>
/// </remarks>
internal static class VibePrompt
{
    /// <summary>The line ending every prompt uses.</summary>
    private const string LineBreak = "\n";

    /// <summary>Reads a raw string literal as the text it spells, with LF line endings.</summary>
    /// <param name="literal">The literal's text.</param>
    /// <returns>The same text with every CRLF written as LF.</returns>
    /// <remarks>
    /// Applied to the literals and not to the finished prompt, which is the distinction the Python
    /// version draws: its tokenizer rewrites the endings inside a literal and leaves the values
    /// interpolated into one alone. That matters for the code an edit is asked about — it is a file,
    /// and rewriting its endings on the way to the model would make the answer disagree with the file
    /// line for line, which the diff counts as a rewrite of the whole file.
    /// </remarks>
    private static string Lf(string literal) => literal.Replace("\r\n", LineBreak, StringComparison.Ordinal);

    /// <summary>The rules an answer is written under.</summary>
    /// <remarks>
    /// The Python version's <c>prompt_rules</c>, verbatim down to the two spaces after the first
    /// sentence's full stop, because a model reads this text and any change to it is a change to
    /// the answers.
    /// </remarks>
    internal static readonly string RewriteRules =
        Lf("""
        Generate code for using the Gradio python library.

        The following RULES must be followed.  Whenever you are forming a response, ensure all rules have been followed otherwise start over.

        RULES:
        Respond with code written in valid Python syntax, along with one coherent explanation surrounded by <reasoning> tags.
        Any text that is not code, should be surrounded by one large <reasoning> tag.
        Never include backticks in your response such as ``` or ```python.
        Do not include any code that is not necessary for the app to run.
        Respond with a full Gradio app.
        Respond with a full Gradio app using correct syntax and features of the latest Gradio version. DO NOT write code that doesn't follow the signatures listed.
        Do not add comments explaining the code, unless they are very necessary to understand the code.
        Make sure the code includes all necessary imports.
        Clearly explain the changes, summary, or reasoning for the code you respond with, inside one large <reasoning> tag. Make sure it's easy to parse. Use markdown formatting when it makes sense, including bullet points if there are multiple changes.


        Here's an example of a valid response:

        <reasoning>
        I created a simple Gradio app that greets the user. It defines a function then creates a gradio interface and launches it.
        </reasoning>

        import gradio as gr

        def greet(name):
            return "Hello " + name + "!"

        demo = gr.Interface(fn=greet, inputs="textbox", outputs="textbox")

        demo.launch()
        """);

    /// <summary>The rules a list of starter questions is written under.</summary>
    /// <remarks>
    /// The Python version's <c>prompt_rules</c> for <c>starter_queries=True</c>. The example
    /// answers at the end are the shape the editor's chips are drawn from, so the count — at most
    /// three, one per line — is part of the instruction rather than a detail of it.
    /// </remarks>
    internal static readonly string StarterQueryRules =
        Lf("""
        You are a prompt generator for a gradio vibe editor.

        Given python code of a gradio app, return a list of starter queries that can be used to generate new code.
        Make sure the queries are short, useful and actually possible with Gradio.
        The queries should be really simple and easy to understand.
        You should respond with at most three queries, each on a new line. Do not include any other text.
        Make sure the features you suggest are actually supported by Gradio, and documented in the docs section below.
        Never suggest a query with more than one gradio feature or concept.
        You may suggest queries that are not related to Gradio, but they must be related to the existing code and app. Never suggest queries that require external packages or libraries other than gradio.
        Don't suggest adding a clear button if the app is an Interface, because Interface already has a clear button.

        Here's an example of a gradio app:

        ```python
        import gradio as gr

        def greet(name):
            return "Hello " + name + "!"

        demo = gr.Interface(fn=greet, inputs="textbox", outputs="textbox")

        demo.launch()
        ```

        Here's an example of a valid response:
        Add a title to the app
        Add examples
        Rewrite this app using Blocks

        Here's an example of another valid response:
        Add another textbox for name
        Change the theme
        Greet the user in many languages
        """);

    /// <summary>The most exchanges the prompt carries.</summary>
    /// <remarks>
    /// The Python version's <c>max_pairs</c> default. An older exchange is not part of what the
    /// next instruction is about, and a transcript that grows without a bound eventually crowds out
    /// the code.
    /// </remarks>
    private const int HistoryPairs = 5;

    /// <summary>The prompt a rewrite is asked with, with its three parts left to be filled in.</summary>
    /// <remarks>
    /// The Python version's f-string, including the blank first and last lines it opens and closes
    /// with: the text is a model's input rather than a document, but it is still the same text. It is
    /// a template rather than an interpolated string because the literal is normalized and the three
    /// parts are put in as they came — <see cref="Lf"/> says why — and an interpolation hole has no
    /// way to say that. The parts are <c>code</c>, <c>prompt</c> and the conversation.
    /// </remarks>
    private static readonly CompositeFormat RewriteTemplate =
        CompositeFormat.Parse(Lf("""

            You are a code generator for Gradio apps. Given the following existing code and prompt, return the full new code.
            Existing code:
            ```python
            {0}
            ```

            Prompt:
            {1}

            History:
            {2}

            """));

    /// <summary>The prompt a list of starter questions is asked with, with its code left to be filled
    /// in.</summary>
    private static readonly CompositeFormat StarterQueriesTemplate =
        CompositeFormat.Parse(Lf("""

            You are a prompt generator for a gradio vibe editor. Given the following existing code, return a list of starter queries that can be used to generate a new code.
            Existing code:
            ```python
            {0}
            ```

            """));

    /// <summary>Builds the prompt a rewrite is asked with.</summary>
    /// <param name="code">The code as it stands.</param>
    /// <param name="prompt">What the user asked for.</param>
    /// <param name="history">The conversation so far.</param>
    /// <returns>The prompt.</returns>
    internal static string RewritePrompt(string code, string prompt, string history) =>
        string.Format(
            CultureInfo.InvariantCulture,
            RewriteTemplate,
            code,
            prompt,
            history.Length == 0 ? "No chat history." : history);

    /// <summary>Builds the prompt a list of starter questions is asked with.</summary>
    /// <param name="code">The code as it stands.</param>
    /// <returns>The prompt.</returns>
    internal static string StarterQueriesPrompt(string code) =>
        string.Format(CultureInfo.InvariantCulture, StarterQueriesTemplate, code);

    /// <summary>Trims a conversation to its last few exchanges.</summary>
    /// <param name="history">The conversation so far.</param>
    /// <returns>The part of it the next prompt carries.</returns>
    /// <remarks>
    /// The Python version's <c>limit_chat_history</c>, including its quirk: the transcript is split
    /// on the marker <c>\nUser: </c>, so the first piece of a transcript that opens with that
    /// marker is the empty string — which is why the re-joined text is prefixed with <c>User: </c>
    /// when the first kept piece does not already start with it. The result reads the same as the
    /// text it came from.
    /// </remarks>
    internal static string LimitHistory(string history)
    {
        if (history.Trim().Length == 0)
        {
            return string.Empty;
        }

        var messages = history.Split("\nUser: ", StringSplitOptions.None);

        if (messages.Length <= HistoryPairs)
        {
            return history;
        }

        var recent = messages[^HistoryPairs..];

        var kept = recent[0].StartsWith("User: ", StringComparison.Ordinal)
            ? recent[0]
            : "User: " + recent[0];

        foreach (var message in recent[1..])
        {
            kept += "\nUser: " + message;
        }

        return kept;
    }
}

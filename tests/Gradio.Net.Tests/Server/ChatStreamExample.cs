using Gradio.Net;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// A chat that answers in two pieces, with the second held back until the test says so.
/// </summary>
/// <remarks>
/// <para>
/// It exists for the browser test, and it is built around what that test has to be able to see: the
/// page showing the first piece while the second has not been produced yet. Holding the second
/// piece back rather than delaying it is what makes that an observation instead of a race — a
/// sleep would make the test pass or fail depending on how fast the machine is.
/// </para>
/// <para>
/// The two pieces are words that only appear in the conversation, so a text search for one is a
/// search of what the chat is showing.
/// </para>
/// </remarks>
internal sealed class ChatStreamExample
{
    /// <summary>The text the browser types into the prompt.</summary>
    internal const string Prompt = "go";

    /// <summary>The answer as it arrives first.</summary>
    internal const string First = "alpha";

    /// <summary>The answer once the second piece has arrived.</summary>
    internal const string Second = "beta";

    private readonly TaskCompletionSource _rest = new(TaskCreationOptions.RunContinuationsAsynchronously);

    /// <summary>Lets the handler produce the rest of the answer.</summary>
    internal void Release() => _rest.TrySetResult();

    /// <summary>Builds the page into an application.</summary>
    /// <param name="blocks">The application to render into.</param>
    internal void Build(Blocks blocks)
    {
        blocks.Title = "Chat stream example";

        Textbox prompt;
        Chatbot chat;

        using (blocks.BeginRender())
        {
            prompt = blocks.Add(new Textbox { Label = "Prompt", SubmitBtn = true });
            chat = blocks.Add(new Chatbot { Label = "Chat" });
        }

        prompt.Submit(
            (Func<string, IReadOnlyList<ChatMessage>, IAsyncEnumerable<IReadOnlyList<ChatMessage>>>)Write,
            [prompt, chat],
            [chat],
            apiName: "write");
    }

    /// <summary>Answers in two pieces: the first at once, the second when the test allows it.</summary>
    private async IAsyncEnumerable<IReadOnlyList<ChatMessage>> Write(
        string text,
        IReadOnlyList<ChatMessage> history)
    {
        var conversation = new List<ChatMessage>(history)
        {
            new(text, ChatRole.User),
            new(First),
        };

        yield return conversation;

        await _rest.Task;

        conversation = [.. conversation[..^1], new ChatMessage($"{First} {Second}")];

        yield return conversation;
    }
}

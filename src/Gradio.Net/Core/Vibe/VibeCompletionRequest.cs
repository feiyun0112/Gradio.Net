namespace Gradio.Net;
/// <summary>
/// One completion the Vibe editor asks for.
/// </summary>
/// <remarks>
/// <para>
/// The two prompts are separate fields rather than a list of messages because the editor always
/// sends exactly one of each, and the Python version's request says so: a system message carrying
/// the rules and a user message carrying the code and the instruction.
/// </para>
/// <para>
/// It is a record so that a backend can log or replay what it was asked without re-deriving it, and
/// so that a test can compare two requests as values.
/// </para>
/// </remarks>
/// <param name="Model">The model to ask for, as <see cref="VibeOptions.Model"/> names it.</param>
/// <param name="SystemPrompt">The rules the answer is written under.</param>
/// <param name="UserPrompt">The code, the instruction, and the conversation so far.</param>
/// <param name="MaxTokens">How many tokens the answer may use.</param>
public sealed record VibeCompletionRequest(
    string Model,
    string SystemPrompt,
    string UserPrompt,
    int MaxTokens);

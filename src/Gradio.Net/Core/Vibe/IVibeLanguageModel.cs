namespace Gradio.Net;
/// <summary>
/// The language model the Vibe editor asks for code, as an interface rather than a vendor.
/// </summary>
/// <remarks>
/// <para>
/// The Python version builds a Hugging Face <c>InferenceClient</c> and calls
/// <c>chat_completion(model="openai/gpt-oss-120b", …)</c>, which makes the editor unusable without
/// a Hugging Face account and a network path to it. The framework here promises that nothing third
/// party is required at runtime, so the call is an interface: an application registers its own
/// implementation — a local model, a gateway, a vendor SDK — and the editor asks it.
/// </para>
/// <para>
/// Nothing is registered by default, and that is deliberate: an editor that quietly reached for some
/// service would be exactly the dependency this abstraction exists to remove. An application that
/// turns the editor on without registering a backend is answered with an error that says so, rather
/// than with a model it did not choose.
/// </para>
/// <para>
/// The shape of the call is the Python version's, narrowed to the part the editor actually varies:
/// a system instruction, a user prompt, the model's name, and a token budget. The conversation is
/// always those two messages — the editor keeps no conversation in the request; what it remembers
/// of one it folds into the prompt as text.
/// </para>
/// </remarks>
public interface IVibeLanguageModel
{
    /// <summary>Asks the model to answer a prompt.</summary>
    /// <param name="request">What to ask, and with what budget.</param>
    /// <param name="cancellationToken">Cancels the call when the browser goes away.</param>
    /// <returns>
    /// The answer, or <see langword="null"/> when the backend has nothing to say — which is
    /// answered as the Python version's "Error generating code", because an empty answer is not a
    /// rewrite.
    /// </returns>
    /// <remarks>
    /// An implementation that fails should throw: the failure is reported as a failed generation and
    /// the reason is written to the log, rather than being dressed up as an empty answer.
    /// </remarks>
    Task<string?> CompleteAsync(VibeCompletionRequest request, CancellationToken cancellationToken = default);
}

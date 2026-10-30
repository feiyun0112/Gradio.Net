using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// A component whose value the session holds rather than the client.
/// </summary>
/// <remarks>
/// <para>
/// This is the Python version's <c>stateful</c> component: the frontend never sends a value for
/// one, and the framework reads the session's instead. It is an interface rather than a flag on
/// <see cref="Component"/> because the behaviour it asks for is not "this component has a value" —
/// every component does — but "the value is not the client's to send", and the four questions that
/// follow from it belong together.
/// </para>
/// <para>
/// The value the session holds is the framework's own: a <c>State</c> declares it as JSON, so what
/// is stored is the JSON tree, and <see cref="ReadState"/> is what turns it into what a handler
/// works with. That keeps one representation in the session — the same one that would cross the
/// wire if the component ever did — so two runs of one session cannot disagree about it.
/// </para>
/// </remarks>
public interface IStatefulComponent
{
    /// <summary>
    /// How long a value lives after it is set, or <see langword="null"/> for a value that never
    /// expires.
    /// </summary>
    TimeSpan? StateTimeToLive { get; }

    /// <summary>
    /// The value a session starts from, as JSON, deep-copied so that two sessions never share one
    /// object.
    /// </summary>
    /// <returns>The initial value.</returns>
    JsonNode? CreateState();

    /// <summary>Turns a stored value into what the handler reads.</summary>
    /// <param name="stored">The value the session holds.</param>
    /// <returns>The handler's argument.</returns>
    object? ReadState(JsonNode? stored);

    /// <summary>Turns what the handler returned into the value the session keeps.</summary>
    /// <param name="value">One of the values the handler's result carried.</param>
    /// <returns>The value to store, as JSON.</returns>
    JsonNode? WriteState(object? value);

    /// <summary>The value a returned component asks for, when the handler returned the component itself.</summary>
    /// <param name="update">The component the handler returned.</param>
    /// <returns>The value to store.</returns>
    /// <remarks>
    /// Returning a component is how a handler says "update this output" rather than "here is a
    /// value". For a stateful output the Python version reads that update's <c>value</c> and stores
    /// it, so a state can be written by handing back the state with a new value rather than by
    /// returning the value alone.
    /// </remarks>
    object? ValueOfUpdate(Block update);

    /// <summary>Called with a value when the session drops it.</summary>
    /// <param name="value">The value being dropped, as the session held it.</param>
    void DeleteState(JsonNode? value);
}

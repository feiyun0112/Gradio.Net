namespace Gradio.Net;
/// <summary>
/// The signals a block raises, as values an application can bind to and merge.
/// </summary>
/// <remarks>
/// <para>
/// A trigger is a value rather than a call: <c>btn.Events.Click</c> names the signal, and
/// <c>Blocks.On</c> takes a list of them when one handler should answer several. The binding itself
/// is <see cref="EventBinding"/>, kept separate because "which signals" and "what to run" are two
/// decisions.
/// </para>
/// <para>
/// They live under <c>Events</c> because the binding methods took their names: <c>btn.Click(...)</c>
/// binds, <c>btn.Events.Click</c> is the trigger, and C# cannot have both under one name.
/// The word did not change — it moved.
/// </para>
/// <para>
/// This base holds the two signals the framework asks every component about rather than one
/// component's set: <see cref="Submit"/> and <see cref="Stream"/>. A component that raises neither
/// leaves them <see langword="null"/>, which is how an <c>Interface</c> assembling a set of
/// triggers asks the Python version's <c>has_event("submit")</c>.
/// </para>
/// </remarks>
public class ComponentEvents
{
    /// <summary>Creates the trigger set of a block.</summary>
    /// <param name="owner">The block the triggers belong to.</param>
    /// <exception cref="ArgumentNullException"><paramref name="owner"/> is <see langword="null"/>.</exception>
    protected internal ComponentEvents(Block owner)
    {
        ArgumentNullException.ThrowIfNull(owner);

        Owner = owner;
    }

    /// <summary>The block the triggers belong to.</summary>
    protected Block Owner { get; }

    /// <summary>
    /// The signal this component raises when the user submits its value, or <see langword="null"/>
    /// when it raises none.
    /// </summary>
    /// <value>
    /// <see langword="null"/> for a component that cannot be submitted — a dropdown, a checkbox, a
    /// button. A component that can be overrides it with its own trigger.
    /// </value>
    public virtual EventTrigger? Submit => null;

    /// <summary>
    /// The signal this component raises as it produces its value a chunk at a time, or
    /// <see langword="null"/> when it raises none.
    /// </summary>
    /// <value>
    /// <see langword="null"/> for every component but an image, an audio player and an image
    /// comparison slider.
    /// </value>
    public virtual EventTrigger? Stream => null;
}

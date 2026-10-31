namespace Gradio.Net;
/// <summary>
/// Declares the C# type an event's payload arrives as, and the events that carry it.
/// </summary>
/// <remarks>
/// <para>
/// A component's events are the Python version's list, and the trigger set and the bindings are
/// generated from it. What the Python version does not say is what the handler's first parameter
/// should be in C#: <c>change</c> on a textbox hands a handler a <see cref="string"/>, on a slider
/// a <see cref="double"/>, on a gallery a list of entries — and a handful of events hand it a
/// selection or a copy instead of the component's value, which is why a component may type some of
/// its events and leave the rest to the raw binding.
/// </para>
/// <para>
/// So the type and the events it applies to are declared here, on the block, where a reader
/// comparing the two sides finds them. The attribute may appear more than once: the three plot
/// components share a base whose events do not all carry the same thing.
/// </para>
/// <para>
/// A layout declares one too. It has no value, so there is nothing for its <c>change</c> to carry,
/// but its <c>select</c> carries the Python version's <see cref="SelectData"/> — the tab that was
/// picked, or the step — and typing it is what makes <c>tabs.Select(evt =&gt; …, …)</c> read like
/// <c>button.Click(value =&gt; …, …)</c>.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
internal sealed class HandlerValueAttribute : Attribute
{
    /// <summary>Declares the type a handler receives, and the events it receives it from.</summary>
    /// <param name="value">
    /// The C# type a handler's first parameter takes for these events, spelled as source — including
    /// the <c>?</c> where the value may be absent.
    /// </param>
    /// <param name="events">The events whose handler is given the component's value, as the Python
    /// version spells them.</param>
    /// <remarks>
    /// A spelling rather than a <see cref="Type"/>, because <c>typeof</c> cannot take a nullable
    /// reference type and three components hand their handler a value that may be absent. The
    /// spelling is emitted into a <c>Func&lt;…&gt;</c>, so a mistake in it is a compile error in the
    /// generated file rather than a silently different signature.
    /// </remarks>
    public HandlerValueAttribute(string value, params string[] events)
    {
        Value = value;
        Events = events;
    }

    /// <summary>The C# type a handler's first parameter takes.</summary>
    public string Value { get; }

    /// <summary>The events whose handler is given the component's value, as the Python version
    /// spells them.</summary>
    public string[] Events { get; }
}

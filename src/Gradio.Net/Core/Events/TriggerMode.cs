namespace Gradio.Net;
/// <summary>
/// How many times an event binding may be submitted while a run of it is still in flight.
/// </summary>
/// <remarks>
/// The Python version's <c>trigger_mode</c>. An unset mode is not the same as <see cref="Once"/>:
/// the Python version picks the mode from the event's name when the caller leaves it unset, so the
/// "unset" state is carried by <see cref="EventBindingOptions.TriggerMode"/> being nullable rather
/// than by a member of this enum.
/// </remarks>
public enum TriggerMode
{
    /// <summary>No further submission while one is pending.</summary>
    Once,

    /// <summary>Unlimited submissions while one is pending.</summary>
    Multiple,

    /// <summary>A second submission is allowed once the pending one completes.</summary>
    AlwaysLast,
}

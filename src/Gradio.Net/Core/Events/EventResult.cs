using System.Runtime.CompilerServices;

namespace Gradio.Net;
/// <summary>
/// What a handler returned, read as the values a binding's outputs receive
/// </summary>
/// <remarks>
/// <para>
/// Three shapes are supported, and they are three because they say three different things: a tuple
/// gives one value per output, a component instance updates that component rather than replacing
/// its value, and an <see cref="IAsyncEnumerable{T}"/> is a stream of the
/// first two.
/// </para>
/// <para>
/// The reading is separated from the invoking on purpose. What a result means for a binding's
/// outputs is a property of the binding — it depends on how many outputs there are and on what the
/// handler's return type is — while running the handler is the queue's business (ticket 08). A
/// test can therefore assert the contract without a queue, and the queue cannot get it wrong in a
/// way a test cannot see.
/// </para>
/// </remarks>
public static class EventResult
{
    /// <summary>
    /// The values a handler's result gives to the outputs, in order.
    /// </summary>
    /// <param name="result">What the handler returned.</param>
    /// <returns>One entry per value the result carries.</returns>
    /// <remarks>
    /// <para>
    /// A tuple is spread across the outputs; anything else is a single value. A handler that
    /// returns one value for several outputs gives it to the first and leaves the rest alone, which
    /// is the Python version's rule: the result is a list, and a list shorter than the outputs
    /// simply stops.
    /// </para>
    /// <para>
    /// <see langword="null"/> is a value rather than an absence — it clears the output — so it
    /// produces one entry, not none.
    /// </para>
    /// </remarks>
    public static IReadOnlyList<object?> Unpack(object? result)
    {
        if (result is EventValues values)
        {
            return values;
        }

        if (result is ITuple tuple)
        {
            var entries = new object?[tuple.Length];
            for (var index = 0; index < tuple.Length; index++)
            {
                entries[index] = tuple[index];
            }

            return entries;
        }

        return [result];
    }

    /// <summary>Whether an output value is a component to update rather than a value to set.</summary>
    /// <param name="value">One of the values <see cref="Unpack"/> produced.</param>
    /// <returns>Whether the output is updated by copying the component's props.</returns>
    /// <remarks>
    /// The Python version tells the two apart by type at the same point: a value that is a
    /// component is an update — the handler built the component it wants the output to become — and
    /// everything else is a value the output's own postprocessing turns into one.
    /// </remarks>
    public static bool IsComponentUpdate(object? value) => value is Block;

    /// <summary>
    /// Whether a result is a stream rather than a single answer.
    /// </summary>
    /// <param name="result">What the handler returned.</param>
    /// <returns>Whether the result is an <see cref="IAsyncEnumerable{T}"/>.</returns>
    /// <remarks>
    /// A stream is the third shape: the handler yields the values one after another, and each yield
    /// is read the way a single result is. Which is why this asks about the result rather than about
    /// the handler's declared return type — an async method that returns a task of a stream is the
    /// same thing to a caller.
    /// </remarks>
    public static bool IsStream(object? result) =>
        result is not null
        && result.GetType().GetInterfaces().Any(implemented =>
            implemented.IsGenericType
            && implemented.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>));
}

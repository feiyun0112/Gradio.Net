using System.Runtime.CompilerServices;

namespace Gradio.Net;
/// <summary>
/// What a handler answers when it produces one value per output and the number of outputs is a
/// property of the application rather than of the handler.
/// </summary>
/// <remarks>
/// <para>
/// A handler normally answers with a tuple, which is spread across the outputs. A tuple
/// is a compile-time shape, though, and the framework's own handlers are written once for every
/// application: a chat's submit answers with the message and the conversation, and then one value
/// per additional output the application happened to ask for. That count is not known when the
/// handler is written, which is what this type is for.
/// </para>
/// <para>
/// It is a distinct type rather than a list because a list is a value: a handler that answers with
/// an array means "this is the value", and one that answers with this means "these are the values".
/// </para>
/// </remarks>
public sealed class EventValues : IReadOnlyList<object?>
{
    private readonly object?[] _values;

    private EventValues(object?[] values) => _values = values;

    /// <summary>One value per output, in the order the outputs are in.</summary>
    /// <param name="values">The values.</param>
    /// <returns>The result.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> is <see langword="null"/>.</exception>
    public static EventValues Of(params object?[] values)
    {
        ArgumentNullException.ThrowIfNull(values);

        return new EventValues([.. values]);
    }

    /// <inheritdoc />
    public int Count => _values.Length;

    /// <inheritdoc />
    public object? this[int index] => _values[index];

    /// <inheritdoc />
    public IEnumerator<object?> GetEnumerator() => ((IEnumerable<object?>)_values).GetEnumerator();

    /// <inheritdoc />
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _values.GetEnumerator();
}

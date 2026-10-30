using System.Text.Json.Nodes;
namespace Gradio.Net;
public static partial class gr
{
    /// <summary>Answers a handler with "leave this output as it is": the Python version's
    /// <c>gr.skip()</c>.</summary> <returns>The update that carries no props.</returns>
    /// <remarks>
    /// <para>
    /// A handler that returns this for one output of several updates the others and leaves that one
    /// alone; returning it alone leaves every output alone. That is the Python version's meaning,
    /// and it is carried by an update with nothing in it rather than by a value the framework
    /// invents: the client merges the props it is given into the output, and an empty set of props
    /// merges nothing.
    /// </para>
    /// <para>
    /// <see cref="Gradio.Net.Update"/> with no props is the same document, which is the Python version's own
    /// answer — <c>skip()</c> is spelled <c>{"__type__": "update"}</c> there too, because skipping
    /// and changing nothing are the same instruction.
    /// </para>
    /// </remarks>
    public static JsonObject Skip() => PropertyUpdate.Partial();

    /// <summary>Answers a handler with "change these props": the Python version's
    /// <c>gr.update(...)</c>.</summary>
    /// <returns>An update that changes nothing, for a handler that builds it up with
    /// <c>with</c>.</returns>
    /// <remarks>
    /// <see cref="Gradio.Net.Update"/> is the typed form of the Python version's keyword arguments: a handler
    /// names the props it changes and leaves the rest alone, and a prop left out of the record is a
    /// prop the client does not touch. <c>gr.Update()</c> is the empty one, which is the same
    /// document as <see cref="Skip"/>.
    /// </remarks>
    public static Update Update() => new();

    /// <summary>Answers a handler with the update it describes.</summary>
    /// <param name="update">The props to change.</param>
    /// <returns>The update, ready to be returned from a handler.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="update"/> is <see langword="null"/>.</exception>
    public static Update Update(Update update)
    {
        ArgumentNullException.ThrowIfNull(update);

        return update;
    }
}

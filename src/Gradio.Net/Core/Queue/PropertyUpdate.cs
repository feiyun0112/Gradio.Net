using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// A component update as the protocol carries it: the component's props with the marker that says
/// they are to be merged into the output rather than replace its value.
/// </summary>
/// <remarks>
/// The marker is written in one place and read in another — a run writes it, and the streaming pass
/// asks whether a value is one before it replaces that value with a stream's file object — so the
/// two halves of it live here rather than in either caller.
/// </remarks>
internal static class PropertyUpdate
{
    /// <summary>The key the marker is written under.</summary>
    private const string MarkerKey = "__type__";

    /// <summary>The marker's value, which the Python version spells the same way.</summary>
    private const string MarkerValue = "update";

    /// <summary>The update payload that makes an output become this component.</summary>
    /// <param name="block">The component the handler returned.</param>
    /// <returns>The component's props, marked as an update.</returns>
    internal static JsonObject Of(Block block)
    {
        ArgumentNullException.ThrowIfNull(block);

        var update = (JsonObject)block.ToConfigEntry()["props"]!.DeepClone();
        update[MarkerKey] = MarkerValue;

        return update;
    }

    /// <summary>The update payload that changes some props and leaves the rest alone.</summary>
    /// <param name="props">The props to change, in the order they are to be written.</param>
    /// <returns>The update, marked as one.</returns>
    /// <remarks>
    /// This is the Python version's <c>gr.update(...)</c>: a handler that answers with it says
    /// "these props changed" rather than "this is the output's new value". The distinction is what
    /// lets a chat empty its message field without also resetting the hint the field was drawn with
    /// — a component instance returned from a handler is also an update, but it names every prop
    /// the application passed at construction rather than the two that changed.
    /// </remarks>
    internal static JsonObject Partial(params (string Prop, JsonNode? Value)[] props)
    {
        ArgumentNullException.ThrowIfNull(props);

        var update = new JsonObject();
        foreach (var (prop, value) in props)
        {
            update[prop] = value?.DeepClone();
        }

        update[MarkerKey] = MarkerValue;

        return update;
    }

    /// <summary>Whether a postprocessed output value is an update rather than a value.</summary>
    /// <param name="data">The value, as the payload carries it.</param>
    /// <returns>Whether the client is being told to merge props into the output.</returns>
    internal static bool Is(JsonNode? data) =>
        data is JsonObject update
        && update.TryGetPropertyValue(MarkerKey, out var marker)
        && marker?.GetValue<string>() == MarkerValue;
}

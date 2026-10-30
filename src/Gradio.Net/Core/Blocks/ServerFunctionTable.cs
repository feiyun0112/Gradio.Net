using System.Text.Json;
using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// The functions a block's frontend may call on the server, and how a call reaches one of them.
/// </summary>
/// <remarks>
/// <para>
/// The Python version marks a method with <c>@server</c> and collects the marked ones by
/// reflection; an application that supplies the functions rather than declaring them hands over
/// callables, and the two components that accept those — the HTML component and the workflow canvas
/// — need the same two things: the names for the config's <c>server_fns</c>, and the conversion of
/// the call's one argument to the delegate's parameter type before it is invoked.
/// </para>
/// <para>
/// It lives in the kernel because it is about the call protocol rather than about either
/// component, and because a second copy of "a function takes at most one argument" is a second
/// rule that can drift.
/// </para>
/// </remarks>
internal static class ServerFunctionTable
{
    /// <summary>The names of the functions a block declares, in the order it declared them.</summary>
    /// <param name="functions">The functions, or <see langword="null"/> for none.</param>
    /// <returns>The names, or an empty list for a block with no functions.</returns>
    /// <remarks>
    /// The order is the dictionary's enumeration order, which is the order the entries were added
    /// in. The Python version writes its functions in the order the decorator found them, and the
    /// config comparison sees the difference.
    /// </remarks>
    internal static IReadOnlyList<string> NamesOf(IReadOnlyDictionary<string, Delegate>? functions) =>
        functions is null ? [] : [.. functions.Keys];

    /// <summary>Calls one of the functions a block declares.</summary>
    /// <param name="function">The delegate the block registered under the call's name.</param>
    /// <param name="data">The argument the browser sent.</param>
    /// <param name="cancellationToken">Cancels the call.</param>
    /// <returns>What the function produced, as the JSON the frontend reads.</returns>
    /// <remarks>
    /// A delegate that returns a task is awaited, so that an application's asynchronous function
    /// works the way the Python version's does. A synchronous one answers through a completed task.
    /// </remarks>
    internal static async ValueTask<JsonNode?> CallAsync(
        Delegate function,
        JsonNode? data,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(function);

        var result = await DelegateInvocation
            .InvokeAsync(function, ArgumentsOf(function, data), cancellationToken)
            .ConfigureAwait(false);

        return JsonValues.FromAnyObject(result);
    }

    /// <summary>The arguments a registered function takes, from the call's one argument.</summary>
    /// <param name="function">The delegate the block registered.</param>
    /// <param name="data">The argument the browser sent.</param>
    /// <returns>No arguments for a parameterless delegate, or the one it takes.</returns>
    /// <exception cref="ArgumentException">The delegate takes more than one argument.</exception>
    private static object?[] ArgumentsOf(Delegate function, JsonNode? data)
    {
        var parameters = function.Method.GetParameters();

        return parameters.Length switch
        {
            0 => [],
            1 => [ArgumentFor(data, parameters[0].ParameterType)],
            _ => throw new ArgumentException(
                "A component server function takes at most one argument.", nameof(function)),
        };
    }

    /// <summary>Converts the call's argument to the type a delegate's parameter declares.</summary>
    /// <param name="data">The argument the browser sent.</param>
    /// <param name="type">The parameter's type.</param>
    /// <returns>The argument as that type.</returns>
    private static object? ArgumentFor(JsonNode? data, Type type) =>
        typeof(JsonNode).IsAssignableFrom(type)
            ? data
            : JsonSerializer.Deserialize(data?.ToJsonString() ?? "null", type);
}

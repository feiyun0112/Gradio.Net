using System.Linq.Expressions;

namespace Gradio.Net;
/// <summary>
/// A handler that takes its inputs as one list rather than one argument each.
/// </summary>
/// <remarks>
/// <para>
/// A binding is normally written with a handler whose signature says how many inputs it reads, and
/// the run fills the arguments in the order the signature declares them. That is the right shape for
/// application code, which knows what it is reading.
/// </para>
/// <para>
/// It is the wrong shape for a handler the framework itself binds on the application's behalf, where
/// the number of inputs is a property of the application rather than of the code: a flag button
/// records whichever components the interface was built with, and an interface with two inputs and
/// one output needs a three-argument handler where the same code in another application needs five.
/// </para>
/// <para>
/// So the handler is written once against a list, and this builds the delegate of the arity the
/// binding has. The parameters are typed <see cref="object"/> because that is what a run hands a
/// binding that preprocesses nothing — the value as the client sent it, read back from JSON.
/// </para>
/// </remarks>
internal static class CollectingDelegate
{
    /// <summary>Builds a delegate of <paramref name="arity"/> arguments that collects them into one list.</summary>
    /// <param name="arity">How many inputs the binding reads.</param>
    /// <param name="handler">What to do with them.</param>
    /// <returns>The delegate to bind.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="arity"/> is negative.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="handler"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// The delegate type is built rather than looked up: <see cref="System.Func{TResult}"/> and its
    /// siblings stop at sixteen arguments, and how many components an interface holds is not bounded
    /// by that. <see cref="Expression.GetDelegateType(Type[])"/> makes the type for the arity that is
    /// asked for, so an interface with twenty inputs binds a twenty-argument handler.
    /// </remarks>
    internal static Delegate Of(int arity, Func<object?[], object?> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        return Build(arity, handler, typeof(object));
    }

    /// <summary>Builds a delegate of <paramref name="arity"/> arguments whose result is a stream.</summary>
    /// <param name="arity">How many inputs the binding reads.</param>
    /// <param name="handler">What to do with them.</param>
    /// <returns>The delegate to bind.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="arity"/> is negative.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="handler"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// A chat's submit handler is a stream when the handler it wraps is one, and the binding says so
    /// in the config — <c>types.generator</c> — which is read off the delegate's return type. A
    /// delegate built by <see cref="Of"/> answers <see cref="object"/>, so a streaming handler needs
    /// a delegate whose return type says what it is.
    /// </remarks>
    internal static Delegate OfStreaming(
        int arity,
        Func<object?[], IAsyncEnumerable<object?>> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        return Build(arity, handler, typeof(IAsyncEnumerable<object?>));
    }

    /// <summary>Builds a delegate of <paramref name="arity"/> arguments whose result is awaited.</summary>
    /// <param name="arity">How many inputs the binding reads.</param>
    /// <param name="handler">What to do with them.</param>
    /// <returns>The delegate to bind.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="arity"/> is negative.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="handler"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// The same shape as <see cref="Of"/>, for a handler whose work is a call to somewhere else — a
    /// request to a service, say — and which should not hold a thread while it waits. A run awaits
    /// the task and unwraps its result, so the binding behaves as it does for any other handler;
    /// saying the result is a task is what lets a wrapper that reads the delegate's return type see
    /// the truth rather than <see cref="object"/>.
    /// </remarks>
    internal static Delegate OfAsync(int arity, Func<object?[], Task<object?>> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        return Build(arity, handler, typeof(Task<object?>));
    }

    /// <summary>Builds the delegate, of the arity and the result the caller asked for.</summary>
    /// <param name="arity">How many arguments it takes.</param>
    /// <param name="handler">What to do with them, which the built delegate calls.</param>
    /// <param name="result">What it answers with.</param>
    /// <returns>The delegate to bind.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="arity"/> is negative.</exception>
    private static Delegate Build(int arity, Delegate handler, Type result)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(arity);

        var parameters = new ParameterExpression[arity];
        for (var index = 0; index < arity; index++)
        {
            parameters[index] = Expression.Parameter(typeof(object), $"input{index}");
        }

        var collected = Expression.NewArrayInit(typeof(object), [.. parameters.Cast<Expression>()]);
        var call = Expression.Invoke(Expression.Constant(handler), collected);
        var type = Expression.GetDelegateType([.. parameters.Select(_ => typeof(object)), result]);

        return Expression.Lambda(type, call, parameters).Compile();
    }

    /// <summary>Builds a delegate of <paramref name="arity"/> arguments and the run's request.</summary>
    /// <param name="arity">How many inputs the binding reads.</param>
    /// <param name="handler">What to do with them.</param>
    /// <returns>The delegate to bind.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="arity"/> is negative.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="handler"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// A chat's wrapper is the one handler this framework binds that then calls a handler of the
    /// application's, and the application's may read who is calling. The wrapper therefore declares
    /// a <see cref="Request"/> of its own — a run fills it by type, so it is not one of the
    /// binding's inputs — and hands it on.
    /// </remarks>
    internal static Delegate OfWithRequest(int arity, Func<object?[], Request, object?> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        return BuildWithRequest(arity, handler, typeof(object));
    }

    /// <summary>Builds a streaming delegate of <paramref name="arity"/> arguments and the run's request.</summary>
    /// <param name="arity">How many inputs the binding reads.</param>
    /// <param name="handler">What to do with them.</param>
    /// <returns>The delegate to bind.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="arity"/> is negative.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="handler"/> is <see langword="null"/>.</exception>
    internal static Delegate OfStreamingWithRequest(
        int arity,
        Func<object?[], Request, IAsyncEnumerable<object?>> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        return BuildWithRequest(arity, handler, typeof(IAsyncEnumerable<object?>));
    }

    /// <summary>Builds the delegate that also takes the run's request.</summary>
    /// <param name="arity">How many inputs the binding reads.</param>
    /// <param name="handler">What to do with them, which the built delegate calls.</param>
    /// <param name="result">What it answers with.</param>
    /// <returns>The delegate to bind.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="arity"/> is negative.</exception>
    private static Delegate BuildWithRequest(int arity, Delegate handler, Type result)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(arity);

        var parameters = new ParameterExpression[arity + 1];
        for (var index = 0; index < arity; index++)
        {
            parameters[index] = Expression.Parameter(typeof(object), $"input{index}");
        }

        parameters[arity] = Expression.Parameter(typeof(Request), "request");

        var collected = Expression.NewArrayInit(
            typeof(object),
            [.. parameters.Take(arity).Cast<Expression>()]);

        var call = Expression.Invoke(Expression.Constant(handler), collected, parameters[arity]);
        var type = Expression.GetDelegateType([.. parameters.Select(parameter => parameter.Type), result]);

        return Expression.Lambda(type, call, parameters).Compile();
    }
}

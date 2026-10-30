using System.Globalization;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace Gradio.Net;
/// <summary>
/// Calls a callable the application handed the framework, and awaits it when it is asynchronous.
/// </summary>
/// <remarks>
/// <para>
/// Two kinds of callable cross this boundary and both may be synchronous or return a
/// <see cref="Task"/>: an event handler, which the queue runs, and a component's server function,
/// which the frontend calls. The Python version's own spelling of both is a plain function, and of
/// a server function specifically <c>async def</c>; awaiting the task and unwrapping its result is
/// the same four steps in both places, so it happens once here rather than once per caller.
/// </para>
/// <para>
/// <see cref="Delegate.DynamicInvoke"/> wraps whatever the callable threw in a
/// <see cref="TargetInvocationException"/>. The original is rethrown instead, so that what a
/// failure says is the failure rather than a reflection artefact.
/// </para>
/// </remarks>
internal static class DelegateInvocation
{
    /// <summary>The parameters the callable declares, in order.</summary>
    /// <param name="callable">The delegate.</param>
    /// <returns>Its parameters.</returns>
    /// <remarks>
    /// The delegate type's own signature rather than the method's: a handler the framework compiled
    /// — a flag button's, whose arity is a property of the application — is a run-time method, and
    /// asking one of those for its parameters answers nothing. A delegate type always says.
    /// </remarks>
    internal static ParameterInfo[] ParametersOf(Delegate callable)
    {
        ArgumentNullException.ThrowIfNull(callable);

        return callable
            .GetType()
            .GetMethod(nameof(Action.Invoke), BindingFlags.Public | BindingFlags.Instance)!
            .GetParameters();
    }

    /// <summary>Calls a delegate, and awaits the task it returns when it returns one.</summary>
    /// <param name="callable">The delegate to call.</param>
    /// <param name="arguments">Its arguments, in the order it declares them.</param>
    /// <param name="cancellationToken">Cancels the wait for a task the delegate returned.</param>
    /// <returns>What the delegate returned, or a task's result when it returned one.</returns>
    internal static async Task<object?> InvokeAsync(
        Delegate callable,
        object?[] arguments,
        CancellationToken cancellationToken)
    {
        object? result;
        try
        {
            result = callable.DynamicInvoke(NumbersAsDeclared(callable, arguments));
        }
        catch (TargetInvocationException error) when (error.InnerException is not null)
        {
            ExceptionDispatchInfo.Capture(error.InnerException).Throw();

            throw;
        }

        if (result is not Task task)
        {
            return result;
        }

        await task.WaitAsync(cancellationToken).ConfigureAwait(false);

        // A task that carries no result — a handler that answered with a bare `Task` — has
        // nothing to hand back, which is what the Python version's `None` is.
        return task.GetType().IsGenericType
            ? task.GetType().GetProperty("Result")!.GetValue(task)
            : null;
    }

    /// <summary>Each argument as the number type the delegate's parameter declares.</summary>
    /// <param name="callable">The delegate.</param>
    /// <param name="arguments">Its arguments, in the order it declares them.</param>
    /// <returns>The arguments, with the numbers converted where they have to be.</returns>
    /// <remarks>
    /// <para>
    /// A component hands over the number its own rule produced — a <c>double</c> from a number
    /// field with no precision, an <c>int</c> from one rounded to a whole — and a handler declares
    /// the type it works in. <see cref="Delegate.DynamicInvoke"/> converts nothing between numeric
    /// types, so without this a <c>Func&lt;int, …&gt;</c> on a number field would fail at the call
    /// rather than at the declaration, which is the failure a ported Python handler would meet
    /// first.
    /// </para>
    /// <para>
    /// Only numbers are converted. A handler that declared a string where a number arrived is a
    /// mistake worth hearing about, and the call reports it.
    /// </para>
    /// </remarks>
    /// <exception cref="OverflowException">A number does not fit the declared type.</exception>
    private static object?[] NumbersAsDeclared(Delegate callable, object?[] arguments)
    {
        var parameters = ParametersOf(callable);

        if (parameters.Length != arguments.Length)
        {
            // The call itself will report the mismatch, with its own message about the arity.
            return arguments;
        }

        for (var index = 0; index < arguments.Length; index++)
        {
            var argument = arguments[index];
            var declared = parameters[index].ParameterType;

            if (argument is null || declared.IsInstanceOfType(argument) || !IsNumber(argument) || !IsNumericType(declared))
            {
                continue;
            }

            arguments[index] = Convert.ChangeType(argument, declared, CultureInfo.InvariantCulture);
        }

        return arguments;
    }

    /// <summary>
    /// Whether a parameter is one the framework fills rather than one of a binding's inputs.
    /// </summary>
    /// <param name="type">The parameter type a handler declared.</param>
    /// <returns>Whether the run supplies it, so that it is not counted as an input.</returns>
    /// <remarks>
    /// <para>
    /// Three types every run supplies: a <see cref="Progress"/> tracker, a
    /// <see cref="CancellationToken"/>, and the <see cref="Request"/> the run belongs to. Everything
    /// else maps to an input positionally, because a .NET delegate carries no parameter names to
    /// match on.
    /// </para>
    /// <para>
    /// The event-data payloads are the fourth family and are asked about separately, by
    /// <see cref="EventData.Is"/>: they are supplied only by an event that carries one, so a place
    /// that counts a handler's inputs has to decide for itself whether such a parameter could be an
    /// input there — an <c>Interface</c>'s handler is never given one, a binding's may be.
    /// </para>
    /// <para>
    /// The question is asked in several places — the API document that counts a handler's inputs,
    /// the cache key that must not hash a request, the workflow canvas that draws a port per input —
    /// and a type added to one list but not the others would be counted as an input somewhere it is
    /// not one. One predicate is what keeps those answers equal.
    /// </para>
    /// </remarks>
    internal static bool IsSuppliedByRun(Type type) =>
        type == typeof(Progress) || type == typeof(CancellationToken) || type == typeof(Request);

    /// <summary>Whether a value or a type is one of the CLR's numbers.</summary>
    /// <param name="value">The value, or the type to ask about.</param>
    /// <returns>Whether it is a number.</returns>
    private static bool IsNumber(object value) => value is
        byte or sbyte or short or ushort or int or uint or long or ulong or float or double or decimal;

    /// <summary>Whether a type is one of the CLR's numbers.</summary>
    /// <param name="type">The type.</param>
    /// <returns>Whether it is a number.</returns>
    private static bool IsNumericType(Type type) =>
        type == typeof(byte) || type == typeof(sbyte)
        || type == typeof(short) || type == typeof(ushort)
        || type == typeof(int) || type == typeof(uint)
        || type == typeof(long) || type == typeof(ulong)
        || type == typeof(float) || type == typeof(double) || type == typeof(decimal);
}

namespace Gradio.Net;
/// <summary>
/// Carries the <see cref="RenderContext"/> of the current asynchronous flow, and establishes it
/// for the duration of a render scope.
/// </summary>
/// <remarks>
/// <para>
/// The implementation holds the value in an <see cref="System.Threading.AsyncLocal{T}"/>, so
/// concurrent renders do not see each other's context even though they share the accessor.
/// The accessor is the framework's replacement for the old static <c>Context</c>: it is resolved
/// from the container rather than reached through a global.
/// </para>
/// <para>
/// The contract sits with the render model rather than with the option types, because it names
/// <see cref="RenderContext"/>: a contract in the folder that may not depend on the render model
/// would invert the dependency.
/// </para>
/// <para>
/// Reads happen inside a render scope and nowhere else. Code that runs after the scope has ended
/// sees no context, which is how "the framework is not rendering right now" is expressed.
/// </para>
/// </remarks>
public interface IRenderContextAccessor
{
    /// <summary>
    /// The render context of the current flow, or <see langword="null"/> when no render is in
    /// progress in this flow.
    /// </summary>
    RenderContext? Current { get; }

    /// <summary>
    /// Establishes <paramref name="context"/> for the current flow until the returned scope is
    /// disposed.
    /// </summary>
    /// <param name="context">The context to make ambient.</param>
    /// <returns>
    /// A scope that restores the previously ambient context — which is <see langword="null"/> at
    /// the outermost level — when it is disposed.
    /// </returns>
    /// <remarks>
    /// Scopes nest and must be disposed in the order they were entered.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
    IDisposable Enter(RenderContext context);
}

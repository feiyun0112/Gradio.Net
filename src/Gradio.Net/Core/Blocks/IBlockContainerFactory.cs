namespace Gradio.Net;
/// <summary>
/// Makes the containers that group components expecting one.
/// </summary>
/// <remarks>
/// <para>
/// The grouping of form components into a form is the Python version behaviour, but a component
/// does not reference the container type itself. A component therefore names the kind of container
/// it expects, and this turns the name into one.
/// </para>
/// <para>
/// Implemented by the factory that owns the layouts and registered with the application. A root
/// built without one renders any tree that needs no grouping, and fails with a message naming the
/// kind when a component does.
/// </para>
/// </remarks>
public interface IBlockContainerFactory
{
    /// <summary>Creates a container of the given kind.</summary>
    /// <param name="kind">The kind a component named, e.g. <c>form</c>.</param>
    /// <returns>The container, with no id yet: the render root gives it one.</returns>
    /// <exception cref="NotSupportedException"><paramref name="kind"/> is not a container this factory knows.</exception>
    BlockContext Create(string kind);
}

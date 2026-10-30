using Microsoft.Extensions.DependencyInjection;

namespace Gradio.Net;
/// <summary>
/// The application a container serves: the one the host handed it, or one the container builds.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="GradioServiceCollectionExtensions.AddGradio"/> registers the application so that the
/// container owns it, and <see cref="GradioEndpointRouteBuilderExtensions.UseGradio(WebApplication)"/>
/// serves whatever the container resolves. That is the right order for an application that is
/// written into the container's application, and the wrong one for an application that was written
/// before the host existed — a page built by <c>gr.Blocks()</c>, or one built in a method the host
/// calls. This slot is the seam that lets the second shape hand its application over: the container
/// still resolves one <see cref="Blocks"/>, and which instance that is becomes the host's answer
/// rather than the container's.
/// </para>
/// <para>
/// Supplying an application after the container has already built one is refused rather than
/// ignored: two applications in one container is the thing the layering says cannot happen, and
/// the failure to say so would be a page that serves the blocks nobody wrote.
/// </para>
/// </remarks>
internal sealed class GradioApplicationSlot
{
    private readonly object _gate = new();
    private Blocks? _supplied;
    private Blocks? _resolved;

    /// <summary>Hands the container the application to serve.</summary>
    /// <param name="application">The application the host wrote into.</param>
    /// <exception cref="InvalidOperationException">The container has already resolved its application.</exception>
    internal void Supply(Blocks application)
    {
        lock (_gate)
        {
            if (_resolved is not null && !ReferenceEquals(_resolved, application))
            {
                throw new InvalidOperationException(
                    "The container has already resolved its application, so it cannot be given another one: "
                    + "call UseGradio(blocks) before anything asks the container for a Blocks.");
            }

            _supplied = application;
        }
    }

    /// <summary>The application the container serves.</summary>
    /// <param name="build">Builds one when the host supplied none.</param>
    /// <returns>The supplied application, or the one <paramref name="build"/> makes.</returns>
    internal Blocks Resolve(Func<Blocks> build)
    {
        lock (_gate)
        {
            return _resolved ??= _supplied ?? build();
        }
    }
}

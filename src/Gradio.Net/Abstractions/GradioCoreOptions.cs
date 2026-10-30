namespace Gradio.Net;
/// <summary>
/// Kernel options, configured through <c>AddGradioCore</c>.
/// </summary>
/// <remarks>
/// This is the root of the kernel's configuration. Options for the capabilities layered on top
/// of the kernel (queue, theme, logging) are added here as those capabilities land.
/// </remarks>
public sealed class GradioCoreOptions
{
    /// <summary>Configuration of the session store.</summary>
    public SessionStoreOptions SessionStore { get; } = new();
}

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

    /// <summary>
    /// How many runs the queue may have in flight at once, across every binding.
    /// </summary>
    /// <value>
    /// 40, the Python version's <c>launch(max_threads=40)</c> default. The standalone entry point
    /// carries its own <c>maxThreads</c> here so the two hosting paths read the same number.
    /// </value>
    /// <remarks>
    /// This is a ceiling on the whole queue, not a thread count: the runs are asynchronous. A
    /// binding's own <c>concurrencyLimit</c> still applies inside it, so raising this number does
    /// not by itself let one handler run twice at once. The Python version names the same ceiling
    /// <c>max_threads</c> and lets an application set it when it launches, rather than fixing it
    /// in the framework.
    /// </remarks>
    public int MaxThreads { get; set; } = 40;
}

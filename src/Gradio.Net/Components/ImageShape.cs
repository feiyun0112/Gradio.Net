namespace Gradio.Net;
/// <summary>
/// How one component wants its images: where they go, what they are written as, and whether a
/// handler is handed the picture or its path.
/// </summary>
/// <param name="Folder">The application's cache folder.</param>
/// <param name="Format">The format the component writes in.</param>
/// <param name="Mode">The mode to convert to, or <see langword="null"/> for the image's own.</param>
/// <param name="Type"><c>numpy</c>, <c>pil</c> or <c>filepath</c>.</param>
/// <param name="Root">The application, so that what it writes can be reclaimed.</param>
/// <remarks>
/// The four parameters a component already has, bundled because they always travel together: the
/// pipeline is handed all four or none, and a method with five parameters of which four are the
/// component's own configuration is a method that reads like the component.
/// </remarks>
internal sealed record ImageShape(string Folder, string Format, string? Mode, string Type, Blocks? Root)
{
    /// <summary>Whether a handler is handed the image itself rather than its path.</summary>
    internal bool WantsImage => Type is "numpy" or "pil";
}

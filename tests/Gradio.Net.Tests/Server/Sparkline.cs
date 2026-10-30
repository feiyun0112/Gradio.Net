using System.Text.Json.Nodes;
using Gradio.Net;
namespace Gradio.Net.Tests.Server;

/// <summary>
/// A component that brings its own frontend, written the way an application writes one: a class of
/// its own, its own parameters, its own reading and writing of values, and the directory its
/// compiled bundle was put in.
/// </summary>
/// <remarks>
/// <para>
/// This is the extension point for a component that ships its own bundle, and it is deliberately
/// not a subclass of <see cref="Html"/>: a component that ships a bundle
/// draws itself, so the frontend has never heard of its <c>type</c> and asks the custom-component
/// route for it instead.
/// What the framework can know about such a component is the two things here — what it is called
/// and where its bundle is — which is why they are an interface rather than a convention.
/// </para>
/// <para>
/// The value is a list of numbers written as text, because that is a shape whose preprocessing is
/// visible: what the browser sends and what the handler reads are not the same type, so a test can
/// tell whether the component's own <see cref="Preprocess"/> ran or the base one did.
/// </para>
/// </remarks>
internal sealed class Sparkline : Component, ICustomComponentBundle
{
    /// <summary>The identity the browser asks for this component's bundle by.</summary>
    internal const string Identity = "sparkline-v1";

    /// <summary>What the bundle's module says, which is nothing a browser can draw.</summary>
    internal const string Script = "export default { name: 'sparkline' };\n";

    /// <summary>The bundle's stylesheet, which is text and is sent as text.</summary>
    internal const string Stylesheet = ".sparkline { display: block; }\n";

    /// <summary>The Svelte runtime the bundle was built against, which the loader fetches too.</summary>
    internal const string Runtime = "export default {};\n";

    /// <summary>Declares the component, pointing at the directory its bundle was built
    /// into.</summary> <param name="templateDirectory">The parent of the variant directories, as
    /// the Python version's TEMPLATE_DIR.</param>
    internal Sparkline(string templateDirectory) => TemplateDirectory = templateDirectory;

    /// <summary>
    /// Writes a bundle into a folder of a test's own, in the shape a build leaves it.
    /// </summary>
    /// <param name="folder">The folder to write into; its <c>templates</c> directory is what a component names.</param>
    /// <returns>The directory a <see cref="Sparkline"/> should be given.</returns>
    /// <remarks>
    /// One variant directory with the three files the loader asks for, because the three are one
    /// request each: a test that only wrote the module would leave two of the three unserved and
    /// could not tell a route that serves the bundle from one that serves its first file.
    /// </remarks>
    internal static string WriteBundle(TempFolder folder)
    {
        ArgumentNullException.ThrowIfNull(folder);

        var templates = Path.Combine(folder.Folder, "templates");
        var variant = Directory.CreateDirectory(Path.Combine(templates, "component")).FullName;

        System.IO.File.WriteAllText(Path.Combine(variant, "index.js"), Script);
        System.IO.File.WriteAllText(Path.Combine(variant, "style.css"), Stylesheet);
        System.IO.File.WriteAllText(Path.Combine(variant, "svelte_runtime_entry.js"), Runtime);

        return templates;
    }

    /// <inheritdoc />
    public string ComponentClassId => Identity;

    /// <inheritdoc />
    public string TemplateDirectory { get; }

    /// <summary>A parameter of its own, which the Python version has no component for.</summary>
    internal string? Caption { get; set; }

    /// <inheritdoc />
    /// <remarks>
    /// A value that is text on the wire, which is what the bundle draws.
    /// </remarks>
    public override JsonObject ApiInfo => new() { ["type"] = "string" };

    /// <inheritdoc />
    /// <remarks>
    /// Written beside the shared props, which is where a component's own parameters go; a parameter
    /// the bundle's template reads would also be written to the extra-props dictionary, as
    /// <see cref="Callout"/> does.
    /// </remarks>
    protected internal override void WriteProps(PropWriter props)
    {
        ArgumentNullException.ThrowIfNull(props);

        props.Add("caption", Caption);
    }

    /// <inheritdoc />
    /// <remarks>
    /// "1,2,3" becomes the three numbers: a handler that adds them up could only do that if this
    /// ran, because the base implementation would hand it the string.
    /// </remarks>
    public override object? Preprocess(JsonNode? value) =>
        (value?.GetValue<string>() ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(int.Parse)
            .ToArray();

    /// <inheritdoc />
    /// <remarks>
    /// The drawing the bundle would produce, as a string: what the handler returned is a number, and
    /// what the browser receives is not.
    /// </remarks>
    public override JsonNode? Postprocess(object? value) => $"<svg data-total=\"{value}\"/>";
}

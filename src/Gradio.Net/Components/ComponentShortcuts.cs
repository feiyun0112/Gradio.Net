using System.Collections.Frozen;
namespace Gradio.Net;
/// <summary>
/// The name an application may use instead of a constructor, and the component that name means.
/// </summary>
/// <remarks>
/// <para>
/// The Python version accepts either a component or its name wherever a component is expected — a
/// Python application writes <c>additional_inputs=["textbox"]</c> because importing the class is
/// more ceremony than the shortcut is worth. The shortcut is part of the served application: the
/// component it names is constructed with every default, so an assembly that refused the name would
/// serve a different page.
/// </para>
/// <para>
/// The table is written out rather than reflected over, which is the same choice the props
/// generation makes: a component that is added to the framework is a component whose
/// name has to be decided on, and a name that is missing from here fails loudly instead of
/// silently becoming a component nobody expected. Two of the entries are not class names:
/// the Python version's component module also writes <c>Text = Textbox</c> and <c>Highlight =
/// HighlightedText</c>, and every tutorial writes <c>inputs="text"</c>.
/// </para>
/// <para>
/// Layouts are not in the table. They are blocks rather than components, and the places that take
/// a name — an additional input, an additional output, a chat's own field — take something a
/// handler can read a value from.
/// </para>
/// </remarks>
internal static class ComponentShortcuts
{
    private static readonly FrozenDictionary<string, Func<Component>> _factories =
        new Dictionary<string, Func<Component>>(StringComparer.Ordinal)
        {
            ["annotatedimage"] = () => new AnnotatedImage(),
            ["api"] = () => new Api(),
            ["audio"] = () => new Audio(),
            ["audiogallery"] = () => new AudioGallery(),
            ["barplot"] = () => new BarPlot(),
            ["browserstate"] = () => new BrowserState(),
            ["button"] = () => new Button(),
            ["chatbot"] = () => new Chatbot(),
            ["checkbox"] = () => new Checkbox(),
            ["checkboxgroup"] = () => new CheckboxGroup(),
            ["clearbutton"] = () => new ClearButton(),
            ["code"] = () => new Code(),
            ["colorpicker"] = () => new ColorPicker(),
            ["coloredcheckboxgroup"] = () => new ColoredCheckboxGroup(),
            ["dataframe"] = () => new Dataframe(),
            ["dataset"] = () => new Dataset(),
            ["datetime"] = () => new DateTime(),
            ["deeplinkbutton"] = () => new DeepLinkButton(),
            ["dialogue"] = () => new Dialogue(),
            ["downloadbutton"] = () => new DownloadButton(),
            ["dropdown"] = () => new Dropdown(),
            ["duplicatebutton"] = () => new DuplicateButton(),
            ["fallback"] = () => new Fallback(),
            ["file"] = () => new File(),
            ["fileexplorer"] = () => new FileExplorer(),
            ["gallery"] = () => new Gallery(),
            ["highlight"] = () => new HighlightedText(),
            ["highlightedtext"] = () => new HighlightedText(),
            ["html"] = () => new Html(),
            ["image"] = () => new Image(),
            ["imageeditor"] = () => new ImageEditor(),
            ["imageslider"] = () => new ImageSlider(),
            ["json"] = () => new JSON(),
            ["label"] = () => new Label(),
            ["lineplot"] = () => new LinePlot(),
            ["loginbutton"] = () => new LoginButton(),
            ["markdown"] = () => new Markdown(),
            ["model3d"] = () => new Model3D(),
            ["multimodaltextbox"] = () => new MultimodalTextbox(),
            ["number"] = () => new Number(),
            ["paramviewer"] = () => new ParamViewer(),
            ["plot"] = () => new Plot(),
            ["radio"] = () => new Radio(),
            ["scatterplot"] = () => new ScatterPlot(),
            ["simpledropdown"] = () => new SimpleDropdown(),
            ["simpleimage"] = () => new SimpleImage(),
            ["simpletextbox"] = () => new SimpleTextbox(),
            ["slider"] = () => new Slider(),
            ["state"] = () => new State(),
            ["text"] = () => new Textbox(),
            ["textbox"] = () => new Textbox(),
            ["timer"] = () => new Timer(),
            ["uploadbutton"] = () => new UploadButton(),
            ["video"] = () => new Video(),
            ["workflowcanvas"] = () => new WorkflowCanvas(),
        }.ToFrozenDictionary(StringComparer.Ordinal);

    /// <summary>The component an entry is, or the one its name means.</summary>
    /// <param name="element">A component, or the name of one.</param>
    /// <param name="parameter">
    /// The parameter the entry came from, which names the mistake in the exception, or
    /// <see langword="null"/> when there is no caller to name.
    /// </param>
    /// <returns>The component.</returns>
    /// <exception cref="ArgumentException">The entry is neither a component nor the name of one.</exception>
    /// <remarks>
    /// The one place a name becomes a component, so that an interface's side, a chat's additional
    /// inputs and a list written inline all read a name the same way.
    /// </remarks>
    internal static Component Resolve(object? element, string? parameter = null) => element switch
    {
        Component component => component,
        string name => Create(name),
        _ => throw new ArgumentException(
            "A component, a name, or a list of either is expected; got "
            + $"{(element is null ? "nothing" : element.GetType().Name)}.",
            parameter),
    };

    /// <summary>The component a name stands for.</summary>
    /// <param name="name">The name, as the Python version spells a component's block.</param>
    /// <returns>A new component, with every prop at its default.</returns>
    /// <exception cref="ArgumentException">No component is called that.</exception>
    /// <remarks>
    /// Underscores are ignored, which is the Python version's own rule: it looks a name up with
    /// <c>cls_name.replace("_", "")</c>, so <c>"highlighted_text"</c> and
    /// <c>"multi_modal_textbox"</c> name the components their class names name.
    /// </remarks>
    internal static Component Create(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        var key = name.Replace("_", string.Empty, StringComparison.Ordinal);

        return _factories.TryGetValue(key, out var factory)
            ? factory()
            : throw new ArgumentException(
                $"'{name}' is not a component this framework ships; give the component itself, or "
                + "one of the names in the component reference.",
                nameof(name));
    }
}

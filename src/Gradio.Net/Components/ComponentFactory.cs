namespace Gradio.Net.Components;

public static class ComponentFactory
{
    public static Component CreateComponent(string type)
    {
        return type.ToLower() switch
        {
            "text" or "textbox" => new Textbox(),
            "slider" => new Slider(),
            "button" => new Button(),
            "markdown" => new Markdown(),
            "checkbox" => new Checkbox(),
            "checkboxgroup" => new CheckboxGroup(),
            "clearbutton" or "clear_button" => new ClearButton(),
            "code" => new Code(),
            "colorpicker" or "color_picker" => new ColorPicker(),
            "dataframe" => new Dataframe(),
            "dataset" => new Dataset(),
            "datetime" => new DateTimeComponent(),
            "deeplinkbutton" or "deep_link_button" => new DeepLinkButton(),
            "dialogue" => new Dialogue(),
            "downloadbutton" or "download_button" => new DownloadButton(),
            "dropdown" => new Dropdown(),
            "duplicatebutton" or "duplicate_button" => new DuplicateButton(),
            "fallback" => new Fallback(),
            "file" => new FileComponent(),
            "fileexplorer" or "file_explorer" => new FileExplorer(),
            "gallery" => new Gallery(),
            "highlightedtext" or "highlighted_text" => new HighlightedText(),
            "html" => new Html(),
            "image" => new Image(),
            "imageslider" or "image_slider" => new ImageSlider(),
            "json" or "json_component" => new JsonComponent(),
            "label" => new Label(),
            "model3d" or "model_3d" => new Model3D(),
            "nativeplot" or "native_plot" => new NativePlot(),
            "barplot" or "bar_plot" => new BarPlot(),
            "lineplot" or "line_plot" => new LinePlot(),
            "scatterplot" or "scatter_plot" => new ScatterPlot(),
            "number" => new Number(),
            "paramviewer" or "param_viewer" => new ParamViewer(),
            "plot" => new Plot(),
            "radio" => new Radio(),
            "state" => new State(),
            "timer" => new Timer(),
            "uploadbutton" or "upload_button" => new UploadButton(),
            "video" => new Video(),
            "audiogallery" or "audio_gallery" => new AudioGallery(new List<string>()),
            "coloredcheckboxgroup" or "colored_checkbox_group" => new ColoredCheckboxGroup(new List<string>(), new List<string>()),
            "simpledropdown" or "simple_dropdown" => new SimpleDropdown(),
            "simpleimage" or "simple_image" => new SimpleImage(),
            "simpletextbox" or "simple_textbox" => new SimpleTextbox(),
            _ => throw new NotSupportedException($"Component type '{type}' is not supported")
        };
    }
}

namespace Gradio.Net;
/// <summary>
/// The app view: the same workflow as an ordinary form, drawn from the graph's outputs.
/// </summary>
/// <remarks>
/// <para>
/// A canvas is for the person arranging the workflow; everybody else wants a form. So the graph is
/// drawn twice — once as the canvas, once as one panel per pipeline: an input per value the graph
/// cannot compute, a button, and an output per result, laid out the way <see cref="Interface"/>
/// lays its own. The components are real components rather than placeholders because they are also
/// what the endpoints are described by: an image input is an image input in the API documentation
/// too, with the file handling that implies.
/// </para>
/// <para>
/// Each value the graph cannot compute is pre-filled with what the canvas saved for its node, which
/// is what makes a saved workflow callable without arguments. A value the component cannot take —
/// a file that is no longer there — is dropped rather than failing the whole panel.
/// </para>
/// </remarks>
public sealed partial class Workflow
{
    /// <summary>The value a JSON input is edited as.</summary>
    /// <remarks>
    /// Four lines: enough to see the shape of a small document without taking over the panel.
    /// </remarks>
    private const int JsonInputLines = 4;

    /// <summary>The value a Markdown input is edited as.</summary>
    private const int MarkdownInputLines = 3;

    /// <summary>Draws the app view into the column the canvas points at.</summary>
    /// <param name="graph">The graph, or <see langword="null"/> for one that is not there.</param>
    /// <param name="appRoot">The column the canvas's <c>app_view</c> names.</param>
    private void BuildAppView(WorkflowGraph? graph, Column appRoot)
    {
        using var scope = _blocks.Enter(appRoot);

        var body = _blocks.Add(new Column { ElemClasses = [AppBodyClass] });
        using var bodyScope = _blocks.Enter(body);

        _blocks.Add(new Markdown { Value = $"# {graph?.Name ?? "Workflow"}" });

        var groups = graph?.Groups() ?? [];

        if (groups.Count == 0)
        {
            // Nothing to run: a graph with no outputs, or a draft that is not a saved graph yet. The
            // note says what to do about it, because the author is the one looking at it.
            _blocks.Add(new Markdown { Value = NoOutputsNote });

            return;
        }

        if (groups.Count == 1)
        {
            BuildGroup(graph!, groups[0]);

            return;
        }

        // One tab per pipeline, the way a tabbed interface separates applications that do not
        // interact: the pipelines of one workflow share no work, so they are separate forms.
        var tabs = _blocks.Add(new Tabs());
        using var tabsScope = _blocks.Enter(tabs);

        foreach (var group in groups)
        {
            var label = group.Subjects[0].Label.Length > 0 ? group.Subjects[0].Label : group.ApiName;

            var tab = _blocks.Add(new Tab { Label = label });
            using var tabScope = _blocks.Enter(tab);

            BuildGroup(graph!, group);
        }
    }

    /// <summary>Draws one pipeline's inputs, its button and its outputs, and wires the endpoint.</summary>
    /// <param name="graph">The graph.</param>
    /// <param name="group">The pipeline.</param>
    private void BuildGroup(WorkflowGraph graph, WorkflowGroup group)
    {
        var freeInputs = graph.GroupFreeInputs(group.Subjects);
        var subjectIds = group.Subjects.Select(subject => subject.Id).ToList();

        var row = _blocks.Add(new Row { EqualHeight = false });
        using var rowScope = _blocks.Enter(row);

        var inputs = new List<Block>();
        Button run;

        var inputColumn = _blocks.Add(new Column());
        using (var columnScope = _blocks.Enter(inputColumn))
        {
            foreach (var input in freeInputs)
            {
                inputs.Add(AddPortComponent(
                    input.Type,
                    input.Label,
                    WorkflowValues.ReferenceDefault(input),
                    output: false));
            }

            run = _blocks.Add(new Button { Value = "Run", Variant = "primary" });
        }

        var outputs = new List<Block>();

        var outputColumn = _blocks.Add(new Column());
        using (var columnScope = _blocks.Enter(outputColumn))
        {
            foreach (var subject in group.Subjects)
            {
                outputs.Add(AddPortComponent(
                    WorkflowGraph.SubjectOutputType(subject),
                    subject.Label,
                    value: null,
                    output: true));
            }
        }

        var handler = CollectingDelegate.OfAsync(freeInputs.Count, async values =>
        {
            var supplied = new Dictionary<string, object?>(StringComparer.Ordinal);
            for (var index = 0; index < freeInputs.Count; index++)
            {
                supplied[freeInputs[index].Node.Id] = values[index];
            }

            var results = await new WorkflowExecutor(graph, Operators())
                .RunManyAsync(subjectIds, supplied, CancellationToken.None)
                .ConfigureAwait(false);

            return results.Count == 1 ? results[0] : EventValues.Of([.. results]);
        });

        run.BindEventRaw(
            run.Events.Click,
            handler,
            [.. inputs],
            [.. outputs],
            new EventBindingOptions
            {
                ApiName = group.ApiName,

                // The endpoint's arguments are named by position because that is all the graph
                // knows about them: a value is "the first thing the caller supplies", and the name
                // it goes by in the API documentation is the position spelled out.
                ParameterNames = [.. freeInputs.Select((_, index) => $"in_{index}")],
            });
    }

    /// <summary>Draws the component a port type is shown as, and adds it to the open container.</summary>
    /// <param name="portType">The port's type.</param>
    /// <param name="label">What the port is called.</param>
    /// <param name="value">The value it starts with, or <see langword="null"/> for none.</param>
    /// <param name="output">Whether the component shows a result rather than taking an input.</param>
    /// <returns>The component.</returns>
    /// <remarks>
    /// The mapping is the canvas's vocabulary read as components, which is why two types map to two
    /// components each: a document shown as a result is rendered, and edited as text; a JSON value
    /// is a viewer on the way out and a text field on the way in, because that is how the canvas
    /// treats it as well.
    /// </remarks>
    private Component AddPortComponent(string portType, string label, object? value, bool output)
    {
        Component component = portType switch
        {
            "image" => new Image { Type = "filepath", Value = JsonValues.FromAnyObject(value) },
            "audio" => new Audio { Type = "filepath", Value = value },
            "video" => new Video { Value = value },
            "model3d" or "3d" => new Model3D { Value = value },
            "file" => new File { Type = "filepath", Value = JsonValues.FromAnyObject(value) },
            "number" => new Number { Value = NumberOf(value) },
            "boolean" => new Checkbox { Value = value is true },
            "dataframe" => new Dataframe { Value = value },
            "gallery" => new Gallery { Value = value },
            "json" when output => new JSON { Value = value },
            "json" => new Code { Language = "json", Lines = JsonInputLines, Value = JsonText(value) },
            "markdown" when output => new Markdown { Container = true, Value = value as string },
            "markdown" => new Textbox { Lines = MarkdownInputLines, Value = value as string },
            _ => new Textbox { Value = value as string },
        };

        component.Label = label.Length > 0 ? label : portType;

        return _blocks.Add(component);
    }

    /// <summary>The value a number component takes, or <see langword="null"/> for one it cannot.</summary>
    private static double? NumberOf(object? value) => value switch
    {
        double number => number,
        float number => number,
        decimal number => (double)number,
        int number => number,
        long number => number,
        string text when double.TryParse(
            text,
            System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture,
            out var parsed) => parsed,
        _ => null,
    };

    /// <summary>The value a JSON input is edited as: the document written out, or the text it was.</summary>
    private static string? JsonText(object? value) => value switch
    {
        null => null,
        string text => text,
        _ => JsonValues.FromAnyObject(value)?.ToJsonString(),
    };
}

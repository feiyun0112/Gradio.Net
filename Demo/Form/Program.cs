using Gradio.Net;

// A form: several fields, one button, and every value read at once.
//
// Nothing here declares a form. The Python version groups consecutive components into one when a button
// follows them, and pressing that button submits everything the form holds — which is why a single
// binding reads eight values. The grouping is not a binding of its own: it decides what a run is
// handed, and nothing else.
//
//     gr.Button("Submit").click(describe, [text, choice, ...], result)
//
// The second half is the other thing a form usually needs: one field deciding what another offers.
// A handler answers with `gr.update(...)`, which the client merges into the output rather than
// replacing it.

static string Describe(
    string text,
    object? choice,
    bool agree,
    IReadOnlyList<object?> tags,
    object? size,
    double marks,
    double count,
    double rating) =>
    $"""
    text: {text}
    dropdown: {choice}
    agree: {agree}
    tags: {string.Join(", ", tags)}
    size: {size}
    marks: {marks}
    count: {count}
    rating: {rating}
    """;

static object Symbols(object? category)
{
    IReadOnlyList<Choice> names = category switch
    {
        "Fruit" => ["Apple", "Pear", "Plum"],
        "Bread" => ["Rye", "Sourdough"],
        _ => [],
    };

    // The update names the props it changes and nothing else: the second dropdown is offered the
    // new list, and its value is cleared so that the browser drops a selection that may no longer
    // be offered.
    return gr.Update(new Update
    {
        Choices = names,
        Value = Update.Clear,
    });
}

var demo = gr.Blocks(title: "Form");

using (demo)
{
    gr.Markdown(
        """
        # Form

        Fill anything in and press **Submit**: the fields above the button are one form, so the run
        is handed all eight values rather than one at a time.
        """);

    var text = gr.Textbox(label: "Text", value: "hello");
    var choice = gr.Dropdown(choices: ["First", "Second", "Third"], value: "Second", label: "Dropdown");
    var agree = gr.Checkbox(label: "Agree", value: true);
    var tags = gr.CheckboxGroup(
        choices: ["one", "two", "three"],
        value: new List<object?> { "one", "three" },
        label: "Tags");
    var size = gr.Radio(choices: ["S", "M", "L"], value: "M", label: "Size");
    var marks = gr.Slider(minimum: 0, maximum: 5, value: 3, step: 1, label: "Marks");
    var count = gr.Number(value: 2, label: "Count");
    var rating = gr.Slider(minimum: 0, maximum: 10, value: 7, step: 1, label: "Rating");

    var submitted = gr.Textbox(label: "Submitted", lines: 8, interactive: false);

    gr.Button("Submit", variant: "primary")
        .Click(
            Describe,
            [text, choice, agree, tags, size, marks, count, rating],
            [submitted],
            apiName: "submit");

    gr.Markdown(
        """
        ## One field deciding another

        The category says which symbols exist; picking one answers with `gr.Update`, which changes
        the second list's choices and leaves everything else about it alone.
        """);

    var category = gr.Dropdown(choices: ["Fruit", "Bread"], label: "Category");
    var symbol = gr.Dropdown(choices: [], label: "Symbol");

    category.Change(
        Symbols,
        [category],
        [symbol],
        apiName: "symbols");
}

// Blocks until Ctrl-C. The page is at http://127.0.0.1:7860.
demo.Launch();

using Gradio.Net;

// The quickstart: one function, two inputs, one output, and a page built around them.
//
// The Python this is ported from is the Python version's own quickstart. The port changes two things: `=`
// becomes `:` on a named argument, and `greet` becomes `Greet` because a member is PascalCase here.
// The title, the description and the endpoint's name are this page's own; nothing else differs.
//
//     import gradio as gr
//
//     def greet(name, intensity):
//         return "Hello, " + name + "!" * int(intensity)
//
//     demo = gr.Interface(fn=greet, inputs=["text", "slider"], outputs=["text"])
//     demo.launch()

static string Greet(string name, double intensity) =>
    "Hello, " + name + new string('!', (int)intensity);

// `api_name` is the one addition: the Python version names the endpoint after the function, and a top-level
// local function has no name the compiler keeps, so the name is given rather than derived.
var demo = gr.Interface(
    Greet,
    inputs: ["text", "slider"],
    outputs: ["text"],
    title: "Hello, Gradio.Net",
    description: "Type a name, pick how many exclamation marks, press Submit.",
    apiName: "greet");

// Blocks until Ctrl-C. The page is at http://127.0.0.1:7860.
demo.Launch();

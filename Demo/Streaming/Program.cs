using System.Runtime.CompilerServices;
using System.Text;
using Gradio.Net;

// A handler that answers in pieces, and a progress bar that moves while it does.
//
// A generator is the third shape a handler can have: one value in, a stream of values out. Every
// value it yields replaces the output, so each yield is the whole answer so far rather than the
// piece that was just added — the same rule the Python version's `yield` follows.
//
//     def type(text, progress=gr.Progress()):
//         written = ""
//         for index, letter in enumerate(text):
//             written += letter
//             progress((index + 1) / len(text), desc=f"{index + 1} of {len(text)}")
//             yield written
//             time.sleep(0.05)
//
//     gr.Button("Type it").click(type, text, output)

static async IAsyncEnumerable<string> Type(
    string text,
    Progress progress,
    [EnumeratorCancellation] CancellationToken cancellationToken)
{
    var written = new StringBuilder();

    for (var index = 0; index < text.Length; index++)
    {
        written.Append(text[index]);

        // The two arguments are the Python version's: how far along the run is, and what it is doing. The
        // page shows them while the stream is still going.
        progress.Report((double)(index + 1) / text.Length, $"{index + 1} of {text.Length}");

        yield return written.ToString();

        await Task.Delay(50, cancellationToken);
    }
}

var demo = gr.Blocks(title: "Streaming");

using (demo)
{
    gr.Markdown(
        """
        # Streaming

        Type a sentence and press **Type it**: the answer arrives a letter at a time, and the
        progress bar reports how much of it is done.
        """);

    var source = gr.Textbox(label: "Text", value: "Streaming is one value at a time.");

    var output = gr.Textbox(label: "Answer", lines: 3, interactive: false);

    // `Progress` and `CancellationToken` are not inputs: the framework fills them in because the
    // handler asks for them by type. The attribute is C#'s own
    // requirement — an async iterator has to say which parameter carries the token.
    gr.Button("Type it", variant: "primary")
        .Click(Type, [source], [output], apiName: "type_it");
}

// Blocks until Ctrl-C. The page is at http://127.0.0.1:7860.
demo.Launch();

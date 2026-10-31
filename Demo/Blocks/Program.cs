using Gradio.Net;

// A page written by hand: the blocks are laid out explicitly and events are bound to them.
//
// `gr.Blocks()` opens the scope the blocks are written into, so every `gr.…(...)` line inside the
// `using` is a line of the page. That is the Python version's `with gr.Blocks() as demo:` with one word
// changed, and it is the whole of the page-building story: what is written is what is drawn.
//
//     with gr.Blocks(title="Shout") as demo:
//         gr.Markdown("…")
//         with gr.Row():
//             with gr.Column():
//                 name = gr.Textbox(label="Name")
//             with gr.Column():
//                 greeting = gr.Textbox(label="Greeting")
//         name.change(hello, name, greeting)
//         gr.Button("Shout", variant="primary").click(shout, name, greeting)
//         with gr.Tabs() as tabs:
//             with gr.Tab("Lion", id="lion"):
//                 …
//             with gr.Tab("Tiger", id="tiger"):
//                 …
//         tabs.select(show_pick, None, picked)
//         gr.Button("Switch to the tiger").click(switch_tab, None, tabs)
//
//     demo.launch()

static string Hello(string name) => $"Hello, {name}";

static string Shout(string name) => $"{name.ToUpperInvariant()}!";

var demo = gr.Blocks(title: "Shout");

using (demo)
{
    gr.Markdown(
        """
        # Shout

        Type a name: the greeting follows as you type. Press **Shout** to have it shouted back.
        """);

    Textbox name;
    Textbox greeting;

    using (gr.Row())
    {
        using (gr.Column(scale: 1))
        {
            name = gr.Textbox(label: "Name", placeholder: "Ada");
        }

        using (gr.Column(scale: 2))
        {
            greeting = gr.Textbox(label: "Greeting", interactive: false);
        }
    }

    // A typed binding: the handler is handed the value of the component and answers the output.
    // The names in the two lists are the components, in the order the handler reads and writes them.
    //
    // `api_name` is given on both bindings because an endpoint's name is derived from the handler's
    // and a top-level local function has no name the compiler keeps.
    name.Change(
        Hello,
        [name],
        [greeting],
        apiName: "hello");

    // The button reads a component that is not itself, so the binding names it as an input. The
    // other overload — `Click(fn, outputs)` — is for a handler that reads nothing.
    gr.Button("Shout", variant: "primary")
        .Click(Shout, [name], [greeting], apiName: "shout");

    gr.Markdown(
        """
        ## Tabs

        A tab bar is a layout like any other: the blocks written inside `gr.Tab("…")` belong to that
        tab, and each tab keeps its own fields — pressing a button in one does not touch the other.
        A layout raises events too, and they bind like a button's click.
        """);

    Tabs tabs;

    using (tabs = gr.Tabs())
    {
        using (gr.Tab("Lion", id: "lion"))
        {
            var lion = gr.Textbox(label: "Lion", value: "lion");

            gr.Button("Shout the lion")
                .Click(Shout, [lion], [lion], apiName: "lion");
        }

        using (gr.Tab("Tiger", id: "tiger"))
        {
            var tiger = gr.Textbox(label: "Tiger", value: "tiger");

            gr.Button("Shout the tiger")
                .Click(Shout, [tiger], [tiger], apiName: "tiger");
        }
    }

    var picked = gr.Textbox(label: "Picked", interactive: false);

    // `tabs.select(fn, inputs, outputs)` in the Python version: the handler is handed the payload
    // the event carries — which tab was picked — and answers a component of its own.
    tabs.Select(
        (SelectData evt) => $"You picked the {evt.Value} tab.",
        [],
        [picked],
        apiName: "picked");

    // A handler that answers `gr.update(selected="tiger")`: the tab bar switches to that tab
    // without the page reloading. The value is the tab's id, or its label for a tab given none.
    gr.Button("Switch to the tiger")
        .Click(() => new Update { Selected = "tiger" }, [tabs], apiName: "switch");
}

// Blocks until Ctrl-C. The page is at http://127.0.0.1:7860.
demo.Launch();

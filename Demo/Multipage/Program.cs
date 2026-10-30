using Gradio.Net;

// One application, several pages, each with its own URL.
//
// A page is declared with `demo.Route(name)`, and everything written inside it belongs to that page:
// the blocks, the bindings, and the endpoints that page's API info describes. The navbar is what
// offers them; it is a block of the page rather than of the application, so each page says what its
// own navigation is.
//
//     with gr.Blocks(title="Two pages") as demo:
//         gr.Navbar()
//         …
//     with demo.route("About"):
//         gr.Navbar()
//         …
//
// Several *applications* behind one tab bar is a different thing — `gr.TabbedInterface` draws tabs
// inside a single page instead of giving each one an address.
//
// `gr.Navbar()` is the one factory written by hand: the block is not in the Python version's `__all__`, so the
// generator does not make it, and it is added to the layer beside the generated ones because a page
// with more than one address has to say what its navigation is.

static string Hello(string name) => $"Hello, {name}";

static string Echo(string value) => value;

var demo = gr.Blocks(title: "Two pages");

using (demo)
{
    gr.Navbar();

    gr.Markdown(
        """
        # Home

        This is the page at `/`. The link above leads to the second page, which is served at
        `/about` and is written further down this file.
        """);

    var name = gr.Textbox(label: "Name", placeholder: "Ada");
    var greeting = gr.Textbox(label: "Greeting", interactive: false);

    name.Change(
        Hello,
        [name],
        [greeting],
        apiName: "hello");

    // Everything from here to the closing brace belongs to the second page.
    using (demo.Route("About"))
    {
        gr.Navbar();

        gr.Markdown(
            """
            # About

            A page is a scope: the blocks written inside `demo.Route("About")` are served at
            `/about` and left out of the home page's document.
            """);

        // A binding made on a page is that page's endpoint: this one is in `/about`'s API info and
        // not in the home page's, which is what makes the two pages separate documents rather than
        // one document served twice.
        var about = gr.Textbox(label: "About you", placeholder: "one line");
        var echoed = gr.Textbox(label: "Echo", interactive: false);

        about.Change(
            Echo,
            [about],
            [echoed],
            apiName: "echo");
    }
}

// Blocks until Ctrl-C. The pages are at http://127.0.0.1:7860/ and http://127.0.0.1:7860/about.
demo.Launch();

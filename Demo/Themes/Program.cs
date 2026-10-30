using Gradio.Net;
// A theme is one line: a preset, or the base theme with a few choices on top.
//
// The twelve presets the Python version ships are `gr.Themes.Base`, `Default`, `Monochrome`, `Soft`, `Glass`,
// `Origin`, `Citrus`, `Ocean`, `Mario`, `Ember`, `Neon` and `Cyberpunk`. Each takes the same eight
// choices, and a choice left out is the preset's own:
//
//     demo.theme = gr.themes.Soft(primary_hue="emerald", radius_size="lg")
//
// `gr.Theme(...)` is the base theme with those choices — the same document as `gr.Themes.Base(...)`,
// which is the same thing spelled as a preset. The theme is written to the page as CSS when the
// application starts, so it is set before `Launch()`.

static string Hello(string name, double marks) =>
    $"Hello, {name}{new string('!', (int)marks)}";

var demo = gr.Blocks(title: "Themes");

using (demo)
{
    gr.Markdown(
        """
        # Themes

        Emerald accents, large corners, and everything else from the **Soft** preset. Change the
        line at the bottom of this file and restart to see another one.
        """);

    var name = gr.Textbox(label: "Name", placeholder: "Ada");

    var marks = gr.Slider(minimum: 0, maximum: 5, value: 3, step: 1, label: "Exclamation marks");

    var greeting = gr.Textbox(label: "Greeting", interactive: false);

    gr.Button("Say hello", variant: "primary")
        .Click(Hello, [name, marks], [greeting]);
}

// The theme belongs to the application, not to a block: it is read when the page is written.
demo.Theme = gr.Themes.Soft(primaryHue: "emerald", radiusSize: "lg");

// Blocks until Ctrl-C. The page is at http://127.0.0.1:7860.
demo.Launch();

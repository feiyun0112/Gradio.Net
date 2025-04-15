namespace Demo.AspNetCore.Launcher.Components;

class LayoutBlockBuilder : IGradioBlockBuilder
{
    public async Task Build()
    {
        gr.Markdown("# Layout Demo");

        gr.Markdown("## Row/Column");
        using (gr.Row())
        {
            using (gr.Column(scale: 1))
            {
                Textbox text1 = gr.Textbox();
                Textbox text2 = gr.Textbox();
            }

            using (gr.Column(scale: 4))
            {
                Button btn1 = gr.Button("Button 1");
                Button btn2 = gr.Button("Button 2");
            }
        }

        gr.Markdown("## Tab");
        using (gr.Tab("Lion"))
        {
            gr.Textbox("lion");
            gr.Button("New Lion");
        }
        using (gr.Tab("Tiger"))
        {
            gr.Textbox("tiger");
            gr.Button("New Tiger");
        }

        gr.Markdown("## Group");
        using (gr.Group())
        {
            gr.Textbox(label: "First");
            gr.Textbox(label: "Last");
        }

        gr.Markdown("## Accordion");
        using (gr.Accordion("See Details"))
        {
            gr.Markdown("lorem ipsum");
        }
    }
}

using Gradio.Net;
using System.Reflection.Emit;
using static System.Formats.Asn1.AsnWriter;

namespace demo
{
    public static class LayoutDemo
    {
        public static async Task Create()
        {
            gr.Markdown("# Layout Demo");

            gr.Markdown("## Row/Column");
            using (gr.Row())
            {
                using (gr.Column(scale: 1))
                {
                    var text1 = gr.Textbox();
                    var text2 = gr.Textbox();
                }

                using (gr.Column(scale: 4))
                {
                    var btn1 = gr.Button("Button 1");
                    var btn2 = gr.Button("Button 2");
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
}

using Gradio.Net;
using System.Reflection.Emit;
using static System.Formats.Asn1.AsnWriter;
using static System.Net.Mime.MediaTypeNames;

namespace demo
{
    public static class FormDemo
    {
        public static async Task Create()
        {
            gr.Markdown("# FormDemo Demo");

            gr.HTML(value: "<p style='margin-top: 1rem, margin-bottom: 1rem'>This <em>example</em> was <strong>written</strong> in <a href='https://en.wikipedia.org/wiki/HTML' _target='blank'>HTML</a> </p>");


            using (gr.Column())
            {
                var text1 = gr.Textbox();
                var dropdown1 = gr.Dropdown(choices: new[] { "First Choice", "Second Choice", "Third Choice" });
                var checkbox1 = gr.Checkbox();
                var checkboxGroup1 = gr.CheckboxGroup(choices: new[] { "First Choice", "Second Choice", "Third Choice" });
                var multimodalTextbox1 = gr.MultimodalTextbox(interactive:true);               
                var number1 = gr.Number();
                var radio1 = gr.Radio(choices: ["First Choice", "Second Choice", "Third Choice"]);
                var slider1 = gr.Slider();

                var text_Result = gr.Textbox(label:"Form Value", interactive:false);

                var btn = gr.Button("Run");
                await btn.Click(fn: async (input) => gr.Output($@"
Textbox: {Textbox.Payload(input.Data[0])}
Dropdown: {string.Join(", ",Dropdown.Payload(input.Data[1]))}
Checkbox: {Checkbox.Payload(input.Data[2])}
CheckboxGroup: {string.Join(", ", CheckboxGroup.Payload(input.Data[3]))}
MultimodalTextbox: {MultimodalTextbox.Payload(input.Data[4]).Files.FirstOrDefault()?.OrigName}
Number: {Number.Payload(input.Data[5])}
Radio: {string.Join(", ", Radio.Payload(input.Data[6]))}
Slider: {Slider.Payload(input.Data[7])}
"), inputs: new Component[] { text1, dropdown1, checkbox1, checkboxGroup1, multimodalTextbox1, number1, radio1, slider1 }, outputs: new[] { text_Result });
            }
        }
    }
}

using Gradio.Net;

namespace demo;

public static class FormDemo
{
    public static async Task Create()
    {
        gr.Markdown("# FormDemo Demo");

        gr.HTML(value: "<p style='margin-top: 1rem, margin-bottom: 1rem'>This <em>example</em> was <strong>written</strong> in <a href='https://en.wikipedia.org/wiki/HTML' _target='blank'>HTML</a> </p>");


        using (gr.Column())
        {
            Textbox text1 = gr.Textbox();
            Dropdown dropdown1 = gr.Dropdown(choices: ["First Choice", "Second Choice", "Third Choice"]);
            Checkbox checkbox1 = gr.Checkbox();
            CheckboxGroup checkboxGroup1 = gr.CheckboxGroup(choices: ["First Choice", "Second Choice", "Third Choice"]);
            MultimodalTextbox multimodalTextbox1 = gr.MultimodalTextbox(interactive:true);
            Number number1 = gr.Number();
            Radio radio1 = gr.Radio(choices: ["First Choice", "Second Choice", "Third Choice"]);
            Slider slider1 = gr.Slider();

            Textbox text_Result = gr.Textbox(label:"Form Value", interactive:false);

            Button btn = gr.Button("Run");
            await btn.Click(fn: async (input) => gr.Output($@"
Textbox: {Textbox.Payload(input.Data[0])}
Dropdown: {string.Join(", ",Dropdown.Payload(input.Data[1]))}
Checkbox: {Checkbox.Payload(input.Data[2])}
CheckboxGroup: {string.Join(", ", CheckboxGroup.Payload(input.Data[3]))}
MultimodalTextbox: {MultimodalTextbox.Payload(input.Data[4]).Files.FirstOrDefault()?.OrigName}
Number: {Number.Payload(input.Data[5])}
Radio: {string.Join(", ", Radio.Payload(input.Data[6]))}
Slider: {Slider.Payload(input.Data[7])}
"), inputs: [text1, dropdown1, checkbox1, checkboxGroup1, multimodalTextbox1, number1, radio1, slider1], outputs: [text_Result]);
        }
    }
}

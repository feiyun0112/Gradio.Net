using Gradio.Net;
using Gradio.Net.Components;

namespace demo;

public static class FormDemo
{
    public static async Task Create()
    {
        gr.Markdown("# FormDemo Demo");

        gr.Html(value: "<p style='margin-top: 1rem, margin-bottom: 1rem'>This <em>example</em> was <strong>written</strong> in <a href='https://en.wikipedia.org/wiki/HTML' _target='blank'>HTML</a> </p>");

        gr.Markdown("## Get Component Property");
        using (gr.Column())
        {
            Textbox text1 = gr.Textbox(type: "password");
            Dropdown dropdown1 = gr.Dropdown(choices: new[] { "First Choice", "Second Choice", "Third Choice" });
            Checkbox checkbox1 = gr.Checkbox();
            CheckboxGroup checkboxGroup1 = gr.CheckboxGroup(choices: new[] { "First Choice", "Second Choice", "Third Choice" });
            MultimodalTextbox multimodalTextbox1 = gr.MultimodalTextbox(interactive: true);
            Number number1 = gr.Number();
            Radio radio1 = gr.Radio(choices: new[] { "First Choice", "Second Choice", "Third Choice" });
            Slider slider1 = gr.Slider();

            Textbox text_Result = gr.Textbox(label: "Form Value", interactive: false);

            Button btn = gr.Button("Run");
            btn.Click(fn: new Func<string, object, bool, object, object, double, object, double, string>(
                (txt, drop, chk, chkgrp, mmtb, num, rad, sld) =>
                {
                    var dropStr = drop?.ToString() ?? "";
                    var chkgrpEnum = chkgrp as System.Collections.IEnumerable;
                    var chkgrpStr = chkgrpEnum != null ? string.Join(", ", chkgrpEnum.Cast<object>()) : "";
                    var mmtbDict = mmtb as Dictionary<string, object>;
                    var mmtbFiles = mmtbDict != null && mmtbDict.TryGetValue("files", out var f) ? f as List<string> : null;
                    var radStr = rad?.ToString() ?? "";
                    return $@"
                Textbox: {txt}
                Dropdown: {dropStr}
                Checkbox: {chk}
                CheckboxGroup: {chkgrpStr}
                MultimodalTextbox: {mmtbFiles?.FirstOrDefault()}
                Number: {num}
                Radio: {radStr}
                Slider: {sld}
                ";
                }), inputs: new object[] { text1, dropdown1, checkbox1, checkboxGroup1, multimodalTextbox1, number1, radio1, slider1 }, outputs: new[] { text_Result });
        }

        

        using (gr.Row())
        {
            var click_count = gr.State(value: 0);

            var btn = gr.Button("Show");
            var textCount = gr.Textbox(label: "Count", visible: false);
            btn.Click(update_props, inputs: new[] { click_count }, outputs: new object[] { click_count, btn, textCount });
        }
    }

    private static (object, object, object) update_props(object clickCountObj)
    {
        var clickCount = Convert.ToInt32(clickCountObj);
        var click_count = clickCount + 1;
        return (click_count,
            gr.Button(value: (click_count + 1) % 2 == 0 ? "Hide" : "Show"),
            gr.Textbox(value: ((click_count + 1) / 2).ToString(), visible: (click_count + 1) % 2 == 0));
    }

    private static (object, object) update_symbols(object categoryObj)
    {
        var symbols = new[] { "FFIU", "IGEB", "VCIT", "FCOR", "SKOR", "KORP", "LQDI" };
        string category = categoryObj is string[] arr ? (arr.FirstOrDefault() ?? "") : (categoryObj?.ToString() ?? "");
        if (category == "Long-Short Equity")
            gr.Warning("This is some warning.");

        return (gr.Dropdown(value: "", interactive: true), gr.Dropdown(choices: symbols, interactive: true));
    }
}

namespace Demo.AspNetCore.Launcher.Components;

class DemoBlockBuilder : IGradioBlockBuilder
{
    private readonly ILogger<DemoBlockBuilder> m_Logger;

    public DemoBlockBuilder(ILogger<DemoBlockBuilder> logger)
    {
        m_Logger = logger;
    }

    private Textbox m_Input;
    private Textbox m_Output;

    public async Task Build()
    {
        gr.Markdown("# First Demo");

        gr.Markdown("Start typing below and then click **Run** to see the output.");
        //Textbox input, output;
        using (gr.Row())
        {
            m_Input = gr.Textbox(placeholder: "What is your name?");
            m_Output = gr.Textbox();
        }
        Button btn = gr.Button("Run");
        await btn.Click(OnRunClick, new Component[] { m_Input }, new Component[] { m_Output, m_Input });
        //await btn.Click(fn: async (input) => gr.Output($"Welcome to Gradio.Net, {Textbox.Payload(input.Data[0])}!", ""),
        //    inputs: [m_Input], outputs: [m_Output, m_Input]);
    }

    private Task<Output> OnRunClick(Input input)
    {
        m_Logger.LogInformation(input.Data[0].ToString());
        return Task.FromResult(gr.Output($"Welcome to Gradio.Net, {Textbox.Payload(input.Data[0])}!", ""));
        //return Task.FromResult(gr.Output(m_Output, m_Input));
    }

}

//public static async Task Create()
//{
//    gr.Markdown("# Progress Demo");

//    Button load = gr.Button("Load");
//    Label label = gr.Label(label: "Loader");
//    load.Click(LoadSet, outputs: [label]);
//}

//static async Task<Output> LoadSet(Input input)
//{
//    const int count = 24;
//    input.Progress = gr.Progress(count);
//    for (int i = 0; i < count; i++)
//    {
//        input.Progress.Report(i, desc: "Loading...");
//        await Task.Delay(100);
//    }
//    return gr.Output("Loaded");
//}
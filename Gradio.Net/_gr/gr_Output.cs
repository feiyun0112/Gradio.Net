namespace Gradio.Net;

public static partial class gr
{
    internal static Input Input(BlockFunction blockFunction, params object[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = blockFunction.Inputs.ElementAt(i).PreProcess(data[i]);
        }
        return new Input { Data = data };
    }
    public static Output Output(params object[] data)
    {
        return new Output { Data = data };
    }

    internal static object[] Output(EventResult eventResult, params object[] data)
    {
        BlockFunction blockFunction = eventResult.BlockFunction;
        string rootUrl = eventResult.Event.RootUrl;

        for (int i = 0; i < data.Length; i++)
        {
            data[i] = blockFunction.Outputs.ElementAt(i).PostProcess(rootUrl, data[i]);
        }

        return data;
    }
}

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
        if (blockFunction.Outputs == null && data != null && data.Length > 0)
        {
            throw new ArgumentException($"Output Component Count not same as Data length");
        }
        else if (blockFunction.Outputs != null && data != null && data.Length != blockFunction.Outputs.Count())
        {
            throw new ArgumentException($"Output Component Count {blockFunction.Outputs.Count()} not same as Data length {data.Length}");
        }
        string rootUrl = eventResult.Event.RootUrl;

        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] is Component component)
            {
                if (data[i].GetType() != blockFunction.Outputs.ElementAt(i).GetType())
                {
                    throw new ArgumentException($"Output Component not same as Output Data");
                }

                data[i] = Component.PostProcess(component, blockFunction.Outputs.ElementAt(i));
            }
            else
            {
                data[i] = blockFunction.Outputs.ElementAt(i).PostProcess(rootUrl, data[i]);
            }
        }

        return data;
    }
}

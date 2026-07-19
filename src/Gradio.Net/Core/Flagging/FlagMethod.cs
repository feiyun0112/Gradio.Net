namespace Gradio.Net.Core.Flagging;

using Gradio.Net;
using Gradio.Net.Core;

public class FlagMethod
{
    protected FlaggingCallback FlaggingCallback { get; }

    protected string Label { get; }

    protected string? Value { get; }

    protected bool VisualFeedback { get; }

    public FlagMethod(FlaggingCallback flaggingCallback, string label, string? value, bool visualFeedback = true)
    {
        FlaggingCallback = flaggingCallback;
        Label = label;
        Value = value;
        VisualFeedback = visualFeedback;
    }

    public object? Invoke(Request request, params object[] flagData)
    {
        try
        {
            FlaggingCallback.Flag(flagData.ToList(), Value, request.Username);
        }
        catch (Exception e)
        {
            if (VisualFeedback)
            {
                return Helpers.Update(
                    kwargs:
                    [
                        new KeyValuePair<string, object>("value", "Error!"),
                        new KeyValuePair<string, object>("interactive", true)
                    ]
                );
            }
        }

        if (!VisualFeedback)
        {
            return null;
        }

        // Provide enough time for the user to observe button change
        Thread.Sleep(800);
        return Reset();
    }

    public Dictionary<string, object> Reset()
    {
        return Helpers.Update(
            kwargs:
            [
                new KeyValuePair<string, object>("value", Label),
                new KeyValuePair<string, object>("interactive", true)
            ]
        );
    }
}

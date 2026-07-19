namespace Gradio.Net.Core.Flagging;


public abstract class FlaggingCallback
{
    public abstract void Setup(IEnumerable<Components.Component> components, string flaggingDir);

    public abstract int Flag(List<object> flagData, string? flagOption = null, string? username = null);
}

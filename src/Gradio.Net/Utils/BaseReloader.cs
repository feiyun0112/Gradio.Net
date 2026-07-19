namespace Gradio.Net.Utils;

public abstract class BaseReloader
{
    public abstract object RunningApp { get; }

    public void SwapBlocks(object demo)
    {
    }
}

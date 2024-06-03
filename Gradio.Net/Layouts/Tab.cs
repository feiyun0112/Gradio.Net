namespace Gradio.Net;

public class Tab : Blocks
{
    internal Tab() { }
    internal string Label { get;  set; }
    internal string ComponentId { get;  set; }
    protected override string GetTypeName()
    {
        return "tabitem";  
    }

}

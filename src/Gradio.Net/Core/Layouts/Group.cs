namespace Gradio.Net.Core.Layouts;

public class Group : BlockContext
{
    public Group(
        bool? visible = null,
        string? elemId = null,
        List<string>? elemClasses = null,
        bool render = true,
        object? key = null,
        List<string>? preservedByKey = null)
        : base(elemId, elemClasses, visible ?? true, render, key, preservedByKey)
    {
    }

    public override bool SkipApi => true;
}

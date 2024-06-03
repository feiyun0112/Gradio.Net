namespace Gradio.Net;

public class Form : Blocks
{
    internal Form() { }
    internal int MinWidth { get; set; }
    internal int Scale { get; set; }

    public override void Add(Block item)
    {
        FormComponent? formComponet = item as FormComponent;
        if (this.ParentBlocks is Row row)
        {
            int? scale = formComponet.Scale;
            int? minWidth = formComponet.MinWidth;
            this.Scale += scale.HasValue ? scale.Value : 1;
            this.MinWidth += minWidth.HasValue ? minWidth.Value : 0;
        }
        else if (this.ParentBlocks is Blocks blocks && blocks.FillHeight.HasValue && blocks.FillHeight.Value)
        {
            int? scale = formComponet.Scale;
            this.Scale += scale.HasValue ? scale.Value : 1;
        }

        base.Add(item);
    }

}

using Gradio.Net.Enums;


namespace Gradio.Net;

public class Row : Blocks
{
    internal Row() { }
    internal RowVariant Variant { get;  set; }
    internal bool EqualHeight { get; set; } = true;
}

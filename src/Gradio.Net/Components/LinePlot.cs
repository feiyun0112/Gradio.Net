namespace Gradio.Net;
/// <summary>
/// A chart that draws its rows as points joined by a line.
/// </summary>
/// <remarks>
/// The whole of the difference from the other two charts is the mark: the parameters, the value and
/// the block the frontend draws with are the base's. The Python version writes the three the same
/// way, each one overriding nothing but <c>get_mark</c>.
/// </remarks>
[GradioBlock("lineplot")]
public sealed partial class LinePlot : NativePlot
{
    internal LinePlot()
    {
    }

    /// <inheritdoc />
    protected override string Mark => "line";
}

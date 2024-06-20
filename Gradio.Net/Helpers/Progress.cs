namespace Gradio.Net.Helpers;

public class Progress
{
    internal Progress(int total)
    {
        if (total <= 0)
        {
            throw new ArgumentException("total must be grater than zero");
        }

        Length = total;
    }

    internal string Unit { get; set; } = "steps";
    internal string Desc { get; set; }
    internal int Index { get; set; }
    internal int Length { get; set; }

    public void Report(
        int value,
        string desc = null,
        int? total = null,
        string unit = "steps"
    )
    {
        Index = value;
        if (desc != null)
        {
            Desc = desc;
        }
        if (total != null)
        {
            Length = total.Value;
        }
        Unit = unit;
    }
}

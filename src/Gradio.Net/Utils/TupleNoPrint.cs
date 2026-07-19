
namespace Gradio.Net.Utils;

public class TupleNoPrint<T1, T2>
{
    public T1 Item1 { get; }

    public T2 Item2 { get; }

    public TupleNoPrint(T1 item1, T2 item2)
    {
        Item1 = item1;
        Item2 = item2;
    }

    public override string ToString()
    {
        return string.Empty;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Item1, Item2);
    }
}

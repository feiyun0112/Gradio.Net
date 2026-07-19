
namespace Gradio.Net.Components;

public class HighlightedTextData
{
    public List<HighlightedTokenData> Root { get; set; } = new();

    public List<Tuple<string, object?>> ModelDump()
        => Root.Select(x => Tuple.Create(x.Token, x.ClassOrConfidence)).ToList();
}

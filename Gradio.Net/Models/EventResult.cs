namespace Gradio.Net;

internal class EventResult
{
    public IAsyncEnumerable<Output> StreamingOutputTask { get; internal set; }
    internal BlockFunction BlockFunction { get;  set; }
    internal Input Input { get;  set; }
    internal Event Event { get; set; }
    internal Task<Output> OutputTask { get; set; }
}

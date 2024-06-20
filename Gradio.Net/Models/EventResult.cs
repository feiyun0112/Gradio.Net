using System.Collections.Generic;

namespace Gradio.Net;

internal class EventResult
{
    public object[] AsyncEnumeratorOutputData { get; internal set; }
    public IAsyncEnumerator<Output> AsyncEnumeratorOutput { get; internal set; }
    public ValueTask<bool> AsyncEnumeratorOutputNextTask { get; internal set; }
    public IAsyncEnumerable<Output>? StreamingOutputTask { get; internal set; }
    internal BlockFunction BlockFunction { get; set; }
    internal Input Input { get; set; }
    internal Event Event { get; set; }
    internal Task<Output> OutputTask { get; set; }
}

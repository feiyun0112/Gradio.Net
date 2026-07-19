using System.Collections.Concurrent;
using System.Diagnostics;
using System.Web;
using Gradio.Net.Core;
using Gradio.Net.Utils;


namespace Gradio.Net;

public class Event
{
    public readonly string Id;
    public string SessionHash { get; set; }
    public BlockFunction Fn { get; set; }
    public object Request { get; set; }
    public string Username { get; set; }
    public string ConcurrencyId { get; set; }
    public object Data { get; set; }
    public bool ProgressPending { get; set; }
    public bool Alive { get; set; }
    public bool Closed { get; set; }
    public int NCalls { get; set; }
    public float RunTime { get; set; }
    public ManualResetEventSlim Signal { get; set; }

    public Event(
        string sessionHash,
        BlockFunction fn,
        object request,
        string username
    )
    {
        Id = Guid.NewGuid().ToString();
        SessionHash = sessionHash ?? Id;
        Fn = fn;
        Request = request;
        Username = username;
        ConcurrencyId = fn.ConcurrencyId;
        Alive = true;
        Closed = false;
        NCalls = 0;
        RunTime = 0;
        Signal = new ManualResetEventSlim(false);
    }

    public bool Streaming => Fn.Connection == "stream";

    public bool IsFinished
    {
        get
        {
            if (!Streaming)
            {
                throw new InvalidOperationException("Cannot access IsFinished during a non-streaming event");
            }
            if (Closed)
            {
                return true;
            }
            if (Fn.TimeLimit == null)
            {
                return false;
            }
            return RunTime >= Fn.TimeLimit;
        }
    }
}

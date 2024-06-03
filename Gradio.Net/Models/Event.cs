namespace Gradio.Net;

using System;

internal class Event
{
    internal string RootUrl { get; set; }
    internal string Id { get; } = Guid.NewGuid().ToString("N").ToLowerInvariant();
    internal string SessionHash { get; set; }
    internal int FnIndex { get; set; }
    internal string Username { get; set; }
    internal string ConcurrencyId { get; set; }
    internal PredictBodyIn Data { get; set; }
    internal bool ProgressPending { get; set; }
    internal bool Alive { get; set; } = true;

}

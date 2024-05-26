using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gradio.Net
{
    using Microsoft.AspNetCore.Http;
    using System;

    internal class Event
    {
        internal string Id { get; } = Guid.NewGuid().ToString("N").ToLowerInvariant();
        internal string SessionHash { get; set; }
        internal int FnIndex { get; set; }
        internal string Username { get; set; }
        internal string ConcurrencyId { get; set; }
        internal PredictBodyIn Data { get; set; }
        internal bool ProgressPending { get; set; }
        internal bool Alive { get; set; } = true;

    }
}

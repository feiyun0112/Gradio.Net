using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gradio.Net
{
    internal class EventResult
    {
        internal Event Event { get; set; }
        internal Task<Output> OutputTask { get; set; }
    }
}

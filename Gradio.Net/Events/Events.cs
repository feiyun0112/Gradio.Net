using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gradio.Net
{
    internal static class Events
    {
        internal static EventListener Click { get; } = new EventListener("click", doc: "Triggered when the {{ component }} is clicked.");
    }
}

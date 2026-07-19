using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

using Gradio.Net.Components;
using Gradio.Net.Core;
using Gradio.Net.Core.Exceptions;
using Gradio.Net.Core.Flagging;
using Gradio.Net.Core.Layouts;
using Gradio.Net.Events;


namespace Gradio.Net;

public static class GradioUtils
{
    public static void CloseAll(bool verbose = true)
    {
        if (verbose)
        {
        }

        // Prefer the canonical implementation.
        Blocks.CloseAll();
    }
}

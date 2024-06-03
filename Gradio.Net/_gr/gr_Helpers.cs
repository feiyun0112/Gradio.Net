
using Gradio.Net.Helpers;

namespace Gradio.Net;

public static partial class gr
{

    public static Progress Progress(int total)
    { 
        return new Progress(total);
    }
}


using System;
using System.Collections.Generic;
using System.Text;


namespace Gradio.Net
{
    public static partial class gr
    {
        public static Input Input(params object[] data)
        {
            return new Input { Data = data };
        }
        public static Output Output(params object[] data)
        {
            return new Output { Data = data };
        }
    }
}

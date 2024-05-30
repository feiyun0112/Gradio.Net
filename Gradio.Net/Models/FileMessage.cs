using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gradio.Net
{
    internal class FileMessage
    {
        public FileData File { get; set; }
        public string AltText { get; set; } = null;
    }
}

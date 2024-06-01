using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gradio.Net
{
    public class MultimodalData
    {
        public string Text { get; set; }
        public IEnumerable<FileData> Files { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gradio.Net.Models
{
    public class LabelData
    {
        public string Label { get; set; }
        public IEnumerable<LabelConfidence> Confidences { get; set; }

        public class LabelConfidence
        {
            public string Label { get; set; }
            public decimal Confidence { get; set; }
        }
    }
}

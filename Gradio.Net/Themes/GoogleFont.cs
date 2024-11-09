using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gradio.Net.Themes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    public class GoogleFont : Font
    {
        public List<int> Weights { get; }

        public GoogleFont(string name, List<int> weights = null) : base(name)
        {
            Weights = weights ?? new List<int> { 400, 600 };
        }

        public override Dictionary<string, string> Stylesheet()
        {
            string urlTemplate = Context.App.GradioServiceConfig.GoogleFontUrlTemplate;

            string url = urlTemplate.Replace("{name}", this.Name.Replace(" ", "+"))
                .Replace("{weight}", string.Join(";", Weights.Select(weight => weight.ToString())));

            return new Dictionary<string, string> { { "url", url }, { "css", null } };
        }
    }
}

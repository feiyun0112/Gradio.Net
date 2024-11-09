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

    public class LocalFont : Font
    {
        public List<int> Weights { get; }

        public LocalFont(string name, List<int> weights = null) : base(name)
        {
            // Provide a default value for weights if not provided  
            Weights = weights ?? new List<int> { 400, 700 };
        }

        public override Dictionary<string, string> Stylesheet()
        {
            string CssTemplate = @"  
            @font-face {  
                font-family: '{name}';  
                src: url('static/fonts/{fileName}/{fileName}-{weight}.woff2') format('woff2');  
                font-weight: {weight};  
                font-style: normal;  
            }  
            ";

            List<string> cssRules = new List<string>();
            foreach (var weight in Weights)
            {
                string weightName = weight == 400 ? "Regular" : weight == 700 ? "Bold" : weight.ToString();
                string formattedCss = CssTemplate.Replace("{name}", Name).Replace("{fileName}", Name.Replace(" ", "")).Replace("{weight}", weightName);
                cssRules.Add(formattedCss);
            }

            return new Dictionary<string, string> { { "url", null }, { "css", string.Join(Environment.NewLine, cssRules) } };
        }
    }
}

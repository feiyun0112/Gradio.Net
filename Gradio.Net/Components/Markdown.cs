using System;
using System.Collections.Generic;
using System.Text;


namespace Gradio.Net
{
    public class Markdown:Component
    {
        private IEnumerable<Dictionary<string, object>> _latexDelimiters;

        internal Markdown()
        {
            this._latexDelimiters = new List<Dictionary<string, object>>
                    { 
                        new Dictionary<string, object>
                        {
                            { "left", "$$"},{"right", "$$" },{ "display", true}
                        }
                    };
        }

        internal IEnumerable<Dictionary<string, object>> LatexDelimiters 
        { 
            get 
            {
                return _latexDelimiters;
            }
            set {
                if (value != null)
                {
                    _latexDelimiters = value;
                }
            }
        }
        internal bool SanitizeHtml { get;  set; }
        internal bool LineBreaks { get;  set; }
        internal bool HeaderLinks { get; set; }

        protected override Dictionary<string, object> GetApiInfo()
        {
            return new Dictionary<string, object>() { { "type", "string" } };
        }
        internal override object PreProcess(object data)
        {
            string result = null;
            if (data == null)
            {
                return result;
            }

            result = data.ToString();
            return result;
        }

        internal override object PostProcess(string rootUrl, object data)
        {
            if (data == null)
            {
                return null;
            }

            return data.ToString();
        }
    }
}

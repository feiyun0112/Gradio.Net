namespace Gradio.Net;

public class Markdown : Component
{
    internal bool? SanitizeHtml { get; set; }
    internal bool? LineBreaks { get; set; }
    internal bool? HeaderLinks { get; set; }
    private IEnumerable<Dictionary<string, object>> _latexDelimiters;

    static Dictionary<string, object> _defaultProps = new Dictionary<string, object>()
    {  { nameof(Value), "" },
         { nameof(ShowLabel), true },
           { nameof(Rtl), false },
             { nameof(Visible), true},
               { nameof(Render), true },


          { nameof(SanitizeHtml), true },


            { nameof(LineBreaks), false },
             { nameof(HeaderLinks), false },
    };
    protected override object? GetDefaultProp(string name) => _defaultProps.ContainsKey(name) ? _defaultProps[name] : null;
    internal Markdown()
    {
        this._latexDelimiters = new List<Dictionary<string, object>>
                {
                    new() {
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
        set
        {
            if (value != null)
            {
                _latexDelimiters = value;
            }
        }
    }


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

using Gradio.Net.Enums;
using System.ComponentModel;


namespace Gradio.Net;

public class Row : Blocks
{
    internal Row() { }
    internal RowVariant? Variant { get; set; }
    internal bool? EqualHeight { get; set; }

    static Dictionary<string, object> _defaultProps = new Dictionary<string, object>()
    {
          { nameof(Variant),RowVariant.Default },
             { nameof(Visible), true},


          { nameof(Render), true },
            { nameof(EqualHeight), true },

    };
    protected override object? GetDefaultProp(string name) => _defaultProps.ContainsKey(name) ? _defaultProps[name] : null;
}

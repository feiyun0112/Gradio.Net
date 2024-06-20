using Gradio.Net.Enums;
using System.ComponentModel;
using static System.Formats.Asn1.AsnWriter;


namespace Gradio.Net;

public class Column : Blocks
{
    internal Column() { }
    internal int? Scale { get; set; }
    internal int? MinWidth { get; set; }
    internal ColumnVariant? Variant { get; set; }
    static Dictionary<string, object> _defaultProps = new Dictionary<string, object>()
    {
        { nameof(Scale),  1 },
         { nameof(MinWidth), 320 },
         { nameof(Variant), ColumnVariant.Default },
           { nameof(Visible), true },
        { nameof(Render), true },
    };
    protected override object? GetDefaultProp(string name) => _defaultProps.ContainsKey(name) ? _defaultProps[name] : null;
}

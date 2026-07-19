
namespace Gradio.Net.Core.Themes.Utils;

public class ThemeColor
{
    public static List<ThemeColor> All { get; } = new();

    public string C50 { get; }
    public string C100 { get; }
    public string C200 { get; }
    public string C300 { get; }
    public string C400 { get; }
    public string C500 { get; }
    public string C600 { get; }
    public string C700 { get; }
    public string C800 { get; }
    public string C900 { get; }
    public string C950 { get; }
    public string? Name { get; }

    public ThemeColor(string c50, string c100, string c200, string c300, string c400, string c500, string c600, string c700, string c800, string c900, string c950, string? name = null)
    {
        C50 = c50;
        C100 = c100;
        C200 = c200;
        C300 = c300;
        C400 = c400;
        C500 = c500;
        C600 = c600;
        C700 = c700;
        C800 = c800;
        C900 = c900;
        C950 = c950;
        Name = name;
        All.Add(this);
    }

    public List<string> Expand() => new() { C50, C100, C200, C300, C400, C500, C600, C700, C800, C900, C950 };
}


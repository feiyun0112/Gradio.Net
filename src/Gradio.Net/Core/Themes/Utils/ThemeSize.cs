
namespace Gradio.Net.Core.Themes.Utils;

public class ThemeSize
{
    public static List<ThemeSize> All { get; } = new();

    public string Xxs { get; }
    public string Xs { get; }
    public string Sm { get; }
    public string Md { get; }
    public string Lg { get; }
    public string Xl { get; }
    public string Xxl { get; }
    public string? Name { get; }

    public ThemeSize(string xxs, string xs, string sm, string md, string lg, string xl, string xxl, string? name = null)
    {
        Xxs = xxs;
        Xs = xs;
        Sm = sm;
        Md = md;
        Lg = lg;
        Xl = xl;
        Xxl = xxl;
        Name = name;
        All.Add(this);
    }

    public List<string> Expand() => new() { Xxs, Xs, Sm, Md, Lg, Xl, Xxl };
}


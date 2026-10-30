namespace Gradio.Net;
/// <summary>
/// One colour ramp: the eleven shades a theme's hue is built from.
/// </summary>
/// <remarks>
/// <para>
/// A theme names three hues — primary, secondary and neutral — and each contributes eleven
/// variables (<c>primary_50</c> through <c>primary_950</c>) that the rest of the variables
/// refer to. The values are CSS colours written as hex, which is the Python version's choice and is
/// what makes a theme's output stable across browsers.
/// </para>
/// <para>
/// The presets live in <see cref="Colors"/>. A caller may also build a ramp of its own and pass
/// it where a hue is expected; it is then reachable by that object rather than by name, because
/// the name lookup answers from the presets (see <see cref="Colors.Find"/>).
/// </para>
/// </remarks>
public sealed class Color
{
    /// <summary>Creates a ramp.</summary>
    /// <param name="name">The name a theme's shortcut lookup answers to.</param>
    /// <param name="c50">The 50 shade.</param>
    /// <param name="c100">The 100 shade.</param>
    /// <param name="c200">The 200 shade.</param>
    /// <param name="c300">The 300 shade.</param>
    /// <param name="c400">The 400 shade.</param>
    /// <param name="c500">The 500 shade.</param>
    /// <param name="c600">The 600 shade.</param>
    /// <param name="c700">The 700 shade.</param>
    /// <param name="c800">The 800 shade.</param>
    /// <param name="c900">The 900 shade.</param>
    /// <param name="c950">The 950 shade.</param>
    public Color(
        string name,
        string c50,
        string c100,
        string c200,
        string c300,
        string c400,
        string c500,
        string c600,
        string c700,
        string c800,
        string c900,
        string c950)
    {
        Name = name;
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
    }

    /// <summary>The name the ramp is known by.</summary>
    public string Name { get; }

    /// <summary>The 50 shade.</summary>
    public string C50 { get; }

    /// <summary>The 100 shade.</summary>
    public string C100 { get; }

    /// <summary>The 200 shade.</summary>
    public string C200 { get; }

    /// <summary>The 300 shade.</summary>
    public string C300 { get; }

    /// <summary>The 400 shade.</summary>
    public string C400 { get; }

    /// <summary>The 500 shade.</summary>
    public string C500 { get; }

    /// <summary>The 600 shade.</summary>
    public string C600 { get; }

    /// <summary>The 700 shade.</summary>
    public string C700 { get; }

    /// <summary>The 800 shade.</summary>
    public string C800 { get; }

    /// <summary>The 900 shade.</summary>
    public string C900 { get; }

    /// <summary>The 950 shade.</summary>
    public string C950 { get; }

    /// <summary>The eleven shades, lightest first.</summary> <returns>The shades in the order the
    /// Python version's <c>expand()</c> returns them.</returns>
    public IReadOnlyList<string> Expand() =>
    [
        C50,
        C100,
        C200,
        C300,
        C400,
        C500,
        C600,
        C700,
        C800,
        C900,
        C950,
    ];
}

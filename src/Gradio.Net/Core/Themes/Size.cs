namespace Gradio.Net;
/// <summary>
/// One size ramp: the seven steps a theme's text, spacing or corner radii are built from.
/// </summary>
/// <remarks>
/// A theme names three ramps — one for text, one for spacing, one for radii — and each contributes
/// seven variables (<c>text_xxs</c> through <c>text_xxl</c>, and likewise for the other two). The
/// presets live in <see cref="Sizes"/>; a caller may build a ramp of its own and pass it where a
/// size is expected.
/// </remarks>
public sealed class Size
{
    /// <summary>Creates a ramp.</summary>
    /// <param name="name">The name a theme's shortcut lookup answers to, prefix included.</param>
    /// <param name="xxs">The xxs step.</param>
    /// <param name="xs">The xs step.</param>
    /// <param name="sm">The sm step.</param>
    /// <param name="md">The md step.</param>
    /// <param name="lg">The lg step.</param>
    /// <param name="xl">The xl step.</param>
    /// <param name="xxl">The xxl step.</param>
    public Size(
        string name,
        string xxs,
        string xs,
        string sm,
        string md,
        string lg,
        string xl,
        string xxl)
    {
        Name = name;
        Xxs = xxs;
        Xs = xs;
        Sm = sm;
        Md = md;
        Lg = lg;
        Xl = xl;
        Xxl = xxl;
    }

    /// <summary>The name the ramp is known by.</summary>
    public string Name { get; }

    /// <summary>The xxs step.</summary>
    public string Xxs { get; }

    /// <summary>The xs step.</summary>
    public string Xs { get; }

    /// <summary>The sm step.</summary>
    public string Sm { get; }

    /// <summary>The md step.</summary>
    public string Md { get; }

    /// <summary>The lg step.</summary>
    public string Lg { get; }

    /// <summary>The xl step.</summary>
    public string Xl { get; }

    /// <summary>The xxl step.</summary>
    public string Xxl { get; }

    /// <summary>The seven steps, smallest first.</summary>
    /// <returns>The steps in the order the Python version's <c>expand()</c> returns them.</returns>
    public IReadOnlyList<string> Expand() =>
    [
        Xxs,
        Xs,
        Sm,
        Md,
        Lg,
        Xl,
        Xxl,
    ];
}

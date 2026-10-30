namespace Gradio.Net;
/// <summary>The size ramps the Python version ships: five for radii, three for spacing, three for
/// text.</summary>
/// <remarks>
/// <para>
/// A theme's constructor takes a size as either one of these or a shortcut name — <c>"md"</c>
/// for <c>Sizes.TextMd</c>, <c>"lg"</c> for <c>Sizes.RadiusLg</c> — because the parameter it is
/// passed to says which prefix the name is completed with.
/// </para>
/// <para>
/// <c>RadiusNone</c> is what a theme uses to square every corner, and <c>RadiusXxl</c> is the
/// Python version's name for the ramp whose smallest step is already 6px.
/// </para>
/// </remarks>
public static class Sizes
{
    /// <summary>The <c>radius_none</c> ramp.</summary>
    public static Size RadiusNone { get; } = new(
        "radius_none",
        "0px",
        "0px",
        "0px",
        "0px",
        "0px",
        "0px",
        "0px");

    /// <summary>The <c>radius_sm</c> ramp.</summary>
    public static Size RadiusSm { get; } = new(
        "radius_sm",
        "1px",
        "1px",
        "2px",
        "4px",
        "6px",
        "8px",
        "12px");

    /// <summary>The <c>radius_md</c> ramp.</summary>
    public static Size RadiusMd { get; } = new(
        "radius_md",
        "1px",
        "2px",
        "4px",
        "6px",
        "8px",
        "12px",
        "22px");

    /// <summary>The <c>radius_lg</c> ramp.</summary>
    public static Size RadiusLg { get; } = new(
        "radius_lg",
        "2px",
        "4px",
        "6px",
        "8px",
        "12px",
        "16px",
        "24px");

    /// <summary>The <c>radius_xxl</c> ramp.</summary>
    public static Size RadiusXxl { get; } = new(
        "radius_xxl",
        "6px",
        "8px",
        "10px",
        "20px",
        "24px",
        "28px",
        "32px");

    /// <summary>The <c>spacing_sm</c> ramp.</summary>
    public static Size SpacingSm { get; } = new(
        "spacing_sm",
        "1px",
        "1px",
        "2px",
        "4px",
        "6px",
        "9px",
        "12px");

    /// <summary>The <c>spacing_md</c> ramp.</summary>
    public static Size SpacingMd { get; } = new(
        "spacing_md",
        "1px",
        "2px",
        "4px",
        "6px",
        "8px",
        "10px",
        "16px");

    /// <summary>The <c>spacing_lg</c> ramp.</summary>
    public static Size SpacingLg { get; } = new(
        "spacing_lg",
        "2px",
        "4px",
        "6px",
        "8px",
        "10px",
        "14px",
        "28px");

    /// <summary>The <c>text_sm</c> ramp.</summary>
    public static Size TextSm { get; } = new(
        "text_sm",
        "8px",
        "9px",
        "11px",
        "13px",
        "16px",
        "20px",
        "24px");

    /// <summary>The <c>text_md</c> ramp.</summary>
    public static Size TextMd { get; } = new(
        "text_md",
        "9px",
        "10px",
        "12px",
        "14px",
        "16px",
        "22px",
        "26px");

    /// <summary>The <c>text_lg</c> ramp.</summary>
    public static Size TextLg { get; } = new(
        "text_lg",
        "10px",
        "12px",
        "14px",
        "16px",
        "20px",
        "24px",
        "28px");

    /// <summary>Every ramp, in the order the Python version declares them.</summary>
    /// <remarks>This is the set a theme's shortcut lookup answers from.</remarks>
    public static IReadOnlyList<Size> All { get; } =
    [
        RadiusNone,
        RadiusSm,
        RadiusMd,
        RadiusLg,
        RadiusXxl,
        SpacingSm,
        SpacingMd,
        SpacingLg,
        TextSm,
        TextMd,
        TextLg,
    ];

    /// <summary>Finds a ramp by its full name.</summary>
    /// <param name="name">The name with its prefix — <c>text_md</c>, not <c>md</c>.</param>
    /// <returns>The ramp, or <see langword="null"/> when no preset has that name.</returns>
    public static Size? Find(string name) =>
        All.FirstOrDefault(ramp => string.Equals(ramp.Name, name, StringComparison.Ordinal));
}

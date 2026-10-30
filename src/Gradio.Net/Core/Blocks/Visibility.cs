using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// Whether a block is drawn: <c>true</c>, <c>false</c>, or <c>hidden</c>.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>visible</c> parameter is typed <c>bool | Literal["hidden"]</c> and carries
/// whichever one the application gave it — the three values are written to the config as
/// <c>true</c>, <c>false</c> and <c>"hidden"</c>, and the frontend tells the last one apart from
/// <c>false</c> (a hidden block keeps its place in the layout, an invisible one gives it up). A
/// second implementation that only had a boolean could not describe an application the Python version
/// describes, which is why this is a type rather than a flag.
/// </para>
/// <para>
/// It converts from <see langword="bool"/>, so <c>Visible = false</c> reads as it always did; the
/// third value is spelled <see cref="Hidden"/>.
/// </para>
/// </remarks>
public readonly struct Visibility : IEquatable<Visibility>, IConfigValue
{
    private readonly byte _state;

    private Visibility(byte state) => _state = state;

    /// <summary>The block is drawn.</summary>
    public static Visibility Shown { get; } = new(0);

    /// <summary>The block is not drawn and takes up no space.</summary>
    public static Visibility Collapsed { get; } = new(1);

    /// <summary>The block is not drawn but keeps the space it would have taken.</summary>
    /// <remarks>The Python version's <c>"hidden"</c>, which is what a layout hides with.</remarks>
    public static Visibility Hidden { get; } = new(2);

    /// <summary>Whether the block is drawn at all.</summary>
    public bool IsShown => _state == 0;

    /// <summary>Whether the block keeps its space in the layout while it is not drawn.</summary>
    /// <remarks>
    /// Named for what it does rather than for the state it is in: <see cref="Collapsed"/> is not
    /// drawn either, and a reader asking "is it hidden" would get the wrong answer for it.
    /// </remarks>
    public bool KeepsSpace => _state == 2;

    /// <summary>Reads a boolean as one of the two plain states.</summary>
    /// <param name="visible">Whether the block is drawn.</param>
    public static implicit operator Visibility(bool visible) => visible ? Shown : Collapsed;

    /// <summary>Reads the state as "is it drawn", which is false for both kinds of not drawn.</summary>
    /// <param name="visibility">The state.</param>
    public static implicit operator bool(Visibility visibility) => visibility.IsShown;

    /// <summary>The state as the config spells it: a boolean, or the string <c>hidden</c>.</summary>
    /// <returns><c>true</c>, <c>false</c> or <c>"hidden"</c>.</returns>
    public JsonNode ToConfigValue() => _state switch
    {
        0 => JsonValue.Create(true)!,
        1 => JsonValue.Create(false)!,
        _ => JsonValue.Create("hidden")!,
    };

    /// <inheritdoc />
    public bool Equals(Visibility other) => _state == other._state;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Visibility other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => _state;

    /// <summary>Whether two states are the same.</summary>
    /// <param name="left">One state.</param>
    /// <param name="right">The other.</param>
    public static bool operator ==(Visibility left, Visibility right) => left.Equals(right);

    /// <summary>Whether two states differ.</summary>
    /// <param name="left">One state.</param>
    /// <param name="right">The other.</param>
    public static bool operator !=(Visibility left, Visibility right) => !left.Equals(right);

    /// <inheritdoc />
    public override string ToString() => _state switch
    {
        0 => "shown",
        1 => "collapsed",
        _ => "hidden",
    };
}

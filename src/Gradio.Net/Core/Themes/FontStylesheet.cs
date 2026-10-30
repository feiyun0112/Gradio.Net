namespace Gradio.Net;
/// <summary>What a font family needs the theme to carry for it to render.</summary>
/// <param name="Url">
/// A stylesheet the page must load, for a font served from somewhere else; <see langword="null"/>
/// when the theme carries the rules itself.
/// </param>
/// <param name="Css">
/// The rules the theme appends to its stylesheet — <c>@font-face</c> declarations for a font that
/// travels with the frontend; <see langword="null"/> when there are none.
/// </param>
/// <remarks>
/// The Python version's <c>stylesheet()</c> returns a dictionary with both keys and one of them
/// filled; a record says the same thing without a shape that can be half-filled by accident.
/// </remarks>
public readonly record struct FontStylesheet(string? Url, string? Css);

namespace Gradio.Net;
/// <summary>
/// One variable a theme carries before an application changes it: its name, the variable it
/// inherits from when it is left alone, and the value it takes when there is nothing to inherit.
/// </summary>
/// <param name="Name">The variable's Python name, with underscores as the Python
/// version spells it.</param>
/// <param name="Fallback">
/// The variable the Python version reads the inherited value from. Usually <paramref name="Name"/>
/// itself; four of the Python version's defaults read a differently-spelled name, and that is
/// reproduced rather than corrected (see <see cref="ThemeDefaults"/>).
/// </param>
/// <param name="Value">
/// The value the variable takes when nothing is inherited, or <see langword="null"/> for a dark
/// variable that has no dark value of its own and so inherits the light one.
/// </param>
/// <remarks>
/// It is a struct because there are 267 of them in a static table and nothing mutates one: the
/// table describes the Python version, and a theme is what changes.
/// </remarks>
internal readonly record struct DefaultVariable(string Name, string Fallback, string? Value);

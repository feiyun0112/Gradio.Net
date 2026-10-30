namespace Gradio.Net;
/// <summary>
/// A function that computes a component's value when a page opens, and the name its endpoint is
/// published under.
/// </summary>
/// <param name="Fn">The function. Its result is written to the component it belongs to.</param>
/// <param name="ApiName">
/// The name the endpoint is published under, or <see langword="null"/> for an unnamed one.
/// </param>
/// <remarks>
/// <para>
/// The Python version's callable <c>value</c>: a component whose value is a function has it called
/// on every browser session, and what it answers is what that session starts with. A C# property
/// cannot be both a value and a function — <c>Value</c> is typed per component, and a <c>Func</c>
/// is not a <see cref="string"/> — so the function travels beside the value rather than in its
/// place, which is the same decision <c>ServerFunctions</c> makes for a canvas.
/// </para>
/// <para>
/// The name is part of the same record because the Python version derives it from the function's
/// own name, which a delegate does not reliably have: a lambda's is generated. An application that
/// wants the endpoint named writes the name; one that does not leaves it out and the binding is
/// unnamed, as any other unnamed binding is.
/// </para>
/// </remarks>
public sealed record LoadFunction(Delegate Fn, string? ApiName = null);

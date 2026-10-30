namespace Gradio.Net;
/// <summary>
/// The compatibility layer: the Python version's names, spelled the way C# spells them.
/// </summary>
/// <remarks>
/// <para>
/// A file ported from Python should need three kinds of edit and no more. The Python version's
/// names are copied — the factory names, the parameter names, their order and their defaults — so
/// <c>gr.Textbox(value="x", label="y")</c> becomes <c>gr.Textbox(value: "x", label: "y")</c> and
/// nothing else changes. Member names keep their word and change only their case, so
/// <c>btn.click(...)</c> becomes <c>btn.Click(...)</c>. What is left is the handful of places C#
/// forces a different shape, and those are listed in the ADR rather than discovered one at a time.
/// </para>
/// <para>
/// The factories return the core types themselves. <see cref="Textbox"/> here is
/// <see cref="Textbox"/>, not a wrapper around it, because the line a port writes most
/// often is <c>outputs=[out]</c>: a wrapper would have to be unwrapped at every one of them. The
/// cost is that a property is still <c>tb.Value</c> rather than <c>tb.value</c>, which is the one
/// place this layer's naming stops at the member boundary — and the ADR says so rather than leaving
/// it to be found.
/// </para>
/// <para>
/// Nothing in this layer decides anything. A factory builds a block, sets what it was given, and
/// puts the block on the page being written — because the Python version's factory is a line on the
/// page rather than a value — and an extension method forwards to the core. A second place where
/// behaviour could live is a second behaviour, and the whole point of the layer is that a ported
/// application runs the same code as a hand-written one.
/// </para>
/// <para>
/// "Which page is being written" is the one question a static factory cannot answer from its
/// arguments, so the layer answers it in <see cref="Ambient"/>: a per-flow stack of the
/// applications this flow has opened, which is what makes <c>gr.Textbox(...)</c> a line of the page
/// instead of <c>demo.Add(gr.Textbox(...))</c>. It is the layer's only state, it is not the core's,
/// and it is the narrow kind of state the layering leaves room for.
/// </para>
/// <para>
/// The factories are not written here. Each one is generated at build time from the recorded the
/// Python version signature, into a file per block, and this directory holds only what a generator
/// cannot produce: the entry point's own remarks, the values a union-typed parameter is read
/// through, and the methods the Python version puts on an application rather than at its top level.
/// A factory written by hand would be a thousand parameters copied by eye, and the way that copy
/// fails is by being almost right.
/// </para>
/// <para>
/// The Python version's top-level names are members here, including the spellings it exports twice
/// — <c>Textbox</c> and <c>Text</c>, <c>JSON</c> and <c>Json</c>, <c>Dataframe</c> and
/// <c>DataFrame</c> — because a ported file writes whichever spelling it wrote in Python. That is
/// also why two members of this class differ only in case, which is the one thing a C# naming rule
/// objects to and the one thing this layer must do.
///
/// The exceptions are written down rather than discovered: <c>GrSurfaceOwed</c> lists the names
/// this layer does not have yet, each with what it waits on, and <c>GrSurfaceOwed.Never</c> the ones
/// it will not have, each with the reason. A name that is missing from both tables fails a test.
/// </para>
/// <para>
/// The type is spelled <c>gr</c> rather than <c>Gr</c>, which is the one place this layer's own
/// naming is Python's rather than C#'s: the Python version's entry point is the module <c>gr</c>,
/// and a port that has to change its case is a port that has to be read carefully. It is also what
/// keeps <c>gr.Textbox</c> unambiguous — a type named <c>Gr</c> beside a namespace named
/// <c>Gradio.Net.Gr</c> would be shadowed by that namespace in any file that imports
/// <c>Gradio.Net</c>, and the call would stop compiling for a reason nobody could see.
/// </para>
/// </remarks>
public static partial class gr
{
}

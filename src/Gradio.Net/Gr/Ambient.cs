using System.Collections.Immutable;
namespace Gradio.Net;
/// <summary>
/// The render the compatibility layer is writing into, for the flow that is writing it.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's factories place the block they build in the application that is currently
/// open: <c>gr.Textbox(...)</c> in Python is not a value, it is a line on the page. C# cannot do
/// that with a static method that only knows its arguments — the factory has to be able to ask
/// "which application am I being written into?" — so the layer keeps the answer here and the
/// factories read it. That is what makes <c>gr.Textbox(...)</c> a line on the page instead of
/// <c>demo.Add(gr.Textbox(...))</c>, and it is the difference between a port that reads like the
/// Python and one that reads like a translation of it.
/// </para>
/// <para>
/// <strong>This is the one piece of state in the framework that is not owned by an object.</strong>
/// It is deliberately small, and deliberately not the core's: <see cref="Blocks"/> still carries its
/// own <see cref="IRenderContextAccessor"/>, and two applications in one process still cannot see
/// each other's blocks. What lives here is a stack of accessors, one entry per
/// <c>gr.Blocks()</c> this flow has opened, and the value is an
/// <see cref="System.Threading.AsyncLocal{T}"/> — so it is per flow, not per process: two requests,
/// two tests and two threads never see each other's stack, and the entry a flow pushes is invisible
/// to every other flow.
/// </para>
/// <para>
/// The stack is searched from the top for the first accessor that is <em>currently rendering</em>,
/// rather than the top entry being taken at face value. That is what makes the state safe to leave
/// behind: a <c>using</c> block that ends does not have to tell this class anything, because the
/// render it opened is what made its accessor live, and the accessor reports that it is over. An
/// entry for an application that is between renders is simply skipped, and an application that is
/// rendered a second time (dynamic rendering, ticket 12) is found again because its accessor is
/// still in the stack.
/// </para>
/// <para>
/// The alternative — passing the application to every factory — is what this replaces: it would put
/// the application into every one of the twelve hundred parameters the Python version declares, and
/// a port would have to write it on every line, which is the shape this class exists to remove.
/// </para>
/// </remarks>
internal static class Ambient
{
    private static readonly AsyncLocal<ImmutableStack<IRenderContextAccessor>?> _open = new();

    /// <summary>The render this flow is writing into, or <see langword="null"/> when there is none.</summary>
    /// <remarks>
    /// "Writing into" and not "belongs to": an application that exists but is not being rendered is
    /// not a place a block can be written, which is why the answer is a
    /// <see cref="RenderContext"/> rather than an application.
    /// </remarks>
    internal static RenderContext? Current
    {
        get
        {
            for (var open = _open.Value; open is { IsEmpty: false }; open = open.Pop())
            {
                if (open.Peek().Current is { } context)
                {
                    return context;
                }
            }

            return null;
        }
    }

    /// <summary>Makes an application the one this flow writes into, until its render ends.</summary>
    /// <param name="accessor">The application's own render accessor.</param>
    /// <remarks>
    /// Pushed rather than set, so that an application opened inside another one's render is the one
    /// written into while it lasts and the outer one is written into again afterwards — which is
    /// what nesting means in Python too.
    /// </remarks>
    internal static void Open(IRenderContextAccessor accessor) =>
        _open.Value = (_open.Value ?? ImmutableStack<IRenderContextAccessor>.Empty).Push(accessor);

    /// <summary>Puts a block on the page being written, and gives it back.</summary>
    /// <typeparam name="TBlock">The block's type, so that a factory can return the concrete type.</typeparam>
    /// <param name="block">The block to place.</param>
    /// <returns><paramref name="block"/>, placed when there is a page to place it on.</returns>
    /// <remarks>
    /// A block built outside a render is built and not placed, which is what makes a factory usable
    /// on its own — a test that wants a block, a helper that builds one to be placed later. It is
    /// also what keeps a factory from throwing when it is called outside the <c>using</c> it was
    /// meant for: the block is simply not on a page yet.
    /// </remarks>
    internal static TBlock Place<TBlock>(TBlock block)
        where TBlock : Block
    {
        if (Current is { } context)
        {
            context.Root.Add(block);
        }

        return block;
    }

    /// <summary>Puts a layout on the page being written, and enters it as the place to write next.</summary>
    /// <typeparam name="TContext">The layout's type, so that a factory can return the concrete type.</typeparam>
    /// <param name="container">The layout to place and enter.</param>
    /// <returns><paramref name="container"/>, which closes the scope when it is disposed.</returns>
    /// <remarks>
    /// The two halves of the Python version's <c>with gr.Row():</c> — the block is placed where the
    /// line is, and the lines under it belong to it. The scope is closed by disposing the layout,
    /// which is why <see cref="BlockContext"/> is <see cref="IDisposable"/>; a layout built outside
    /// a render has no scope to enter and nothing to close.
    /// </remarks>
    internal static TContext Enter<TContext>(TContext container)
        where TContext : BlockContext
    {
        if (Current is not { } context)
        {
            return container;
        }

        context.Root.Add(container);
        context.Root.Enter(container);

        return container;
    }
}

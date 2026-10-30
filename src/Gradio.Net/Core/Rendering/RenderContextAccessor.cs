namespace Gradio.Net;
/// <summary>
/// The default <see cref="IRenderContextAccessor"/>: one <see cref="System.Threading.AsyncLocal{T}"/>
/// holding the ambient <see cref="RenderContext"/> of each asynchronous flow.
/// </summary>
/// <remarks>
/// <para>
/// The <see cref="System.Threading.AsyncLocal{T}"/> is an instance field, not a static one, so the
/// framework keeps no process-wide state at all: a test can build two accessors and be sure they
/// cannot see each other.
/// </para>
/// <para>
/// The value is written on entry and restored on exit, so leaving a scope never leaves a context
/// behind for the code that runs after it.
/// </para>
/// </remarks>
public sealed class RenderContextAccessor : IRenderContextAccessor
{
    private readonly AsyncLocal<RenderContext?> _current = new();

    /// <inheritdoc />
    public RenderContext? Current => _current.Value;

    /// <inheritdoc />
    public IDisposable Enter(RenderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var previous = _current.Value;
        _current.Value = context;

        return new RenderScope(this, previous);
    }

    private sealed class RenderScope : IDisposable
    {
        private readonly RenderContextAccessor _owner;
        private readonly RenderContext? _previous;
        private bool _disposed;

        internal RenderScope(RenderContextAccessor owner, RenderContext? previous)
        {
            _owner = owner;
            _previous = previous;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _owner._current.Value = _previous;
        }
    }
}

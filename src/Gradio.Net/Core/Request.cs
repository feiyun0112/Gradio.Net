namespace Gradio.Net;
/// <summary>
/// Who a handler is running for: the caller's name, and the session they are looking at.
/// </summary>
/// <param name="UserName">
/// Who signed in, or <see langword="null"/> for a run that belongs to nobody — an application that
/// asks no one to sign in, or an application that does and has not been answered.
/// </param>
/// <param name="SessionHash">
/// The session the run belongs to, as the caller named it, or <see langword="null"/> when the
/// caller named none.
/// </param>
/// <remarks>
/// <para>
/// The Python version's <c>gr.Request</c>, as much of it as a run can carry. A handler asks for it
/// by declaring a parameter of this type, which is how a .NET signature says what a Python one says
/// with <c>request: gr.Request</c> — the difference being that the type is enough to recognise it,
/// so there is no default value to write. It is one of the structural deviations a port accepts: an
/// argument the Python version injects by annotation is an explicit parameter here.
/// </para>
/// <para>
/// The two members are what a run can still know about the request that started it. The Python
/// version's <c>Request</c> also carries <c>headers</c>, <c>client</c>, <c>query_params</c> and
/// <c>path_params</c>, because it keeps the request object itself; here a queued run outlives the
/// request that admitted it — the browser can close the stream and the run still finishes — so what
/// travels with a run is what it needs: who is calling, and which session they are looking at. The
/// rest is a gap rather than a decision, and the scope ledger records it.
/// </para>
/// <para>
/// A run nobody asked for — an unload handler, an example being cached — carries an instance with
/// both members empty rather than nothing, because the Python version hands such a handler a
/// <c>Request</c> too. A handler can therefore read <see cref="UserName"/> without asking whether
/// it has one.
/// </para>
/// </remarks>
public sealed record Request(string? UserName = null, string? SessionHash = null);


namespace Gradio.Net.Tests.Server;

/// <summary>
/// A renderer that cannot start, so that the server's half of a failed start can be seen without a
/// runtime that fails for reasons of its own.
/// </summary>
/// <remarks>
/// <para>
/// A start that fails is part of the contract, not an accident of Node: a sidecar that is not up
/// yet, a container whose runtime is missing, a port already taken. What the package promises is
/// that the failure reaches the caller with its reason intact and that the application does not stay
/// listening on an address nobody was told about. This is the smallest implementation
/// that can ask those two questions.
/// </para>
/// <para>
/// It records the request before refusing it, so a test can see that the server had formed one: a
/// renderer that is asked nothing and fails anyway would pass a test written only around the
/// failure.
/// </para>
/// </remarks>
internal sealed class FailingSsrRuntimeFactory : ISsrRuntimeFactory
{
    /// <summary>The reason this renderer gives, which the caller should be able to read back.</summary>
    internal const string Reason = "the renderer was not reachable at the address it was given";

    /// <summary>What the server asked for, once it has asked.</summary>
    internal SsrRuntimeRequest? Request { get; private set; }

    /// <inheritdoc />
    /// <remarks>
    /// The failure is a faulted task rather than a synchronous throw, because that is how the
    /// built-in factory fails: it awaits a start that went wrong. A fake that threw before
    /// returning would exercise a different path in the caller than the one an operator meets.
    /// </remarks>
    public Task<ISsrRuntime> StartAsync(SsrRuntimeRequest request, CancellationToken cancellationToken)
    {
        Request = request;

        return Task.FromException<ISsrRuntime>(new InvalidOperationException(Reason));
    }
}

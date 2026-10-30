using System.Diagnostics;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// Watches this process's outbound HTTP, so that "nothing leaves" can be asserted rather than
/// believed.
/// </summary>
/// <remarks>
/// <para>
/// Every request <see cref="HttpClient"/> makes is announced through the <c>HttpHandlerDiagnosticListener</c>
/// that the BCL writes to, so subscribing to it is the closest thing to a wire tap a test can have
/// without a firewall rule: it sees a request before any connection is opened, whichever library
/// asked for it.
/// </para>
/// <para>
/// The test's own requests to the host are loopback and are expected; anything else is the failure.
/// <see cref="Count"/> is what proves the tap is connected at all — a subscription that silently saw
/// nothing would make "no outbound request" true for the wrong reason.
/// </para>
/// </remarks>
internal sealed class OutboundWatch : IDisposable
{
    /// <summary>The listener the BCL's HTTP stack writes to.</summary>
    private const string ListenerName = "HttpHandlerDiagnosticListener";

    /// <summary>The event announced before a request is sent.</summary>
    private const string RequestStart = "System.Net.Http.HttpRequestOut.Start";

    private readonly object _gate = new();
    private readonly List<Uri?> _addresses = [];
    private readonly List<string> _listenerNames = [];
    private readonly List<string> _eventNames = [];
    private readonly List<IDisposable> _subscriptions = [];
    private readonly IDisposable _listeners;

    /// <summary>Starts watching.</summary>
    internal OutboundWatch()
    {
        _listeners = DiagnosticListener.AllListeners.Subscribe(new ListenerObserver(this));
    }

    /// <summary>How many outbound requests were announced.</summary>
    internal int Count
    {
        get
        {
            lock (_gate)
            {
                return _addresses.Count;
            }
        }
    }

    /// <summary>The names of the diagnostic listeners this watch was told about.</summary>
    internal IReadOnlyList<string> Listeners
    {
        get
        {
            lock (_gate)
            {
                return [.. _listenerNames];
            }
        }
    }

    /// <summary>The names of the events this watch was handed.</summary>
    internal IReadOnlyList<string> Events
    {
        get
        {
            lock (_gate)
            {
                return [.. _eventNames];
            }
        }
    }

    /// <summary>The requests that were not to this machine, or whose address could not be read.</summary>
    internal IReadOnlyList<Uri?> External
    {
        get
        {
            lock (_gate)
            {
                return [.. _addresses.Where(address => address is not { IsLoopback: true })];
            }
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _listeners.Dispose();

        foreach (var subscription in _subscriptions)
        {
            subscription.Dispose();
        }
    }

    /// <summary>Records the name of a listener the watch was told about.</summary>
    /// <param name="name">The listener's name.</param>
    private void Note(string name)
    {
        lock (_gate)
        {
            _listenerNames.Add(name);
        }
    }

    /// <summary>Records one event a listener announced.</summary>
    /// <param name="name">The event's name.</param>
    /// <param name="payload">What the listener handed over with it.</param>
    private void Record(string name, object? payload)
    {
        var started = string.Equals(name, RequestStart, StringComparison.Ordinal);

        lock (_gate)
        {
            _eventNames.Add(name);

            if (started)
            {
                _addresses.Add(AddressOf(payload));
            }
        }
    }

    /// <summary>The address a request-start payload names.</summary>
    /// <param name="payload">What the listener handed over.</param>
    /// <returns>The address, or <see langword="null"/> when the payload does not carry one.</returns>
    /// <remarks>
    /// The payload is either the message itself or a record holding it, depending on the runtime, so
    /// both shapes are read. A payload that carries neither is recorded as unknown rather than
    /// skipped: an announcement the test cannot read is not evidence that nothing was sent.
    /// </remarks>
    private static Uri? AddressOf(object? payload) => payload switch
    {
        HttpRequestMessage request => request.RequestUri,
        null => null,
        _ => payload.GetType().GetProperty("Request")?.GetValue(payload) is HttpRequestMessage inner
            ? inner.RequestUri
            : null,
    };

    /// <summary>Subscribes to a listener that announces HTTP requests.</summary>
    /// <param name="listener">The listener.</param>
    /// <remarks>
    /// Every event the listener announces, rather than the one this watch reads: the name-filtered
    /// overload was observed to leave the HTTP stack's own enabled-check answering "nobody is
    /// listening", so the filter is applied on this side instead — in <see cref="Record"/>.
    /// </remarks>
    private void Subscribe(DiagnosticListener listener)
    {
        var subscription = listener.Subscribe(new EventObserver(this));

        lock (_gate)
        {
            _subscriptions.Add(subscription);
        }
    }

    /// <summary>Finds the HTTP listener among everything the BCL announces.</summary>
    private sealed class ListenerObserver : IObserver<DiagnosticListener>
    {
        private readonly OutboundWatch _watch;

        internal ListenerObserver(OutboundWatch watch) => _watch = watch;

        public void OnNext(DiagnosticListener value)
        {
            _watch.Note(value.Name);

            if (string.Equals(value.Name, ListenerName, StringComparison.Ordinal))
            {
                _watch.Subscribe(value);
            }
        }

        public void OnCompleted()
        {
            // Nothing to finish: the subscription lives as long as the watch.
        }

        public void OnError(Exception error)
        {
            // A listener that fails is not a request; the watch keeps what it has.
        }
    }

    /// <summary>Records the events of one listener.</summary>
    private sealed class EventObserver : IObserver<KeyValuePair<string, object?>>
    {
        private readonly OutboundWatch _watch;

        internal EventObserver(OutboundWatch watch) => _watch = watch;

        public void OnNext(KeyValuePair<string, object?> value) => _watch.Record(value.Key, value.Value);

        public void OnCompleted()
        {
            // Nothing to finish.
        }

        public void OnError(Exception error)
        {
            // Nothing to do: the event that failed is one the test did not ask about.
        }
    }
}

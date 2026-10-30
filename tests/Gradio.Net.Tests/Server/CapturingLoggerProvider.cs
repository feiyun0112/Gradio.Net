using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// Collects what the framework logs, so that a test can assert on it without reading a console.
/// </summary>
internal sealed class CapturingLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentQueue<CapturedLogEntry> _entries = new();

    internal IReadOnlyCollection<CapturedLogEntry> Entries => _entries;

    public ILogger CreateLogger(string categoryName) => new CapturingLogger(categoryName, _entries);

    public void Dispose()
    {
        // Nothing to release: the entries are the test's to read after the host is gone.
    }

    private sealed class CapturingLogger : ILogger
    {
        private readonly string _category;
        private readonly ConcurrentQueue<CapturedLogEntry> _entries;

        internal CapturingLogger(string category, ConcurrentQueue<CapturedLogEntry> entries)
        {
            _category = category;
            _entries = entries;
        }

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            ArgumentNullException.ThrowIfNull(formatter);

            _entries.Enqueue(new CapturedLogEntry(_category, logLevel, eventId, formatter(state, exception), exception)
            {
                Properties = PropertiesOf(state),
            });
        }

        /// <summary>The named values a log entry carries, as the logging abstraction hands them over.</summary>
        /// <param name="state">What was logged.</param>
        /// <returns>The values, or an empty set when the entry carries none.</returns>
        /// <remarks>
        /// A structured entry is a list of key/value pairs rather than a formatted string — that is
        /// what lets a host send the fields somewhere other than a console — so this is how a test
        /// can see the difference between "the message contains the address" and "the address is a
        /// field".
        /// </remarks>
        private static Dictionary<string, object?> PropertiesOf<TState>(TState state) =>
            state is IEnumerable<KeyValuePair<string, object?>> pairs
                ? pairs.ToDictionary(pair => pair.Key, pair => pair.Value)
                : new Dictionary<string, object?>();
    }
}

/// <summary>One log entry, flattened.</summary>
/// <param name="Category">Who logged it.</param>
/// <param name="Level">How serious it is.</param>
/// <param name="EventId">Which event it is.</param>
/// <param name="Message">What it says.</param>
/// <param name="Exception">What was thrown, when the entry is about a failure.</param>
internal sealed record CapturedLogEntry(
    string Category,
    LogLevel Level,
    EventId EventId,
    string Message,
    Exception? Exception = null)
{
    /// <summary>The named values the entry carried, if it was written structurally.</summary>
    internal IReadOnlyDictionary<string, object?> Properties { get; init; } =
        new Dictionary<string, object?>();
}

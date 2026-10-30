using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Gradio.Net;
/// <summary>
/// Turns a finished run into a record, and hands it to the store.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>record_run</c> and <c>schedule_record_run</c>, as one object. It decides
/// what is worth recording — a public endpoint's final answer, and nothing else — collects the
/// files the run named into assets, and writes the record.
/// </para>
/// <para>
/// The write happens after the answer has been sent, on a task of its own, because a run must not
/// wait for a bucket: the client asked for a result, not for a receipt. Writes are bounded, and a run
/// whose record cannot be started is dropped rather than queued without limit — a history that falls
/// behind is worse than a history with a gap in it, because the gap is a record that was never
/// promised to anybody.
/// </para>
/// </remarks>
internal sealed class RunHistoryRecorder
{
    /// <summary>How many records may be in flight at once.</summary>
    internal const int MaxConcurrentWrites = 8;

    private readonly Blocks _blocks;
    private readonly IRunHistoryStore _store;
    private readonly RunHistoryOptions _options;
    private readonly ILogger<RunHistoryRecorder> _logger;
    private int _inFlight;
    private readonly Action<ILogger, string, Exception?> _logNotRecorded;
    private readonly Action<ILogger, Exception?> _logNotRecordedWithFailure;
    private readonly Action<ILogger, Exception?> _logPoolFull;

    /// <remarks>
    /// Public so that the container can build it: the built-in container only considers public
    /// constructors, and the type stays internal to the assembly.
    /// </remarks>
    public RunHistoryRecorder(
        Blocks blocks,
        IRunHistoryStore store,
        RunHistoryOptions options,
        ILogger<RunHistoryRecorder>? logger = null)
    {
        ArgumentNullException.ThrowIfNull(blocks);
        ArgumentNullException.ThrowIfNull(store);
        ArgumentNullException.ThrowIfNull(options);

        _blocks = blocks;
        _store = store;
        _options = options;
        _logger = logger ?? NullLogger<RunHistoryRecorder>.Instance;

        _logNotRecorded = LoggerMessage.Define<string>(
            LogLevel.Warning,
            new EventId(1, "GradioRunNotRecorded"),
            "Run history: the run was not recorded: {Reason}");

        _logNotRecordedWithFailure = LoggerMessage.Define(
            LogLevel.Warning,
            new EventId(2, "GradioRunNotRecordedWithFailure"),
            "Run history: the run was not recorded.");

        _logPoolFull = LoggerMessage.Define(
            LogLevel.Debug,
            new EventId(3, "GradioRunHistoryPoolFull"),
            "Run history: the write pool is full, so this run was not recorded.");
    }

    /// <summary>Whether a binding's runs are recorded at all.</summary>
    /// <param name="dependency">The binding.</param>
    /// <returns><see langword="true"/> when a run of it would be recorded.</returns>
    /// <remarks>
    /// Three questions, and each of them is the Python version's: the application has to keep a
    /// history, the endpoint has to be public — a private one is a binding the application did not
    /// mean to expose — and a cancel function is not a run of its own but a message about another
    /// one.
    /// </remarks>
    internal bool Records(Dependency dependency)
    {
        ArgumentNullException.ThrowIfNull(dependency);

        return _blocks.RunHistory
            && dependency.ApiVisibility == ApiVisibility.Public
            && !dependency.IsCancelFunction;
    }

    /// <summary>Records a finished run, without making the caller wait for it.</summary>
    /// <param name="dependency">The binding that ran.</param>
    /// <param name="inputs">The values the client sent.</param>
    /// <param name="outputs">The values the run produced.</param>
    /// <param name="startedAt">When the run started, or <see langword="null"/> for now.</param>
    /// <returns>The task that writes the record, or <see langword="null"/> when none was started.</returns>
    /// <remarks>
    /// The task is returned so that a test can wait for the record it is about to look for. A
    /// caller in a run ignores it, which is the point of the method being asynchronous.
    /// </remarks>
    internal Task? Record(
        Dependency dependency,
        JsonNode? inputs,
        JsonNode? outputs,
        string? startedAt = null)
    {
        ArgumentNullException.ThrowIfNull(dependency);

        if (!Records(dependency))
        {
            return null;
        }

        // A count rather than a semaphore: nothing ever waits for a slot, so what is needed is "how
        // many are in flight", and a counter has no lifetime to get wrong at shutdown.
        if (Interlocked.Increment(ref _inFlight) > MaxConcurrentWrites)
        {
            Interlocked.Decrement(ref _inFlight);
            _logPoolFull(_logger, null);

            return null;
        }

        var record = new RunHistoryRecord
        {
            RecordId = RunHistoryKeys.NewRecordId(),
            Endpoint = EndpointOf(dependency),
            StartedAt = startedAt ?? RunHistoryKeys.Now(),
        };

        var assets = new Dictionary<string, RunHistoryAsset>(StringComparer.Ordinal);
        var counter = 0;

        record = record with
        {
            Inputs = Externalize(inputs, assets, ref counter),
            Outputs = Externalize(outputs, assets, ref counter),
        };

        return Task.Run(() => SaveAsync(record, assets));
    }

    /// <summary>Writes one record, and gives its slot back.</summary>
    /// <param name="record">The record.</param>
    /// <param name="assets">The files it carries.</param>
    /// <returns>A task that completes once the write is done or has failed.</returns>
    private async Task SaveAsync(RunHistoryRecord record, IReadOnlyDictionary<string, RunHistoryAsset> assets)
    {
        try
        {
            await _store
                .SaveAsync(_options.Bucket, RunHistoryKeys.AppIdOf(_blocks), record, assets, CancellationToken.None)
                .ConfigureAwait(false);
        }
        catch (RunHistoryException failure)
        {
            _logNotRecorded(_logger, failure.Message, null);
        }
        catch (Exception failure)
        {
            // A history that cannot be written must not fail the run that produced it: the client
            // already has its answer, and there is nobody left to tell.
            _logNotRecordedWithFailure(_logger, failure);
        }
        finally
        {
            Interlocked.Decrement(ref _inFlight);
        }
    }

    /// <summary>The path segment a run is filed under.</summary>
    /// <param name="dependency">The binding.</param>
    /// <returns>The endpoint's name, made safe to use as a directory.</returns>
    /// <remarks>
    /// An endpoint's name is the application's own text — <c>api_name</c> — so it is reduced to a
    /// name a store can file: the Python version's <c>sanitize_segment</c>, which is the same rule
    /// applied here.
    /// </remarks>
    private static string EndpointOf(Dependency dependency)
    {
        var name = dependency.ApiName.TrimStart('/');
        var slug = new string([.. name.Select(character =>
            char.IsAsciiLetterOrDigit(character) || character is '_' or '.' or '-' ? character : '-')])
            .Trim('-', '.');

        if (slug.Length > 80)
        {
            slug = slug[..80].Trim('-', '.');
        }

        return slug.Length == 0 ? $"fn-{dependency.Id}" : slug;
    }

    /// <summary>Replaces the files a value names with the names of the assets that hold them.</summary>
    /// <param name="value">The value.</param>
    /// <param name="assets">Where the files are collected.</param>
    /// <param name="counter">How many assets have been named so far.</param>
    /// <returns>The value, with its files externalized.</returns>
    /// <remarks>
    /// A file is kept only when it is one this application produced: a path outside the
    /// application's upload folder is left as it is rather than copied, because a record is not a
    /// way to read the server's files. The Python version makes the same check and for the same
    /// reason.
    /// </remarks>
    private JsonNode? Externalize(
        JsonNode? value,
        Dictionary<string, RunHistoryAsset> assets,
        ref int counter)
    {
        switch (value)
        {
            case JsonObject map when Component.IsFileObject(map):
                return AssetOf(map, assets, ref counter);

            case JsonObject map:
            {
                var copy = new JsonObject();

                foreach (var (key, child) in map)
                {
                    copy[key] = Externalize(child, assets, ref counter);
                }

                return copy;
            }

            case JsonArray array:
            {
                var copy = new JsonArray();

                foreach (var child in array)
                {
                    copy.Add(Externalize(child, assets, ref counter));
                }

                return copy;
            }

            default:
                return value?.DeepClone();
        }
    }

    /// <summary>One file, collected as an asset and replaced by the name it was given.</summary>
    /// <param name="file">The file object.</param>
    /// <param name="assets">Where the files are collected.</param>
    /// <param name="counter">How many assets have been named so far.</param>
    /// <returns>The node that stands in for the file.</returns>
    private JsonNode AssetOf(JsonObject file, Dictionary<string, RunHistoryAsset> assets, ref int counter)
    {
        var path = JsonValues.Text(file["path"]);

        if (path is not { Length: > 0 } || !MediaCache.IsInFolder(path, _blocks.UploadFolder) || !System.IO.File.Exists(path))
        {
            // A file the application did not produce, or one that is already gone: the record keeps
            // what it was given rather than inventing a copy of something it is not allowed to read.
            return file.DeepClone();
        }

        counter++;

        var name = $"a{counter:000}{ExtensionOf(path)}";

        assets[name] = new RunHistoryAsset
        {
            Name = name,
            Path = path,
            ContentType = RunHistoryContentTypes.Of(name),
        };

        return new JsonObject { ["__asset__"] = name };
    }

    /// <summary>The extension a file's name carries, when it is one a name can carry.</summary>
    /// <param name="path">The file.</param>
    /// <returns>The extension, or <c>.bin</c>.</returns>
    /// <remarks>
    /// The extension is kept as it was written rather than lowered, which is the Python version's
    /// rule: an asset is named after the file it holds, and a name is not the place to tidy a
    /// file's case.
    /// </remarks>
    private static string ExtensionOf(string path)
    {
        var extension = Path.GetExtension(path);

        return extension.Length is > 1 and <= 12 && extension[1..].All(char.IsAsciiLetterOrDigit)
            ? extension
            : ".bin";
    }
}

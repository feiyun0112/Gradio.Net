using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// Reports how far a long-running handler has got, as the progress bar the client draws
/// </summary>
/// <remarks>
/// <para>
/// A handler asks for one by declaring a parameter of this type; the queue supplies it, and a
/// parameter the framework supplies is not one of the binding's inputs. This mirrors the Python
/// version, where a handler declares <c>progress=gr.Progress()</c> and the framework fills it in —
/// the difference being that a .NET method's parameter type is enough to recognise it, so there is
/// no default value to write.
/// </para>
/// <para>
/// Reporting outside a run does nothing rather than failing. The Python version's tracker is a
/// no-op when no run is listening to it, and a handler that is also called directly — from a test,
/// say — should not have to know which way it was called.
/// </para>
/// <para>
/// Progress is advisory and coalesced: the queue keeps the most recent report and sends it on its
/// next tick, so a loop that reports a million times costs one message per tick rather than a
/// million messages.
/// </para>
/// </remarks>
public sealed class Progress
{
    private readonly Action<JsonArray>? _report;
    private readonly List<JsonObject> _units = [];

    /// <summary>Creates a tracker that reports nothing.</summary>
    /// <remarks>
    /// The framework hands a reporting tracker to a handler; this constructor is for a tracker
    /// that exists outside a run, which is the Python version's behaviour for one.
    /// </remarks>
    public Progress()
    {
    }

    /// <summary>Creates a tracker that hands its reports to <paramref name="report"/>.</summary>
    /// <param name="report">Where a report goes.</param>
    internal Progress(Action<JsonArray> report)
    {
        _report = report;
    }

    /// <summary>Reports how far a run has got, as a fraction.</summary> <param name="fraction">How
    /// much is done, between 0 and 1, or <see langword="null"/> to hide the bar.</param>
    /// <param name="description">What the run is doing, or <see langword="null"/> for no
    /// text.</param> <param name="total">How many steps the run has, when the fraction is not what
    /// is drawn.</param> <param name="unit">What a step is called.</param> <remarks>This is the
    /// Python version's <c>progress(0.5, desc="...")</c>.</remarks>
    public void Report(double? fraction, string? description = null, double? total = null, string unit = "steps") =>
        Push(Unit(index: null, length: total, unit: unit, progress: fraction, description: description));

    /// <summary>Reports how many steps a run has done.</summary> <param name="index">How many steps
    /// are done.</param> <param name="total">How many steps there are, or <see langword="null"/>
    /// when unknown.</param> <param name="description">What the run is doing, or
    /// <see langword="null"/> for no text.</param> <param name="unit">What a step is
    /// called.</param> <remarks>This is the Python version's <c>progress((i, n), desc="...")</c>,
    /// which carries a count instead of a fraction.</remarks>
    public void ReportSteps(double index, double? total = null, string? description = null, string unit = "steps") =>
        Push(Unit(index: index, length: total, unit: unit, progress: null, description: description));

    /// <summary>Reports progress as a sequence is walked, one step per element.</summary>
    /// <typeparam name="T">What the sequence carries.</typeparam>
    /// <param name="values">The sequence to walk.</param>
    /// <param name="description">What the run is doing, or <see langword="null"/> for no text.</param>
    /// <param name="total">How many steps there are, or <see langword="null"/> to count the sequence.</param>
    /// <param name="unit">What a step is called.</param>
    /// <returns>The same elements, reported one at a time as they are taken.</returns>
    /// <remarks>
    /// This is the Python version's <c>progress.tqdm(iterable)</c>. The tracker stays on the report
    /// until the sequence is exhausted, so a second, nested sequence is what the client sees
    /// afterwards.
    /// </remarks>
    public IEnumerable<T> Track<T>(
        IEnumerable<T> values,
        string? description = null,
        double? total = null,
        string unit = "steps")
    {
        ArgumentNullException.ThrowIfNull(values);

        if (_report is null)
        {
            foreach (var value in values)
            {
                yield return value;
            }

            yield break;
        }

        var length = total ?? (values is ICollection<T> collection ? collection.Count : null);
        var tracked = Unit(index: 0, length: length, unit: unit, progress: null, description: description);
        _units.Add(tracked);
        Push(null);

        var index = 0d;
        foreach (var value in values)
        {
            yield return value;

            index++;
            tracked["index"] = index;
            Push(null);
        }

        _units.Remove(tracked);
    }

    /// <summary>Adds a step to the sequence being walked, which is the Python version's
    /// <c>update(n)</c>.</summary> <param name="steps">How many steps to add.</param>
    public void Update(double steps = 1)
    {
        if (_units.Count == 0 || _units[^1]["index"] is not JsonValue index || !index.TryGetValue<double>(out var current))
        {
            return;
        }

        _units[^1]["index"] = current + steps;
        Push(null);
    }

    /// <summary>One progress entry, as the client reads it.</summary>
    private static JsonObject Unit(double? index, double? length, string unit, double? progress, string? description) => new()
    {
        ["index"] = index is { } at ? JsonValue.Create(at) : null,
        ["length"] = length is { } size ? JsonValue.Create(size) : null,
        ["unit"] = unit,
        ["progress"] = progress is { } done ? JsonValue.Create(done) : null,
        ["desc"] = description,
    };

    /// <summary>Hands the current report to the queue, with one extra entry appended.</summary>
    /// <param name="extra">An entry to append, or <see langword="null"/> to report what is being tracked.</param>
    private void Push(JsonObject? extra)
    {
        if (_report is null)
        {
            return;
        }

        var units = new JsonArray();
        foreach (var unit in _units)
        {
            units.Add(unit.DeepClone());
        }

        if (extra is not null)
        {
            units.Add(extra);
        }

        _report(units);
    }
}

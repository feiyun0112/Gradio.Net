using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// What a flag button runs: it records the sample and answers with the button's own new state.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>FlagMethod</c>. It is a callable rather than a plain function because it
/// carries the two things a click needs and a handler signature cannot: the callback that does the
/// writing, and the option this particular button stands for. An interface with three flag buttons
/// binds three of these, one per option.
/// </para>
/// <para>
/// The answer is a button update — the label again, interactive again — because the click has
/// already changed it to say the sample is being saved. A button that answered with nothing would
/// stay in that state.
/// </para>
/// </remarks>
public sealed class FlagMethod
{
    private readonly FlaggingCallback _callback;
    private readonly string _label;
    private readonly string? _value;
    private readonly bool _visualFeedback;

    /// <summary>Creates the method one flag button runs.</summary>
    /// <param name="callback">The callback that records the sample.</param>
    /// <param name="label">The label the button goes back to when the click is done.</param>
    /// <param name="value">The option recorded in the log, or <see langword="null"/> for none.</param>
    /// <param name="visualFeedback">
    /// Whether the button is put back the way it was. An interface that flags automatically has no
    /// button to put back, which is what this turns off.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="callback"/> is <see langword="null"/>.</exception>
    public FlagMethod(FlaggingCallback callback, string label, string? value, bool visualFeedback = true)
    {
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentNullException.ThrowIfNull(label);

        _callback = callback;
        _label = label;
        _value = value;
        _visualFeedback = visualFeedback;
    }

    /// <summary>The name the endpoint is exposed under, which is the Python version's.</summary>
    /// <remarks>
    /// Every flag button of an application shares it, so the second one is served as
    /// <c>Flag_1</c> — the suffix is what keeps two endpoints from answering to one name, and the
    /// name is otherwise the same on both sides.
    /// </remarks>
    public string Name => "Flag";

    /// <summary>
    /// The name the reset endpoint is exposed under.
    /// </summary>
    /// <remarks>The Python version names it after the method it binds, which is
    /// <c>reset</c>.</remarks>
    public string ResetName => "reset";

    /// <summary>The handler to bind, for a button that flags <paramref name="arity"/> components.</summary>
    /// <param name="arity">How many components the interface records when this button is clicked.</param>
    /// <returns>The delegate the click runs.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="arity"/> is negative.</exception>
    /// <remarks>
    /// The values arrive as the client sent them, because the binding this is used for preprocesses
    /// nothing: what is recorded is what the user submitted, not what a handler would have been
    /// handed.
    /// </remarks>
    public Delegate Binding(int arity) =>
        CollectingDelegate.Of(
            arity,
            values => InvokeAsync([.. values.Select(JsonValues.FromObject)]));

    /// <summary>Puts the button back the way it was, for a click on the interface's clear button.</summary>
    /// <returns>The button's own label and its interactivity, as an update.</returns>
    /// <remarks>
    /// The Python version's <c>FlagMethod.reset</c>: a flag button says "Saving..." while the
    /// sample is being written, and clearing the interface is what puts it back when the click
    /// never completed.
    /// </remarks>
    public object? Reset() => new Button { Value = _label, Interactive = true };

    /// <summary>Records one flagged sample.</summary>
    /// <param name="values">One value per component the interface flags, as the client sent them.</param>
    /// <returns>The button's new state, or <see langword="null"/> when no feedback is shown.</returns>
    /// <exception cref="Exception">The sample could not be recorded and there is no button to say so on.</exception>
    /// <remarks>
    /// <para>
    /// A callback that throws is not a failed run when the user is looking at a button: the sample
    /// could not be recorded, and the button says so rather than an error dialog — which is the
    /// Python version's answer, and it is the answer for anything that can go wrong rather than for
    /// the few failures a log is expected to have. A component that refuses a sample, a directory
    /// that is a file, a disk that filled up: all of them are the same thing to the person who
    /// clicked.
    /// </para>
    /// <para>
    /// Automatic flagging has no button, so it has nowhere to say it, and there the failure is the
    /// run's: a broken directory surfaces instead of losing samples quietly. That is a deliberate
    /// difference from the Python version, which swallows the failure in both cases.
    /// </para>
    /// </remarks>
    public async Task<object?> InvokeAsync(IReadOnlyList<JsonNode?> values)
    {
        try
        {
            _callback.Flag(values, _value);
        }
        catch (Exception) when (_visualFeedback)
        {
            return new Button { Value = "Error!" };
        }

        if (!_visualFeedback)
        {
            return null;
        }

        // The Python version waits a moment so that the user sees the button change. The wait is here rather
        // than in the browser because the state the button was put into is the server's answer, and
        // a wait the client cannot see is the one that makes the change visible at all.
        await Task.Delay(TimeSpan.FromMilliseconds(800)).ConfigureAwait(false);

        return new Button { Value = _label, Interactive = true };
    }
}

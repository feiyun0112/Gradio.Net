namespace Gradio.Net;
/// <summary>
/// How an application wants its runs scheduled.
/// </summary>
/// <remarks>
/// <para>
/// These are the Python version's <c>Blocks.queue()</c> parameters, kept as one record so that the
/// settings are read from one place rather than from four fields the queue would have to be taught
/// about one at a time.
/// </para>
/// <para>
/// The defaults are the ones an application that never calls <see cref="Blocks.Queue"/> gets,
/// which is what makes queueing work out of the box: the Python version enables the queue when a
/// <c>Blocks</c> is constructed, so an application that says nothing still gets the scheduling
/// rules and the messages that describe them.
/// </para>
/// </remarks>
public sealed record QueueSettings
{
    /// <summary>
    /// Whether the queue re-describes itself whenever a run finishes, rather than on a timer.
    /// </summary>
    /// <value>
    /// <see langword="true"/>, which is the Python version's <c>status_update_rate="auto"</c>: an
    /// estimate that is recomputed when the thing it estimates changes is both fresher and cheaper
    /// than one recomputed on a schedule.
    /// </value>
    public bool LiveUpdates { get; init; } = true;

    /// <summary>How often the queue re-describes itself when it is not doing so live, in seconds.</summary>
    public double UpdateIntervalSeconds { get; init; } = 1;

    /// <summary>The most events the queue holds, or <see langword="null"/> for no limit.</summary>
    /// <remarks>A queue that is full refuses a run rather than growing without bound.</remarks>
    public int? MaxSize { get; init; }

    /// <summary>
    /// The limit a binding that names none runs under.
    /// </summary>
    /// <value>
    /// <see cref="ConcurrencyLimit.ApplicationDefault"/>, which resolves to one run at a time —
    /// the Python version's default, and the one that keeps a single slow handler from being called
    /// concurrently with itself.
    /// </value>
    public ConcurrencyLimit DefaultConcurrencyLimit { get; init; } = ConcurrencyLimit.ApplicationDefault;

    /// <summary>Whether the direct call routes are open, letting a caller skip the queue.</summary>
    /// <remarks>
    /// The Python version defaults this to true outside a Hugging Face Space. Closed, the direct
    /// routes answer 404 with the Python version's own words while the binding itself keeps working
    /// through the queue — the switch closes a way in, not the endpoint.
    /// </remarks>
    public bool ApiOpen { get; init; } = true;
}

/// <summary>
/// The scheduling an application asked for, and the bindings a request can name.
/// </summary>
/// <remarks>
/// This lives apart from the render root's own code because it changes for its own reason: the
/// root changes when rendering does, and this when a deployment decides how much work to admit at
/// once.
/// </remarks>
public sealed partial class Blocks
{
    private QueueSettings _queueSettings = new();

    /// <summary>The scheduling settings in force.</summary>
    public QueueSettings QueueSettings => _queueSettings;

    /// <summary>
    /// Configures the queue, the way the Python version's <c>Blocks.queue()</c> does.
    /// </summary>
    /// <param name="statusUpdateRate">
    /// How often the queue re-describes itself, in seconds, or <see langword="null"/> for
    /// the Python version's <c>"auto"</c> — re-describe whenever a run finishes.
    /// </param>
    /// <param name="apiOpen">
    /// Whether the direct call routes are open, or <see langword="null"/> to leave the setting as
    /// it is.
    /// </param>
    /// <param name="maxSize">The most events the queue holds, or <see langword="null"/> for no limit.</param>
    /// <param name="defaultConcurrencyLimit">
    /// The limit a binding that names none runs under, defaulting to one run at a time.
    /// </param>
    /// <returns>This application, so that the call can be chained.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="statusUpdateRate"/> is not positive.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxSize"/> is not positive.</exception>
    /// <remarks>
    /// Called before the host starts serving: the queue reads these once, when it starts. Not "when
    /// it is created" — the container builds the queue while the host is being assembled, which is
    /// before the application has had its say, so a setting written between the two would be read by
    /// nobody.
    /// </remarks>
    public Blocks Queue(
        double? statusUpdateRate = null,
        bool? apiOpen = null,
        int? maxSize = null,
        ConcurrencyLimit defaultConcurrencyLimit = default)
    {
        if (statusUpdateRate is { } interval)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(interval, 0);
        }

        if (maxSize is { } size)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(size, 1);
        }

        _queueSettings = new QueueSettings
        {
            LiveUpdates = statusUpdateRate is null,
            UpdateIntervalSeconds = statusUpdateRate ?? 1,
            MaxSize = maxSize,
            DefaultConcurrencyLimit = defaultConcurrencyLimit,
            ApiOpen = apiOpen ?? _queueSettings.ApiOpen,
        };

        return this;
    }

    /// <summary>The binding a request named, or <see langword="null"/> when there is no such binding.</summary>
    /// <param name="index">The binding's id, which is its position in <see cref="Dependencies"/>.</param>
    /// <returns>The binding, or <see langword="null"/>.</returns>
    /// <remarks>
    /// A client names a binding by index, which is what the config advertises, so the lookup is by
    /// position rather than by name. An index nothing was registered at is a client asking for
    /// something this application does not have, which is a refusal rather than a failure.
    /// </remarks>
    internal Dependency? DependencyAt(int index) =>
        index >= 0 && index < _dependencies.Count ? _dependencies[index] : null;
}

namespace Gradio.Net;
/// <summary>
/// What the Vibe editor needs to know about this deployment: which file it edits, which model it
/// asks, and how much of an answer it accepts.
/// </summary>
/// <remarks>
/// <para>
/// The switch that turns the editor on is not here: it is <see cref="Blocks.VibeMode"/>, because the
/// config carries it as <c>vibe_mode</c> and the frontend reads it there to decide whether to draw
/// the panel at all. What is here is the rest — the file the editor rewrites and the request it
/// makes — which the config says nothing about.
/// </para>
/// <para>
/// The demo file is the Python version's <c>GRADIO_WATCH_DEMO_PATH</c>, the file its reload watcher
/// is following: the editor writes the model's answer into that file, and the watcher reloads the
/// app. An application that names no file has nothing to edit, and the routes say so rather than
/// editing something else.
/// </para>
/// <para>
/// <see cref="Model"/> and <see cref="MaxTokens"/> are the Python version's two literals for the
/// completion it asks for. They are options rather than constants because the backend is the
/// application's: a backend that serves one model, or that refuses a request above a few thousand
/// tokens, has to be told what the editor wants rather than discovering it.
/// </para>
/// </remarks>
public sealed class VibeOptions
{
    /// <summary>The model the Python version asks, written the way the Python version writes
    /// it.</summary>
    /// <remarks>
    /// The name is Hugging Face's spelling of an open model, and it is here only as the default: a
    /// backend this framework does not ship is what turns the name into a request, and it is free to
    /// map the name onto whatever it serves.
    /// </remarks>
    public const string DefaultModel = "openai/gpt-oss-120b";

    /// <summary>The file the editor rewrites.</summary>
    /// <value>
    /// <c>GRADIO_WATCH_DEMO_PATH</c>, or the empty string when the deployment names none.
    /// </value>
    /// <remarks>
    /// An empty path is a deployment that has no file to edit, which is the answer the routes give:
    /// the Python version's <c>open("")</c> fails the same way, because a server started without a
    /// demo path has nothing for the editor to rewrite.
    /// </remarks>
    public string DemoPath { get; init; } = DemoPathFromEnvironment();

    /// <summary>The model the completion is asked for.</summary>
    /// <value><see cref="DefaultModel"/>.</value>
    public string Model { get; init; } = DefaultModel;

    /// <summary>How many tokens the answer may use.</summary>
    /// <value>The Python version's ten thousand, which is what its own request asks for.</value>
    public int MaxTokens { get; init; } = 10_000;

    /// <summary>The demo file the environment names.</summary>
    /// <returns>The path, or the empty string.</returns>
    private static string DemoPathFromEnvironment() =>
        Environment.GetEnvironmentVariable("GRADIO_WATCH_DEMO_PATH") ?? string.Empty;
}

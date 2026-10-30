namespace Gradio.Net;
/// <summary>
/// The endpoints this framework exposes, as an authority rather than as a by-product of
/// registration code.
/// </summary>
/// <remarks>
/// <para>
/// The table mirrors the Python version's route table: every route the Python version has is here,
/// and no route it does not have is, so that a request the Python version answers is answered here
/// and a request it refuses is refused here.
/// </para>
/// <para>
/// Paths are written in the Python version's syntax, converters included, because the Python
/// version is the authority: the ASP.NET Core form is derived (see <see cref="RouteTemplate"/>)
/// rather than maintained beside it.
/// </para>
/// <para>
/// There is one entry per route that can be matched, so the Python version's trailing-slash
/// spellings are not repeated here: ASP.NET Core matches <c>/config</c> and <c>/config/</c> to the
/// same endpoint, and registering both makes every request to either one ambiguous. The URLs the
/// Python version accepts are still accepted; they are one route instead of two.
/// <see cref="RouteTemplate.Normalize"/> states that equivalence once, for both sides of the
/// comparison.
/// </para>
/// <para>
/// Routes are registered but not implemented yet. The capabilities behind them land batch by
/// batch, and until then each answers with the not-implemented placeholder, so that a request to a
/// route this framework has not built is distinguishable from a request to a route that does not
/// exist.
/// </para>
/// <para>
/// Routes the Python version registers conditionally are listed unconditionally, because a route
/// that is sometimes present is still a route this framework must not be missing; gating them
/// arrives with the capabilities that own the condition (profiling, run history, OAuth, MCP).
/// </para>
/// <para>
/// A route the Python version attaches <c>login_check</c> to is declared here as
/// <see cref="GradioRoute.RequiresLogin"/>, so that the set is one list rather than a list beside a
/// list: what the Python version guards, what this framework guards, and what a test can walk are
/// the same thing.
/// </para>
/// </remarks>
public sealed class GradioRouteTable
{
    /// <summary>Builds the table.</summary>
    public GradioRouteTable()
    {
        Routes = new List<GradioRoute>
        {
            // The application page and the assets it loads.
            new("GET", "/"),
            new("HEAD", "/"),
            Guarded("GET", "/config"),
            new("GET", "/favicon.ico"),
            new("GET", "/manifest.json"),
            new("GET", "/robots.txt"),
            new("GET", "/theme.css"),
            new("GET", "/gradio_api/theme.css"),
            new("GET", "/assets/{path:path}"),
            new("GET", "/static/{path:path}"),
            new("GET", "/svelte/{path:path}"),
            new("GET", "/pwa_icon"),
            new("GET", "/pwa_icon/{size}"),

            // Signing in, signing out, and who is asking.
            new("POST", "/login"),
            new("GET", "/logout"),
            new("GET", "/login/huggingface"),
            new("GET", "/login/callback"),
            new("GET", "/gradio_api/user"),
            new("GET", "/gradio_api/login_check"),
            new("GET", "/gradio_api/token"),
            new("GET", "/gradio_api/app_id"),

            // The machine-readable descriptions of the app.
            Guarded("GET", "/gradio_api/info"),
            Guarded("GET", "/gradio_api/openapi.json"),
            Guarded("GET", "/gradio_api/deep_link"),

            // Calling a function: through the queue, straight through, and through the API.
            Guarded("POST", "/gradio_api/queue/join"),
            Guarded("GET", "/gradio_api/queue/data"),
            Guarded("GET", "/gradio_api/queue/status"),
            new("POST", "/gradio_api/cancel"),
            Guarded("POST", "/gradio_api/run/{api_name}"),
            Guarded("POST", "/gradio_api/api/{api_name}"),
            Guarded("POST", "/gradio_api/call/{api_name}"),
            Guarded("GET", "/gradio_api/call/{api_name}/{event_id}"),
            Guarded("POST", "/gradio_api/call/v2/{api_name}"),
            Guarded("GET", "/gradio_api/call/v2/{api_name}/{event_id}"),
            new("GET", "/gradio_api/heartbeat/{session_hash}"),

            // Streaming: the SSE channels, and the HLS playlist of a long media stream.
            new("POST", "/gradio_api/stream/{event_id}"),
            new("POST", "/gradio_api/stream/{event_id}/close"),
            new("GET", "/gradio_api/stream/{session_hash}/{run}/{component_id}/playlist.m3u8"),
            new("GET", "/gradio_api/stream/{session_hash}/{run}/{component_id}/playlist-file"),
            new("GET", "/gradio_api/stream/{session_hash}/{run}/{component_id}/{segment_id}.{ext}"),

            // Files and media.
            Guarded("POST", "/gradio_api/upload"),
            Guarded("GET", "/gradio_api/upload_progress"),
            Guarded("GET", "/gradio_api/file/{path:path}"),
            Guarded("GET", "/gradio_api/file={path_or_url:path}"),
            Guarded("HEAD", "/gradio_api/file={path_or_url:path}"),
            Guarded("GET", "/gradio_api/proxy={url_path:path}"),
            Guarded("HEAD", "/gradio_api/proxy={url_path:path}"),

            // The browser calling a component's own server-side functions.
            Guarded("POST", "/gradio_api/component_server"),
            new("GET", "/gradio_api/custom_component/{id}/{environment}/{type}/{file_name}"),
            Guarded("POST", "/gradio_api/process_recording"),
            new("POST", "/gradio_api/reset"),

            // Run history.
            new("GET", "/gradio_api/runs"),
            Guarded("POST", "/gradio_api/run-history/connect"),
            Guarded("GET", "/gradio_api/run-history/buckets"),
            Guarded("GET", "/gradio_api/run-history/records"),
            Guarded("GET", "/gradio_api/run-history/records/{endpoint}/{record_id}/assets/{asset_id}"),

            // Vibe: the in-browser editor and the code it produces.
            new("POST", "/gradio_api/vibe-edit"),
            new("POST", "/gradio_api/undo-vibe-edit"),
            new("GET", "/gradio_api/vibe-code"),
            new("POST", "/gradio_api/vibe-code"),
            new("POST", "/gradio_api/vibe-starter-queries"),

            // The reload channel of a development server.
            Guarded("GET", "/gradio_api/dev/reload"),
            new("GET", "/gradio_api/startup-events"),

            // Monitoring and profiling.
            Guarded("GET", "/monitoring"),
            Guarded("GET", "/monitoring/summary"),
            new("GET", "/monitoring/{key}"),
            Guarded("GET", "/gradio_api/profiling/traces"),
            Guarded("GET", "/gradio_api/profiling/summary"),
            Guarded("POST", "/gradio_api/profiling/clear"),
        }.AsReadOnly();
    }

    /// <summary>Every endpoint, in the Python version's own template syntax.</summary>
    public IReadOnlyList<GradioRoute> Routes { get; }

    /// <summary>A route that answers only to someone who has signed in.</summary>
    /// <param name="method">The HTTP method, as the Python version spells it.</param>
    /// <param name="path">The Python path template.</param>
    /// <returns>The route, marked as one the Python version guards.</returns>
    private static GradioRoute Guarded(string method, string path) =>
        new(method, path) { RequiresLogin = true };
}


namespace Gradio.Net.Tests.Server;

/// <summary>
/// The Python routes whose capability has landed, and therefore no longer answer the
/// not-implemented placeholder.
/// </summary>
/// <remarks>
/// <para>
/// The list is written out rather than derived, because it is a statement about this build that a
/// reviewer should have to make: the tests that walk the whole route table assert that every route
/// is either one of these or a placeholder, so a route that quietly starts answering for real
/// fails them until it is declared here.
/// </para>
/// <para>
/// A route named here has to be one the Python version declares, so a typo cannot pass as an
/// implementation.
/// </para>
/// <para>
/// Each entry also carries the request a test can make without preparing anything, and the status
/// the capability answers it with. Not every route can be asked a bare question and still answer
/// for itself: a request that names no session is refused, and a stream does not end until the
/// client stops reading. Naming both here keeps that a declared property of the route rather than
/// something a reader has to reconstruct from the capability's code.
/// </para>
/// </remarks>
internal static class ImplementedRoutes
{
    /// <summary>One implemented route, and what it answers a bare request with.</summary>
    /// <param name="Method">The HTTP method, as the Python version spells it.</param>
    /// <param name="Path">The Python path template.</param> <param name="Status">The status the
    /// route answers a request that carries nothing with.</param>
    /// <param name="SampleUrl">
    /// A concrete URL reaching the route, or <see langword="null"/> when the template is already
    /// one.
    /// </param>
    /// <param name="Streams">
    /// Whether the answer is an event stream, which a caller stops reading rather than reads out.
    /// </param>
    internal sealed record ImplementedRoute(
        string Method,
        string Path,
        int Status = 200,
        string? SampleUrl = null,
        bool Streams = false);

    /// <summary>The routes a capability answers, in the order the route table lists them.</summary>
    internal static readonly IReadOnlyList<ImplementedRoute> All =
    [
        // The application page and everything it loads. Three of these routes are templates,
        // and the sample they are asked with names a file that is not in the tree: the route's
        // job is to answer, and "a path naming nothing is missing" is the answer that does not
        // depend on which files the vendored tree happens to carry. The assets that do exist are
        // walked in FrontendHostingTests, which is where serving them is the subject.
        new("GET", "/"),
        new("HEAD", "/"),
        new("GET", "/config"),
        new("GET", "/favicon.ico"),
        new("GET", "/manifest.json"),
        new("GET", "/robots.txt"),
        new("GET", "/theme.css"),
        new("GET", "/gradio_api/theme.css"),
        new("GET", "/assets/{path:path}", Status: 404, SampleUrl: "/assets/a.txt"),
        new("GET", "/static/{path:path}", Status: 404, SampleUrl: "/static/a.txt"),
        new("GET", "/svelte/{path:path}", Status: 404, SampleUrl: "/svelte/a.txt"),

        // The icon routes are asked by an application that names no icon, which is the Python version's 404
        // for the same application; PwaTests is where serving a configured icon is the subject.
        new("GET", "/pwa_icon", Status: 404),
        new("GET", "/pwa_icon/{size}", Status: 404, SampleUrl: "/pwa_icon/192"),

        new("GET", "/gradio_api/info"),
        new("GET", "/gradio_api/openapi.json"),

        // Signing in and out, and who is asking. The sign-in form is a required body, so a POST
        // without one is refused the way FastAPI refuses it; signing out sends the browser back to
        // the page, which is the only thing it ever answers. The two routes of the identity-provider
        // flow are only for an application with a LoginButton — the Python version registers them for one that
        // has, and answers 404 otherwise, which is what a bare application gets.
        new("POST", "/login", Status: 422),
        new("GET", "/logout", Status: 302),
        new("GET", "/login/huggingface", Status: 404),
        new("GET", "/login/callback", Status: 404),
        new("GET", "/gradio_api/user"),
        new("GET", "/gradio_api/login_check"),
        new("GET", "/gradio_api/token"),
        new("GET", "/gradio_api/app_id"),

        // The link a share button copies. The session is a required parameter, so a request that
        // names none is refused the way FastAPI refuses it; a request naming a session the server
        // does not hold is answered with the empty string, which the script that asks reads as
        // "no link".
        new("GET", "/gradio_api/deep_link", Status: 422, SampleUrl: "/gradio_api/deep_link"),

        // The queue's six routes are one capability. Two of them only answer a request that
        // carries nothing by refusing it, and two answer with a stream, which is why each says
        // what to ask it and what comes back.
        new("POST", "/gradio_api/queue/join", Status: 400),
        new("GET", "/gradio_api/queue/data", Streams: true),
        new("GET", "/gradio_api/queue/status"),
        new("POST", "/gradio_api/cancel"),
        new("POST", "/gradio_api/reset"),
        new("GET", "/gradio_api/heartbeat/{session_hash}", SampleUrl: "/gradio_api/heartbeat/a", Streams: true),

        // The direct route a binding that skipped the queue is called through, under both of the
        // names the Python version gives it. A request naming a binding an application does not have is a
        // miss, which is the answer that does not depend on what the application bound.
        new("POST", "/gradio_api/run/{api_name}", Status: 404, SampleUrl: "/gradio_api/run/not_bound"),
        new("POST", "/gradio_api/api/{api_name}", Status: 404, SampleUrl: "/gradio_api/api/not_bound"),

        // The file capability. An upload with no multipart body is refused, a progress request
        // that names no upload is missing, and the three serving routes are asked with a path
        // that names nothing the application keeps — which is the refusal that does not depend on
        // which files the run happens to have written. What they answer when there *is* a file is
        // FileEndpointTests' subject.
        new("POST", "/gradio_api/upload", Status: 400),
        new("GET", "/gradio_api/upload_progress", Status: 404),
        new("GET", "/gradio_api/file/{path:path}", Status: 403, SampleUrl: "/gradio_api/file/a.txt"),
        new("GET", "/gradio_api/file={path_or_url:path}", Status: 403, SampleUrl: "/gradio_api/file=a.txt"),
        new("HEAD", "/gradio_api/file={path_or_url:path}", Status: 403, SampleUrl: "/gradio_api/file=a.txt"),

        // The bundle a component that brings its own frontend is served from. It is asked for an
        // identity no component of a bare application claims, which is the refusal that does not
        // depend on which components the application happens to have; CustomComponentTests is
        // where serving one is the subject.
        new("GET", "/gradio_api/custom_component/{id}/{environment}/{type}/{file_name}", Status: 404,
            SampleUrl: "/gradio_api/custom_component/not_a_component/client/component/index.js"),

        // The browser calling a component's own server functions, and the screen recording a video
        // component uploads: a request that carries neither a JSON nor a multipart body is refused
        // by both, which is the answer that does not depend on the application's components.
        new("POST", "/gradio_api/component_server", Status: 400),
        new("POST", "/gradio_api/process_recording", Status: 400),

        // Streaming media. The two feeding routes are asked for an event nobody admitted, and the
        // three serving routes for a stream nobody opened: a 404 that says "not here" rather than
        // "not implemented". The segment route is asked with the extension it refuses, because a
        // sample that named a real one would answer "no such stream" and stop proving that the
        // extension is checked at all.
        new("POST", "/gradio_api/stream/{event_id}", Status: 404, SampleUrl: "/gradio_api/stream/x"),
        new("POST", "/gradio_api/stream/{event_id}/close", Status: 404, SampleUrl: "/gradio_api/stream/x/close"),
        new("GET", "/gradio_api/stream/{session_hash}/{run}/{component_id}/playlist.m3u8", Status: 404,
            SampleUrl: "/gradio_api/stream/a/b/1/playlist.m3u8"),
        new("GET", "/gradio_api/stream/{session_hash}/{run}/{component_id}/{segment_id}.{ext}", Status: 400,
            SampleUrl: "/gradio_api/stream/a/b/1/c.txt"),
        new("GET", "/gradio_api/stream/{session_hash}/{run}/{component_id}/playlist-file", Status: 404,
            SampleUrl: "/gradio_api/stream/a/b/1/playlist-file"),

        // Run history. The page is served by the frontend capability, which is where every other
        // page comes from; the records and the buckets are answered from the local store, which is
        // empty for a host that has run nothing. Connecting with no body names no bucket, which is
        // the refusal; the asset route is asked for a record that was never written.
        new("GET", "/gradio_api/runs"),
        new("POST", "/gradio_api/run-history/connect", Status: 422),
        new("GET", "/gradio_api/run-history/buckets"),
        new("GET", "/gradio_api/run-history/records"),
        new("GET", "/gradio_api/run-history/records/{endpoint}/{record_id}/assets/{asset_id}", Status: 404,
            SampleUrl: "/gradio_api/run-history/records/echo/a1/assets/a001.txt"),

        // A proxy URL names a file on an application this one was loaded from; loading is its own
        // batch, so no application can have one and the refusal is the answer.
        new("GET", "/gradio_api/proxy={url_path:path}", Status: 400, SampleUrl: "/gradio_api/proxy=a.txt"),
        new("HEAD", "/gradio_api/proxy={url_path:path}", Status: 400, SampleUrl: "/gradio_api/proxy=a.txt"),

        // Monitoring. The address route answers the page the Python version answers, and the summary is a
        // document even when nothing has run. The key route is asked with a key that is not the
        // application's, which is the refusal that does not depend on what has been recorded.
        new("GET", "/monitoring"),
        new("GET", "/monitoring/summary"),
        new("GET", "/monitoring/{key}", Status: 403, SampleUrl: "/monitoring/not-the-key"),

        // Profiling. The Python version registers these three only when profiling is on, so a bare host — which
        // is not profiled — answers "not here" rather than the placeholder: the capability exists and
        // is switched off, which is not the same as not built.
        new("GET", "/gradio_api/profiling/traces", Status: 404),
        new("GET", "/gradio_api/profiling/summary", Status: 404),
        new("POST", "/gradio_api/profiling/clear", Status: 404),

        // Vibe. The editor is off unless the deployment turns it on, and an application that has not
        // is refused — which is what the Python version answers too, rather than the route being absent. Three
        // of the five take a body, and FastAPI validates a body before the handler sees it, so a
        // bare request to one of those is refused as malformed (422) rather than as switched off;
        // the two that take no body answer the switch (403).
        new("POST", "/gradio_api/vibe-edit", Status: 422),
        new("POST", "/gradio_api/undo-vibe-edit", Status: 422),
        new("GET", "/gradio_api/vibe-code", Status: 403),
        new("POST", "/gradio_api/vibe-code", Status: 422),
        new("POST", "/gradio_api/vibe-starter-queries", Status: 403),

        // The application's own lifecycle. The startup route answers the call that ran the
        // application's startup events with true and every later call with false; the reload route
        // is the stream a development page watches, which a caller stops reading rather than reads
        // out.
        new("GET", "/gradio_api/dev/reload", Streams: true),
        new("GET", "/gradio_api/startup-events"),
    ];

    /// <summary>Whether a route is one a capability answers.</summary>
    /// <param name="method">The HTTP method, as the Python version spells it.</param>
    /// <param name="path">The Python path template.</param>
    /// <returns><see langword="true"/> when the route is implemented.</returns>
    /// <remarks>
    /// Compared through <see cref="GradioRoute.Key"/>, because the Python version declares
    /// <c>/config</c> and <c>/config/</c> as two entries and this framework serves them with one
    /// endpoint. A route that is implemented is implemented whichever way it is spelled.
    /// </remarks>
    internal static bool Contains(string method, string path)
    {
        var key = new GradioRoute(method, path).Key;

        return All.Any(route => new GradioRoute(route.Method, route.Path).Key == key);
    }
}

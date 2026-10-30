using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// Pins the two template equivalences the route comparison rests on.
/// </summary>
/// <remarks>
/// The expected values here are written out from the Python source and from ASP.NET Core's
/// routing rules, not derived from the implementation: if the normalisation ever collapsed routes
/// that are actually different, only these literals would notice.
/// </remarks>
public sealed class RouteTemplateTests
{
    [Theory]
    [InlineData("/gradio_api/user", "/gradio_api/user")]
    [InlineData("/config", "/config")]
    [InlineData("/gradio_api/stream/{event_id}/close", "/gradio_api/stream/{event_id}/close")]
    [InlineData("/gradio_api/stream/{session_hash}/{run}/{component_id}/{segment_id}.{ext}", "/gradio_api/stream/{session_hash}/{run}/{component_id}/{segment_id}.{ext}")]
    [InlineData("/gradio_api/run-history/records/{endpoint}/{record_id}/assets/{asset_id}", "/gradio_api/run-history/records/{endpoint}/{record_id}/assets/{asset_id}")]
    [InlineData("/assets/{path:path}", "/assets/{**path}")]
    [InlineData("/static/{path:path}", "/static/{**path}")]
    [InlineData("/svelte/{path:path}", "/svelte/{**path}")]
    [InlineData("/gradio_api/file/{path:path}", "/gradio_api/file/{**path}")]
    [InlineData("/gradio_api/proxy={url_path:path}", "/gradio_api/{**url_path:regex(^proxy=.*)}")]
    [InlineData("/gradio_api/file={path_or_url:path}", "/gradio_api/{**path_or_url:regex(^file=.*)}")]
    public void An_upstream_template_becomes_the_pattern_the_host_registers(string upstream, string expected)
    {
        Assert.Equal(expected, new GradioRoute("GET", upstream).RegisteredPattern);
    }

    [Theory]
    [InlineData("GET", "/config/", "GET /config")]
    [InlineData("GET", "/gradio_api/user/", "GET /gradio_api/user")]
    [InlineData("GET", "/assets/{path:path}", "GET /assets/{path}")]
    [InlineData("GET", "/gradio_api/proxy={url_path:path}", "GET /gradio_api/proxy={url_path}")]
    [InlineData("GET", "/gradio_api/file={path_or_url:path}", "GET /gradio_api/file={path_or_url}")]
    [InlineData("GET", "/", "GET /")]
    [InlineData("HEAD", "/gradio_api/proxy={url_path:path}", "HEAD /gradio_api/proxy={url_path}")]
    public void The_comparison_form_folds_a_converter_and_a_trailing_slash(string method, string path, string expected)
    {
        Assert.Equal(expected, new GradioRoute(method, path).Key);
    }

    [Fact]
    public void Routes_that_differ_do_not_compare_equal()
    {
        var first = new GradioRoute("GET", "/gradio_api/user").Key;
        var second = new GradioRoute("GET", "/gradio_api/token").Key;
        var third = new GradioRoute("POST", "/gradio_api/user").Key;

        Assert.NotEqual(first, second);
        Assert.NotEqual(first, third);
    }
}

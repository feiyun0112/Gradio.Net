using System.Text.Json.Nodes;
using Gradio.Net;
using Microsoft.AspNetCore.Builder;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The address an application reports, which is the address a browser can actually use.
/// </summary>
/// <remarks>
/// <para>
/// The page, the config and the API info all carry this address, and it is derived from the request
/// rather than configured. Three sources can name it, and the order matters: a proxy's
/// <c>x-forwarded-host</c>, then the SSR runtime's <c>x-gradio-server</c>, then the address the
/// request arrived on. The Python version resolves them in that order because the first two are the
/// address a browser used, and the last one may be a loopback port behind a proxy.
/// </para>
/// <para>
/// The failure this prevents is quiet: a page whose theme link points at an internal port looks
/// right on the machine that serves it and arrives unstyled everywhere else.
/// </para>
/// </remarks>
public sealed class RootUrlTests
{
    [Fact]
    public async Task A_proxy_that_reports_its_host_names_the_address()
    {
        await using var host = await StartAsync();

        using var request = new HttpRequestMessage(HttpMethod.Get, "/config");
        request.Headers.Add("x-forwarded-host", "app.example.test");
        request.Headers.Add("x-forwarded-proto", "https");

        Assert.Equal("https://app.example.test", await RootAsync(host, request));
    }

    [Fact]
    public async Task The_ssr_runtime_names_the_address_when_no_proxy_does()
    {
        await using var host = await StartAsync();

        using var request = new HttpRequestMessage(HttpMethod.Get, "/config");
        request.Headers.Add("x-gradio-server", "http://app.example.test:8080");

        Assert.Equal("http://app.example.test:8080", await RootAsync(host, request));
    }

    [Fact]
    public async Task A_proxys_host_wins_over_the_ssr_runtimes_origin()
    {
        await using var host = await StartAsync();

        using var request = new HttpRequestMessage(HttpMethod.Get, "/config");
        request.Headers.Add("x-forwarded-host", "outer.example.test");
        request.Headers.Add("x-gradio-server", "http://inner.example.test:8080");

        Assert.Equal("http://outer.example.test", await RootAsync(host, request));
    }

    [Fact]
    public async Task A_reported_host_still_carries_the_path_the_application_is_mounted_at()
    {
        await using var host = await GradioTestHost.StartIntegratedAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            build: ClickExample.Build,
            pipeline: app => app.UsePathBase("/mounted"));

        using var request = new HttpRequestMessage(HttpMethod.Get, "/mounted/config");
        request.Headers.Add("x-forwarded-host", "app.example.test");

        Assert.Equal("http://app.example.test/mounted", await RootAsync(host, request));
    }

    [Fact]
    public async Task A_request_that_names_no_other_address_uses_its_own()
    {
        await using var host = await StartAsync();

        using var request = new HttpRequestMessage(HttpMethod.Get, "/config");

        Assert.Equal(host.BaseAddress.GetLeftPart(UriPartial.Authority), await RootAsync(host, request));
    }

    private static Task<GradioTestHost> StartAsync()
    {
        return GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            ClickExample.Build);
    }

    private static async Task<string> RootAsync(GradioTestHost host, HttpRequestMessage request)
    {
        using var response = await host.Client.SendAsync(request, TestContext.Current.CancellationToken);
        var config = JsonNode.Parse(
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))!.AsObject();

        return config["root"]!.GetValue<string>();
    }
}

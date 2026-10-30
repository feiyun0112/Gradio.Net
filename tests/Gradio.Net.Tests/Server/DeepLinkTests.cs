using System.Net;
using System.Text.Json.Nodes;
using Gradio.Net;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The endpoint a share button asks for a link to: the session's state written down under a name,
/// and the name answered.
/// </summary>
/// <remarks>
/// <para>
/// The link the button copies is the page's own address with that name in it, so this endpoint is
/// the half that writes; the half that reads is the page route, which describes the state the name
/// points at and reports <c>deep_link_state</c> in the config. Both halves are exercised end to end
/// in <c>InterfaceTests</c> — share a session, open the link it names, find the values it held —
/// and what is asserted here is the write: a session the server holds is written out and answered
/// with a name, and a session it does not hold is answered with nothing, which is what the Python
/// version answers.
/// </para>
/// <para>
/// The name is opaque, so nothing asserts its value; what it is asserted to do is name a file, and
/// the file is asserted to hold the application's components.
/// </para>
/// </remarks>
public sealed class DeepLinkTests : IDisposable
{
    private const string Route = "/gradio_api/deep_link";

    /// <summary>The directory this class's applications keep their files in.</summary>
    /// <remarks>
    /// One of its own rather than the process default: these tests write a state file, and the
    /// default folder is shared with every other test that renders an application without saying
    /// where its files go.
    /// </remarks>
    private readonly string _folder = Path.Combine(Path.GetTempPath(), $"gradio-deep-link-{Guid.NewGuid():N}");

    /// <inheritdoc />
    public void Dispose()
    {
        if (Directory.Exists(_folder))
        {
            Directory.Delete(_folder, recursive: true);
        }
    }

    /// <summary>An application whose files go in this class's own directory.</summary>
    /// <param name="blocks">The application being rendered.</param>
    private void Build(Blocks blocks)
    {
        blocks.UploadFolder = _folder;

        Page(blocks);
    }

    [Fact]
    public async Task A_session_the_server_does_not_hold_answers_with_nothing()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            Build);

        using var response = await host.Client.GetAsync(
            $"{Route}?session_hash=never-seen",
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

        // The Python version answers with an empty string rather than with an error: the script that asked
        // reads it as "no link" and copies the page's address alone.
        Assert.Equal("\"\"", await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task A_request_naming_no_session_is_refused_the_way_upstream_refuses_it()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            Build);

        using var response = await host.Client.GetAsync(Route, TestContext.Current.CancellationToken);

        // The session is a required query parameter the Python version, so FastAPI refuses the request before
        // the endpoint runs — and a client written against the Python server reads that body.
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Equal(
            """{"detail":[{"type":"missing","loc":["query","session_hash"],"msg":"Field required","input":null}]}""",
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task A_session_hash_that_is_empty_names_no_session()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            Build);

        using var response = await host.Client.GetAsync(
            $"{Route}?session_hash=",
            TestContext.Current.CancellationToken);

        // Named, and naming nothing: the parameter is there, so this is the empty answer rather than
        // the refusal.
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("\"\"", await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task A_session_the_server_holds_is_written_down_and_answered_with_its_name()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            Build);

        var sessions = host.Services.GetRequiredService<ISessionStore>();
        var blocks = host.Services.GetRequiredService<Blocks>();
        var session = sessions.GetOrAdd("a-session");

        using var response = await host.Client.GetAsync(
            $"{Route}?session_hash=a-session",
            TestContext.Current.CancellationToken);

        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var name = JsonNode.Parse(body)!.GetValue<string>();

        Assert.NotEmpty(name);
        Assert.DoesNotContain('+', name);
        Assert.DoesNotContain('/', name);
        Assert.DoesNotContain('=', name);

        var file = Path.Combine(blocks.UploadFolder, "deep_links", name, "state.json");

        Assert.True(System.IO.File.Exists(file), $"'{file}' was written");

        var components = JsonNode.Parse(await System.IO.File.ReadAllTextAsync(
            file,
            TestContext.Current.CancellationToken))!.AsArray();

        // The state is the application's components as the config describes them, in the order the
        // application rendered them.
        Assert.Equal(blocks.Registered.Count, components.Count);
        Assert.Equal(blocks.Registered[0].Id, components[0]!["id"]!.GetValue<int>());
        Assert.Equal(
            blocks.GetConfig()["components"]!.AsArray()[^1]!.ToJsonString(),
            components[^1]!.ToJsonString());

        // Nothing has been written to the session's state, so there is nothing to write down for it.
        Assert.Equal(0, session.State.Count);
    }

    [Fact]
    public async Task A_sessions_state_is_written_down_with_it()
    {
        await using var host = await GradioTestHost.StartStandaloneAsync(
            logs: null,
            TestContext.Current.CancellationToken,
            Build);

        var sessions = host.Services.GetRequiredService<ISessionStore>();
        var blocks = host.Services.GetRequiredService<Blocks>();
        var session = sessions.GetOrAdd("a-session");
        var state = blocks.Registered.OfType<State>().Single();

        session.State.Set(state.Id, JsonValue.Create("kept"), timeToLive: null, onDelete: null, DateTimeOffset.UtcNow);

        using var response = await host.Client.GetAsync(
            $"{Route}?session_hash=a-session",
            TestContext.Current.CancellationToken);

        var name = JsonNode.Parse(await response.Content.ReadAsStringAsync(
            TestContext.Current.CancellationToken))!.GetValue<string>();

        var file = Path.Combine(blocks.UploadFolder, "deep_links", name, "state.json");
        var components = JsonNode.Parse(await System.IO.File.ReadAllTextAsync(
            file,
            TestContext.Current.CancellationToken))!.AsArray();

        var entry = components.Single(component => component!["id"]!.GetValue<int>() == state.Id);

        // A link to a session that has filled a state is a link to the filled one.
        Assert.Equal("kept", entry!["props"]!["value"]!.GetValue<string>());
    }

    /// <summary>An application with a field and a state, which is all a link needs to name.</summary>
    private static void Page(Blocks blocks)
    {
        using (blocks.BeginRender())
        {
            blocks.Add(new Textbox { Label = "Name" });
            blocks.Add(new State());
        }
    }
}

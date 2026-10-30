using System.Net;
using System.Text.Json.Nodes;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The parts of a served config a test asserts on: the document itself, and the two shapes a
/// dependency's references have.
/// </summary>
/// <remarks>
/// <para>
/// The two readers are what a dependency assertion is made of — a target as <c>id:name</c>, and the
/// ids an input or output list names — and they were spelled once already, in the workflow's tests.
/// A second copy is a second chance to disagree about what a target is, so the shape lives here and
/// both classes call it.
/// </para>
/// <para>
/// The document read is here for the same reason, though the suite still has a copy per test class:
/// each was written to answer one question (a page's config, a config with a query) and they
/// disagree about little things rather than about the document. New tests ask this one.
/// </para>
/// </remarks>
internal static class ConfigDocument
{
    /// <summary>Reads a document the application serves.</summary>
    /// <param name="host">The host serving it.</param>
    /// <param name="path">The route to read.</param>
    /// <param name="cancellationToken">Cancels the test.</param>
    /// <returns>The document.</returns>
    /// <exception cref="Xunit.Sdk.XunitException">The route did not answer with a document.</exception>
    public static async Task<JsonObject> ReadAsync(
        GradioTestHost host,
        string path,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(host);

        using var response = await host.Client.GetAsync(path, cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        return JsonNode.Parse(await response.Content.ReadAsStringAsync(cancellationToken))!.AsObject();
    }

    /// <summary>The targets of a dependency, as <c>id:name</c>.</summary>
    /// <param name="dependency">The dependency.</param>
    /// <returns>The targets, in the order the config carries them.</returns>
    public static string[] Targets(JsonNode? dependency) =>
        [.. dependency!["targets"]!.AsArray()
            .Select(target => $"{target![0]!.GetValue<int>()}:{target[1]!.GetValue<string>()}")];

    /// <summary>The ids a dependency's inputs or outputs name.</summary>
    /// <param name="entries">The array.</param>
    /// <returns>The ids, in the order the config carries them.</returns>
    public static int[] Ids(JsonNode? entries) =>
        [.. entries!.AsArray().Select(entry => entry!.GetValue<int>())];
}

using System.Text.Json.Nodes;

namespace Gradio.Net.Tests.Shared;

/// <summary>
/// Reads the documents the application page carries inside its script elements.
/// </summary>
/// <remarks>
/// <para>
/// The page embeds the config and the API info as JSON, and both are delimited the same way: a
/// marker, the value, then <c>;</c> before the end of the script element. The value ends at the
/// first <c>;</script></c> after the marker because the Python version's serializer escapes
/// <c>&lt;</c>, so the sequence cannot occur inside the JSON itself.
/// </para>
/// <para>
/// The reader is the test project's own, so that a mistake in the code that produced the page
/// cannot hide in the code that reads it.
/// </para>
/// </remarks>
internal static class EmbeddedDocuments
{
    /// <summary>Where the page carries its config.</summary>
    internal const string ConfigMarker = "window.gradio_config = ";

    /// <summary>Where the page carries its API info.</summary>
    internal const string ApiInfoMarker = "window.gradio_api_info = ";

    /// <summary>What the config is replaced by when the page is compared as a template.</summary>
    internal const string ConfigPlaceholder = "{{config}}";

    /// <summary>What the API info is replaced by when the page is compared as a template.</summary>
    internal const string ApiInfoPlaceholder = "{{gradio_api_info}}";

    /// <summary>What the address of the run is replaced by.</summary>
    internal const string RootPlaceholder = "{{root}}";

    /// <summary>Replaces one embedded value with a placeholder, and returns the value.</summary>
    /// <param name="page">The page, or what is left of it after an earlier replacement.</param>
    /// <param name="marker">The text the value follows.</param>
    /// <param name="placeholder">What to leave in its place.</param>
    /// <param name="value">The value, or <see langword="null"/> when the page carries none.</param>
    /// <returns>The page with the value replaced, or <see langword="null"/> when it carries none.</returns>
    internal static string? Replace(string page, string marker, string placeholder, out JsonNode? value)
    {
        ArgumentNullException.ThrowIfNull(page);

        value = null;

        var start = page.IndexOf(marker, StringComparison.Ordinal);
        if (start < 0)
        {
            return null;
        }

        start += marker.Length;

        var end = page.IndexOf(";</script>", start, StringComparison.Ordinal);
        if (end < 0)
        {
            return null;
        }

        value = JsonNode.Parse(page[start..end]);

        return page[..start] + placeholder + page[end..];
    }

    /// <summary>Replaces the address this run was served from with its placeholder.</summary>
    /// <param name="page">The page.</param>
    /// <param name="address">The address the host is listening on.</param>
    /// <returns>The page without the run's address in it.</returns>
    /// <remarks>
    /// The address is a property of the request rather than of the application: it appears in the
    /// inline script that loads the iframe resizer, and two runs on two ports would otherwise be
    /// two different pages.
    /// </remarks>
    internal static string WithoutRunAddress(string page, Uri address)
    {
        ArgumentNullException.ThrowIfNull(page);
        ArgumentNullException.ThrowIfNull(address);

        return page.Replace(address.GetLeftPart(UriPartial.Authority), RootPlaceholder, StringComparison.Ordinal);
    }
}

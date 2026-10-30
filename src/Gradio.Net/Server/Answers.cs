using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;

namespace Gradio.Net;
/// <summary>
/// The few answers more than one capability gives: a JSON body, a redirect, a miss, and the entries
/// FastAPI's validation document is built from.
/// </summary>
/// <remarks>
/// They are here rather than in each capability because several of them answer the same way — the
/// gate's refusal, the sign-in form's answers, the flow's redirects, the editor's refusals — and
/// three copies of "write JSON with the status and the length" is three places for one of them to
/// drift. The rest of the framework writes its own because each writes something different; these
/// write the same thing.
/// </remarks>
internal static class Answers
{
    /// <summary>Writes a JSON document.</summary>
    /// <param name="context">The request being answered.</param>
    /// <param name="status">The status to answer with.</param>
    /// <param name="document">The document.</param>
    /// <returns>A task that completes once it is written.</returns>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    internal static Task JsonAsync(HttpContext context, int status, JsonNode document)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(document);

        return JsonAsync(context, status, JsonText.Serialize(document));
    }

    /// <summary>Writes a JSON body that has already been serialized.</summary>
    /// <param name="context">The request being answered.</param>
    /// <param name="status">The status to answer with.</param>
    /// <param name="body">The body.</param>
    /// <returns>A task that completes once it is written.</returns>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    internal static Task JsonAsync(HttpContext context, int status, string body)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(body);

        var bytes = Encoding.UTF8.GetBytes(body);

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";
        context.Response.ContentLength = bytes.Length;

        return context.Response.Body.WriteAsync(bytes, context.RequestAborted).AsTask();
    }

    /// <summary>Sends the browser elsewhere.</summary>
    /// <param name="context">The request being answered.</param>
    /// <param name="location">Where to send it.</param>
    /// <exception cref="ArgumentNullException">An argument is <see langword="null"/>.</exception>
    internal static void Redirect(HttpContext context, string location)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(location);

        context.Response.StatusCode = StatusCodes.Status302Found;
        context.Response.Headers.Location = location;
    }

    /// <summary>Answers a route the capability was handed but does not claim.</summary>
    /// <param name="context">The request being answered.</param>
    /// <returns>A task that completes once the status is set.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
    internal static Task NotFoundAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.Response.StatusCode = StatusCodes.Status404NotFound;

        return Task.CompletedTask;
    }

    /// <summary>A token as the Python version draws it: URL-safe, without padding, and long enough
    /// not to be guessed.</summary> <param name="bytes">How many bytes of randomness it
    /// carries.</param> <returns>The token.</returns>
    /// <remarks>
    /// The Python version's <c>secrets.token_urlsafe</c>: 32 bytes for a state or an identifier, 16
    /// for the token a sign-in hands out.
    /// </remarks>
    internal static string UrlSafeToken(int bytes) =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(bytes))
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');

    /// <summary>One entry of FastAPI's validation detail, for a body field that was not sent.</summary>
    /// <param name="field">The field's name.</param>
    /// <returns>The entry.</returns>
    /// <remarks>
    /// A client written against the Python server reads this document, so its shape is the contract
    /// rather than a message this framework chose: <c>type</c>, <c>loc</c>, <c>msg</c> and the
    /// <c>input</c> it refused.
    /// </remarks>
    internal static JsonObject MissingBodyField(string field) => new()
    {
        ["type"] = "missing",
        ["loc"] = new JsonArray("body", field),
        ["msg"] = "Field required",
        ["input"] = null,
    };

    /// <summary>One entry of FastAPI's validation detail, for a body field sent as the wrong type.</summary>
    /// <param name="field">The field's name.</param>
    /// <param name="input">What was sent instead.</param>
    /// <returns>The entry.</returns>
    /// <remarks>
    /// Only the string case exists, because the only body models this framework reads field by field
    /// are two models whose fields are all strings.
    /// </remarks>
    internal static JsonObject WrongBodyType(string field, JsonNode? input) => new()
    {
        ["type"] = "string_type",
        ["loc"] = new JsonArray("body", field),
        ["msg"] = "Input should be a valid string",
        ["input"] = input?.DeepClone(),
    };
}

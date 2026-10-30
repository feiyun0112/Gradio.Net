using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// The form a browser-side state travels in: the value, sealed with a key only the server has.
/// </summary>
/// <remarks>
/// <para>
/// The Python version sends a <c>BrowserState</c> value as it is and leaves the protecting to the
/// browser, which encrypts what it stores in local storage with the component's secret. That
/// protects the browser's copy from being edited into something it can read, but it protects
/// nothing on the way back: a client that sends a value is believed. Worse, the secret it would
/// have to know to forge one is in the config the server handed it.
/// </para>
/// <para>
/// So the value crosses the wire sealed instead, under a key that is generated when the component
/// is built and never serialized anywhere. The frontend treats the value as opaque JSON — it stores
/// it, reads it back and sends it again without ever looking inside — while the server can tell a
/// value it sealed from one that was edited or forged, because no client can compute the key. The
/// config keeps the Python version's plain <c>default_value</c> and <c>secret</c>: the former is
/// what the browser starts from before it has anything stored, and the latter is what the browser
/// encrypts its own copy with.
/// </para>
/// <para>
/// The seal is AES-GCM rather than the browser's AES-CBC: the browser never seals a value itself,
/// so the only requirement is that a value the server sealed is the value it opens, and
/// authentication says that exactly. The tag is what refuses an edited value instead of decrypting
/// it into noise.
/// </para>
/// <para>
/// The key belongs to the process, so a value a browser stored before a restart is refused and the
/// default is used — which is what the Python version does too when it generated the secret itself,
/// because the browser could not decrypt what it stored either. A value that has to outlive the
/// process is server-side state, which is what <see cref="State"/> is for.
/// </para>
/// </remarks>
internal static class BrowserStateEnvelope
{
    /// <summary>The prefix that identifies the seal and its version.</summary>
    private const string Prefix = "v1.";

    /// <summary>How many random bytes a key holds, which is what AES-256 takes.</summary>
    internal const int KeySize = 32;

    private const int NonceSize = 12;
    private const int TagSize = 16;

    /// <summary>Draws a key.</summary>
    /// <returns>A key no client can compute.</returns>
    internal static byte[] NewKey() => RandomNumberGenerator.GetBytes(KeySize);

    /// <summary>Seals a value for the client.</summary>
    /// <param name="key">The component's seal key.</param>
    /// <param name="value">The value to seal, or <see langword="null"/>.</param>
    /// <returns>The sealed form, as the string the client stores.</returns>
    /// <exception cref="ArgumentException"><paramref name="key"/> is not <see cref="KeySize"/> bytes.</exception>
    internal static string Seal(byte[] key, JsonNode? value)
    {
        var plaintext = Encoding.UTF8.GetBytes(value?.ToJsonString(JsonText.Options) ?? "null");
        var payload = new byte[NonceSize + TagSize + plaintext.Length];

        RandomNumberGenerator.Fill(payload.AsSpan(0, NonceSize));

        using var cipher = new AesGcm(Validated(key), TagSize);
        cipher.Encrypt(
            payload.AsSpan(0, NonceSize),
            plaintext,
            payload.AsSpan(NonceSize + TagSize),
            payload.AsSpan(NonceSize, TagSize));

        return Prefix + Base64Url(payload);
    }

    /// <summary>Opens a value the server sealed.</summary>
    /// <param name="key">The component's seal key.</param>
    /// <param name="value">What the client sent.</param>
    /// <param name="opened">The value inside, when it opens.</param>
    /// <returns>Whether the value was one this server sealed with this key.</returns>
    /// <exception cref="ArgumentException"><paramref name="key"/> is not <see cref="KeySize"/> bytes.</exception>
    /// <remarks>
    /// Anything that is not a sealed value — a plain JSON value, a string that does not decode, a
    /// sealed value with a different key or an edited byte — is reported as not opened rather than
    /// thrown, because "the client sent something the server did not issue" is a value to refuse
    /// rather than a failure to report.
    /// </remarks>
    internal static bool TryOpen(byte[] key, JsonNode? value, out JsonNode? opened)
    {
        opened = null;

        if (JsonValues.Text(value) is not { } text || !text.StartsWith(Prefix, StringComparison.Ordinal))
        {
            return false;
        }

        byte[] payload;
        try
        {
            payload = FromBase64Url(text[Prefix.Length..]);
        }
        catch (FormatException)
        {
            return false;
        }

        if (payload.Length < NonceSize + TagSize)
        {
            return false;
        }

        var plaintext = new byte[payload.Length - NonceSize - TagSize];

        try
        {
            using var cipher = new AesGcm(Validated(key), TagSize);
            cipher.Decrypt(
                payload.AsSpan(0, NonceSize),
                payload.AsSpan(NonceSize + TagSize),
                payload.AsSpan(NonceSize, TagSize),
                plaintext);
        }
        catch (CryptographicException)
        {
            return false;
        }

        try
        {
            opened = JsonNode.Parse(plaintext);

            return true;
        }
        catch (System.Text.Json.JsonException)
        {
            return false;
        }
    }

    /// <summary>The key, checked to be the size the cipher takes.</summary>
    /// <param name="key">The component's seal key.</param>
    /// <returns>The key.</returns>
    /// <exception cref="ArgumentException">The key is not <see cref="KeySize"/> bytes.</exception>
    private static byte[] Validated(byte[] key) =>
        key.Length == KeySize
            ? key
            : throw new ArgumentException($"The seal key must be {KeySize} bytes.", nameof(key));

    /// <summary>Base64 in the URL-safe alphabet, without padding.</summary>
    private static string Base64Url(byte[] bytes) =>
        Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    /// <summary>The bytes a URL-safe base64 string carries.</summary>
    private static byte[] FromBase64Url(string text)
    {
        var padded = text.Replace('-', '+').Replace('_', '/');

        return Convert.FromBase64String(padded.PadRight(padded.Length + ((4 - (padded.Length % 4)) % 4), '='));
    }
}

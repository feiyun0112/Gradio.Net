using System.Security.Cryptography;
using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A value the browser keeps in its own local storage, so that it survives a reload and a new
/// session.
/// </summary>
/// <remarks>
/// <para>
/// This is the counterpart of <see cref="State"/>: the value belongs to the browser rather than to
/// the server, which is what makes it outlive the session. Two keys are involved and they are not
/// the same one: the browser encrypts what it stores with <see cref="Secret"/>, which is in the
/// config because the browser needs it, and the server seals what it hands over with a key of its
/// own that no client ever sees — so a value that was edited, or forged by a client that read the
/// config, is refused (see <see cref="BrowserStateEnvelope"/>).
/// </para>
/// <para>
/// The component draws nothing either: it is a value a page remembers, not a field.
/// </para>
/// </remarks>
[GradioBlock("browserstate")]
[HandlerValue("object?", "change")]
public sealed partial class BrowserState : Component
{
    internal BrowserState()
    {
    }

    private const int GeneratedLength = 16;

    /// <summary>Letters and digits, which is the alphabet the Python version draws a key
    /// from.</summary>
    private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    private readonly object _gate = new();

    /// <summary>The key the value is sealed with, which is this component's and nobody else's.</summary>
    /// <remarks>
    /// Not a prop and never serialized: a client that could read it could seal a value of its own
    /// and the server would believe it. Drawn when the component is built, so it lives as long as
    /// the application does.
    /// </remarks>
    private readonly byte[] _sealKey = BrowserStateEnvelope.NewKey();

    private JsonNode? _defaultValue;
    private string? _storageKey;
    private string? _secret;

    /// <summary>The value a browser starts from, before it has stored anything.</summary>
    /// <value>Any JSON value, or <see langword="null"/> for a browser that starts with nothing.</value>
    /// <remarks>
    /// This is the one value that crosses the wire unsealed, because it is what the browser falls
    /// back to when it has nothing stored: the server declares it, so a client that sends it back is
    /// sending the server's own value.
    /// </remarks>
    public JsonNode? DefaultValue
    {
        get => _defaultValue;
        set => _defaultValue = value?.DeepClone();
    }

    /// <summary>The key the value is stored under in the browser's local storage.</summary>
    /// <value>
    /// The key the application gave, or a random one drawn when it is first read.
    /// </value>
    /// <remarks>
    /// Drawn rather than required because two applications served from one origin would otherwise
    /// share a key, and each would read the other's value. Drawn once and remembered, so that a
    /// second page load finds what the first one stored.
    /// </remarks>
    public string StorageKey
    {
        get => Generated(ref _storageKey);
        set
        {
            lock (_gate)
            {
                _storageKey = value;
            }
        }
    }

    /// <summary>The secret the browser encrypts its own copy with.</summary>
    /// <value>The secret the application gave, or a random one drawn when it is first read.</value>
    /// <remarks>
    /// <para>
    /// This is the Python version's parameter and it is in the config because the browser is the
    /// one that uses it: what it protects is the copy in local storage. It is <em>not</em> what the
    /// server's seal is keyed by — a client knows this value, so a seal keyed by it could be
    /// forged.
    /// </para>
    /// <para>
    /// A random secret means the browser cannot read what it stored after a restart, which is the
    /// Python version's behaviour too. An application that wants the browser's copy to outlive the
    /// process passes a secret of its own.
    /// </para>
    /// </remarks>
    public string Secret
    {
        get => Generated(ref _secret);
        set
        {
            lock (_gate)
            {
                _secret = value;
            }
        }
    }

    /// <summary>
    /// Whether this block ends a run of siblings that would otherwise be grouped into one container.
    /// </summary>
    /// <value><see langword="false"/>, which is the Python version's rule for a browser-side
    /// state.</value>
    protected internal override bool BreaksGrouping => false;

    /// <summary>The typing information of a browser-side state.</summary>
    /// <value>Any JSON-serializable value, which is what the Python version declares.</value>
    public override JsonObject ApiInfo => new()
    {
        ["type"] = new JsonObject(),
        ["description"] = "any json-serializable value",
    };

    /// <inheritdoc />
    public override JsonNode? ExampleInputs => JsonValue.Create("test");

    /// <summary>The value a run reads.</summary>
    /// <param name="value">What the client sent.</param>
    /// <returns>The value the browser stored, or the default when there is none or it was edited.</returns>
    /// <remarks>
    /// A value the server did not seal is refused and the default is used instead, which is what the
    /// browser does with a stored value it cannot decrypt: a state that was tampered with is a state
    /// that was lost, rather than one that is believed.
    /// </remarks>
    public override object? Preprocess(JsonNode? value)
    {
        if (value is null)
        {
            return Default();
        }

        if (BrowserStateEnvelope.TryOpen(_sealKey, value, out var opened))
        {
            return JsonValues.ToObject(opened);
        }

        return Default();
    }

    /// <summary>The value a run writes, sealed for the browser to keep.</summary>
    /// <param name="value">What the handler returned.</param>
    /// <returns>The sealed value.</returns>
    public override JsonNode? Postprocess(object? value) =>
        JsonValue.Create(BrowserStateEnvelope.Seal(_sealKey, JsonValues.FromAnyObject(value)));

    /// <summary>The default value, as a handler reads it.</summary>
    private object? Default() => JsonValues.ToObject(_defaultValue?.DeepClone());

    /// <summary>A generated key, drawn once and remembered.</summary>
    /// <param name="field">The field holding it.</param>
    /// <returns>The value the application gave, or the one drawn on first use.</returns>
    /// <remarks>
    /// Under a lock because the config is built per request: two requests arriving together must not
    /// draw two secrets, because the value one of them hands out would be sealed with a key the
    /// other does not know.
    /// </remarks>
    private string Generated(ref string? field)
    {
        lock (_gate)
        {
            return field is { Length: > 0 } ? field : field = Draw();
        }
    }

    /// <summary>Draws a key of <see cref="GeneratedLength"/> letters and digits.</summary>
    /// <returns>The key.</returns>
    /// <remarks>
    /// From the cryptographic generator rather than the shared one: a secret that could be guessed
    /// is not a secret, and the alphabet is the Python version's so that a key looks like one of
    /// its own.
    /// </remarks>
    private static string Draw()
    {
        var characters = new char[GeneratedLength];

        for (var index = 0; index < characters.Length; index++)
        {
            characters[index] = Alphabet[RandomNumberGenerator.GetInt32(Alphabet.Length)];
        }

        return new string(characters);
    }
}

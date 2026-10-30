using System.Text.Json;
using System.Text.Json.Nodes;

namespace Gradio.Net;
/// <summary>
/// A string that asks the browser to translate itself rather than to be shown as it is.
/// </summary>
/// <remarks>
/// <para>
/// The Python version writes such a string as <c>__i18n__</c> followed by the translation metadata,
/// which the frontend recognises and replaces with the reader's own language. The spelling is on
/// the wire rather than in a type because the value sits in props that are otherwise plain strings
/// — a label, a placeholder, a button's text — and a second implementation that carried an object
/// there would describe a different application.
/// </para>
/// <para>
/// Only the defaults this framework itself writes are translated: a chat's own label, the message
/// field's hint, the new-conversation button and the fallback title of a saved conversation. The
/// Python version also ships a translation catalogue and lets an application supply its own, which
/// is <see cref="I18n"/>; what is here is the piece an assembly that renders a chat needs, and it
/// produces the same bytes the Python version's <c>I18nData</c> produces.
/// </para>
/// </remarks>
internal static class TranslationKey
{
    /// <summary>What marks a string as a translation rather than as text.</summary>
    private const string Prefix = "__i18n__";

    /// <summary>The metadata's own type name, as the Python version spells it.</summary>
    private const string MetadataType = "translation_metadata";

    /// <summary>The label of a chat's conversation.</summary>
    internal const string Chatbot = "chat_interface.chatbot";

    /// <summary>The text of the button that starts a new conversation.</summary>
    internal const string NewChat = "chat_interface.new_chat";

    /// <summary>The hint shown in an empty message field.</summary>
    internal const string MessagePlaceholder = "chat_interface.message_placeholder";

    /// <summary>The title of an accordion holding the additional inputs.</summary>
    internal const string AdditionalInputs = "chat_interface.additional_inputs";

    /// <summary>The title a saved conversation is listed under when it has no first message.</summary>
    internal const string Conversation = "chat_interface.conversation";

    /// <summary>The string the browser translates, for one key.</summary>
    /// <param name="key">The translation key.</param>
    /// <returns>The string, in the spelling the frontend recognises.</returns>
    /// <remarks>
    /// <para>
    /// The metadata is written by <see cref="PythonJson"/> rather than by hand, because the
    /// spelling is part of the value: Python's <c>json.dumps</c> puts a space after every colon and
    /// comma, and a string that differed in its spacing would be a string the frontend does not
    /// recognise. The same writer escapes the key the way Python escapes one, which matters because
    /// a key is application data — a non-ASCII key is served as <c>\uXXXX</c>, and one holding a
    /// newline would otherwise be served as JSON the frontend cannot parse.
    /// </para>
    /// </remarks>
    internal static string Of(string key) =>
        Prefix + PythonJson.Dump(new JsonObject
        {
            ["__type__"] = MetadataType,
            ["key"] = key,
        });

    /// <summary>Whether a string is a translation, and which key it names.</summary>
    /// <param name="value">The string.</param>
    /// <param name="key">The key, when it is one.</param>
    /// <returns>Whether <paramref name="value"/> is a translation.</returns>
    internal static bool TryParse(string? value, out string key)
    {
        key = string.Empty;

        if (value is null || !value.StartsWith(Prefix, StringComparison.Ordinal))
        {
            return false;
        }

        var metadata = value[Prefix.Length..];

        // The metadata is read with a JSON parser rather than with a substring search, because a
        // key is application data: one holding a quote or a backslash is escaped in the string and
        // has to come back out as it went in.
        try
        {
            if (JsonNode.Parse(metadata) is JsonObject parsed
                && JsonValues.Text(parsed["key"]) is { } parsedKey)
            {
                key = parsedKey;

                return true;
            }
        }
        catch (JsonException)
        {
            return false;
        }

        return false;
    }
}
